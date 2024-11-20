using Bank.Classes;
using System;
using System.Windows;

namespace Bank
{
    public partial class MainWindow : Window
    {
        private BankAccount bankAccount1; // Счет пользователя 1
        private BankAccount bankAccount2; // Счет пользователя 2

        public MainWindow()
        {
            InitializeComponent();
        }

        // Переключение на пользователя 1
        private void SwitchToUser1Button_Click(object sender, RoutedEventArgs e)
        {
            User1Grid.Visibility = Visibility.Visible;
            User2Grid.Visibility = Visibility.Collapsed;
        }

        // Переключение на пользователя 2
        private void SwitchToUser2Button_Click(object sender, RoutedEventArgs e)
        {
            User1Grid.Visibility = Visibility.Collapsed;
            User2Grid.Visibility = Visibility.Visible;
        }

        // Открытие счета для пользователя 1
        private void OpenAccountButton1_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox1.Text;
            string passport = PassportTextBox1.Text;
            DateTime? birthDate = BirthDatePicker1.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Client client = new Client(fullName, passport, birthDate.Value);
            bankAccount1 = new BankAccount(string.Empty, DateTime.Now, client, 0, DateTime.Now.AddYears(5));
            bankAccount1.GenerateAccountNumber();

            OutputTextBox1.Text = bankAccount1.OutputUser();
        }

        // Открытие счета для пользователя 2
        private void OpenAccountButton2_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox2.Text;
            string passport = PassportTextBox2.Text;
            DateTime? birthDate = BirthDatePicker2.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Client client = new Client(fullName, passport, birthDate.Value);
            bankAccount2 = new BankAccount(string.Empty, DateTime.Now, client, 0, DateTime.Now.AddYears(5));
            bankAccount2.GenerateAccountNumber();

            OutputTextBox2.Text = bankAccount2.OutputUser();
        }

        // Пополнение счета пользователя 1
        private void DepositButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(AmountTextBox1.Text, out decimal amount) && amount > 0)
            {
                bankAccount1.Deposit(amount);
                OutputTextBox1.Text = bankAccount1.OutputUser();
                AmountTextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Пополнение счета пользователя 2
        private void DepositButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(AmountTextBox2.Text, out decimal amount) && amount > 0)
            {
                bankAccount2.Deposit(amount);
                OutputTextBox2.Text = bankAccount2.OutputUser();
                AmountTextBox2.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Снятие средств с пользователя 1
        private void WithdrawButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null || bankAccount1.Status == "счет закрыт")
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(AmountTextBox1.Text, out decimal amount))
            {
                if (amount <= 0 || amount > bankAccount1.Balance)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bankAccount1.Withdraw(amount);
                OutputTextBox1.Text = bankAccount1.OutputUser();
                AmountTextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Снятие средств с пользователя 2
        private void WithdrawButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null || bankAccount2.Status == "счет закрыт")
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(AmountTextBox2.Text, out decimal amount))
            {
                if (amount <= 0 || amount > bankAccount2.Balance)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bankAccount2.Withdraw(amount);
                OutputTextBox2.Text = bankAccount2.OutputUser();
                AmountTextBox2.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void TransferFunds(BankAccount fromAccount, BankAccount toAccount, decimal amount, string fromUser, string toUser)
        {
            if (fromAccount == null || toAccount == null)
            {
                MessageBox.Show($"Один из счетов не открыт! Проверьте {fromUser} или {toUser}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (amount <= 0)
            {
                MessageBox.Show("Сумма перевода должна быть больше нуля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (amount > fromAccount.Balance)
            {
                MessageBox.Show($"На счете {fromUser} недостаточно средств для перевода!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Совершение перевода
            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);

            MessageBox.Show($"Успешно переведено {amount:C} от {fromUser} к {toUser}!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //private void TransferButton1_Click(object sender, RoutedEventArgs e)
        //{
        //    if (decimal.TryParse(AmountTextBox1.Text, out decimal amount))
        //    {
        //        TransferFunds(user1Account, user2Account, amount, "Пользователь 1", "Пользователь 2");
        //        OutputTextBox1.Text = user1Account?.OutputUser();
        //        OutputTextBox2.Text = user2Account?.OutputUser();
        //        AmountTextBox1.Clear();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Введите корректную сумму для перевода!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        //private void TransferButton2_Click(object sender, RoutedEventArgs e)
        //{
        //    if (decimal.TryParse(AmountTextBox2.Text, out decimal amount))
        //    {
        //        TransferFunds(user2Account, user1Account, amount, "Пользователь 2", "Пользователь 1");
        //        OutputTextBox1.Text = user1Account?.OutputUser();
        //        OutputTextBox2.Text = user2Account?.OutputUser();
        //        AmountTextBox2.Clear();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Введите корректную сумму для перевода!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}


        // Закрытие счета пользователя 1
        private void CloseAccountButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bankAccount1.CloseAccount();
            OutputTextBox1.Text = bankAccount1.OutputUser();
            MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Закрытие счета пользователя 2
        private void CloseAccountButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bankAccount2.CloseAccount();
            OutputTextBox2.Text = bankAccount2.OutputUser();
            MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
