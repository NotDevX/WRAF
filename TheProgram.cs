using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace RDresses
{
    internal class TheProgram
    {
        private const string ProcessName = "RobloxPlayerBeta";

        private static void Main(string[] args)
        {
            Console.Title = "RDresses";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Timer timer = new Timer(ChangeConsoleTitle, null, 0, 1500); // Randomize window title to avoid roblox crashing

            Process[] processesByName = Process.GetProcessesByName(ProcessName);
            if (processesByName.Length != 0)
            {
                Process process = processesByName[0];
                IntPtr processHandle = OpenProcess(ProcessAccessFlags.All, false, process.Id);

                if (processHandle != IntPtr.Zero)
                {
                    long playersOffset = 0x100; // Not finding offsets
                    long localPlayerOffset = 240; // Not finding offsets
                    long nameOffset = 48; // Not finding offsets

                    Console.WriteLine("Weed R.A.F.");

                    Console.WriteLine("===== PROCESS =====");

                    Console.WriteLine("Process: " + process.ProcessName);
                    Console.WriteLine("Process ID: " + process.Id);

                    Console.WriteLine("===== OFFSETS =====");

                    Console.WriteLine($"Players Offset: 0x{playersOffset}");
                    Console.WriteLine($"LocalPlayer Offset: {localPlayerOffset}");
                    Console.WriteLine($"Name Offset: {nameOffset}");

                    IntPtr playersAddress = (IntPtr)(process.MainModule.BaseAddress.ToInt64() + playersOffset);
                    IntPtr localPlayerAddress = (IntPtr)(playersAddress.ToInt64() + localPlayerOffset);
                    IntPtr nameAddress = (IntPtr)(localPlayerAddress.ToInt64() + nameOffset);

                    Console.WriteLine("===== ADDRESSES =====");

                    Console.WriteLine($"Players Address: 0x{playersAddress.ToInt64():X}");
                    Console.WriteLine($"LocalPlayer Address: 0x{localPlayerAddress.ToInt64():X}");
                    Console.WriteLine($"Name Address: 0x{nameAddress.ToInt64():X}");
                }
                else   
             Console.WriteLine("[ERROR] Unable to open process.");
            }
            else
            Console.WriteLine("[WARNING] RobloxPlayerBeta process not found!");

            Console.WriteLine("Reload? [Y/N]");
            string reload = Console.ReadLine();

            if(reload.ToLower() == "y")
            {
                Process.Start(AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

        private static string ReadString(IntPtr processHandle, IntPtr address, int maxLength)
        {
            byte[] buffer = new byte[maxLength];
            ReadProcessMemory(processHandle, address, buffer, (uint)maxLength, out _);
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "1234567890!@#$%^&*()";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static void ChangeConsoleTitle(object state)
        {
            string randomTitle = GenerateRandomString(10);
            Console.Title = randomTitle;
        }
    }

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }
}
