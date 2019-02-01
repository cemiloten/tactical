using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> agentsPrefabs;
    private List<Agent> agents;

    public Agent Selection { get; private set; }

    void Start()
    {
        agents = new List<Agent>(agentsPrefabs.Count);
        for (int i = 0; i < agentsPrefabs.Count; ++i)
        {
            GameObject go = Instantiate(agentsPrefabs[i]) as GameObject;
            Agent agent = go.GetComponent<Agent>();
            if (agent == null)
            {
                agent = go.AddComponent<Agent>();
            }
            agents.Add(agent);
        }

        MapManager.Instance.PlaceAgents(agents);
        Selection = null;
    }

    void Update()
    {
        if (!Camera.main)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Selection = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // RaycastHit hit;
            // if (Physics.Raycast(
            //     Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("Map")))
            // {
            //     Vector2Int pos = new Vector2Int((int)hit.point.x, (int)hit.point.z);
            //     if (MapManager.Instance.IsPositionOnMap(pos))
            //     {
            //         if (Selection != null)
            //         {
            //             Selection.MoveTo(pos);
            //         }
            //         else
            //         {
            //             Selection = AgentAt(pos);
            //             Debug.Log(Selection);
            //         }
            //     }
            // }
        }
    }

    private Agent AgentAt(Vector2Int pos)
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            if (agents[i].Position == pos)
            {
                return agents[i];
            }
        }
        return null;
    }

    private bool SelectionIsEmpty()
    {
        return Selection == null;
    }
}
