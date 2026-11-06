var account = new BankAccount("Олена", 1000m);
account.Deposit(250m);
Console.WriteLine(account.Balance);

class BankAccount
{
    public string Owner { get; }
    public decimal Balance { get; private set; }

    public BankAccount(string owner, decimal balance = 0)
    {
        Owner = owner;
        Balance = balance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException();
        Balance += amount;
    }
}