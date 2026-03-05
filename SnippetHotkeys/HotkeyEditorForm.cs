using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnippetHotkeys.Platform;
using SnippetHotkeys.Services;
using SnippetHotkeys.Models;

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
        // and example usage. Token metadata is provided by
        // SnippetExpander.GetTokenHelp()
        private void btnHelp_Click(object sender, EventArgs e)
        {
            var lines = _expander.GetTokenHelp()
                .Select(t => $"{t.Token} - {t.Description}\n  Example: {t.Example}")
                .ToList();

            MessageBox.Show(string.Join("\n\n", lines), "Snippet Tokens", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
