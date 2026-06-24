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
    /// ***************************************************************** ///
    /// Class:      TokenHelpForm
    /// Summary:    Displays available snippet tokens, descriptions,
    ///             and examples in a dedicated help window.
    ///             
    /// Responsibilities:
    ///     - Retrieve token metadata from SnippetExpander
    ///     - Organize tokens into logical categories
    ///     - Generate readable help text for display
    ///     - Provide users with a quick token reference
    /// ***************************************************************** /// 
    public partial class TokenHelpForm : Form
    {
        // Provides token metadata used to build the help display
        private readonly SnippetExpander _expander = new SnippetExpander();

        // Initializes the form and loads token reference information
        public TokenHelpForm()
        {
            InitializeComponent();

            Text = "Snippet Tokens";
            StartPosition = FormStartPosition.CenterParent;

            LoadHelpText();
        }

        // Builds the help text shown in the RichTextBox.
        // Tokens are grouped into categories to improve readability.
        private void LoadHelpText()
        {
            var sb = new StringBuilder();

            sb.AppendLine("DATE / TIME TOKENS");
            sb.AppendLine("────────────────────────");
            sb.AppendLine();

            foreach (var t in _expander.GetTokenHelp())
            {
                // Skip formatting tokens in this section
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
                // Only show formatting-related tokens in this section
                if (t.Token is not ("{TAB}" or "{ENTER}" or "{LINEBREAK}"))
                    continue;

                sb.AppendLine(t.Token);
                sb.AppendLine($"  {t.Description}");
                sb.AppendLine($"  Example: {t.Example}");
                sb.AppendLine();
            }

            // Display generated help text
            rtbHelp.Text = sb.ToString();
        }

        // Reserved for future use if a dedicated Close button
        // is reintroduced to the form.
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
