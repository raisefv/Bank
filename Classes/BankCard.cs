using System;
using System.Linq;
using System.Windows;

public class BankCard : BankAccount
{
    public string CardNumber { get; set; }
    public double CardBalance { get; set; }
    public int PinCode { get; private set; }
    public string PhoneNumber { get; private set; }
    public double CashbackPercentage { get; private set; }

    public BankCard(string accountNumber, DateTime openDate, string fullName, string passportNumber, DateTime dateBirth, double balance, DateTime endDate,
                    string cardNumber, double initialBalance, int pinCode, string phoneNumber, double cashbackPercentage)
        : base(accountNumber, openDate, fullName, passportNumber, dateBirth, balance, endDate)
    {
        CardNumber = cardNumber;
        CardBalance = initialBalance;
        PinCode = pinCode;
        PhoneNumber = phoneNumber;
        CashbackPercentage = cashbackPercentage;
    }

    public static string GenerateCardNumber()
    {
        Random random = new Random();
        int firstDigit = random.Next(1, 10);
        string otherDigits = string.Concat(Enumerable.Range(0, 15).Select(_ => random.Next(0, 10).ToString()));
        return firstDigit.ToString() + otherDigits;
    }

    new public void Deposit(double amount)
    {
        if (amount <= 0)
        {
            MessageBox.Show("Сумма пополнения должна быть положительной.", "Ошибка", MessageBoxButton.OK);
            return;
        }

        CardBalance += amount;
    }

    new public void Withdraw(double amount)
    {
        if (amount <= 0)
        {
            MessageBox.Show("Сумма снятия должна быть положительной.", "Ошибка", MessageBoxButton.OK);
            return;
        }

        if (amount > CardBalance)
        {
            MessageBox.Show("Недостаточно средств на карте.", "Ошибка", MessageBoxButton.OK);
            return;
        }

        CardBalance -= amount;
    }

    public void MakePurchaseWithCashback(double amount)
    {
        if (amount <= 0)
        {
            MessageBox.Show("Сумма покупки должна быть положительной.", "Ошибка", MessageBoxButton.OK);
            return;
        }

        if (amount > CardBalance)
        {
            MessageBox.Show("Недостаточно средств на карте.", "Ошибка", MessageBoxButton.OK);
            return;
        }

        CardBalance -= amount;
        double cashback = amount * (CashbackPercentage / 100);
        CardBalance += cashback;
    }

    public string GetCardInfo()
    {
        return $"Номер карты: {CardNumber}{Environment.NewLine}" +
               $"Баланс карты: {CardBalance}{Environment.NewLine}" +
               $"Номер телефона: {PhoneNumber}{Environment.NewLine}" +
               $"Кешбек: {CashbackPercentage}%";
    }
}