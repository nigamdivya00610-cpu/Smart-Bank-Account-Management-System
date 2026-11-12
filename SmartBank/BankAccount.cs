using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace SmartBank
{
    public class Transaction
    {
        public string Type { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }

        public Transaction(string type, double amount, DateTime date)
        {
            Type = type;
            Amount = amount;
            Date = date;
        }
    }

    public class BankAccount
    {
        public string AccountNumber { get; set; }
        public string AccountHolder { get; set; }
        public double Balance { get; private set; }

        private static string connStr =
            ConfigurationManager.ConnectionStrings["BankDBConnection"].ConnectionString;

        // Constructor Overloading
        public BankAccount(string accountNumber, string accountHolder)
        {
            AccountNumber = accountNumber;
            AccountHolder = accountHolder;
            Balance = 0;
        }

        // Constructor Overloading
        public BankAccount(string accountNumber, string accountHolder, double balance)
        {
            AccountNumber = accountNumber;
            AccountHolder = accountHolder;
            Balance = balance;
        }

        // Insert a new account row into SQL Server (Create)
        public void CreateAccount()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Accounts (AccountNumber, AccountHolder, Balance) " +
                    "VALUES (@accNo, @accHolder, 0)", con);

                cmd.Parameters.AddWithValue("@accNo", AccountNumber);
                cmd.Parameters.AddWithValue("@accHolder", AccountHolder);

                cmd.ExecuteNonQuery();
            }
        }

        // Load an existing account from SQL Server (Read)
        public static BankAccount Load(string accountNumber)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT AccountHolder, Balance FROM Accounts " +
                    "WHERE AccountNumber = @accNo", con);

                cmd.Parameters.AddWithValue("@accNo", accountNumber);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BankAccount(
                            accountNumber,
                            reader["AccountHolder"].ToString(),
                            Convert.ToDouble(reader["Balance"])
                        );
                    }
                }
            }

            return null;
        }

        // Update: add funds (Balance column) and log the transaction
        public void Deposit(double amount)
        {
            if (amount <= 0)
                throw new Exception("Deposit amount must be greater than zero.");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Accounts SET Balance = Balance + @amt " +
                    "WHERE AccountNumber = @accNo", con);

                cmd.Parameters.AddWithValue("@amt", amount);
                cmd.Parameters.AddWithValue("@accNo", AccountNumber);

                cmd.ExecuteNonQuery();

                LogTransaction("Deposit", amount, con);
            }

            Balance += amount;
        }

        // Update: subtract funds (Balance column) and log the transaction
        public void Withdraw(double amount)
        {
            if (amount <= 0)
                throw new Exception("Withdrawal amount must be greater than zero.");

            if (amount > Balance)
                throw new Exception("Insufficient balance.");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Accounts SET Balance = Balance - @amt " +
                    "WHERE AccountNumber = @accNo", con);

                cmd.Parameters.AddWithValue("@amt", amount);
                cmd.Parameters.AddWithValue("@accNo", AccountNumber);

                cmd.ExecuteNonQuery();

                LogTransaction("Withdrawal", amount, con);
            }

            Balance -= amount;
        }

        // Insert a row into Transactions (Create)
        private void LogTransaction(
            string type,
            double amount,
            SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Transactions " +
                "(AccountNumber, TransactionType, Amount, TransactionDate) " +
                "VALUES (@accNo, @type, @amt, @date)", con);

            cmd.Parameters.AddWithValue("@accNo", AccountNumber);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@amt", amount);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        // Read: full transaction history for this account
        public List<Transaction> GetTransactions()
        {
            List<Transaction> list = new List<Transaction>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT TransactionType, Amount, TransactionDate " +
                    "FROM Transactions " +
                    "WHERE AccountNumber = @accNo " +
                    "ORDER BY TransactionDate", con);

                cmd.Parameters.AddWithValue("@accNo", AccountNumber);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Transaction(
                            reader["TransactionType"].ToString(),
                            Convert.ToDouble(reader["Amount"]),
                            Convert.ToDateTime(reader["TransactionDate"])
                        ));
                    }
                }
            }

            return list;
        }

        // AI feature: get a short saving suggestion from the Ollama Cloud API

        // AI feature: Get saving suggestions from Ollama Local
        public async System.Threading.Tasks.Task<string> GetSavingSuggestion()
        {
            string prompt =
                "You are a simple banking assistant. " +
                "Give one short and general saving suggestion to a bank customer. " +
                "The customer's current balance is ₹" + Balance + ". " +
                "Do not give investment or financial product advice. " +
                "Give the answer in simple language.";

            var requestData = new
            {
                model = "llama3.2",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                stream = false
            };

            string json = JsonConvert.SerializeObject(requestData);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);

                StringContent content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response = await client.PostAsync(
                    "http://localhost:11434/api/chat",
                    content
                );

                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("AI Error: " + result);
                }

                dynamic data = JsonConvert.DeserializeObject(result);

                return data.message.content.ToString();
            }
        }
    }
}