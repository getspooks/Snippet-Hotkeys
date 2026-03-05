using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnippetHotkeys.Models;
using SnippetHotkeys.Services;
using SnippetHotkeys.Platform;

namespace SnippetHotkeys.Models
{
    /// ***************************************************************** ///
    /// Class:      AppConfig
    /// Summary:    Root configuration object for the application   
    /// ***************************************************************** ///
    public sealed class AppConfig
    {
        // Config Schema Vers - for future migrations if the config format changes
        public int Version { get; set; } = 1;

        // Collection of user-defined hotkey bindings
        public List<HotkeyBinding> Hotkeys { get; set; } = new();

        // Determines how snippet text is inserted into the target application
        public enum OutputMode
        {
            ClipboardPaste,
            DirectTyping
        }
    }
}
