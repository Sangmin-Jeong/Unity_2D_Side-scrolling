////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: EnemyController.cs
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

public class EnemyController : MonoBehaviour
{
    private GameObject _player;
    
    // Stats
    [Header("Character Stats")]
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    private float _MAX_Health;
    private Slider _healthSlider;
    [SerializeField] private Image _fill;
    private bool _canBeAbsorbed = false;
    
    // Damage
    private float _cooldown = 2f;
    private bool _canAttack = false;
    
    // Animation
    private SpriteRenderer _spriteRenderer;
    
    // SFX
    private EnemyAudio _enemyAudio;

    void Start()
    {
        _enemyAudio = GetComponent<EnemyAudio>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.Find("Player").gameObject;
        _healthSlider = GetComponentInChildren<Slider>();
        _MAX_Health = _health;
        _healthSlider.value = _health / _MAX_Health;
    }
    
    void Update()
    {
        if (_player)
        {
            _healthSlider.value = _health / _MAX_Health;
            Attack();
            Move();

            if (_canBeAbsorbed)
            {
                _fill.color = Color.red;
            }
            else
            {
                _fill.color = Color.yellow;
            }
        }
    }
    
    public void TakeDamage(GameObject causer, float damage)
    {
        if (_health <= 0) return;
        
        _health -= damage;
        float healthPercent = _health / _MAX_Health;
        _healthSlider.value = healthPercent;
        
        _enemyAudio.PlayHitSFX();
        
        // Hit Animation
        if(!DOTween.IsTweening(_spriteRenderer))
            _spriteRenderer.DOColor(Color.black, 0.1f).SetLoops(4, LoopType.Yoyo).OnComplete(() =>
            {
                _spriteRenderer.color = Color.white;
            });

        if (healthPercent < 0.2f)// && healthPercent > 0)
        {
            SetCanBeAbsorbed(true);
        }
        // else if (_health <= 0)
        // {
        //     Respawn(causer);
        // }
        
        
        //Debug.Log(_health);
    }

    private void Respawn(GameObject causer)
    {
        //causer.GetComponent<PlayerController>()._enemies.Remove(gameObject);
        transform.position = new Vector3(transform.position.x + 20, transform.position.y, transform.position.z);
        _canBeAbsorbed = false;
        _health = _MAX_Health;
        _healthSlider.value = 1.0f;
    }

    public void Absorbed(GameObject causer)
    {
        if (GetCanBeAbsorbed())
        {
            Respawn(causer);
        }
    }

    private void Attack()
    {
        _cooldown += Time.deltaTime;
        if (_cooldown > 2f)
        {
            if (_canAttack)
            {
                _player.GetComponent<PlayerController>().TakeDamage(gameObject, _damage);
                _cooldown = 0f;
            }
        }
    }

    private void Move()
    {
        float distance = Vector3.Distance(_player.transform.position, transform.position);
        if (distance < 0.5) return;
        
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        Vector3 movement = direction * (Time.deltaTime * _speed);
        
        // Mathf.Sign() = Return value 1 when parameter is positive or zero
        // -1 when parameter is negative
        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(-direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            _healthSlider.gameObject.transform.localScale = new Vector2(Mathf.Sign(-direction.x) * Mathf.Abs(_healthSlider.gameObject.transform.localScale.x), _healthSlider.gameObject.transform.localScale.y);

        }
        transform.Translate(movement);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _canAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _canAttack = false;
        }
    }

    public void SetCanBeAbsorbed(bool b)
    {
        _canBeAbsorbed = b;
    }

    public bool GetCanBeAbsorbed()
    {
        return _canBeAbsorbed;
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }
}
