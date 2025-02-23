using Bank.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bank
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BankAccount> BankAccounts = new List<BankAccount>();

        private List<Transactions> deposits = new List<Transactions>();
        private List<Transactions> withdrawals = new List<Transactions>();
        private List<Transactions> transfers = new List<Transactions>();

        private const string AccountsFile = "bankUsers.json";

        public MainWindow()
        {
            InitializeComponent();
            BirthDatePicker.DisplayDateEnd = DateTime.Now.AddYears(-14);
            BirthDatePicker.DisplayDateStart = DateTime.Now.AddYears(-100);
            UpdateUserComboBox();
        }

        private void OpenAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string passport = PassportTextBox.Text;
            DateTime? birthDate = BirthDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(passport) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (fullName.Any(char.IsDigit))
            {
                MessageBox.Show("Поле ФИО не должно содержать цифр", "Ошибка", MessageBoxButton.OK);
                FullNameTextBox.Clear();
                return;
            }

            var bankAccount = new BankAccount(string.Empty, DateTime.Now, fullName, passport, birthDate.Value, 0, DateTime.Now.AddYears(10));
            bankAccount.GenerateAccountNumber();
            BankAccounts.Add(bankAccount);

            UpdateUserComboBox();

            FullNameTextBox.Clear();
            PassportTextBox.Clear();
            BirthDatePicker.SelectedDate = null;

            MessageBox.Show("Счет успешно открыт!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem != null)
            {
                string selectedUserName = UserComboBox.SelectedItem.ToString();
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

                if (selectedAccount != null && selectedAccount.Status != BankAccount.AccountStatus.закрыт)
                {
                    selectedAccount.CloseAccount();

                    OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                             "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                             "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                             "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                             "Статус счета: " + selectedAccount.Status + Environment.NewLine;

                    if (UserComboBox.SelectedItem == User2ComboBox.SelectedItem)
                    {
                        OutputUser2TextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                         "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                         "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                         "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                         "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                         "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                         "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                         "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                         "Статус счета: " + selectedAccount.Status + Environment.NewLine;
                    }

                    MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Счет уже закрыт или не существует.", "Ошибка", MessageBoxButton.OK);
                }
            }
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem != null && decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                string selectedUserName = UserComboBox.SelectedItem.ToString();
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

                if (selectedAccount != null)
                {
                    selectedAccount.Balance += amount;

                    Transactions newTransaction = new Transactions(
                        accountNumber: selectedAccount.AccountNumber,
                        operation: Transactions.OperationType.Пополнение,
                        timestamp: DateTime.Now,
                        isSuccessful: true,
                        amount: amount,
                        getterAccountNumber: "",
                        senderAccountName: "",
                        getterAccountName: "");

                    AddTransaction(newTransaction);

                    OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                             "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                             "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                             "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                             "Статус счета: " + selectedAccount.Status + Environment.NewLine;

                    if (UserComboBox.SelectedItem == User2ComboBox.SelectedItem)
                    {
                        OutputUser2TextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                         "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                         "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                         "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                         "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                         "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                         "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                         "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                         "Статус счета: " + selectedAccount.Status + Environment.NewLine;
                    }

                    AmountTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox.Clear();
            }
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem != null && decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                string selectedUserName = UserComboBox.SelectedItem.ToString();
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

                if (selectedAccount != null)
                {
                    if (amount <= selectedAccount.Balance)
                    {
                        selectedAccount.Balance -= amount;

                        Transactions newTransaction = new Transactions(
                        accountNumber: selectedAccount.AccountNumber,
                        operation: Transactions.OperationType.Снятие,
                        timestamp: DateTime.Now,
                        isSuccessful: true,
                        amount: amount,
                        getterAccountNumber: "",
                        senderAccountName: "",
                        getterAccountName: "");

                        AddTransaction(newTransaction);

                        OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                             "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                             "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                             "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                             "Статус счета: " + selectedAccount.Status + Environment.NewLine;

                        if (UserComboBox.SelectedItem == User2ComboBox.SelectedItem)
                        {
                            OutputUser2TextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                             "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                             "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                             "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                             "Статус счета: " + selectedAccount.Status + Environment.NewLine;
                        }

                        AmountTextBox.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Сумма снятия не может быть больше суммы баланса!", "Ошибка", MessageBoxButton.OK);
                        AmountTextBox.Clear();
                    }
                }

                else
                {
                    MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox.Clear();
            }
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem != null && User2ComboBox.SelectedItem != null && decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                string senderUserName = UserComboBox.SelectedItem.ToString();
                string receiverUserName = User2ComboBox.SelectedItem.ToString();

                var senderUser = BankAccounts.FirstOrDefault(user => user.FullName == senderUserName);
                var senderAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == senderUserName);

                var receiverUser = BankAccounts.FirstOrDefault(user => user.FullName == receiverUserName);
                var receiverAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == receiverUserName);

                if (UserComboBox.SelectedItem != User2ComboBox.SelectedItem)
                {
                    if (senderAccount != null && receiverAccount != null)
                    {
                        if (amount <= senderAccount.Balance)
                        {
                            senderAccount.Balance -= amount;
                            receiverAccount.Balance += amount;

                            Transactions newTransaction = new Transactions(
                             accountNumber: senderAccount.AccountNumber,
                             operation: Transactions.OperationType.Перевод,
                             timestamp: DateTime.Now,
                             isSuccessful: true,
                             amount: amount,
                             getterAccountNumber: receiverAccount.AccountNumber,
                             senderAccountName: senderUserName,
                             getterAccountName: receiverUserName);

                            AddTransaction(newTransaction);

                            OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                                 "ФИО: " + senderUser.FullName + Environment.NewLine +
                                                 "Текущий баланс: " + senderAccount.Balance + Environment.NewLine +
                                                 "Паспорт: " + senderUser.PassportNumber + Environment.NewLine +
                                                 "Дата рождения: " + senderUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                                 "Дата открытия счета: " + senderAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                                 "Дата закрытия счета: " + senderAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                                 "Номер счета: " + senderAccount.AccountNumber + Environment.NewLine +
                                                 "Статус счета: " + senderAccount.Status + Environment.NewLine;

                            OutputUser2TextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                                 "ФИО: " + receiverUser.FullName + Environment.NewLine +
                                                 "Текущий баланс: " + receiverAccount.Balance + Environment.NewLine +
                                                 "Паспорт: " + receiverUser.PassportNumber + Environment.NewLine +
                                                 "Дата рождения: " + receiverUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                                 "Дата открытия счета: " + receiverAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                                 "Дата закрытия счета: " + receiverAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                                 "Номер счета: " + receiverAccount.AccountNumber + Environment.NewLine +
                                                 "Статус счета: " + receiverAccount.Status + Environment.NewLine;

                            AmountTextBox.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно средств для перевода!", "Ошибка", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оба пользователя должны иметь открытые счета!", "Ошибка", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не может перевести средства сам себе!", "Ошибка", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Введите корректную сумму и выберите обоих пользователей!", "Ошибка", MessageBoxButton.OK);
            }
        }

        private void TransactionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserComboBox == null || UserComboBox.SelectedItem == null || TransactionTypeComboBox == null || TransactionTypeComboBox.SelectedItem == null)
            {
                return;
            }

            if (UserComboBox.SelectedItem is string selectedUserName)
            {
                var bankAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);
                if (bankAccount != null)
                {
                    var transactions = GetFilteredTransactions(bankAccount);
                    UpdateTransactionOutput(bankAccount, OutputTransactionsTextBox, transactions);
                }
            }
        }

        private List<Transactions> GetFilteredTransactions(BankAccount account)
        {
            if (TransactionTypeComboBox == null || TransactionTypeComboBox.SelectedItem == null)
            {
                return new List<Transactions>();
            }

            var selectedType = (TransactionTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var allTransactions = deposits.Concat(withdrawals).Concat(transfers).ToList();

            allTransactions = allTransactions.Where(t => t.AccountNumber == account.AccountNumber).ToList();

            if (string.IsNullOrEmpty(selectedType) || selectedType == "Все")
            {
                return allTransactions;
            }

            var filteredTransactions = allTransactions.Where(t =>
            {
                switch (selectedType)
                {
                    case "Пополнение":
                        return t.Operation == Transactions.OperationType.Пополнение;
                    case "Снятие":
                        return t.Operation == Transactions.OperationType.Снятие;
                    case "Перевод":
                        return t.Operation == Transactions.OperationType.Перевод;
                    default:
                        return false;
                }
            }).ToList();

            return filteredTransactions;
        }

        private void AddTransaction(Transactions transaction)
        {
            switch (transaction.Operation)
            {
                case Transactions.OperationType.Пополнение:
                    deposits.Add(transaction);
                    break;
                case Transactions.OperationType.Снятие:
                    withdrawals.Add(transaction);
                    break;
                case Transactions.OperationType.Перевод:
                    transfers.Add(transaction);
                    break;
                default:
                    throw new InvalidOperationException("Неизвестный тип транзакции.");
            }

            var bankAccount = BankAccounts.FirstOrDefault(acc => acc.AccountNumber == transaction.AccountNumber);
            if (bankAccount != null)
            {
                UpdateTransactionOutput(bankAccount, OutputTransactionsTextBox, GetFilteredTransactions(bankAccount));
            }
        }

        private void UpdateTransactionOutput(BankAccount account, TextBox outputTransactionsTextBox, List<Transactions> transactionList)
        {
            if (account == null || transactionList == null)
            {
                outputTransactionsTextBox.Text = "Нет данных о транзакциях.";
                return;
            }

            if (transactionList.Count == 0)
            {
                outputTransactionsTextBox.Text = "Нет совершенных транзакций.";
                return;
            }

            StringBuilder output = new StringBuilder();

            foreach (var transaction in transactionList)
            {
                output.AppendLine(transaction.OutputTransaction());

                if (transaction.Operation == Transactions.OperationType.Перевод)
                {
                    output.AppendLine("Отправитель: " + transaction.SenderAccountName);
                    output.AppendLine("Перевод на счет: " + transaction.GetterAccountNumber);
                    output.AppendLine("Получатель: " + transaction.GetterAccountName);
                }

                output.AppendLine();
            }

            Dispatcher.Invoke(() =>
            {
                outputTransactionsTextBox.Text = output.ToString();
            });
        }

        private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserComboBox.SelectedItem != null)
            {
                string selectedUserName = UserComboBox.SelectedItem.ToString();
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

                if (selectedUser != null && selectedAccount != null)
                {
                    OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                             "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                             "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                             "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                             "Статус счета: " + selectedAccount.Status + Environment.NewLine;
                }
                else
                {
                    OutputUserTextBox.Text = "Пользователь не найден или у него нет счета.";
                }
            }
        }

        private void User2ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (User2ComboBox.SelectedItem is string selectedUserName)
            {
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

                if (selectedUser != null && selectedAccount != null)
                {
                    OutputUser2TextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             "ФИО: " + selectedUser.FullName + Environment.NewLine +
                                             "Текущий баланс: " + selectedAccount.Balance + Environment.NewLine +
                                             "Паспорт: " + selectedUser.PassportNumber + Environment.NewLine +
                                             "Дата рождения: " + selectedUser.DateBirth.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата открытия счета: " + selectedAccount.OpenDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Дата закрытия счета: " + selectedAccount.EndDate.ToString("dd-MM-yyyy") + Environment.NewLine +
                                             "Номер счета: " + selectedAccount.AccountNumber + Environment.NewLine +
                                             "Статус счета: " + selectedAccount.Status + Environment.NewLine;
                }
                else
                {
                    OutputUser2TextBox.Text = "Пользователь не найден или у него нет счета.";
                }
            }
        }

        private void UpdateUserComboBox()
        {
            UserComboBox.Items.Clear();
            User2ComboBox.Items.Clear();

            foreach (var user in BankAccounts)
            {
                UserComboBox.Items.Add(user.FullName);
                User2ComboBox.Items.Add(user.FullName);
            }

            if (BankAccounts.Any())
            {
                UserComboBox.SelectedIndex = 0;
                User2ComboBox.SelectedIndex = 0;
            }
        }

        private void SaveUsersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string resultJson = JsonConvert.SerializeObject(BankAccounts, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(AccountsFile, resultJson);
                MessageBox.Show("Данные успешно записаны в JSON!", "Успех", MessageBoxButton.OK);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения!", "Ошибка", MessageBoxButton.OK);
            }
        }

        private void LoadUsersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(AccountsFile))
                {
                    string resultJson = File.ReadAllText(AccountsFile);
                    BankAccounts = JsonConvert.DeserializeObject<List<BankAccount>>(resultJson);
                    MessageBox.Show("Данные успешно считаны из JSON!", "Успех", MessageBoxButton.OK);

                    UpdateUserComboBox();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при загрузке данных", "Ошибка", MessageBoxButton.OK);
            }
        }
    }
}