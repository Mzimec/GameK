using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class RollTests {
    [Test]
    [TestCase(1, 10, 0, ERollResult.CriticalFailure)]
    [TestCase(20, 10, 0, ERollResult.CriticalSuccess)]
    [TestCase(15, 10, 0, ERollResult.Success)]
    [TestCase(5, 10, 0, ERollResult.Failure)]
    public void RollRecord_DetermineResult_WorksCorrectly(int roll, int target, int bonus, ERollResult expected) {
        var record = new RollRecord(roll, target, bonus);
        Assert.AreEqual(expected, record.Result, $"Roll {roll} vs {target} (+{bonus})");
    }

    [Test]
    public void RollRecord_Log_ReturnsReadableMessage() {
        var record = new RollRecord(15, 10, 2);
        StringAssert.Contains("Roll of 15 against target 10", record.Log);
        StringAssert.Contains(record.Result.ToString(), record.Log);
    }

    [Test]
    public void SaveRollData_ResolvesRollWithinValidRange() {
        var resolver = new SaveRollData(12);
        var ctx = new ActionContext(new CharacterCore(), new DummyTarget(), null, null);

        var record = resolver.ResolveRoll(ctx);

        Assert.That(record.Roll, Is.InRange(1, 20));
        Assert.That(record.TargetNumber, Is.EqualTo(12));
    }

    [Test]
    public void AttackRollData_ResolvesRollAndUsesAttackBonus() {
        // Arrange
        var source = new CharacterCore();
        IStatBase bab = new IntComparableStat(5, 0, 10, new StatTypeSO { StatCategory = EStatCategory.AttackBonus });
        source.Stats.AddStats(new List<IStatBase> { bab });

        var target = new CharacterCore();
        IStatBase ac = new IntComparableStat(15, 0, 20, new StatTypeSO { StatCategory = EStatCategory.AC });
        target.Stats.AddStats(new List<IStatBase> { ac });

        var resolver = new AttackRollData();
        var ctx = new ActionContext(source, new DummyTarget { Entity = target }, null, null);

        // Act
        var record = resolver.ResolveRoll(ctx);
        var successChances = resolver.SuccessChance(record);

        // Assert
        Assert.That(record.Roll, Is.InRange(1, 20));
        Assert.AreEqual(15, record.TargetNumber, "Should use target's AC");
        Assert.AreEqual(5, record.RollBonus, "Should use source's AttackBonus");
        Assert.AreEqual(successChances[ERollResult.CriticalSuccess], 0.05f, "Should use roll distribution based on target`s AC and source AttackBonus");
        Assert.AreEqual(successChances[ERollResult.Success], 0.5f, "Should use roll distribution based on target`s AC and source AttackBonus");
        Assert.AreEqual(successChances[ERollResult.Failure], 0.4f, "Should use roll distribution based on target`s AC and source AttackBonus");
        Assert.AreEqual(successChances[ERollResult.CriticalFailure], 0.05f, "Should use roll distribution based on target`s AC and source AttackBonus");
    }
}
