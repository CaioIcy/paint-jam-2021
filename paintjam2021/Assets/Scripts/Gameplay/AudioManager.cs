using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource SourceMusic;
    public AudioSource SourceSFX;

    [Header("Music")] //
    public AudioClip MusicLoop;

    [Header("SFX")] //
    public AudioClip SquishRoach;
    public List<AudioClip> Selects;
    public AudioClip Correct;
    public AudioClip Incorrect;
    public List<AudioClip> Aliens;


    public static AudioManager ins;

    private void Awake()
    {
        ins = this;
        //SourceMusic.loop = true;
        //SourceMusic.PlayOneShot(MusicLoop);
    }

    public void Play(AudioClip clip)
    {
        SourceSFX.PlayOneShot(clip);
    }

    public void Play(List<AudioClip> clips)
    {
        var rand = new System.Random();
        Play(clips[rand.Next(0, clips.Count)]);
    }
}
