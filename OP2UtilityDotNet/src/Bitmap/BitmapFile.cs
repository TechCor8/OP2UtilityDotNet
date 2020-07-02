
using System.Collections.Generic;
using System.IO;

namespace OP2UtilityDotNet.Bitmap
{
	// BMP Writer only supporting Indexed Color palettes (1, 2, and 8 bit BMPs). 
	public class BitmapFile
	{
		public BmpHeader bmpHeader;
		public ImageHeader imageHeader;
		public Color[] palette;
		public byte[] pixels;

		public static BitmapFile CreateDefaultIndexed(ushort bitCount, uint width, uint height)
		{
			BitmapFile bitmapFile = new BitmapFile();
			bitmapFile.imageHeader = ImageHeader.Create((int)width, (int)height, bitCount);
			bitmapFile.palette = new Color[bitmapFile.imageHeader.CalcMaxIndexedPaletteSize()];
			bitmapFile.pixels = new byte[bitmapFile.imageHeader.CalculatePitch() * height];

			int pixelOffset = BmpHeader.SizeInBytes + ImageHeader.SizeInBytes + bitmapFile.palette.Length * Color.SizeInBytes;
			int bitmapFileSize = pixelOffset + bitmapFile.pixels.Length * sizeof(byte);

			//if (bitmapFileSize > uint.MaxValue) {
			//	throw new System.Exception("Maximum size of a bitmap file has been exceeded");
			//}

			bitmapFile.bmpHeader = BmpHeader.Create((uint)bitmapFileSize, (uint)pixelOffset);

			return bitmapFile;
		}

		// BMP Reader only supports Indexed Color palettes (1, 2, and 8 bit BMPs).
		public static BitmapFile ReadIndexed(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (BinaryReader fileReader = new BinaryReader(fs))
			{
				return ReadIndexed(fileReader);
			}
		}
		public static BitmapFile ReadIndexed(BinaryReader seekableReader)
		{
			BitmapFile bitmapFile = new BitmapFile();
			bitmapFile.bmpHeader = ReadBmpHeader(seekableReader);
			bitmapFile.imageHeader = ReadImageHeader(seekableReader);

			ReadPalette(seekableReader, bitmapFile);
			ReadPixels(seekableReader, bitmapFile);

			return bitmapFile;
		}

		// BMP Writer only supporting Indexed Color palettes (1, 2, and 8 bit BMPs).
		// @indexedPixels: Must include padding to fill each image row out to the next 4 byte memory border (pitch).
		public static void WriteIndexed(string filename, ushort bitCount, int width, int height, Color[] palette, byte[] indexedPixels)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			using (BinaryWriter fileWriter = new BinaryWriter(fs))
			{
				WriteIndexed(fileWriter, bitCount, width, height, palette, indexedPixels);
			}
		}
		public static void WriteIndexed(BinaryWriter seekableWriter, ushort bitCount, int width, int height, Color[] palette, byte[] indexedPixels)
		{
			VerifyIndexedImageForSerialization(bitCount);
			VerifyIndexedPaletteSizeDoesNotExceedBitCount(bitCount, palette.Length);
			VerifyPixelSizeMatchesImageDimensionsWithPitch(bitCount, width, height, indexedPixels.Length);

			int maxSize = ImageHeader.CalcMaxIndexedPaletteSize(bitCount);
			if (palette.Length != maxSize)
			{
				Color[] newPalette = new Color[maxSize];
				System.Array.Copy(palette, newPalette, palette.Length < newPalette.Length ? palette.Length : newPalette.Length);

				palette = newPalette;
			}

			WriteHeaders(seekableWriter, bitCount, width, height, palette);

			for (int i=0; i < palette.Length; ++i)
				palette[i].Serialize(seekableWriter);

			WritePixels(seekableWriter, indexedPixels, width, bitCount);
		}
		public static void WriteIndexed(string filename, BitmapFile bitmapFile)
		{
			// Test all properties that are auto-generated as correct when writing bitmap piecemeal
			if (bitmapFile.imageHeader.compression != BmpCompression.Uncompressed) {
				throw new System.Exception("Unable to write compressed bitmap files");
			}

			bitmapFile.Validate();

			WriteIndexed(filename, bitmapFile.imageHeader.bitCount, bitmapFile.imageHeader.width, bitmapFile.imageHeader.height, bitmapFile.palette, bitmapFile.pixels);
		}

		public void VerifyIndexedPaletteSizeDoesNotExceedBitCount()
		{
			VerifyIndexedPaletteSizeDoesNotExceedBitCount(imageHeader.bitCount, palette.Length);
		}
		public static void VerifyIndexedPaletteSizeDoesNotExceedBitCount(ushort bitCount, int paletteSize)
		{
			if (paletteSize > ImageHeader.CalcMaxIndexedPaletteSize(bitCount)) {
				throw new System.Exception("Too many colors listed on the indexed palette");
			}
		}

		// Check the pixel count is correct and already includes dummy pixels out to next 4 byte boundary.
		// @width: Width in pixels. Do not include the pitch in width.
		// @pixelsWithPitchSize: Number of pixels including padding pixels to next 4 byte boundary.
		public void VerifyPixelSizeMatchesImageDimensionsWithPitch()
		{
			VerifyPixelSizeMatchesImageDimensionsWithPitch(imageHeader.bitCount, imageHeader.width, imageHeader.height, pixels.Length);
		}
		public static void VerifyPixelSizeMatchesImageDimensionsWithPitch(ushort bitCount, int width, int height, int pixelsWithPitchSize)
		{
			if (pixelsWithPitchSize != ImageHeader.CalculatePitch(bitCount, width) * System.Math.Abs(height)) {
				throw new System.Exception("The size of pixels does not match the image's height time pitch");
			}
		}

		public void Validate()
		{
			bmpHeader.VerifyFileSignature();
			imageHeader.Validate();

			VerifyIndexedPaletteSizeDoesNotExceedBitCount();
			VerifyPixelSizeMatchesImageDimensionsWithPitch();
		}

		private static void VerifyIndexedImageForSerialization(ushort bitCount)
		{
			if (!ImageHeader.IsIndexedImage(bitCount)) {
				throw new System.Exception("Unable to read/write a non-indexed bitmap file. Bit count is " + bitCount + " but must be 8 or less");
			}
		}

		// Read
		private static BmpHeader ReadBmpHeader(BinaryReader seekableReader)
		{
			BmpHeader bmpHeader = new BmpHeader(seekableReader);

			bmpHeader.VerifyFileSignature();

			if (bmpHeader.size < seekableReader.BaseStream.Length) {
				throw new System.Exception("Bitmap file size exceed length of stream.");
			}

			return bmpHeader;
		}
		private static ImageHeader ReadImageHeader(BinaryReader seekableReader)
		{
			ImageHeader imageHeader = new ImageHeader(seekableReader);

			imageHeader.Validate();

			VerifyIndexedImageForSerialization(imageHeader.bitCount);

			return imageHeader;
		}

		private static void ReadPalette(BinaryReader seekableReader, BitmapFile bitmapFile)
		{
			if (bitmapFile.imageHeader.usedColorMapEntries != 0) {
				bitmapFile.palette = new Color[bitmapFile.imageHeader.usedColorMapEntries];
			}
			else {
				bitmapFile.palette = new Color[bitmapFile.imageHeader.CalcMaxIndexedPaletteSize()];
			}

			for (int i = 0; i < bitmapFile.palette.Length; ++i)
			{
				bitmapFile.palette[i] = new Color(seekableReader);
			}
		}

		private static void ReadPixels(BinaryReader seekableReader, BitmapFile bitmapFile)
		{
			uint pixelContainerSize = bitmapFile.bmpHeader.size - bitmapFile.bmpHeader.pixelOffset;
			VerifyPixelSizeMatchesImageDimensionsWithPitch(bitmapFile.imageHeader.bitCount, bitmapFile.imageHeader.width, bitmapFile.imageHeader.height, (int)pixelContainerSize);

			bitmapFile.pixels = new byte[pixelContainerSize];

			for (int i=0; i < bitmapFile.pixels.Length; ++i)
				bitmapFile.pixels[i] = seekableReader.ReadByte();
		}

		// Write
		private static void WriteHeaders(BinaryWriter seekableWriter, ushort bitCount, int width, int height, Color[] palette)
		{
			int pixelOffset = BmpHeader.SizeInBytes + ImageHeader.SizeInBytes + palette.Length * Color.SizeInBytes;
			int fileSize = pixelOffset + ImageHeader.CalculatePitch(bitCount, width) * System.Math.Abs(height);

			//if (fileSize > uint.MaxValue) {
			//	throw new System.Exception("Bitmap size is too large to save to disk.");
			//}

			BmpHeader bmpHeader = BmpHeader.Create((uint)fileSize, (uint)pixelOffset);
			ImageHeader imageHeader = ImageHeader.Create(width, height, bitCount);

			bmpHeader.Serialize(seekableWriter);
			imageHeader.Serialize(seekableWriter);
		}
		private static void WritePixels(BinaryWriter seekableWriter, byte[] pixels, int width, ushort bitCount)
		{
			int pitch = ImageHeader.CalculatePitch(bitCount, width);
			int bytesOfPixelsPerRow = ImageHeader.CalcPixelByteWidth(bitCount, width);
			byte[] padding = new byte[pitch - bytesOfPixelsPerRow];

			for (int i = 0; i < pixels.Length;) {
				seekableWriter.Write(pixels, i, bytesOfPixelsPerRow);
				seekableWriter.Write(padding);
				i += pitch;
			}
		}

		public override bool Equals(object obj)
		{
			BitmapFile rhs = obj as BitmapFile;

			return this == rhs;
		}

		public override int GetHashCode()
		{
			return bmpHeader.GetHashCode() + imageHeader.GetHashCode();
		}

		public static bool operator ==(BitmapFile lhs, BitmapFile rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;

			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;

			if (lhs.palette.Length != rhs.palette.Length)
				return false;

			if (lhs.pixels.Length != rhs.pixels.Length)
				return false;

			for (int i = 0; i < lhs.palette.Length; ++i)
			{
				if (lhs.palette[i] != rhs.palette[i])
					return false;
			}

			for (int i = 0; i < lhs.pixels.Length; ++i)
			{
				if (lhs.pixels[i] != rhs.pixels[i])
					return false;
			}

			return lhs.bmpHeader == rhs.bmpHeader
				&& lhs.imageHeader == rhs.imageHeader;
		}
		public static bool operator !=(BitmapFile lhs, BitmapFile rhs)
		{
			return !(lhs == rhs);
		}
	}
}
