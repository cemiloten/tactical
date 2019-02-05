using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private int pathIndex = 0;
    private Dictionary<Ability.CastType, Ability> abilities;

    public Ability CurrentAbility { get; set; }
    public Vector2Int Position { get; set; }
    public bool Busy
    {
        get
        {
            if (abilities == null || abilities.Count < 1)
                return false;
            var values = System.Enum.GetValues(typeof(Ability.CastType)) as Ability.CastType[];
            for (int i = 0; i < values.Length; ++i)
            {
                Ability a = null;
                if (abilities.TryGetValue(values[i], out a))
                {
                    if (a.Casting)
                        return true;
                }
            }
            return false;
        }
    }

    void Start()
    {
        // List < Ability
        // abilities.AddRange(GetComponents<Ability>());
        // if (abilities.Count > 0)
        //     CurrentAbility = abilities[0];
    }

    // public void CastAbility()
    // {
    //     currentAbility.Cast(cell);
    // }

    public void SetCurrentAbility(int index)
    {
        if (abilities == null)
        {
            Debug.LogError("Cannot update ability, {abilities} is null.");
            return;
        }

        if (abilities.Count < 1)
        {
            Debug.LogError("Cannot update ability, {abilities} count is 0");
            return;
        }

        if (index > abilities.Count - 1)
        {
            Debug.LogError("{index} outside of {abilities} range");
            return;
        }

        if (index < 0)
        {
            CurrentAbility = null;
            return;
        }

        // CurrentAbility = abilities[index];
    }

    public void SnapTo(Vector2Int pos)
    {
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Empty;
        Position = pos;
        transform.position = Utilities.ToWorldPosition(pos, transform);
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Agent;
    }
}
