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

        public MainWindow()
        {
            InitializeComponent();
            BirthDatePicker.DisplayDateEnd = DateTime.Now.AddYears(-14);
            BirthDatePicker.DisplayDateStart = DateTime.Now.AddYears(-100);
            UpdateUserComboBox();
        }

        private void OpenAccountButton(object sender, RoutedEventArgs e)
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

            var bankAccount = new BankAccount(BankAccount.GenerateAccountNumber(), DateTime.Now, fullName, passport, birthDate.Value, 0, DateTime.Now.AddYears(10));
            BankAccounts.Add(bankAccount);

            UpdateUserComboBox();

            FullNameTextBox.Clear();
            PassportTextBox.Clear();
            BirthDatePicker.SelectedDate = null;

            MessageBox.Show("Счет успешно открыт!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseAccountButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string selectedUserName = UserComboBox.SelectedItem.ToString();
            var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

            if (selectedAccount == null)
            {
                MessageBox.Show("Ошибка: Счет не найден!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (selectedAccount.Status == BankAccount.AccountStatus.Закрыт)
            {
                MessageBox.Show("Счет уже закрыт.", "Ошибка", MessageBoxButton.OK);
                return;
            }

            selectedAccount.CloseAccount();

            OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine + 
                                    selectedAccount.GetAccountInfo();

            MessageBox.Show("Счет успешно закрыт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DepositButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null || CardComboBox.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите пользователя и карту!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (!double.TryParse(AmountTextBox.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string selectedUserName = UserComboBox.SelectedItem.ToString();
            var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

            if (selectedAccount == null)
            {
                MessageBox.Show("Ошибка при поиске счета!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox.SelectedItem.ToString());

            if (selectedCard == null)
            {
                MessageBox.Show("Ошибка при поиске карты!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardPINTextBox.Text) || !int.TryParse(CardPINTextBox.Text, out int enteredPin))
            {
                MessageBox.Show("Введите корректный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (enteredPin != selectedCard.PinCode)
            {
                MessageBox.Show("Неверный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            selectedCard.Deposit(amount);
            selectedAccount.Balance += amount;

            Transactions newTransaction = new Transactions(
                accountNumber: selectedAccount.AccountNumber,
                operation: Transactions.OperationType.Пополнение,
                timestamp: DateTime.Now,
                isSuccessful: true,
                amount: amount,
                getterAccountNumber: selectedAccount.AccountNumber,
                senderAccountName: selectedUserName,
                getterAccountName: selectedUserName);

            AddTransaction(newTransaction);

            OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                         selectedAccount.GetAccountInfo() + Environment.NewLine +
                         selectedCard.GetCardInfo();

            MessageBox.Show("Операция выполнена успешно!", "Успех", MessageBoxButton.OK);

            AmountTextBox.Clear();
            CardPINTextBox.Clear();
        }

        private void WithdrawButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null || CardComboBox.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите пользователя и карту!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (!double.TryParse(AmountTextBox.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string selectedUserName = UserComboBox.SelectedItem.ToString();
            var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

            if (selectedAccount == null)
            {
                MessageBox.Show("Ошибка при поиске счета!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox.SelectedItem.ToString());

            if (selectedCard == null)
            {
                MessageBox.Show("Ошибка при поиске карты!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardPINTextBox.Text) || !int.TryParse(CardPINTextBox.Text, out int enteredPin))
            {
                MessageBox.Show("Введите корректный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (enteredPin != selectedCard.PinCode)
            {
                MessageBox.Show("Неверный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (selectedCard.CardBalance < amount)
            {
                MessageBox.Show("Недостаточно средств на карте!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            selectedCard.Withdraw(amount);
            selectedAccount.Balance -= amount;

            Transactions newTransaction = new Transactions(
                accountNumber: selectedAccount.AccountNumber,
                operation: Transactions.OperationType.Снятие,
                timestamp: DateTime.Now,
                isSuccessful: true,
                amount: amount,
                getterAccountNumber: "",
                senderAccountName: selectedUserName,
                getterAccountName: "");

            AddTransaction(newTransaction);

            OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                     selectedAccount.GetAccountInfo() + Environment.NewLine +
                                     selectedCard.GetCardInfo();

            MessageBox.Show("Операция выполнена успешно!", "Успех", MessageBoxButton.OK);

            AmountTextBox.Clear();
            CardPINTextBox.Clear();
        }

        private void TransferButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null || UserComboBox2.SelectedItem == null)
            {
                MessageBox.Show("Выберите обоих пользователей!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (!double.TryParse(AmountTextBox.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string senderUserName = UserComboBox.SelectedItem.ToString();
            string receiverUserName = UserComboBox2.SelectedItem.ToString();

            if (senderUserName == receiverUserName)
            {
                MessageBox.Show("Нельзя перевести средства самому себе!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var senderAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == senderUserName);
            var receiverAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == receiverUserName);

            if (senderAccount == null || receiverAccount == null)
            {
                MessageBox.Show("Оба пользователя должны иметь открытые счета!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var senderCard = senderAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox.SelectedItem?.ToString());
            var receiverCard = receiverAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox2.SelectedItem?.ToString());

            if (senderCard == null || receiverCard == null)
            {
                MessageBox.Show("Выберите корректные карты отправителя и получателя!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardPINTextBox.Text))
            {
                MessageBox.Show("Введите PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (Convert.ToInt64(CardPINTextBox.Text) != senderCard.PinCode)
            {
                MessageBox.Show("Неверный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (amount > senderCard.CardBalance)
            {
                MessageBox.Show("Недостаточно средств для перевода!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            senderCard.Withdraw(amount);
            senderAccount.Balance -= amount;
            receiverCard.Deposit(amount);
            receiverAccount.Balance += amount;

            Transactions newTransaction = new Transactions(
                accountNumber: senderAccount.AccountNumber,
                operation: Transactions.OperationType.Перевод,
                timestamp: DateTime.Now,
                isSuccessful: true,
                amount: amount,
                getterAccountNumber: receiverAccount.AccountNumber,
                senderAccountName: senderUserName,
                getterAccountName: receiverUserName
            );

            AddTransaction(newTransaction);

            OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine + 
                                senderAccount.GetAccountInfo() + Environment.NewLine +
                                senderCard.GetCardInfo();
            OutputUserTextBox2.Text = "Выбранный пользователь:" + Environment.NewLine + 
                                receiverAccount.GetAccountInfo() + Environment.NewLine +
                                receiverCard.GetCardInfo();

            MessageBox.Show("Перевод выполнен успешно!", "Успех", MessageBoxButton.OK);

            AmountTextBox.Clear();
            CardPINTextBox.Clear();
        }

        private void PurchaseWithCashbackButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null || CardComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя и карту!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (!double.TryParse(AmountTextBox.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", MessageBoxButton.OK);
                AmountTextBox.Clear();
                return;
            }

            string selectedUserName = UserComboBox.SelectedItem.ToString();
            var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

            if (selectedAccount == null)
            {
                MessageBox.Show("Сначала откройте счет!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox.SelectedItem?.ToString());

            if (selectedCard == null)
            {
                MessageBox.Show("Выберите корректную карту!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardPINTextBox.Text))
            {
                MessageBox.Show("Введите PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (Convert.ToInt64(CardPINTextBox.Text) != selectedCard.PinCode)
            {
                MessageBox.Show("Неверный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (amount > selectedCard.CardBalance)
            {
                MessageBox.Show("Недостаточно средств на карте!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            selectedCard.MakePurchaseWithCashback(amount);
            double cashback = amount * (selectedCard.CashbackPercentage / 100);
            selectedAccount.Balance += cashback;

            OutputUserTextBox.Text = "Выбранный пользователь:" + Environment.NewLine + selectedAccount.GetAccountInfo();

            MessageBox.Show($"Покупка на {amount} выполнена успешно! Кэшбэк: {cashback}", "Успех", MessageBoxButton.OK);

            AmountTextBox.Clear();
            CardPINTextBox.Clear();
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

        private void CreateCardButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя перед созданием карты.");
                return;
            }

            string phoneNumber = PhoneNumberTextBox.Text;
            string pinCodeText = CardPINTextBox.Text;

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+7\d{10}$"))
            {
                MessageBox.Show("Номер телефона должен быть в формате +7XXXXXXXXXX.");
                return;
            }

            if (string.IsNullOrEmpty(phoneNumber) || pinCodeText.Length != 4)
            {
                MessageBox.Show("Пожалуйста, введите номер телефона и 4-значный PIN-код.");
                return;
            }

            if (!int.TryParse(pinCodeText, out int pinCode))
            {
                MessageBox.Show("PIN-код должен быть числом.");
                return;
            }

            if (UserComboBox.SelectedItem is string selectedUserName)
            {
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);
                if (selectedUser == null)
                {
                    MessageBox.Show("Пользователь не найден.");
                    return;
                }

                var newCard = new BankCard(
                    cardNumber: BankCard.GenerateCardNumber(),
                    initialBalance: 0,
                    pinCode: pinCode,
                    phoneNumber: phoneNumber,
                    cashbackPercentage: 3
                );

                selectedUser.AddCard(newCard);

                UpdateCardComboBox(selectedUser.Cards, CardComboBox);

                PhoneNumberTextBox.Clear();
                CardPINTextBox.Clear();

                MessageBox.Show("Карта успешно создана!");
            }
        }

        private void RemoveCardButton(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem != null && CardComboBox.SelectedItem != null)
            {
                string selectedUserName = UserComboBox.SelectedItem.ToString();
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);

                if (selectedAccount != null)
                {
                    var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox.SelectedItem.ToString());

                    if (selectedCard != null)
                    {
                        if (selectedAccount.Cards.Count > 1)
                        {
                            selectedAccount.Cards.Remove(selectedCard);
                            MessageBox.Show("Карта успешно удалена!", "Успех", MessageBoxButton.OK);

                            CardComboBox.Items.Clear();
                            CardComboBox.ItemsSource = selectedAccount.Cards.Select(card => card.CardNumber).ToList();

                        }
                        else
                        {
                            MessageBox.Show("Нельзя удалить единственную карту!", "Ошибка", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выбранная карта не найдена!", "Ошибка", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя и карту!", "Ошибка", MessageBoxButton.OK);
            }
        }

        private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedUser(UserComboBox, CardComboBox, OutputUserTextBox);
            UpdateSelectedUser(UserComboBox2, CardComboBox2, OutputUserTextBox2);
        }

        private void UserComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedUser(UserComboBox, CardComboBox, OutputUserTextBox);
            UpdateSelectedUser(UserComboBox2, CardComboBox2, OutputUserTextBox2);
        }

        private void UpdateSelectedUser(ComboBox userComboBox, ComboBox cardComboBox, TextBox outputTextBox)
        {
            if (userComboBox?.SelectedItem is string selectedUserName)
            {
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == selectedUserName);

                if (selectedUser != null)
                {
                    outputTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                          selectedUser.GetAccountInfo();

                    UpdateCardComboBox(selectedUser.Cards, cardComboBox);
                }
                else
                {
                    outputTextBox.Text = "Пользователь не найден или у него нет счета.";
                    cardComboBox.ItemsSource = null;
                }
            }
        }

        private void CardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedCard(UserComboBox, CardComboBox, OutputUserTextBox);
        }

        private void CardComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedCard(UserComboBox2, CardComboBox2, OutputUserTextBox2);
        }

        private void UpdateSelectedCard(ComboBox userComboBox, ComboBox cardComboBox, TextBox outputTextBox)
        {
            if (cardComboBox.SelectedItem is string selectedCardNumber)
            {
                var selectedUser = BankAccounts.FirstOrDefault(user => user.FullName == userComboBox.SelectedItem as string);
                var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUser?.FullName);

                if (selectedAccount != null)
                {
                    var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == selectedCardNumber);

                    if (selectedCard != null)
                    {
                        outputTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             selectedAccount.GetAccountInfo() + Environment.NewLine +
                                             selectedCard.GetCardInfo();
                    }
                    else
                    {
                        outputTextBox.Text += "\nКарта не найдена.";
                    }
                }
            }
        }

        private void UpdateUserComboBox()
        {
            UserComboBox.Items.Clear();
            UserComboBox2.Items.Clear();

            foreach (var user in BankAccounts)
            {
                UserComboBox.Items.Add(user.FullName);
                UserComboBox2.Items.Add(user.FullName);
            }

            if (BankAccounts.Any())
            {
                UserComboBox.SelectedIndex = 0;
                UserComboBox2.SelectedIndex = 0;
            }
        }

        private void UpdateCardComboBox(List<BankCard> cards, ComboBox cardComboBox)
        {
            cardComboBox.Items.Clear();

            if (cards.Any())
            {
                foreach (var card in cards)
                {
                    cardComboBox.Items.Add(card.CardNumber);
                }
                cardComboBox.SelectedIndex = 0;
            }
            else
            {
                cardComboBox.Items.Add("Нет карт");
            }
        }

        private const string AccountsFile = "bankUsers.json";

        private void SaveUsersButton(object sender, RoutedEventArgs e)
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

        private void LoadUsersButton(object sender, RoutedEventArgs e)
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