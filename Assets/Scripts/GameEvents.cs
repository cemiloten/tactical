using Agents;
using UnityEngine.Events;

public static class GameEvents {
    public static readonly UnityEvent TurnEnd = new UnityEvent();
    public static readonly CellEvent CellSelected = new CellEvent();
    public static readonly AgentEvent AgentDead = new AgentEvent();
    public static readonly CastEvent FinishedCasting = new CastEvent();

    public class CellEvent : UnityEvent<Cell> {}

    public class AgentEvent : UnityEvent<Agent> {}

    public struct CastData {
        private Agent caster;
        private Cell source, target;
    }

    public class CastEvent : UnityEvent<CastData> {}
}