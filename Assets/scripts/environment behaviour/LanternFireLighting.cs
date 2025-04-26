using UnityEngine;
using UnityEngine.VFX;

public class LanternFireLighting : MonoBehaviour
{
    [SerializeField]private VisualEffect fireEffect;
    [SerializeField]private GameObject lightEffect;

    void Awake()
    {
        fireEffect.Stop();
        lightEffect.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireEffect.Play();
            lightEffect.SetActive(true);
        }
    }
}
