using System;
using System.Diagnostics;
using System.Linq;

namespace DMemory.Detouring
{
    /// <summary>
    /// A child of the <see cref="CaveBase"/> class not required by any means, but gives a simplified structure for injection/ejection without worrying about writing fails.
    /// </summary>
    public sealed class Cave : CaveBase
    {
        /// <summary>
        /// Settings for allocation
        /// </summary>
        public readonly Allocation Allocator;

        /// <summary>
        /// The address we read/write to and from
        /// </summary>
        public readonly IntPtr Address;

        /// <summary>
        /// The byte[] that was read on instance creation.
        /// </summary>
        public readonly byte[] OverwrittenBytes;

        /// <summary>
        /// Bytes we are going to inject
        /// </summary>
        public byte[] Injection;

        /// <summary>
        /// Allocated memory
        /// </summary>
        /// <remarks>Recommend letting the cave allocate this for you. Or using the <see cref="Alloc"/> method</remarks>
        public IntPtr MemoryBlock;

        /// <summary>
        /// Allocation settings for automatically allocating/deallocating the <see cref="MemoryBlock"/>
        /// </summary>
        public bool FreeOnEject = true, AllocateOnInjection = true;

        /// <summary>
        /// The Memory object you provided
        /// </summary>
        public Memory Mem { get; private set; }

        /// <summary>
        /// True if, <see cref="CaveBase.CreateCodeCave(Memory, IntPtr, IntPtr, int, byte[])"/> was successful on injection.
        /// </summary>
        public bool IsInjected { get; private set; } = false;

        /// <summary>
        /// Opposes <see cref="IsInjected"/>
        /// </summary>
        public bool IsEjected => !IsInjected;


        /// <summary>
        /// Create a new Cave with a specific address.
        /// </summary>
        /// <param name="mem">Used for Reading, writing, and allocating.</param>
        /// <param name="addr">A byte[] pattern</param>
        /// <param name="injection">The writable bytes to write to the allocated space.</param>
        /// <param name="alloc">Allocation settings for your cave.</param>
        public Cave(Memory mem, IntPtr addr, byte[] injection, Allocation alloc)
        {
            if (addr == IntPtr.Zero)
                throw new Exception("Original Address may not be null");

            Address = addr;

            Injection = injection;
            Allocator = alloc;

            OverwrittenBytes = new byte[alloc.replacementSize];
            mem.ReadMemory(Address, OverwrittenBytes);
            Mem = mem;
        }

        /// <summary>
        /// Create a new Cave with a specific address.
        /// </summary>
        /// <param name="mem">Used for Reading, writing, and allocating.</param>
        /// <param name="addr">A byte[] pattern</param>
        /// <param name="injection">The writable bytes to write to the allocated space.</param>
        /// <param name="alloc">Allocation settings for your cave.</param>
        public Cave(Memory mem, IntPtr addr, string injection, Allocation alloc) : this(mem, addr, Convert(injection), alloc) { }

        /// <summary>
        /// Create a new Cave with a pattern of Bytes.
        /// </summary>
        /// <param name="scanner">Used for Reading, writing, allocating, and scanning.</param>
        /// <param name="aobPattern">A byte[] pattern</param>
        /// <param name="injection">The writable bytes to write to the allocated space.</param>
        /// <param name="alloc">Allocation settings for your cave.</param>
        /// <param name="module">The module to scan for the <paramref name="aobPattern"/>. If null uses <see cref="Memory.Proc"/>.MainModule by default.</param>
        public Cave(Memory scanner, string aobPattern, byte[] injection, Allocation alloc, ProcessModule module = null) : this(scanner, scanner.ModuleAoB(aobPattern, module ?? scanner.Proc.MainModule).FirstOrDefault(), injection, alloc) { }

        /// <summary>
        /// Create a new Cave with a pattern of Bytes.
        /// </summary>
        /// <param name="scanner">Used for Reading, writing, allocating, and scanning.</param>
        /// <param name="aobPattern">A byte[] pattern in string format</param>
        /// <param name="injection">The writable bytes to write to the allocated space.</param>
        /// <param name="alloc">Allocation settings for your cave.</param>
        /// <param name="module">The module to scan for the <paramref name="aobPattern"/>. If null uses <see cref="Memory.Proc"/>.MainModule by default.</param>
        public Cave(Memory scanner, string aobPattern, string injection, Allocation alloc, ProcessModule module = null) : this(scanner, aobPattern, Convert(injection), alloc, module) { }



        /// <summary>
        /// Writes the original byte[] back to the address. Frees the memory if <see cref="FreeOnEject"/> is true.
        /// </summary>
        public void Eject()
        {
            if (IsEjected)
                return;

            Mem.WriteMemory(Address, OverwrittenBytes);
            if (FreeOnEject)
                Free();
            IsInjected = false;
        }

        /// <summary>
        /// Writes the new set of bytes to the address. Allocates memory if <see cref="AllocateOnInjection"/> is true.
        /// </summary>
        public void Inject()
        {
            if (IsInjected)
                return;

            if (AllocateOnInjection)
                Alloc();

            IsInjected = CreateCodeCave(Mem, Address, MemoryBlock, Allocator.replacementSize, Injection);
        }

        /// <summary>
        /// If <see cref="IsInjected"/> is true, <seealso cref="Eject"/> is called. Vise versa.
        /// </summary>
        public void Toggle()
        {
            if (IsInjected)
                Eject();
            else
                Inject();
        }

        /// <summary>
        /// Allocates the region of memory needed for your code cave. <see cref="Allocation"/>  <seealso cref="Allocator"/>
        /// </summary>
        public void Alloc()
        {
            MemoryBlock = Native.VirtualAllocEx(Mem.Handle, IntPtr.Zero, Allocator.memorySize, Native.AllocationType.MEM_COMMIT | Native.AllocationType.MEM_RESERVE, Native.MemoryProtection.PAGE_EXECUTE_READWRITE);
        }

        /// <summary>
        /// Frees the allocated memory region.
        /// </summary>
        public void Free()
        {
            if (MemoryBlock == IntPtr.Zero)
                return;

            Native.VirtualFreeEx(Mem.Handle, MemoryBlock, 0, Native.FreeType.MEM_RELEASE);
        }

        /// <summary>
        /// Specific settings for allocation of a code cave.
        /// </summary>
        public struct Allocation
        {
            /// <summary>
            /// How many bytes to replace; at least 5
            /// </summary>
            public int replacementSize;

            /// <summary>
            /// The region size to allocate
            /// </summary>
            public int memorySize;
        }

    }
}
