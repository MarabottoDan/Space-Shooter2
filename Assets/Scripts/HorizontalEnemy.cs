using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalEnemy : MonoBehaviour
{
    [SerializeField] private float _horizontalSpeed = 5;
    [SerializeField] private GameObject _laser2Prefab;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _fireRate = 6f;
    [SerializeField] private float _laserSpeed = 5f;
    private float _canFire = -1f;
    private Player _player;
    [SerializeField]private Animator _anim;
    [SerializeField]private AudioSource _audioSource;
    private bool _isDead = false;
    private bool _isMovingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        FireLaser();
    }

    void Move()
    {
        if (_isDead) return;

        if (_isMovingRight)
        {
            transform.Translate(Vector3.right * _horizontalSpeed * Time.deltaTime);

            if (transform.position.x >= 24f)
            {
                _isMovingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * _horizontalSpeed * Time.deltaTime);

            if (transform.position.x <= -28f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void FireLaser()
    {
        if (Time.time > _canFire &&  !_isDead)
        {
            _fireRate = Random.Range(2f, 5f);
            _canFire = Time.time + _fireRate;
            Instantiate(_laser2Prefab, transform.position, Quaternion.identity);
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HorizontalEnemy HIT" + other.name);
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Damage();
        }
    }

    public void Damage()
    {
        if (_isDead) return;

        _isDead = true;

        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Explosion Spawned");
        }

        else
        {
            Debug.LogWarning("Explosion Prefab is not assigned");
        }
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null) sr.enabled = false;

        _horizontalSpeed = 0;

        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        if (_player != null)
        {
            _player.AddScore(20);
        }
       
        Destroy(this.gameObject, 2.8f);
    }


}
