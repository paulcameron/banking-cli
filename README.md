# banking-cli-starter

A small C# command-line banking simulator with in-memory chequing and savings accounts. Both accounts start at $0.00 each time you run the program.

## Development

By default, this project is set up to run against a locally run LLM running in Ollama (`ollama run ornith:9b`).  You can use Open Ai by updating the app settings or otherwise overriding the values (env vars or user secrets).

You'll also need to set `OpenAI:ApiKey` user secret or `OpenAI__ApiKey` environment variable to your OpenAI API key.  When running locally with ollama, the key is required, although it can be set to any value.

For prod, we should change the default and select a model.
```
{
  "OpenAI": {
    "Model": "some_model",
    "Endpoint": "https://api.openai.com/v1"
  }
}
```

## Run

With the .NET 10 SDK installed, run this from the repo root.

```sh
dotnet run --project BankingCli
```

In Visual Studio 2026 (18.0 or later), open `BankingCli.sln` and press F5. Visual Studio 2022 users can run the app from a terminal using the .NET 10 SDK.

## Commands

| Command | Effect |
| --- | --- |
| `deposit <account> <amount>` | Add money to an account. |
| `withdraw <account> <amount>` | Take money out of an account. |
| `transfer <from> <to> <amount>` | Move money between accounts. |
| `balance [account]` | Show one balance or both balances when the account is omitted. |
| `help` or `?` | Show the command list. |
| `exit` | End the session. |

Accounts are `chequing` and `savings`. Amounts are positive, with up to two decimal places, such as `5` or `5.25`. Withdrawals and transfers require sufficient funds, and transfers require two different accounts. A failed command leaves both balances as they were.

## Sample Sessions

```
Banking AI Assistant ready!
Type 'exit' to quit.
bank> put a grand in my savings

Confirmation:
You are asking to Deposit $1000.00 into savings.
Do you want to proceed? (y/n): y
I successfully deposited $1000.00 into your savings account. Your savings balance is now $1000.00. Would you like to perform another transaction or check another account?
bank> move half of my savings into chequing

Confirmation:
You wish to transfer $500.00 from savings to chequing.
Do you want to proceed? (y/n): y
I successfully transferred $500.00 from your savings account to chequing. Both accounts now have a balance of $500.00. Would you like to perform another transaction or check another account?
```
