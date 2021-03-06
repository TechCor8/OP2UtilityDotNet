using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace OP2UtilityDotNet.Bitmap
{
	public class BmpHeader
	{
		public const int SizeInBytes = 14;

		// @pixelOffset: Offset from start of file to first pixel in image
		public static BmpHeader Create(uint fileSize, uint pixelOffset)
		{
			BmpHeader header = new BmpHeader();

			header.fileSignature = new byte[FileSignature.Count];
			FileSignature.CopyTo(header.fileSignature, 0);
			//System.Array.Copy(FileSignature, header.fileSignature, FileSignature.Count);

			header.size = fileSize;
			header.reserved1 = DefaultReserved1;
			header.reserved2 = DefaultReserved2;
			header.pixelOffset = pixelOffset;

			return header;
		}

		public byte[] fileSignature = new byte[2];
		public uint size;
		public ushort reserved1;
		public ushort reserved2;
		public uint pixelOffset;

		public static readonly ReadOnlyCollection<byte> FileSignature = new ReadOnlyCollection<byte>(new byte[2] { (byte)'B', (byte)'M' });
		public const ushort DefaultReserved1 = 0;
		public const ushort DefaultReserved2 = 0;

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(fileSignature);
			writer.Write(size);
			writer.Write(reserved1);
			writer.Write(reserved2);
			writer.Write(pixelOffset);
		}

		public BmpHeader() { }

		public BmpHeader(BinaryReader reader)
		{
			fileSignature = reader.ReadBytes(2);
			size = reader.ReadUInt32();
			reserved1 = reader.ReadUInt16();
			reserved2 = reader.ReadUInt16();
			pixelOffset = reader.ReadUInt32();
		}

		public bool IsValidFileSignature()
		{
			if (fileSignature.Length != FileSignature.Count)
				return false;

			for (int i = 0; i < fileSignature.Length; ++i)
			{
				if (fileSignature[i] != FileSignature[i])
					return false;
			}

			return true;
		}
		public void VerifyFileSignature()
		{
			if (!IsValidFileSignature()) {
				throw new System.Exception("BmpHeader does not contain a proper File Signature (Magic Number).");
			}
		}

		public override bool Equals(object obj)
		{
			BmpHeader header = obj as BmpHeader;

			return this == header;
		}

		public override int GetHashCode()
		{
			return size.GetHashCode() + reserved1.GetHashCode() + reserved2.GetHashCode() + pixelOffset.GetHashCode();
		}

		public static bool operator ==(BmpHeader lhs, BmpHeader rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;

			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;

			if (lhs.fileSignature.Length != rhs.fileSignature.Length)
				return false;

			for (int i = 0; i < lhs.fileSignature.Length; ++i)
			{
				if (lhs.fileSignature[i] != rhs.fileSignature[i])
					return false;
			}

			return (lhs.size == rhs.size) && (lhs.reserved1 == rhs.reserved1) && (lhs.reserved2 == rhs.reserved2) && (lhs.pixelOffset == rhs.pixelOffset);
		}
		public static bool operator !=(BmpHeader lhs, BmpHeader rhs)
		{
			return !(lhs == rhs);
		}

		public BmpHeader Clone()
		{
			BmpHeader header = new BmpHeader();
			header.fileSignature[0] = fileSignature[0];
			header.fileSignature[1] = fileSignature[1];
			header.size = size;
			header.reserved1 = reserved1;
			header.reserved2 = reserved2;
			header.pixelOffset = pixelOffset;

			return header;
		}
	}

	//static_assert(14 == sizeof(BmpHeader), "BmpHeader is an unexpected size");
}
