using DG.Tweening;
using UnityEngine;

namespace UI.Hub {

public class HubMenu : MonoBehaviour {
    [SerializeField] private float tapToStartDuration = 1f;
    [SerializeField] private float tapToStartScale = 1.5f;
    [Header("Tap to start values")]
    [SerializeField] private RectTransform tapToStartText = default;

    private void Start() {
        tapToStartText.DOScale(tapToStartScale * Vector3.one, tapToStartDuration)
            .SetEase(Ease.InOutCubic)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnTapToStart() {
        GameEvents.OnCombat();
    }
}

}