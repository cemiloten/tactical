using UnityEngine.Events;

public static class GameEvents
{
    public static readonly UnityEvent TurnEnd = new UnityEvent();

    public class CellEvent : UnityEvent<Cell> { }
    public static readonly CellEvent CellSelected = new CellEvent();

    public class AgentEvent : UnityEvent<Agent> { }
    public static readonly AgentEvent AgentDead = new AgentEvent();

    public struct CastData
    {
        Agent caster;
        Cell source, target;
    }
    public class CastEvent : UnityEvent<CastData> { }
    public static readonly CastEvent FinishedCasting = new CastEvent();
}
