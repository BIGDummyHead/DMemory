using System;

namespace DummyMemory.Inheritance
{
    /// <summary>
    /// Garbage Disposal
    /// </summary>
    public class GarbageDispose : IDisposable
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
