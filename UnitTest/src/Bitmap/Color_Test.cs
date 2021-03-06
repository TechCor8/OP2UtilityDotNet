using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;

namespace UnitTest.src.Bitmap
{
	[TestClass]
	public class Color_Test
	{
		[TestMethod]
		public void CompareEqual()
		{
			// Note: Internally EXPECT_EQ uses `operator ==`
			//       EXPECT_EQ has better error formatting than calling == explicitly

			// Black and white compare equal with themselves
			Assert.AreEqual(DiscreteColor.Black, DiscreteColor.Black);
			Assert.AreEqual(DiscreteColor.White, DiscreteColor.White);
			// Primary colors compare equal with themselves
			Assert.AreEqual(DiscreteColor.Red, DiscreteColor.Red);
			Assert.AreEqual(DiscreteColor.Green, DiscreteColor.Green);
			Assert.AreEqual(DiscreteColor.Blue, DiscreteColor.Blue);

			// Check negative result
			Assert.IsFalse(DiscreteColor.Black == DiscreteColor.White);
		}

		[TestMethod]
		public void CompareEqualWithNewlyConstructedValue()
		{
			// Construct these non-inline so as to not interfere with macro expansion
			// (Preprocessor gets confused when C++ grammar ","s appear on the same line)
			Color Black = new Color( 0, 0, 0, 0 );
			Color White = new Color( 255, 255, 255, 0 );
			// Colors compare equal with constructed equivalents
			Assert.AreEqual(DiscreteColor.Black, Black);
			Assert.AreEqual(DiscreteColor.White, White);
		}

		[TestMethod]
		public void CompareNotEqual()
		{
			// Note: Internally EXPECT_NE uses `operator !=`
			//       EXPECT_NE has better error formatting than calling != explicitly

			// Black and white are distinct (3 components differ)
			Assert.AreNotEqual(DiscreteColor.Black, DiscreteColor.White);
			Assert.AreNotEqual(DiscreteColor.White, DiscreteColor.Black);

			// Primary colors are distinct (2 components differ)
			Assert.AreNotEqual(DiscreteColor.Red, DiscreteColor.Green);
			Assert.AreNotEqual(DiscreteColor.Red, DiscreteColor.Blue);
			Assert.AreNotEqual(DiscreteColor.Green, DiscreteColor.Blue);
			Assert.AreNotEqual(DiscreteColor.Green, DiscreteColor.Red);
			Assert.AreNotEqual(DiscreteColor.Blue, DiscreteColor.Red);
			Assert.AreNotEqual(DiscreteColor.Blue, DiscreteColor.Green);

			// Primary and Black differ (1 component differs)
			Assert.AreNotEqual(DiscreteColor.Red, DiscreteColor.Black);
			Assert.AreNotEqual(DiscreteColor.Green, DiscreteColor.Black);
			Assert.AreNotEqual(DiscreteColor.Blue, DiscreteColor.Black);
			// Primary and Secondary colors are distinct (1 component differs)
			Assert.AreNotEqual(DiscreteColor.Red, DiscreteColor.Yellow);
			Assert.AreNotEqual(DiscreteColor.Red, DiscreteColor.Magenta);
			Assert.AreNotEqual(DiscreteColor.Green, DiscreteColor.Yellow);
			Assert.AreNotEqual(DiscreteColor.Green, DiscreteColor.Cyan);
			Assert.AreNotEqual(DiscreteColor.Blue, DiscreteColor.Cyan);
			Assert.AreNotEqual(DiscreteColor.Blue, DiscreteColor.Magenta);
		}

		[TestMethod]
		public void CompareEqualNotEqualTransparent()
		{
			// Transparent color should be equal with itself
			Assert.AreEqual(DiscreteColor.TransparentBlack, DiscreteColor.TransparentBlack);
			Assert.AreEqual(DiscreteColor.TransparentWhite, DiscreteColor.TransparentWhite);

			// Transparent color should not compare equal with non-transparent version
			Assert.AreNotEqual(DiscreteColor.TransparentBlack, DiscreteColor.Black);
			Assert.AreNotEqual(DiscreteColor.TransparentWhite, DiscreteColor.White);
		}

		[TestMethod]
		public void SwapRedAndBlue()
		{
			Color color = DiscreteColor.Blue;
			color.SwapRedAndBlue();

			Assert.AreEqual(color, DiscreteColor.Red);
		}
	}
}
