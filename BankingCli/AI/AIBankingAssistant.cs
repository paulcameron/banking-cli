
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Responses;
using System.ClientModel;
using System.Globalization;
using System.Text.Json;

namespace BankingCli.AI;

#pragma warning disable OPENAI001

public class AiBankingAssistant
{
    private readonly ResponsesClient _client;
    private readonly string _model;
    private readonly string _apiEndpoint;
    private readonly BankSession _banking;

    private readonly List<ResponseItem> _conversation = [];

    public AiBankingAssistant(
        string apiKey,
        string model,
        string apiEndpoint,
        BankSession banking)
    {
        _client = new(
            options: new ResponsesClientOptions
                {
                    Endpoint = new Uri(apiEndpoint)
                },
            credential: new ApiKeyCredential(apiKey)
            );

        _model = model;
        _apiEndpoint = apiEndpoint;
        _banking = banking;

        // Add initial instructions once.
        _conversation.Add(
            ResponseItem.CreateDeveloperMessageItem("""
                You are a conversational banking assistant.

                Available accounts:
                - chequing
                - savings

                Understand requests expressed in conversational English.

                Use the available functions to retrieve balances
                and perform banking transactions.

                Rules:
                - Never invent account balances.
                - Never invent transaction results.
                - Never assume a missing account or amount.
                - Never use markdown in output.
                - Never use asterisks to highlight information or list items.
                - Ask for clarification when information is missing.
                - Never execute a transaction that the user has not requested.
                - Always explain results in conversational English.
                - Never claim a transaction succeeded unless the
                  application confirms it.
                - Do not treat a request to explain or discuss a
                  transaction as authorization to execute it.
                - When providing a summary of the operation, include the new
                  balance of the affected account(s).
                - Always format all dollar amounts with a dollar sign and two decimal
                  places, e.g. $123.45.  Do this in summaries and any help or
                  confirmation messages as well.
                - Limit conversations to banking operations that this application
                  supports.  Do not provide advice or guidance on other topics.
                  Redirect conversations back to the help if they veer off-topic.
                """));
    }

    public async Task<string> ProcessAsync(string userInput)
    {
        _conversation.Add(
            ResponseItem.CreateUserMessageItem(userInput));

        bool requiresAction;

        do
        {
            requiresAction = false;

            CreateResponseOptions options =
                new(_model, _conversation)
                {
                    Tools =
                    {
                        BankingTools.GetBalance,
                        BankingTools.Deposit,
                        BankingTools.Withdraw,
                        BankingTools.Transfer,
                        BankingTools.Help
                    }
                };

            ResponseResult response =
                await _client.CreateResponseAsync(options);

            _conversation.AddRange(response.OutputItems);

            foreach (ResponseItem item in response.OutputItems)
            {
                if (item is not FunctionCallResponseItem call)
                    continue;

                string result;

                try
                {
                    result = await ExecuteFunctionAsync(
                        call.FunctionName,
                        call.FunctionArguments.ToString());
                }
                catch (Exception ex)
                {
                    result = JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = ex.Message
                    });
                }

                _conversation.Add(
                    new FunctionCallOutputResponseItem(
                        call.CallId,
                        result));

                requiresAction = true;
            }

            if (!requiresAction)
            {
                return response.GetOutputText();
            }

        } while (requiresAction);

        return "I couldn't complete that request.";
    }

    private async Task<string> ExecuteFunctionAsync(
        string functionName,
        string arguments)
    {
        using JsonDocument json =
            JsonDocument.Parse(arguments);

        JsonElement root = json.RootElement;

        string account;
        string amount;
        string result;

        switch (functionName)
        {
            case "GetBalance":
                {
                    account = GetString(root, "account")
                        .ToLowerInvariant();

                    result = _banking.GetBalance(account);

                    return CreateResult(result);
                }

            case "Deposit":
                {
                    account = GetString(root, "account")
                        .ToLowerInvariant();

                    amount = GetString(root, "amount");

                    if (!ConfirmTransaction(
                        $"You are asking to Deposit {BankingUtils.FormatMoney(decimal.Parse(amount))} into {account}."))
                    {
                        return CreateCancelledResult();
                    }

                    result = _banking.Deposit(account, amount);

                    return CreateResult(result);
                }

            case "Withdraw":
                {
                    account = GetString(root, "account")
                        .ToLowerInvariant();

                    amount = GetString(root, "amount");

                    if (!ConfirmTransaction(
                        $"You want to withdraw {BankingUtils.FormatMoney(decimal.Parse(amount))} from {account}."))
                    {
                        return CreateCancelledResult();
                    }

                    result = _banking.Withdraw(account, amount);

                    return CreateResult(result);
                }

            case "Transfer":
                {
                    string fromAccount =
                        GetString(root, "fromAccount")
                        .ToLowerInvariant();

                    string toAccount =
                        GetString(root, "toAccount")
                        .ToLowerInvariant();

                    amount = GetString(root, "amount");

                    if (!ConfirmTransaction(
                        $"You wish to transfer {BankingUtils.FormatMoney(decimal.Parse(amount))} from {fromAccount} to {toAccount}."))
                    {
                        return CreateCancelledResult();
                    }

                    result = _banking.Transfer(
                        fromAccount,
                        toAccount,
                        amount);

                    return CreateResult(result);
                }

            case "Help":
                {
                    Console.WriteLine(_banking.Help());
                    return "The user was provided with help.  No further action is required.  Do not summarise or add any addition output.  A blank response is all that is required.";
                }

            default:
                throw new InvalidOperationException(
                    $"Unsupported function: {functionName}");
        }
    }

    private static string GetString(
        JsonElement root,
        string property)
    {
        if (!root.TryGetProperty(property, out JsonElement value)
            || value.ValueKind != JsonValueKind.String)
        {
            throw new ArgumentException(
                $"Missing or invalid parameter: {property}");
        }

        return value.GetString()
            ?? throw new ArgumentException(
                $"Missing parameter: {property}");
    }

    private static bool ConfirmTransaction(string description)
    {
        Console.WriteLine();
        Console.WriteLine("Confirmation:");
        Console.WriteLine(description);
        Console.Write("Do you want to proceed? (y/n): ");

        string? answer = Console.ReadLine();

        return string.Equals(
            answer?.Trim(),
            "y",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string CreateResult(string output)
    {
        return JsonSerializer.Serialize(new
        {
            success = !output.StartsWith(
                "ERROR ",
                StringComparison.Ordinal),
            result = output
        });
    }

    private static string CreateCancelledResult()
    {
        return JsonSerializer.Serialize(new
        {
            success = false,
            cancelled = true,
            result = "The user declined the transaction."
        });
    }
}

#pragma warning restore OPENAI001
