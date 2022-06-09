using DMemory.Inheritance;
using DMemory.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Debug = DMemory.Debugging.Debug;

namespace DMemory
{
    /// <summary>
    /// Core class for doing basic memory operations
    /// </summary>
    public class Memory : GarbageDispose, IGenericMem
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
                {
                    Debug.Log($"Process found: {proc.ProcessName} | {proc.Id}");
                    return proc;
                }
            }

            return null;
        }

        private static bool m_admin_checked = false; //has RefreshAdmin been called?
        private static bool backing_admin = false; //backing field for Admin

        /// <summary>
        /// Checks if the Current Program is in Admin mode
        /// </summary>
        /// <remarks>If false, may make some methods invalid for use</remarks>
        public static bool Admin
        {
            get
            {
                if (!m_admin_checked)
                    return RefreshAdmin();

                return backing_admin;
            }
        }

        /// <summary>
        /// Rechecks if the application is in Administrator mode
        /// </summary>
        /// <returns></returns>
        public static bool RefreshAdmin()
        {
            using (WindowsIdentity id = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal princ = new WindowsPrincipal(id);

                m_admin_checked = true;
                backing_admin = princ.IsInRole(WindowsBuiltInRole.Administrator);


                Debug.Log($"Admin status refresh: {backing_admin}");

                return backing_admin;
            }
        }



        /// <summary>
        /// Refreshes and checks if the process has closed.
        /// </summary>
        /// <param name="pid">Process ID to find and check</param>
        /// <returns></returns>
        public static bool IsOpen(int pid)
        {
            return Process.GetProcessById(pid).IsOpen();
        }

        /// <summary>
        /// Refreshes and checks if the process has closed.
        /// </summary>
        /// <param name="procName">Process to find and check</param>
        /// <returns></returns>
        public static bool IsOpen(string procName)
        {
            Process[] procs = Process.GetProcessesByName(procName);
            if (procs.Length <= 0)
                return false;

            return procs[0].IsOpen();
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
        private IntPtr m_handle;
        #endregion

        #region Constructors
        /// <summary>
        /// VM OPERATION | VM WRITE | VM READ
        /// </summary>
        /// <remarks>0x38</remarks>
        public const Native.ProcessAccessFlags Access = Native.ProcessAccessFlags.PROCESS_ALL_ACCESS;

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
        /// Handle to the process is closed.
        /// </summary>
        public override void Dispose()
        {
            Proc = null;
            if (Native.CloseHandle(m_handle))
                m_handle = IntPtr.Zero;

            isDiposed = true;

            Debug.LogWarning("Memory Object disposed!");
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

            Proc = proc ?? throw new Exception($"'{nameof(Proc)}' was null.", new Exception("Make sure that the process is running."));

            m_handle = Native.OpenProcess((uint)access, false, proc.Id);

            if (m_handle == IntPtr.Zero)
                throw new Exception("Process could not be opened and resulted in Process Handle being null.");

            Debug.Log($"{Proc.ProcessName}'s Handle was successfully opened and is now set to this instance of {nameof(Memory)}");
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

            return Native.ReadProcessMemory(m_handle, addr, buffer, (UIntPtr)buffer.Length, out _);
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
            return Native.WriteProcessMemory(m_handle, address, buffer, buffer.Length, out _);
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
        public static byte[] MarshalBytes<T>(T val)
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
        public static T Convert<T>(byte[] conv)
        {
            GCHandle gHandle = GCHandle.Alloc(conv, GCHandleType.Pinned);
            T data = (T)Marshal.PtrToStructure(gHandle.AddrOfPinnedObject(), typeof(T));
            gHandle.Free();

            return data;
        }

        /// <summary>
        /// Turns a type into writeable bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">Turn type.</param>
        /// <returns>Writeable bytes</returns>
        /// <remarks>This maybe invalid for some types that the <see cref="Marshal"/> cannot handle.</remarks>
        public byte[] GetBytes<T>(T val) => MarshalBytes(val);


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
                Native.ReadProcessMemory(Handle, address, buffer, new UIntPtr(4), out _);
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
                bool isWildcard = s == "?" || s == "??";

                //if the pattern byte is a wild card we know the parse will return false so we just continue to the next iteration
                if (isWildcard)
                    continue;

                //we know that the pattern byte is not a wild card so we will check for a parse here.
                if (!byte.TryParse(s, NumberStyles.HexNumber, null, out byte res))
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
        public IEnumerable<IntPtr> AoB(string pattern, int start, int end)
        {
            Debug.Log($"Area of Bytes scanned started with a pattern of '{pattern}'. Address start/end: 0x{start:X8} - 0x{end:X8}");
            string[] splitPattern = pattern.Split(' ');

            byte[] moduleMemory = new byte[end];

            Native.ReadProcessMemory(m_handle, (IntPtr)start, moduleMemory, (UIntPtr)(uint)end, out _);

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
                        IntPtr rPtr = (IntPtr)(start + y);
                        Debug.Log($"0x{rPtr.ToInt64():X8} found in Area of Bytes Scan!");
                        yield return rPtr;
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
            if (!Proc.Modules.HasModule(module))
                return Array.Empty<IntPtr>();

            return AoB(pattern, (int)module.BaseAddress, (int)module.BaseAddress + module.ModuleMemorySize);
        }

        /// <summary>
        /// Uses the <see cref="ModuleAoB(string, ProcessModule)"/> and passes in the <see cref="Process.MainModule"/>
        /// </summary>
        /// <param name="pattern">byte[] pattern</param>
        /// <returns>Addresses that match the byte pattern</returns>
        public IEnumerable<IntPtr> BaseAoB(string pattern)
        {
            var X = ModuleAoB(pattern, Proc.MainModule);
            return X;
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
                {
                    Debug.Log($"Found module {moduleName}");
                    return module;
                }

            return null;
        }

        /// <summary>
        /// Shorthand for <see cref="GetModule(string, StringComparison)"/>.BaseAddress
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="comparer">How to compare module names</param>
        /// <returns></returns>
        public IntPtr GetBaseAddress(string moduleName, StringComparison comparer = StringComparison.Ordinal)
        {
            return GetModule(moduleName, comparer).BaseAddress;
        }
    }
}



