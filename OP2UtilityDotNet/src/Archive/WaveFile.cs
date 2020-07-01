using System.IO;

namespace OP2UtilityDotNet.Archive
{
	public class CommonTags
	{
		public readonly static Tag tagRIFF = new Tag("RIFF");
		public readonly static Tag tagWAVE = new Tag("WAVE");
		public readonly static Tag tagFMT_ = new Tag("fmt ");
		public readonly static Tag tagDATA = new Tag("data");
	}

	/*
	*  extended waveform format structure used for all non-PCM formats. this
	*  structure is common to all non-PCM formats.
	*  Identical to Windows.h WAVEFORMATEX typedef contained in mmeapi.h
	*/
	public class WaveFormatEx
	{
		public const int SizeInBytes = 18;

		public ushort wFormatTag;         /* format type */
		public ushort nChannels;          /* number of channels (i.e. mono, stereo...) */
		public uint nSamplesPerSec;       /* sample rate */
		public uint nAvgBytesPerSec;      /* for buffer estimation */
		public ushort nBlockAlign;        /* block size of data */
		public ushort wBitsPerSample;     /* number of bits per sample of mono data */
		public ushort cbSize;             /* the count in bytes of the size of extra information (after cbSize) */

		public WaveFormatEx() { }

		public WaveFormatEx(ushort wFormatTag, ushort nChannels, uint nSamplesPerSec, uint nAvgBytesPerSec, ushort nBlockAlign, ushort wBitsPerSample, ushort cbSize)
		{
			this.wFormatTag = wFormatTag;
			this.nChannels = nChannels;
			this.nSamplesPerSec = nSamplesPerSec;
			this.nAvgBytesPerSec = nAvgBytesPerSec;
			this.nBlockAlign = nBlockAlign;
			this.wBitsPerSample = wBitsPerSample;
			this.cbSize = cbSize;
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(wFormatTag);
			writer.Write(nChannels);
			writer.Write(nSamplesPerSec);
			writer.Write(nAvgBytesPerSec);
			writer.Write(nBlockAlign);
			writer.Write(wBitsPerSample);
			writer.Write(cbSize);
		}

		public WaveFormatEx(BinaryReader reader)
		{
			wFormatTag = reader.ReadUInt16();
			nChannels = reader.ReadUInt16();
			nSamplesPerSec = reader.ReadUInt32();
			nAvgBytesPerSec = reader.ReadUInt32();
			nBlockAlign = reader.ReadUInt16();
			wBitsPerSample = reader.ReadUInt16();
			cbSize = reader.ReadUInt16();
		}

		public override bool Equals(object obj)
		{
			WaveFormatEx b = obj as WaveFormatEx;
			if (obj == null)
				return false;

			return wFormatTag == b.wFormatTag
				&& nChannels == b.nChannels
				&& nSamplesPerSec == b.nSamplesPerSec
				&& nAvgBytesPerSec == b.nAvgBytesPerSec
				&& nBlockAlign == b.nBlockAlign
				&& wBitsPerSample == b.wBitsPerSample
				&& cbSize == b.cbSize;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(WaveFormatEx a, WaveFormatEx b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(WaveFormatEx a, WaveFormatEx b)
		{
			return !a.Equals(b);
		}
	};

	public class RiffHeader
	{
		public const int SizeInBytes = Tag.SizeInBytes + sizeof(uint) + Tag.SizeInBytes;

		public Tag riffTag;
		public uint chunkSize;
		public Tag waveTag;

		public RiffHeader()
		{
			riffTag = new Tag();
			waveTag = new Tag();
		}

		public void Serialize(BinaryWriter writer)
		{
			riffTag.Serialize(writer);
			writer.Write(chunkSize);
			waveTag.Serialize(writer);
		}

		public RiffHeader(BinaryReader reader)
		{
			riffTag = new Tag(reader);
			chunkSize = reader.ReadUInt32();
			waveTag = new Tag(reader);
		}
	};

	//static_assert(12 == sizeof(RiffHeader), "RiffHeader is an unexpected size");

	public class FormatChunk
	{
		public const int SizeInBytes = Tag.SizeInBytes + sizeof(uint) + WaveFormatEx.SizeInBytes;

		public Tag fmtTag;
		public uint formatSize;
		public WaveFormatEx waveFormat;


		public FormatChunk()
		{
			fmtTag = new Tag();
			waveFormat = new WaveFormatEx();
		}

		public void Serialize(BinaryWriter writer)
		{
			fmtTag.Serialize(writer);
			writer.Write(formatSize);
			waveFormat.Serialize(writer);
		}

		public FormatChunk(BinaryReader reader)
		{
			fmtTag = new Tag(reader);
			formatSize = reader.ReadUInt32();
			waveFormat = new WaveFormatEx(reader);
		}
	};

	//static_assert(8 + sizeof(WaveFormatEx) == sizeof(FormatChunk), "FormatChunk is an unexpected size");

	public class ChunkHeader
	{
		public const int SizeInBytes = Tag.SizeInBytes + 4;

		public Tag formatTag;
		public uint length;


		public ChunkHeader() { }

		public void Serialize(BinaryWriter writer)
		{
			formatTag.Serialize(writer);
			writer.Write(length);
		}

		public ChunkHeader(BinaryReader reader)
		{
			formatTag = new Tag(reader);
			length = reader.ReadUInt32();
		}
	};

	//static_assert(8 == sizeof(ChunkHeader), "ChunkHeader is an unexpected size");

	// http://soundfile.sapp.org/doc/WaveFormat/
	public class WaveHeader
	{
		public static WaveHeader Create(WaveFormatEx waveFormat, uint dataLength)
		{
			WaveHeader waveHeader = new WaveHeader();

			waveHeader.riffHeader.riffTag = CommonTags.tagRIFF;
			waveHeader.riffHeader.waveTag = CommonTags.tagWAVE;
			waveHeader.riffHeader.chunkSize = Tag.SizeInBytes + FormatChunk.SizeInBytes + ChunkHeader.SizeInBytes + dataLength;

			waveHeader.formatChunk.fmtTag = CommonTags.tagFMT_;
			waveHeader.formatChunk.formatSize = WaveFormatEx.SizeInBytes;
			waveHeader.formatChunk.waveFormat = waveFormat;
			waveHeader.formatChunk.waveFormat.cbSize = 0;

			waveHeader.dataChunk.formatTag = CommonTags.tagDATA;
			waveHeader.dataChunk.length = dataLength;

			return waveHeader;
		}

		public RiffHeader riffHeader = new RiffHeader();
		public FormatChunk formatChunk = new FormatChunk();
		public ChunkHeader dataChunk = new ChunkHeader();


		public WaveHeader() { }

		public void Serialize(BinaryWriter writer)
		{
			riffHeader.Serialize(writer);
			formatChunk.Serialize(writer);
			dataChunk.Serialize(writer);
		}

		public WaveHeader(BinaryReader reader)
		{
			riffHeader = new RiffHeader(reader);
			formatChunk = new FormatChunk(reader);
			dataChunk = new ChunkHeader(reader);
		}
	}

	//static_assert(sizeof(RiffHeader) + sizeof(FormatChunk) + sizeof(ChunkHeader) == sizeof(WaveHeader), "WaveHeader is an unexpected size");
}
