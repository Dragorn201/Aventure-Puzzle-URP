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
    
    bool keyPressed = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space))
        {
            keyPressed = true;
        }
    }
    


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
        float elapsedTime = 0;
        keyPressed = false;
        while (elapsedTime < cinematicTime && !keyPressed)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        creditPlayer.SetActive(true);
        soundManagerHolder.SetActive(false);
        elapsedTime = 0;
        keyPressed = false;
        while (elapsedTime < creditTime && !keyPressed)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SceneLoader.LoadMenu();
    }
}
