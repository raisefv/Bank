﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Classes
{
    public class BankAccount
    {
        public string AccountNumber { get; set; }
        public DateTime OpenDate { get; set; }
        public Client AccountHolder { get; set; }
        public decimal Balance { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; private set; }

        public BankAccount(string accountNumber, DateTime openDate, Client accountHolder, decimal initialBalance, DateTime endDate)
        {
            AccountNumber = accountNumber;
            OpenDate = openDate;
            AccountHolder = accountHolder;
            Balance = initialBalance;
            EndDate = endDate;
            Status = " счет открыт";
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
            Status = "счет закрыт";
            Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            Balance -= amount;
        }

        public string OutputUser()
        {
            return "ФИО: " + this.AccountHolder.FullName + Environment.NewLine +
                "Номер счета: " + this.AccountNumber + Environment.NewLine +
                "Дата открытия счета: " + this.OpenDate + Environment.NewLine +
                "Дата закрытия счета: " + this.EndDate + Environment.NewLine +
                "Баланс: " + this.Balance + Environment.NewLine +
                "Статус: " + this.Status + Environment.NewLine;
        }
    }
}