public interface IRollResolver {
    RollRecord ResolveRoll(ActionContext ctx);
}

public enum ERollResult {
    CriticalFailure,
    Failure,
    Success,
    CriticalSuccess
}

public struct RollRecord {
    public int Roll { get; set; }
    public int TargetNumber { get; set; }
    public ERollResult result;
    public string Log { get; set; }
}