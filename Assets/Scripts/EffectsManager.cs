using UnityEngine;

public class EffectsManager : MonoBehaviourSingleton<EffectsManager> {
    [SerializeField] private ParticleSystem boom;

    protected override void OnAwake() {}

    public static void Boom(Cell cell) {
        ParticleSystem ps = Instantiate(Instance.boom);
        ps.transform.position = cell.Position.ToWorldPosition();
    }
}
