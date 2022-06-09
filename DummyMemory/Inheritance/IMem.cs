using System;

namespace DMemory.Interfaces
{
    /// <summary>
    /// Methods for reading and writing byte[] to addresses
    /// </summary>
    public interface IMem : IDisposable, IOpen
    {
        /// <summary>
        /// Read a memory address
        /// </summary>
        /// <param name="ptr">Address</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="offsets">Offsets of address</param>
        /// <returns></returns>
        bool ReadMemory(IntPtr ptr, byte[] buffer, params int[] offsets);
        /// <summary>
        /// Write to a memory address
        /// </summary>
        /// <param name="ptr">Address</param>
        /// <param name="vs"></param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        bool WriteMemory(IntPtr ptr, byte[] vs, params int[] offsets);
    }
}
