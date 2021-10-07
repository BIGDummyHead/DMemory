using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DummyMemory
{
    /// <summary>
    /// A class for exposing some win32api.dll methods.
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// Used to represent of a window.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// X - Position of upper-left corner of window.
            /// </summary>
            public int Left;
            /// <summary>
            /// Y - Position of upper-left corner of window.
            /// </summary>
            public int Top;
            /// <summary>
            /// X - Position of lower-right corner of window.
            /// </summary>
            public int Right;
            /// <summary>
            /// Y - Position of lower-right corner of window.
            /// </summary>
            public int Bottom;

            /// <summary>
            /// Width of window.
            /// </summary>
            /// <remarks><see cref="Right"/> - <seealso cref="Left"/></remarks>
            public int Width => Right - Left;
            /// <summary>
            /// Height of window.
            /// </summary>
            /// <remarks><see cref="Bottom"/> - <seealso cref="Top"/></remarks>
            public int Height => Bottom - Top;

            /// <summary>
            /// Shows as Width x Height
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }

        /// <summary>
        /// Flag access
        /// </summary>
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            /// <summary>
            /// All 
            /// </summary>
            All = 0x001F0FFF,
            /// <summary>
            /// Terminate Access
            /// </summary>
            Terminate = 0x00000001,
            /// <summary>
            /// Create Thread Access
            /// </summary>
            CreateThread = 0x00000002,
            /// <summary>
            /// Broad Virtual Memory Operation
            /// </summary>
            VirtualMemoryOperation = 0x00000008,
            /// <summary>
            /// Virtually read memory
            /// </summary>
            VirtualMemoryRead = 0x00000010,
            /// <summary>
            /// Virtually write memory
            /// </summary>
            VirtualMemoryWrite = 0x00000020,
            /// <summary>
            /// 
            /// </summary>
            DuplicateHandle = 0x00000040,
            /// <summary>
            /// 
            /// </summary>
            CreateProcess = 0x000000080,
            /// <summary>
            /// 
            /// </summary>
            SetQuota = 0x00000100,
            /// <summary>
            /// 
            /// </summary>
            SetInformation = 0x00000200,
            /// <summary>
            /// 
            /// </summary>
            QueryInformation = 0x00000400,
            /// <summary>
            /// 
            /// </summary>
            QueryLimitedInformation = 0x00001000,
            /// <summary>
            /// 
            /// </summary>
            Synchronize = 0x00100000
        }

        /// <summary>
        /// Used for changing memory protection
        /// </summary>
        [Flags]
        public enum MemoryProtection : uint
        {
            /// <summary>
            /// Memory may only execute
            /// </summary>
            Execute = 0x10,
            /// <summary>
            /// Memory may execute and read
            /// </summary>
            ExecuteRead = 0x20,
            /// <summary>
            /// Memory may Execute, read, and write
            /// </summary>
            ExecuteReadWrite = 0x40,
            /// <summary>
            /// Memory may Execute, write, and copy
            /// </summary>
            ExecuteWriteCopy = 0x80,
            /// <summary>
            /// No access is given
            /// </summary>
            NoAccess = 0x01,
            /// <summary>
            /// Memory can only be read
            /// </summary>
            ReadOnly = 0x02,
            /// <summary>
            /// Memory can be read or written to
            /// </summary>
            ReadWrite = 0x04,
            /// <summary>
            /// Memory can only be copied and written to
            /// </summary>
            WriteCopy = 0x08,
            /// <summary>
            /// 
            /// </summary>
            GuardModifierflag = 0x100,
            /// <summary>
            /// 
            /// </summary>
            NoCacheModifierflag = 0x200,
            /// <summary>
            /// 
            /// </summary>
            WriteCombineModifierflag = 0x400
        }


        /// <summary>
        /// Used for allocating / deallocating memory regions
        /// </summary>
        [Flags]
        public enum AllocationType
        {
            /// <summary>
            /// </summary>
            Commit = 0x1000,
            /// <summary>
            /// Reserve a region, size is specified in allocation methods
            /// </summary>
            Reserve = 0x2000,
            /// <summary>
            /// </summary>
            Decommit = 0x4000,
            /// <summary>
            /// Release memory, used for deallocating memory regions
            /// </summary>
            Release = 0x8000,
            /// <summary>
            /// </summary>
            Reset = 0x80000,
            /// <summary>
            /// </summary>
            Physical = 0x400000,
            /// <summary>
            /// </summary>
            TopDown = 0x100000,
            /// <summary>
            /// </summary>
            WriteWatch = 0x200000,
            /// <summary>
            /// </summary>
            LargePages = 0x20000000
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const uint WS_BORDER = 0x800000;
        public const int GWL_STYLE = (-16);
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_SHOWWINDOW = 0x0040;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get window size
        /// </summary>
        /// <param name="hWnd"><see cref="Process.MainWindowHandle"/></param>
        /// <param name="lpRect">Window size.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        /// <summary>
        /// Get window size
        /// </summary>
        /// <param name="hWnd"><see cref="Process.MainWindowHandle"/></param>
        /// <param name="lpRect">Window size.</param>
        /// <returns></returns>
        public static bool GetWindowRect(IntPtr hWnd, out RECT lpRect) => GetWindowRect(new HandleRef(null, hWnd), out lpRect);

        /// <summary>
        /// Get <see cref="IntPtr"/> for process.
        /// </summary>
        /// <param name="dwAccess"></param>
        /// <param name="inherit"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwAccess, bool inherit, int pid);

        /// <summary>
        /// Get <see cref="IntPtr"/> for process
        /// </summary>
        /// <param name="proc">The process to open</param>
        /// <param name="flags">Access Flags</param>
        /// <returns></returns>
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags) => OpenProcess((uint)flags, false, proc.Id);

        /// <summary>
        /// Close process handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Read process memory
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpBaseAddress">Where to start read</param>
        /// <param name="lpBuffer">Where to place data.</param>
        /// <param name="dwSize">Bytes to read</param>
        /// <param name="lpNumberOfBytesRead">Bytes actually read.</param>
        /// <returns></returns>

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// Read process memory
        /// </summary>
        /// <param name="hProcess">Process</param>
        /// <param name="lpBaseAddress">Where to start read.</param>
        /// <param name="size">Size of byte[] passed back out</param>
        /// <param name="bytesRead">The bytes read from address</param>
        /// <returns></returns>
        /// <remarks>Used for the lazy</remarks>
        public static bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, int size, out byte[] bytesRead)
        {
            bytesRead = new byte[size];
            return ReadProcessMemory(hProcess, lpBaseAddress, bytesRead, size, out _);
        }

        /// <summary>
        /// Read process memory
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpBaseAddress">Where to start read</param>
        /// <param name="lpBuffer">Where to place data.</param>
        /// <param name="dwSize">Bytes to read</param>
        /// <param name="lpNumberOfBytesRead">Bytes actually read.</param>
        /// <returns></returns>

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, ulong lpBaseAddress, [In, Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// Write bytes to a memory address
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpBaseAddress">Address to start write</param>
        /// <param name="lpBuffer">Bytes to write</param>
        /// <param name="dwSize">Size of bytes to write.</param>
        /// <param name="lpNumberOfBytesWritten">How many bytes were really written?</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        /// <summary>
        /// Write bytes to a memory address
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpBaseAddress">Address to start write</param>
        /// <param name="write">Bytes to write</param>
        /// <returns></returns>
        public static bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] write)
        {
            return WriteProcessMemory(hProcess, lpBaseAddress, write, write.Length, out _);
        }

        /// <summary>
        /// Virtually protect a piece an address, this can either change the protection rights of an address to make it read/writable or more stringent. 
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpAddress">Memory address to protect</param>
        /// <param name="dwSize">Size of memory to protect</param>
        /// <param name="flNewProtect">New protection</param>
        /// <param name="lpflOldProtect">Removed protection</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);



        /// <summary>
        /// Virtually protect a piece an address, this can either change the protection rights of an address to make it read/writable or more stringent. 
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpAddress">Memory address to protect</param>
        /// <param name="dwSize">Size of memory to protect</param>
        /// <param name="flNewProtect">New protection</param>
        /// <param name="lpflOldProtect">Removed protection</param>
        /// <returns></returns>
        public static bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, ProcessAccessFlags flNewProtect, out ProcessAccessFlags lpflOldProtect)
        {
            bool ret = VirtualProtectEx(hProcess, lpAddress, dwSize, (uint)flNewProtect, out uint old);
            lpflOldProtect = (ProcessAccessFlags)old;

            return ret;
        }

        /// <summary>
        /// Virtually allocate a memory region to write/read/execute to
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpAddress">Address to allocate to, note: <see cref="IntPtr.Zero"/></param>
        /// <param name="dwSize">Memory region size</param>
        /// <param name="flAllocationType">Type of allocation, usually <see cref="AllocationType.Commit"/> | <seealso cref="AllocationType.Reserve"/></param>
        /// <param name="flProtect">Protection</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        /// <summary>
        /// Virtually allocate a memory region to write/read/execute to
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpAddress">Address to allocate to, note: <see cref="IntPtr.Zero"/></param>
        /// <param name="dwSize">Memory region size</param>
        /// <param name="flAllocationType">Type of allocation, usually <see cref="AllocationType.Commit"/> | <seealso cref="AllocationType.Reserve"/></param>
        /// <param name="flProtect">Protection</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// Virtually free a memory region
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpAddress">Memory region to free</param>
        /// <param name="dwSize">Size to free</param>
        /// <param name="dwFreeType">How to free, note: <see cref="AllocationType.Release"/></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType dwFreeType);

        /// <summary>
        /// Virtually free a memory region
        /// </summary>
        /// <param name="hProcess">The handle for the opened process.</param>
        /// <param name="lpAddress">Memory region to free</param>
        /// <param name="dwSize">Size to free</param>
        /// <param name="dwFreeType">How to free, note: <see cref="AllocationType.Release"/></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        /// <summary>
        /// Move window to position
        /// </summary>
        /// <param name="handle"><see cref="Process.MainWindowHandle"/></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="redraw"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        /// <summary>
        /// Set the window position
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


    }
}
