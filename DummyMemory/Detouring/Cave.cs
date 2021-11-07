using System;
using System.Linq;

namespace DummyMemory.Detouring
{
    /// <summary>
    /// An extension of the <see cref="CaveBase"/> not required by any means, but gives a simplified structure for injection/ejection without worrying about writing fails
    /// </summary>
    public sealed class Cave : CaveBase
    {
        /// <summary>
        /// Allocation settings
        /// </summary>
        public readonly Allocation m_alloc;
        /// <summary>
        /// Address to write to
        /// </summary>
        public readonly IntPtr originalAddr;
        /// <summary>
        /// Original bytes of the <see cref="originalAddr"/>
        /// </summary>
        public readonly byte[] originalBytes;
        //public readonly IModifier m_mod;

        /// <summary>
        /// Bytes we are injecting
        /// </summary>
        public byte[] Injection;

        /// <summary>
        /// The memory block allocated
        /// </summary>
        /// <remarks>Recommend letting the cave allocate this for you. Or using the <see cref="Alloc"/> method</remarks>
        public IntPtr MemoryBlock;

        /// <summary>
        /// Allocation settings for automatically allocating/deallocating the <see cref="MemoryBlock"/>
        /// </summary>
        public bool DeallocateOnInjection = true, AllocateOnInjection = true;

        /// <summary>
        /// The Memory object you provided
        /// </summary>
        public Memory M { get; private set; }

        /// <summary>
        /// True if, <see cref="CaveBase.CreateCodeCave(Memory, IntPtr, IntPtr, int, byte[])"/> was successful on injection.
        /// </summary>
        public bool IsInjected { get; private set; } = false;
        /// <summary>
        /// Opposes <see cref="IsInjected"/>
        /// </summary>
        public bool IsEjected => !IsInjected;

        /// <summary>
        /// Create the setup for a cave.
        /// </summary>
        /// <param name="m">Used for Aob Scan</param>
        /// <param name="aobPattern">Pattern to scan for</param>
        /// <param name="injection">Bytes to write to allocated space</param>
        /// <param name="alloc">Settings for allocation</param>
        public Cave(Memory m, string aobPattern, byte[] injection, Allocation alloc)
        {
            Injection = injection;
            m_alloc = alloc;

            //figure out aob scan for later.
            originalAddr = m.AoB(aobPattern).FirstOrDefault();

            if (originalAddr == IntPtr.Zero)
                throw new Exception("Area of byte scan failed.");

            originalBytes = new byte[alloc.replacementSize];
            m.ReadMemory(originalAddr, originalBytes);
            M = m;
        }

        /// <summary>
        /// Create a setup for a cave, with string formed injection
        /// </summary>
        /// <param name="m">Used for AoB Scan</param>
        /// <param name="aobPattern">Pattern to scan for</param>
        /// <param name="injection">String converted to byte[]</param>
        /// <param name="alloc">Setting for allocation</param>
        public Cave(Memory m, string aobPattern, string injection, Allocation alloc) : this(m, aobPattern, CaveBase.Convert(injection), alloc)
        {

        }


        /// <summary>
        /// Eject cave, only called when <see cref="IsInjected"/> is true
        /// </summary>
        public void Eject()
        {
            if (IsEjected)
                return;

            M.WriteMemory(originalAddr, originalBytes);
            if (DeallocateOnInjection)
                Dealloc();
            IsInjected = false;
        }

        /// <summary>
        /// Inject cave, only called when <see cref="IsInjected"/> is false
        /// </summary>
        public void Inject()
        {
            if (IsInjected)
                return;

            if (AllocateOnInjection)
                Alloc();

            //CreateCodeCave(originalAddr, MemoryBlock, m_alloc.replacementSize, Injection);

            IsInjected = CreateCodeCave(M, originalAddr, MemoryBlock, m_alloc.replacementSize, Injection);
        }

        /// <summary>
        /// Inject/Eject
        /// </summary>
        public void Toggle()
        {
            if (IsInjected)
                Eject();
            else
                Inject();
        }

        /// <summary>
        /// Sets the memory block
        /// </summary>
        public void Alloc()
        {
            MemoryBlock = Native.VirtualAllocEx(M.ProcHandle, IntPtr.Zero, m_alloc.memorySize, Native.AllocationType.Commit | Native.AllocationType.Reserve, Native.MemoryProtection.ExecuteReadWrite);
        }

        /// <summary>
        /// Frees the memory block
        /// </summary>
        public void Dealloc()
        {
            if (MemoryBlock == IntPtr.Zero)
                return;

            Native.VirtualFreeEx(M.ProcHandle, MemoryBlock, 0, Native.AllocationType.Release);
        }

        /// <summary>
        /// Structure for allocation of a code cave 
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
