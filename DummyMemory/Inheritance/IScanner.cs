using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<IntPtr> AoB(string pattern, int start, int end);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        IEnumerable<IntPtr> ModuleAoB(string pattern, ProcessModule module);

    }
}
