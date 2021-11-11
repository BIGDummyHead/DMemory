using DummyMemory.Inheritance;
using DummyMemory.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace DummyMemory
{
    /// <summary>
    /// Core class for doing basic memory operations
    /// </summary>
    public class Memory : GarbageDispose, IGenericMem, IScanner
    {
        #region Static
        /// <summary>
        /// Get a process under certain requirements using a delegate.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Process GetProcess(string name, Func<Process, bool> predicate = null)
        {
            var procs = Process.GetProcessesByName(name);

            if (procs.Length < 1)
                return null;
            else if (predicate == null)
                return procs[0];

            foreach (Process proc in procs)
            {
                if (predicate.Invoke(proc))
                    return proc;
            }

            return null;
        }

        /// <summary>
        /// Checks if the Current Program is in Admin mode
        /// </summary>
        /// <remarks>If false, may make some methods invalid for use</remarks>
        public static bool Admin
        {
            get
            {
                using (WindowsIdentity id = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal princ = new WindowsPrincipal(id);
                    return princ.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
        }

        /// <summary>
        /// Refreshes and checks if the process has closed.
        /// </summary>
        /// <param name="proc">Process to check</param>
        /// <returns></returns>
        public static bool IsOpen(Process proc)
        {
            if (proc == null)
                return false;

            proc.Refresh();

            return !proc.HasExited;
        }

        /// <summary>
        /// Refreshes and checks if the process has closed.
        /// </summary>
        /// <param name="pid">Process ID to find and check</param>
        /// <returns></returns>
        public static bool IsOpen(int pid)
        {
            return IsOpen(Process.GetProcessById(pid));
        }

        /// <summary>
        /// Refreshes and checks if the process has closed.
        /// </summary>
        /// <param name="procName">Process to find and check</param>
        /// <returns></returns>
        public static bool IsOpen(string procName)
        {
            Process[] procs;
            if ((procs = Process.GetProcessesByName(procName)).Length == 0)
                return false;

            return IsOpen(procs[0]);
        }

        /// <summary>
        /// Inject dll into running process
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static InjectionStatus Inject(Process proc, string dllPath)
        {
            if (!File.Exists(dllPath))
                return InjectionStatus.DllDoesNotExist;

            if (!Admin)
                return InjectionStatus.NotAdmin;

            IntPtr hProc = Native.OpenProcess(proc, Native.ProcessAccessFlags.All);

            if (hProc == IntPtr.Zero)
                return InjectionStatus.BadPointer;

            int size = (dllPath.Length + 1) * Marshal.SizeOf(typeof(char));

            IntPtr alloc = Native.VirtualAllocEx(hProc, IntPtr.Zero, size, Native.AllocationType.Commit | Native.AllocationType.Reserve, Native.MemoryProtection.ReadWrite);

            if (alloc == IntPtr.Zero)
                return InjectionStatus.BadPointer;

            Native.WriteProcessMemory(hProc, alloc, Encoding.Default.GetBytes(dllPath), size, out _);

            IntPtr module = Native.GetProcAddress(Native.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (module == IntPtr.Zero)
                return InjectionStatus.BadPointer;

            //at this point forward all is injected
            IntPtr hThread = Native.CreateRemoteThread(hProc, IntPtr.Zero, 0, module, alloc, 0, out _);

            //2 close fails occured
            if (!Native.CloseHandle(hProc))
                return InjectionStatus.CloseFail_Injected;

            //hThread == IntPtr.Zero
            if (hThread == IntPtr.Zero)
                return InjectionStatus.CloseFail_Injected | InjectionStatus.BadPointer;

            //close final handle
            if (!Native.CloseHandle(hThread))
                return InjectionStatus.CloseFail_Injected;

            return InjectionStatus.Injected;
        }
        #endregion

        #region Props
        /// <summary>
        /// Process 
        /// </summary>
        public Process Proc { get; private set; }
        /// <summary>
        /// Main module base address
        /// </summary>
        public IntPtr Base => Proc.MainModule.BaseAddress;
        /// <summary>
        /// Process Handle, used for most basic operations
        /// </summary>
        public IntPtr Handle => Proc.Handle;

        /// <summary>
        /// The handle when the process is opened
        /// </summary>
        public IntPtr ProcHandle { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// VM OPERATION | VM WRITE | VM READ
        /// </summary>
        /// <remarks>0x38</remarks>
        public const Native.ProcessAccessFlags Access = Native.ProcessAccessFlags.VirtualMemoryOperation | Native.ProcessAccessFlags.VirtualMemoryWrite | Native.ProcessAccessFlags.VirtualMemoryRead;

        /// <summary>
        /// Open process for reading/writing of memory
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="access"></param>
        public Memory(Process proc, Native.ProcessAccessFlags access = Access)
        {
            Open(proc, access);
        }

        /// <summary>
        /// Get proc by pid
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="access"></param>
        public Memory(int pid, Native.ProcessAccessFlags access = Access)
        {
            Open(Process.GetProcessById(pid), access);
        }

        /// <summary>
        /// Get proc by name
        /// </summary>
        /// <param name="procName">Process Name</param>
        /// <param name="access"></param>
        public Memory(string procName, Native.ProcessAccessFlags access = Access)
        {
            Open(GetProcess(procName), access);
        }


        bool isDiposed = true;
        /// <summary>
        /// Free up resources.
        /// </summary>
        public override void Dispose()
        {
            Proc = null;
            if (Native.CloseHandle(ProcHandle))
                ProcHandle = IntPtr.Zero;

            isDiposed = true;
        }
        #endregion


        /// <summary>
        /// Used by the constructor to set properties. May only be called when disposed.
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="access"></param>
        public bool Open(Process proc, Native.ProcessAccessFlags access)
        {
            if (!isDiposed)
                return false;

            Proc = proc ?? throw new ArgumentNullException(nameof(proc));

            ProcHandle = Native.OpenProcess((uint)access, false, proc.Id);

            if (ProcHandle == IntPtr.Zero)
                throw new Exception("Process could not be opened and resulted in Process Handle being null.");

            //reset to false
            isDiposed = false;
            return true;
        }

        /// <summary>
        /// Read memory from an address with offsets.
        /// </summary>
        /// <param name="address">The address to read from</param>
        /// <param name="buffer">Buffer, used for size to read. Bytes are written to this.</param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        public bool ReadMemory(IntPtr address, byte[] buffer, params int[] offsets)
        {
            IntPtr addr = FindDMAAddy(address, offsets);

            return Native.ReadProcessMemory(ProcHandle, (long)addr, buffer, buffer.Length, out _);
        }

        /// <summary>
        /// Write memory to an address
        /// </summary>
        /// <param name="address">Address to write to</param>
        /// <param name="buffer">Bytes to write to memory region</param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        public bool WriteMemory(IntPtr address, byte[] buffer, params int[] offsets)
        {
            address = FindDMAAddy(address, offsets);
            return Native.WriteProcessMemory(ProcHandle, address, buffer, buffer.Length, out _);
        }

        /// <summary>
        /// Read a memory address and convert
        /// </summary>
        /// <typeparam name="T">Any struct</typeparam>
        /// <param name="address">Address</param>
        /// <param name="offsets">Address offsets.</param>
        /// <returns>The value of the read address</returns>
        public T Read<T>(IntPtr address, params int[] offsets)
        {
            //create byte array with size of type
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];

            ReadMemory(address, buffer, offsets);

            return Convert<T>(buffer);
        }

        /// <summary>
        /// Write a Generic T type to an Address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">Address to write to</param>
        /// <param name="val">Values to write</param>
        /// <param name="offsets">Offsets if any.</param>
        /// <returns>True if memory was sucessfully written</returns>
        /// <remarks>Note: Can be considered slow when writing trivial things like <see cref="int"/></remarks>
        public bool Write<T>(IntPtr address, T val, params int[] offsets)
        {
            return WriteMemory(address, GetBytes(val), offsets);
        }

        /// <summary>
        /// Turns a type into writeable bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">Turn type.</param>
        /// <returns>Writeable bytes</returns>
        /// <remarks>This maybe invalid for some types that the <see cref="Marshal"/> cannot handle.</remarks>
        public byte[] GetBytes<T>(T val)
        {
            int size = Marshal.SizeOf<T>();
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(val, ptr, false);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            return bytes;
        }

        /// <summary>
        /// Converts a byte[] to a <typeparamref name="T"/> but has limitations as does the <see cref="Read{T}(IntPtr, int[])"/> method
        /// </summary>
        /// <typeparam name="T">Conversion type</typeparam>
        /// <param name="conv">Bytes to T</param>
        /// <returns></returns>
        public T Convert<T>(byte[] conv)
        {
            GCHandle gHandle = GCHandle.Alloc(conv, GCHandleType.Pinned);
            T data = (T)Marshal.PtrToStructure(gHandle.AddrOfPinnedObject(), typeof(T));
            gHandle.Free();

            return data;
        }

        /// <summary>
        /// Find a memory address that has offsets.
        /// </summary>
        /// <param name="address">base address</param>
        /// <param name="offsets">offsets of address</param>
        /// <returns></returns>
        public IntPtr FindDMAAddy(IntPtr address, params int[] offsets)
        {
            var buffer = new byte[IntPtr.Size];
            foreach (int i in offsets)
            {

                Native.ReadProcessMemory(ProcHandle, (ulong)address, buffer, 4UL, out address);

                address = (IntPtr.Size == 4)
                ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                : address = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);

            }
            return address;
        }

        private bool CheckPattern(string pattern, byte[] array2check)
        {
            string[] eachByte = pattern.Split(' ');
            for (int i = 0; i < array2check.Length; i++)
            {
                byte b = array2check[i];
                string s = eachByte[i];

                //is the pattern wild? save this for other measures.
                bool isWildcard = s != "?" || s != "??";

                //if the pattern byte is a wild card we know the parse will return false so we just continue to the next iteration
                if (isWildcard)
                    continue;

                //we know that the pattern byte is not a wild card so we will check for a parse here.
                if (!byte.TryParse(s, out byte res))
                    return false;

                //compare the byte and the parsed byte
                if (res != b)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Scan for a unique area of bytes, this is not reserved for one module but the process
        /// </summary>
        /// <param name="pattern">byte[] pattern</param>
        /// <param name="start">Start of the module</param>
        /// <param name="end">End of the module, set to <see cref="int.MaxValue"/></param>
        /// <returns>Addresses that match the byte pattern</returns>
        /// <remarks>Can take a long time when scanning the entire process.</remarks>
        public IEnumerable<IntPtr> AoB(string pattern, int start = 0, int end = int.MaxValue)
        {
            string[] splitPattern = pattern.Split(' ');

            byte[] moduleMemory = new byte[end];

            Native.ReadProcessMemory(ProcHandle, start, moduleMemory, end, out _);

            //module memory is byte[]
            for (int y = 0; y < moduleMemory.Length; y++)
            {
                if (moduleMemory[y] == byte.Parse(splitPattern[0], NumberStyles.HexNumber))
                {
                    byte[] checkArray = new byte[splitPattern.Length];
                    for (int x = 0; x < splitPattern.Length; x++)
                    {
                        checkArray[x] = moduleMemory[y + x];
                    }
                    if (CheckPattern(pattern, checkArray))
                    {
                        yield return (IntPtr)((uint)start + y);
                    }
                    else
                    {
                        y += splitPattern.Length - (splitPattern.Length / 2);
                    }
                }
            }
        }

        /// <summary>
        /// Scan for an AoB using start as <see cref="ProcessModule.BaseAddress"/> and using end as <see cref="ProcessModule.ModuleMemorySize"/>
        /// </summary>
        /// <param name="pattern">byte[] pattern</param>
        /// <param name="module">Process module that is included in the <see cref="Proc"/></param>
        /// <returns>Addresses that match the byte pattern</returns>
        public IEnumerable<IntPtr> ModuleAoB(string pattern, ProcessModule module)
        {
            if (!Proc.Modules.Contains(module))
                return Array.Empty<IntPtr>();

            return AoB(pattern, (int)module.BaseAddress, module.ModuleMemorySize);
        }

        /// <summary>
        /// Uses the <see cref="ModuleAoB(string, ProcessModule)"/> and passes in the <see cref="Process.MainModule"/>
        /// </summary>
        /// <param name="pattern">byte[] pattern</param>
        /// <returns>Addresses that match the byte pattern</returns>
        public IEnumerable<IntPtr> BaseAoB(string pattern)
        {
            return ModuleAoB(pattern, Proc.MainModule);
        }

        

        /// <summary>
        /// Get a Process Module based off name
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <param name="comparer">How to compare module names</param>
        /// <returns>A process module</returns>
        public ProcessModule GetModule(string moduleName, StringComparison comparer = StringComparison.Ordinal)
        {
            foreach (ProcessModule module in Proc.Modules)
                if (module.ModuleName.Equals(moduleName, comparer))
                    return module;

            return null;
        }

        /// <summary>
        /// Get a module base address from the name
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="comparer">How to compare module names</param>
        /// <returns></returns>
        public IntPtr GetBaseAddress(string moduleName, StringComparison comparer = StringComparison.Ordinal)
        {
            return GetModule(moduleName, comparer).BaseAddress;
        }



        /// <summary>
        /// Inject dll into running process
        /// </summary>
        /// <param name="dllPath">Dll to inject</param>
        /// <returns></returns>
        public InjectionStatus Inject(string dllPath)
        {
            return Inject(Proc, dllPath);
        }


        /// <summary>
        /// Status of Injection
        /// </summary>
        [Flags]
        public enum InjectionStatus : int
        {
            /// <summary>
            /// Dll does not exist
            /// </summary>
            DllDoesNotExist = -2,
            /// <summary>
            /// You are not an admin
            /// </summary>
            NotAdmin = -1,
            /// <summary>
            /// Pointer was equal to 0
            /// </summary>
            BadPointer = 1,
            /// <summary>
            /// Injection success!
            /// </summary>
            Injected = 10,
            /// <summary>
            /// Injection success, Handle(s) failed to close
            /// </summary>
            CloseFail_Injected = 20
        }


    }
}



