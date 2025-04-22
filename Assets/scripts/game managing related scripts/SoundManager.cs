using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]private AudioSource musicSource;
    [SerializeField]private AudioSource effectSource;
    
    public AudioClip backGroundMusic;

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
