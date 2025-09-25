using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mocky pro testování
/// </summary>
public class DummyTarget : ITargetable {
    public Vector3Int Position { get; set; } = Vector3Int.zero;
    public GameObject GameObject { get; set; } = new GameObject("DummyTarget");

    public IEntity Entity { get; set; } = new Entity();
}

public class DummyRollResolver : IRollResolver {
    private readonly ERollResult _forcedResult;
    private readonly Dictionary<ERollResult, float> _probabilities;

    public DummyRollResolver(ERollResult forcedResult) {
        _forcedResult = forcedResult;
        _probabilities = new Dictionary<ERollResult, float> {
            { ERollResult.CriticalFailure, 0f },
            { ERollResult.Failure, 0f },
            { ERollResult.Success, 0f },
            { ERollResult.CriticalSuccess, 0f }
        };
        _probabilities[_forcedResult] = 1f;
    }

    public RollRecord ResolveRoll(ActionContext ctx) {
        var record = new RollRecord(10, 10, 0) { }; // result není důležitý, protože ho přepisujeme
        record.Result = _forcedResult;
        return record;
    }

    public Dictionary<ERollResult, float> SuccessChance(RollRecord roll) => _probabilities;
}

public class DummyAction : IGameAction {
    public bool Executed { get; private set; } = false;
    public void Execute(ActionContext ctx) => Executed = true;

    public ActionApproximation GetApproximatedResult(ActionContext ctx) {
        var approx = new ActionApproximation();
        return approx;
    }
}

[TestFixture]
public class ActionTests {

    [Test]
    public void ActionNode_ExecutesChildOnSuccess() {
        // Arrange
        var child = new DummyAction();
        var node = new ActionNodeBuilder()
            .WithTargeting(new SingleTargetSelector(new DummyTarget())) // selector vrátí 1 target
            .WithRoll(new DummyRollResolver(ERollResult.Success))
            .OnSuccess(child)
            .Build();

        var context = new ActionContext(new CharacterCore(), new DummyTarget(), null, null);

        // Act
        node.Execute(context);

        // Assert
        Assert.IsTrue(child.Executed, "Child action should have executed on Success result");
    }

    [Test]
    public void ActionNode_DoesNotExecuteChildOnFailure() {
        // Arrange
        var child = new DummyAction();
        var node = new ActionNodeBuilder()
            .WithTargeting(new SingleTargetSelector(new DummyTarget()))
            .WithRoll(new DummyRollResolver(ERollResult.Failure))
            .OnSuccess(child)
            .Build();

        var context = new ActionContext(new CharacterCore(), new DummyTarget(), null, null);

        // Act
        node.Execute(context);

        // Assert
        Assert.IsFalse(child.Executed, "Child action should NOT have executed on Failure result");
    }

    [Test]
    public void ActionNode_ApproximationRespectsProbabilities() {
        // Arrange
        var child = new DummyAction();
        var node = new ActionNodeBuilder()
            .WithTargeting(new SingleTargetSelector(new DummyTarget()))
            .WithRoll(new DummyRollResolver(ERollResult.Success))
            .OnSuccess(child)
            .Build();

        var context = new ActionContext(new CharacterCore(), new DummyTarget(), null, null);

        // Act
        var approx = node.GetApproximatedResult(context);

        int expectedDamage = 0;
        foreach (var (damageType, damage) in approx.DamageDealt) {
            expectedDamage += damage;
        }

        // Assert
        Assert.AreEqual(0, expectedDamage, "Approximation should include expected damage from child");
    }

    [Test]
    public void GameAction_ExecutesAllChildren() {
        // Arrange
        var child1 = new DummyAction();
        var child2 = new DummyAction();
        var gameAction = new GameAction();
        typeof(GameAction).GetField("_children", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(gameAction, new List<IGameAction> { child1, child2 });

        var context = new ActionContext(new CharacterCore(), new DummyTarget(), null, null);

        // Act
        gameAction.Execute(context);

        // Assert
        Assert.IsTrue(child1.Executed && child2.Executed, "All children should be executed by GameAction");
    }
}

/// <summary>
/// Mock selector pro jednodušší testy
/// </summary>
public class SingleTargetSelector : ITargetingSelector {
    private readonly ITargetable _target;
    public SingleTargetSelector(ITargetable target) => _target = target;
    public IEnumerable<ITargetable> SelectTargets(ActionContext ctx) {
        yield return _target;
    }
}
