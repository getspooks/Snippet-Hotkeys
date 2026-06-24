using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnippetHotkeys.Forms;
using SnippetHotkeys.Models;
using SnippetHotkeys.Platform;
using SnippetHotkeys.Services;

namespace SnippetHotkeys
{
    /// ***************************************************************** ///
    /// Class:      HotkeyEditorForm
    /// Summary:    UI dialog used to create or edit a HotkeyBinding.
    ///
    ///             The form allows the user to define:
    ///                 - A friendly name
    ///                 - The hotkey combination (Ctrl+Alt+N)
    ///                 - The snippet text that will be inserted
    ///                 
    /// ***************************************************************** /// 
    public partial class HotkeyEditorForm : Form
    {
        // The binding object being edited
        private readonly HotkeyBinding _binding;

        // Provides token expansion metadata for the Help button
        private readonly SnippetExpander _expander = new SnippetExpander();

        // This is the constructor your MainWindow wants to call
        public HotkeyEditorForm(HotkeyBinding binding)
        {
            InitializeComponent();

            txtHotkey.PlaceholderText = "Press desired hotkey...";

            StartPosition = FormStartPosition.Manual;

            _binding = binding;

            // Populate controls from the binding
            txtName.Text = _binding.Name ?? "";
            txtHotkey.Text = _binding.Hotkey ?? "";
            txtSnippet.Text = _binding.Snippet ?? "";
        }

        // Saves user input back into the HotkeyBinding object and
        // closes the dialog with an OK result
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Basic save back into the same object
            _binding.Name = txtName.Text.Trim();
            _binding.Hotkey = txtHotkey.Text.Trim();
            _binding.Snippet = txtSnippet.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        // Cancels editing and closes the dialog without modifying
        // the original binding object
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Displays a help dialog listing available snippet tokens
        // and example usage from TokenHelpForm. 
        private void btnHelp_Click(object sender, EventArgs e)
        {
            using var help = new TokenHelpForm();
            help.ShowDialog(this);
        }

        private void txtHotkey_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            // Prevent the textbox from typing normal characters
            e.SuppressKeyPress = true;

            // Ignore modifier-only presses
            if (e.KeyCode == Keys.ControlKey ||
                e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu ||
                e.KeyCode == Keys.LWin ||
                e.KeyCode == Keys.RWin)
            {
                return;
            }

            var parts = new List<string>();

            if (e.Control)
                parts.Add("Ctrl");

            if (e.Alt)
                parts.Add("Alt");

            if (e.Shift)
                parts.Add("Shift");

            // Optional: support Windows key later if needed
            // Win key is trickier because KeyEventArgs does not expose it as cleanly.

            string key = FormatKey(e.KeyCode);

            if (string.IsNullOrWhiteSpace(key))
                return;

            parts.Add(key);

            txtHotkey.Text = string.Join("+", parts);
            txtHotkey.SelectionStart = txtHotkey.Text.Length;
        }

        private void txtHotkey_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Prevent typed characters from appearing in the textbox
            e.Handled = true;
        }

        private static string FormatKey(Keys key)
        {
            // Letters A-Z
            if (key >= Keys.A && key <= Keys.Z)
                return key.ToString();

            // Number row 0-9
            if (key >= Keys.D0 && key <= Keys.D9)
                return ((int)(key - Keys.D0)).ToString();

            // Numpad 0-9
            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return "NumPad" + ((int)(key - Keys.NumPad0));

            // Function keys F1-F24
            if (key >= Keys.F1 && key <= Keys.F24)
                return key.ToString();

            // Common named keys
            return key switch
            {
                Keys.Space => "Space",
                Keys.Tab => "Tab",
                Keys.Enter => "Enter",
                Keys.Back => "Back",
                Keys.Delete => "Delete",
                Keys.Insert => "Insert",
                Keys.Home => "Home",
                Keys.End => "End",
                Keys.PageUp => "PageUp",
                Keys.PageDown => "PageDown",
                Keys.Up => "Up",
                Keys.Down => "Down",
                Keys.Left => "Left",
                Keys.Right => "Right",
                _ => key.ToString()
            };
        }
    }
}
