using System.Collections.Generic;

namespace DMemory
{
    /// <summary>
    /// A class for handling Input from the keyboard and mouse
    /// </summary>
    public static class Input
    {
        const int up = 0;
        const int down = 32768;

        private readonly static Dictionary<int, State> keys = new Dictionary<int, State>();

        /// <summary>
        /// Calls the <see cref="Native.GetAsyncKeyState(int)"/>
        /// </summary>
        /// <param name="key">Virtual Key</param>
        /// <returns></returns>
        public static int GetIntState(int key) => Native.GetAsyncKeyState(key) & 0x8000; //this should be the only native call!

        /// <summary>
        /// Check the state of your key
        /// </summary>
        /// <param name="key">Virtual key code</param>
        /// <param name="last">Last known state, null if first call.</param>
        /// <returns>The current <see cref="State"/> of the key</returns>
        public static State GetState(int key, out State? last)
        {
            State current = (State)GetIntState(key);

            if (!keys.ContainsKey(key))
            {
                last = null;
                keys.Add(key, current);
                return current;
            }

            last = keys[key];

            if (current == State.Down && last == State.Down || current == State.Down && last == State.Held)
                current = State.Held;
            else if (current == State.Up && last == State.Up || current == State.Up && last == State.Released)
                current = State.Released;

            return keys[key] = current;
        }

        /// <summary>
        /// Check the state of your key
        /// </summary>
        /// <param name="key">Virtual key code</param>
        /// <returns>The current <see cref="State"/> of the key</returns>
        public static State GetState(int key) => GetState(key, out _);

        /// <summary>
        /// Checks if the state of your key is Down
        /// </summary>
        /// <param name="key">Virtual key code</param>
        /// <returns>True, if your key's state is down</returns>
        public static bool GetDown(int key) => GetState(key) == State.Down;
        /// <summary>
        /// Checks if the state of your key is Held
        /// </summary>
        /// <param name="key">Virtual key code</param>
        /// <returns>True, if your key's state is held</returns>
        public static bool GetHeld(int key) => GetState(key) == State.Held;
        /// <summary>
        /// Checks if the state of your key is Up
        /// </summary>
        /// <param name="key">Virtual key code</param>
        /// <returns>True, if your key's state is up</returns>
        public static bool GetUp(int key) => GetState(key) == State.Up;

        /// <summary>
        /// Checks if the state of your key is Released
        /// </summary>
        /// <param name="key">Virtual key code</param>
        /// <returns>True, if your key's state is Released</returns>
        public static bool GetReleased(int key) => GetState(key) == State.Released;

        /// <summary>
        /// Check the state of your key
        /// </summary>
        /// <param name="key">Virtual key</param>
        /// <param name="last">Last known state, null if first call.</param>
        /// <returns>The current <see cref="State"/> of the key</returns>
        public static State GetState(KeyBoard key, out State? last) => GetState((int)key, out last);
        /// <summary>
        /// Check the state of your key
        /// </summary>
        /// <param name="key">Virtual key</param>
        /// <returns>The current <see cref="State"/> of the key</returns>
        public static State GetState(KeyBoard key) => GetState((int)key, out _);

        /// <summary>
        /// Checks if the state of your key is Down
        /// </summary>
        /// <param name="key">Virtual key</param>
        /// <returns>True, if your key's state is Down</returns>
        public static bool GetDown(KeyBoard key) => GetState((int)key) == State.Down;
        /// <summary>
        /// Checks if the state of your key is Held
        /// </summary>
        /// <param name="key">Virtual key</param>
        /// <returns>True, if your key's state is Held</returns>
        public static bool GetHeld(KeyBoard key) => GetState((int)key) == State.Held;
        /// <summary>
        /// Checks if the state of your key is Up
        /// </summary>
        /// <param name="key">Virtual key</param>
        /// <returns>True, if your key's state is Up</returns>
        public static bool GetUp(KeyBoard key) => GetState((int)key) == State.Up;

        /// <summary>
        /// Checks if the state of your key is Released
        /// </summary>
        /// <param name="key">Virtual key</param>
        /// <returns>True, if your key's state is Released</returns>
        public static bool GetReleased(KeyBoard key) => GetState((int)key) == State.Released;


        /// <summary>
        /// Represents 3 different states a key can be in.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// The key is completely untouched
            /// </summary>
            Released = up - 10,
            /// <summary>
            /// The key went from Down/Held to released.
            /// </summary>
            Up = up,
            /// <summary>
            /// The key went from Released / Up to pressed
            /// </summary>
            Down = down,
            /// <summary>
            /// The key went from Down to Held
            /// </summary>
            Held = down + 10
        }

    }



}



