
public interface IEvent { }

public interface ICharacterEvent : IEvent { }

public struct TestEvent : IEvent { }

public struct PlayerEvent : IEvent {
    public int health;
    public int mana;
}
