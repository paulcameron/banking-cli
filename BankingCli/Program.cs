using BankingCli;
using BankingCli.AI;

AppConfig config = new AppConfig();

BankSession banking = new BankSession();

AiBankingAssistant assistant = new AiBankingAssistant(
    config.OpenAIApiKey,
    config.OpenAIModel,
    config.OpenAIEndpoint,
    banking);

Console.WriteLine("Banking AI Assistant ready!");
Console.WriteLine("Type 'exit' to quit.");

while (true)
{
    // Keep redirected output limited to command results so it can be compared line by line.
    if (!Console.IsInputRedirected)
    {
        Console.Write("bank> ");
    }

    string? input = Console.ReadLine();

    // if we get an EOF or the stream ends or is closed, exit the loop
    if (input is null)
        break;

    // short circuit for empty input, exit, and help commands
    if (input.Equals("", StringComparison.OrdinalIgnoreCase))
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine(banking.Help());
        continue;
    }

    // Process the input using the AI assistant
    try
    {
        string output = await assistant.ProcessAsync(input);

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine(output);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"ERROR: Unable to process request: {ex.Message}");
    }
}
