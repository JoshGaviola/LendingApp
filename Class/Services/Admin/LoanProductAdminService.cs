using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using LendingApp.Class.Models.Loans;
using MySql.Data.MySqlClient;

namespace LendingApp.Class.Services.Admin
{
    public sealed class LoanProductAdminService
    {
        public int CreateLoanProduct(LoanProductCreateRequest req)
        {
            Validate(req);

            using (var db = new AppDbContext())
            {
                var exists = db.LoanProducts.AsNoTracking()
                    .Any(p => p.ProductName != null && p.ProductName.Equals(req.ProductName, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    throw new ValidationException("A loan product with the same name already exists.");

                var entity = new LoanProductEntity
                {
                    ProductName = req.ProductName,
                    Description = req.Description,

                    MinAmount = req.MinAmount,
                    MaxAmount = req.MaxAmount,

                    MinTermMonths = req.SelectedTerms.Min(),
                    MaxTermMonths = req.SelectedTerms.Max(),

                    InterestRate = req.InterestRate,

                    // existing schema column
                    ProcessingFeePct = req.ServiceFeePct,

                    PenaltyRate = req.PenaltyRatePct,
                    GracePeriodDays = req.GracePeriodDays,

                    IsActive = req.IsActive,
                    CreatedDate = DateTime.Now
                };

                db.LoanProducts.Add(entity);
                db.SaveChanges();

                SaveExtended(entity.ProductId, req);

                return entity.ProductId;
            }
        }

        public void UpdateLoanProduct(int productId, LoanProductCreateRequest req)
        {
            Validate(req);

            using (var db = new AppDbContext())
            {
                var entity = db.LoanProducts.SingleOrDefault(x => x.ProductId == productId);
                if (entity == null)
                    throw new ValidationException("Loan product not found.");

                // Allow keeping same name, but prevent collision with other products
                var existsOther = db.LoanProducts.AsNoTracking()
                    .Any(p =>
                        p.ProductId != productId &&
                        p.ProductName != null &&
                        p.ProductName.Equals(req.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existsOther)
                    throw new ValidationException("A loan product with the same name already exists.");

                entity.ProductName = req.ProductName;
                entity.Description = req.Description;

                entity.MinAmount = req.MinAmount;
                entity.MaxAmount = req.MaxAmount;

                entity.MinTermMonths = req.SelectedTerms.Min();
                entity.MaxTermMonths = req.SelectedTerms.Max();

                entity.InterestRate = req.InterestRate;
                entity.ProcessingFeePct = req.ServiceFeePct;

                entity.PenaltyRate = req.PenaltyRatePct;
                entity.GracePeriodDays = req.GracePeriodDays;

                entity.IsActive = req.IsActive;

                db.SaveChanges();

                // Update extended tables/columns as well
                SaveExtended(productId, req);
            }
        }

        private static void Validate(LoanProductCreateRequest req)
        {
            if (req == null) throw new ValidationException("Invalid request.");

            req.ProductName = (req.ProductName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(req.ProductName))
                throw new ValidationException("Please enter a valid Loan Type Name.");

            if (req.MaxAmount < req.MinAmount)
                throw new ValidationException("Max Loan Amount must be greater than or equal to Min Loan Amount.");

            if (req.MinAmount <= 0 || req.MaxAmount <= 0)
                throw new ValidationException("Min/Max Loan Amount must be greater than 0.");

            if (req.InterestRate <= 0)
                throw new ValidationException("Interest Rate must be greater than 0.");

            if (req.ServiceFeePct < 0 || req.ServiceFeePct > 100)
                throw new ValidationException("Service Fee (%) must be between 0 and 100.");

            if (req.PenaltyRatePct < 0 || req.PenaltyRatePct > 100)
                throw new ValidationException("Late Payment Penalty must be between 0 and 100.");

            if (req.GracePeriodDays < 0)
                throw new ValidationException("Grace Period cannot be negative.");

            if (req.SelectedTerms == null || req.SelectedTerms.Count == 0)
                throw new ValidationException("Please select at least one available term.");

            req.InterestType = (req.InterestType ?? "Fixed").Trim();
            req.InterestPeriod = (req.InterestPeriod ?? "Year").Trim();
            req.PenaltyPeriod = (req.PenaltyPeriod ?? "Month").Trim();

            if (req.Requirements != null)
            {
                var others = req.Requirements.FirstOrDefault(r => string.Equals(r.Key, "Others", StringComparison.OrdinalIgnoreCase));
                if (others != null)
                {
                    var t = (others.Text ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(t))
                        throw new ValidationException("Please specify the 'Others' required document.");
                }
            }
        }

        private static void SaveExtended(int productId, LoanProductCreateRequest req)
        {
            var cs = ConfigurationManager.ConnectionStrings["LendingAppDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ValidationException("Missing connection string: LendingAppDb");

            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    // extended columns on loan_products
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
UPDATE loan_products
SET
  interest_type = @interestType,
  interest_period = @interestPeriod,
  service_fee_fixed_amount = @serviceFeeFixed,
  penalty_period = @penaltyPeriod,
  requires_collateral = @requiresCollateral
WHERE product_id = @productId;";
                        cmd.Parameters.AddWithValue("@interestType", req.InterestType);
                        cmd.Parameters.AddWithValue("@interestPeriod", req.InterestPeriod);
                        cmd.Parameters.AddWithValue("@serviceFeeFixed", (object)req.ServiceFeeFixedAmount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@penaltyPeriod", req.PenaltyPeriod);
                        cmd.Parameters.AddWithValue("@requiresCollateral", req.RequiresCollateral ? 1 : 0);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.ExecuteNonQuery();
                    }

                    // terms
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "DELETE FROM loan_product_terms WHERE product_id = @productId;";
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var t in req.SelectedTerms.Distinct().OrderBy(x => x))
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "INSERT INTO loan_product_terms (product_id, term_months) VALUES (@productId, @term);";
                            cmd.Parameters.AddWithValue("@productId", productId);
                            cmd.Parameters.AddWithValue("@term", t);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // requirements
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "DELETE FROM loan_product_requirements WHERE product_id = @productId;";
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.ExecuteNonQuery();
                    }

                    if (req.Requirements != null)
                    {
                        foreach (var r in req.Requirements)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.Transaction = tx;
                                cmd.CommandText = @"
INSERT INTO loan_product_requirements (product_id, requirement_key, requirement_text, is_required)
VALUES (@productId, @key, @text, 1);";
                                cmd.Parameters.AddWithValue("@productId", productId);
                                cmd.Parameters.AddWithValue("@key", r.Key);
                                cmd.Parameters.AddWithValue("@text", (object)r.Text ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                }
            }
        }

        public static decimal ParseMoney(string text)
        {
            var cleaned = (text ?? "").Trim().Replace("₱", "").Replace(",", "");
            decimal v;
            if (!decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out v))
                throw new ValidationException("Invalid amount.");
            return v;
        }

        public static decimal ParseDecimal(string text)
        {
            var cleaned = (text ?? "").Trim().Replace("%", "");
            decimal v;
            if (!decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out v))
                throw new ValidationException("Invalid number.");
            return v;
        }

        public static int ParseInt(string text)
        {
            var cleaned = (text ?? "").Trim();
            int v;
            if (!int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
                throw new ValidationException("Invalid number.");
            return v;
        }
    }
}