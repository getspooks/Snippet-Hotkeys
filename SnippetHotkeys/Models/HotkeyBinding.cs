using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetHotkeys.Models
{
    /// ***************************************************************** ///
    /// Class:      HotkeyBinding
    /// Summary:    Represents a single user-defined hotkey configuration
    /// ***************************************************************** ///
    public class HotkeyBinding
    {
        public string Name { get; set; } = "";      // The name of the hotkey
        public string Hotkey { get; set; } = "";    // The hotkey - ex. "Ctrl+Alt+1"
        public string Snippet { get; set; } = "";   // What gets pasted
        public bool Enabled { get; set; } = true;   // Is the hotkey enabled?
    }
}
