namespace OP2UtilityDotNet
{
	/// <summary>
	/// Contains platform-specific constants.
	/// </summary>
	internal class Platform
	{
#if LINUX
		public const string DLLPath = "OP2UtilityForC.so";
#else
		public const string DLLPath = "OP2UtilityForC.dll";
#endif
	}
}
