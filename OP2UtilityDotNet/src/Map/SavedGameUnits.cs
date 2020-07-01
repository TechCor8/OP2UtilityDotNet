using System.Collections.Generic;
using System.IO;

namespace OP2UtilityDotNet.Map
{
	// Placeholder for unknown object
	public class ObjectType1
	{
		public byte[] data = new byte[512];

		public ObjectType1() { }

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(data);
		}

		public ObjectType1(BinaryReader reader)
		{
			data = reader.ReadBytes(data.Length);
		}
	}

	// Placeholder struct for unit data
	public class UnitRecord
	{
		private const int DefaultSizeOfUnit = 120;

		public byte[] data = new byte[DefaultSizeOfUnit];


		public UnitRecord() { }

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(data);
		}

		public UnitRecord(BinaryReader reader)
		{
			data = reader.ReadBytes(DefaultSizeOfUnit);
		}
	}

	// Second section of saved game specifc data (not included in .map files)
	public class SavedGameUnits
	{
		public uint unitCount;
		public uint lastUsedUnitIndex;
		public uint nextFreeUnitSlotIndex;
		public uint firstFreeUnitSlotIndex;
		public uint sizeOfUnit;
		public uint objectCount1;
		public uint objectCount2;
		public List<ObjectType1> objects1 = new List<ObjectType1>();
		public List<uint> objects2 = new List<uint>();
		public uint nextUnitIndex; //Type UnitID
		public uint prevUnitIndex; //Type UnitID
		public UnitRecord[] units = new UnitRecord[2047]; // Was 1023 before patch
		public uint[] freeUnits = new uint[2048];

		public void CheckSizeOfUnit()
		{
			if (sizeOfUnit != 120 && unitCount != 0) {
				throw new System.Exception("Size of unit must by 120 bytes if unit count is not 0");
			}
		}
	}
}
