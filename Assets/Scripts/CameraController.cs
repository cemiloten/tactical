using System;
using Lean.Touch;
using UnityEngine;

public class CameraController : MonoBehaviourSingleton<CameraController> {
    protected override void OnAwake() {
    }

    private void OnEnable() {
        Lean.Touch.LeanTouch.OnFingerDown += OnFingerDown;
    }

    private void OnDisable() {
        Lean.Touch.LeanTouch.OnFingerDown -= OnFingerDown;
    }

    private void OnFingerDown(LeanFinger finger) {
        throw new NotImplementedException();
    }
}
