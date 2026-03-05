using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SnippetHotkeys.Models;
using SnippetHotkeys.Platform;

namespace SnippetHotkeys.Services
{
    public sealed record HotkeyApplyFailure(HotkeyBinding Binding, string Reason);
    public sealed record HotkeyApplyResult(int RegisteredCount, int RequestedCount, List<HotkeyApplyFailure> Failures);

    /// ***************************************************************** ///
    /// Class:      HotkeyService
    /// Summary:    Registers/unregisters hotkeys with Windows and raises a
    ///             .NET event when WM_HOTKEY is received.
    /// ***************************************************************** ///
    public sealed class HotkeyService : IDisposable
    {
        // Win32 API to register a global hotkey
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Win32 API to unregister a previously registered hotkey
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifier flag constants (bitwise combinable)
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        // Hidden message window that receives WM_HOTKEY from Windows
        private readonly HotkeyWindow _window;

        // registered id -> binding
        private readonly Dictionary<int, HotkeyBinding> _idToBinding = new();

        // Incrementing ID used when calling RegisterHotKey
        private int _nextId = 1;

        // Last apply report (for status UI / diagnostics)
        public HotkeyApplyResult LastApplyResult { get; private set; }
            = new HotkeyApplyResult(0, 0, new List<HotkeyApplyFailure>());


        // Raised when a registered hotkey fires
        // Provides the HotkeyBinding associated with the triggered hotkey ID
        public event EventHandler<HotkeyBinding>? HotkeyTriggered;

        // Creates the service and subscribes to the HotkeyWindow
        public HotkeyService(HotkeyWindow window)
        {
            _window = window;
            _window.HotkeyPressed += OnHotkeyPressed;
        }

        // Applies bindings and returns how many were successfully registered.
        public int ApplyBindings(IEnumerable<HotkeyBinding> bindings)
        {
            LastApplyResult = ApplyBindingsDetailed(bindings);
            return LastApplyResult.RegisteredCount;
        }


        // Applies bindings and returns a detailed result (failures, requested, etc.).
        public HotkeyApplyResult ApplyBindingsDetailed(IEnumerable<HotkeyBinding> bindings)
        {
            Clear();

            var list = bindings?.ToList() ?? new List<HotkeyBinding>();

            int requested = 0;
            int registered = 0;
            var failures = new List<HotkeyApplyFailure>();

            // Detect duplicates like "Ctrl+Alt+N" repeated twice in config
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var b in list)
            {
                if (!b.Enabled) continue;

                requested++;

                var hotkeyText = (b.Hotkey ?? "").Trim();
                if (hotkeyText.Length == 0)
                {
                    failures.Add(new HotkeyApplyFailure(b, "Hotkey is blank."));
                    continue;
                }

                if (!seen.Add(hotkeyText))
                {
                    failures.Add(new HotkeyApplyFailure(b, $"Duplicate hotkey '{hotkeyText}' in your list."));
                    continue;
                }

                if (!TryParseHotkey(hotkeyText, out uint mods, out uint vk))
                {
                    failures.Add(new HotkeyApplyFailure(b, $"Invalid hotkey format: '{hotkeyText}'."));
                    continue;
                }

                int id = _nextId++;

                if (RegisterHotKey(_window.Handle, id, mods, vk))
                {
                    _idToBinding[id] = b;
                    registered++;
                }
                else
                {
                    int err = Marshal.GetLastWin32Error();
                    string reason = err == 1409
                        ? $"'{hotkeyText}' is already in use on this PC."
                        : $"RegisterHotKey failed (Win32 error {err}) for '{hotkeyText}'.";

                    failures.Add(new HotkeyApplyFailure(b, reason));
                }
            }

            return new HotkeyApplyResult(registered, requested, failures);
        }

        // Unregisters all currently registered hotkeys and resets internal tracking
        // Called before applying a new config or when disposing
        public void Clear()
        {
            // ToList() is safest if anything ever changes collection during unregister
            foreach (var id in _idToBinding.Keys.ToList())
                UnregisterHotKey(_window.Handle, id);

            _idToBinding.Clear();
            _nextId = 1;
        }

        // Called when HotkeyWindow receives WM_HOTKEY
        // Converts the integer ID back into a binding and raises HotkeyTriggered
        private void OnHotkeyPressed(object? sender, int id)
        {
            if (_idToBinding.TryGetValue(id, out var binding))
                HotkeyTriggered?.Invoke(this, binding);
        }

        // Converts a System.Windows.Forms.Keys value into the uint vk
        // expected by RegisterHotKey. (Safe cast via int to avoid runtime issues...)
        private static uint ToVk(Keys k) => unchecked((uint)(int)k);

        /// Parses a user-entered string like "Ctrl+Alt+N" into:
        ///     modifiers (MOD_CONTROL | MOD_ALT | ...)
        ///     vk (virtual key code)
        ///
        /// Supported keys:
        ///     - Letters A-Z
        ///     - Digits 0-9
        ///     - Function keys F1-F24
        ///     - Named Keys enum values (Enter, Tab, Space, etc.)
        ///
        /// Rules:
        ///     - At least one modifier is required (prevents stealing common keys).
        private static bool TryParseHotkey(string input, out uint modifiers, out uint vk)
        {
            modifiers = 0;
            vk = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var parts = input.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length < 2)
                return false;

            string keyPart = parts[^1];

            for (int i = 0; i < parts.Length - 1; i++)
            {
                var p = parts[i].ToLowerInvariant();
                if (p is "ctrl" or "control") modifiers |= MOD_CONTROL;
                else if (p == "alt") modifiers |= MOD_ALT;
                else if (p == "shift") modifiers |= MOD_SHIFT;
                else if (p is "win" or "windows") modifiers |= MOD_WIN;
            }

            if (modifiers == 0)
                return false;

            // Digits 0-9 (Keys.D0..Keys.D9)
            if (keyPart.Length == 1 && char.IsDigit(keyPart[0]))
            {
                var parsed = (Keys)Enum.Parse(typeof(Keys), "D" + keyPart);
                vk = ToVk(parsed);
                return true;
            }

            // Letters A-Z
            if (keyPart.Length == 1 && char.IsLetter(keyPart[0]))
            {
                var parsed = (Keys)char.ToUpperInvariant(keyPart[0]);
                vk = ToVk(parsed);
                return true;
            }

            // Function keys F1..F24
            if (keyPart.StartsWith("F", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(keyPart[1..], out int fNum) && fNum is >= 1 and <= 24)
            {
                var parsed = (Keys)Enum.Parse(typeof(Keys), "F" + fNum);
                vk = ToVk(parsed);
                return true;
            }

            // Named keys like Enter, Tab, Space, etc.
            if (Enum.TryParse<Keys>(keyPart, true, out var k))
            {
                vk = ToVk(k);
                return true;
            }

            return false;
        }

        // Unregisters hotkeys and detaches event handler
        // Prevents "ghost" registrations if the app closes unexpectedly
        public void Dispose()
        {
            Clear();
            _window.HotkeyPressed -= OnHotkeyPressed;
        }
    }
}