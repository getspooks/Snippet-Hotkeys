using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SnippetHotkeys.Services
{
    /// ***************************************************************** ///
    /// Class:      TypeService
    /// Summary:    Sends simulated keyboard input to the currently focused
    ///             application using the Windows SendInput API.
    /// ***************************************************************** ///
    public sealed class TypeService
    {
        // Win32 API used to simulate keyboard input
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private const uint INPUT_KEYBOARD = 1;

        // Indicates a key release event
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Indicates that wScan contains a Unicode character instead of a
        // hardware scan code.
        private const uint KEYEVENTF_UNICODE = 0x0004;

        // Base Win32 input structure passed to SendInput
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        // Union containing the actual input data
        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        // Mouse input structure (not used, required for union size)
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // Keyboard input structure used by SendInput
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // Hardware input structure (not used, required for union size)
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        // Types text into the currently focused application
        public void TypeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Let focus settle after global hotkey
            Thread.Sleep(20);

            // Normalize CRLF -> LF (we'll send '\n')
            text = text.Replace("\r\n", "\n").Replace("\r", "");

            // Chunking avoids extremely large SendInput calls
            // 250 chars => 500 INPUT events (down+up). Adjust if you want
            const int chunkSize = 250;

            for (int start = 0; start < text.Length; start += chunkSize)
            {
                int len = Math.Min(chunkSize, text.Length - start);
                string chunk = text.Substring(start, len);

                if (!SendChunk(chunk))
                    break;

                // Tiny pause between chunks improves reliability in some targets
                Thread.Sleep(5);
            }
        }

        // Converts a text chunk into a sequence of keyboard events and
        // sends them using SendInput
        private static bool SendChunk(string chunk)
        {
            // Each char = 2 INPUTs (down + up)
            var inputs = new INPUT[chunk.Length * 2];
            int idx = 0;

            foreach (char ch in chunk)
            {
                // key down
                inputs[idx++] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = ch,
                            dwFlags = KEYEVENTF_UNICODE,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                // key up
                inputs[idx++] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = ch,
                            dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };
            }

            uint sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            if (sent != inputs.Length)
            {
                int err = Marshal.GetLastWin32Error();
                Debug.WriteLine($"SendInput failed/partial. Sent={sent}/{inputs.Length}, Win32Error={err}");
                return false;
            }

            return true;
        }
    }
}