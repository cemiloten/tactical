using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Level", menuName="Tactical/Level")]
public class Level : ScriptableObject
{
    [System.Serializable]
    public struct AgentWithPosition
    {
        public Agent.Type type;

        [Range(0, 1)]
        public int team;

        public Vector2Int position;
    }

    public int width;

    public int height;

    public AgentWithPosition[] agents;

    public Vector2Int[] holes;
}