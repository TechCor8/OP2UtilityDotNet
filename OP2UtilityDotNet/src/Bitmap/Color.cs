
using System;
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

		// Swap Red and Blue color values. Does not affect alpha.
		public void SwapRedAndBlue()
		{
			byte swap = blue;
			blue = red;
			red = swap;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			Color c = (Color)obj;

			return this == c;
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


		public Palette() { }

		public Palette(Palette clone)
		{
			if (colors.Length != clone.colors.Length)
				throw new Exception("clone color length is invalid. Expected: " + colors.Length + " Actual: " + clone.colors.Length);

			Array.Copy(clone.colors, colors, colors.Length);
		}

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
		public static Color Black { get { return new Color( 0, 0, 0, 0 ); }  }
		public static Color White { get { return new Color( 255, 255, 255,0 ); }  }

		public static Color Red { get { return new Color( 255, 0, 0, 0 ); }  }
		public static Color Green { get { return new Color( 0, 255, 0, 0 ); }  }
		public static Color Blue { get { return new Color( 0, 0, 255, 0 ); }  }

		public static Color Yellow { get { return new Color( 255, 255, 0 ); }  }
		public static Color Cyan { get { return new Color( 0, 255, 255 ); }  }
		public static Color Magenta { get { return new Color( 255, 0, 255 ); }  }

		public static Color TransparentBlack { get { return new Color( 0, 0, 0, 255 ); }  }
		public static Color TransparentWhite { get { return new Color( 255, 255, 255, 255 ); }  }
	}
}
