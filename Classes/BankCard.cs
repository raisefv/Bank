using System;
using System.Linq;
using System.Collections.Generic;

namespace Bank.Classes
{
    class BankCard : BankAccount
    {
        public string CardNumber { get; set; }
        public double CardBalance { get; set; }
        public int PinCode { get; set; }
        public string PhoneNumber { get; set; }
        public double CashbackPercentage { get; set; }

        public BankCard(string accountNumber, DateTime openDate, string fullName, string passportNumber, DateTime dateBirth, double balance, DateTime endDate,
            string cardNumber, double cardBalance, int pinCode, string phoneNumber, double cashbackPercentage)
            : base(accountNumber, openDate, fullName, passportNumber, dateBirth, balance, endDate)
        {
            CardNumber = cardNumber;
            CardBalance = cardBalance;
            PinCode = pinCode;
            PhoneNumber = phoneNumber;
            CashbackPercentage = cashbackPercentage;
        }

        public string GenerateCardNumber()
        {
            Random random = new Random();
            int DigitOne = random.Next(1, 10);
            string DigitElse = string.Concat(Enumerable.Range(0, 15).Select(_ => random.Next(0, 10).ToString()));
            return DigitOne.ToString() + DigitElse;
        }

        public void Deposit(double amount)
        {
            CardBalance += amount;
        }

        public void Withdraw(double amount)
        { 
            CardBalance -= amount;
        }

        public void MakePurchaseWithCashback(double amount)
        {
            CardBalance -= amount;
            double cashback = amount * (CashbackPercentage / 100);
            CardBalance += cashback;
        }
    }
}