using System;
using Lean.Touch;

public class CameraController : MonoBehaviourSingleton<CameraController> {
    private void OnEnable() {
        LeanTouch.OnFingerDown += OnFingerDown;
    }

    private void OnDisable() {
        LeanTouch.OnFingerDown -= OnFingerDown;
    }

    private void OnFingerDown(LeanFinger finger) {
        throw new NotImplementedException();
    }
}