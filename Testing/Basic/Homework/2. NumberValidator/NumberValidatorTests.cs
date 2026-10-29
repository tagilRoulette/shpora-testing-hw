using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase("0.0", false, "no fraction is expected")]
    [TestCase("0", true, "there is no fraction")]
    public void NotValid_When_Fraction_Given_But_Not_Expected(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 0, false);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [TestCase("1", true, "scale limit is not exceeded")]
    [TestCase("1.00", true, "scale limit is not exceeded")]
    [TestCase("1.000", false, "scale limit is exceeded")]
    public void NotValid_When_Scale_Is_Exceeded(string number, bool result, string because)
    {
        var validator = new NumberValidator(5, 2, true);
        validator.IsValidNumber(number).Should().Be(result, because);
    }

    [TestCase("10.00", false, "last zero in fraction exceeds precision of 3", 3)]
    [TestCase("1.0", true, "int part length and fraction part length give sum of 2, which is not greater than precision", 3)]
    [TestCase("100", true, "int part length is 3, which is not greater than precision", 3)]
    [TestCase("1000", false, "int part length is 4, which is greater than precision", 3)]
    public void Invalid_When_Precision_Is_Exceeded(string number, bool result, string because, int precision)
    {
        var validator = new NumberValidator(precision, 2, true);

        validator.IsValidNumber(number).Should().Be(result, because);
    }

    [Test]
    public void Invalid_When_Negative_Number_Given_But_Accepting_Only_Positive_Numbers()
    {
        var validator = new NumberValidator(4, 2, true);

        validator.IsValidNumber("-1.0").Should().Be(false, "number validator accepts only positive numbers");
    }

    [Test]
    public void Consider_Number_Positive_When_No_Sign_Given()
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber("0").Should().Be(true, "'-' is abscent, so the number is considered positive");
    }

    [Test]
    public void Valid_When_Negative_Number_Given_And_Accepting_Not_Only_Positive_Numbers()
    {
        var validator = new NumberValidator(4, 2, false);

        validator.IsValidNumber("-1.0").Should().Be(true, "number validator accepts positive and nonpositive numbers");
    }

    [TestCase("-1.00", false, "total length is greater than precision")]
    [TestCase("+1.00", false, "total length is greater than precision")]
    [TestCase("1.00", true, "this time there's no sign, so total length is not greater than precision")]
    public void Account_Sign_In_Precision(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [Test]
    public void Valid_When_Comma_Is_Separator()
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber("1,0").Should().Be(true, "comma is a valid separator");
    }

    [Test]
    public void Invalid_When_More_Than_One_Separator()
    {
        var validator = new NumberValidator(5, 2, true);

        validator.IsValidNumber("1,0.0").Should().Be(false, "there can be only one separator in the number");
    }

    [TestCase("", false, "number string is empty")]
    [TestCase(" ", false, "number string is empty")]
    [TestCase("a.sd", false, "number string contains non-numeric characters")]
    [TestCase("asd", false, "number string contains non-numeric characters")]
    [TestCase("1.O", false, "number string contains non-numeric characters")]
    [TestCase("1 5.5", false, "number string contains whitespace")]
    [TestCase("5.", false, "number is missing fraction")]
    [TestCase(null, false, "type 'null' is not a correct number")]
    public void Invalid_When_Invalid_Number_String_Given(string number, bool result, string explanation)
    {
        var validator = new NumberValidator(3, 2, true);

        validator.IsValidNumber(number).Should().Be(result, explanation);
    }

    [Test]
    public void Throw_When_Scale_Is_Negative()
    {
        Action act = () => new NumberValidator(10, -2, true);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a non-negative number less or equal than precision");
    }

    [Test]
    public void Throw_When_Scale_Is_Greater_Than_Precision()
    {
        Action act = () => new NumberValidator(1, 2, true);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a non-negative number less or equal than precision");
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void Throw_When_Precision_Is_Not_Positive(int precision)
    {
        Action act = () => new NumberValidator(precision, 2, true);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a positive number");
    }
}