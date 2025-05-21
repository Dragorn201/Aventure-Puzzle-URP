using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameEvent : MonoBehaviour
{

    public float cinematicTime;
    public GameObject creditPlayer;
    public float creditTime;
    public GameObject soundManagerHolder;
    


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
        soundManagerHolder.SetActive(false);
        yield return new WaitForSeconds(creditTime);
        SceneLoader.LoadMenu();
    }
}
