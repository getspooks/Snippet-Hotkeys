using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SnippetHotkeys.Services;

namespace SnippetHotkeys.Forms
{
    public partial class TokenHelpForm : Form
    {
        private readonly SnippetExpander _expander = new SnippetExpander();

        public TokenHelpForm()
        {
            InitializeComponent();

            Text = "Snippet Tokens";
            StartPosition = FormStartPosition.CenterParent;

            LoadHelpText();
        }

        private void LoadHelpText()
        {
            var sb = new StringBuilder();

            sb.AppendLine("DATE / TIME TOKENS");
            sb.AppendLine("────────────────────────");
            sb.AppendLine();

            foreach (var t in _expander.GetTokenHelp())
            {
                // Rough grouping until we add categories properly
                if (t.Token is "{TAB}" or "{ENTER}" or "{LINEBREAK}")
                    continue;

                sb.AppendLine(t.Token);
                sb.AppendLine($"  {t.Description}");
                sb.AppendLine($"  Example: {t.Example}");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("FORMATTING TOKENS");
            sb.AppendLine("────────────────────────");
            sb.AppendLine();

            foreach (var t in _expander.GetTokenHelp())
            {
                if (t.Token is not ("{TAB}" or "{ENTER}" or "{LINEBREAK}"))
                    continue;

                sb.AppendLine(t.Token);
                sb.AppendLine($"  {t.Description}");
                sb.AppendLine($"  Example: {t.Example}");
                sb.AppendLine();
            }

            rtbHelp.Text = sb.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
