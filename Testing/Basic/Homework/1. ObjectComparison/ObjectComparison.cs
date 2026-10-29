using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Переписанный код с использованием Fluent Assertions.

        actualTsar
            .Should()
            .BeEquivalentTo(
                expectedTsar,
                config => config.Excluding(info => info.DeclaringType == typeof(Person)
                    && info.Name == nameof(Person.Id)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 

        // При добавлении новых полей, свойств нужно будет переписывать проверку в методе AreEqual:
        // вручную прописывать каждое поле, свойство и сравнивать его с ожидаемым.
        // В моем решении эта проблема решена - FluentAssertions рекурсивно проходит
        // по полям и свойствам объекта, сравнивая их, но игнорирует ID царя и его родителя, как и в исходном коде.
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
