
using System.Collections.Generic;
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class Animation
	{
		public class Frame
		{
			public struct LayerMetadata
			{
				public byte backingField;

				public byte count // : 7;
				{
					get { return (byte)GetBitValue(0, 7); }
					set { SetBitValue(0, 7, value); }
				}

				public byte bReadOptionalData // : 1;
				{
					get { return (byte)GetBitValue(7, 1); }
					set { SetBitValue(7, 1, value); }
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

					backingField = (byte)field;
				}

				public void Serialize(BinaryWriter writer)
				{
					writer.Write(backingField);
				}

				public LayerMetadata(BinaryReader reader)
				{
					backingField = reader.ReadByte();
				}
			}

			//static_assert(1 == sizeof(LayerMetadata), "Animation::Frame::LayerMetadata is an unexpected size");

			public struct Layer
			{
				public ushort bitmapIndex;
				public byte unknown; // Unused by Outpost 2
				public byte frameIndex;
				public Point16 pixelOffset;

				public void Serialize(BinaryWriter writer)
				{
					writer.Write(bitmapIndex);
					writer.Write(unknown);
					writer.Write(frameIndex);
					writer.Write(pixelOffset.x);
					writer.Write(pixelOffset.y);
				}

				public Layer(BinaryReader reader)
				{
					bitmapIndex = reader.ReadUInt16();
					unknown = reader.ReadByte();
					frameIndex = reader.ReadByte();
					pixelOffset.x = reader.ReadInt16();
					pixelOffset.y = reader.ReadInt16();
				}
			}

			//static_assert(8 == sizeof(Layer), "Animation::Frame::Layer is an unexpected size");

			public LayerMetadata layerMetadata = new LayerMetadata();
			public LayerMetadata unknownBitfield = new LayerMetadata();

			public byte optional1;
			public byte optional2;
			public byte optional3;
			public byte optional4;

			// Maximum count of items in container is 128
			public List<Layer> layers = new List<Layer>();
		}

		public struct UnknownContainer
		{
			public uint unknown1;
			public uint unknown2;
			public uint unknown3;
			public uint unknown4;

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(unknown1);
				writer.Write(unknown2);
				writer.Write(unknown3);
				writer.Write(unknown4);
			}

			public UnknownContainer(BinaryReader reader)
			{
				unknown1 = reader.ReadUInt32();
				unknown2 = reader.ReadUInt32();
				unknown3 = reader.ReadUInt32();
				unknown4 = reader.ReadUInt32();
			}
		}

		//static_assert(16 == sizeof(UnknownContainer), "Animation::UnknownContainer is an unexpected size");

		public uint unknown;
		public Rect selectionRect; // pixels
		public Point32 pixelDisplacement; // Reverse direction from an offset, origin is center of tile
		public uint unknown2; //(0x3C CC/DIRT/Garage/Std. Lab construction/dock, 0x0D spider walking)

		public List<Frame> frames = new List<Frame>();

		public List<UnknownContainer> unknownContainer = new List<UnknownContainer>();
	}
}
