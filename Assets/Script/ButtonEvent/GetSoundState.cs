using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class GetSoundState : MonoBehaviour {

    AudioSource audio;
    [SerializeField]
    bool IsBack;
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update ()
    {
       // if(IsBack)
      //  audio.enabled = Static.Instance.MusicSwich;
        //else
          //  audio.enabled = Static.Instance.MusicSwichButton;
    }

    public void SwithMusic(Toggle tpg)
    {
        Debug.Log(tpg.isOn);
        audio.enabled = tpg.isOn;
    }
}
