using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase("0.0", false, "no fraction is expected")]
    [TestCase("0", true, "there is no fraction")]
    public void NoFracitions(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 0, false);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [TestCase("0", true, "'-' is abscent, so the number is considered positive")]
    [TestCase("-0", false, "'-' is present, so the number is considered negative")]
    public void CorrectPositiveCheck(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    #region Limits
    [Test]
    public void ScaleLimitCorrect()
    {
        var validator = new NumberValidator(3, 2, true);
        string because = "scale limit is not exceeded";

        using (new AssertionScope())
        {
            validator.IsValidNumber("1").Should().Be(true, because);
            validator.IsValidNumber("1.0").Should().Be(true, because);
            validator.IsValidNumber("1.00").Should().Be(true, because);
            validator.IsValidNumber("1.000").Should().Be(false, "scale limit is exceeded");
        }
    }

    [Test]
    public void PrecisionLimitCorrect()
    {
        var validator = new NumberValidator(3, 2, true);
        string exceeded = "scale limit is exceeded";
        string inLimits = "scale limit is not exceeded";

        using (new AssertionScope())
        {
            validator.IsValidNumber("10.00").Should().Be(false, exceeded);
            validator.IsValidNumber("1.00").Should().Be(true, inLimits);
            validator.IsValidNumber("1.0").Should().Be(true, inLimits);
            validator.IsValidNumber("1").Should().Be(true, inLimits);
            validator.IsValidNumber("10").Should().Be(true, inLimits);
            validator.IsValidNumber("100").Should().Be(true, inLimits);
            validator.IsValidNumber("1000").Should().Be(false, exceeded);
        }
    }
    #endregion

    #region Only positive flag
    [Test]
    public void OnlyPositiveFlag()
    {
        var validator = new NumberValidator(4, 2, true);

        validator.IsValidNumber("-1.0").Should().Be(false, "number validator accepts only positive numbers");
    }

    [Test]
    public void NoPositiveFlag()
    {
        var validator = new NumberValidator(4, 2, false);

        validator.IsValidNumber("-1.0").Should().Be(true, "number validator accepts positive and negative numbers");
    }
    #endregion

    [TestCase("-1.00", false, "total length is greater than precision")]
    [TestCase("+1.00", false, "total length is greater than precision")]
    [TestCase("1.00", true, "this time there's no sign, so total length is not greater than precision")]
    public void ScaleAccountsSign(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [Test]
    public void CommaSeparator()
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber("1,0").Should().Be(true, "comma is a valid separator");
    }

    [Test]
    public void MoreThanOneSeparator()
    {
        var validator = new NumberValidator(5, 2, true);

        validator.IsValidNumber("1,0.0").Should().Be(false, "there can be only one separator in the number");
    }


    #region Invalid, empty or null number string
    [Test]
    public void NullInsteadOfString()
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(null).Should().Be(false, "type 'null' is not a correct number");
    }

    [TestCase("", false, "number string is empty")]
    [TestCase(" ", false, "number string is empty")]
    [TestCase("a.sd", false, "number string is invalid")]
    [TestCase("asd", false, "number string is invalid")]
    [TestCase("1.O", false, "number string is invalid")]
    [TestCase("1 5.5", false, "number string is invalid")]
    [TestCase("5.", false, "number is missing fraction")]
    public void InvalidString(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }
    #endregion

    #region Throw exception
    [Test]
    public void ThrowOnNegativeScale()
    {
        Action act = () => new NumberValidator(10, -2, true);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a non-negative number less or equal than precision");
    }

    [Test]
    public void ThrowOnScaleGreaterThanPrecision()
    {
        Action act = () => new NumberValidator(1, 2, true);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a non-negative number less or equal than precision");
    }

    [Test]
    public void ThrowOnNegativePrecision()
    {
        Action act = () => new NumberValidator(-1, 2, true);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a positive number");
    }
    #endregion
}