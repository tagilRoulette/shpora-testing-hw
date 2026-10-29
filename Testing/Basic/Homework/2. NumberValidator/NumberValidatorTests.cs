using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase("0.0", false, "no fraction is expected")]
    [TestCase("0", true, "there is no fraction")]
    public void IsValidNumber_When_ScaleIsZero(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 0, false);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [TestCase("1", true, "scale limit is not exceeded")]
    [TestCase("1.00", true, "scale limit is not exceeded")]
    [TestCase("1.000", false, "scale limit is exceeded")]
    public void Invalid_When_ScaleIsExceeded(string number, bool result, string because)
    {
        var validator = new NumberValidator(5, 2, true);
        validator.IsValidNumber(number).Should().Be(result, because);
    }

    [TestCase("10.00", false, "last zero in fraction exceeds precision of 3", 3)]
    [TestCase("1.0", true, "int part length and fraction part length give sum of 2, which is not greater than precision", 3)]
    [TestCase("100", true, "int part length is 3, which is not greater than precision", 3)]
    [TestCase("1000", false, "int part length is 4, which is greater than precision", 3)]
    public void Invalid_When_PrecisionIsExceeded(string number, bool result, string because, int precision)
    {
        var validator = new NumberValidator(precision, 2, true);

        validator.IsValidNumber(number).Should().Be(result, because);
    }

    [Test]
    public void Invalid_When_NegativeNumberGivenButAcceptingOnlyPositive()
    {
        var validator = new NumberValidator(4, 2, true);

        validator.IsValidNumber("-1.0").Should().Be(false, "number validator accepts only positive numbers");
    }

    [Test]
    public void ConsiderNumberPositive_When_NoSignGiven()
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber("0").Should().Be(true, "'-' is abscent, so the number is considered positive");
    }

    [Test]
    public void Valid_When_NegativeNumberGivenAndAcceptingNotOnlyPositive()
    {
        var validator = new NumberValidator(4, 2, false);

        validator.IsValidNumber("-1.0").Should().Be(true, "number validator accepts positive and nonpositive numbers");
    }

    [TestCase("-1.00", false, "total length is greater than precision")]
    [TestCase("+1.00", false, "total length is greater than precision")]
    [TestCase("1.00", true, "this time there's no sign, so total length is not greater than precision")]
    public void AccountSignInPrecision(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [Test]
    public void Valid_When_CommaIsSeparator()
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber("1,0").Should().Be(true, "comma is a valid separator");
    }

    [TestCase("", false, "number string is empty")]
    [TestCase(" ", false, "number string is empty")]
    [TestCase("a.sd", false, "number string contains non-numeric characters")]
    [TestCase("asd", false, "number string contains non-numeric characters")]
    [TestCase("1.O", false, "number string contains non-numeric characters")]
    [TestCase("1O.1", false, "number string contains non-numeric characters")]
    [TestCase("1 5.5", false, "number string contains whitespace")]
    [TestCase("5.", false, "number is missing fraction")]
    [TestCase(null, false, "type 'null' is not a correct number")]
    [TestCase("1,0.0", false, "there can be only one separator in the number")]
    public void Invalid_When_InvalidNumberStringGiven(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [TestCase(-1, 2, "precision must be a positive number")]
    [TestCase(0, 2, "precision must be a positive number")]
    [TestCase(1, 2, "scale must be a non-negative number less than precision")]
    [TestCase(10, -2, "scale must be a non-negative number less than precision")]
    public void Throw_When_PrecisionOrScaleAreInvalid(int precision, int scale, string because)
    {
        Action act = () => new NumberValidator(precision, scale, true);

        act.Should().Throw<ArgumentException>(because);
    }
}