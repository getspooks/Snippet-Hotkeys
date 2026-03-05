using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnippetHotkeys.Platform
{
    /// ***************************************************************** ///
    /// Class:      HotkeyWindow
    /// Summary:    Lightweight hidden window used to receive WM_HOTKEY
    ///             messages from the Windows operating system.
    /// Message Flow:
    ///             Windows OS
    ///                 ↓
    ///             WM_HOTKEY message
    ///                 ↓
    ///             HotkeyWindow.WndProc
    ///                 ↓
    ///             HotkeyPressed event
    ///                 ↓
    ///             HotkeyService → Snippet execution   
    /// ***************************************************************** /// 
    public sealed class HotkeyWindow : NativeWindow, IDisposable
    {
        // Identify when a hotkey is fired
        public event EventHandler<int>? HotkeyPressed;

        // Create a hidden window handle to receive WM_HOTKEY
        public HotkeyWindow()
        {
            CreateHandle(new CreateParams());
        }

        // Interrupt handler - turns WM_HOTKEY into an event
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                HotkeyPressed?.Invoke(this, id);
            }

            base.WndProc(ref m);
        }

        // Destroys native window handle when done
        public void Dispose()
        {
            try { DestroyHandle(); } catch { /* ignore */ }
        }

    }
}
