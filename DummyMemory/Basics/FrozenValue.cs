using DMemory.Debugging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DMemory
{
    /// <summary>
    /// A value that can be (un)frozen
    /// </summary>
    public sealed class FrozenValue : Inheritance.GarbageDispose
    {
        /// <summary>
        /// The address to write to.
        /// </summary>
        public IntPtr WritingTo { get; private set; }
        /// <summary>
        /// Value we are writing to <see cref="WritingTo"/>
        /// </summary>
        public byte[] Writing { get; private set; }
        /// <summary>
        /// The memory instance used.
        /// </summary>
        public readonly Memory m;
        /// <summary>
        /// The value read.
        /// </summary>
        public byte[] OriginalValue { get; private set; }

        private bool freeze_backer = false;

        /// <summary>
        /// Freeze the value?
        /// </summary>
        public bool Freeze
        {
            get => freeze_backer;

            set
            {
                Debug.LogWarning($"{string.Format("0x{0:X8}", this.WritingTo.ToInt64())} Frozen Status: {value}");

                freeze_backer = value;
            }
        }

        private static TimeSpan debug_span = TimeSpan.FromMilliseconds(125);
        private static readonly List<FrozenValue> frozen = new List<FrozenValue>();

        private readonly static Timer global_timer = new Timer(delegate (object state)
        {
            Debug.Log($"FrozenValue Tick: {debug_span}");
            for (int i = 0; i < frozen.Count; i++)
            {
                FrozenValue fv = frozen[i];

                if (fv.isDisposed)
                {
                    Debug.LogWarning("Removed a Frozen Value because it was disposed...");
                    frozen.RemoveAt(i);
                    continue;
                }
                else if (fv.Freeze)
                {
                    fv.m.WriteMemory(fv.WritingTo, fv.Writing);
                }
            }

        }, null, TimeSpan.Zero, debug_span);

        /// <summary>
        /// Updates how long it takes for the Timer Callback to be invoked.
        /// </summary>
        /// <param name="time"></param>
        public static void UpdateTimer(TimeSpan time)
        {
            debug_span = time;
            global_timer.Change(TimeSpan.Zero, time);
        }

        internal FrozenValue(Memory m, IntPtr addr, byte[] rewritingValue)
        {
            this.m = m;
            Writing = rewritingValue;
            WritingTo = addr;

            OriginalValue = new byte[rewritingValue.Length];

            m.ReadMemory(addr, OriginalValue); //OriginalValue is set.

            frozen.Add(this);
        }


        /// <summary>
        /// Rewrites the value back to the <see cref="WritingTo"/> when disposed.
        /// </summary>
        public bool rewriteOnDispose = false;

        private bool isDisposed = false;
        /// <summary>
        /// Stops the value from being frozen and removes it from the updating list.
        /// </summary>
        public override void Dispose()
        {
            if (isDisposed)
                return;

            Freeze = false;

            if (rewriteOnDispose)
                ReWrite();

            isDisposed = true;

        }

        /// <summary>
        /// ReWrite the original value that belonged to <see cref="WritingTo"/>
        /// </summary>
        /// <param name="overrideFreeze">Sets <see cref="Freeze"/> to false.</param>
        /// <remarks>Will only write if <see cref="Freeze"/> is false.</remarks>
        public void ReWrite(bool overrideFreeze = false)
        {
            if (overrideFreeze)
                Freeze = false;

            if (!Freeze)
                m.WriteMemory(WritingTo, OriginalValue);
        }

        /// <summary>
        /// Change the writing value.
        /// </summary>
        /// <param name="value"></param>
        public void ChangeValue(byte[] value)
        {
            bool ori = Freeze;
            Freeze = false;
            lock (Writing)
            {
                Writing = value;
            }
            Freeze = ori;
        }
    }
}
