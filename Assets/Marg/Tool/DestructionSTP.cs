using UnityEngine;
using HoudiniEngineUnity;

public class DestructionSTP : MonoBehaviour
{
    public HEU_HoudiniAsset myHoudiniAsset; // HDA dans Unity
    public string houdiniParameter = "StartDestruction"; // Nom du paramètre Houdini


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) // Si le personnage touche l'objet
        {
            Debug.Log("Le Player a touché l'objet !");

            if (myHoudiniAsset != null)
            {
                myHoudiniAsset.SetBoolParameter(houdiniParameter, 1); // Active la destruction
                myHoudiniAsset.RequestCook(true, true); // Force le recook de l'HDA
            }
            else
            {
                Debug.LogError("Houdini Asset non assigné !");
            }
        }
    }

}
