# BinarySerializer.SourceGeneration

An optional Roslyn source generator for [BinarySerializer](https://www.nuget.org/packages/BinarySerializer). It removes the manual rooting step that trimmed and Native AOT applications otherwise need.

BinarySerializer discovers object graphs through reflection at runtime, so the trimmer cannot see which types and members a trimmed or AOT-compiled application serializes. This generator finds those types at compile time and emits code that keeps them.

## Installation

Add the package to the project that defines your serialized types, alongside BinarySerializer itself:

```
dotnet add package BinarySerializer
dotnet add package BinarySerializer.SourceGeneration
```

The generator runs only at build time. It adds no runtime dependency, and nothing else in your code needs to change.

## What it does

The generator collects every type that carries BinarySerializer attributes, everything reachable from their members, subtype, converter and factory types, and closed generic collections such as `List<MyItem>`. For those types it emits a module initializer that:

* roots the types for trimming and AOT compilation, so no `TrimmerRoots.xml` or `[DynamicDependency]` is needed for them;
* registers compiled member accessors and parameterless constructor factories, so building the type graph needs neither reflection nor dynamic code generation.

Serialized bytes are identical with or without the generator.

## Things to know

* **Types without BinarySerializer attributes.** A root type that has no BinarySerializer attributes on it or its members is not discovered automatically. Mark it with `[BinarySerializerRoot]`.
* **Types the generated code cannot reference.** Private or protected nested types and C# `file` types are skipped. They still serialize through the reflection path, but the generator does not root them, so root them manually in a trimmed or AOT-compiled application.
* **Version requirement.** The generator needs BinarySerializer 10.2.0 or later. With an older BinarySerializer it generates nothing and reports warning `BSG001`.

## Documentation

See the [AOT and Trimming section of the BinarySerializer README](https://github.com/jefffhaynes/BinarySerializer#aot-and-trimming) for the full guide, including manual rooting without the generator.
