using Microsoft.Extensions.Configuration;

using System.Globalization;

namespace BankingCli;

public class BankSession
{
    private const string Chequing = "chequing";
    private const string Savings = "savings";

    // Leave room for the sum of two valid amounts without losing cent precision.
    private const decimal MaximumBalance = 99_999_999_999_999_999_999_999_999.99m;

    private readonly Dictionary<string, decimal> balances = new Dictionary<string, decimal>()
    {
        [Chequing] = 0m,
        [Savings] = 0m
    };

    public string Withdraw(string account, string amount)
    {
        if (!balances.ContainsKey(account))
        {
            return "ERROR INVALID_ACCOUNT";
        }

        if (!TryParseAmount(amount, out decimal parsedAmount))
        {
            return "ERROR INVALID_AMOUNT";
        }

        if (parsedAmount > balances[account])
        {
            return $"ERROR INSUFFICIENT_FUNDS | {FormatBalance(account)}";
        }

        balances[account] -= parsedAmount;
        
        string formattedAmount = BankingUtils.FormatMoney(parsedAmount);
        return $"I withdrew {formattedAmount} from {account}. The balance of {account} is now {FormatBalance(account)}";
    }

    public string Deposit(string account, string amount)
    {
        if (!balances.ContainsKey(account))
        {
            return "ERROR INVALID_ACCOUNT";
        }

        if (!TryParseAmount(amount, out decimal parsedAmount))
        {
            return "ERROR INVALID_AMOUNT";
        }

        if (balances[account] + parsedAmount > MaximumBalance)
        {
            return "ERROR BALANCE_LIMIT";
        }

        balances[account] += parsedAmount;

        string formattedAmount = BankingUtils.FormatMoney(parsedAmount);
        return $"I deposited {formattedAmount} in {account}. The balance of {account} is now {FormatBalance(account)}";
    }

    public string Transfer(string fromAccount, string toAccount, string amount)
    {
        if (!balances.ContainsKey(fromAccount) || !balances.ContainsKey(toAccount))
        {
            return "ERROR INVALID_ACCOUNT";
        }
        if (!TryParseAmount(amount, out decimal parsedAmount))
        {
            return "ERROR INVALID_AMOUNT";
        }
        if (fromAccount == toAccount)
        {
            return "ERROR SAME_ACCOUNT";
        }
        if (parsedAmount > balances[fromAccount])
        {
            return $"ERROR INSUFFICIENT_FUNDS | {FormatBalance(fromAccount)}";
        }
        if (balances[toAccount] + parsedAmount > MaximumBalance)
        {
            return "ERROR BALANCE_LIMIT";
        }
        balances[fromAccount] -= parsedAmount;
        balances[toAccount] += parsedAmount;
        string formattedAmount = BankingUtils.FormatMoney(parsedAmount);
        return $"I transferred {formattedAmount} from {fromAccount} to {toAccount}. The balance of {fromAccount} is now {FormatBalance(fromAccount)} and the balance of {toAccount} is now {FormatBalance(toAccount)}";
    }

    public string GetBalance(string account)
    {
        if (account == "all" || account == "")
        {
            return $"The balance in your chequing account is {FormatBalance(Chequing)} and the balance in your savings account is {FormatBalance(Savings)}";
        }

        if (!balances.ContainsKey(account))
        {
            return "ERROR INVALID_ACCOUNT";
        }
        return FormatBalance(account);
    }

    private static bool TryParseAmount(string input, out decimal amount)
    {
        amount = 0m;
        int decimalPoint = input.IndexOf('.');
        if (decimalPoint >= 0 && input.Length - decimalPoint - 1 is < 1 or > 2)
        {
            return false;
        }

        // Keep the decimal separator consistent across locales in both commands and results.
        return decimal.TryParse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount)
            && amount > 0m
            && amount <= MaximumBalance;
    }

    private string FormatBalance(string account) => $"The balance in {account} is {BankingUtils.FormatMoney(balances[account])}";
}

public readonly record struct CommandResult(string Output, bool ShouldExit = false);
