# Dummy Memory

#### This library has simple methods to read and write memory, relies heavily on IntPtr, and has a unique Detouring system using Code Caves.

### Copy Me 

```csharp

using DummyMemory;

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
