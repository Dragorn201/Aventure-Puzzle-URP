using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameEvent : MonoBehaviour
{
    public void EndGame()
    {
        SceneLoader.LoadMenu();
    }
}
