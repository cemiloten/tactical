using System;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState {
    Hub,
    Combat,
    Victory,
    Defeat
}

public static class GameStateExtensions {
    public static int ToSceneIndex(this GameState gameState) {
        switch (gameState) {
            case GameState.Hub:     return 1;
            case GameState.Combat:  return 2;
            case GameState.Victory: return 3;
            case GameState.Defeat:  return 3;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }
}

public class GameManager : MonoBehaviourSingleton<GameManager> {
    private static GameState State = GameState.Hub;

    private void OnEnable() {
        GameEvents.Victory += OnVictory;
        GameEvents.Defeat += OnDefeat;
        GameEvents.Combat += OnCombat;
        GameEvents.Hub += OnHub;

        State = GameState.Hub;
        SceneManager.LoadSceneAsync(State.ToSceneIndex(), LoadSceneMode.Additive);
    }

    private void OnDisable() {
        GameEvents.Victory -= OnVictory;
        GameEvents.Defeat -= OnDefeat;
        GameEvents.Combat -= OnCombat;
        GameEvents.Hub -= OnHub;
    }

    private void SetGameState(GameState newState) {
        StartCoroutine(UnloadScene(State));

        State = newState;
        SceneManager.LoadSceneAsync(State.ToSceneIndex(), LoadSceneMode.Additive);
    }

    private IEnumerator UnloadScene(GameState state) {
        yield return null;
        SceneManager.UnloadSceneAsync(state.ToSceneIndex());
    }

    private void OnVictory() {
        SetGameState(GameState.Victory);
    }

    private void OnDefeat() {
        SetGameState(GameState.Defeat);
    }

    private void OnCombat() {
        SetGameState(GameState.Combat);
    }

    private void OnHub() {
        SetGameState(GameState.Hub);
    }
}
