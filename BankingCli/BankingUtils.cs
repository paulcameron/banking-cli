using System.Globalization;

namespace BankingCli;

public static class BankingUtils
{
    public static string FormatMoney(decimal amount) =>
        "$" + amount.ToString("F2", CultureInfo.InvariantCulture);
}
