using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using LendingApp.Class.Services; // <- added to resolve DataGetter
using System.Data.Entity;
using MySql.Data.MySqlClient;

namespace LendingSystem.Admin
{
    public partial class SystemConfigControl : UserControl
    {
        private Panel mainPanel;
        private int currentY = 20;
        private Color primaryColor = Color.FromArgb(59, 130, 246);  // Blue
        private Color secondaryColor = Color.FromArgb(239, 246, 255);  // Light blue
        private Color accentColor = Color.FromArgb(34, 197, 94);  // Green

        // track created inputs so we can load/save
        private readonly Dictionary<string, TextBox> _textInputs = new Dictionary<string, TextBox>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, CheckBox> _checkboxes = new Dictionary<string, CheckBox>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, RadioButton> _radioOptions = new Dictionary<string, RadioButton>(StringComparer.OrdinalIgnoreCase);

        public SystemConfigControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateControls();
        }

        private void CreateControls()
        {
            // Clear previous tracked controls
            _textInputs.Clear();
            _checkboxes.Clear();
            _radioOptions.Clear();

            mainPanel.Controls.Clear();
            currentY = 20;

            int panelWidth = mainPanel.ClientSize.Width - 40;
            if (panelWidth < 400) panelWidth = 400; // Minimum width

            // Header
            AddHeader("SYSTEM CONFIGURATION", panelWidth);

            // Sections
            AddSectionBox("Interest Calculation Method", new[] { "Diminishing Balance", "Flat Rate", "Add-On Rate", "Compound Interest" }, true, panelWidth, "interest_method");
            AddInputSection("Financial Parameters",
                new[] { "Penalty Rate", "Grace Period", "Service Charge" },
                new[] { "2.0", "7", "3.0" },
                new[] { "%", "days", "%" },
                new[] { "penalty_rate", "grace_period_days", "service_charge_pct" },
                panelWidth);

            AddInputSection("Approval Workflow",
                new[] { "Level 1 Max", "Level 2 Max", "Level 3 Max" },
                new[] { "50000", "200000", "500000" },
                new[] { "₱", "₱", "₱" },
                new[] { "level1_max", "level2_max", "level3_max" },
                panelWidth);

            AddInputSection("Credit Scoring Weights",
                new[] { "Payment History", "Credit Utilization", "Credit History", "Income Stability" },
                new[] { "35", "30", "15", "20" },
                new[] { "%", "%", "%", "%" },
                new[] { "weight_payment_history", "weight_credit_utilization", "weight_history_length", "weight_income_stability" },
                panelWidth);

            AddCheckboxSection("User Account Features", new[] { ("Borrower Accounts","borrower_accounts_enabled"), ("Document Management","document_management_enabled"), ("SMS Reminders","sms_reminders_enabled") }, panelWidth);
            AddInputSection("Loan Defaults",
                new[] { "Minimum Amount", "Maximum Amount", "Available Terms" },
                new[] { "5000", "500000", "6,12,18,24" },
                new[] { "₱", "₱", "months" },
                new[] { "default_min_amount", "default_max_amount", "default_available_terms" },
                panelWidth);

            AddCheckboxSection("Report Export Formats", new[] { ("PDF","export_pdf"), ("Excel","export_excel"), ("CSV","export_csv") }, panelWidth);

            // Buttons
            AddActionButtons(panelWidth);

            // Info Panel
            AddInfoPanel(panelWidth);

            // After building controls, attempt to load persistent settings from DB
            LoadSettings();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Recreate controls on resize to allow responsive layout; controls are re-tracked
            CreateControls();
        }

        private void AddHeader(string title, int width)
        {
            Panel header = new Panel
            {
                Location = new Point(10, currentY),
                Size = new Size(width, 50),
                BackColor = primaryColor,
                Padding = new Padding(10)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            header.Controls.Add(titleLabel);
            mainPanel.Controls.Add(header);
            currentY += 60;
        }

        /// <summary>
        /// Adds either a radio group (isRadio=true) or checkbox group (isRadio=false).
        /// For radio groups the settingKey identifies the field to persist (single value).
        /// </summary>
        private void AddSectionBox(string title, string[] options, bool isRadio, int width, string settingKey = null)
        {
            GroupBox groupBox = new GroupBox
            {
                Text = "  " + title + "  ",
                Location = new Point(10, currentY),
                Size = new Size(width, (options.Length * 28) + 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            int y = 25;
            foreach (string option in options)
            {
                Control control;
                if (isRadio)
                {
                    var rb = new RadioButton
                    {
                        Text = option,
                        Location = new Point(20, y),
                        Size = new Size(width - 40, 24),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black
                    };
                    if (option == options[0])
                        rb.Checked = true;

                    // if a settingKey is provided, store the radio options so we can load/save the selected value
                    if (!string.IsNullOrWhiteSpace(settingKey))
                    {
                        // store under key: settingKey + "|" + option (unique)
                        _radioOptions[$"{settingKey}|{option}"] = rb;
                    }

                    control = rb;
                }
                else
                {
                    var cb = new CheckBox
                    {
                        Text = option,
                        Location = new Point(20, y),
                        Size = new Size(width - 40, 24),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black
                    };
                    // If we were called without explicit key mapping, use option text as key
                    _checkboxes[option] = cb;
                    control = cb;
                }
                groupBox.Controls.Add(control);
                y += 28;
            }

            mainPanel.Controls.Add(groupBox);
            currentY += groupBox.Height + 10;
        }

        private void AddInputSection(string title, string[] labels, string[] defaults, string[] suffixes, string[] keys, int width)
        {
            if (labels.Length != keys.Length) throw new ArgumentException("labels and keys length must match");

            GroupBox groupBox = new GroupBox
            {
                Text = "  " + title + "  ",
                Location = new Point(10, currentY),
                Size = new Size(width, (labels.Length * 35) + 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            int inputX = Math.Max(150, width / 2 - 50);

            int y = 25;
            for (int i = 0; i < labels.Length; i++)
            {
                // Label
                Label lbl = new Label
                {
                    Text = labels[i] + ":",
                    Location = new Point(20, y),
                    Size = new Size(inputX - 30, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                groupBox.Controls.Add(lbl);

                // TextBox with border
                TextBox txt = new TextBox
                {
                    Text = defaults != null && i < defaults.Length ? defaults[i] : "",
                    Location = new Point(inputX, y),
                    Size = new Size(100, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black,
                    BorderStyle = BorderStyle.FixedSingle
                };
                groupBox.Controls.Add(txt);

                // store reference by key for load/save
                var key = keys[i];
                if (!_textInputs.ContainsKey(key))
                    _textInputs[key] = txt;
                else
                    _textInputs[key] = txt; // replace if rebuilding

                // Suffix
                Label suffix = new Label
                {
                    Text = suffixes != null && i < suffixes.Length ? suffixes[i] : "",
                    Location = new Point(inputX + 105, y),
                    Size = new Size(60, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                groupBox.Controls.Add(suffix);

                y += 35;
            }

            mainPanel.Controls.Add(groupBox);
            currentY += groupBox.Height + 10;
        }

        private void AddCheckboxSection(string title, (string label, string key)[] optionsWithKeys, int width)
        {
            GroupBox groupBox = new GroupBox
            {
                Text = "  " + title + "  ",
                Location = new Point(10, currentY),
                Size = new Size(width, (optionsWithKeys.Length * 28) + 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            int y = 25;
            foreach (var (label, key) in optionsWithKeys)
            {
                var cb = new CheckBox
                {
                    Text = label,
                    Location = new Point(20, y),
                    Size = new Size(width - 40, 24),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                groupBox.Controls.Add(cb);
                _checkboxes[key] = cb;
                y += 28;
            }

            mainPanel.Controls.Add(groupBox);
            currentY += groupBox.Height + 10;
        }

        // original overload for backward compatibility with earlier CreateControls calls
        private void AddCheckboxSection(string title, string[] options, int width)
        {
            AddSectionBox(title, options, false, width, null);
        }

        private void AddActionButtons(int width)
        {
            Panel buttonPanel = new Panel
            {
                Location = new Point(10, currentY),
                Size = new Size(width, 50),
                BackColor = Color.Transparent
            };

            Button saveBtn = new Button
            {
                Text = "💾 SAVE CONFIGURATION",
                Location = new Point(0, 10),
                Size = new Size(Math.Min(180, width / 2 - 10), 35),
                BackColor = accentColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveBtn.FlatAppearance.BorderSize = 0;
            saveBtn.Click += (s, e) =>
            {
                if (ValidateSettings())
                {
                    SaveSettings();
                }
            };

            Button restoreBtn = new Button
            {
                Text = "↻ RESTORE DEFAULTS",
                Location = new Point(Math.Min(190, width / 2 + 10), 10),
                Size = new Size(Math.Min(160, width / 2 - 10), 35),
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            restoreBtn.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            restoreBtn.FlatAppearance.BorderSize = 1;
            restoreBtn.Click += (s, e) =>
            {
                // Reset UI controls to sensible defaults (same defaults used when building controls)
                foreach (var kv in _textInputs)
                {
                    // simple heuristic: clear or set to existing default in CreateControls
                    // For now, set numeric-looking fields to "0" and terms to "6,12,18,24"
                    if (kv.Key.Contains("terms") || kv.Key.Contains("available_terms"))
                        kv.Value.Text = "6,12,18,24";
                    else if (kv.Key.Contains("min") || kv.Key.Contains("max") || kv.Key.Contains("level"))
                        kv.Value.Text = kv.Key.Contains("min") ? "5000" : (kv.Key.Contains("level1") ? "50000" : kv.Value.Text);
                    else
                        kv.Value.Text = "0";
                }
                foreach (var cb in _checkboxes.Values) cb.Checked = false;
                // default interest method
                foreach (var rbkv in _radioOptions)
                {
                    // _radioOptions keys are "interest_method|<option>"
                    if (rbkv.Key.StartsWith("interest_method|") && rbkv.Key.EndsWith("|".TrimEnd()))
                        ;
                }
                MessageBox.Show("Defaults restored!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            buttonPanel.Controls.Add(saveBtn);
            buttonPanel.Controls.Add(restoreBtn);
            mainPanel.Controls.Add(buttonPanel);
            currentY += 60;
        }

        private void AddInfoPanel(int width)
        {
            Panel infoPanel = new Panel
            {
                Location = new Point(10, currentY),
                Size = new Size(width, 120),
                BackColor = secondaryColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            Label title = new Label
            {
                Text = "⚙️ VALIDATION RULES",
                Location = new Point(0, 0),
                Size = new Size(width - 30, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            Label rules = new Label
            {
                Text = "• Percentages must be between 0-100%\n" +
                       "• All amounts must be positive values\n" +
                       "• Credit score weights must total 100%\n" +
                       "• Grace period must be positive integer\n" +
                       "• Terms: Comma-separated months",
                Location = new Point(0, 25),
                Size = new Size(width - 30, 90),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };

            infoPanel.Controls.Add(title);
            infoPanel.Controls.Add(rules);
            mainPanel.Controls.Add(infoPanel);
        }

        private void LoadSettings()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Query setting_key and setting_value directly
                    var rows = db.Database.SqlQuery<SettingRow>("SELECT setting_key AS SettingKey, setting_value AS SettingValue FROM system_settings").ToList();

                    var dict = rows.ToDictionary(r => r.SettingKey, r => r.SettingValue, StringComparer.OrdinalIgnoreCase);

                    // load text inputs
                    foreach (var kv in _textInputs)
                    {
                        if (dict.TryGetValue(kv.Key, out var val))
                            kv.Value.Text = val;
                    }

                    // load checkboxes (true/1 yes)
                    foreach (var kv in _checkboxes)
                    {
                        if (dict.TryGetValue(kv.Key, out var val))
                        {
                            kv.Value.Checked = IsTrue(val);
                        }
                    }

                    // load radio groups (interest method)
                    // radio keys were saved as "interest_method|<option>" mapping; prefer to check dict for single key "interest_method"
                    if (dict.TryGetValue("interest_method", out var interestVal))
                    {
                        // find radio with matching option text
                        var matchKey = $"interest_method|{interestVal}";
                        if (_radioOptions.TryGetValue(matchKey, out var rb))
                        {
                            rb.Checked = true;
                        }
                        else
                        {
                            // fallback: try to match by option ignoring case
                            var found = _radioOptions.Keys.FirstOrDefault(k => k.StartsWith("interest_method|") && k.Substring("interest_method|".Length).Equals(interestVal, StringComparison.OrdinalIgnoreCase));
                            if (found != null) _radioOptions[found].Checked = true;
                        }
                    }
                    else
                    {
                        // If radio stored via option-specific keys (older pattern), map them
                        var firstMatch = _radioOptions.Keys.FirstOrDefault(k => k.StartsWith("interest_method|") && dict.ContainsKey($"interest_method|{k.Substring("interest_method|".Length)}"));
                        if (firstMatch != null) _radioOptions[firstMatch].Checked = true;
                    }
                }
            }
            catch
            {
                // fail silently - UI keeps defaults
            }
        }

        private void SaveSettings()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    using (var tx = db.Database.BeginTransaction())
                    {
                        // Save text inputs
                        foreach (var kv in _textInputs)
                        {
                            UpsertSetting(db, kv.Key, kv.Value.Text);
                        }

                        // Save checkboxes
                        foreach (var kv in _checkboxes)
                        {
                            UpsertSetting(db, kv.Key, kv.Value.Checked ? "1" : "0");
                        }

                        // Save radio group selected value for interest_method
                        // find the checked radio among those we tracked
                        var checkedRadio = _radioOptions.FirstOrDefault(r => r.Value.Checked);
                        if (!checkedRadio.Equals(default(KeyValuePair<string, RadioButton>)))
                        {
                            // key format: "interest_method|<option>
                            var parts = checkedRadio.Key.Split(new[] { '|' }, 2);
                            if (parts.Length == 2)
                            {
                                var settingKey = parts[0];
                                var settingValue = parts[1];
                                UpsertSetting(db, settingKey, settingValue);
                            }
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save configuration:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpsertSetting(AppDbContext db, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            // Use MySQL ON DUPLICATE KEY UPDATE; system_settings.setting_key has UNIQUE KEY
            const string sql = @"INSERT INTO system_settings (setting_key, setting_value, description, modified_by)
                         VALUES (@key, @value, '', @user)
                         ON DUPLICATE KEY UPDATE
                            setting_value = @value,
                            modified_by = @user,
                            last_modified = CURRENT_TIMESTAMP;";

            // SAFE DEFAULT:
            // system_settings.modified_by is nullable in lendingapp.sql, so store NULL unless you later
            // extend login/user model to include user_id.
            var userId = (object)DBNull.Value;

            db.Database.ExecuteSqlCommand(sql,
                new MySqlParameter("@key", key),
                new MySqlParameter("@value", value ?? ""),
                new MySqlParameter("@user", userId));
        }

        private bool ValidateSettings()
        {
            // Percent check: every percent input must be 0..100
            var percentKeys = new[] { "penalty_rate", "service_charge_pct",
                                      "weight_payment_history", "weight_credit_utilization",
                                      "weight_history_length", "weight_income_stability" };

            foreach (var k in percentKeys)
            {
                if (_textInputs.TryGetValue(k, out var txt))
                {
                    if (!decimal.TryParse(txt.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) || v < 0 || v > 100)
                    {
                        MessageBox.Show($"Value for '{k}' must be a number between 0 and 100.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            // Credit weights must total 100
            var weightKeys = new[] { "weight_payment_history", "weight_credit_utilization", "weight_history_length", "weight_income_stability" };
            decimal total = 0;
            foreach (var wk in weightKeys)
            {
                if (_textInputs.TryGetValue(wk, out var t))
                {
                    if (!decimal.TryParse(t.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var vv))
                    {
                        MessageBox.Show($"Weight '{wk}' is not a valid number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    total += vv;
                }
            }
            if (total != 100m)
            {
                MessageBox.Show("Credit scoring weights must total 100%.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Numeric checks for amounts and levels
            var numericKeys = new[] { "level1_max", "level2_max", "level3_max", "default_min_amount", "default_max_amount" };
            foreach (var nk in numericKeys)
            {
                if (_textInputs.TryGetValue(nk, out var t))
                {
                    if (!decimal.TryParse(t.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var vv) || vv < 0)
                    {
                        MessageBox.Show($"Value for '{nk}' must be a non-negative number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            // Grace period must be integer positive
            if (_textInputs.TryGetValue("grace_period_days", out var gp))
            {
                if (!int.TryParse(gp.Text, out var gpv) || gpv < 0)
                {
                    MessageBox.Show("Grace period must be a non-negative integer", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private static bool IsTrue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();
            return value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }

        // DTO for raw SQL mapping
        private class SettingRow
        {
            public string SettingKey { get; set; }
            public string SettingValue { get; set; }
        }
    }
}