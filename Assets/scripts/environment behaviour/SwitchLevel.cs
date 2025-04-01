using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchLevel : MonoBehaviour
{
    public int levelToLoad;
    
    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
