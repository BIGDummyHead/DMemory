using System;

namespace DMemory.Interfaces
{
    /// <summary>
    /// Generic methods for reading/writing memory
    /// </summary>
    public interface IGenericMem : IMem
    {
        /// <summary>
        /// Generically read memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        T Read<T>(IntPtr ptr, params int[] offsets);
        /// <summary>
        /// Generically write memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="val"></param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        bool Write<T>(IntPtr ptr, T val, params int[] offsets);
        /// <summary>
        /// Turn type into <see cref="byte"/>[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        byte[] GetBytes<T>(T val);
    }
}
