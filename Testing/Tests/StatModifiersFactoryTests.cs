using NUnit.Framework;

[TestFixture]
public class StatTests {
    [Test]
    public void BaseStat_ReturnsCorrectValue() {
        var strength = new ModifiableStat<int>(10);
        Assert.AreEqual(10, strength.Value);
    }

    [Test]
    public void FlatModifier_AddsToBaseValue() {
        var strength = new ModifiableStat<int>(10);
        strength.AddModifier(new IntFlatStatModifier(5, "source"));
        Assert.AreEqual(15, strength.Value);
    }

    [Test]
    public void PercentModifier_MultipliesBaseValue() {
        var strength = new ModifiableStat<int>(10);
        strength.AddModifier(new IntPercentageStatModifier(50, "source")); // +50 %
        Assert.AreEqual(15, strength.Value);
    }

    [Test]
    public void MultipleModifiers_AreAppliedInCorrectOrder() {
        var stat = new ModifiableStat<int>(10);
        stat.AddModifier(new IntFlatStatModifier(5, "source"));       // 10 + 5 = 15
        stat.AddModifier(new IntPercentageStatModifier(100, "source"));  // +100 % → 30
        Assert.AreEqual(30, stat.Value);
    }

    [Test]
    public void SetterModifier_OverridesAllOtherModifiers() {
        var stat = new ModifiableStat<int>(10);
        stat.AddModifier(new IntFlatStatModifier(5, "source"));        // 15
        stat.AddModifier(new IntPercentageStatModifier(100, "source"));   
        stat.AddModifier(new SetterStatModifier<int>(30, "source"));      // set base to (30 + 5) * 2 = 70
        Assert.AreEqual(70, stat.Value);
    }

    [Test]
    public void ClampModifier_LimitsValue() {
        var stat = new ModifiableStat<int>(50);
        stat.AddModifier(new IntFlatStatModifier(100, "source")); // → 150
        stat.AddModifier(new IntClampStatModifier(120, false, "source"));
        Assert.AreEqual(120, stat.Value);
    }

    [Test]
    public void RemovingModifier_RestoresValue() {
        var stat = new ModifiableStat<int>(10);
        var mod = new IntFlatStatModifier(5, "source");
        stat.AddModifier(mod);
        Assert.AreEqual(15, stat.Value);

        stat.RemoveModifier(mod);
        Assert.AreEqual(10, stat.Value);
    }

    [Test]
    public void RemoveBySource_RemovesAllModifiersFromThatSource() {
        var stat = new ModifiableStat<int>(10);
        stat.AddModifier(new IntFlatStatModifier(5, "buff1"));
        stat.AddModifier(new IntPercentageStatModifier(100, "buff1"));
        stat.AddModifier(new IntFlatStatModifier(3, "buff2"));

        // před odstraněním
        Assert.AreEqual(36, stat.Value); 

        // odstraníme všechny buff1
        stat.RemoveAllModifiers("buff1");

        Assert.AreEqual(13, stat.Value); // jen základ + buff2
    }

    [Test]
    public void MultipleModifiersOfSameType_AreAllApplied() {
        var stat = new ModifiableStat<int>(10);
        stat.AddModifier(new IntFlatStatModifier(5, "s1"));
        stat.AddModifier(new IntFlatStatModifier(2, "s2"));

        Assert.AreEqual(17, stat.Value);
    }

    [Test]
    public void ClearAll_RemovesAllModifiers() {
        var stat = new ModifiableStat<int>(10);
        stat.AddModifier(new IntFlatStatModifier(5, "buff"));
        stat.AddModifier(new IntPercentageStatModifier(50, "buff"));

        Assert.AreNotEqual(10, stat.Value);

        stat.ResetValue();

        Assert.AreEqual(10, stat.Value);
    }

    [Test]
    public void RemovingNonExistingModifier_DoesNothing() {
        var stat = new ModifiableStat<int>(10);
        var fake = new IntFlatStatModifier(5, "ghost");

        // Remove neexistující modifikátor -> hodnota se nesmí změnit
        stat.RemoveModifier(fake);

        Assert.AreEqual(10, stat.Value);
    }

    [Test]
    public void SameSourceCanStackIfAddedMultipleTimes() {
        var stat = new ModifiableStat<int>(10);
        stat.AddModifier(new IntFlatStatModifier(5, "buff"));
        stat.AddModifier(new IntFlatStatModifier(5, "buff")); // stejný sourceId, ale přidaný znovu

        Assert.AreEqual(20, stat.Value);
    }
}
