
namespace OP2UtilityDotNet
{
	// Code based on "Bit Twiddling Hacks" by Sean Eron Anderson
	// https://graphics.stanford.edu/~seander/bithacks.html#DetermineIfPowerOf2
	public static class BitTwiddle
	{
		public static bool IsPowerOf2(uint value)
		{
			return value != 0 && (value & (value - 1)) == 0;
		}

		private static readonly uint[] MultiplyDeBruijnBitPosition2 = new uint[32]
		{
			0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
			31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
		};

		// This method assumes the argument is a power of 2
		public static uint Log2OfPowerOf2(uint value)
		{
			return MultiplyDeBruijnBitPosition2[(value * 0x077CB531U) >> 27];
		}
	}
}