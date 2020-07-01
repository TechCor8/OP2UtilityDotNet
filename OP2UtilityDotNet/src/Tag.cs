using System;
using System.IO;

namespace OP2UtilityDotNet
{
	public class Tag
	{
		public const int SizeInBytes = 4;

		private byte[] text = new byte[4];


		public byte[] GetBytes()
		{
			return text;
		}

		// Allow default construction
		public Tag()
		{
		}

		// Allow construction from string literals
		public Tag(string text)
		{
			for (int i=0; i < text.Length && i < this.text.Length; ++i)
				this.text[i] = (byte)text[i];
		}

		// Allow construction from other Tag objects
		public Tag(Tag tag)
		{
			Array.Copy(tag.text, text, tag.text.Length);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(text);
		}

		public Tag(BinaryReader reader)
		{
			text = reader.ReadBytes(4);
		}

		// Equality and inequality comparable
		public override bool Equals(object obj)
		{
			Tag t = obj as Tag;

			if (t == null)
				return false;

			return this == t;
		}

		public override int GetHashCode()
		{
			return text.GetHashCode();
		}

		public static bool operator ==(Tag tag1, Tag tag2)
		{
			if (tag1.text.Length != tag2.text.Length)
				return false;

			for (int i=0; i < tag1.text.Length; ++i)
			{
				if (tag1.text[i] != tag2.text[i])
					return false;
			}

			return true;
		}

		public static bool operator !=(Tag tag1, Tag tag2)
		{
			return !(tag1 == tag2);
		}

		public override string ToString()
		{
			return System.Text.Encoding.ASCII.GetString(text);
		}
	}
}
