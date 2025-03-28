using Bank.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private List<Transactions> buys = new List<Transactions>();

        public MainWindow()
        {
            InitializeComponent();
            BirthDatePicker.DisplayDateEnd = DateTime.Now.AddYears(-14);
            BirthDatePicker.DisplayDateStart = DateTime.Now.AddYears(-100);
            UpdateUserComboBox();
            _ = GetRatesMethod();
            tbFrom.Text = "0";
            tbTo.Text = "0";
        }

        private void OpenAccountButton(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string passport = PassportTextBox.Text;
            DateTime ? birthDate  = BirthDatePicker.SelectedDate;

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
            selectedAccount.Deposit(amount);

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
            selectedAccount.Withdraw(amount);

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

            if (string.IsNullOrEmpty(CardPINTextBox.Text) || !int.TryParse(CardPINTextBox.Text, out int enteredPin))
            {
                MessageBox.Show("Введите корректный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (enteredPin != senderCard.PinCode)
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
            senderAccount.Withdraw(amount);
            receiverCard.Deposit(amount);
            receiverAccount.Deposit(amount);

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

            if (amount > selectedCard.CardBalance)
            {
                MessageBox.Show("Недостаточно средств на карте!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            selectedCard.MakePurchaseWithCashback(amount);
            selectedAccount.Balance -= amount;
            double cashback = amount * (selectedCard.CashbackPercentage / 100);
            selectedAccount.Balance += cashback;

            Transactions newTransaction = new Transactions(
                accountNumber: selectedAccount.AccountNumber,
                operation: Transactions.OperationType.Покупка,
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

            var allTransactions = deposits.Concat(withdrawals).Concat(transfers).Concat(buys).ToList();

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
                    case "Покупка с кешбеком":
                        return t.Operation == Transactions.OperationType.Покупка;
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
                case Transactions.OperationType.Покупка:
                    buys.Add(transaction);
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

            string output = "";

            foreach (var transaction in transactionList)
            {
                output += transaction.OutputTransaction() + Environment.NewLine;

                if (transaction.Operation == Transactions.OperationType.Перевод)
                {
                    output += "Отправитель: " + transaction.SenderAccountName + Environment.NewLine;
                    output += "Перевод на счет: " + transaction.GetterAccountNumber + Environment.NewLine;
                    output += "Получатель: " + transaction.GetterAccountName + Environment.NewLine;
                }

                output += Environment.NewLine;
            }

            Dispatcher.Invoke(() =>
            {
                outputTransactionsTextBox.Text = output;
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

            if (string.IsNullOrEmpty(phoneNumber) || pinCodeText.Length != 4)
            {
                MessageBox.Show("Пожалуйста, введите номер телефона и 4-значный PIN-код.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+7\d{10}$"))
            {
                MessageBox.Show("Номер телефона должен быть в формате +7XXXXXXXXXX.");
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
                    accountNumber: selectedUser.AccountNumber,
                    openDate: selectedUser.OpenDate,
                    fullName: selectedUser.FullName,
                    passportNumber: selectedUser.PassportNumber,
                    dateBirth: selectedUser.DateBirth,
                    balance: selectedUser.Balance,
                    endDate: selectedUser.EndDate,
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
            if (UserComboBox.SelectedItem == null || CardComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя и карту!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string selectedUserName = UserComboBox.SelectedItem.ToString();
            var selectedAccount = BankAccounts.FirstOrDefault(acc => acc.FullName == selectedUserName);
            var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == CardComboBox.SelectedItem as string);

            if (selectedAccount == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (selectedCard == null)
            {
                MessageBox.Show("Карта не выбрана!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (selectedAccount.Cards.Count == 1)
            {
                MessageBox.Show("Нельзя удалить единственную карту!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardPINTextBox.Text))
            {
                MessageBox.Show("Введите PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (!int.TryParse(CardPINTextBox.Text, out int enteredPin) || enteredPin != selectedCard.PinCode)
            {
                MessageBox.Show("Неверный PIN-код!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            selectedAccount.Cards.Remove(selectedCard);
            selectedAccount.UpdateAccountBalance();

            MessageBox.Show("Карта успешно удалена!", "Успех", MessageBoxButton.OK);

            CardPINTextBox.Clear();
            UpdateCardComboBox(selectedAccount.Cards, CardComboBox);
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
                var selectedCard = selectedAccount.Cards.FirstOrDefault(card => card.CardNumber == selectedCardNumber);

                if (selectedAccount != null)
                {
                    if (selectedCard != null)
                    {
                        outputTextBox.Text = "Выбранный пользователь:" + Environment.NewLine +
                                             selectedAccount.GetAccountInfo() + Environment.NewLine +
                                             selectedCard.GetCardInfo();
                    }
                    else
                    {
                        outputTextBox.Text += Environment.NewLine + "Карта не найдена.";
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
                MessageBox.Show("Данные успешно записаны в JSON!");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения!");
            }
        }

        private void LoadUsersButton(object sender, RoutedEventArgs e)
        {
            if (File.Exists(AccountsFile))
            {
                string resultJson = File.ReadAllText(AccountsFile);
                BankAccounts = JsonConvert.DeserializeObject<List<BankAccount>>(resultJson);
                MessageBox.Show("Данные успешно считаны из JSON!");

                UpdateUserComboBox();
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке данных");
            }
        }

        public class CurrencyRate
        {
            public string ID { get; set; }
            public string NumCode { get; set; }
            public string CharCode { get; set; }
            public int Nominal { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public double Previous { get; set; }
        }

        public class ExchangeRatesResponse
        {
            public DateTime Date { get; set; }
            public DateTime PreviousDate { get; set; }
            public string PreviousURL { get; set; }
            public DateTime Timestamp { get; set; }
            public Dictionary<string, CurrencyRate> Valute { get; set; }
        }

        private Dictionary<string, CurrencyRate> currencyRates = new Dictionary<string, CurrencyRate>();
        HttpClient client = new HttpClient();
        const string url = "https://www.cbr-xml-daily.ru/daily_json.js";

        private async Task GetRatesMethod()
        {
            string json = await client.GetStringAsync(url);
            ExchangeRatesResponse rates = JsonConvert.DeserializeObject<ExchangeRatesResponse>(json);

            if (rates != null && rates.Valute != null)
            {
                currencyRates = rates.Valute;

                currencyRates["RUB"] = new CurrencyRate
                {
                    CharCode = "RUB",
                    Name = "Российский рубль",
                    Nominal = 1,
                    Value = 1
                };

                foreach (var currency in currencyRates)
                {
                    comboBoxFrom.Items.Add(currency.Key);
                    comboBoxTo.Items.Add(currency.Key);
                }

                comboBoxFrom.SelectedItem = "USD";
                comboBoxTo.SelectedItem = "RUB";
            }
        }

        public void Convert()
        {
            if (comboBoxFrom.SelectedItem == null || comboBoxTo.SelectedItem == null) return;

            string fromCurrency = comboBoxFrom.SelectedValue.ToString();
            string toCurrency = comboBoxTo.SelectedValue.ToString();

            if (tbFrom.Text.Contains("."))
            {
                MessageBox.Show("Точка не может быть разделителем", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbFrom.Text = null;
                tbTo.Text = null;
                return;
            }

            if (!double.TryParse(tbFrom.Text, out double amount))
            {
                MessageBox.Show("Введите корректную сумму.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbFrom.Text = null;
                tbTo.Text = null;
                return;
            }

            double rateFrom = currencyRates[fromCurrency].Value / currencyRates[fromCurrency].Nominal;
            double rateTo = currencyRates[toCurrency].Value / currencyRates[toCurrency].Nominal;

            double result = amount * (rateFrom / rateTo);
            tbTo.Text = result.ToString("F2");
        }

        private void ChangeBtn(object sender, RoutedEventArgs e)
        {
            object cbTo = comboBoxTo.SelectedItem;
            comboBoxTo.SelectedItem = comboBoxFrom.SelectedItem;
            comboBoxFrom.SelectedItem = cbTo;
            tbTo.Text = "0";

            Convert();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Convert();
        }

        private void tbFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFrom.Text))
            {
                tbFrom.Text = "0";
            }
            else
            {
                Convert();
            }
        }
    }
}