using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot rainSnapshot;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        Play("AMB_Temple");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        foreach (AudioSource source in s.sources)
        {
            source.outputAudioMixerGroup = s.audioMixerGroup;
            source.clip = s.clip;
            source.loop = s.loop;
            source.volume = s.volume;
            source.pitch = s.pitch;
            source.Play();
        }

    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        foreach (AudioSource source in s.sources)
        {
            source.Stop();
        }


    }

    private void Update()
    {
        if(Input.GetKeyDown("a"))
        {
            Play("SFX_Flags");
        }


        
    }



}
