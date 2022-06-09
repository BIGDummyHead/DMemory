using DMemory;
using DMemory.Debugging;
using DMemory.Detouring;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

//A testing app for testing out core components of the library.
class Program
{
    readonly static Memory mem = new Memory("ac_client");
    static IntPtr PlayerBase => mem.Base + 0x109B74;

    static void Main()
    {
        DMemory.Debugging.Debug.UseConsole = true;

        EndTests();
    }

    static void EndTests(bool force = false)
    {
        if (!force)
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        Environment.Exit(0);
    }

    static IntPtr FindDMAAddyTest()
    {
        var x = mem.FindDMAAddy(PlayerBase, 0xF8);
        END();
        return x;
    }

    /// <summary>
    /// Uses health test.
    /// </summary>
    /// <returns></returns>
    static int ReadTest()
    {
        var x = mem.Read<int>(FindDMAAddyTest());
        END();
        return x;
    }

    static bool WriteTest()
    {
        var x = mem.Write(FindDMAAddyTest(), 1337);
        END();
        return x;
    }

    static void FreezeTest()
    {
        mem.Freeze(FindDMAAddyTest(), 1);
        END();
    }

    static void CaveTest()
    {
        string aob = "FF 0E 57 8B 7C 24 14";

        //the bytes we are going to write to our allocated block of memory
        //optionally we could use a string formatted version of this:
        //"FF 06 57 8B 7C 24 14";
        byte[] inject =
        {
            0xFF, 0x06, 0x57, 0x8B, 0x7C, 0x24, 0x14
        };

        //create a new instance of cave, make sure that the aob has not been changed or written to by another 'Cave' or CheatEngine

        Cave cave = new Cave(mem, aob, inject, new Cave.Allocation
        {
            //size of memory block
            memorySize = 1000,
            //replacement size, jmp + nops
            replacementSize = inject.Length
        });


        //inject our cave, bytes from both the address and the region our inserted
        cave.Inject();

        Console.WriteLine("Press any key to Eject...");
        Console.ReadKey(true);

        //our region of memory is deallocated and our original bytes are written back to the address
        cave.Eject();
        END();
    }

    static void InjectionTest(string path, bool testSuspend = true)
    {
        Injector inj = mem.NewInj(path, false);

        if (testSuspend)
        {
            Console.WriteLine(inj.LoadLibraryASuspended(out IntPtr hThread));

            Native.ResumeThread(hThread);
            Native.CloseHandle(hThread);
        }
        else
        {
            Console.WriteLine(inj.LoadLibraryA());
        }

        END();
    }

    static IEnumerable<IntPtr> TestAoB()
    {
        string aob = "FF 0E 57 8B 7C 24 14";
        var x = mem.BaseAoB(aob);
        END();

        return x;
    }

    static void END([CallerMemberName] string caller = "")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(caller);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("--END TEST--");
        Console.ResetColor();
    }
}
