using UnityEngine;
using UnityEngine.VFX;

public class LanternFireLighting : MonoBehaviour
{
    [SerializeField]private VisualEffect fireEffect;

    void Awake()
    {
        fireEffect = GetComponent<VisualEffect>();
        fireEffect.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireEffect.Play();
        }
    }
}
