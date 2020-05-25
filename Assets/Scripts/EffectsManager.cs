using UnityEngine;

public class EffectsManager : MonoBehaviourSingleton<EffectsManager> {
    [SerializeField] private ParticleSystem attack = default;
    [SerializeField] private ParticleSystem boom = default;

    public static void Boom(Cell cell) {
        ParticleSystem ps = Instantiate(Instance.boom);
        ps.transform.position = cell.Position.ToWorldPosition();
    }

    public static Transform Attack(Cell cell) {
        ParticleSystem ps = Instantiate(Instance.attack);
        Transform trs = ps.transform;
        trs.position = cell.Position.ToWorldPosition();
        return trs;
    }
}