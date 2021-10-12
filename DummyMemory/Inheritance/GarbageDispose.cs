using System;

namespace DummyMemory.Inheritance
{
    /// <summary>
    /// A class that you can inherit that will automatically dispose when the <see cref="GC.Collect()"/> is called
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
