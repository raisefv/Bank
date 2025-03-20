using System.Linq;
using System;

public class BankCard
{
    public string CardNumber { get; set; }
    public double CardBalance { get; set; }
    public int PinCode { get; private set; }
    public string PhoneNumber { get; private set; }
    public double CashbackPercentage { get; private set; }

    public BankCard(string cardNumber, double initialBalance, int pinCode, string phoneNumber, double cashbackPercentage)
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

    public void Deposit(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма пополнения должна быть положительной.");

        CardBalance += amount;
    }

    public void Withdraw(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма снятия должна быть положительной.");

        if (amount > CardBalance)
            throw new InvalidOperationException("Недостаточно средств на карте.");

        CardBalance -= amount;
    }

    public void MakePurchaseWithCashback(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма покупки должна быть положительной.");

        if (amount > CardBalance)
            throw new InvalidOperationException("Недостаточно средств на карте.");

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
