using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet;

namespace UnitTest.src
{
	[TestClass]
	public class Rect_Test
	{
		[TestMethod]
		public void CanComputeWidthAndHeight()
		{
			Rect rect1 = new Rect(0, 0, 640, 480);
			Assert.AreEqual(640, rect1.Width());
			Assert.AreEqual(480, rect1.Height());

			Rect rect2 = new Rect(10, 10, 12, 15);
			Assert.AreEqual(2, rect2.Width());
			Assert.AreEqual(5, rect2.Height());
		}

		[TestMethod]
		public void ComparisonOperators()
		{
			Rect rect1 = new Rect(0, 0, 640, 480);
			Rect rect2 = new Rect(0, 0, 640, 480);
			Rect rect3 = new Rect(10, 10, 12, 15);

			// Note: Internally EXPECT_EQ uses `operator ==`
			Assert.AreEqual(rect1, rect1);
			Assert.AreEqual(rect1, rect2);
			Assert.AreEqual(rect2, rect1);
			Assert.AreEqual(rect2, rect2);
			// Note: Internally EXPECT_NE uses `operator !=`
			Assert.AreNotEqual(rect1, rect3);
			Assert.AreNotEqual(rect2, rect3);
		}
	}
}
