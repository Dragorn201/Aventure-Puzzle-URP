using UnityEngine;
using HoudiniEngineUnity;

public class DestructionSTP : MonoBehaviour
{
    public HEU_HoudiniAsset myHoudiniAsset; //jassigne mon asset dans l'inspector


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision d�tect�e avec : " + other.gameObject.name); // V�rifie si le trigger fonctionne

        if (other.CompareTag("Player")) // V�rifie si l'objet a bien le tag "Player"
        {
            Debug.Log("Le Player a touch� l'objet !");
            if (myHoudiniAsset != null)
            {
                Debug.Log("Houdini Asset trouv�, d�clenchement de la destruction !");
                myHoudiniAsset.RequestCook(true, true); // Force le recalcul de la simulation
            }
            else
            {
                Debug.LogError("Houdini Asset non assign� !");
            }
        }
    }

}
