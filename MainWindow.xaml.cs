using Bank.Classes;
using System;
using System.Windows;

namespace Bank
{
    public partial class MainWindow : Window
    {
        private BankAccount bankAccount1;
        private BankAccount bankAccount2;

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
            DateTime birthDate = Convert.ToDateTime(BirthDatePicker1.SelectedDate);

            for (int i = 0; i < fullName.Length; i++)
            {
                if (char.IsDigit(fullName[i]))
                {
                    MessageBox.Show("Поле ФИО не должо содержать цифр", "Ошибка", MessageBoxButton.OK);
                    FullNameTextBox1.Text = null;
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK);
                return;
            }

            Client client = new Client(fullName, passport, birthDate);
            bankAccount1 = new BankAccount(string.Empty, DateTime.Now, client, 0, DateTime.Now.AddYears(5));
            bankAccount1.GenerateAccountNumber();

            OutputTextBox1.Text = bankAccount1.OutputUser();

            FullNameTextBox1.Clear();
            PassportTextBox1.Clear();
            BirthDatePicker1.SelectedDate = null;
        }

        private void OpenAccountButton2_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox2.Text;
            string passport = PassportTextBox2.Text;
            DateTime birthDate = Convert.ToDateTime(BirthDatePicker2.SelectedDate);

            for (int i = 0; i < fullName.Length; i++)
            {
                if (char.IsDigit(fullName[i]))
                {
                    MessageBox.Show("Поле ФИО не должо содержать цифр", "Ошибка", MessageBoxButton.OK);
                    FullNameTextBox2.Text = null;
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK);
                return;
            }

            Client client = new Client(fullName, passport, birthDate);
            bankAccount2 = new BankAccount(string.Empty, DateTime.Now, client, 0, DateTime.Now.AddYears(5));
            bankAccount2.GenerateAccountNumber();

            OutputTextBox2.Text = bankAccount2.OutputUser();

            FullNameTextBox2.Clear();
            PassportTextBox2.Clear();
            BirthDatePicker2.SelectedDate = null;
        }

        private void DepositButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox1.Clear();
                return;
            }

            if (decimal.TryParse(AmountTextBox1.Text, out decimal amount) && amount > 0)
            {
                bankAccount1 += amount;
                //bankAccount1.Deposit(amount);
                OutputTextBox1.Text = bankAccount1.OutputUser();
                AmountTextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox1.Clear();
            }
        }

        private void DepositButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox2.Clear();
                return;
            }

            if (decimal.TryParse(AmountTextBox2.Text, out decimal amount) && amount > 0)
            {
                bankAccount2 += amount;
                //bankAccount2.Deposit(amount);
                OutputTextBox2.Text = bankAccount2.OutputUser();
                AmountTextBox2.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox2.Clear();
            }
        }

        private void WithdrawButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null || bankAccount1.Status == BankAccount.AccountStatus.закрыт)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (decimal.TryParse(AmountTextBox1.Text, out decimal amount))
            {
                if (amount <= 0 || amount > bankAccount1.Balance)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                    AmountTextBox1.Clear();
                    return;
                }

                bankAccount1 -= amount;
                //bankAccount1.Withdraw(amount);
                OutputTextBox1.Text = bankAccount1.OutputUser();
                AmountTextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox2.Clear();
            }
        }

        private void WithdrawButton2_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount2 == null || bankAccount2.Status == BankAccount.AccountStatus.закрыт)
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
                //bankAccount2.Withdraw(amount);
                OutputTextBox2.Text = bankAccount2.OutputUser();
                AmountTextBox2.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox2.Clear();
            }
        }

        private void CloseAccountButton1_Click(object sender, RoutedEventArgs e)
        {
            if (bankAccount1 == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            bankAccount1.CloseAccount();
            OutputTextBox1.Text = bankAccount1.OutputUser();
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
            OutputTextBox2.Text = bankAccount2.OutputUser();
            MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK);
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
                    AmountTextBox1.Clear();
                    return;
                }

                bankAccount1 -= amount;
                bankAccount2 += amount;
                //bankAccount1.Withdraw(amount);
                //bankAccount2.Deposit(amount);
                OutputTextBox1.Text = bankAccount1.OutputUser();
                OutputTextBox2.Text = bankAccount2.OutputUser();
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


            if (bankAccount2 == null || bankAccount2.Status == BankAccount.AccountStatus.закрыт)
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
                //bankAccount2.Withdraw(amount);
                //bankAccount1.Deposit(amount);
                OutputTextBox1.Text = bankAccount1.OutputUser();
                OutputTextBox2.Text = bankAccount2.OutputUser();
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