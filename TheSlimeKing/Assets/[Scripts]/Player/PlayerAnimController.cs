////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PlayerAnimController.cs
//StudentName: Sangmin Jeong
//StudentID: 101369732
//Last Modified On: 18/10/2023
//Program Description: GAME2014-Mobile
//Revision History: V1.0
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayAttackAnim()
    {
        StopWalkAnim();
        _animator.SetTrigger("Attack");
    }
    public void PlayAbsorbingAnim()
    {
        StopWalkAnim();
        _animator.SetTrigger("Absorbing");
    }
    public void PlayWalkAnim()
    {
        _animator.SetBool("Walk", true);
    }
    public void StopWalkAnim()
    {
        _animator.SetBool("Walk", false);
    }

    public void DisableAnimator()
    {
        _animator.enabled = false;
    }
}