
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class ImageMeta
	{
		public ushort GetBitCount()
		{
			return (ushort)(type.bShadow != 0 ? 1 : 8);
		}

		public class ImageType
		{
			public ushort backingField;

			public ushort bGameGraphic // : 1;  // 0 = MenuGraphic, 1 = GameGraphic
			{
				get { return (ushort)GetBitValue(0, 1); }
				set { SetBitValue(0, 1, value); }
			}

			public ushort unknown1 // : 1; // 2
			{
				get { return (ushort)GetBitValue(1, 1); }
				set { SetBitValue(1, 1, value); }
			}

			public ushort bShadow // : 1; // 4
			{
				get { return (ushort)GetBitValue(2, 1); }
				set { SetBitValue(2, 1, value); }
			}

			public ushort unknown2 // : 1; // 8
			{
				get { return (ushort)GetBitValue(3, 1); }
				set { SetBitValue(3, 1, value); }
			}

			public ushort unknown3 // : 1; // 16
			{
				get { return (ushort)GetBitValue(4, 1); }
				set { SetBitValue(4, 1, value); }
			}

			public ushort unknown4 // : 1; // 32
			{
				get { return (ushort)GetBitValue(5, 1); }
				set { SetBitValue(5, 1, value); }
			}

			public ushort bTruckBed // : 1; // 64
			{
				get { return (ushort)GetBitValue(6, 1); }
				set { SetBitValue(6, 1, value); }
			}

			public ushort unknown5 // : 1; // 128
			{
				get { return (ushort)GetBitValue(7, 1); }
				set { SetBitValue(7, 1, value); }
			}

			public ushort unknown6 // : 8;
			{
				get { return (ushort)GetBitValue(8, 8); }
				set { SetBitValue(8, 8, value); }
			}

			private int GetBitValue(int offset, int length)
			{
				int result = backingField >> offset;// Remove the offset from the backing field
				result &= ~(-1 << length);			// Creates a mask with "length" number of bits set and clear the bits
			
				return result;
			}

			private void SetBitValue(int offset, int length, int value)
			{
				value = value << offset;			// Move the value to set to the correct offset

				int mask = ~(-1 << length);			// Creates a mask with "length" number of bits set
				mask = mask << offset;				// Move masked bits to the correct offset

				int field = backingField;
				field &= ~mask;						// Clear the masked bits in the backing field
				field |= value & mask;				// Set the value in the masked bits

				backingField = (ushort)field;
			}
		}

		//static_assert(2 == sizeof(ImageType), "ImageMeta::ImageType is an unexpected size");

		public uint scanLineByteWidth; //number of bytes in each scan line of image (this should be the width of the image rounded up to a 32 bit boundary)
		public uint pixelDataOffset; // Offset of the pixel data in the .bmp file
		public uint height; // Height of image in pixels
		public uint width; // Width of image in pixels
		public ImageType type = new ImageType();
		public ushort paletteIndex;


		public ImageMeta() { }

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(scanLineByteWidth);
			writer.Write(pixelDataOffset);
			writer.Write(height);
			writer.Write(width);
			writer.Write(type.backingField);
			writer.Write(paletteIndex);
		}

		public ImageMeta(BinaryReader reader)
		{
			scanLineByteWidth = reader.ReadUInt32();
			pixelDataOffset = reader.ReadUInt32();
			height = reader.ReadUInt32();
			width = reader.ReadUInt32();
			type.backingField = reader.ReadUInt16();
			paletteIndex = reader.ReadUInt16();
		}
	}

	//static_assert(18 + sizeof(ImageMeta::ImageType) == sizeof(ImageMeta), "ImageMeta is an unexpected size");
}