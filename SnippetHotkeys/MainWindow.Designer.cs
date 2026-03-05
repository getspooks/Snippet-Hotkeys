namespace SnippetHotkeys
{
    partial class SnippetHotkeysMW
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            lvHotkeys = new ListView();
            lvhHotkey = new ColumnHeader();
            lvhName = new ColumnHeader();
            panelButtons = new Panel();
            lblStatus = new Label();
            btnRemove = new Button();
            btnEdit = new Button();
            btnAdd = new Button();
            tableLayoutPanel1.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lvHotkeys, 0, 0);
            tableLayoutPanel1.Controls.Add(panelButtons, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(384, 161);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lvHotkeys
            // 
            lvHotkeys.CheckBoxes = true;
            lvHotkeys.Columns.AddRange(new ColumnHeader[] { lvhHotkey, lvhName });
            lvHotkeys.Dock = DockStyle.Fill;
            lvHotkeys.FullRowSelect = true;
            lvHotkeys.GridLines = true;
            lvHotkeys.Location = new Point(3, 3);
            lvHotkeys.MultiSelect = false;
            lvHotkeys.Name = "lvHotkeys";
            lvHotkeys.Size = new Size(378, 105);
            lvHotkeys.TabIndex = 0;
            lvHotkeys.UseCompatibleStateImageBehavior = false;
            lvHotkeys.View = View.Details;
            lvHotkeys.ItemChecked += lvHotkeys_ItemChecked;
            // 
            // lvhHotkey
            // 
            lvhHotkey.Text = "Hotkey";
            lvhHotkey.Width = 120;
            // 
            // lvhName
            // 
            lvhName.Text = "Name";
            lvhName.Width = 250;
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(lblStatus);
            panelButtons.Controls.Add(btnRemove);
            panelButtons.Controls.Add(btnEdit);
            panelButtons.Controls.Add(btnAdd);
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Location = new Point(3, 114);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(378, 44);
            panelButtons.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Location = new Point(0, 29);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(378, 17);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Status";
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemove.Location = new Point(165, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 2;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnEdit
            // 
            btnEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEdit.Location = new Point(84, 3);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(75, 23);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAdd.Location = new Point(3, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // SnippetHotkeysMW
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 161);
            Controls.Add(tableLayoutPanel1);
            MaximumSize = new Size(400, 600);
            MinimumSize = new Size(400, 200);
            Name = "SnippetHotkeysMW";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Snippet Hotkeys V1.1";
            tableLayoutPanel1.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private ListView lvHotkeys;
        private ColumnHeader lvhHotkey;
        private ColumnHeader lvhName;
        private Panel panelButtons;
        private Button btnRemove;
        private Button btnEdit;
        private Button btnAdd;
        private Label lblStatus;
    }
}