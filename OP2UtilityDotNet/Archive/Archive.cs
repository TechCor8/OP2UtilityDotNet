using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	public abstract class Archive : IDisposable
	{
		protected IntPtr m_ArchivePtr;

		public abstract void Dispose();

		public string GetArchiveFilename()										{ return Archive_GetArchiveFilename(m_ArchivePtr);							}
		public ulong GetArchiveFileSize()										{ return Archive_GetArchiveFileSize(m_ArchivePtr);							}
		public ulong GetCount()													{ return Archive_GetCount(m_ArchivePtr);									}

		public bool Contains(string name)										{ return Archive_Contains(m_ArchivePtr, name);								}
		public void ExtractFileByName(string name, string pathOut)				{ Archive_ExtractFileByName(m_ArchivePtr, name, pathOut);					}

		public ulong GetIndex(string name)										{ return Archive_GetIndex(m_ArchivePtr, name);								}
		public string GetName(ulong index)										{ return Marshalling.GetString(Archive_GetName(m_ArchivePtr, index));		}

		public uint GetSize(ulong index)										{ return Archive_GetSize(m_ArchivePtr, index);								}
		public void ExtractFileByIndex(ulong index, string pathOut)				{ Archive_ExtractFileByIndex(m_ArchivePtr, index, pathOut);					}
		public void ExtractAllFiles(string destDirectory)						{ Archive_ExtractAllFiles(m_ArchivePtr, destDirectory);						}

		
		public byte[] ReadFileByIndex(ulong index)
		{
			int size = (int)GetSize(index);
			IntPtr buffer = Marshal.AllocHGlobal(size);

			Archive_ReadFileByIndex(m_ArchivePtr, index, buffer);
			byte[] file = new byte[size];
			Marshal.Copy(buffer, file, 0, size);

			Marshal.FreeHGlobal(buffer);

			return file;
		}
		public byte[] ReadFileByName(string name)								{ return ReadFileByIndex(GetIndex(name));									}


		[DllImport(Platform.DLLPath)] private static extern string Archive_GetArchiveFilename(IntPtr archive);
		[DllImport(Platform.DLLPath)] private static extern ulong Archive_GetArchiveFileSize(IntPtr archive);
		[DllImport(Platform.DLLPath)] private static extern ulong Archive_GetCount(IntPtr archive);

		[DllImport(Platform.DLLPath)] private static extern  bool Archive_Contains(IntPtr archive, string name);
		[DllImport(Platform.DLLPath)] private static extern void Archive_ExtractFileByName(IntPtr archive, string name, string pathOut);

		[DllImport(Platform.DLLPath)] private static extern ulong Archive_GetIndex(IntPtr archive, string name);
		[DllImport(Platform.DLLPath)] private static extern IntPtr Archive_GetName(IntPtr archive, ulong index);

		[DllImport(Platform.DLLPath)] private static extern uint Archive_GetSize(IntPtr archive, ulong index);
		[DllImport(Platform.DLLPath)] private static extern void Archive_ExtractFileByIndex(IntPtr archive, ulong index, string pathOut);
		[DllImport(Platform.DLLPath)] private static extern void Archive_ExtractAllFiles(IntPtr archive, string destDirectory);

		[DllImport(Platform.DLLPath)] private static extern void Archive_ReadFileByIndex(IntPtr archive, ulong index, IntPtr buffer);
	}
}
