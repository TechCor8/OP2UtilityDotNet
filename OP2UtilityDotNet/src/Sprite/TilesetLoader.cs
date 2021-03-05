using OP2UtilityDotNet.Bitmap;
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public static class TilesetLoader
	{
		public static readonly Tag TagFileSignature = new Tag("PBMP");

		public static bool PeekIsCustomTileset(Stream reader)
		{
			byte[] text = new byte[Tag.SizeInBytes];
			int readCount = reader.Read(text, 0, text.Length);
			reader.Position -= readCount;

			Tag tag = new Tag(text);

			return tag == TagFileSignature;
		}

		// Read either custom tileset format or standard bitmap format tileset into memory
		// After reading into memory, if needed, reformats into standard 8 bit indexed bitmap before returning
		public static BitmapFile ReadTileset(Stream reader)
		{
			if (PeekIsCustomTileset(reader)) {
				return ReadCustomTileset(reader);
			}
		
			try
			{
				BitmapFile tileset = BitmapFile.ReadIndexed(reader);
				ValidateTileset(tileset);
				return tileset;
			}
			catch (System.Exception e)
			{
				throw new System.Exception("Unable to read tileset represented as standard bitmap. " + e.ToString());
			}
		}

		// Read tileset represented by Outpost 2 specific format into memory
		// After Reading into memory, reformats into standard 8 bit indexed bitmap before returning results
		public static BitmapFile ReadCustomTileset(Stream streamToRead)
		{
			using (BinaryReader reader = new BinaryReader(streamToRead, System.Text.Encoding.UTF8, true))
			{
				SectionHeader fileSignature = new SectionHeader(reader);
				ValidateFileSignatureHeader(fileSignature);

				TilesetHeader tilesetHeader = new TilesetHeader(reader);
				tilesetHeader.Validate();

				PpalHeader ppalHeader = new PpalHeader(reader);
				ppalHeader.Validate();

				SectionHeader paletteHeader = new SectionHeader(reader);
				ValidatePaletteHeader(paletteHeader);

				BitmapFile bitmapFile = new BitmapFile((ushort)tilesetHeader.bitDepth, tilesetHeader.pixelWidth, (int)tilesetHeader.pixelHeight * -1);
				for (int i=0; i < bitmapFile.palette.Length; ++i)
				{
					bitmapFile.palette[i] = new Color(reader);
				}

				SectionHeader pixelHeader = new SectionHeader(reader);
				ValidatePixelHeader(pixelHeader, tilesetHeader.pixelHeight);

				reader.Read(bitmapFile.pixels, 0, bitmapFile.pixels.Length);
				// Tilesets store red and blue Color values swapped from standard Bitmap file format
				bitmapFile.SwapRedAndBlue();

				ValidateTileset(bitmapFile);

				return bitmapFile;
			}
		}

		// Write tileset in Outpost 2's custom bitmap format.
		// To write tileset in standard bitmap format, use BitmapFile::WriteIndexed
		public static void WriteCustomTileset(Stream streamToWrite, BitmapFile tileset)
		{
			ValidateTileset(tileset);

			// OP2 Custom Tileset assumes a positive height and TopDown Scan Line (Contradicts Windows Bitmap File Format)
			if (tileset.GetScanLineOrientation() == ScanLineOrientation.BottomUp)
			{
				tileset.InvertScanLines();
			}
			uint absoluteHeight = (uint)tileset.AbsoluteHeight();

			SectionHeader fileSignature = new SectionHeader(TagFileSignature, CalculatePbmpSectionSize(absoluteHeight));
			TilesetHeader tilesetHeader = TilesetHeader.Create(absoluteHeight / TilesetHeader.DefaultPixelHeightMultiple);
			PpalHeader ppalHeader = PpalHeader.Create();
		
			SectionHeader paletteHeader = new SectionHeader(TilesetCommon.DefaultTagData, TilesetCommon.DefaultPaletteHeaderSize);
			SwapPaletteRedAndBlue(tileset.palette);

			SectionHeader pixelHeader = new SectionHeader(TilesetCommon.DefaultTagData, CalculatePixelHeaderLength(absoluteHeight));

			using (BinaryWriter writer = new BinaryWriter(streamToWrite, System.Text.Encoding.UTF8, true))
			{
				fileSignature.Serialize(writer);
				tilesetHeader.Serialize(writer);
				ppalHeader.Serialize(writer);
				paletteHeader.Serialize(writer);
				for (int i=0; i < tileset.palette.Length; ++i)
				{
					tileset.palette[i].Serialize(writer);
				}
				pixelHeader.Serialize(writer);
				writer.Write(tileset.pixels);
			}
		}

		// Throw runtime error if provided bitmap does not meet specific tileset requirements
		// Assumes provided tileset is already properly formed to standard bitmap file format
		public static void ValidateTileset(BitmapFile tileset)
		{
			int DefaultPixelWidth = 32;
			int DefaultPixelHeightMultiple = DefaultPixelWidth;
			ushort DefaultBitCount = 8;

			if (tileset.imageHeader.bitCount != DefaultBitCount)
			{
				TilesetCommon.throwReadError("Bit Depth", tileset.imageHeader.bitCount, DefaultBitCount);
			}

			if (tileset.imageHeader.width != DefaultPixelWidth)
			{
				TilesetCommon.throwReadError("Pixel Width", tileset.imageHeader.width, DefaultPixelWidth);
			}

			if (tileset.imageHeader.height % DefaultPixelHeightMultiple != 0)
			{
				throw new System.Exception("Tileset property Pixel Height reads " + tileset.imageHeader.height +
					". Expected value must be a multiple of " + DefaultPixelHeightMultiple + " pixels");
			}
		}

		private static void ValidateFileSignatureHeader(SectionHeader fileSignatureHeader)
		{
			if (fileSignatureHeader.tag != TagFileSignature)
			{
				TilesetCommon.throwReadError("File Signature", fileSignatureHeader.tag, TagFileSignature);
			}

			if (fileSignatureHeader.length == 0)
			{
				throw new System.Exception("Tileset property File Length reads " + 
					fileSignatureHeader.length + ", which is too small.");
			}
		}

		private static void ValidatePaletteHeader(SectionHeader paletteHeader)
		{
			if (paletteHeader.tag != TilesetCommon.DefaultTagData)
			{
				TilesetCommon.throwReadError("Palette Header Tag", paletteHeader.tag, TilesetCommon.DefaultTagData);
			}

			if (paletteHeader.length != TilesetCommon.DefaultPaletteHeaderSize)
			{
				TilesetCommon.throwReadError("Palette Header Section Size", paletteHeader.length, TilesetCommon.DefaultPaletteHeaderSize);
			}
		}

		private static void ValidatePixelHeader(SectionHeader pixelHeader, uint height)
		{
			if (pixelHeader.tag != TilesetCommon.DefaultTagData)
			{
				TilesetCommon.throwReadError("Pixel Header Tag", pixelHeader.tag, TilesetCommon.DefaultTagData);
			}

			uint expectedLength = CalculatePixelHeaderLength(height);
			if (pixelHeader.length != expectedLength)
			{
				TilesetCommon.throwReadError("Pixel Header Length", pixelHeader.length, expectedLength);
			}
		}

		private static uint CalculatePbmpSectionSize(uint pixelLength)
		{
			return Tag.SizeInBytes + // PBMP Header
				TilesetHeader.SizeInBytes +
				PpalHeader.SizeInBytes +
				Tag.SizeInBytes + TilesetCommon.DefaultPaletteHeaderSize + //Palette and Header
				Tag.SizeInBytes + CalculatePixelHeaderLength(pixelLength) - //Pixels and Header
				16;
		}

		private static uint CalculatePixelHeaderLength(uint height)
		{
			// Because tilesets are have a bitDepth of 8 and are always 32 pixels wide, 
			// can assume a padding of 0 bytes on each scan line.
			return TilesetHeader.DefaultPixelWidth * height;
		}

		private static void SwapPaletteRedAndBlue(Color[] palette)
		{
			foreach (Color color in palette)
			{
				color.SwapRedAndBlue();
			}
		}
	}
}