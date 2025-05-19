using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [Header("audio sources")]
    [SerializeField]private AudioSource musicSource;
    [SerializeField]private AudioSource effectSource;
    
    [Header("music clips")]
    public AudioClip[] musicClip;
    
    [Header("sfx clips")]
    public AudioClip playerTrhowingHook;
    public AudioClip hookHitWall;
    public AudioClip playerLandOnWall;
    public AudioClip environmentMovingBloc;
    public AudioClip playerDestroyWall;
    public AudioClip lanternFire;
    public AudioClip bellGong;
    public AudioClip endingPurification;
    
    bool cinematicSkipped = false;

    void Start()
    {
        int ActualScene = SceneManager.GetActiveScene().buildIndex;
        
        musicSource.clip = musicClip[ActualScene];
        switch (ActualScene)
        {
            case 1:
                StartCoroutine(WaitBeforePlayingMusic(57));
                break;
            default:
                StartCoroutine(WaitBeforePlayingMusic(7));
                break;
                
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    IEnumerator WaitBeforePlayingMusic(float timer)
    {
        float elapsedTime = 0;
        while (!cinematicSkipped && elapsedTime < timer)
        {
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        musicSource.Play();
    }

    public void StartMusicEarly()
    {
        cinematicSkipped = true;
    }
    
}
