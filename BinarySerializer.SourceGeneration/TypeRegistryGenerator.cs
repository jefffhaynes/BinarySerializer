using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BinarySerializer.SourceGeneration
{
    /// <summary>
    ///     Roslyn incremental generator that emits static BinarySerializer registration metadata.
    ///     For every reachable serializable type it generates a module initializer that:
    ///     <list type="bullet">
    ///         <item>roots the type and its transitive object graph for trimming and Native AOT
    ///         (via the DynamicallyAccessedMembers annotation on GeneratedTypeRegistry.Register/Keep),
    ///         including closed generic instantiations such as List&lt;Item&gt;;</item>
    ///         <item>registers statically compiled member accessor delegates so type graph construction
    ///         needs no reflection and no dynamic code generation;</item>
    ///         <item>registers parameterless constructor factories where available.</item>
    ///     </list>
    ///     Types without generated registration fall back to the reflection discovery path, so
    ///     behavior is identical with or without this package.
    /// </summary>
    [Generator]
    public sealed class TypeRegistryGenerator : IIncrementalGenerator
    {
        private const string BinarySerializerNamespace = "BinarySerialization";
        private const string MarkerAttributeName = "BinarySerializerRootAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(context.CompilationProvider,
                static (productionContext, compilation) => Execute(productionContext, compilation));
        }

        private static void Execute(SourceProductionContext context, Compilation compilation)
        {
            var collector = new TypeCollector(compilation);
            collector.Collect();

            if (collector.Registrations.Count == 0 && collector.Keeps.Count == 0)
            {
                return;
            }

            var source = Emitter.Emit(collector.Registrations.Values.ToList(), collector.Keeps.Values.ToList(),
                collector.AttributeKeeps.Values.ToList());
            context.AddSource("BinarySerializer.GeneratedRegistry.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        /// <summary>
        ///     True for attributes declared in the BinarySerialization namespace or deriving from a
        ///     BinarySerialization attribute (for example user-defined binding attributes).
        /// </summary>
        internal static bool IsBinarySerializationAttribute(ITypeSymbol attributeType)
        {
            for (var type = attributeType; type != null; type = type.BaseType)
            {
                if (type.ContainingNamespace?.ToDisplayString() == BinarySerializerNamespace)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsMarkerAttribute(ITypeSymbol attributeType)
        {
            return attributeType.Name == MarkerAttributeName &&
                   attributeType.ContainingNamespace?.ToDisplayString() == BinarySerializerNamespace;
        }
    }

    /// <summary>
    ///     Registration information for a single type.
    /// </summary>
    internal sealed class RegistrationModel
    {
        public RegistrationModel(INamedTypeSymbol type)
        {
            Type = type;
        }

        public INamedTypeSymbol Type { get; }

        public bool HasParameterlessConstructor { get; set; }

        public List<MemberModel> Members { get; } = new List<MemberModel>();
    }

    /// <summary>
    ///     Accessor information for a single member.
    /// </summary>
    internal sealed class MemberModel
    {
        public MemberModel(string name, ITypeSymbol type, bool isField, bool hasGetter, bool hasSetter)
        {
            Name = name;
            Type = type;
            IsField = isField;
            HasGetter = hasGetter;
            HasSetter = hasSetter;
        }

        public string Name { get; }

        public ITypeSymbol Type { get; }

        /// <summary>
        ///     True for fields (which assign null values unconditionally) and false for properties
        ///     (whose setters skip null values, mirroring MagicMethods semantics).
        /// </summary>
        public bool IsField { get; }

        public bool HasGetter { get; }

        public bool HasSetter { get; }
    }

    /// <summary>
    ///     Walks the compilation and collects the serializable type closure reachable from seeded
    ///     types (types carrying BinarySerializer attributes or a BinarySerializerRoot attribute).
    /// </summary>
    internal sealed class TypeCollector
    {
        private readonly Compilation _compilation;
        private readonly HashSet<INamedTypeSymbol> _visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        private readonly HashSet<ITypeSymbol> _keepSet = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        private readonly HashSet<ITypeSymbol> _attributeKeepSet = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        private readonly HashSet<INamedTypeSymbol> _registeredSet = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        private readonly Queue<ITypeSymbol> _queue = new Queue<ITypeSymbol>();

        public TypeCollector(Compilation compilation)
        {
            _compilation = compilation;
        }

        public SortedDictionary<string, RegistrationModel> Registrations { get; } =
            new SortedDictionary<string, RegistrationModel>(StringComparer.Ordinal);

        public SortedDictionary<string, ITypeSymbol> Keeps { get; } =
            new SortedDictionary<string, ITypeSymbol>(StringComparer.Ordinal);

        public SortedDictionary<string, ITypeSymbol> AttributeKeeps { get; } =
            new SortedDictionary<string, ITypeSymbol>(StringComparer.Ordinal);

        public void Collect()
        {
            foreach (var type in EnumerateSourceTypes())
            {
                if (IsSeed(type))
                {
                    Enqueue(type);
                }
            }

            while (_queue.Count > 0)
            {
                Process(_queue.Dequeue());
            }

            // finalize keep list: everything visited but not registered
            foreach (var symbol in _keepSet)
            {
                // primitives, string and object are never trimmed and need no rooting
                if (symbol.SpecialType != SpecialType.None)
                {
                    continue;
                }

                if (symbol is INamedTypeSymbol named && _registeredSet.Contains(named))
                {
                    continue;
                }

                var display = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (!Keeps.ContainsKey(display) && !Registrations.ContainsKey(display))
                {
                    Keeps[display] = symbol;
                }
            }

            foreach (var symbol in _attributeKeepSet)
            {
                if (symbol.SpecialType != SpecialType.None)
                {
                    continue;
                }

                var display = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (!AttributeKeeps.ContainsKey(display) && !Registrations.ContainsKey(display))
                {
                    AttributeKeeps[display] = symbol;
                }
            }
        }

        private IEnumerable<INamedTypeSymbol> EnumerateSourceTypes()
        {
            foreach (var type in EnumerateNamespace(_compilation.Assembly.GlobalNamespace))
            {
                yield return type;
            }
        }

        private static IEnumerable<INamedTypeSymbol> EnumerateNamespace(INamespaceSymbol namespaceSymbol)
        {
            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                foreach (var nested in EnumerateType(type))
                {
                    yield return nested;
                }
            }

            foreach (var child in namespaceSymbol.GetNamespaceMembers())
            {
                foreach (var type in EnumerateNamespace(child))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> EnumerateType(INamedTypeSymbol type)
        {
            yield return type;

            foreach (var nested in type.GetTypeMembers())
            {
                foreach (var child in EnumerateType(nested))
                {
                    yield return child;
                }
            }
        }

        private static bool IsSeed(INamedTypeSymbol type)
        {
            if (type.IsImplicitlyDeclared || type.TypeKind != TypeKind.Class)
            {
                return false;
            }

            foreach (var attribute in type.GetAttributes())
            {
                if (attribute.AttributeClass != null &&
                    (TypeRegistryGenerator.IsBinarySerializationAttribute(attribute.AttributeClass) ||
                     attribute.AttributeClass.Name == "BinarySerializerRootAttribute"))
                {
                    return true;
                }
            }

            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol || member is IFieldSymbol)
                {
                    foreach (var attribute in member.GetAttributes())
                    {
                        if (attribute.AttributeClass != null &&
                            TypeRegistryGenerator.IsBinarySerializationAttribute(attribute.AttributeClass))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void Enqueue(ITypeSymbol type)
        {
            _queue.Enqueue(type);
        }

        private void Process(ITypeSymbol symbol)
        {
            switch (symbol)
            {
                case IArrayTypeSymbol arrayType:
                    // Array types need no member rooting (construction goes through
                    // TypeExtensions.CreateArray); rooting them would drag Array.CreateInstance
                    // into the preserved surface and trigger IL3050 warnings.
                    EnqueueIfUnvisited(arrayType.ElementType);
                    return;

                case ITypeParameterSymbol:
                    return;

                case INamedTypeSymbol named:
                    ProcessNamed(named);
                    return;
            }
        }

        private void EnqueueIfUnvisited(ITypeSymbol symbol)
        {
            switch (symbol)
            {
                case IArrayTypeSymbol arrayType:
                    EnqueueIfUnvisited(arrayType.ElementType);
                    break;

                case ITypeParameterSymbol:
                    break;

                case INamedTypeSymbol named when IsConcrete(named):
                    Enqueue(named);
                    break;
            }
        }

        private void ProcessNamed(INamedTypeSymbol type)
        {
            if (!IsConcrete(type) || !_visited.Add(type))
            {
                return;
            }

            if (IsRegistrable(type))
            {
                Registrations[type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = BuildRegistration(type);
                _registeredSet.Add(type);
            }
            else
            {
                Keep(type);
            }

            // type arguments of closed generics (e.g. List<Item> -> Item)
            foreach (var argument in type.TypeArguments)
            {
                EnqueueIfUnvisited(argument);
            }

            // attributes on the type and its serializable members
            foreach (var attribute in type.GetAttributes())
            {
                ProcessAttribute(attribute);
            }

            // serializable members (mirrors reflection discovery: public instance properties and fields)
            foreach (var member in type.GetMembers())
            {
                if (member.IsStatic)
                {
                    continue;
                }

                foreach (var attribute in member.GetAttributes())
                {
                    ProcessAttribute(attribute);
                }

                switch (member)
                {
                    case IPropertySymbol property when property.DeclaredAccessibility == Accessibility.Public:
                        EnqueueIfUnvisited(property.Type);
                        break;

                    case IFieldSymbol field when field.DeclaredAccessibility == Accessibility.Public:
                        EnqueueIfUnvisited(field.Type);
                        break;
                }
            }

            // base type members are part of the serialized graph as well
            if (type.BaseType != null && type.BaseType.SpecialType != SpecialType.System_Object)
            {
                EnqueueIfUnvisited(type.BaseType);
            }
        }

        private void ProcessAttribute(AttributeData attribute)
        {
            if (attribute.AttributeClass != null)
            {
                _attributeKeepSet.Add(attribute.AttributeClass);
            }

            foreach (var argument in attribute.ConstructorArguments)
            {
                ProcessTypedConstant(argument);
            }

            foreach (var named in attribute.NamedArguments)
            {
                ProcessTypedConstant(named.Value);
            }
        }

        private void ProcessTypedConstant(TypedConstant constant)
        {
            switch (constant.Kind)
            {
                case TypedConstantKind.Type when constant.Value is ITypeSymbol type:
                    EnqueueIfUnvisited(type);
                    break;

                case TypedConstantKind.Array:
                    foreach (var element in constant.Values)
                    {
                        ProcessTypedConstant(element);
                    }

                    break;
            }
        }

        private void Keep(ITypeSymbol type)
        {
            _keepSet.Add(type);
        }

        private static bool IsConcrete(INamedTypeSymbol type)
        {
            if (type.IsUnboundGenericType || type.IsGenericType && type.TypeArguments.Any(argument => argument is ITypeParameterSymbol))
            {
                return false;
            }

            return type.TypeKind == TypeKind.Class ||
                   type.TypeKind == TypeKind.Struct ||
                   type.TypeKind == TypeKind.Enum ||
                   type.TypeKind == TypeKind.Interface ||
                   type.TypeKind == TypeKind.Delegate;
        }

        private bool IsRegistrable(INamedTypeSymbol type)
        {
            if (type.TypeKind != TypeKind.Class ||
                type.IsStatic ||
                type.IsGenericType ||
                !SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, _compilation.Assembly))
            {
                return false;
            }

            // nested types inside generic types cannot be expressed as closed registrations
            for (var containing = type.ContainingType; containing != null; containing = containing.ContainingType)
            {
                if (containing.IsGenericType)
                {
                    return false;
                }
            }

            return true;
        }

        private RegistrationModel BuildRegistration(INamedTypeSymbol type)
        {
            var model = new RegistrationModel(type);

            var hasRequiredMembers = type.GetMembers().Any(member =>
                (member is IPropertySymbol property && property.IsRequired) ||
                (member is IFieldSymbol field && field.IsRequired));

            model.HasParameterlessConstructor =
                !type.IsAbstract &&
                !hasRequiredMembers &&
                type.InstanceConstructors.Any(constructor =>
                    constructor.Parameters.Length == 0 &&
                    constructor.DeclaredAccessibility == Accessibility.Public);

            foreach (var member in type.GetMembers())
            {
                if (member.IsStatic)
                {
                    continue;
                }

                switch (member)
                {
                    case IPropertySymbol property:
                        AddProperty(model, property);
                        break;

                    case IFieldSymbol field:
                        AddField(model, field);
                        break;
                }
            }

            return model;
        }

        private static void AddProperty(RegistrationModel model, IPropertySymbol property)
        {
            if (property.DeclaredAccessibility != Accessibility.Public || property.IsIndexer)
            {
                return;
            }

            var hasPublicGetter = property.GetMethod != null &&
                                  property.GetMethod.DeclaredAccessibility == Accessibility.Public;
            var hasPublicSetter = property.SetMethod != null &&
                                  property.SetMethod.DeclaredAccessibility == Accessibility.Public;
            var isInitOnly = property.SetMethod != null && property.SetMethod.IsInitOnly;

            // parity with the reflection path: write-only members and init-only setters fall back
            // to the reflection accessors, so keep registration out of the way for those.
            if (!hasPublicGetter || isInitOnly)
            {
                return;
            }

            model.Members.Add(new MemberModel(property.Name, property.Type, false, true, hasPublicSetter));
        }

        private static void AddField(RegistrationModel model, IFieldSymbol field)
        {
            if (field.DeclaredAccessibility != Accessibility.Public || field.IsReadOnly || field.IsConst)
            {
                return;
            }

            model.Members.Add(new MemberModel(field.Name, field.Type, true, true, true));
        }
    }

    /// <summary>
    ///     Emits the registration source.
    /// </summary>
    internal static class Emitter
    {
        private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat;

        public static string Emit(IReadOnlyList<RegistrationModel> registrations, IReadOnlyList<ITypeSymbol> keeps,
            IReadOnlyList<ITypeSymbol> attributeKeeps)
        {
            var builder = new StringBuilder();
            builder.AppendLine("// <auto-generated />");
            builder.AppendLine("// Generated by BinarySerializer.SourceGeneration.  Do not edit.");
            builder.AppendLine("#nullable disable");
            builder.AppendLine("#pragma warning disable");
            builder.AppendLine();
            builder.AppendLine("namespace BinarySerialization.Generated");
            builder.AppendLine("{");
            builder.AppendLine("    internal static class BinarySerializerGeneratedRegistry");
            builder.AppendLine("    {");
            builder.AppendLine("        [global::System.Runtime.CompilerServices.ModuleInitializer]");
            builder.AppendLine("        [global::System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(\"Trimming\", \"IL2110\",");
            builder.AppendLine("            Justification = \"Attribute metadata is rooted for GetCustomAttributes; annotated attribute members receive their required types via explicit typeof rooting emitted alongside.\")]");
            builder.AppendLine("        [global::System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(\"Trimming\", \"IL2111\",");
            builder.AppendLine("            Justification = \"Attribute metadata is rooted for GetCustomAttributes; annotated attribute members receive their required types via explicit typeof rooting emitted alongside.\")]");
            builder.AppendLine("        internal static void Initialize()");
            builder.AppendLine("        {");

            foreach (var keep in attributeKeeps)
            {
                builder.Append("            global::BinarySerialization.GeneratedTypeRegistry.KeepAttribute(typeof(")
                    .Append(keep.ToDisplayString(TypeFormat))
                    .AppendLine("));");
            }

            foreach (var keep in keeps)
            {
                builder.Append("            global::BinarySerialization.GeneratedTypeRegistry.Keep(typeof(")
                    .Append(keep.ToDisplayString(TypeFormat))
                    .AppendLine("));");
            }

            foreach (var registration in registrations)
            {
                builder.AppendLine();
                EmitRegistration(builder, registration);
            }

            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private static void EmitRegistration(StringBuilder builder, RegistrationModel registration)
        {
            var typeDisplay = registration.Type.ToDisplayString(TypeFormat);

            builder.Append("            global::BinarySerialization.GeneratedTypeRegistry.Register(typeof(")
                .Append(typeDisplay)
                .Append("), ");

            if (registration.HasParameterlessConstructor)
            {
                builder.Append("static () => new ").Append(typeDisplay).Append("()");
            }
            else
            {
                builder.Append("null");
            }

            if (registration.Members.Count == 0)
            {
                builder.AppendLine(");");
                return;
            }

            builder.AppendLine(",");

            for (var i = 0; i < registration.Members.Count; i++)
            {
                var member = registration.Members[i];
                builder.Append("                new global::BinarySerialization.GeneratedMemberAccessors(")
                    .Append(Literal(member.Name))
                    .Append(", ");

                // getter
                builder.Append("static o => ((").Append(typeDisplay).Append(")o).@").Append(member.Name);

                builder.Append(", ");

                // setter
                if (!member.HasSetter)
                {
                    builder.Append("null");
                }
                else if (member.IsField)
                {
                    // fields assign unconditionally (mirrors FieldInfo.SetValue semantics)
                    builder.Append("static (o, v) => { ((").Append(typeDisplay).Append(")o).@")
                        .Append(member.Name).Append(" = (").Append(member.Type.ToDisplayString(TypeFormat))
                        .Append(")v; }");
                }
                else
                {
                    // property setters ignore null values (mirrors MagicMethods semantics)
                    builder.Append("static (o, v) => { if (v != null) ((").Append(typeDisplay).Append(")o).@")
                        .Append(member.Name).Append(" = (").Append(member.Type.ToDisplayString(TypeFormat))
                        .Append(")v; }");
                }

                builder.AppendLine(i == registration.Members.Count - 1 ? "));" : "),");
            }
        }

        private static string Literal(string value)
        {
            var builder = new StringBuilder("\"");

            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(ch);
                        break;
                }
            }

            builder.Append('"');
            return builder.ToString();
        }
    }
}
