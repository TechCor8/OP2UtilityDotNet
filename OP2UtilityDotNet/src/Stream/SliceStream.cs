using System.IO;

namespace OP2UtilityDotNet.Streams
{
	public class SliceStream : Stream
	{
		private Stream _BaseStream;
		private long _DataOffset;
		private long _DataLength;
		private long _Position;


		public SliceStream(Stream baseStream, long dataOffset, long dataLength)
		{
			_BaseStream = baseStream;
			_DataOffset = dataOffset;
			_DataLength = dataLength;
		}

		public override bool CanRead => _BaseStream.CanRead;
		public override bool CanSeek => _BaseStream.CanSeek;
		public override bool CanWrite => _BaseStream.CanWrite;

		public override long Length => _DataLength;

		public override long Position { get => _Position; set => Seek(value, SeekOrigin.Begin); }

		public override void Flush()
		{
			_BaseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_Position + count > _DataLength)
			{
				throw new EndOfStreamException();
			}

			// Set base stream position
			long basePosition = _BaseStream.Position;
			_BaseStream.Position = _DataOffset + _Position;

			int bytesRead;

			try
			{
				// Read
				bytesRead = _BaseStream.Read(buffer, offset, count);
				_Position += bytesRead;
			}
			finally
			{
				// Restore base stream position
				_BaseStream.Position = basePosition;
			}

			return bytesRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					_Position = offset;
					break;

				case SeekOrigin.Current:
					_Position = _Position + offset;
					break;

				case SeekOrigin.End:
					_Position = _DataLength - offset;
					break;
			}

			if (_Position < 0)
				_Position = 0;

			if (_Position >= _DataLength)
				_Position = _DataLength - 1;

			return _Position;
		}

		public override void SetLength(long value)
		{
			long newLength = value;

			if (_DataOffset + newLength > _BaseStream.Length)
			{
				newLength = _BaseStream.Length - _DataOffset;
			}

			_DataLength = newLength;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_Position + count > _DataLength)
			{
				throw new EndOfStreamException();
			}

			// Set base stream position
			long basePosition = _BaseStream.Position;
			_BaseStream.Position = _DataOffset + _Position;

			try
			{
				// Write
				_BaseStream.Write(buffer, offset, count);
				_Position += count;
			}
			finally
			{
				// Restore base stream position
				_BaseStream.Position = basePosition;
			}
		}
	}
}
