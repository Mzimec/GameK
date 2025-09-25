using UnityEngine;
using NUnit.Framework;

public class EventBusTests
{
    private BaseEventBus _eventBus;
    private bool _wasCalled;

    public struct TestEvent : IEvent {
        public int Value;
    }


    [SetUp]
    public void SetUp() {
        _eventBus = new BaseEventBus();
        _wasCalled = false;
    }

    [Test]
    public void SubscribeAndPublish_Event_CallsHandler() {
        _eventBus.Subscribe<TestEvent>(OnTestEvent);
        _eventBus.Publish(new TestEvent { Value = 123 });

        Assert.IsTrue(_wasCalled, "Event handler was not called.");
    }

    [Test]
    public void Unsubscribe_Event_DoesNotCallHandler() {
        var subscription = _eventBus.Subscribe<TestEvent>(OnTestEvent);
        subscription.Dispose(); // Unsubscribe
        _eventBus.Publish(new TestEvent());

        Assert.IsFalse(_wasCalled, "Handler was still called after unsubscription.");
    }

    private void OnTestEvent(TestEvent evt) {
        _wasCalled = true;
    }
}
