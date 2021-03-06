
using System;
using System.Collections.ObjectModel;
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
		public static readonly ReadOnlyCollection<ushort> ValidBitCounts = new ReadOnlyCollection<ushort>(new ushort[6] { 01, 4, 8, 16, 24, 32 });

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
			for (int i=0; i < ValidBitCounts.Count; ++i)
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
				if (headerSize == ImageHeaderV4.SizeInBytes) {
					throw new System.Exception("Image Header size indicates header version 4. Only version 1 is supported.");
				}
			
				if (headerSize == ImageHeaderV5.SizeInBytes) {
					throw new System.Exception("Image header size indicates header version 5. Only version 1 is supported.");
				}

				throw new System.Exception("Unknown image header size of " + headerSize + " detected. Header size must be equal to " + ImageHeader.SizeInBytes);
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

			return this == header;
		}

		public override int GetHashCode()
		{
			return headerSize.GetHashCode() + width.GetHashCode() + height.GetHashCode() + planes.GetHashCode()
				+ bitCount.GetHashCode() + compression.GetHashCode() + imageSize.GetHashCode()
				+ xResolution.GetHashCode() + yResolution.GetHashCode() + usedColorMapEntries.GetHashCode() + importantColorCount.GetHashCode();
		}

		public static bool operator ==(ImageHeader lhs, ImageHeader rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;

			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;

			return lhs.headerSize == rhs.headerSize
				&& lhs.width == rhs.width
				&& lhs.height == rhs.height
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

		public ImageHeader Clone()
		{
			ImageHeader header = new ImageHeader();

			header.headerSize = headerSize;
			header.width = width;
			header.height = height;
			header.planes = planes;
			header.bitCount = bitCount;
			header.compression = compression;
			header.imageSize = imageSize; //Size in bytes of pixels. Valid to set to 0 if no compression used.
			header.xResolution = xResolution;
			header.yResolution = yResolution;
			header.usedColorMapEntries = usedColorMapEntries;
			header.importantColorCount = importantColorCount;

			return header;
		}
	}

	//static_assert(40 == sizeof(ImageHeader), "ImageHeader is an unexpected size");

	public class ImageHeaderV4
	{
		public const int SizeInBytes = 17 * 4 + ImageHeader.SizeInBytes;

		public ImageHeader imageHeader = new ImageHeader();
		public uint redMask;
		public uint greenMask;
		public uint blueMask;
		public uint alphaMask;
		public uint colorSpaceType;
		public int redEndpointX;
		public int redEndpointY;
		public int redEndpointZ;
		public int greenEndpointX;
		public int greenEndpointY;
		public int greenEndpointZ;
		public int blueEndpointX;
		public int blueEndpointY;
		public int blueEndpointZ;
		public uint gammaRed;
		public uint gammaGreen;
		public uint gammaBlue;
	}

	//static_assert(108 == sizeof(ImageHeaderV4), "ImageHeaderV4 is an unexpected size");

	public class ImageHeaderV5
	{
		public const int SizeInBytes = 4 * 4 + ImageHeaderV4.SizeInBytes;

		public ImageHeaderV4 imageHeader = new ImageHeaderV4();
		public uint gamutMatchingIntent;
		// Offset in bytes from the beginning of the image header to the start of the profile data
		public uint profileDataOffset;
		// Size in bytes of embedded profile data
		public uint profileDataSize;
		// Reservered member should always be set to zero.
		public uint reserved;
	}

	//static_assert(124 == sizeof(ImageHeaderV5), "ImageHeaderV5 is an unexpected size");
}
