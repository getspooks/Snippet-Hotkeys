namespace SnippetHotkeys.Forms
{
    partial class TokenHelpForm
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
            rtbHelp = new RichTextBox();
            SuspendLayout();
            // 
            // rtbHelp
            // 
            rtbHelp.BorderStyle = BorderStyle.None;
            rtbHelp.Dock = DockStyle.Fill;
            rtbHelp.Location = new Point(0, 0);
            rtbHelp.Name = "rtbHelp";
            rtbHelp.ReadOnly = true;
            rtbHelp.Size = new Size(584, 561);
            rtbHelp.TabIndex = 0;
            rtbHelp.Text = "";
            // 
            // TokenHelpForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 561);
            Controls.Add(rtbHelp);
            MaximumSize = new Size(600, 600);
            MinimumSize = new Size(400, 200);
            Name = "TokenHelpForm";
            Text = "Token Help";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox rtbHelp;
    }
}