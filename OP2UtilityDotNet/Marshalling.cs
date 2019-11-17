using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	internal class Marshalling
	{
		public static string GetString(IntPtr cStr)
		{
			string result = Marshal.PtrToStringAnsi(cStr);
			FreeString(cStr);

			return result;
		}


		[DllImport(Platform.DLLPath)] private static extern void FreeString(IntPtr str);
	}
}
