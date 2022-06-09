using System;
using System.Collections.Generic;

namespace DMemory.Detouring
{
    /// <summary>
    /// Methods for writing a Code Cave
    /// </summary>
    public class CaveBase
    {
        /// <summary>
        /// Trys to convert a string to a byte[]
        /// </summary>
        /// <param name="byteFormatted">A string formatted as a byte[]</param>
        /// <param name="read">The bytes read from the formatted string</param>
        /// <param name="byteSeperator">Determines how the string will be split</param>
        /// <returns>False if any bytes could not be parsed.</returns>
        public static bool TryConvert(string byteFormatted, out byte[] read, char byteSeperator = ' ')
        {
            List<byte> bysRead = new List<byte>();

            foreach (string word in byteFormatted.Split(byteSeperator))
            {
                if (long.TryParse(word, System.Globalization.NumberStyles.HexNumber, null, out long hex))
                {
                    bysRead.Add((byte)hex);
                    continue;
                }

                    read = Array.Empty<byte>();
                    return false;
                }
            }

            read = bysRead.ToArray();
            return true;
        }

        /// <summary>
        /// Trys to convert a string to a byte[]
        /// </summary>
        /// <param name="byteFormatted">A string formatted as a byte[]</param>
        /// <param name="byteSeperator">Determines how the string will be split</param>
        /// <returns>The byte[] read from the <see cref="TryConvert(string, out byte[], char)"/></returns>
        /// <exception cref="Exception"/>
        public static byte[] Convert(string byteFormatted, char byteSeperator = ' ')
        {
            if (!TryConvert(byteFormatted, out byte[] read, byteSeperator))
                throw new Exception("Failed to convert!");

            return read;
        }

        /// <summary>
        /// A static instance created for global use.
        /// </summary>
        public static CaveBase Global { get; private set; } = new CaveBase();

        /// <summary>
        /// Get the bytes to jmp from one address to another
        /// </summary>
        /// <param name="from">Address to call jmp.</param>
        /// <param name="to">Address to jump to.</param>
        /// <param name="replacementSize">Size to replace</param>
        /// <returns>Bytes to write to <paramref name="from"/></returns>
        public virtual byte[] GetJump(IntPtr from, IntPtr to, int replacementSize)
        {
            if (replacementSize < 5)
                throw new Exception("Replacement size is invalid for jump, must be greater than or equal to 5");

            replacementSize -= 5;

            IntPtr toAlloc = Jmp(from, to);

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
        /// Calculates the bytes needed to jump back 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="requiredSize">Size of jump</param>
        /// <param name="writing"></param>
        /// <returns></returns>
        public virtual byte[] GetBack(IntPtr to, IntPtr from, int requiredSize, byte[] writing)
        {
            //add the memoryblock + how many bytes we are writing, this gives use the correct address for an origin
            //we want to jump to the next address from our "injection" so we add the injection address + the size required : 5 + nop
            int jmpAddress = (int)Jmp(to + writing.Length, from + requiredSize);

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
        //set up information for writing to the address later, no need to write instantly 
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
