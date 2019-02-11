using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Map Layout", menuName="Tactical/Map Layout")]
public class MapLayout : ScriptableObject
{
    [System.Serializable]
    public struct AgentWithPositions
    {
        public GameObject agentPrefab;
        public List<Vector2Int> positions;
    }

    public int width;
    public int height;
    public bool mirrorX;
    public bool mirrorY;

    public AgentWithPositions pusher;
    public AgentWithPositions puller;
    public AgentWithPositions swapper;
    public AgentWithPositions heart;
}