using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SnippetHotkeys.Models;
using SnippetHotkeys.Storage;
using SnippetHotkeys.Platform;
using SnippetHotkeys.Services;

namespace SnippetHotkeys
{
    /// ***************************************************************** ///
    /// Class:      SnippetHotkeysMW
    /// Summary:    Main application window
    ///
    /// Responsibilities:
    ///     - Load/save user configuration (AppConfig) via ConfigStore
    ///     - Display current hotkeys in a ListView
    ///     - Add/Edit/Remove hotkeys through HotkeyEditorForm
    ///     - Register enabled hotkeys with Windows via HotkeyService
    ///     - Execute snippets when hotkeys fire (expand tokens + type into target app)
    ///
    /// ***************************************************************** /// 
    public partial class SnippetHotkeysMW : Form
    {
        // Hidden Win32 message window (receives WM_HOTKEY)
        private HotkeyWindow _hotkeyWindow = null!;

        // Registers/unregisters global hotkeys and maps them back to HotkeyBinding objects
        private HotkeyService _hotkeyService = null!;

        // Handles loading/saving config.json in AppData\SnippetHotkeys
        private readonly ConfigStore _store = new ConfigStore();

        // In-memory config object backing the UI
        private AppConfig _config = new AppConfig();

        // Injects expanded snippet text into the currently focused application
        private readonly TypeService _typeService = new TypeService();

        // Expands snippet tokens like {TODAY}, {NOW}, {NEXT_BUSINESS_DAY_DATE}
        private readonly SnippetExpander _expander = new SnippetExpander();

        private bool _loadingList;   // blocks ItemChecked firing during list rebuild

        public SnippetHotkeysMW()
        {
            InitializeComponent();

            // Prevent designer from running runtime code
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            // Load config from disk (creates defaults on first run only)
            _config = _store.Load();

            // Create hotkey window + service BEFORE using them
            _hotkeyWindow = new HotkeyWindow();
            _hotkeyService = new HotkeyService(_hotkeyWindow);

            _hotkeyService.HotkeyTriggered += (s, binding) =>
            {
                var raw = binding.Snippet ?? "";
                var expanded = _expander.Expand(raw);

                // If you still see braces after expanding, it *might* mean a token didn't match
                // (This is a quick diagnostic - you can refine later)
                if (expanded.Contains("{") && expanded.Contains("}"))
                    SetStatus("⚠ Token may not have expanded (check spelling / version)");
                else
                    SetStatus("Pasted snippet");

                _typeService.TypeText(expanded);
            };

            // Register hotkeys + populate UI + set status (single path)
            CommitConfigAndRebind();
        }

        // Central helper: persist config + re-register hotkeys + refresh UI + update status
        // Use this after any user change (add/edit/remove/enable toggle)
        private void CommitConfigAndRebind(bool refreshList = true)
        {
            _store.Save(_config);

            // Re-register based on latest config
            // NOTE: This assumes you changed HotkeyService.ApplyBindings(...) to return int registeredCount.
            int enabled = _config.Hotkeys.Count(h => h.Enabled);
            int registered = _hotkeyService.ApplyBindings(_config.Hotkeys);

            SetStatus($"Hotkeys: {registered}/{enabled} registered");

            if (refreshList)
                RefreshHotkeyList();
        }

        // Central helper: persist config + re-register hotkeys + refresh UI + update status.
        // Use this after any user change (add/edit/remove/enable toggle)
        private void RefreshHotkeyList()
        {
            _loadingList = true;

            lvHotkeys.BeginUpdate();
            lvHotkeys.Items.Clear();

            foreach (var b in _config.Hotkeys)
            {
                var item = new ListViewItem(b.Hotkey ?? "")
                {
                    Tag = b,
                    Checked = b.Enabled
                };

                item.SubItems.Add(b.Name ?? "");

                if (!b.Enabled)
                    item.ForeColor = SystemColors.GrayText;

                lvHotkeys.Items.Add(item);
            }

            lvHotkeys.EndUpdate();

            _loadingList = false;
        }

        // Remove selected binding from config, save, rebind, refresh UI
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvHotkeys.SelectedItems.Count == 0) return;
            if (lvHotkeys.SelectedItems[0].Tag is not HotkeyBinding binding) return;

            _config.Hotkeys.Remove(binding);
            CommitConfigAndRebind();
        }

        // Opens editor dialog with a new binding. If user clicks OK:
        // add to config, save, rebind, refresh UI
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var newBinding = new HotkeyBinding
            {
                Enabled = true,
                Hotkey = "Ctrl+Alt+F12",
                Name = "",
                Snippet = ""
            };

            using var dlg = new HotkeyEditorForm(newBinding);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            _config.Hotkeys.Add(newBinding);
            CommitConfigAndRebind();
        }

        // Opens editor dialog for the selected binding
        // The dialog edits the binding object directly
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvHotkeys.SelectedItems.Count == 0) return;
            if (lvHotkeys.SelectedItems[0].Tag is not HotkeyBinding binding) return;

            using var dlg = new HotkeyEditorForm(binding);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            CommitConfigAndRebind();
        }

        // Checkbox toggle handler (Enabled/Disabled)
        /// Updates the binding.Enabled property and re-registers hotkeys
        private void lvHotkeys_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_loadingList) return;
            if (e.Item.Tag is not HotkeyBinding binding) return;

            binding.Enabled = e.Item.Checked;

            // Only rebind once per user toggle
            CommitConfigAndRebind(refreshList: false);
        }

        // Cleanup on exit - unregister hotkeys and destroy hidden message window
        /// Prevents ghost hotkey registrations
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _hotkeyService?.Dispose();
            _hotkeyWindow?.Dispose();
            base.OnFormClosed(e);
        }

        // Updates the status label text at the bottom of the main window
        private void SetStatus(string text)
        {
            lblStatus.Text = text;
        }
    }
}