using UnityEngine;
using UnityEngine.VFX;

public class SmokeTrigger : MonoBehaviour
{
    public VisualEffect vfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) 
        {
            vfx.SetVector3("ImpactPosition", other.transform.position);
            vfx.SendEvent("OnObjectEnter");
        }
    }
}
