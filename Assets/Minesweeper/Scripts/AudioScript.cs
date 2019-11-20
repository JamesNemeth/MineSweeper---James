using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioClip SoundEffect;

    public AudioSource MusicSource;

    void Start()
    {
        //Play Audio attached to the audio clip
        MusicSource.clip = SoundEffect;
    }
    void Update()
    {
        //Left mouse is clicked...
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Play audio
            MusicSource.Play();
        }   
    }
}
