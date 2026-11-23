# banking-cli-starter

A small C# command-line banking simulator with in-memory chequing and savings accounts. Both accounts start at $0.00 each time you run the program.

## Development

By default, this project is set up to run against a locally run LLM running in Ollama (`ollama run ornith:9b`).  You can use Open Ai by updating the app settings or otherwise overriding the values (env vars or user secrets).

You'll also need to set the `OpenAI:ApiKey` user secret or `OpenAI__ApiKey` environment variable to your OpenAI API key.  When running locally with ollama, the key is required, although it can be set to any value.

## Run

With the .NET 10 SDK installed, run this from the repo root.

```sh
dotnet run --project BankingCli
```

In Visual Studio 2026 (18.0 or later), open `BankingCli.sln` and press F5. Visual Studio 2022 users can run the app from a terminal using the .NET 10 SDK.

## Commands

You can use natural language commands to deposit, withdraw, transfer and check balances.

Accounts are `chequing` and `savings`. Amounts are positive, with up to two decimal places, such as `5` or `5.25`. Withdrawals and transfers require sufficient funds, and transfers require two different accounts. A failed command leaves both balances as they were.

## Tests 

You can run the unit tests withing Visual Studio or on the command line:
```sh
dotnet test
```

## Sample Session

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
