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

        // Used to check if modifier keys are currently held down
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const uint INPUT_KEYBOARD = 1;

        // Indicates a key release event
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Indicates that wScan contains a Unicode character instead of a
        // hardware scan code.
        private const uint KEYEVENTF_UNICODE = 0x0004;

        // Hardware scan code flag (more “physical key” like)
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        // Virtual keys (for releasing modifiers)
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;   // Alt
        private const int VK_SHIFT = 0x10;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;

        // Scan codes (US keyboard) - these tend to work best in browsers
        private const ushort SC_TAB = 0x0F;
        private const ushort SC_ENTER = 0x1C;
        private const ushort SC_LSHIFT = 0x2A;

        // Marker used by SnippetExpander for {LINEBREAK}
        private const char LINEBREAK_MARKER = '\uE000';

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

            // Normalize CRLF -> LF (we handle '\n' ourselves)
            text = text.Replace("\r\n", "\n").Replace("\r", "");

            // Chunking avoids extremely large SendInput calls
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
            // Unicode chars are batched for speed; special keys force a flush.
            var inputs = new System.Collections.Generic.List<INPUT>(chunk.Length * 2);

            bool FlushUnicodeBatch()
            {
                if (inputs.Count == 0) return true;

                uint sent = SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
                if (sent != inputs.Count)
                {
                    int err = Marshal.GetLastWin32Error();
                    Debug.WriteLine($"SendInput unicode batch failed/partial. Sent={sent}/{inputs.Count}, Win32Error={err}");
                    return false;
                }

                inputs.Clear();
                return true;
            }

            foreach (char ch in chunk)
            {
                // {ENTER} expands to '\n' in SnippetExpander
                if (ch == '\n')
                {
                    if (!FlushUnicodeBatch()) return false;

                    // Global hotkeys can leave modifiers "down" briefly; clean state first.
                    ReleaseCommonModifiers();

                    // Gmail/Chrome respects a real Enter keypress far more reliably than Unicode '\n'
                    if (!SendScanKeyPress(SC_ENTER)) return false;

                    Thread.Sleep(2);
                    continue;
                }

                // {TAB} expands to '\t'
                if (ch == '\t')
                {
                    if (!FlushUnicodeBatch()) return false;

                    ReleaseCommonModifiers();

                    if (!SendScanKeyPress(SC_TAB)) return false;

                    Thread.Sleep(2);
                    continue;
                }

                // {LINEBREAK} expands to marker (Shift+Enter)
                if (ch == LINEBREAK_MARKER)
                {
                    if (!FlushUnicodeBatch()) return false;

                    ReleaseCommonModifiers();

                    if (!SendShiftEnterScan()) return false;

                    Thread.Sleep(2);
                    continue;
                }

                // Normal characters (Unicode typing)
                AddUnicode(inputs, ch);
            }

            // Send any remaining unicode in one batch
            return FlushUnicodeBatch();
        }

        // Adds a single Unicode character to the batch (down+up)
        private static void AddUnicode(System.Collections.Generic.List<INPUT> list, char ch)
        {
            // key down (unicode)
            list.Add(new INPUT
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
            });

            // key up (unicode)
            list.Add(new INPUT
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
            });
        }

        // Sends a single "physical" key press via scan code (down+up)
        private static bool SendScanKeyPress(ushort scanCode)
        {
            var seq = new INPUT[]
            {
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = scanCode, dwFlags = KEYEVENTF_SCANCODE } }
                },
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = scanCode, dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP } }
                }
            };

            uint sent = SendInput((uint)seq.Length, seq, Marshal.SizeOf(typeof(INPUT)));
            if (sent != seq.Length)
            {
                int err = Marshal.GetLastWin32Error();
                Debug.WriteLine($"SendInput scan keypress failed. Sent={sent}/{seq.Length}, Win32Error={err}");
                return false;
            }

            return true;
        }

        // Sends Shift+Enter using scan codes (best match for Gmail "soft line break")
        private static bool SendShiftEnterScan()
        {
            var seq = new INPUT[]
            {
                // Shift down
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = SC_LSHIFT, dwFlags = KEYEVENTF_SCANCODE } }
                },

                // Enter down
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = SC_ENTER, dwFlags = KEYEVENTF_SCANCODE } }
                },

                // Enter up
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = SC_ENTER, dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP } }
                },

                // Shift up
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = SC_LSHIFT, dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP } }
                }
            };

            uint sent = SendInput((uint)seq.Length, seq, Marshal.SizeOf(typeof(INPUT)));
            if (sent != seq.Length)
            {
                int err = Marshal.GetLastWin32Error();
                Debug.WriteLine($"SendInput Shift+Enter scan failed. Sent={sent}/{seq.Length}, Win32Error={err}");
                return false;
            }

            return true;
        }

        // Releases commonly held modifiers so injected keys don't become Ctrl+Enter, etc.
        private static void ReleaseCommonModifiers()
        {
            // Only send "key up" if the key is actually down to avoid weirdness.
            if (IsDown(VK_CONTROL)) SendVkKeyUp((ushort)VK_CONTROL);
            if (IsDown(VK_MENU)) SendVkKeyUp((ushort)VK_MENU);
            if (IsDown(VK_SHIFT)) SendVkKeyUp((ushort)VK_SHIFT);
            if (IsDown(VK_LWIN)) SendVkKeyUp((ushort)VK_LWIN);
            if (IsDown(VK_RWIN)) SendVkKeyUp((ushort)VK_RWIN);
        }

        private static bool IsDown(int vKey) => (GetAsyncKeyState(vKey) & 0x8000) != 0;

        private static void SendVkKeyUp(ushort vk)
        {
            var input = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT { wVk = vk, wScan = 0, dwFlags = KEYEVENTF_KEYUP, time = 0, dwExtraInfo = IntPtr.Zero }
                }
            };

            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}