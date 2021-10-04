using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace DummyMemory
{
    /// <summary>
    /// Core class for doing basic memory operations
    /// </summary>
    public class Memory : IDisposable
    {
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
        /// Garbage collection
        /// </summary>
        ~Memory()
        {
            Dispose();
        }

        /// <summary>
        /// Open process for reading/writing of memory
        /// </summary>
        /// <param name="proc"></param>
        public Memory(Process proc)
        {
            Open(proc);
        }

        /// <summary>
        /// Get proc by pid
        /// </summary>
        /// <param name="pid">Process ID</param>
        public Memory(int pid)
        {
            Open(Process.GetProcessById(pid));
        }

        /// <summary>
        /// Get proc by name
        /// </summary>
        /// <param name="procName">Process Name</param>
        public Memory(string procName)
        {
            Open(GetProcess(procName));
        }

        bool isDiposed = true;
        /// <summary>
        /// Free up resources.
        /// </summary>
        public virtual void Dispose()
        {
            Proc = null;
            if (Native.CloseHandle(ProcHandle))
                ProcHandle = IntPtr.Zero;

            isDiposed = true;
        }



        /// <summary>
        /// Used by the constructor to set properties. May only be called once disposed.
        /// </summary>
        /// <param name="proc"></param>
        public void Open(Process proc)
        {
            if (!isDiposed)
                return;

            Proc = proc ?? throw new ArgumentNullException(nameof(proc));

            ProcHandle = Native.OpenProcess(0x38, false, proc.Id);

            if (ProcHandle == IntPtr.Zero)
                throw new Exception("Process could not be opened and resulted in Process Handle being null.");

            //reset to false
            isDiposed = false;
        }

        /// <summary>
        /// Read memory from an address
        /// </summary>
        /// <param name="address">The address to read from</param>
        /// <param name="buffer">Buffer, used for size to read. Bytes are written to this.</param>
        /// <returns></returns>
        public bool ReadMemory(IntPtr address, byte[] buffer)
        {
            return Native.ReadProcessMemory(ProcHandle, (long)address, buffer, buffer.Length, out _);
        }

        /// <summary>
        /// Read memory from an address
        /// </summary>
        /// <param name="address">Address to read from</param>
        /// <param name="size">Size of read</param>
        /// <param name="read">Bytes read</param>
        /// <returns></returns>
        public bool ReadMemory(IntPtr address, int size, out byte[] read)
        {
            read = new byte[size];
            return ReadMemory(address, read);
        }

        /// <summary>
        /// Write memory to an address
        /// </summary>
        /// <param name="address">Address to write to</param>
        /// <param name="buffer">Bytes to write to memory region</param>
        /// <returns></returns>
        public bool WriteMemory(IntPtr address, byte[] buffer)
        {
            return Native.WriteProcessMemory(ProcHandle, (long)address, buffer, buffer.Length, out _);
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
            string[] strBytes = pattern.Split(' ');
            int x = 0;
            foreach (byte b in array2check)
            {
                if (strBytes[x] == "?" || strBytes[x] == "??")
                {
                    x++;
                }
                else if (byte.Parse(strBytes[x], NumberStyles.HexNumber) == b)
                {
                    x++;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Scan for an AoB.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        /// <remarks>From Guided Hacking</remarks>
        public IEnumerable<IntPtr> AoB(string pattern)
        {
            ProcessModule mod = Proc.MainModule;
            byte[] moduleMemory = new byte[mod.ModuleMemorySize];
            Native.ReadProcessMemory(ProcHandle, (long)Base, moduleMemory, mod.ModuleMemorySize, out _);

            string[] splitPattern = pattern.Split(' ');

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
                        yield return (IntPtr)((uint)mod.BaseAddress + y);
                    }
                    else
                    {
                        y += splitPattern.Length - (splitPattern.Length / 2);
                    }
                }
            }
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

    }
}



