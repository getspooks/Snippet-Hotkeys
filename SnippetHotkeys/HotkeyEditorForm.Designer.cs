namespace SnippetHotkeys
{
    partial class HotkeyEditorForm
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
            labelName = new Label();
            labelHotkey = new Label();
            labelSnippet = new Label();
            txtName = new TextBox();
            txtHotkey = new TextBox();
            txtSnippet = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            btnHelp = new Button();
            SuspendLayout();
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(20, 44);
            labelName.Name = "labelName";
            labelName.Size = new Size(42, 15);
            labelName.TabIndex = 0;
            labelName.Text = "Name:";
            // 
            // labelHotkey
            // 
            labelHotkey.AutoSize = true;
            labelHotkey.Location = new Point(14, 15);
            labelHotkey.Name = "labelHotkey";
            labelHotkey.Size = new Size(48, 15);
            labelHotkey.TabIndex = 1;
            labelHotkey.Text = "Hotkey:";
            // 
            // labelSnippet
            // 
            labelSnippet.AutoSize = true;
            labelSnippet.Location = new Point(12, 68);
            labelSnippet.Name = "labelSnippet";
            labelSnippet.Size = new Size(50, 15);
            labelSnippet.TabIndex = 2;
            labelSnippet.Text = "Snippet:";
            // 
            // txtName
            // 
            txtName.Location = new Point(82, 7);
            txtName.Name = "txtName";
            txtName.Size = new Size(290, 23);
            txtName.TabIndex = 3;
            // 
            // txtHotkey
            // 
            txtHotkey.Location = new Point(82, 36);
            txtHotkey.Name = "txtHotkey";
            txtHotkey.Size = new Size(290, 23);
            txtHotkey.TabIndex = 4;
            // 
            // txtSnippet
            // 
            txtSnippet.AcceptsReturn = true;
            txtSnippet.Location = new Point(82, 65);
            txtSnippet.Multiline = true;
            txtSnippet.Name = "txtSnippet";
            txtSnippet.ScrollBars = ScrollBars.Vertical;
            txtSnippet.Size = new Size(290, 47);
            txtSnippet.TabIndex = 5;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(3, 126);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 7;
            btnOK.Text = "Ok";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(84, 126);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            btnHelp.Location = new Point(306, 126);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(75, 23);
            btnHelp.TabIndex = 9;
            btnHelp.Text = "Help";
            btnHelp.UseVisualStyleBackColor = true;
            btnHelp.Click += btnHelp_Click;
            // 
            // HotkeyEditorForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(384, 161);
            Controls.Add(btnHelp);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(txtSnippet);
            Controls.Add(txtHotkey);
            Controls.Add(txtName);
            Controls.Add(labelSnippet);
            Controls.Add(labelHotkey);
            Controls.Add(labelName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MaximumSize = new Size(400, 200);
            MinimizeBox = false;
            MinimumSize = new Size(400, 200);
            Name = "HotkeyEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Hotkey";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelName;
        private Label labelHotkey;
        private Label labelSnippet;
        private TextBox txtName;
        private TextBox txtHotkey;
        private TextBox txtSnippet;
        private Button btnOK;
        private Button btnCancel;
        private Button btnHelp;
    }
}