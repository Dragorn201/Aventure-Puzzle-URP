using UnityEngine;
using UnityEngine.VFX;

public class LanternFireLighting : MonoBehaviour
{
    [SerializeField]private VisualEffect fireEffect;
    [SerializeField]private GameObject lightEffect;
    [SerializeField]private  SoundManager soundManager;

    void Awake()
    {
        fireEffect.enabled = false;
        lightEffect.SetActive(false);
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (soundManager != null) soundManager.PlaySoundEffect(soundManager.lanternFire);
            fireEffect.enabled = true;
            lightEffect.SetActive(true);
        }
    }
}
