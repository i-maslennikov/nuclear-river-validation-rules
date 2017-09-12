using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NuClear.ValidationRules.Querying.Host
{
    // Handle IIS start/stop logic, this class not needed in .net core
    internal static class LibRdKafkaLoader
    {
        private const string DllName = "librdkafka.dll";

        private enum LoadLibraryFlags : uint
        {
            // ReSharper disable once InconsistentNaming
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        [DllImport("kernel32")]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static IntPtr _librdkafkaHandle;

        public static void LoadLibrary()
        {
            var baseUri = new Uri(Assembly.GetExecutingAssembly().GetName().EscapedCodeBase);
            var baseDirectory = Path.GetDirectoryName(baseUri.LocalPath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var dllDirectory = Path.Combine(baseDirectory, Environment.Is64BitProcess ? "x64" : "x86");

            _librdkafkaHandle = LoadLibraryEx(Path.Combine(dllDirectory, DllName), IntPtr.Zero, LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH);
            if (_librdkafkaHandle == IntPtr.Zero)
            {
                var lastError = new Win32Exception();
                throw new InvalidOperationException($"Error while loading {DllName} or its dependencies from {dllDirectory}", lastError);
            }
        }

        public static void FreeLibrary()
        {
            if (_librdkafkaHandle != IntPtr.Zero)
            {
                // call method twice to unload correctly
                // 1 reference from LoadLibraryEx and 1 reference from DllImport (in confluent.kafka.dll)
                FreeLibrary(_librdkafkaHandle);
                FreeLibrary(_librdkafkaHandle);
            }
        }
    }
}