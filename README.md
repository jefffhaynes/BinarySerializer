BinarySerializer
================

A .NET declarative serialization framework for controlling formatting of data at the byte level.  BinarySerializer is designed to make interop with binary formats and protocols fast and simple.

## Field Ordering ##

There is no completely reliable way to get POCO member ordering from the CLR, so as of 3.0 FieldOrder attributes are required on all classes with more than one field or property.  By convention, base classes are serialized first, followed by any derived classes.  For example, the following class will serialize in the order A B C.

    public class MyBaseClass
    {
        public int A { get; set; }
    }

    public class MyDerivedClass : MyBaseClass
    {
        [FieldOrder(0)]
        public int B { get; set; }

        [FieldOrder(1)]
        public int C;
    }

    ...

    var stream = new MemoryStream();
    var serializer = new BinarySerializer();

    serializer.Serialize(stream, myDerivedClass);

    ...

Note that we're using properties and fields interchangeably.  Both are treated as the same by the serializer.

## Binding ##
The most powerful feature of BinarySerializer is the ability to bind attributes of fields to other fields in the object graph.  Using the various attributes, this approach can allow for interop with complex formats and protocols.  One of the simplest examples of this is field length binding.

    public class MyBoundClass
    {
        [FieldOrder(0)]
        public byte NameLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("NameLength")]
        public string Name { get; set; }
    }

!(/BinarySerializer.Docs/SimpleBinding_2.png)

It is not necessary that <code>NameLength</code> contains the length of <code>Name</code> as that value will be computed during serialization and updated in the serialized graph.  During deserialization the <code>NameLength</code> value will be used to correctly deserialize the <code>Name</code> field.  See below for a summary all possible bindings and attributes.

## Default Behavior ##
Although most behavior can be overridden, in many cases the serializer will attempt to guess the intended behavior based on class design.  For example, in the following class a null-terminated string will be used during serialization as deserialization would be impossible without more information.

    public class MyUnboundClass
    {
        [FieldOrder(0)]
        public string Name { get; set; }

        [FieldOrder(1)]
        public int Age { get; set; }
    }


## Attributes ##

There are a number of attributes that can be used to control how your fields are serialized.  Following is a summary with examples:

### Ignore ###

Any field or property with an Ignore attribute will not be included in serialization or deserialization.  However, these fields can still be used in bindings.  This can be used to create "dummy" properties that perform some conversion or computation; however, better practice for this is to define a value converter (see below).

### FieldOrder ###

Again, this attribute is required on any field/property in a class with more than one field or property.  Only relative value matters.  Base values are serialized before dervied values.

    public class MyBaseClass
    {
        public int A { get; set; }
    }

    public class MyDerivedClass : MyBaseClass
    {
        [FieldOrder(0)]
        public int B { get; set; }

        [FieldOrder(1)]
        public int C;
    }

### FieldLength ###
The most basic attribute, this can be used to either specify bound or constant field length.  Field lengths can apply to anything that is sizeable; strings, arrays, lists, streams, and even complex objects.

    public class MyConstFieldClass
    {
        [FieldLength(32)]
        public string Name { get; set; }
    }

Alternatively, this same field could be bound to a different field:

    public class MyBoundFieldClass
    {
        [FieldOrder(0)]
        public int NameLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("NameLength")]
        public string Name { get; set; }
    }

In some cases you may want to limit a collection of items by the total serialized length:

    public class MyBoundCollectionClass
    {
        [FieldOrder(0)]
        public int NameLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("NameLength")]
        public List<string> Names { get; set; }
    }

More generically some formats and protocols will define a set of fields of specified size, with "optional" trailing fields:

    public class Person
    {
        [FieldOrder(0)]
        [FieldLength(32)]
        public string Name { get; set; }

        [FieldOrder(1)]
        public int Age { get; set; }
        
        [FieldOrder(2)]
        public string OptionalDescription { get; set; }
    }

    public class PersonEntry
    {
        [FieldOrder(0)]
        public int EntryLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("EntryLength")]
        public Person Person { get; set; }
     }

This is a pretty nasty way to define a specification, but again our goal here to maximize interoperability with existing formats and protocols and we may have no say in the matter.

### FieldCount ###

The FieldCount attribute is used to define how many items are contained in a collection.  In practice, this is either an array or a list with a single generic argument.

    public class Directory
    {
        [FieldOrder(0)]
        public int EntryCount { get; set; }

        [FieldOrder(1)]
        [FieldCount("EntryCount")]
        public List<Entry> Entries { get; set; }
    }

Note the special case of a byte array, for which length and count attributes are interchangeable.

### ItemLength ###

This attribute can be used to control the length of items in a collection.

    public class Manifest
    {
        [FieldOrder(0)]
        public int EntryLength { get; set; }

        [FieldOrder(1)]
        [ItemLength("EntryLength")]
        public List<string> Entries { get; set; }
    }


### FieldOffset ###

The FieldOffset attribute should be used sparingly but can be used if an absolute offset is required.  In most cases, implicit offset (e.g. just define the structure) is preferable.  For an example application, see the ISO9660 example.

### Subtype ###

The Subtype attribute allows dynamic switching to subtypes based on a binding.

    public class Packet
    {
        [FieldOrder(0)]
        public FrameType FrameType { get; set; }

        [FieldOrder(1)]
        [Subtype("FrameType", FrameType.Message, typeof(MessageFrame)]
        [Subtype("FrameType", FrameType.Control, typeof(ControlFrame)]
        [Subtype("FrameType", FrameType.Trigger, typeof(TriggerFrame)]
        public Frame Frame { get; set; }
    }

It is not necessary that FrameType be correct during serialization; it will be updated with the appropriate value based on the instantiated type.  During deserialization the FrameType field will be used to construct the correct type.

### SerializeWhen ###

The SerializeWhen attribute can be used to conditionally serialize or deserialize a field based on bound predicate.

<code>
  [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }
</code>

### SerializeUntil ###

The SerializedUntil attribute can be used to terminate a collection once a specified value is encountered.

<code>
        [FieldOffset("FirstSector", ConverterType = typeof(SectorByteConverter))]
        [SerializeUntil((byte)0)]
        public List<DirectoryRecord> Records { get; set; }
</code>

### ItemSerializeUntil ###

The ItemSerializeUntil attribute can be used to terminate a collection when an item with a specified value is encountered.

<code>
     public class Toy
     {
         public string Name { get; set; }
         public bool IsLast { get; set; }
     }
     
     public class ToyChest
     {
         [ItemSerializeUntil("IsLast", true)]
         public List&lt;Toy&gt; Toys { get; set; }
     }
</code>


## Enums ##
Enums can be used to create expressive definitions.  Depending on what attributes are specified enums will be interpreted by the serializer as either the underlying value, the literal value of the enum, or a value specified with the SerializeAsEnum attribute.  In the following example, the field will be serialized using the enum underlying byte.

    public enum Shape : byte
    {
        Circle = 0x0,
        Square = 0x1
    }

    public class MyEnumClass
    {
        public Shape Shape { get; set; }
    }

Serializing this class would result in a single byte.  Alternatively, you may want the name of the enum to be serialized:

    public class MyEnumClass
    {
        [SerializeAs(SerializedType.NullTerminatedString)]
        public Shape Shape { get; set; }
    }

You could also specify this to be a fixed-sized string, etc.

## Streams ##

In some cases you may be serializing or deserializing large amounts of data, which is logically broken into blocks or volumes.  In these cases it may be adventageous to defer handling of those sections, rather than dealing with large in-memory buffers.

<code>
        [FieldOrder(22)]
        [SerializeWhen("RecordType", RecordType.File)]
        [FieldOffset("FirstSectorData", ConverterType = typeof(SectorByteConverter))]
        [FieldLength("DataLength")]
        public Stream Data { get; set; }
</code>

In this example, the Data property will be copied from the source stream during serialization.  On deserialization, the resulting object graph will contain a Streamlet object which references a section of the source stream and allows for deferred read access.  Note that this feature is only supported when the underlying source stream supports seeking.  When dealing with non-seekable streams (e.g. NetworkStream), it is better to deserialize the stream in frames or packets where possible rather than try to deserialize the entire stream (which in some cases may be open-ended) at once.
