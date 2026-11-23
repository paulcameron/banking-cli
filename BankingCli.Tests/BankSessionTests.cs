namespace BankingCli.Tests;

[TestClass]
public sealed class BankSessionTests
{
    [TestMethod]
    public void GetBalanceReturnsZeroForAccount()
    {
        BankSession session = new();

        Assert.AreEqual("chequing=$0.00", session.GetBalance("chequing"));
    }

    [TestMethod]
    public void GetBalanceReturnsBothAccountsForAllOrEmptyAccount()
    {
        BankSession session = new();
        const string expected = "The balance in your chequing account is chequing=$0.00 and the balance in your savings account is savings=$0.00";

        Assert.AreEqual(expected, session.GetBalance("all"));
        Assert.AreEqual(expected, session.GetBalance(""));
    }

    [TestMethod]
    public void GetBalanceRejectsUnknownAccount()
    {
        BankSession session = new();

        Assert.AreEqual("ERROR INVALID_ACCOUNT", session.GetBalance("investment"));
    }

    [TestMethod]
    public void DepositAddsAmountToAccount()
    {
        BankSession session = new();

        Assert.AreEqual(
            "I deposited $25.50 in savings. The balance of savings is now savings=$25.50",
            session.Deposit("savings", "25.5"));

        Assert.AreEqual(
            "I deposited $100.05 in savings. The balance of savings is now savings=$125.55",
            session.Deposit("savings", "100.05"));
    }

    [TestMethod]
    public void DepositRejectsUnknownAccount()
    {
        BankSession session = new();

        Assert.AreEqual("ERROR INVALID_ACCOUNT", session.Deposit("investment", "10"));
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("-1")]
    [DataRow("1.234")]
    [DataRow("abc")]
    public void DepositRejectsInvalidAmounts(string amount)
    {
        BankSession session = new();

        Assert.AreEqual("ERROR INVALID_AMOUNT", session.Deposit("chequing", amount));
    }

    [TestMethod]
    public void DepositRejectsAmountThatWouldExceedBalanceLimit()
    {
        BankSession session = new();
        const string maximumAmount = "99999999999999999999999999.99";

        Assert.IsTrue(session.Deposit("chequing", maximumAmount).StartsWith(
            "I deposited $99999999999999999999999999.99",
            StringComparison.Ordinal));
        Assert.AreEqual("ERROR BALANCE_LIMIT", session.Deposit("chequing", "0.01"));
    }

    [TestMethod]
    public void WithdrawSubtractsAmountFromAccount()
    {
        BankSession session = new();
        session.Deposit("chequing", "100");

        Assert.AreEqual(
            "I withdrew $25.25 from chequing. The balance of chequing is now chequing=$74.75",
            session.Withdraw("chequing", "25.25"));

        Assert.AreEqual(
            "I withdrew $10.55 from chequing. The balance of chequing is now chequing=$64.20",
            session.Withdraw("chequing", "10.55"));
    }

    [TestMethod]
    public void WithdrawRejectsInsufficientFundsWithoutChangingBalance()
    {
        BankSession session = new();

        session.Deposit("chequing", "100");

        Assert.AreEqual(
            "ERROR INSUFFICIENT_FUNDS | chequing=$100.00",
            session.Withdraw("chequing", "125"));
        Assert.AreEqual("chequing=$100.00", session.GetBalance("chequing"));
    }

    [TestMethod]
    public void WithdrawRejectsUnknownAccount()
    {
        BankSession session = new();

        Assert.AreEqual("ERROR INVALID_ACCOUNT", session.Withdraw("investment", "10"));
    }

    [TestMethod]
    public void TransferMovesFundsBetweenAccounts()
    {
        BankSession session = new();
        session.Deposit("chequing", "100");

        Assert.AreEqual(
            "I transferred $25.25 from chequing to savings. The balance of chequing is now chequing=$74.75 and the balance of savings is now savings=$25.25",
            session.Transfer("chequing", "savings", "25.25"));
    }

    [TestMethod]
    public void TransferRejectsSameAccountWithoutChangingBalance()
    {
        BankSession session = new();
        session.Deposit("chequing", "100");

        Assert.AreEqual("ERROR SAME_ACCOUNT", session.Transfer("chequing", "chequing", "10"));
        Assert.AreEqual("chequing=$100.00", session.GetBalance("chequing"));
    }

    [TestMethod]
    public void TransferRejectsUnknownAccount()
    {
        BankSession session = new();

        Assert.AreEqual("ERROR INVALID_ACCOUNT", session.Transfer("chequing", "investment", "10"));
    }

    [TestMethod]
    public void TransferRejectsInsufficientFundsWithoutChangingBalances()
    {
        BankSession session = new();

        session.Deposit("chequing", "100");
        session.Deposit("savings", "100");

        Assert.AreEqual(
            "ERROR INSUFFICIENT_FUNDS | chequing=$100.00",
            session.Transfer("chequing", "savings", "1000"));
        Assert.AreEqual("chequing=$100.00", session.GetBalance("chequing"));
        Assert.AreEqual("savings=$100.00", session.GetBalance("savings"));
    }
}
