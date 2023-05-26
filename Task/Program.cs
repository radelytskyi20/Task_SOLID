IOperation depositOperation = new Deposit(1000);
IOperation withdrawOperation = new Withdraw(1000);
INotification consoleNotification = new ConsoleNotification();
ILogger consoleLogger = new ConsoleLogger();
IAccount a = new Account("#21", 100000000, "Bill");

IBankService bankService = new BankService(a, consoleLogger, consoleNotification);


bankService.Operation(depositOperation);

Console.WriteLine();
Console.WriteLine();

bankService.Operation(withdrawOperation);


#region MainLogic

interface IAccount
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public string Owner { get; set; }
}

class Account : IAccount
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public string Owner { get; set; }

    public Account(string accountNumber, decimal balance, string owner)
    {
        AccountNumber = accountNumber;
        Balance = balance;
        Owner = owner;
    }
}

interface IOperation
{
    void Excecute(IAccount account, INotification notification, ILogger logger);
    public decimal Amount { get; set; }
}

class Deposit : IOperation
{
    public decimal Amount { get; set; }

    public Deposit(decimal amount)
    {
        Amount = amount;
    }

    public void Excecute(IAccount account, INotification notification, ILogger logger)
    {
        account.Balance += Amount;
        notification.ShowMessage($"Deposited {Amount} into account {account.AccountNumber}");
        logger.Log(new Transaction(Amount, DateTime.Now, "Deposit"));

    }
}

class Withdraw : IOperation
{
    public decimal Amount { get; set; }

    public Withdraw(decimal amount)
    {
        Amount = amount;
    }

    public void Excecute(IAccount account, INotification notification, ILogger logger)
    {
        Transaction currentTransaction = new Transaction(Amount, DateTime.Now, "Withdraw");

        if (Amount <= account.Balance)
        {
            account.Balance -= Amount;
            notification.ShowMessage($"Withdrew {Amount} from account {account.AccountNumber}");
            logger.Log(currentTransaction);
        }
        else
        {
            notification.ShowMessage("Insufficient balance");
            logger.Log(currentTransaction);
        }
    }
}

#endregion

#region Logger

interface ILogger
{
    void Log(ITransaction transaction);
}

class ConsoleLogger : ILogger
{
    public void Log(ITransaction transaction)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("--Log: " + transaction.GetInfo());
        Console.ResetColor();
    }
}

#endregion

#region Notification

interface INotification
{
    void ShowMessage(string message);
}
class ConsoleNotification : INotification
{
    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }
}

#endregion

#region Transaction

interface ITransaction
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string TransactionType { get; set; }
    string GetInfo();
}

class Transaction : ITransaction
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string TransactionType { get; set; }

    public Transaction(decimal amount, DateTime date, string transactionType)
    {
        Amount = amount;
        Date = date;
        TransactionType = transactionType;
    }

    public string GetInfo()
    {
        return $"Transaction: {TransactionType}, Amount: {Amount}, Date: {Date}";
    }
}

#endregion

#region BankService

interface IBankService
{
    void Operation(IOperation operation);
}
class BankService : IBankService
{
    private IAccount Account { get; set; }
    private ILogger Logger { get; set; }
    private INotification Notification { get; set; }

    public BankService(IAccount account, ILogger logger, INotification notification)
    {
        Account = account;
        Logger = logger;
        Notification = notification;
    }

    public void Operation(IOperation operation)
    {
        operation.Excecute(Account, Notification, Logger);
    }
}

#endregion