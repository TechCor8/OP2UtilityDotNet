using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.OP2Map;

namespace UnitTest.src.OP2Map
{
	[TestClass]
	public class SavedGameUnits_Test
	{
		[TestMethod]
		public void CheckSizeOfUnit() 
		{
			SavedGameUnits savedGameUnits = new SavedGameUnits();
			savedGameUnits.unitCount = 0;
			savedGameUnits.sizeOfUnit = 0;

			// Does not throw for any unit size if unit count is 0;
			savedGameUnits.CheckSizeOfUnit();

			// Throws if unit count is not 0 and unit size is not 120
			savedGameUnits.unitCount = 1;
			Assert.ThrowsException<Exception>(() => savedGameUnits.CheckSizeOfUnit());

			savedGameUnits.sizeOfUnit = UnitRecord.DefaultSizeOfUnit;
			savedGameUnits.CheckSizeOfUnit();
		}
	}
}
