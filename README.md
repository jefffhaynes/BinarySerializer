BinarySerializer

BinarySerializer is a declarative serialization framework for controlling the formatting of data at the byte level.  It employs patterns and concepts from the XmlSerializer and WPF.

Intro

If you’ve used the XmlSerializer before then you know it can be a powerful albeit sometimes limited approach for dealing with XML.  The most important difference between XML and “raw” binary is, of course, the syntax.  That is to say, XML has syntax and raw data has none.  As such, it is important to remember that when you create the object model for BinarySerializer you are representing both the data and the structure of the data.  This can be especially tricky when dealing with legacy formats that you are effectively reverse engineering.  You’ll probably find yourself walking metaphysical musings such as, is this collection terminated internally or externally?  However, if you can pull it off it makes for highly readable and maintainable code.

At first blush, expressing data with no inherent syntax seems almost impossible.  However, it turns out that the majority of formats out there have a number of patterns that tend to show up again and again.  As long as we can account for those patterns, we can start to form a language that can be abstractly applied.  One of the key enabling ingredients for this is bindings.

Introduction to Bindings

If you’ve used WPF then you know Bindings are a huge part of XAML-based design.  When faced with the problem of declarative binary serialization I decided to steal a pattern that Microsoft has already shown to be effective.  Let’s jump into some code.  Say you have a string prefixed with a length (similar to something you might see in the COM world, except we’re going to skimp and use one byte for simplicity’s sake), like this:

 



SimpleBinding
 

If we were going to process this procedurally, it might look something like:

var reader = new BinaryReader(stream);
var nameLength = reader.ReadByte();
var nameData = reader.ReadBytes(nameLength);
var name = Encoding.UTF8.GetString(nameData);
Not too bad but not too readable either and this is a fairly trivial example.  Wouldn’t it be nice if we could just write:

public byte NameLength { get; set; }

[FieldLength("NameLength")]
public string Name { get; set; }
Well, we can.  Feeling adventurous?  Of course you are.

ISO9660 Example

Your mother said you would never understand the ISO9660 specification, but you knew better.  Then life happened and one thing lead to another and well you just never got around to it.  Thankfully there are still one or two CDROMs left in the world and so it isn’t too late.  I’m not going to reproduce the procedural version here but let’s assume that it’s rather horrendous.  Hopefully we can do better, starting with the Primary Volume Descriptor as it is essentially a big structure that shows up at the beginning of the ISO and doesn’t contain anything too exotic:

 public class PrimaryVolumeDescriptor
    {
        [FieldCount(16)]
        public List<Sector> BootArea { get; set; }

        public VolumeDescriptorType Type { get; set; }

        ...
    }
Let’s hold up here for a second.  We’ve got two things so far: a list and an enum.  We’ve already dealt with a sort of list in the first example when we deserialized a string.  However, there we knew the length, which is to say that we knew the byte count.  In this case the spec tells us that we’re dealing with 16 boot sectors.  For the time being we don’t really care what the structure of those sectors is so we’ll define them as:

    public class Sector
    {
        [FieldLength(2048)]
        public byte[] Filler { get; set; }
    }
Maybe we’ll go back and get clever later (bonus points for writing an object model for boot code) but for now we’ll just store the contents.  It’s worth mentioning that we could have represented the boot sectors as:

 public class PrimaryVolumeDescriptor
    {
        [FieldLength(32768)]
        public byte[] BootArea { get; set; }

        ...
    }
with similar results.  For that matter we could have represented them in any number of ways, which hopefully starts to give you some idea of the difference between binary serialization and the document model for a well-defined language like XML.  However, we might as well retain some of the intended structure.  What about the enum?  Well in this case, that’s easy:

    public enum VolumeDescriptorType : byte
    {
        BootRecord = 0,
        PrimaryVolumeDescriptor = 1,
        SupplementaryVolumeDescriptor = 2,
        VolumePartitionDescriptor = 3,
        VolumeDescriptorSetTerminator = 255
    }
This allows the serializer to use the underlying byte type to represent the enum.  Ok, moving on a bit more we’re going to skip over some boring stuff and come to:

        ...

        public ushort SectorSize { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public ushort SectorSizeBig { get; set; }

        public uint PathTableLength { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public uint PathTableLengthBig { get; set; }

        public uint FirstLittleEndianPathTableSector { get; set; }

        public uint SecondLittleEndianPathTableSector { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public uint FirstBigEndianPathTableSector { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public uint SecondBigEndianPathTableSector { get; set; }

        ...
This somewhat strange pattern of alternating little and big endian representation is something we see sprinkled throughout the ISO9660 spec.  In an attempt to ease adoption and increase performance the authors of the spec decided to encode key entries in interleaved big and little-endian byte representation.  Fortunately the big endian fields are easily dispatched by specifying the SerializeAs attribute and specifying endianness.  There are three important fields here: PathTableLength, SectorSize, and FirstLittleEndianPathTableSector.  Two of these can be used together to give us the all-important PathTableOffset, which will give us the offset to the master catalog for the the ISO.  So, further down in the class we’ll define:

        [Ignore]
        public long PathTableOffset
        {
            get { return FirstLittleEndianPathTableSector * SectorSize; }
        }
This is a bit of a cheat as we’re including a member suggesting a field that doesn’t really exist in the spec.  However, we want to bind to it later so it’s useful to leave it here and decorate it with Ignore.  The more “correct” way to do this is probably using a Converter, but we’ll get to that.

Backing off a bit, let’s start to put some of this together and jump up to the root of our object model:

    public class Iso9660
    {
        public PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; set; }

        [FieldOffset("PrimaryVolumeDescriptor.PathTableOffset")]
        [FieldLength("PrimaryVolumeDescriptor.PathTableLength")]
        public List<PathTableRecord> PathTableRecords { get; set; }
    }
All of a sudden the spec doesn’t look so bad.  We’ve got that big structure at the top and then a list of these PathTableRecords and for the first time we start to see what the field bindings can do for readability.  Again, it’s worth mentioning that putting the PathTableRecords at the root like this was purely an object model design decision.  We could have just as easily put the PathTableRecords member at the end of the PrimaryVolumeDescriptor definition and achieved the desired result.  However, that doesn’t feel quite right and would seem to deviate somewhat from the intention.

The PathTableRecords are essentially the directories we can expect to find on the iso.

    public class PathTableRecord
    {
        public byte NameLength { get; set; }
        public byte ExtendedAttributeRecordSectors { get; set; }
        public uint FirstSector { get; set; }
        public ushort ParentDirectoryRecord { get; set; }

        [FieldLength("NameLength")]
        public string Name { get; set; }

        [FieldLength("PaddingLength")]
        public byte[] Padding { get; set; }

        [Ignore]
        public int PaddingLength
        {
            get { return NameLength % 2; }
        }

        [FieldOffset("FirstSector", ConverterType = typeof(SectorByteConverter))]
        [SerializeUntil((byte)0)]
        public List<DirectoryRecord> Records { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
And now things start to get a little more interesting.  Here you can see a few items, including our old NameLength from the original example as well as the computed field trick again.  Something may seem off here: in all other bindings we were binding to source fields that preceded the target.  Normally that would be a requirement of the framework as the type definition is walked linearly and the source field must be valuated before any binding can be resolved.  However, since this is a computed and representative field it doesn’t really matter as long as the arguments to the computation (in this case just NameLength) have already been resolved.  In fact, it doesn’t matter at all where we place the PaddingLength member in the class definition since it isn’t part of our serialization.

The other thing we’ve introduced here is a converter.  Another WPF concept, the binding converter allows a transformation of the source during evaluation.

    class SectorByteConverter : IValueConverter
    {
        public object Convert(object value, BinarySerializationContext ctx)
        {
            var sector = (uint) value;
            var iso = ctx.FindAncestor<Iso9660>();
            var sectorSize = iso.PrimaryVolumeDescriptor.SectorSize;
            return sector*sectorSize;
        }
    }
Here we also get our first real glimpse behind the curtain.  The BinarySerializationContext is our reference to the object graph at this point in the serialization and allows us to go and grab the SectorSize we need during conversion.  This approach is used in place of the WPF ConverterParameter, which works fine in XAML but less so when we’re restricted to using custom attributes.

Lastly, we see a SerializeUntil attribute here.  Let’s skip that momentarily and come back to it after we look at the structure of the DirectoryRecord:

    public class DirectoryRecord
    {
        private const int PrePaddingLength = 33;

        public byte Length { get; set; }
        public byte ExtendedAttributeRecordSectors { get; set; }

        public uint FirstSectorData { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public uint FirstSectorDataBig { get; set; }

        public uint DataLength { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public uint DataLengthBig { get; set; }

        public byte YearsSince1900 { get; set; }
        public byte Month { get; set; }
        public byte Day { get; set; }
        public byte Hour { get; set; }
        public byte Minute { get; set; }
        public byte Second { get; set; }
        public sbyte GmtOffset { get; set; }
        public RecordType RecordType { get; set; }
        public byte InterleaveFileUnitSize { get; set; }
        public byte InterleaveGapSize { get; set; }
        public ushort VolumeSequenceNumber { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSequenceNumberBig { get; set; }
        public byte IdentifierLength { get; set; }

        [FieldLength("IdentifierLength")]
        public string Identifier { get; set; }

        [FieldLength("PaddingLength")]
        public byte[] Padding { get; set; }

        [Ignore]
        public int PaddingLength 
        {
            get { return (IdentifierLength + 1) % 2; } 
        }

        [Ignore]
        public DateTime DateTime
        {
            get
            {
                return new DateTime(1900 + YearsSince1900, Month, Day,
                                    Hour, Minute, Second, DateTimeKind.Local);
            }

            set
            {
                YearsSince1900 = (byte)(value.Year - 1900);
                Month = (byte)(value.Month);
                Day = (byte)(value.Day);
                Hour = (byte)(value.Hour);
                Minute = (byte)(value.Minute);
                Second = (byte)(value.Second);
            }
        }

        [Ignore]
        public int SystemReserveLength
        {
            get { return Length - PrePaddingLength - IdentifierLength - PaddingLength; }
        }

        [FieldLength("SystemReserveLength")]
        public byte[] SystemReserve { get; set; }

        [SerializeWhen("RecordType", RecordType.File)]
        [FieldOffset("FirstSectorData", ConverterType = typeof(SectorByteConverter))]
        [FieldLength("DataLength")]
        public Stream Data { get; set; }

        public override string ToString()
        {
            return Identifier;
        }
    }
A lot of this is more of the same so let’s skip down to the good stuff: the data.  The first thing we see here is the SerializeWhenAttribute.  This should probably be a SubtypeAttribute but the SubtypeAttribute didn’t exist when I wrote this example and this accomplishes more-or-less the same thing.  SerializeWhen does what it says and only serializes or deserializes the field in question when the equality is true.  In this case, the Data field will only be included if the RecordType field equals RecordType.File (which happens to be denoted by zero per the spec).

The next thing is the FieldOffsetAttribute, which is going to temporarily relocate the serialization to an arbitrary offset in the stream and then bring us back again.  Again, we see the same converter here to give us the byte offset from the sector offset.

The last thing to note here is the use of a Stream rather than a byte array.  At first this may seem a little strange but it yields some nice behavior.  We could easily have specified a byte array here but this would result in large allocations since each instance would represent a complete file.

During serialization the onus is on us to provide streams here pointing to the source data.  However, during deserialization the framework is going to allocate special Streamlets here, which are really just references to offsets in the source stream and allow for deferred, restricted access to each file.  It’s worth noting that this approach depends on the source steam being seekable, which isn’t a problem in this case since we’re reading from an iso.

Lastly let’s quickly go back to the SerializeUntilAttribute from the PathTableRecord class.  When I originally wrote this, I wrote the Records member as

        [FieldOffset("FirstSector", ConverterType = typeof(SectorByteConverter))]
        [ItemSerializeUntil("Length", 0)]
        public List<DirectoryRecord> Records { get; set; }
At first blush this may look similar and this approach is actually more in-line with what is stated in the spec, which is something like “read this list until you encounter a DirectoryRecord with length zero.”    Well that’s fine to say, but it turns out that a DirectoryRecord of length zero can’t actually exist!  If the length is zero, the computed SystemReserveLength is negative.  You could argue that this should be a special case and that if the length is zero then so should be the SystemReserveLength.  However, the spec doesn’t say anything about that.  As such, I decided it was more correct to serialize the collection until a zero was encountered.  It may seem like a fine line but this is the sort of thing you run into quite a bit when trying to create object models for formats like this.

Conclusion

The ISO9660 example was the second program I wrote using BinarySerializer but even so I was pleasantly surprised to find that it took only a few days to have a simple program that could handle large and complex ISOs with great aplomb.  Others have since gone on to write even more complicated models for a wide variety of uses but hopefully this gives you some insight into some of the capabilities provided by the framework.

This entire example can be found in the BinarySerializer solution.
