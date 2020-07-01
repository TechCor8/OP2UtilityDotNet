
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class PaletteHeader
	{
		public const int SizeInBytes = 4 + 3 * SectionHeader.SizeInBytes;

		public static readonly Tag TagSection = new Tag("PPAL");
		public static readonly Tag TagHeader = new Tag("head");
		public static readonly Tag TagData = new Tag("data");


		public PaletteHeader() { }
		public static PaletteHeader CreatePaletteHeader()
		{
			PaletteHeader paletteHeader = new PaletteHeader();
			paletteHeader.remainingTagCount = 1;

			ulong dataSize = Bitmap.Palette.SizeInBytes;

			ulong overallSize = 4 + SectionHeader.SizeInBytes + 
				SectionHeader.SizeInBytes + sizeof(uint) + dataSize;

			if (overallSize > uint.MaxValue) {
				throw new System.Exception("PRT palettes section is too large.");
			}
			
			paletteHeader.overallHeader = new SectionHeader(TagSection, (uint)overallSize);
			paletteHeader.sectionHeader = new SectionHeader(TagHeader, sizeof(uint));
			paletteHeader.dataHeader = new SectionHeader(TagData, (uint)dataSize);

			return paletteHeader;
		}

		public SectionHeader overallHeader = new SectionHeader();
		public SectionHeader sectionHeader = new SectionHeader();
		public uint remainingTagCount;
		public SectionHeader dataHeader = new SectionHeader();

		public void Validate()
		{
			overallHeader.Validate(TagSection);
			sectionHeader.Validate(TagHeader);
			dataHeader.Validate(TagData);

			if (overallHeader.length != SectionHeader.SizeInBytes + sectionHeader.TotalLength() + sizeof(uint) + dataHeader.TotalLength())
			{
				throw new System.Exception("Lengths defined in palette headers do not match.");
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			overallHeader.Serialize(writer);
			sectionHeader.Serialize(writer);
			writer.Write(remainingTagCount);
			dataHeader.Serialize(writer);
		}

		public PaletteHeader(BinaryReader reader)
		{
			overallHeader = new SectionHeader(reader);
			sectionHeader = new SectionHeader(reader);
			remainingTagCount = reader.ReadUInt32();
			dataHeader = new SectionHeader(reader);
		}
	}

	//static_assert(4 + 3 * sizeof(SectionHeader) == sizeof(PaletteHeader), "PaletteHeader is an unexpected size");
}
