using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Classes
{
    class BankAccount
    {
        public string AccountNumber { get; set; }
        public DateTime OpenDate { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime DateBirth { get; set; }
        public decimal Balance { get; set; }
        public DateTime EndDate { get; set; }
        public AccountStatus Status { get; set; }

        public enum AccountStatus { открыт, закрыт, банкрот }
        public BankAccount(string accountNumber, DateTime openDate, string fullName, string passportNumber, DateTime dateBirth, decimal balance, DateTime endDate)
        {
            AccountNumber = accountNumber;
            OpenDate = openDate;
            FullName = fullName;
            PassportNumber = passportNumber;
            DateBirth = dateBirth;
            Balance = balance;
            EndDate = endDate;
            Status = AccountStatus.открыт;
        }

        public void GenerateAccountNumber()
        {
            Random random = new Random();
            int DigitOne = random.Next(1, 10);
            string DigitElse = string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));
            AccountNumber = DigitOne.ToString() + DigitElse;
        }
        public void CloseAccount()
        {
            Status = AccountStatus.закрыт;
            Balance = 0;
        }
        public string OutputUser()
        {
            return "ФИО: " + this.FullName + Environment.NewLine +
                "Дата рождения: " + this.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                "Номер счета: " + this.AccountNumber + Environment.NewLine +
                "Дата открытия счета: " + this.OpenDate.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine +
                "Дата закрытия счета: " + this.EndDate.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine +
                "Баланс: " + this.Balance + Environment.NewLine +
                "Статус: счет " + this.Status + Environment.NewLine;
        }
        public static BankAccount operator +(BankAccount account, decimal amount)
        {
            account.Balance += amount;
            return account;
        }
        public static BankAccount operator -(BankAccount account, decimal amount)
        {
            account.Balance -= amount;
            return account;
        }
    }
}