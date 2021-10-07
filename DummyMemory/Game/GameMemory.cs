using System;
using System.Diagnostics;

namespace DummyMemory.Game
{
    /// <summary>
    /// Used to help read some structs and classes from the <see cref="DummyMemory.Game"/> namespace
    /// </summary>
    public class GameMemory : Memory
    {
        /// <summary>
        /// Init by Process
        /// </summary>
        /// <param name="proc"></param>
        public GameMemory(Process proc) : base(proc)
        {

        }

        /// <summary>
        /// Init by name
        /// </summary>
        /// <param name="procName"></param>
        public GameMemory(string procName) : base(procName)
        {

        }

        /// <summary>
        /// Init by ProcID
        /// </summary>
        /// <param name="pid"></param>
        public GameMemory(int pid) : base(pid)
        {

        }

        /// <summary>
        /// Get a matrix object from a Memory address
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="buffer">Byte[] buffer</param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool ReadMatrix(IntPtr address, byte[] buffer, out Matrix matrix)
        {
            bool read = ReadMemory(address, buffer);

            if (!read)
            {
                matrix = null;
                return false;
            }

            matrix = GetMatrix(buffer);
            return true;
        }

        /// <summary>
        /// Get a Vector3 struct from a Memory address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool ReadVector3(IntPtr address, out Vector3 vector)
        {
            byte[] buffer = new byte[12];
            bool read = ReadMemory(address, buffer);

            if (!read)
            {
                vector = Vector3.Zero;
                return false;
            }

            vector = GetVector3(buffer);
            return true;
        }

        /// <summary>
        /// Get a Vector3 struct from a Memory address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool ReadVector2(IntPtr address, out Vector2 vector)
        {
            byte[] buffer = new byte[8];
            bool read = ReadMemory(address, buffer);

            if (!read)
            {
                vector = Vector2.Zero;
                return false;
            }

            vector = GetVector2(buffer);
            return true;
        }


        /// <summary>
        /// Write a vector3 to a memory address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public bool WriteVector3(IntPtr address, Vector3 vec)
        {
            return WriteMemory(address, BitConverter.GetBytes(vec.x))
            && WriteMemory(address + 4, BitConverter.GetBytes(vec.y))
            && WriteMemory(address + 8, BitConverter.GetBytes(vec.z));
        }

        /// <summary>
        /// Write a vector3 to a memory address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public bool WriteVector2(IntPtr address, Vector3 vec)
        {
            return WriteMemory(address, BitConverter.GetBytes(vec.x))
            && WriteMemory(address + 4, BitConverter.GetBytes(vec.y));
        }


        private Vector2 GetVector2(byte[] buffer)
        {
            Vector2 vec = new Vector2
            {
                x = BitConverter.ToSingle(buffer, (0 * 4)),
                y = BitConverter.ToSingle(buffer, (1 * 4)),
            };
            return vec;
        }

        private Vector3 GetVector3(byte[] buffer)
        {
            Vector3 vec = new Vector3
            {
                x = BitConverter.ToSingle(buffer, (0 * 4)),
                y = BitConverter.ToSingle(buffer, (1 * 4)),
                z = BitConverter.ToSingle(buffer, (2 * 4))
            };
            return vec;
        }

        private Matrix GetMatrix(byte[] buffer)
        {
            return new Matrix
            {
                m11 = BitConverter.ToSingle(buffer, (0 * 4)),
                m12 = BitConverter.ToSingle(buffer, (1 * 4)),
                m13 = BitConverter.ToSingle(buffer, (2 * 4)),
                m14 = BitConverter.ToSingle(buffer, (3 * 4)),

                m21 = BitConverter.ToSingle(buffer, (4 * 4)),
                m22 = BitConverter.ToSingle(buffer, (5 * 4)),
                m23 = BitConverter.ToSingle(buffer, (6 * 4)),
                m24 = BitConverter.ToSingle(buffer, (7 * 4)),

                m31 = BitConverter.ToSingle(buffer, (8 * 4)),
                m32 = BitConverter.ToSingle(buffer, (9 * 4)),
                m33 = BitConverter.ToSingle(buffer, (10 * 4)),
                m34 = BitConverter.ToSingle(buffer, (11 * 4)),

                m41 = BitConverter.ToSingle(buffer, (12 * 4)),
                m42 = BitConverter.ToSingle(buffer, (13 * 4)),
                m43 = BitConverter.ToSingle(buffer, (14 * 4)),
                m44 = BitConverter.ToSingle(buffer, (15 * 4))
            };
        }
    }
}
