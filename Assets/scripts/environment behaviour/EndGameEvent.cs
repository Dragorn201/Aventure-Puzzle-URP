using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameEvent : MonoBehaviour
{

    public float cinematicTime;
    public GameObject creditPlayer;
    public float creditTime;


    private void Start()
    {
        creditPlayer.SetActive(false);
    }

    public void EndGame()
    {
        StartCoroutine(Wiat());
    }

    IEnumerator Wiat()
    {
        yield return new WaitForSeconds(cinematicTime);
        creditPlayer.SetActive(true);
        yield return new WaitForSeconds(creditTime);
        SceneLoader.LoadMenu();
    }
}
