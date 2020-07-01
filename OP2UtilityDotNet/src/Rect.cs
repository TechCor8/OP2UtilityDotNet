using System.IO;

namespace OP2UtilityDotNet
{
	public struct Rect
	{
		public int x1;
		public int y1;
		public int x2;
		public int y2;

		public int Width()
		{
			return x2 - x1;
		}

		public int Height()
		{
			return y2 - y1;
		}

		public Rect(int x1, int y1, int x2, int y2)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(x1);
			writer.Write(y1);
			writer.Write(x2);
			writer.Write(y2);
		}

		public Rect(BinaryReader reader)
		{
			x1 = reader.ReadInt32();
			y1 = reader.ReadInt32();
			x2 = reader.ReadInt32();
			y2 = reader.ReadInt32();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			Rect r = (Rect)obj;

			return r == this;
		}

		public override int GetHashCode()
		{
			return x1.GetHashCode() * x2.GetHashCode() * y1.GetHashCode() * y2.GetHashCode();
		}

		public static bool operator ==(Rect lhs, Rect rhs)
		{
			return (lhs.x1 == rhs.x1) && (lhs.y1 == rhs.y1) && (lhs.x2 == rhs.x2) && (lhs.y2 == rhs.y2);
		}
		public static bool operator !=(Rect lhs, Rect rhs)
		{
			return !(lhs == rhs);
		}
	}
}
