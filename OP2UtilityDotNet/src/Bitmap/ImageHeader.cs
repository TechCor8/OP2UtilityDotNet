
using System.IO;

namespace OP2UtilityDotNet.Bitmap
{
	public class ImageHeader
	{
		public const int SizeInBytes = 40;

		public static ImageHeader Create(int width, int height, ushort bitCount)
		{
			VerifyValidBitCount(bitCount);

			ImageHeader header = new ImageHeader();

			header.headerSize = SizeInBytes;
			header.width = width;
			header.height = height;
			header.planes = DefaultPlanes;
			header.bitCount = bitCount;
			header.compression = BmpCompression.Uncompressed;
			header.imageSize = DefaultImageSize;
			header.xResolution = DefaultXResolution;
			header.yResolution = DefaultYResolution;
			header.usedColorMapEntries = DefaultUsedColorMapEntries;
			header.importantColorCount = DefaultImportantColorCount;

			return header;
		}

		public uint headerSize;
		public int width;
		public int height;
		public ushort planes;
		public ushort bitCount;
		public BmpCompression compression;
		public uint imageSize; //Size in bytes of pixels. Valid to set to 0 if no compression used.
		public uint xResolution;
		public uint yResolution;
		public uint usedColorMapEntries;
		public uint importantColorCount;

		// Default values
		public const ushort DefaultPlanes = 1;
		public const uint DefaultImageSize = 0;
		public const uint DefaultXResolution = 0;
		public const uint DefaultYResolution = 0;
		public const uint DefaultUsedColorMapEntries = 0;
		public const uint DefaultImportantColorCount = 0;

		// BitCount verification
		public static readonly ushort[] ValidBitCounts = new ushort[6] { 01, 4, 8, 16, 24, 32 };

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(headerSize);
			writer.Write(width);
			writer.Write(height);
			writer.Write(planes);
			writer.Write(bitCount);
			writer.Write((uint)compression);
			writer.Write(imageSize);
			writer.Write(xResolution);
			writer.Write(yResolution);
			writer.Write(usedColorMapEntries);
			writer.Write(importantColorCount);
		}

		public ImageHeader() { }

		public ImageHeader(BinaryReader reader)
		{
			headerSize = reader.ReadUInt32();
			width = reader.ReadInt32();
			height = reader.ReadInt32();
			planes = reader.ReadUInt16();
			bitCount = reader.ReadUInt16();
			compression = (BmpCompression)reader.ReadUInt32();
			imageSize = reader.ReadUInt32();
			xResolution = reader.ReadUInt32();
			yResolution = reader.ReadUInt32();
			usedColorMapEntries = reader.ReadUInt32();
			importantColorCount = reader.ReadUInt32();
		}

		public bool IsValidBitCount()
		{
			return IsValidBitCount(bitCount);
		}
		public static bool IsValidBitCount(ushort bitCount)
		{
			for (int i=0; i < ValidBitCounts.Length; ++i)
			{
				if (ValidBitCounts[i] == bitCount)
					return true;
			}
			
			return false;
		}

		public bool IsIndexedImage()
		{
			return IsIndexedImage(bitCount);
		}

		public static bool IsIndexedImage(ushort bitCount)
		{
			return bitCount <= 8;
		}

		public void VerifyValidBitCount()
		{
			VerifyValidBitCount(bitCount);
		}

		public static void VerifyValidBitCount(ushort bitCount)
		{
			if (!IsValidBitCount(bitCount)) {
				throw new System.Exception("A bit count of " + bitCount + " is not supported");
			}
		}

		public int CalculatePitch()
		{
			return CalculatePitch(bitCount, width);
		}
		public static int CalculatePitch(ushort bitCount, int width)
		{
			int bytesOfPixelsPerRow = CalcPixelByteWidth(bitCount, width);
			return (bytesOfPixelsPerRow + 3) & ~3;
		}

		// Does not include padding
		public int CalcPixelByteWidth()
		{
			return CalcPixelByteWidth(bitCount, width);
		}
		public static int CalcPixelByteWidth(ushort bitCount, int width)
		{
			const int bitsPerByte = 8;
			return ((width * bitCount) + (bitsPerByte - 1)) / bitsPerByte;
		}

		public int CalcMaxIndexedPaletteSize()
		{
			return CalcMaxIndexedPaletteSize(bitCount);
		}
		public static int CalcMaxIndexedPaletteSize(ushort bitCount)
		{
			if (!IsIndexedImage(bitCount)) {
				throw new System.Exception("Bit count does not have an associated max palette size");
			}

			return 1 << bitCount;
		}

		public void Validate()
		{
			if (headerSize != ImageHeader.SizeInBytes) {
				throw new System.Exception("Image Header size must be equal to " + ImageHeader.SizeInBytes);
			}

			if (planes != DefaultPlanes) {
				throw new System.Exception("Image format not supported: only single plane images are supported, but this image has " + planes);
			}

			VerifyValidBitCount(bitCount);

			if (usedColorMapEntries > CalcMaxIndexedPaletteSize()) {
				throw new System.Exception("Used color map entries is greater than possible range of color map (palette)");
			}

			if (importantColorCount > CalcMaxIndexedPaletteSize()) {
				throw new System.Exception("Important Color Count is greater than possible range of color map (palette)");
			}
		}

		public override bool Equals(object obj)
		{
			ImageHeader header = obj as ImageHeader;

			if (header == null)
				return false;

			return header == this;
		}

		public override int GetHashCode()
		{
			return headerSize.GetHashCode() + width.GetHashCode() + height.GetHashCode() + planes.GetHashCode()
				+ bitCount.GetHashCode() + compression.GetHashCode() + imageSize.GetHashCode()
				+ xResolution.GetHashCode() + yResolution.GetHashCode() + usedColorMapEntries.GetHashCode() + importantColorCount.GetHashCode();
		}

		public static bool operator ==(ImageHeader lhs, ImageHeader rhs)
		{
			return lhs.headerSize == rhs.headerSize
				&& lhs.width == rhs.width
				&& lhs.height == rhs.headerSize
				&& lhs.planes == rhs.planes
				&& lhs.bitCount == rhs.bitCount
				&& lhs.compression == rhs.compression
				&& lhs.imageSize == rhs.imageSize
				&& lhs.xResolution == rhs.xResolution
				&& lhs.yResolution == rhs.yResolution
				&& lhs.usedColorMapEntries == rhs.usedColorMapEntries
				&& lhs.importantColorCount == rhs.importantColorCount;
		}
		public static bool operator !=(ImageHeader lhs, ImageHeader rhs)
		{
			return !(lhs == rhs);
		}
	}

	//static_assert(40 == sizeof(ImageHeader), "ImageHeader is an unexpected size");
}
