////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: MainUI.cs
//StudentName: Sangmin Jeong
//StudentID: 101369732
//Last Modified On: 18/10/2023
//Program Description: GAME2014-Mobile
//Revision History: V1.0
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;
    [SerializeField] private Button absorbingBtn;
    [SerializeField] private Button attackBtn;
    [SerializeField] private Button optionBtn;
    [SerializeField] private OptionUI optionPanel;
    [SerializeField] private GameObject perkPanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private Text touchText;
    
    private PlayerController _player;
    
    private void Start()
    {
        Hide();
        
        _player = FindObjectOfType<PlayerController>();
        _player.OnSelectPerk += PlayerController_OnSelectPerk;
        
        instructionPanel.SetActive(true);
        touchText.transform.DOScale(new Vector3(0.8f,0.8f,0.8f),1.0f).SetLoops(-1, LoopType.Yoyo);
        SoundManager.Instance.StopMusic();
        EnemySpawner.Instance.gameObject.SetActive(false);
        //Time.timeScale = 0;
        
        // optionBtn.onClick.AddListener(() =>
        // {
        //
        // });
    }

    public void OptionBtnPressed()
    {
        optionPanel.Show();
        Time.timeScale = 0;
        Hide();
    }

    private void Update()
    {
        if (!instructionPanel.activeSelf) return;
        
        if (Input.anyKeyDown)
        {
            instructionPanel.SetActive(false);
            SoundManager.Instance.PlayBGM();
            EnemySpawner.Instance.gameObject.SetActive(true);
            Show();
            Time.timeScale = 1;
            DOTween.Clear();
        }
    }

    public void Show()
    {
        leftBtn.gameObject.SetActive(true);
        rightBtn.gameObject.SetActive(true);
        absorbingBtn.gameObject.SetActive(true);
        attackBtn.gameObject.SetActive(true);
        optionBtn.gameObject.SetActive(true);
    }

    public void Hide()
    {
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(false);
        absorbingBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        optionBtn.gameObject.SetActive(false);
    }
    
    private void PlayerController_OnSelectPerk(object sender, EventArgs e)
    {
        Time.timeScale = 0;
        
        perkPanel.SetActive(true);
    }
}
