using System;

namespace DMemory.Inheritance
{
    /// <summary>
    /// An abstract class that will dispose when the object is collected by <see cref="GC.Collect()"/>
    /// </summary>
    public abstract class GarbageDispose : IDisposable
    {
        /// <summary>
        /// Disposes are garbage collection.
        /// </summary>
        ~GarbageDispose()
        {
            Dispose();
        }

        /// <summary>
        /// Disposal
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
