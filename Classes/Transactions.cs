using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bank.Classes
{
    public class Transaction
    {
        public string AccountNumber { get; set; }
        public OperationType Operation { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
        public decimal Amount { get; set; }
        public enum OperationType { Снятие, Пополнение, Перевод }
        public string GetterAccountNumber { get; set; }
        public string GetterAccountName { get; set; }

        public Transaction(string accountNumber, OperationType operation, DateTime timestamp, bool isSuccessful, decimal amount, string getterAccountNumber, string getterAccountName)
        {
            AccountNumber = accountNumber;
            Operation = operation;
            Timestamp = timestamp;
            IsSuccessful = isSuccessful;
            Amount = amount;
            GetterAccountNumber = getterAccountNumber;
            GetterAccountName = getterAccountName;
        }
        public string OutputTransaction()
        {
            string transactionInfo = "Операция: " + this.Operation + Environment.NewLine +
                                     "Счет: " + this.AccountNumber + Environment.NewLine +
                                     "Сумма: " + this.Amount + Environment.NewLine +
                                     "Результат транзакции: " + (this.IsSuccessful ? "успешно" : "неудача") + Environment.NewLine +
                                     "Дата: " + this.Timestamp.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine;

            return transactionInfo;
        }
    }
}
