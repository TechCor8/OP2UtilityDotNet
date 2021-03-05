
namespace OP2UtilityDotNet.Sprite
{
	public static class TilesetCommon
	{
		public static readonly Tag DefaultTagData = new Tag("data");
		public const uint DefaultPaletteHeaderSize = 1024;

		public static string formatReadErrorMessage(string propertyName, object value, object expectedValue)
		{
			return "Tileset property " + propertyName + " reads " + value.ToString() + ". Expected a value of " + expectedValue.ToString() + ".";
		}

		public static void throwReadError(string propertyName, object value, object expectedValue)
		{
			throw new System.Exception(formatReadErrorMessage(propertyName, value, expectedValue));
		}
	}
}