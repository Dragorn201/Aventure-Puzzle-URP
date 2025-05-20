using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameEvent : MonoBehaviour
{

    public float cinematicTime;


    public void EndGame()
    {
        StartCoroutine(Wiat());
    }

    IEnumerator Wiat()
    {
        yield return new WaitForSeconds(cinematicTime);
        SceneLoader.LoadMenu();
    }
}
