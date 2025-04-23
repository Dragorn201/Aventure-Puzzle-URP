using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("audio sources")]
    [SerializeField]private AudioSource musicSource;
    [SerializeField]private AudioSource effectSource;
    
    [Header("music clips")]
    public AudioClip backGroundMusic;
    
    [Header("sfx clips")]
    public AudioClip playerLandOnWall;
    public AudioClip playerDestroyWall;
    public AudioClip playerTrhowingHook;
    public AudioClip bellGong;

    void Start()
    {
        musicSource.clip = backGroundMusic;
        musicSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
        
    }
    
}
