using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace DMemory
{
    /// <summary>
    /// A class for exposing some win32api.dll methods.
    /// </summary>
    public static class Native
    {

        /// <summary>
        /// Used to represent of a window.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// X - Position of upper-left corner of window.
            /// </summary>
            public int Left;
            /// <summary>
            /// Y - Position of upper-left corner of window.
            /// </summary>
            public int Top;
            /// <summary>
            /// X - Position of lower-right corner of window.
            /// </summary>
            public int Right;
            /// <summary>
            /// Y - Position of lower-right corner of window.
            /// </summary>
            public int Bottom;

            /// <summary>
            /// Width of window.
            /// </summary>
            /// <remarks><see cref="Right"/> - <seealso cref="Left"/></remarks>
            public int Width => Right - Left;
            /// <summary>
            /// Height of window.
            /// </summary>
            /// <remarks><see cref="Bottom"/> - <seealso cref="Top"/></remarks>
            public int Height => Bottom - Top;

            /// <summary>
            /// Shows as Width x Height
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }

        /// <summary>
        /// The window sizing and positioning flags. 
        /// </summary>
        [Flags]
        public enum WindowSizingPositioning : uint
        {
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,
            /// <summary>
            /// Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,
            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,
            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,
            /// <summary>
            /// Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,
            /// <summary>
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,
            /// <summary>
            /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,
            /// <summary>
            /// Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,
            /// <summary>
            /// 	Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,
            /// <summary>
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,
            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,
            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,
            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,
            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,
            /// <summary>
            /// Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040
        }

        /// <summary>
        /// The following enum contains the process-specific access rights.
        /// </summary>
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            /// <summary>
            /// All possible access rights for a process object.
            /// </summary>
            PROCESS_ALL_ACCESS = 0x001F0FFF,

            /// <summary>
            /// Required to use this process as the parent process with PROC_THREAD_ATTRIBUTE_PARENT_PROCESS.
            /// </summary>
            PROCESS_CREATE_PROCESS = 0x000000080,

            /// <summary>
            /// Required to create a thread in the process.
            /// </summary>
            PROCESS_CREATE_THREAD = 0x00000002,

            /// <summary>
            /// Required to duplicate a handle using DuplicateHandle.
            /// </summary>
            PROCESS_DUP_HANDLE = 0x00000040,

            /// <summary>
            /// Required to retrieve certain information about a process, such as its token, exit code, and priority class.
            /// </summary>
            PROCESS_QUERY_INFORMATION = 0x00000400,

            /// <summary>
            /// Required to retrieve certain information about a process
            /// </summary>
            PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000,

            /// <summary>
            /// Required to set certain information about a process, such as its priority class
            /// </summary>
            PROCESS_SET_INFORMATION = 0x00000200,

            /// <summary>
            /// Required to set memory limits using SetProcessWorkingSetSize.
            /// </summary>

            PROCESS_SET_QUOTA = 0x00000100,

            /// <summary>
            /// Required to suspend or resume a process.
            /// </summary>
            PROCESS_SUSPEND_RESUME = 0x0800,

            /// <summary>
            /// Required to terminate a process using TerminateProcess.
            /// </summary>
            PROCESS_TERMINATE = 0x00000001,

            /// <summary>
            /// Required to perform an operation on the address space of a process.
            /// </summary>
            PROCESS_VM_OPERATION = 0x00000008,

            /// <summary>
            /// Required to read memory in a process using ReadProcessMemory.
            /// </summary>
            PROCESS_VM_READ = 0x00000010,

            /// <summary>
            /// Required to write to memory in a process using WriteProcessMemory.
            /// </summary>
            PROCESS_VM_WRITE = 0x00000020,

            /// <summary>
            /// Required to wait for the process to terminate using the wait functions.
            /// </summary>
            SYNCHRONIZE = 0x00100000
        }

        /// <summary>
        /// The valid access rights for process objects include the standard access rights and some process-specific access rights. The following enum contains the standard access rights used by all objects.
        /// </summary>
        [Flags]
        public enum StandardRights : long
        {
            /// <summary>
            /// Required to delete the object
            /// </summary>
            DELETE = 0x00010000L,
            /// <summary>
            /// Required to read information in the security descriptor for the object, not including the information in the SACL.
            /// </summary>
            READ_CONTROL = 0x00020000L,
            /// <summary>
            /// 	The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.
            /// </summary>
            SYNCHRONIZE = 0x00100000L,
            /// <summary>
            /// Required to modify the DACL in the security descriptor for the object.
            /// </summary>
            WRITE_DAC = 0x00040000L,
            /// <summary>
            /// Required to change the owner in the security descriptor for the object.
            /// </summary>
            WRITE_OWNER = 0x00080000L
        }


        /// <summary>
        /// The following are the memory-protection options; you must specify one of the following values when allocating or protecting a page in memory. Protection attributes cannot be assigned to a portion of a page; they can only be assigned to a whole page.
        /// </summary>
        [Flags]
        public enum MemoryProtection : uint
        {
            /// <summary>
            /// Enables execute access to the committed region of pages. An attempt to write to the committed region results in an access violation.
            /// </summary>
            PAGE_EXECUTE = 0x10,

            /// <summary>
            /// Enables execute or read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation.
            /// </summary>
            PAGE_EXECUTE_READ = 0x20,

            /// <summary>
            /// nables execute, read-only, or read/write access to the committed region of pages.
            /// </summary>
            PAGE_EXECUTE_READWRITE = 0x40,

            /// <summary>
            /// Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page results in a private copy of the page being made for the process. The private page is marked as PAGE_EXECUTE_READWRITE, and the change is written to the new page.
            /// </summary>
            PAGE_EXECUTE_WRITECOPY = 0x80,

            /// <summary>
            /// Disables all access to the committed region of pages. An attempt to read from, write to, or execute the committed region results in an access violation.
            /// </summary>
            PAGE_NOACCESS = 0x01,

            /// <summary>
            /// Enables read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation. If Data Execution Prevention is enabled, an attempt to execute code in the committed region results in an access violation.
            /// </summary>
            PAGE_READONLY = 0x02,

            /// <summary>
            /// Enables read-only or read/write access to the committed region of pages. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.
            /// </summary>
            PAGE_READWRITE = 0x04,

            /// <summary>
            /// Enables read-only or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page results in a private copy of the page being made for the process. The private page is marked as PAGE_READWRITE, and the change is written to the new page. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.
            /// </summary>
            PAGE_WRITECOPY = 0x08,

            /// <summary>
            /// Sets all locations in the pages as invalid targets for CFG. Used along with any execute page protection like PAGE_EXECUTE, PAGE_EXECUTE_READ, PAGE_EXECUTE_READWRITE and PAGE_EXECUTE_WRITECOPY. Any indirect call to locations in those pages will fail CFG checks and the process will be terminated. The default behavior for executable pages allocated is to be marked valid call targets for CFG.
            /// </summary>
            PAGE_TARGETS_INVALID = 0x40000000,

            /// <summary>
            /// Pages in the region become guard pages. Any attempt to access a guard page causes the system to raise a STATUS_GUARD_PAGE_VIOLATION exception and turn off the guard page status. Guard pages thus act as a one-time access alarm. For more information, see Creating Guard Pages. When an access attempt leads the system to turn off guard page status, the underlying page protection takes over.
            /// </summary>
            PAGE_GUARD = 0x100,

            /// <summary>
            /// Sets all pages to be non-cachable. Applications should not use this attribute except when explicitly required for a device. Using the interlocked functions with memory that is mapped with SEC_NOCACHE can result in an EXCEPTION_ILLEGAL_INSTRUCTION exception. The PAGE_NOCACHE flag cannot be used with the PAGE_GUARD, PAGE_NOACCESS, or PAGE_WRITECOMBINE flags.
            /// </summary>
            PAGE_NOCACHE = 0x200,

            /// <summary>
            /// Sets all pages to be write-combined.
            /// </summary>
            PAGE_WRITECOMBINE = 0x400
        }


        /// <summary>
        /// Used for allocating memory regions
        /// </summary>
        [Flags]
        public enum AllocationType
        {
            /// <summary>
            /// Allocates memory charges (from the overall size of memory and the paging files on disk) for the specified reserved memory pages. The function also guarantees that when the caller later initially accesses the memory, the contents will be zero. Actual physical pages are not allocated unless/until the virtual addresses are actually accessed.
            /// </summary>
            MEM_COMMIT = 0x1000,
            /// <summary>
            /// Reserves a range of the process's virtual address space without allocating any actual physical storage in memory or in the paging file on disk.You can commit reserved pages in subsequent calls to the VirtualAlloc function. To reserve and commit pages in one step, call VirtualAlloc with MEM_COMMIT | MEM_RESERVE.
            /// </summary>
            MEM_RESERVE = 0x2000,
            /// <summary>
            /// Indicates that data in the memory range specified by lpAddress and dwSize is no longer of interest. The pages should not be read from or written to the paging file. However, the memory block will be used again later, so it should not be decommitted. This value cannot be used with any other value.
            /// </summary>
            MEM_RESET = 0x8000,
            /// <summary>
            /// MEM_RESET_UNDO should only be called on an address range to which MEM_RESET was successfully applied earlier. It indicates that the data in the specified memory range specified by lpAddress and dwSize is of interest to the caller and attempts to reverse the effects of MEM_RESET. If the function succeeds, that means all data in the specified address range is intact. If the function fails, at least some of the data in the address range has been replaced with zeroes.
            /// </summary>
            MEM_RESET_UNDO = 0x1000000,
            /// <summary>
            /// Allocates memory using large page support.
            /// </summary>
            MEM_LARGE_PAGES = 0x20000000,
            /// <summary>
            /// Reserves an address range that can be used to map Address Windowing Extensions (AWE) pages.
            /// </summary>
            MEM_PHYSICAL = 0x400000,
            /// <summary>
            /// Allocates memory at the highest possible address. This can be slower than regular allocations, especially when there are many allocations.
            /// </summary>
            MEM_TOP_DOWN = 0x100000,
            /// <summary>
            /// Causes the system to track pages that are written to in the allocated region. If you specify this value, you must also specify MEM_RESERVE.
            /// </summary>
            MEM_WRITE_WATCH = 0x200000
        }

            /// <summary>
        /// Flags used for creating a new remote thread.
            /// </summary>
        [Flags]
        public enum ThreadCreationFlags : uint
        {
            /// <summary>
            /// 	The thread runs immediately after creation.
            /// </summary>
            NOW = 0,
            /// <summary>
            /// The thread is created in a suspended state, and does not run until the ResumeThread function is called.
            /// </summary>
            CREATE_SUSPENDED = 0x00000004,
            /// <summary>
            /// The dwStackSize parameter specifies the initial reserve size of the stack. If this flag is not specified, dwStackSize specifies the commit size.
            /// </summary>
            STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000
        }

        /// <summary>
        /// Freeing Types
        /// </summary>
        [Flags]
        public enum FreeType : uint
        {
            /// <summary>
            /// Decommits the specified region of committed pages. After the operation, the pages are in the reserved state.
            /// </summary>
            MEM_DECOMMIT = 0x00004000,

        /// <summary>
            /// Releases the specified region of pages, or placeholder (for a placeholder, the address space is released and available for other allocations). After this operation, the pages are in the free state.
        /// </summary>
            MEM_RELEASE = 0x00008000,

        /// <summary>
            /// To coalesce two adjacent placeholders, specify MEM_RELEASE | MEM_COALESCE_PLACEHOLDERS. When you coalesce placeholders, lpAddress and dwSize must exactly match the overall range of the placeholders to be merged.
        /// </summary>
            /// <remarks>Must be used with <see cref="MEM_RELEASE"/></remarks>
            MEM_COALESCE_PLACEHOLDERS = 0x00000001,
            /// <summary>
            /// Frees an allocation back to a placeholder (after you've replaced a placeholder with a private allocation using VirtualAlloc2 or Virtual2AllocFromApp). To split a placeholder into two placeholders, specify MEM_RELEASE | MEM_PRESERVE_PLACEHOLDER.
            /// </summary>
            /// <remarks>Must be used with <see cref="MEM_RELEASE"/></remarks>
            MEM_PRESERVE_PLACEHOLDER = 0x00000002
        }

        /// <summary>
        /// Flags for the <see cref="GetWindowLong(IntPtr, int)"/>
        /// </summary>
        public enum IndexFlags : int
        {
            /// <summary>
            /// Retrieves the extended window styles.
            /// </summary>
            GWL_EXSTYLE = -20,
            /// <summary>
            /// Retrieves a handle to the application instance.
            /// </summary>
            GWL_HINSTANCE = -6,
            /// <summary>
            /// Retrieves a handle to the parent window, if any.
            /// </summary>
            GWL_HWNDPARENT = -8,
            /// <summary>
            /// Retrieves the identifier of the window.
            /// </summary>
            GWL_ID = -12,
            /// <summary>
            /// Retrieves the window styles.
            /// </summary>
            GWL_STYLE = -16,
            /// <summary>
            /// Retrieves the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
            /// </summary>
            GWl_USERDATA = -21,
            /// <summary>
            /// Retrieves the address of the window procedure, or a handle representing the address of the window procedure. You must use the CallWindowProc function to call the window procedure.
            /// </summary>
            GWL_WINDPRC = -4
        }

        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        public const int HWND_TOP = 0;
        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        public const int HWND_BOTTOM = 1;
        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        public const int HWND_TOPMOST = -1;
        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        public const int HWND_NOTOPMOST = -2;

        /// <summary>
        /// Terminates the specfied process and all of its threads.
        /// </summary>
        /// <param name="hProcess">A handle to the process to be terminated.</param>
        /// <param name="uExitCode">The exit code to be used by the process and threads terminated as a result of this call.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>

        [DllImport("kernel32.dll")]
        public static extern bool TerminateProcess([In] IntPtr hProcess, [In] uint uExitCode);





        /// <summary>
        /// Retrieves the address of an exported function (also known as a procedure) or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">A handle to the DLL module that contains the function or variable. The LoadLibrary, LoadLibraryEx, LoadPackagedLibrary, or GetModuleHandle function returns this handle.</param>
        /// <param name="procName">The function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function or variable.</returns>
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress([In] IntPtr hModule, [In] string procName);

        
       


        /// <summary>
        /// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
        /// </summary>
        /// <param name="lpModuleName">The name of the loaded module (either a .dll or .exe file).</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified module.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle([In, Optional] string lpModuleName);

        /// <summary>
        /// Creates a thread that runs in the virtual address space of another process.
        /// </summary>
        /// <param name="hProcess">A handle to the process in which the thread is to be created. The handle must have the PROCESS_CREATE_THREAD, PROCESS_QUERY_INFORMATION, PROCESS_VM_OPERATION, PROCESS_VM_WRITE, and PROCESS_VM_READ access rights, and may fail without these rights on certain platforms. For more information, see Process Security and Access Rights.</param>
        /// <param name="lpThreadAttributes">A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new thread and determines whether child processes can inherit the returned handle. If lpThreadAttributes is NULL, the thread gets a default security descriptor and the handle cannot be inherited. The access control lists (ACL) in the default security descriptor for a thread come from the primary token of the creator.</param>
        /// <param name="dwStackSize">The initial size of the stack, in bytes. The system rounds this value to the nearest page. If this parameter is 0 (zero), the new thread uses the default size for the executable. For more information, see Thread Stack Size.</param>
        /// <param name="lpStartAddress">A pointer to the application-defined function of type LPTHREAD_START_ROUTINE to be executed by the thread and represents the starting address of the thread in the remote process. The function must exist in the remote process. For more information, see ThreadProc.</param>
        /// <param name="lpParameter">A pointer to a variable to be passed to the thread function</param>
        /// <param name="dwCreationFlags">The flags that control the creation of the thread.</param>
        /// <param name="lpThreadId">A pointer to a variable that receives the thread identifier.</param>
        /// <returns>If the function succeeds, the return value is a handle to the new thread.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread([In] IntPtr hProcess, [In] IntPtr lpThreadAttributes, [In] uint dwStackSize, [In] IntPtr lpStartAddress, [In] IntPtr lpParameter, [In] uint dwCreationFlags, [Out] out IntPtr lpThreadId);

        /// <summary>
        /// Creates a thread that runs in the virtual address space of another process.
        /// </summary>
        /// <param name="hProcess">A handle to the process in which the thread is to be created. The handle must have the PROCESS_CREATE_THREAD, PROCESS_QUERY_INFORMATION, PROCESS_VM_OPERATION, PROCESS_VM_WRITE, and PROCESS_VM_READ access rights, and may fail without these rights on certain platforms. For more information, see Process Security and Access Rights.</param>
        /// <param name="lpThreadAttributes">A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new thread and determines whether child processes can inherit the returned handle. If lpThreadAttributes is NULL, the thread gets a default security descriptor and the handle cannot be inherited. The access control lists (ACL) in the default security descriptor for a thread come from the primary token of the creator.</param>
        /// <param name="dwStackSize">The initial size of the stack, in bytes. The system rounds this value to the nearest page. If this parameter is 0 (zero), the new thread uses the default size for the executable. For more information, see Thread Stack Size.</param>
        /// <param name="lpStartAddress">A pointer to the application-defined function of type LPTHREAD_START_ROUTINE to be executed by the thread and represents the starting address of the thread in the remote process. The function must exist in the remote process. For more information, see ThreadProc.</param>
        /// <param name="lpParameter">A pointer to a variable to be passed to the thread function</param>
        /// <param name="dwCreationFlags">The flags that control the creation of the thread.</param>
        /// <param name="lpThreadId">A pointer to a variable that receives the thread identifier.</param>
        /// <returns>If the function succeeds, the return value is a handle to the new thread.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread([In] IntPtr hProcess, [In] IntPtr lpThreadAttributes, [In] uint dwStackSize, [In] IntPtr lpStartAddress, [In] IntPtr lpParameter, [In] ThreadCreationFlags dwCreationFlags, [Out] out IntPtr lpThreadId);

        /// <summary>
        /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count. When the reference count reaches zero, the module is unloaded from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hModule">A handle to the loaded library module. The LoadLibrary, LoadLibraryEx, GetModuleHandle, or GetModuleHandleEx function returns this handle.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary([In] IntPtr hModule);

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwAccess">The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights.</param>
        /// <param name="inherit">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="pid">The identifier of the local process to be opened.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess([In] uint dwAccess, [In] bool inherit, [In] int pid);

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwAccess">The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights.</param>
        /// <param name="inherit">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="pid">The identifier of the local process to be opened.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess([In] ProcessAccessFlags dwAccess, [In] bool inherit, [In] int pid);


        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwAccess">The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights.</param>
        /// <param name="inherit">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="proc">Uses the <see cref="Process.Id"/>. The identifier of the local process to be opened.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags dwAccess, bool inherit = false)
            => OpenProcess((uint)dwAccess, inherit, proc.Id);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle([In] IntPtr handle);

        /// <summary>
        /// Read process memory
        /// </summary>
        /// <param name="hProcess">A handle to the process with memory that is being read. The handle must have PROCESS_VM_READ access to the process.</param>
        /// <param name="lpBaseAddress">A pointer to the base address in the specified process from which to read. Before any data transfer occurs, the system verifies that all data in the base address and memory of the specified size is accessible for read access, and if it is not accessible the function fails.</param>
        /// <param name="lpBuffer">A pointer to a buffer that receives the contents from the address space of the specified process.</param>
        /// <param name="nSize">The number of bytes to be read from the specified process.</param>
        /// <param name="lpNumberOfBytesRead">A pointer to a variable that receives the number of bytes transferred into the specified buffer. If lpNumberOfBytesRead is NULL, the parameter is ignored.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory([In] IntPtr hProcess, [In] IntPtr lpBaseAddress, [Out] byte[] lpBuffer, [In] UIntPtr nSize, [Out] out UIntPtr lpNumberOfBytesRead);


        /// <summary>
        /// Writes data to an area of memory in a specified process. The entire area to be written to must be accessible or the operation fails.
        /// </summary>
        /// <param name="hProcess">A handle to the process memory to be modified. The handle must have PROCESS_VM_WRITE and PROCESS_VM_OPERATION access to the process.</param>
        /// <param name="lpBaseAddress">A pointer to the base address in the specified process to which data is written. Before data transfer occurs, the system verifies that all data in the base address and memory of the specified size is accessible for write access, and if it is not accessible, the function fails.</param>
        /// <param name="lpBuffer">A pointer to the buffer that contains data to be written in the address space of the specified process.</param>
        /// <param name="dwSize">The number of bytes to be written to the specified process.</param>
        /// <param name="lpNumberOfBytesWritten">A pointer to a variable that receives the number of bytes transferred into the specified process. This parameter is optional. If lpNumberOfBytesWritten is NULL, the parameter is ignored.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory([In] IntPtr hProcess, [In] IntPtr lpBaseAddress, [In] byte[] lpBuffer, [In] int dwSize, [Out] out IntPtr lpNumberOfBytesWritten);

        /// <summary>
        /// Writes data to an area of memory in a specified process. The entire area to be written to must be accessible or the operation fails.
        /// </summary>
        /// <param name="hProcess">A handle to the process memory to be modified. The handle must have PROCESS_VM_WRITE and PROCESS_VM_OPERATION access to the process.</param>
        /// <param name="lpBaseAddress">A pointer to the base address in the specified process to which data is written. Before data transfer occurs, the system verifies that all data in the base address and memory of the specified size is accessible for write access, and if it is not accessible, the function fails.</param>
        /// <param name="lpBuffer">A pointer to the buffer that contains data to be written in the address space of the specified process.</param>
        /// <param name="dwSize">The number of bytes to be written to the specified process.</param>
        /// <param name="lpNumberOfBytesWritten">A pointer to a variable that receives the number of bytes transferred into the specified process. This parameter is optional. If lpNumberOfBytesWritten is NULL, the parameter is ignored.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory([In] IntPtr hProcess, [In] IntPtr lpBaseAddress, [In] IntPtr lpBuffer, [In] int dwSize, [Out] out IntPtr lpNumberOfBytesWritten);

        /// <summary>
        /// Changes the protection on a region of committed pages in the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process whose memory protection is to be changed. The handle must have the PROCESS_VM_OPERATION access right. For more information, see Process Security and Access Rights.</param>
        /// <param name="lpAddress">A pointer to the base address of the region of pages whose access protection attributes are to be changed. All pages in the specified region must be within the same reserved region allocated when calling the VirtualAlloc or VirtualAllocEx function using MEM_RESERVE. The pages cannot span adjacent reserved regions that were allocated by separate calls to VirtualAlloc or VirtualAllocEx using MEM_RESERVE.</param>
        /// <param name="dwSize">The size of the region whose access protection attributes are changed, in bytes. The region of affected pages includes all pages containing one or more bytes in the range from the lpAddress parameter to (lpAddress+dwSize). This means that a 2-byte range straddling a page boundary causes the protection attributes of both pages to be changed.</param>
        /// <param name="flNewProtect">The memory protection option. This parameter can be one of the memory protection constants. For mapped views, this value must be compatible with the access protection specified when the view was mapped(see MapViewOfFile, MapViewOfFileEx, and MapViewOfFileExNuma).</param>
        /// <param name="lpflOldProtect">A pointer to a variable that receives the previous access protection of the first page in the specified region of pages. If this parameter is NULL or does not point to a valid variable, the function fails.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx([In] IntPtr hProcess, [In] IntPtr lpAddress, [In] int dwSize, [In] uint flNewProtect, [Out] out uint lpflOldProtect);

        /// <summary>
        /// Changes the protection on a region of committed pages in the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process whose memory protection is to be changed. The handle must have the PROCESS_VM_OPERATION access right. For more information, see Process Security and Access Rights.</param>
        /// <param name="lpAddress">A pointer to the base address of the region of pages whose access protection attributes are to be changed. All pages in the specified region must be within the same reserved region allocated when calling the VirtualAlloc or VirtualAllocEx function using MEM_RESERVE. The pages cannot span adjacent reserved regions that were allocated by separate calls to VirtualAlloc or VirtualAllocEx using MEM_RESERVE.</param>
        /// <param name="dwSize">The size of the region whose access protection attributes are changed, in bytes. The region of affected pages includes all pages containing one or more bytes in the range from the lpAddress parameter to (lpAddress+dwSize). This means that a 2-byte range straddling a page boundary causes the protection attributes of both pages to be changed.</param>
        /// <param name="flNewProtect">The memory protection option. This parameter can be one of the memory protection constants. For mapped views, this value must be compatible with the access protection specified when the view was mapped(see MapViewOfFile, MapViewOfFileEx, and MapViewOfFileExNuma).</param>
        /// <param name="lpflOldProtect">A pointer to a variable that receives the previous access protection of the first page in the specified region of pages. If this parameter is NULL or does not point to a valid variable, the function fails.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx([In] IntPtr hProcess, [In] IntPtr lpAddress, [In] int dwSize, [In] ProcessAccessFlags flNewProtect, [Out] out ProcessAccessFlags lpflOldProtect);

        /// <summary>
        /// Reserves, commits, or changes the state of a region of memory within the virtual address space of a specified process. The function initializes the memory it allocates to zero.
        /// </summary>
        /// <param name="hProcess">The handle to a process. The function allocates memory within the virtual address space of this process.</param>
        /// <param name="lpAddress">The pointer that specifies a desired starting address for the region of pages that you want to allocate. If you are reserving memory, the function rounds this address down to the nearest multiple of the allocation granularity. If you are committing memory that is already reserved, the function rounds this address down to the nearest page boundary.To determine the size of a page and the allocation granularity on the host computer, use the GetSystemInfo function.  If lpAddress is NULL, the function determines where to allocate the region. If this address is within an enclave that you have not initialized by calling InitializeEnclave, VirtualAllocEx allocates a page of zeros for the enclave at that address.The page must be previously uncommitted, and will not be measured with the EEXTEND instruction of the Intel Software Guard Extensions programming model. If the address in within an enclave that you initialized, then the allocation operation fails with the ERROR_INVALID_ADDRESS error.That is true for enclaves that do not support dynamic memory management (i.e.SGX1). SGX2 enclaves will permit allocation, and the page must be accepted by the enclave after it has been allocated.</param>
        /// <param name="dwSize">The size of the region of memory to allocate, in bytes.</param>
        /// <param name="flAllocationType">The type of memory allocation.</param>
        /// <param name="flProtect">The memory protection for the region of pages to be allocated. If the pages are being committed, you can specify any one of the memory protection constants.</param>
        /// <returns>If the function succeeds, the return value is the base address of the allocated region of pages.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx([In] IntPtr hProcess, [In, Optional] IntPtr lpAddress, [In] int dwSize, [In] AllocationType flAllocationType, [In] MemoryProtection flProtect);

        /// <summary>
        /// Reserves, commits, or changes the state of a region of memory within the virtual address space of a specified process. The function initializes the memory it allocates to zero.
        /// </summary>
        /// <param name="hProcess">The handle to a process. The function allocates memory within the virtual address space of this process.</param>
        /// <param name="lpAddress">The pointer that specifies a desired starting address for the region of pages that you want to allocate. If you are reserving memory, the function rounds this address down to the nearest multiple of the allocation granularity. If you are committing memory that is already reserved, the function rounds this address down to the nearest page boundary.To determine the size of a page and the allocation granularity on the host computer, use the GetSystemInfo function.  If lpAddress is NULL, the function determines where to allocate the region. If this address is within an enclave that you have not initialized by calling InitializeEnclave, VirtualAllocEx allocates a page of zeros for the enclave at that address.The page must be previously uncommitted, and will not be measured with the EEXTEND instruction of the Intel Software Guard Extensions programming model. If the address in within an enclave that you initialized, then the allocation operation fails with the ERROR_INVALID_ADDRESS error.That is true for enclaves that do not support dynamic memory management (i.e.SGX1). SGX2 enclaves will permit allocation, and the page must be accepted by the enclave after it has been allocated.</param>
        /// <param name="dwSize">The size of the region of memory to allocate, in bytes.</param>
        /// <param name="flAllocationType">The type of memory allocation.</param>
        /// <param name="flProtect">The memory protection for the region of pages to be allocated. If the pages are being committed, you can specify any one of the memory protection constants.</param>
        /// <returns>If the function succeeds, the return value is the base address of the allocated region of pages.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx([In] IntPtr hProcess, [In, Optional] IntPtr lpAddress, [In] int dwSize, [In] uint flAllocationType, [In] uint flProtect);



        /// <summary>
        /// Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to a process. The function frees memory within the virtual address space of the process. The handle must have the PROCESS_VM_OPERATION access right.For more information, see Process Security and Access Rights.</param>
        /// <param name="lpAddress">A pointer to the starting address of the region of memory to be freed. If the dwFreeType parameter is MEM_RELEASE, lpAddress must be the base address returned by the VirtualAllocEx function when the region is reserved.</param>
        /// <param name="dwSize">The size of the region of memory to free, in bytes. If the dwFreeType parameter is MEM_RELEASE, dwSize must be 0 (zero). The function frees the entire region that is reserved in the initial allocation call to VirtualAllocEx. If dwFreeType is MEM_DECOMMIT, the function decommits all memory pages that contain one or more bytes in the range from the lpAddress parameter to (lpAddress+dwSize). This means, for example, that a 2-byte region of memory that straddles a page boundary causes both pages to be decommitted.If lpAddress is the base address returned by VirtualAllocEx and dwSize is 0 (zero), the function decommits the entire region that is allocated by VirtualAllocEx.After that, the entire region is in the reserved state.</param>
        /// <param name="dwFreeType">The type of free operation. </param>
        /// <returns>If the function succeeds, the return value is a true value.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx([In] IntPtr hProcess, [In] IntPtr lpAddress, [In] int dwSize, [In] FreeType dwFreeType);

        /// <summary>
        /// Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to a process. The function frees memory within the virtual address space of the process. The handle must have the PROCESS_VM_OPERATION access right.For more information, see Process Security and Access Rights.</param>
        /// <param name="lpAddress">A pointer to the starting address of the region of memory to be freed. If the dwFreeType parameter is MEM_RELEASE, lpAddress must be the base address returned by the VirtualAllocEx function when the region is reserved.</param>
        /// <param name="dwSize">The size of the region of memory to free, in bytes. If the dwFreeType parameter is MEM_RELEASE, dwSize must be 0 (zero). The function frees the entire region that is reserved in the initial allocation call to VirtualAllocEx. If dwFreeType is MEM_DECOMMIT, the function decommits all memory pages that contain one or more bytes in the range from the lpAddress parameter to (lpAddress+dwSize). This means, for example, that a 2-byte region of memory that straddles a page boundary causes both pages to be decommitted.If lpAddress is the base address returned by VirtualAllocEx and dwSize is 0 (zero), the function decommits the entire region that is allocated by VirtualAllocEx.After that, the entire region is in the reserved state.</param>
        /// <param name="dwFreeType">The type of free operation. </param>
        /// <returns>If the function succeeds, the return value is a true value.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx([In] IntPtr hProcess, [In] IntPtr lpAddress, [In] int dwSize, [In] uint dwFreeType);

        /// <summary>
        /// Decrements a thread's suspend count. When the suspend count is decremented to zero, the execution of the thread is resumed.
        /// </summary>
        /// <param name="hThread">A handle to the thread to be restarted.</param>
        /// <returns>If the function succeeds, the return value is the thread's previous suspend count.</returns>
        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread([In] IntPtr hThread);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpRect">Window size.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect([In] HandleRef hWnd, [Out] out RECT lpRect);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="hWnd">A handle to the window. <see cref="Process.MainWindowHandle"/></param>
        /// <param name="lpRect">Window size.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        public static bool GetWindowRect(IntPtr hWnd, out RECT lpRect) => GetWindowRect(new HandleRef(null, hWnd), out lpRect);

        /// <summary>
        /// Changes the position and dimensions of the specified window. For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. For a child window, they are relative to the upper-left corner of the parent window's client area.
        /// </summary>
        /// <param name="handle">A handle to the window.</param>
        /// <param name="x">The new position of the left side of the window.</param>
        /// <param name="y">The new position of the top of the window.</param>
        /// <param name="nWidth">The new width of the window.</param>
        /// <param name="nHeight">The new height of the window.</param>
        /// <param name="bRepaint">Indicates whether the window is to be repainted. If this parameter is TRUE, the window receives a message. If the parameter is FALSE, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of moving a child window.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        public static extern bool MoveWindow([In] IntPtr handle, [In] int x, [In] int y, [In] int nWidth, [In] int nHeight, [In] bool bRepaint);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. Optional values: <see cref="HWND_BOTTOM"/> / <seealso cref="HWND_NOTOPMOST"/> / <seealso cref="HWND_TOP"/> / <see cref="HWND_TOPMOST"/></param>
        /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="uFlags">The window sizing and positioning flags.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos([In] IntPtr hWnd, [In, Optional] IntPtr hWndInsertAfter, [In] int X, [In] int Y, [In] int cx, [In] int cy, [In] uint uFlags);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. Optional values: <see cref="HWND_BOTTOM"/> / <seealso cref="HWND_NOTOPMOST"/> / <seealso cref="HWND_TOP"/> / <see cref="HWND_TOPMOST"/></param>
        /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="uFlags">The window sizing and positioning flags.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos([In] IntPtr hWnd, [In, Optional] IntPtr hWndInsertAfter, [In] int X, [In] int Y, [In] int cx, [In] int cy, [In] WindowSizingPositioning uFlags);

        /// <summary>
        /// Retrieves the coordinates of a window's client area. The client coordinates specify the upper-left and lower-right corners of the client area. Because client coordinates are relative to the upper-left corner of a window's client area, the coordinates of the upper-left corner are (0,0).
        /// </summary>
        /// <param name="hWnd">A handle to the window whose client coordinates are to be retrieved.</param>
        /// <param name="lpRect">Window size.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect([In] IntPtr hWnd, [Out] out RECT lpRect);

        /// <summary>
        /// Retrieves information about the specified window. The function also retrieves the 32-bit (DWORD) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex"><see cref="IndexFlags"/>: The zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes of extra window memory, minus four; for example, if you specified 12 or more bytes of extra memory, a value of 8 would be an index to the third 32-bit integer. To retrieve any other value, specify one of the following values.</param>
        /// <returns>If the function succeeds, the return value is the requested value.</returns>

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong([In] IntPtr hWnd, [In] int nIndex);

        /// <summary>
        /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex"><see cref="IndexFlags"/>: The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values.</param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.</returns>

        [DllImport("user32.dll")]
        public static extern int SetWindowLong([In] IntPtr hWnd, [In] int nIndex, [In] long dwNewLong);

        /// <summary>
        /// Retrieves the calling thread's last-error code value. The last-error code is maintained on a per-thread basis. Multiple threads do not overwrite each other's last-error code.
        /// </summary>
        /// <returns>The return value is the calling thread's last-error code.</returns>
        /// <remarks>Only use this if <see cref="MarshalLastError"/> is not sufficient!!!</remarks>
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        /// <summary>
        /// ShortHand for <see cref="Marshal.GetLastWin32Error"/>
        /// </summary>
        /// <returns>The last error call set by a call to the Win32 SetLastError function.</returns>
        public static int MarshalLastError() => Marshal.GetLastWin32Error();


    }
}
