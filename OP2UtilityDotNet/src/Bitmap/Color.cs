
using System.IO;

namespace OP2UtilityDotNet.Bitmap
{
	public struct Color
	{
		public const int SizeInBytes = 4;

		public byte red;
		public byte green;
		public byte blue;
		public byte alpha;

		//public Color() { }
		public Color(byte red, byte green, byte blue, byte alpha=0)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
			this.alpha = alpha;
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(red);
			writer.Write(green);
			writer.Write(blue);
			writer.Write(alpha);
		}

		public Color(BinaryReader reader)
		{
			red = reader.ReadByte();
			green = reader.ReadByte();
			blue = reader.ReadByte();
			alpha = reader.ReadByte();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			Color c = (Color)obj;

			return c == this;
		}

		public override int GetHashCode()
		{
			return red.GetHashCode() + green.GetHashCode() * 255 + blue.GetHashCode() * 510 + alpha.GetHashCode() * 765;
		}

		public static bool operator ==(Color lhs, Color rhs)
		{
			return (lhs.red == rhs.red) && (lhs.green == rhs.green) && (lhs.blue == rhs.blue) && (lhs.alpha == rhs.alpha);
		}
		public static bool operator !=(Color lhs, Color rhs)
		{
			return !(lhs == rhs);
		}
	}

	public class Palette
	{
		public const int SizeInBytes = 256 * Color.SizeInBytes;

		public Color[] colors = new Color[256];


		public void Serialize(BinaryWriter writer)
		{
			for (int i=0; i < colors.Length; ++i)
				colors[i].Serialize(writer);
		}

		public Palette(BinaryReader reader)
		{
			for (int i=0; i < colors.Length; ++i)
				colors[i] = new Color(reader);
		}
	}

	public class DiscreteColor
	{
		public static readonly Color Black = new Color( 0, 0, 0, 0 );
		public static readonly Color White = new Color( 255, 255, 255,0 );

		public static readonly Color Red = new Color( 255, 0, 0, 0 );
		public static readonly Color Green = new Color( 0, 255, 0, 0 );
		public static readonly Color Blue = new Color( 0, 0, 255, 0 );

		public static readonly Color Yellow = new Color( 255, 255, 0 );
		public static readonly Color Cyan = new Color( 0, 255, 255 );
		public static readonly Color Magenta = new Color( 255, 0, 255 );

		public static readonly Color TransparentBlack = new Color( 0, 0, 0, 255 );
		public static readonly Color TransparentWhite = new Color( 255, 255, 255, 255 );
	}
}
