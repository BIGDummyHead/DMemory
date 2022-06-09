using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Debug = DMemory.Debugging.Debug;

namespace DMemory
{
    /// <summary>
    /// A class used for Injecting Dynamic Link Library Files into running processes.
    /// </summary>
    public sealed class Injector
    {
        const string invalid_dll = "The dll file you are trying to inject is not valid.";
        const string allocated_mem_ptr_bad = "The allocated memory for the injection was null.";
        const string allocated_module_ptr_bad = "The allocated memory for the module was null.";
        const string rem_thread_failed_inject = "The remote thread pointer is 0";
        const string failed_thread_close = "Failed to close the remote thread.";
        const string failed_write_path = "Failed to write the path of the Dll to the allocated space.";
        const string not_verify = "Failed to verify Admin, HProc, and or Dll file.";


        /// <summary>
        /// A valid pointer to the Process.
        /// </summary>
        public readonly IntPtr hProc;

        /// <summary>
        /// A verified Dll file that can be injected into the <see cref="hProc"/> process.
        /// </summary>
        public readonly string validDll;

        /// <summary>
        /// A direct getter to <see cref="Memory.Admin"/>
        /// </summary>
        public bool Admin => Memory.Admin;

        private readonly bool valid_injection = false; //used if the ctor cannot throw
        private readonly bool m_throw;


        /// <summary>
        /// Create a new instance of the injector based on a Process and a Valid Dynamic Link Library file!
        /// </summary>
        /// <param name="handle">A handle to a process.</param>
        /// <param name="dllFile">A valid dll file</param>
        /// <param name="throw">Throw when an error occurs?</param>
        /// <exception cref="Exception">Thrown for invalid processes or files.</exception>
        public Injector(IntPtr handle, string dllFile, bool @throw = false)
        {
            m_throw = @throw;

            if (!Admin)
            {
                LogError("You must run this application as an Administrator");
                return;
            }
            else if (handle == IntPtr.Zero)
            {
                LogError("The handle for the process you are trying to inject was null!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(dllFile))
            {
                LogError(invalid_dll, $"{nameof(dllFile)} cannot be null or whitespace.");
                return;
            }
            else if (!File.Exists(dllFile) || Path.GetExtension(dllFile) != ".dll")
            {
                LogError(invalid_dll, $"'{dllFile}' is not a valid file to inject into the process.");
                return;
            }

            validDll = dllFile;
            this.hProc = handle;
            valid_injection = true;
        }

        const string LLA = "LoadLibraryA";

        readonly IntPtr kernel32Module = Native.GetModuleHandle("kernel32.dll");

        /// <summary>
        /// Creates a suspended remote thread that can be resumed/closed using the <paramref name="hThread"/>.
        /// </summary>
        /// <param name="hThread">The created remote thread</param>
        /// <returns>True, if the injection was successful.</returns>
        public bool LoadLibraryASuspended(out IntPtr hThread)
        {
            hThread = IntPtr.Zero;

            if (!valid_injection)
                return LogError(not_verify);


            if (!SizeAndAlloc(out string error, out IntPtr lpParam))
                return LogError(error);

            IntPtr module = Native.GetProcAddress(kernel32Module, LLA); //we find the LoadLibraryA function inside of the kernel32.dll for this specific PROCESS

            if (!CreateSuspendedThread(module, lpParam, out hThread, out error))
                return LogError(error);

            return true;
        }

        /// <summary>
        /// Creates a new remote thread that is automatically ran.
        /// </summary>
        /// <returns>Status of the Injection. Empty if no failure.</returns>
        public bool LoadLibraryA()
        {
            if (!LoadLibraryASuspended(out IntPtr hThread))
                return false;

            Native.ResumeThread(hThread);

            if (!Native.CloseHandle(hThread))
                return LogError(failed_thread_close);

            return true;
        }

        bool CreateSuspendedThread(IntPtr module, IntPtr lpParam, out IntPtr hThread, out string error)
        {
            if (module == IntPtr.Zero)
            {
                hThread = IntPtr.Zero;
                error = allocated_module_ptr_bad;
                return false;
            }

            hThread = Native.CreateRemoteThread(hProc, IntPtr.Zero, 0, module, lpParam, Native.ThreadCreationFlags.NOW, out _);

            if (hThread == IntPtr.Zero)
            {
                error = rem_thread_failed_inject;
                return false;
            }

            error = string.Empty;
            return true;
        }

        bool SizeAndAlloc(out string r, out IntPtr lpParam)
        {
            byte[] vd = Encoding.Default.GetBytes(validDll);
            int lpParamSize = (validDll.Length + 1) * Marshal.SizeOf<char>();

            lpParam = Native.VirtualAllocEx(hProc, IntPtr.Zero, lpParamSize, Native.AllocationType.MEM_COMMIT | Native.AllocationType.MEM_RESERVE, Native.MemoryProtection.PAGE_READWRITE);

            if (lpParam == IntPtr.Zero)
            {
                r = allocated_mem_ptr_bad;
                return false;
            }

            if (!Native.WriteProcessMemory(hProc, lpParam, vd, vd.Length, out var x))
            {
                r = failed_write_path;
                return false;
            }

            r = string.Empty;
            return true;
        }


        /// <summary>
        /// Short hand for <see cref="Native.ResumeThread(IntPtr)"/>
        /// </summary>
        /// <param name="hThread">Thread to restart.</param>
        public uint Resume(IntPtr hThread) => Native.ResumeThread(hThread);

        /// <summary>
        /// Short hand for <see cref="Native.CloseHandle(IntPtr)"/>
        /// </summary>
        /// <param name="hThread">Thread to close.</param>
        public bool Close(IntPtr hThread) => Native.CloseHandle(hThread);

        bool LogError(string error, string inner = "", [CallerMemberName] string caller = "")
        {
            Debug.LogError(error, inner, m_throw, caller);
            return false;
        }

    }






}
