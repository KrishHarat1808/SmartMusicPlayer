using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClip;
    AudioSource audioSource;
    int index;
    public Text SongName;
    public Slider TimeBar;
    private int firstPlayInt;
    public Slider volumeslider;
    public float volumefloat;

    void Start()
    {
        index = 0;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip[index];

        volumefloat = 0.50f;
        volumeslider.value = volumefloat;
    }

    void Update()
    {
        SongName.text = audioClip[index].name;
        TimeBar.maxValue = audioClip[index].length;
        TimeBar.value = audioSource.time;
    }
   void __Play()
    {
        audioSource.Play();
    }

    void __Pause()
    {
        audioSource.Pause();
    }

    void __Mute()
    {
        audioSource.mute = true;
    }

    void __UnMute()
    {
        audioSource.mute = false;
    }
    
    void __Next()
    {
        
        if(index== audioClip.Length -1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        audioSource.clip = audioClip[index];
        __Play();
    }

    void __Prev()
    {
       
        if (index == 0)
        {
            index = audioClip.Length - 1;
        }
        else
        {
            index--;
        }
        audioSource.clip = audioClip[index];
        __Play();
    }

   /* void __VolAdd()
    {
        volumeslider.value += 0.25f;
    }

    void __VolMin()
    {
        volumeslider.value -= 0.25f;
    }*/
    public void UpdateSound()
    {
        audioSource.volume = volumeslider.value;
    }
    public void UpdateSoundVoice(float a)
    {
        audioSource.volume +=a;
    }
}
