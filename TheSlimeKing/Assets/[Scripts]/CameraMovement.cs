////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: CameraMovement.cs
//StudentName: Sangmin Jeong
//StudentID: 101369732
//Last Modified On : 18/10/2023
//Program Description: GAME2014-Mobile
//Revision History: V1.0
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject _player;
    private Vector3 offset;
    
    void Start()
    {
        _player = GameObject.Find("Player").gameObject;
        
        offset = new Vector3(transform.position.x,transform.position.y - _player.transform.position.y,transform.position.z);
    }

    
    void Update()
    {
        transform.position = _player.transform.position + offset;
    }
}
