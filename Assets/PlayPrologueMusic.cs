using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPrologueMusic : MonoBehaviour
{
    void Start()
    {
        JSAM.AudioManager.PlayMusic(JSAM.Music.Music_Prolouge);
    }

    void Enable()
    {
        JSAM.AudioManager.PlayMusic(JSAM.Music.Music_Prolouge);
    }
}
