////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PlayerAudio.cs
//StudentName: Sangmin Jeong
//StudentID: 101369732
//Last Modified On: 18/10/2023
//Program Description: GAME2014-Mobile
//Revision History: V1.0
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource HitAudioSource;
    [SerializeField] private AudioSource EatAudioSource;
    [SerializeField] private AudioClip HitSFX;
    [SerializeField] private AudioClip EatSFX;

    private void Start()
    {
        HitAudioSource.clip = HitSFX;
        EatAudioSource.clip = EatSFX;
    }

    public void PlayHitSFX()
    {
        HitAudioSource.Play();
    }
    public void PlayEatSFX()
    {
        EatAudioSource.Play();
    }
}
