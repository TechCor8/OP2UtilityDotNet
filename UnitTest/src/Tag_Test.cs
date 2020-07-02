using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet;

namespace UnitTest.src
{
	[TestClass]
	public class Tag_Test
	{
		[TestMethod]
		public void MakeTag()
		{
			Tag tag1 = new Tag("VOL ");
			Tag tag2 = new Tag("VOL ");
			Tag tag3 = new Tag("volh");

			// Equality and inequality comparable
			Assert.AreEqual(tag1, tag2);
			Assert.AreNotEqual(tag1, tag3);

			// Convertible to std::string
			string strTag1 = tag1.ToString();
			Assert.AreEqual("VOL ", strTag1);

			// Concatenation with string literals
			string appendString1 = "String literal: " + tag1;
			Assert.AreEqual("String literal: VOL ", appendString1);

			// Concatenation with std::string
			string appendString2 = "std::string: " + tag1;
			Assert.AreEqual("std::string: VOL ", appendString2);
		}
	}
}
