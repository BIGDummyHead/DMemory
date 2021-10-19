using System;
using System.Diagnostics;

namespace DummyMemory.Interfaces
{
    /// <summary>
    /// An interface for setting up <see cref="IMem"/>
    /// </summary>
    public interface IOpen
    {
        /// <summary>
        /// Open a process here
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="access"></param>
        bool Open(Process proc, Native.ProcessAccessFlags access);
        /// <summary>
        /// Search a process for it's module by name.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        ProcessModule GetModule(string moduleName, StringComparison comparer = StringComparison.Ordinal);
    }
}
