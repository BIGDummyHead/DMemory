using System;
using System.Collections.Generic;

namespace DummyMemory.Interfaces
{
    /// <summary>
    /// Used for scanning for address with offsets or patterns
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// Find an address with offsets.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        IntPtr FindDMAAddy(IntPtr address, params int[] offsets);
        /// <summary>
        /// Commit an Area of Bytes scan.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<IntPtr> AoB(string pattern);
    }
}
