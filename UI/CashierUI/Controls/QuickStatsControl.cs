using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    public class QuickStatsControl : UserControl
    {
        // Data
        private decimal _thisMonth = 128592.44m;
        private decimal _lastMonth = 115420.00m;
        private decimal _changePercent = 11.4m;
        private string _topLoanType = "Personal Loan";
        private string _topLoanTypePercent = "65%";
        private string _topPaymentMode = "Cash";
        private string _topPaymentModePercent = "58%";
        private string _collectionRate = "94.2%";
        private List<(string DateLabel, decimal Amount)> _dailyBreakdown = new List<(string, decimal)>();

        // UI references for dynamic updates
        private Panel pnlChart;
        private Label lblThisMonthValue;
        private Label lblLastMonthValue;
        private Label lblChangeValue;
        private Panel cardChange;
        private Label lblChangeTitle;
        private Label lblTopLoanTypeValue;
        private Label lblTopLoanTypePercent;
        private Label lblTopPaymentModeValue;
        private Label lblTopPaymentModePercent;
        private Label lblCollectionRateValue;

        public QuickStatsControl()
        {
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Width = 850;
            Height = 420;

            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Clear();

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = ColorTranslator.FromHtml("#EFF6FF"),
                BorderStyle = BorderStyle.FixedSingle
            };
            var headerIcon = new Label
            {
                Text = "📈",
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                Location = new Point(16, 12)
            };
            var headerTitle = new Label
            {
                Text = "QUICK STATS & CHARTS",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(46, 14)
            };
            header.Controls.Add(headerIcon);
            header.Controls.Add(headerTitle);
            Controls.Add(header);

            // Body
            var body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                BackColor = Color.White
            };
            Controls.Add(body);

            // Chart section title
            var lblChartTitle = new Label
            {
                Text = "Collection Trend",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 0)
            };
            body.Controls.Add(lblChartTitle);

            // Chart panel (will draw bars)
            pnlChart = new Panel
            {
                Location = new Point(0, 24),
                Width = 816,
                Height = 140,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlChart.Paint += PnlChart_Paint;
            body.Controls.Add(pnlChart);

            // Stats row (Monthly Comparison + Top Categories)
            var statsRow = new TableLayoutPanel
            {
                Location = new Point(0, 174),
                Width = 816,
                Height = 180,
                ColumnCount = 2,
                RowCount = 1
            };
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            body.Controls.Add(statsRow);

            // Left column: Monthly Comparison
            var leftCol = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(0, 0, 8, 0)
            };

            var cardThisMonth = CreateStatCard("This Month", "", "💵", "#ECFDF5", "#166534", "#15803D", out lblThisMonthValue);
            var cardLastMonth = CreateStatCard("Last Month", "", "📅", "#F3F4F6", "#374151", "#6B7280", out lblLastMonthValue);
            cardChange = CreateStatCard("Change", "", "📈", "#DBEAFE", "#1E40AF", "#2563EB", out lblChangeValue);
            lblChangeTitle = (Label)cardChange.Controls[0];

            leftCol.Controls.Add(cardThisMonth);
            leftCol.Controls.Add(cardLastMonth);
            leftCol.Controls.Add(cardChange);

            // Right column: Top Categories
            var rightCol = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(8, 0, 0, 0)
            };

            var cardTopLoan = CreateCategoryCard("Top Loan Type", "", "", "👥", "#F3E8FF", "#7C3AED", out lblTopLoanTypeValue, out lblTopLoanTypePercent);
            var cardTopMode = CreateCategoryCard("Top Payment Mode", "", "", "💰", "#FFEDD5", "#EA580C", out lblTopPaymentModeValue, out lblTopPaymentModePercent);
            var cardCollRate = CreateCategoryCard("Collection Rate", "", "On-time payments", "⚠️", "#FEF3C7", "#D97706", out lblCollectionRateValue, out _);

            rightCol.Controls.Add(cardTopLoan);
            rightCol.Controls.Add(cardTopMode);
            rightCol.Controls.Add(cardCollRate);

            statsRow.Controls.Add(leftCol, 0, 0);
            statsRow.Controls.Add(rightCol, 1, 0);

            // Initial display
            RefreshDisplay();
        }

        private Panel CreateStatCard(string title, string value, string icon, string bgHex, string titleHex, string valueHex, out Label valueLabel)
        {
            var card = new Panel
            {
                Width = 390,
                Height = 54,
                BackColor = ColorTranslator.FromHtml(bgHex),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 6)
            };

            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml(titleHex),
                Location = new Point(12, 8)
            };

            valueLabel = new Label
            {
                Text = value,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml(valueHex),
                Location = new Point(12, 26)
            };

            var lblIcon = new Label
            {
                Text = icon,
                AutoSize = true,
                Font = new Font("Segoe UI", 16),
                Location = new Point(card.Width - 50, 12)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(valueLabel);
            card.Controls.Add(lblIcon);

            return card;
        }

        private Panel CreateCategoryCard(string title, string value, string sub, string icon, string bgHex, string accentHex, out Label valueLabel, out Label subLabel)
        {
            var card = new Panel
            {
                Width = 390,
                Height = 54,
                BackColor = ColorTranslator.FromHtml(bgHex),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 6)
            };

            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml(accentHex),
                Location = new Point(12, 6)
            };

            valueLabel = new Label
            {
                Text = value,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(12, 22)
            };

            subLabel = new Label
            {
                Text = sub,
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml(accentHex),
                Location = new Point(12, 38)
            };

            var lblIcon = new Label
            {
                Text = icon,
                AutoSize = true,
                Font = new Font("Segoe UI", 16),
                Location = new Point(card.Width - 50, 12)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(valueLabel);
            card.Controls.Add(subLabel);
            card.Controls.Add(lblIcon);

            return card;
        }

        private void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            if (_dailyBreakdown == null || _dailyBreakdown.Count == 0) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int padding = 16;
            int labelHeight = 24;
            int chartHeight = pnlChart.Height - padding * 2 - labelHeight;
            int chartWidth = pnlChart.Width - padding * 2;

            decimal maxAmount = _dailyBreakdown.Max(d => d.Amount);
            if (maxAmount <= 0) maxAmount = 1;

            int barCount = _dailyBreakdown.Count;
            int gap = 6;
            int barWidth = (chartWidth - (gap * (barCount - 1))) / barCount;

            int x = padding;
            int y = padding;

            for (int i = 0; i < barCount; i++)
            {
                var day = _dailyBreakdown[i];
                float heightPercent = (float)(day.Amount / maxAmount);
                int barHeight = (int)(chartHeight * heightPercent);

                // Bar gradient
                var rect = new Rectangle(x, y + chartHeight - barHeight, barWidth, barHeight);
                using (var brush = new LinearGradientBrush(rect, ColorTranslator.FromHtml("#60A5FA"), ColorTranslator.FromHtml("#2563EB"), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                // Date label
                var labelText = day.DateLabel.Length > 3 ? day.DateLabel.Substring(day.DateLabel.Length - 2) : day.DateLabel;
                var labelSize = g.MeasureString(labelText, Font);
                g.DrawString(labelText, Font, Brushes.Gray, x + (barWidth - labelSize.Width) / 2, y + chartHeight + 4);

                x += barWidth + gap;
            }
        }

        private void RefreshDisplay()
        {
            lblThisMonthValue.Text = $"₱{_thisMonth:N2}";
            lblLastMonthValue.Text = $"₱{_lastMonth:N2}";

            bool positive = _changePercent >= 0;
            lblChangeValue.Text = (positive ? "+" : "") + $"{_changePercent:N1}%";

            // Update change card color based on positive/negative
            if (positive)
            {
                cardChange.BackColor = ColorTranslator.FromHtml("#DBEAFE");
                lblChangeTitle.ForeColor = ColorTranslator.FromHtml("#1E40AF");
                lblChangeValue.ForeColor = ColorTranslator.FromHtml("#2563EB");
            }
            else
            {
                cardChange.BackColor = ColorTranslator.FromHtml("#FEE2E2");
                lblChangeTitle.ForeColor = ColorTranslator.FromHtml("#991B1B");
                lblChangeValue.ForeColor = ColorTranslator.FromHtml("#DC2626");
            }

            lblTopLoanTypeValue.Text = _topLoanType;
            lblTopLoanTypePercent.Text = $"{_topLoanTypePercent} of total";

            lblTopPaymentModeValue.Text = _topPaymentMode;
            lblTopPaymentModePercent.Text = $"{_topPaymentModePercent} of total";

            lblCollectionRateValue.Text = _collectionRate;

            pnlChart.Invalidate();
        }

        /// <summary>
        /// Call from CashierReport to update all stats.
        /// </summary>
        public void SetStats(
            decimal thisMonth,
            decimal lastMonth,
            string topLoanType,
            string topLoanTypePercent,
            string topPaymentMode,
            string topPaymentModePercent,
            string collectionRate,
            IEnumerable<(string DateLabel, decimal Amount)> dailyBreakdown)
        {
            _thisMonth = thisMonth;
            _lastMonth = lastMonth;
            _changePercent = _lastMonth > 0 ? ((_thisMonth - _lastMonth) / _lastMonth) * 100m : 0m;

            _topLoanType = topLoanType ?? "N/A";
            _topLoanTypePercent = topLoanTypePercent ?? "0%";
            _topPaymentMode = topPaymentMode ?? "N/A";
            _topPaymentModePercent = topPaymentModePercent ?? "0%";
            _collectionRate = collectionRate ?? "0%";

            _dailyBreakdown = dailyBreakdown?.ToList() ?? new List<(string, decimal)>();

            RefreshDisplay();
        }
    }
}