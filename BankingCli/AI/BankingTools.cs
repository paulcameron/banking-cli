
using OpenAI.Responses;

namespace BankingCli.AI;

#pragma warning disable OPENAI001

public static class BankingTools
{
    public static readonly FunctionTool GetBalance =
        ResponseTool.CreateFunctionTool(
            functionName: "GetBalance",
            functionDescription:
                "Gets the balance of a bank account. Use 'all' to retrieve all accounts.",
            functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "account": {
                        "type": "string",
                        "enum": ["chequing", "savings", "all"]
                    }
                },
                "required": ["account"],
                "additionalProperties": false
            }
            """u8.ToArray()),
            strictModeEnabled: true);

    public static readonly FunctionTool Deposit =
        ResponseTool.CreateFunctionTool(
            functionName: "Deposit",
            functionDescription:
                "Deposits money into an account. Only call after the user has explicitly requested a deposit.",
            functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "account": {
                        "type": "string",
                        "enum": ["chequing", "savings"]
                    },
                    "amount": {
                        "type": "string",
                        "description": "Positive decimal amount, without currency symbols."
                    }
                },
                "required": ["account", "amount"],
                "additionalProperties": false
            }
            """u8.ToArray()),
            strictModeEnabled: true);

    public static readonly FunctionTool Withdraw =
        ResponseTool.CreateFunctionTool(
            functionName: "Withdraw",
            functionDescription:
                "Withdraws money from an account. Only call after the user has explicitly requested a withdrawal.",
            functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "account": {
                        "type": "string",
                        "enum": ["chequing", "savings"]
                    },
                    "amount": {
                        "type": "string",
                        "description": "Positive decimal amount, without currency symbols."
                    }
                },
                "required": ["account", "amount"],
                "additionalProperties": false
            }
            """u8.ToArray()),
            strictModeEnabled: true);

    public static readonly FunctionTool Transfer =
        ResponseTool.CreateFunctionTool(
            functionName: "Transfer",
            functionDescription:
                "Transfers money between two accounts. Only call after the user has explicitly requested a transfer.",
            functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "fromAccount": {
                        "type": "string",
                        "enum": ["chequing", "savings"]
                    },
                    "toAccount": {
                        "type": "string",
                        "enum": ["chequing", "savings"]
                    },
                    "amount": {
                        "type": "string",
                        "description": "Positive decimal amount, without currency symbols."
                    }
                },
                "required": ["fromAccount", "toAccount", "amount"],
                "additionalProperties": false
            }
            """u8.ToArray()),
            strictModeEnabled: true);

    public static readonly FunctionTool Help =
        ResponseTool.CreateFunctionTool(
            functionName: "Help",
            functionDescription:
                "Provides help information about available banking commands and how to use the application.",
            functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {},
                "required": [],
                "additionalProperties": false
            }
            """u8.ToArray()),
            strictModeEnabled: true);
}

#pragma warning restore OPENAI001
