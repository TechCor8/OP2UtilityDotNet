
namespace OP2UtilityDotNet.Archive
{
	// This is designed for use in the .vol file decompressor
	public class BitStreamReader
	{
		public BitStreamReader(byte[] buffer, uint bufferSize) // Construct stream around given buffer
		{
			m_BufferBitSize = (bufferSize << 3);
			m_Buffer = buffer;
			m_ReadBitIndex = 0;
			m_ReadBuff = 0;

			// Check bufferSize does not exceed the max addressable bit index
			if (bufferSize > uint.MaxValue / 8)
			{
				throw new System.ArgumentException("BitStreamReader cannot support a buffer size of " + bufferSize);
			}
		}

		// Get bit at Read index and advance index
		public bool ReadNextBit()
		{
			bool bNextBit;

			// Check for end of stream
			if (m_ReadBitIndex >= m_BufferBitSize)
			{
				return false;
			}

			// Check if a new byte needs to be buffered
			if ((m_ReadBitIndex & 0x07) == 0)
			{
				m_ReadBuff = m_Buffer[m_ReadBitIndex >> 3];
			}

			// Extract the uppermost bit
			bNextBit = (m_ReadBuff & 0x80) == 0x80;	// Get the MSB

			// Update the Read Buffer and Read Index
			m_ReadBuff <<= 1;	// Shift buffer left 1 bit (MSB out)
			m_ReadBitIndex++;

			return bNextBit;	// Return the next bit in the stream
		}

		// Get next 8 bits at Read index and advance index
		public int  ReadNext8Bits()
		{
			int value;

			// Check for end of stream
			if (m_ReadBitIndex >= m_BufferBitSize)
			{
				return 0;
			}

			int i = (int)(m_ReadBitIndex & 0x07);
			if (i == 0)
			{
				// Read the next byte and return it
				value = m_Buffer[m_ReadBitIndex >> 3];
				m_ReadBitIndex += 8;
				return value;
			}

			value = m_ReadBuff;
			m_ReadBitIndex += 8;

			// Check for end of stream
			if (m_ReadBitIndex >= m_BufferBitSize)
			{
				m_ReadBuff = 0;
			}
			else
			{
				m_ReadBuff = m_Buffer[m_ReadBitIndex >> 3];
			}

			value |= (m_ReadBuff >> (8 - i));
			m_ReadBuff <<= i;
			return value;
		}

		// Returns true if the last bit has been read
		public bool EndOfStream()
		{
			return m_ReadBitIndex >= m_BufferBitSize;
		}

		// Returns the position (in bits) of the read pointer
		public uint GetBitReadPos()
		{
			return m_ReadBitIndex;
		}
	
		private uint m_BufferBitSize;// Size of buffer in bits
		private byte[] m_Buffer;	// Byte array of data

		private uint m_ReadBitIndex;	// Current bit being read from the stream

		private byte m_ReadBuff;	// 1 Byte read buffer (shift bits out of)
	}
}
