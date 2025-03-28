using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public int sceneToLoad;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
