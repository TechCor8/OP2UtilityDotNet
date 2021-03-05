
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class TilesetHeader
	{
		public const int SizeInBytes = 5 * 4 + 1 * SectionHeader.SizeInBytes;

		public SectionHeader sectionHead = new SectionHeader();
		public uint tagCount;
		public uint pixelWidth;
		public uint pixelHeight; // Assume height is always positive, unlike standard Windows Bitmap Format
		public uint bitDepth;
		public uint flags;

		public static readonly Tag DefaultTagHead = new Tag("head");
		public const uint DefaultSectionSize = 0x14;
		public const uint DefaultTagCount = 2;
		public const uint DefaultPixelWidth = 32;
		public const uint DefaultPixelHeightMultiple = DefaultPixelWidth;
		public const uint DefaultBitDepth = 8;
		public const uint DefaultFlags = 8; // Unsure meaning of Flags


		public TilesetHeader() { }
		public static TilesetHeader Create(uint heightInTiles)
		{
			return new TilesetHeader()
			{
				sectionHead = new SectionHeader(DefaultTagHead, DefaultSectionSize),
				tagCount = DefaultTagCount,
				pixelWidth = DefaultPixelWidth,
				pixelHeight = heightInTiles * DefaultPixelHeightMultiple,
				bitDepth = DefaultBitDepth,
				flags = DefaultFlags
			};
		}

		public void Validate()
		{
			if (sectionHead.tag != DefaultTagHead)
			{
				TilesetCommon.throwReadError("Header Tag", sectionHead.tag, DefaultTagHead);
			}

			if (sectionHead.length != DefaultSectionSize)
			{
				TilesetCommon.throwReadError("Header Section Size", sectionHead.length, DefaultSectionSize);
			}

			if (pixelWidth != DefaultPixelWidth)
			{
				TilesetCommon.throwReadError("Pixel Width", pixelWidth, DefaultPixelWidth);
			}

			if (pixelHeight % DefaultPixelHeightMultiple != 0)
			{
				throw new System.Exception("Tileset property Pixel Height reads " + pixelHeight +
					". Pixel Height must be a multiple of " + DefaultPixelHeightMultiple + ".");
			}

			if (tagCount != DefaultTagCount)
			{
				TilesetCommon.throwReadError("Header tag count", tagCount, DefaultTagCount);
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			sectionHead.Serialize(writer);
			writer.Write(tagCount);
			writer.Write(pixelWidth);
			writer.Write(pixelHeight);
			writer.Write(bitDepth);
			writer.Write(flags);
		}

		public TilesetHeader(BinaryReader reader)
		{
			sectionHead = new SectionHeader(reader);
			tagCount = reader.ReadUInt32();
			pixelWidth = reader.ReadUInt32();
			pixelHeight = reader.ReadUInt32();
			bitDepth = reader.ReadUInt32();
			flags = reader.ReadUInt32();
		}
	}

	//static_assert(4 + 3 * sizeof(SectionHeader) == sizeof(PaletteHeader), "PaletteHeader is an unexpected size");

	public class PpalHeader
	{
		public const int SizeInBytes = 4 + 2 * SectionHeader.SizeInBytes;

		public SectionHeader ppal = new SectionHeader();
		public SectionHeader head = new SectionHeader();
		public uint tagCount;

		public static readonly Tag DefaultTagPpal = new Tag("PPAL");
		public const uint DefaultPpalSectionSize = 1048;
		public static readonly Tag DefaultTagHead = new Tag("head");
		public const uint DefaultHeadSectionSize = 4;
		public const uint DefaultTagCount = 1;

		public static PpalHeader Create()
		{
			return new PpalHeader()
			{
				ppal = new SectionHeader(DefaultTagPpal, DefaultPpalSectionSize),
				head = new SectionHeader(DefaultTagHead, DefaultHeadSectionSize),
				tagCount = DefaultTagCount
			};
		}

		public PpalHeader() { }

		public void Validate()
		{
			if (ppal.tag != DefaultTagPpal)
			{
				TilesetCommon.throwReadError("PPAL Tag", ppal.tag, DefaultTagPpal);
			}

			if (ppal.length != DefaultPpalSectionSize)
			{
				TilesetCommon.throwReadError("PPAL Section Size", ppal.length, DefaultPpalSectionSize);
			}

			if (head.tag != DefaultTagHead)
			{
				TilesetCommon.throwReadError("PPAL Head Tag", head.tag, DefaultTagHead);
			}

			if (head.length != DefaultHeadSectionSize)
			{
				TilesetCommon.throwReadError("PPAL Head Section Size", head.length, DefaultHeadSectionSize);
			}

			if (tagCount != DefaultTagCount)
			{
				TilesetCommon.throwReadError("PPAL Section Tag Count", tagCount, DefaultTagCount);
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			ppal.Serialize(writer);
			head.Serialize(writer);
			writer.Write(tagCount);
		}

		public PpalHeader(BinaryReader reader)
		{
			ppal = new SectionHeader(reader);
			head = new SectionHeader(reader);
			tagCount = reader.ReadUInt32();
		}
	}
}