using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    [SerializeField] public LevelData[] levelDatas = default;

    private void Start() {}
}