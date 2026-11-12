using System;

namespace SmartBank
{
    public partial class Default : System.Web.UI.Page
    {
        // Session only remembers WHICH account is active;
        // the account data itself lives in SQL Server.
        private string CurrentAccountNumber
        {
            get
            {
                return Session["AccountNumber"] as string;
            }

            set
            {
                Session["AccountNumber"] = value;
            }
        }

        // Create Account
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAccountNumber.Text == "" ||
                    txtAccountHolder.Text == "")
                {
                    throw new Exception(
                        "Please enter account number and account holder name.");
                }

                BankAccount account =
                    new BankAccount(
                        txtAccountNumber.Text.Trim(),
                        txtAccountHolder.Text.Trim());

                account.CreateAccount();

                CurrentAccountNumber = account.AccountNumber;

                lblMessage.Text =
                    "Account created successfully.<br/>" +
                    "Account Number: " + account.AccountNumber +
                    "<br/>Account Holder: " + account.AccountHolder;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    lblMessage.Text =
                        "Error: This account number already exists. " +
                        "Please enter a different account number.";
                }
                else
                {
                    lblMessage.Text =
                        "Database Error: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Error: " + ex.Message;
            }
        }

        // Deposit
        protected void btnDeposit_Click(object sender, EventArgs e)
        {
            try
            {
                BankAccount account =
                    LoadCurrentAccount();

                double amount =
                    Convert.ToDouble(txtAmount.Text);

                account.Deposit(amount);

                lblMessage.Text =
                    "₹" + amount +
                    " deposited successfully.<br/>" +
                    "Current Balance: ₹" +
                    account.Balance;
            }
            catch (FormatException)
            {
                lblMessage.Text =
                    "Error: Please enter a valid number.";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Error: " + ex.Message;
            }
        }

        // Withdraw
        protected void btnWithdraw_Click(object sender, EventArgs e)
        {
            try
            {
                BankAccount account =
                    LoadCurrentAccount();

                double amount =
                    Convert.ToDouble(txtAmount.Text);

                account.Withdraw(amount);

                lblMessage.Text =
                    "₹" + amount +
                    " withdrawn successfully.<br/>" +
                    "Current Balance: ₹" +
                    account.Balance;
            }
            catch (FormatException)
            {
                lblMessage.Text =
                    "Error: Please enter a valid number.";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Error: " + ex.Message;
            }
        }

        // Display Balance
        protected void btnBalance_Click(object sender, EventArgs e)
        {
            try
            {
                BankAccount account =
                    LoadCurrentAccount();

                lblMessage.Text =
                    "<b>Account Details</b><br/>" +
                    "Account Number: " +
                    account.AccountNumber +
                    "<br/>Account Holder: " +
                    account.AccountHolder +
                    "<br/>Current Balance: ₹" +
                    account.Balance;
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Error: " + ex.Message;
            }
        }

        // Display Transactions
        protected void btnTransactions_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                BankAccount account =
                    LoadCurrentAccount();

                GridView1.DataSource =
                    account.GetTransactions();

                GridView1.DataBind();

                lblMessage.Text =
                    "<b>Transaction History</b>";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Error: " + ex.Message;
            }
        }

        // Saving Suggestions
        protected async void btnSaving_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                BankAccount account =
                    LoadCurrentAccount();

                string suggestion =
                    await account.GetSavingSuggestion();

                lblMessage.Text =
                    "<b>AI Saving Suggestion</b><br/><br/>" +
                    suggestion;
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Error: " + ex.Message;
            }
        }

        // Loads the active account fresh from SQL Server on every postback
        private BankAccount LoadCurrentAccount()
        {
            string accountNumber = CurrentAccountNumber;

            // If session is empty, take account number from the textbox
            if (string.IsNullOrEmpty(accountNumber))
            {
                accountNumber = txtAccountNumber.Text.Trim();

                if (string.IsNullOrEmpty(accountNumber))
                    throw new Exception("Please enter an account number.");
            }

            BankAccount account = BankAccount.Load(accountNumber);

            if (account == null)
                throw new Exception("Account not found.");

            // Remember this account for the current session
            CurrentAccountNumber = account.AccountNumber;

            return account;
        }
    }
}