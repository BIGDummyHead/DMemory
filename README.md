# Dummy Memory

#### A memory package covering all your most inner basic needs for creating a game hack.

Features:

* A Native collection of User32.dll and Kernel32.dll methods.
* Freezing values
* LoadLibraryA injector
* Code Caves with Inject/Eject methods
* AoB Scanning
* FindDMAAddy
* Admin checks
* Generically converting types into byte[]
* Generically converting byte[] to types

This package has a nuget source which can be found [here](https://github.com/BIGDummyHead?tab=packages&repo_name=Dummy-Memory)

### Copy Me 

```csharp

using DMemory;

class Program
{
    static Memory mem = new Memory("ac_client");
    static IntPtr PlayerBase => mem.Base + 0x109B74;
    
    static void Main()
    {
       int offset = 0xF8;
       
       //can return default 
       int health = mem.Read<int>(PlayerBase, offset);
       
       bool didWrite = mem.Write<int>(PlayerBase, 100 + health, offset);
    }
    
    static Vector3 GetPlayerPos()
    {
        int offset = 34;
        
        //we can even use the Write<T> to do the same thing
        //mem.Write<Vector3>(PlayerBase, vec3, offset);
        return mem.Read<Vector3>(PlayerBase, offset);
    }
    
    public Vector3
    {
       public float x, z, y;
    }
}
``` 

## Creating Detours

Detours has been coined as AoB injection by Cheat Engine, it allows us to essentialy replace the game's code by jumping to a allocated region of memory, and then seamlessly returning to that original call. I've tried to make code caves as friendly as possible by simply supplying basic info about your AoB injection and then being able to say `cave.Inject();` or `cave.Eject();`

Below you can see how to write your own code cave! 

```csharp

using DMemory;
using DMemory.Detouring;

//this code cave will simply increase instead of decrease the ammo count.
static Memory mem = new Memory("ac_client");
static void Main()
{
    //aob pattern to find
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
        replacementSize = 7
    });
   
    //inject our cave, bytes from both the address and the region our inserted
    cave.Inject();

    Console.ReadLine();

    //our region of memory is deallocated and our original bytes are written back to the address
    cave.Eject();
}
```

## Injecting a Dll

Injecting Dynamic Link Libraries can sometimes be frustrating in C# so I've made it pretty simple in this latest update. 

> Note: You'll have to run your program as an Administrator for an injection to be possible!


```csharp

Memory m = new Memory("ac_client");

string path = "C:\\Path\\target.dll";

Injector inj = new Injector(m.Handle, dllFile: path, @throw: true); //we indicate that we want to inject the Process Handle for m with the dll from the path. 
//if the injector fails it will throw an exception.

bool success = inj.LoadLibraryASuspended(out IntPtr hThread); //this creates a suspended remote thread that can later be resumed/closed.

//to start your thread simply do:
inj.Resume(hThread);
inj.Close(hThread); //make sure you close this handle as well.


//Let's say that we just wanted to inject and close immediately
success = inj.LoadLibraryA();

```

## Area of Byte Scan

Luckily there are 3 ways to scan for an area of bytes!

```csharp

Memory m = new Memory("ac_client");
string pat = "FF 0E 57 8B 7C 24 14";

//we can also use wildcards such as '?' or '??' instead of a byte
IEnumerable<IntPtr> aob = m.AoB(pat, start:0, end:40000); //gives the addresses of byte[] that match this pattern

//uses the start of this module and its size.
aob = m.ModuleAoB(pat, m.MainModule);

//uses the main module of the process to scan for a pattern!
aob = m.BaseAoB(pat); 
```

## Freezing Values

We can freeze values at a certain address like so!

```csharp

Memory m = new Memory("ac_client");

//creates a new frozen value at 0x509B74 with the offset of 0xF8
//the address is written over with the value of 200
FrozenValue fv = new FrozenValue(m, m.FindDMAAddy(0x509B74, 0xF8), m.GetBytes(200));

fv.Freeze = true; //the value is now frozen and is written over every .125 seconds.

fv.Freeze = false;

fv.ReWrite();

//to change this tick we can call UpdateTimer
FrozenValue.UpdateTimer(TimeSpan.FromSeconds(5)); //tick is now every 5 seconds.

```

Alternatively we can freeze values many different ways!

```csharp

Memory m = new Memory("ac_client");

using(FrozenValue fv = m.Freeze<int>(0x509B74, 200, 0xF8)) //FrozenValue implements IDispoable.
{
    fv.rewriteOnDispose = true;
    //code
} //value is unfrozen at the end and removed from the global timer tick.

```
