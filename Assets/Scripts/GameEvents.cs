using Agents;

public static class GameEvents {
    // Agents
    public delegate void AgentEvent(Agent agent);


    // Cells
    public delegate void CellEvent(Cell cell);
    // Void
    public delegate void Event();

    public static event Event Hub;
    public static void OnHub() => Hub?.Invoke();

    public static event Event Combat;
    public static void OnCombat() => Combat?.Invoke();

    public static event Event Victory;
    public static void OnVictory() => Victory?.Invoke();

    public static event Event Defeat;
    public static void OnDefeat() => Defeat?.Invoke();

    public static event CellEvent CellSelected;
    public static void OnCellSelected(Cell cell) => CellSelected?.Invoke(cell);

    public static event AgentEvent AgentDead;
    public static void OnAgentDead(Agent agent) => AgentDead?.Invoke(agent);

    public static event AgentEvent AgentStartTurn;
    public static void OnAgentStartTurn(Agent agent) => AgentStartTurn?.Invoke(agent);

    public static event AgentEvent AgentEndTurn;
    public static void OnAgentEndTurn(Agent agent) => AgentEndTurn?.Invoke(agent);
}