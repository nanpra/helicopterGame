using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    
    public Sound[] sounds;
    public static AudioManager instance;
    public bool canPlay = true;

    public AudioSource bgSound;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }
    }
   
    public void Play(string name)
    {
        if(canPlay)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.Play();
        }
    }
    public void Play(int startIndex, int endIndex)
    {
        string name = sounds[UnityEngine.Random.Range(startIndex, endIndex)].name;
        if(canPlay)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.Play();
        }
    }
   
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }
    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
        
    }
}
