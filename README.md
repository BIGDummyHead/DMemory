# Dummy Memory

#### This library has simple methods to read and write memory, relies heavily on IntPtr, and has a unique Detouring system using Code Caves.

### Classes 

* Native - PInvokes for kernel32.dll
* Memory - Features methods for reading and writing memory
* CaveBase - Base for creating caves, mainly used for calculating what and where to write.
* Cave - Inherits CaveBase and features methods to easily Inject/Eject your CodeCave
________________________________________________________________________________________

### Memory Example

```csharp

//pid
//new Memory(0);
//we can pass in a Process 
Memory mem = new Memory("ac_client");

IntPtr plyrBase = mem.Base + 0x109B74;

IntPtr ammoAddress = mem.FindDMAAddy(plyrBase, 0x374, 0x14, 0x0);

//byte[] read = new byte[4];
//mem.ReadMemory(ammoAddress, read);

if( mem.ReadMemory(ammoAddress, 4, out byte[] ammo) )
{
     int readAmmo = BitConverter.ToInt32(ammo);
     
     Console.WriteLine("Player has {0} bullets", readAmmo);
     
     if(!mem.WriteMemory(ammoAddress, BitConverter.GetBytes(readAmmo + 10)))
     {
        //failed to write to memory
     }
}
```

### Code Cave Example

```csharp
            //what to write to allocated space
            byte[] inject =
            {
                0xFF, 0x06, 0x57, 0x8B, 0x7C, 0x24, 0x14
            };
            
            //area of bytes
            string aob = "FF 0E 57 8B 7C 24 14 8D 74 24 28 E8 87";
            
            //our memory
            Memory mem = new Memory("ac_client");

            Cave cave = new Cave(mem, aob, inject, new Cave.Allocation 
            {
                //settings for memory block
                memorySize = 1000,
                //how many times to nop + 5 for jmp
                replacementSize = 7
            });
            
            //Creates a cave to jmp to the allocated memory region
            cave.Inject();
            
            Console.ReadKey();
            
            //Rewrites the original byte[] and deallocates the memory region
            cave.Eject();
            
```            
            
