using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData : ScriptableObject {
    public int height = 20;
    public int npcCount = 3;

    public Vector2Int playerPosition = Vector2Int.zero;
    public int width = 10;
}