using System;
using System.Collections.Generic;

namespace DummyMemory.Detouring
{
    /// <summary>
    /// Methods for writing a Code Cave
    /// </summary>
    public class CaveBase
    {
        /// <summary>
        /// Trys to convert string to byte[]
        /// </summary>
        /// <param name="byteFormatted">A string formatted byte string</param>
        /// <param name="read"></param>
        /// <returns></returns>
        public static bool TryConvert(string byteFormatted, out byte[] read)
        {
            List<byte> bysRead = new List<byte>();

            foreach (string word in GetWords(byteFormatted))
            {
                if (long.TryParse(word, System.Globalization.NumberStyles.HexNumber, null, out long hex))
                {
                    bysRead.Add((byte)hex);
                }
                else
                {
                    read = Array.Empty<byte>();
                    return false;
                }
            }

            read = bysRead.ToArray();
            return true;
        }

        /// <summary>
        /// Converts string to byte[]
        /// </summary>
        /// <param name="byteFormatted"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] Convert(string byteFormatted)
        {
            if (!TryConvert(byteFormatted, out byte[] read))
                throw new Exception("Failed to convert!");
            else
                return read;
        }

        private static string[] GetWords(string sentence, char sep = ' ')
        {
            List<string> wrds = new List<string>();

            string _res = string.Empty;

            foreach (char a in sentence)
            {
                if (a == sep)
                {
                    wrds.Add(_res);
                    _res = string.Empty;
                }
                else
                    _res += a;
            }

            if (_res != string.Empty)
                wrds.Add(_res);

            return wrds.ToArray();
        }

        /// <summary>
        /// A static instance created in the static constructor.
        /// </summary>
        /// <remarks>Used for static methods and virtual methods.</remarks>
        public static CaveBase Creator { get; private set; }
        static CaveBase()
        {
            Creator = new CaveBase();
        }

        /// <summary>
        /// Get the bytes to jump from injection to allocated
        /// </summary>
        /// <param name="codePtr">Injection address</param>
        /// <param name="jmpAlloc">Allocated memory</param>
        /// <param name="replacementSize">Size to replace</param>
        /// <returns></returns>
        public virtual byte[] GetJump(IntPtr codePtr, IntPtr jmpAlloc, int replacementSize)
        {
            if (replacementSize < 5)
                throw new Exception("Replacement Size Invalid");

            replacementSize -= 5;

            IntPtr toAlloc = Jmp(codePtr, jmpAlloc);

            //get bytes for the jmp
            byte[] bbs = BitConverter.GetBytes((int)toAlloc);

            byte[] nopping = new byte[replacementSize];

            for (int i = 0; i < nopping.Length; i++)
            {
                //nop all
                nopping[i] = 0x90;
            }

            //give full size for jmp address and nop replacements
            byte[] full = new byte[bbs.Length + replacementSize + 1];

            //jmp
            full[0] = 0xE9;

            //copy bytes to the full 
            bbs.CopyTo(full, 1);

            //copy nops the full scheme
            nopping.CopyTo(full, bbs.Length + 1);

            return full;
        }

        /// <summary>
        /// Calculates the bytes to jump back to the next address 
        /// </summary>
        /// <param name="allocated"></param>
        /// <param name="codePtr"></param>
        /// <param name="requiredSize"></param>
        /// <param name="writing"></param>
        /// <returns></returns>
        public virtual byte[] GetBack(IntPtr allocated, IntPtr codePtr, int requiredSize, byte[] writing)
        {
            //add the memoryblock + how many bytes we are writing, this gives use the correct address for an origin
            //we want to jump to the next address from our "injection" so we add the injection address + the size required : 5 + nop
            int jmpAddress = (int)Jmp(allocated + writing.Length, codePtr + requiredSize);

            byte[] jmpBytes = BitConverter.GetBytes(jmpAddress);

            byte[] ret = new byte[jmpBytes.Length + 1];

            ret[0] = 0xE9;

            jmpBytes.CopyTo(ret, 1);

            return ret;
        }

        /// <summary>
        /// Create information for writing to the origin and destination for your code cave. Manual version of <see cref="CreateCodeCave(Memory, IntPtr, IntPtr, int, byte[])"/>
        /// </summary>
        /// <param name="codePtr">Injection address</param>
        /// <param name="allocated">Memory region</param>
        /// <param name="replacementSize">Size to replace. Must be >= 5</param>
        /// <param name="inject">Bytes to inject</param>
        /// <param name="injectionPoint">Information on what to write at injection site.</param>
        /// <param name="allocPoint">Information on what to write at the allocated space.</param>
        public virtual void Create(IntPtr codePtr, IntPtr allocated, int replacementSize, byte[] inject,
        out Info injectionPoint,
        out Info allocPoint)
        {
            byte[] writeToInjection = GetJump(codePtr, allocated, replacementSize);
            byte[] back = GetBack(allocated, codePtr, replacementSize, inject);

            byte[] writeToAlloc = new byte[back.Length + inject.Length];

            inject.CopyTo(writeToAlloc, 0);
            back.CopyTo(writeToAlloc, inject.Length);

            //M.WriteBytes(allocated.AsUPtr(), writeToAlloc);
            //M.WriteBytes(codePtr.AsUPtr(), writeToInjection);

            allocPoint = new Info
            {
                addr = allocated,
                write = writeToAlloc
            };
            injectionPoint = new Info
            {
                addr = codePtr,
                write = writeToInjection
            };

        }

        /// <summary>
        /// Creates a Code Cave and writes the bytes for you.
        /// </summary>
        /// <param name="m">Memory</param>
        /// <param name="codePtr">Where to inject</param>
        /// <param name="allocated">The allocated memory region</param>
        /// <param name="replacementSize">Size for replacement, size must be >= 5</param>
        /// <param name="inject">Bytes to inject.</param>
        /// <returns>True if memory write was successful</returns>
        public virtual bool CreateCodeCave(Memory m, IntPtr codePtr, IntPtr allocated, int replacementSize, byte[] inject)
        {
            Create(codePtr, allocated, replacementSize, inject, out Info i, out Info a);

            return m.WriteMemory(a.addr, a.write) && m.WriteMemory(i.addr, i.write);
        }

        /// <summary>
        /// Calculate a jump from one Address to another
        /// </summary>
        /// <param name="ori">Origin</param>
        /// <param name="dest">Destination</param>
        /// <returns>Jmp address</returns>
        public IntPtr Jmp(IntPtr ori, IntPtr dest)
        {
            return dest - (int)ori - 5;
        }

        /// <summary>
        /// Specifies where and what to write.
        /// </summary>
        public struct Info
        {
            /// <summary>
            /// Address to write to 
            /// </summary>
            public IntPtr addr;
            /// <summary>
            /// Bytes to write.
            /// </summary>
            public byte[] write;
        }

    }
}
