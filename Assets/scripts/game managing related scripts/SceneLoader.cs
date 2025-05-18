using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadSceneClean(string sceneName)
    {
        // Détruire tous les objets racines de la scène actuelle
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        // Forcer un chargement propre (mode Single)
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public static void LoadMenu()
    {
        LoadSceneClean("MainMenu");
    }

    public static void LoadGame()
    {
        LoadSceneClean("LD1 Final"); 
    }
}