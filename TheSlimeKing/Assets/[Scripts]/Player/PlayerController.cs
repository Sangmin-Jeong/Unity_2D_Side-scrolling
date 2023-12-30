////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PlayerController.cs
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
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PlayerInputAction _inputAction;
    
    // Stats
    [Header("Character Stats")] 
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    public float _MAX_Health;
    private bool _isDead = false;
    public int absorbingCount;

    // Movement
    private float _direction = 0;
    private Rigidbody2D _rigidbody2D;
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private Boundary _boundary;
    
    // Attack
    public List<GameObject> _enemies = new List<GameObject>();
    [HideInInspector] public bool _canAttack = true;
    
    // Animation
    private PlayerAnimController _playerAnimController;
    private SpriteRenderer _spriteRenderer;
    
    // Events
    public EventHandler OnHealthChanged;
    public EventHandler OnSelectPerk;
    
    // UI
    private Slider _healthSlider;
    [SerializeField] private Image _crown;
    
    // SFX
    private PlayerAudio _playerAudio;
    
    // Win Condition
    [SerializeField] private int _winSpecifier;
    
    private void Awake()
    {
        // Input Actions
        _inputAction = new PlayerInputAction();
        _inputAction.Enable();

        _inputAction.Player.Move.performed += Move =>
        {
            _direction = Move.ReadValue<float>();
            _playerAnimController.PlayWalkAnim();
        };
        _inputAction.Player.Move.canceled += MoveCanceled =>
        {
            if(!_isDead)
            _playerAnimController.StopWalkAnim();
        };
        _inputAction.Player.Attack.performed += Attack;
        _inputAction.Player.Absorbing.performed += Absorbing;
        //_inputAction.Player.Option.performed += OptionKey;
    }

    private void Start()
    {
        // Initialize components
        _playerAudio = GetComponent<PlayerAudio>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _healthSlider = GetComponentInChildren<Slider>();
        _playerAnimController = GetComponent<PlayerAnimController>();
        _MAX_Health = _health;
        _healthSlider.value = _health / _MAX_Health;
        _crown.gameObject.SetActive(false);
        
        // Events
        OnHealthChanged += PlayerController_OnHealthChanged;
    }

    private void PlayerController_OnHealthChanged(object sender, EventArgs e)
    {
        
    }

    // private void OptionKey(InputAction.CallbackContext obj)
    // {
    // }

    private void Absorbing(InputAction.CallbackContext obj)
    {
        _playerAnimController.PlayAbsorbingAnim();
        Absorbing(_enemies);
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        _playerAnimController.PlayAttackAnim();
        Attack(_enemies);
    }
    void Update()
    {
        if (_isDead) return;
        

        
        DebugInput();

        Movement();
    }
    private void Movement()
    {
        _rigidbody2D.velocity = new Vector2(_direction * (_speed * Time.deltaTime), _rigidbody2D.velocity.y);
        
        if (transform.position.x < _boundary.MIN_X)
        {
            _rigidbody2D.velocity = Vector2.zero;
            float offset = _boundary.MIN_X - transform.position.x;
            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        }
        else if(transform.position.x > _boundary.MAX_X)
        {
            _rigidbody2D.velocity = Vector2.zero;
            float offset = transform.position.x - _boundary.MAX_X;
            transform.position = new Vector3(transform.position.x - offset, transform.position.y, transform.position.z);
        }
        
        
        
        // Mathf.Sign() = Return value 1 when parameter is positive or zero
        // -1 when parameter is negative
        if (_direction != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(-_direction) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            _healthSlider.gameObject.transform.localScale = new Vector2(Mathf.Sign(-_direction) * Mathf.Abs(_healthSlider.gameObject.transform.localScale.x), _healthSlider.gameObject.transform.localScale.y);
        }
        
    }

    private static void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoader.Instance.LoadScene("GameOverScene");
        }
    }

    public void TakeDamage(GameObject causer, float damage)
    {
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        
        _playerAudio.PlayHitSFX();
        
        // Hit Animation
        if(!DOTween.IsTweening(_spriteRenderer))
        _spriteRenderer.DOColor(Color.black, 0.1f).SetLoops(4, LoopType.Yoyo).OnComplete(() =>
        {
            _spriteRenderer.color = Color.white;
        });
        
        _health -= damage;
        if (_health <= 0)
        {
            // Dead
            SetDead();
        }
        
        _healthSlider.value = _health / _MAX_Health;
        //Debug.Log(_health);
    }

    private void SetDead()
    {
        _playerAnimController.DisableAnimator();
        _inputAction.Disable();
        _health = 0;
        _isDead = true;
        transform.DOScale(new Vector3(0, 0, 1), 2f).OnComplete(() => { SceneLoader.Instance.LoadScene("GameOverScene"); });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            _enemies.Add(other.gameObject);
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            _enemies.Remove(other.gameObject);
        }
    }
    
    private void Attack(List<GameObject> targets)
    {
        if (_canAttack)
        {
            foreach (GameObject enemy in targets)
            {
                enemy.GetComponent<EnemyController>().TakeDamage(gameObject, _damage);
                if (targets.Count < 1) return;
            }
            
        }
    }
    
    private void Absorbing(List<GameObject> targets)
    {
        if (_canAttack)
        {
            _playerAudio.PlayEatSFX();
            foreach (GameObject enemy in targets)
            {
                EnemyController temp = enemy.GetComponent<EnemyController>();
                if (temp.GetCanBeAbsorbed())
                {
                    temp.Absorbed(gameObject);
                    absorbingCount++;
                    transform.localScale = new Vector3(transform.localScale.x + Mathf.Sign(transform.localScale.x)
                        ,transform.localScale.y + Mathf.Sign(transform.localScale.y)
                        ,transform.localScale.z);
                }
                
                if (targets.Count < 1) return;
            }
        }

        // Become the King = win the game
        if (Mathf.Abs(transform.localScale.x) > _winSpecifier)
        {
            StartCoroutine(WaitForWin());
        }
        else
        {
            // Absorbed enough to get a perk
            if (absorbingCount >= 8)
            {
                StartCoroutine(WaitForPerk());
            }
        }
    }
    
    private void OnDestroy()
    {
        absorbingCount = 0;
        _enemies.Clear();
        DOTween.Clear();
    }

    IEnumerator WaitForWin()
    {
        _inputAction.Disable();
        _playerAnimController.DisableAnimator();
        SoundManager.Instance.PlayWinSFX();
        _crown.gameObject.SetActive(true);
        _healthSlider.gameObject.SetActive(false);
        EnemySpawner.Instance.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(2f);
        
        SceneLoader.Instance.LoadScene("GameOverScene");
    }

    public void PowerUpForSeconds(float seconds)
    {
        StartCoroutine(PowerUpDuration(seconds));
    }

    IEnumerator PowerUpDuration(float seconds)
    {
        _damage *= 2;
        
        yield return new WaitForSeconds(seconds);

        _damage /= 2;
    }
    
    public void IncreaseMaxHealth()
    {
        _MAX_Health += _MAX_Health * 0.3f;
    }
    
    public void GetHeal()
    {
        
        if ((_health + _MAX_Health * 0.3f) > _MAX_Health)
        {
            _health = _MAX_Health;
        }
        else
        {
            _health += _MAX_Health * 0.3f;
        }
    }

    IEnumerator WaitForPerk()
    {
        absorbingCount = 0;
        yield return new WaitForSeconds(0.5f);
        OnSelectPerk?.Invoke(this, EventArgs.Empty);
    }
}