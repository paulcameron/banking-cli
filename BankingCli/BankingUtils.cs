using System.Globalization;

namespace BankingCli;

public static class BankingUtils
{
    private const string _helpText = """
        Welcome to our banking application.  You can interact with this applicatiuon using natural language commands.  Here are some examples of what you can do:

        "Deposit ten dollars into my chequing account."
        "Withdraw 5 bucks from my savings account."
        "Move 15 dollars from my chequing to my savings."
        "Show me my money"

        Entering "exit" will terminate the application.
        Entering "help" will display this help message again.

        """;
    public static string HelpText = _helpText;

    public static string FormatMoney(decimal amount) =>
        "$" + amount.ToString("F2", CultureInfo.InvariantCulture);
}
