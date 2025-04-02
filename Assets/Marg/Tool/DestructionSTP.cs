using UnityEngine;
using HoudiniEngineUnity;

public class DestructionSTP : MonoBehaviour
{
    public HEU_HoudiniAsset myHoudiniAsset; //jassigne mon asset dans l'inspector


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision détectée avec : " + other.gameObject.name); // Vérifie si le trigger fonctionne

        if (other.CompareTag("Player")) // Vérifie si l'objet a bien le tag "Player"
        {
            Debug.Log("Le Player a touché l'objet !");
            if (myHoudiniAsset != null)
            {
                Debug.Log("Houdini Asset trouvé, déclenchement de la destruction !");
                myHoudiniAsset.RequestCook(true, true); // Force le recalcul de la simulation
            }
            else
            {
                Debug.LogError("Houdini Asset non assigné !");
            }
        }
    }

}
