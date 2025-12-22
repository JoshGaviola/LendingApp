using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    public class QuickStatsControl : UserControl
    {
        public QuickStatsControl()
        {
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Width = 850;
            Height = 150;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = ColorTranslator.FromHtml("#F0FDF4")
            };
            header.Controls.Add(new Label
            {
                Text = "📈 QUICK STATS",
                Location = new Point(16, 14),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            });
            Controls.Add(header);

            AddStatCard("This Month", "₱128,592", ColorTranslator.FromHtml("#DCFCE7"), 20);
            AddStatCard("Last Month", "₱115,420", ColorTranslator.FromHtml("#F3F4F6"), 190);
            AddStatCard("Change", "+11.4%", ColorTranslator.FromHtml("#DBEAFE"), 360);
        }

        private void AddStatCard(string title, string value, Color color, int x)
        {
            var card = new Panel
            {
                Location = new Point(x, 70),
                Width = 150,
                Height = 60,
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };
            card.Controls.Add(new Label { Text = title, Location = new Point(10, 8), AutoSize = true, Font = new Font("Segoe UI", 8) });
            card.Controls.Add(new Label { Text = value, Location = new Point(10, 28), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) });
            Controls.Add(card);
        }
    }
}