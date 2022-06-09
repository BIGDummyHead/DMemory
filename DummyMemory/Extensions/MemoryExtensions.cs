using System;
using System.Diagnostics;

namespace DMemory
{
    /// <summary>
    /// Extensions for <see cref="Memory"/>
    /// </summary>
    public static class MemoryExtensions
    {
        /// <summary>
        /// Shorthand for creating a new <see cref="Injector"/> and calling the <seealso cref="Injector.LoadLibraryA"/> method.
        /// </summary>
        /// <param name="m">Memory instance</param>
        /// <param name="path">Dll path</param>
        /// <param name="throw">Throw on error?</param>
        /// <returns>True if successful.</returns>
        public static bool FastInject(this Memory m, string path, bool @throw = false) => new Injector(m.Handle, path, @throw).LoadLibraryA();

        /// <summary>
        /// Checks if a <see cref="ProcessModuleCollection"/> by comparing the names of the two <seealso cref="ProcessModule"/>s
        /// </summary>
        /// <param name="pmc">Collection of <see cref="ProcessModule"/></param>
        /// <param name="pm">A comparing <see cref="ProcessModule"/></param>
        /// <param name="cmp">Extensional checks to use on each <see cref="ProcessModule"/> in the <paramref name="pmc"/></param>
        /// <returns>True, if the Name and Extensional checks were true.</returns>
        public static bool HasModule(this ProcessModuleCollection pmc, ProcessModule pm, Func<ProcessModule, ProcessModule, bool> cmp = null)
        {
            foreach (ProcessModule checkPM in pmc)
            {
                if (checkPM.ModuleName == pm.ModuleName)
                {
                    if (cmp != null && cmp(checkPM, pm))
                        return true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Freeze a value to an address.
        /// </summary>
        /// <param name="m">Instance of memory</param>
        /// <param name="addr">Address to freeze</param>
        /// <param name="write">Value to write to the address</param>
        /// <param name="offsets">Any offsets</param>
        /// <returns>A new frozen value</returns>
        public static FrozenValue Freeze(this Memory m, IntPtr addr, byte[] write, params int[] offsets)
        {
            addr = m.FindDMAAddy(addr, offsets);
            return new FrozenValue(m, addr, write)
            {
                Freeze = true
            };
        }

        /// <summary>
        /// Generically freezes a value to an address.
        /// </summary>
        /// <typeparam name="T">A limited Marshaled T type</typeparam>
        /// <param name="m">Instance of memory</param>
        /// <param name="addr">Address to freeze</param>
        /// <param name="write">Value to write to the address</param>
        /// <param name="offsets">Any offsets</param>
        /// <returns>A new frozen value</returns>
        public static FrozenValue Freeze<T>(this Memory m, IntPtr addr, T write, params int[] offsets) => Freeze(m, addr, m.GetBytes(write), offsets);

        /// <summary>
        /// Generically change the writing value to a <see cref="FrozenValue"/>
        /// </summary>
        /// <typeparam name="T">A limited Marshaled T type</typeparam>
        /// <param name="f">FrozenValue instance</param>
        /// <param name="newVal">Value to now write.</param>
        public static void ChangeValue<T>(this FrozenValue f, T newVal) => f.ChangeValue(f.m.GetBytes(newVal));

        /// <summary>
        /// Refreshes and checks if the process has closed.
        /// </summary>
        /// <param name="proc">Process to check</param>
        /// <returns></returns>
        public static bool IsOpen(this Process proc)
        {
            if (proc == null)
                return false;

            proc.Refresh();

            return !proc.HasExited;
        }

        /// <summary>
        /// Create an injector based off a Dll file!
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dll">Path to a dll file</param>
        /// <param name="throw">Throw on error?</param>
        public static Injector NewInj(this Memory m, string dll, bool @throw = false) => new Injector(m.Handle, dll, @throw);
    }
}
