
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class SectionHeader
	{
		public const int SizeInBytes = 8;

		public SectionHeader()
		{
		}
		public SectionHeader(Tag tag, uint length)
		{
			this.tag = new Tag(tag);
			this.length = length;
		}

		public Tag tag = new Tag();

		// Does not include sectionHeader
		public uint length;

		public void Validate(Tag tagName)
		{
			if (tag != tagName) {
				throw new System.Exception("The tag " + tag + " should read as " + tagName);
			}
		}

		// Includes sectionHeader
		public uint TotalLength() { return length + Tag.SizeInBytes; }

		public void Serialize(BinaryWriter writer)
		{
			tag.Serialize(writer);
			writer.Write(length);
		}

		public SectionHeader(BinaryReader reader)
		{
			tag = new Tag(reader);
			length = reader.ReadUInt32();
		}
	}

	//static_assert(8 == sizeof(SectionHeader), "SectionHeader is an unexpected size");
}