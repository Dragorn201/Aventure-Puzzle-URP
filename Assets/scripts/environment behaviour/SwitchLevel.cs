using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchLevel : MonoBehaviour
{
    public int levelToLoad;

    public void CallSwitchLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
