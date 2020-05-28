using DG.Tweening;
using UnityEngine;

public class TurnIndicator : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float transitionTime = 1f;

    private bool HasAgent { get; set; }
    private Transform Agent { get; set; }

    private void OnEnable() {
        GameEvents.Victory += OnEndGame;
        GameEvents.Defeat += OnEndGame;
    }

    private void OnDisable() {
        GameEvents.Victory -= OnEndGame;
        GameEvents.Defeat -= OnEndGame;
    }

    private void Awake() {
        HasAgent = false;
    }

    private void OnEndGame() {
        HasAgent = false;
        Agent = null;
    }

    private void LateUpdate() {
        if (!HasAgent)
            return;

        Transform trs = transform;
        trs.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        trs.position = Agent.position;
    }

    public void SetAgent(Transform agent) {
        HasAgent = false;

        transform.DOMove(agent.position, transitionTime)
            .SetEase(Ease.InOutQuad)
            .OnComplete(delegate {
                Agent = agent;
                HasAgent = true;
            });
    }
}