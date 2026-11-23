using System.Globalization;

namespace BankingCli.Tests;

[TestClass]
public sealed class BankingUtilsTests
{
    [TestMethod]
    public void FormatMoneyAlwaysUsesTwoDecimalPlaces()
    {
        Assert.AreEqual("$123.40", BankingUtils.FormatMoney(123.4m));
        Assert.AreEqual("$0.00", BankingUtils.FormatMoney(0m));
    }

    [TestMethod]
    public void FormatMoneyUsesCultureDecimalSeparator()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

            Assert.AreEqual("$1234.50", BankingUtils.FormatMoney(1234.5m));

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            Assert.AreEqual("$1234.50", BankingUtils.FormatMoney(1234.5m));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
