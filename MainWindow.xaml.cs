using Bank.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bank
{
    public partial class MainWindow : Window
    {
        private BankAccount bankAccount1;
        private BankAccount bankAccount2;

        private List<Transaction> deposits = new List<Transaction>();
        private List<Transaction> withdrawals = new List<Transaction>();
        private List<Transaction> transfers = new List<Transaction>();

        public MainWindow()
        {
            InitializeComponent();
            BirthDatePicker1.DisplayDateEnd = DateTime.Now.AddYears(-14);
            BirthDatePicker1.DisplayDateStart = DateTime.Now.AddYears(-100);
            BirthDatePicker2.DisplayDateEnd = DateTime.Now.AddYears(-14);
            BirthDatePicker2.DisplayDateStart = DateTime.Now.AddYears(-100);
        }

        private void SwitchToUser1Button_Click(object sender, RoutedEventArgs e)
        {
            User1Grid.Visibility = Visibility.Visible;
            User2Grid.Visibility = Visibility.Collapsed;
        }

        private void SwitchToUser2Button_Click(object sender, RoutedEventArgs e)
        {
            User1Grid.Visibility = Visibility.Collapsed;
            User2Grid.Visibility = Visibility.Visible;
        }

        private void OpenAccountButton1_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox1.Text;
            string passport = PassportTextBox1.Text;
            DateTime? birthDate = BirthDatePicker1.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (fullName.Any(char.IsDigit))
            {
                MessageBox.Show("Поле ФИО не должно содержать цифр", "Ошибка", MessageBoxButton.OK);
                FullNameTextBox1.Clear();
                return;
            }

            Client client = new Client(fullName, passport, birthDate.Value);
            bankAccount1 = new BankAccount(string.Empty, DateTime.Now, client, 0, DateTime.Now.AddYears(10));
            bankAccount1.GenerateAccountNumber();

            OutputUserTextBox1.Text = bankAccount1.OutputUser();
            FullNameTextBox1.Clear();
            PassportTextBox1.Clear();
            BirthDatePicker1.SelectedDate = null;
        }

        private void OpenAccountButton2_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox2.Text;
            string passport = PassportTextBox2.Text;
            DateTime? birthDate = BirthDatePicker2.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (fullName.Any(char.IsDigit))
            {
                MessageBox.Show("Поле ФИО не должно содержать цифр", "Ошибка", MessageBoxButton.OK);
                FullNameTextBox2.Clear();
                return;
            }

            Client client = new Client(fullName, passport, birthDate.Value);
            bankAccount2 = new BankAccount(string.Empty, DateTime.Now, client, 0, DateTime.Now.AddYears(10));
            bankAccount2.GenerateAccountNumber();

            OutputUserTextBox2.Text = bankAccount2.OutputUser();
            FullNameTextBox2.Clear();
            PassportTextBox2.Clear();
            BirthDatePicker2.SelectedDate = null;
        }

        private void DepositButton1_Click(object sender, RoutedEventArgs e)
        {
            ProcessDeposit(bankAccount1, AmountTextBox1, OutputTransactionsTextBox1, OutputUserTextBox1);
        }

        private void DepositButton2_Click(object sender, RoutedEventArgs e)
        {
            ProcessDeposit(bankAccount2, AmountTextBox2, OutputTransactionsTextBox2, OutputUserTextBox2);
        }

        private void ProcessDeposit(BankAccount account, TextBox amountTextBox, TextBox transactionOutput, TextBox userOutput)
        {
            if (account == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                amountTextBox.Clear();
                return;
            }

            if (decimal.TryParse(amountTextBox.Text, out decimal amount) && amount > 0)
            {
                account += amount;

                var transaction = new Transaction(
                    account.AccountNumber,
                    Transaction.OperationType.Пополнение,
                    DateTime.Now,
                    true,
                    amount,
                    null,
                    null);

                AddTransaction(transaction);
                UpdateTransactionOutput(account, transactionOutput, deposits);

                userOutput.Text = account.OutputUser();
                amountTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                amountTextBox.Clear();
            }
        }
        private void ProcessWithdrawal(BankAccount account, TextBox amountTextBox, TextBox transactionOutput, TextBox userOutput)
        {
            if (account == null || account.Status == BankAccount.AccountStatus.закрыт)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (decimal.TryParse(amountTextBox.Text, out decimal amount) && amount > 0 && amount <= account.Balance)
            {
                account -= amount;

                var transaction = new Transaction(
                    account.AccountNumber,
                    Transaction.OperationType.Снятие,
                    DateTime.Now,
                    true,
                    amount,
                    null,
                    null);

                AddTransaction(transaction);
                UpdateTransactionOutput(account, transactionOutput, withdrawals);

                userOutput.Text = account.OutputUser();
                amountTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                amountTextBox.Clear();
            }
        }


        private void WithdrawButton1_Click(object sender, RoutedEventArgs e)
        {
            ProcessWithdrawal(bankAccount1, AmountTextBox1, OutputTransactionsTextBox1, OutputUserTextBox1);
        }

        private void WithdrawButton2_Click(object sender, RoutedEventArgs e)
        {
            ProcessWithdrawal(bankAccount2, AmountTextBox2, OutputTransactionsTextBox2, OutputUserTextBox2);
        }
        private void CloseAccountButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            bankAccount1.CloseAccount();
            OutputUserTextBox1.Text = bankAccount1.OutputUser();
            MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseAccountButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            bankAccount2.CloseAccount();
            OutputUserTextBox2.Text = bankAccount2.OutputUser();
            MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK);
        }

        private void UpdateTransactionOutput(BankAccount account, TextBox outputTransactionsTextBox, List<Transaction> transactionList)
        {
            if (account == null)
            {
                outputTransactionsTextBox.Text = "Нет данных о транзакциях.";
                return;
            }

            var userTransactions = transactionList
                .Where(t => t.AccountNumber == account.AccountNumber)
                .ToList();

            if (userTransactions.Count == 0)
            {
                outputTransactionsTextBox.Text = "Нет совершенных транзакций.";
            }
            else
            {
                outputTransactionsTextBox.Text = string.Empty;

                foreach (var transaction in userTransactions)
                {
                    string transactionInfo = transaction.OutputTransaction();

                    if (transaction.Operation == Transaction.OperationType.Перевод)
                    {
                        transactionInfo += $"Перевод на счет: {transaction.GetterAccountNumber}{Environment.NewLine}" +
                                           $"Получатель: {transaction.GetterAccountName}{Environment.NewLine}";
                    }

                    outputTransactionsTextBox.Text += transactionInfo + Environment.NewLine;
                }
            }
        }

        private void AddTransaction(Transaction transaction)
        {
            switch (transaction.Operation)
            {
                case Transaction.OperationType.Пополнение:
                    deposits.Add(transaction);
                    break;
                case Transaction.OperationType.Снятие:
                    withdrawals.Add(transaction);
                    break;
                case Transaction.OperationType.Перевод:
                    transfers.Add(transaction);
                    break;
                default:
                    throw new InvalidOperationException("Неизвестный тип транзакции.");
            }
        }
        private void TransferButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null || bankAccount1.Status == BankAccount.AccountStatus.закрыт || bankAccount2 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (decimal.TryParse(AmountTextBox1.Text, out decimal amount))
            {
                if (amount <= 0 || amount > bankAccount1.Balance)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                    AmountTextBox2.Clear();
                    return;
                }

                bankAccount1 -= amount;
                bankAccount2 += amount;

                var transaction = new Transaction(
                    bankAccount1.AccountNumber,
                    Transaction.OperationType.Перевод,
                    DateTime.Now,
                    true,
                    amount,
                    bankAccount2.AccountNumber,
                    bankAccount2.AccountHolder.FullName);

                AddTransaction(transaction);
                UpdateTransactionOutput(bankAccount1, OutputTransactionsTextBox1, transfers);

                OutputUserTextBox1.Text = bankAccount1.OutputUser();
                OutputUserTextBox2.Text = bankAccount2.OutputUser();
                AmountTextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox1.Clear();
            }
        }
        private void TransferButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null || bankAccount2.Status == BankAccount.AccountStatus.закрыт || bankAccount1 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (decimal.TryParse(AmountTextBox2.Text, out decimal amount))
            {
                if (amount <= 0 || amount > bankAccount2.Balance)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                    AmountTextBox2.Clear();
                    return;
                }

                bankAccount2 -= amount;
                bankAccount1 += amount;

                var transaction = new Transaction(
                    bankAccount2.AccountNumber,
                    Transaction.OperationType.Перевод,
                    DateTime.Now,
                    true,
                    amount,
                    bankAccount1.AccountNumber,
                    bankAccount1.AccountHolder.FullName);

                AddTransaction(transaction);
                UpdateTransactionOutput(bankAccount2, OutputTransactionsTextBox2, transfers);

                OutputUserTextBox1.Text = bankAccount1.OutputUser();
                OutputUserTextBox2.Text = bankAccount2.OutputUser();
                AmountTextBox2.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox2.Clear();
            }
        }
    }
}