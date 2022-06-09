using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace DMemory.Debugging
{
    /// <summary>
    /// Used for debugging different parts of the library
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// A list of outputs to write to.
        /// </summary>
        public static readonly List<StreamWriter> Outputs = new List<StreamWriter>();

        /// <summary>
        /// Use the <see cref="Console.OpenStandardOutput()"/>? False by default.
        /// </summary>
        public static bool UseConsole = false;

        /// <summary>
        /// When Debugged, Warning, or Errors are logged.
        /// </summary>
        public static event Action<string> OnAny;

        /// <summary>
        /// Debug
        /// </summary>
        public static event Action<string> OnDebug;
        /// <summary>
        /// Warn
        /// </summary>
        public static event Action<string> OnWarn;
        /// <summary>
        /// Error
        /// </summary>
        public static event Action<Exception> OnError;

        /// <summary>
        /// Sends a message for Debug
        /// </summary>
        /// <param name="debug">Write</param>
        /// <param name="caller">Caller</param>
        public static void Log(object debug, [CallerMemberName] string caller = "")
        {
            string dbg = debug.ToString();
            Format(ref dbg, "DEBUG", caller);
            OnDebug?.Invoke(dbg);
            WriteOut(dbg);
        }

        /// <summary>
        /// Sends a message for Warn
        /// </summary>
        /// <param name="warn">Write</param>
        /// <param name="caller">Caller</param>
        public static void LogWarning(object warn, [CallerMemberName] string caller = "")
        {
            string wrn = warn.ToString();
            Format(ref wrn, "WARNING", caller);
            OnWarn?.Invoke(wrn);
            WriteOut(wrn);
        }

        /// <summary>
        /// Logs an error and can throw an exception if needed.
        /// </summary>
        /// <param name="ex">Exception to format</param>
        /// <param name="throw">Throw <paramref name="ex"/></param>
        /// <param name="caller">Caller</param>
        /// <returns>a reference to <paramref name="ex"/></returns>
        public static Exception LogError(Exception ex, bool @throw = false, [CallerMemberName] string caller = "")
        {
            if (@throw)
                throw ex;

            string msg = $"Message: '{ex.Message}', Inner: '{ex.InnerException?.Message}'";
            Format(ref msg, "ERROR", caller);
            OnError?.Invoke(ex);
            WriteOut(msg);

            return ex;
        }

        /// <summary>
        /// Logs an error and can throw an exception if needed.
        /// </summary>
        /// <param name="ex">Message to create the exception from.</param>
        /// <param name="inner">Inner message to create the exception from. [Optional]</param>
        /// <param name="throw">Throw the created exception?</param>
        /// <param name="caller">Caller</param>
        /// <returns>The newly created exception.</returns>
        public static Exception LogError(object ex, string inner = "", bool @throw = false, [CallerMemberName] string caller = "")
            => LogError(new Exception(ex.ToString(), new Exception(inner)), @throw, caller);

        static void WriteOut(string formatted)
        {
            OnAny?.Invoke(formatted);

            foreach (StreamWriter writer in Outputs)
            {
                writer.WriteLine(formatted);
                writer.WriteLine();
            }

            if (UseConsole)
            {
                Console.WriteLine(formatted);
                Console.WriteLine();
            }
        }

        static void Format(ref string form, string type, string caller)
        {
            form = $"Method({caller}) | {type}: [ {DateTime.UtcNow} ] - '{form}'";
        }
    }
}
