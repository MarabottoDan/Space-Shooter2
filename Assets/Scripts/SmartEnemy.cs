using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : MonoBehaviour
{
    [Header("GeneralSettings")]
    [SerializeField] private int _health = 3;// How many hits the SmartEnemy can take

    [Header("Laser Settings")]
    [SerializeField] private GameObject _normalLaserPrefab;// Prefab for the normal front laser
    [SerializeField] private GameObject _dnaShotPrefab; // Prefab for the DNA shot
    [SerializeField] private Transform _frontLaserSpawnPoint;// Where normal lasers are spawned
    [SerializeField] private Transform _backLaserSpawnPoint;// Where DNA shots are spawned
    [SerializeField] private float _fireRate = 1.5f;//Time between normal laser fires
    [SerializeField] private float _dnaShotSpeed = 5f; // Speed of the DNA shots
    [SerializeField] private int _dnaShotCount = 5; // Number of DNA shots fired at once
    [SerializeField] private bool _canFireDNA = true;// Controls if DNA has already been fired

    [SerializeField] private float _moveSpeed = 2.0f; // Speed of downward movement

    [Header("Explosion Settings")]
    [SerializeField] private GameObject _explosionPrefab; // Prefab of the explosion effect


    private float _nextFire = 0f;// Timer for normal laser firing
    private GameObject _player;// Reference to player object


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        MoveDownward();
        NormalFiring();
        if (_player != null)
        {
            // Fire DNA shots if SmartEnemy is lower in Y-axis than player and has not fired yet
            if (_canFireDNA && transform.position.y < _player.transform.position.y)
            {
                FireDNAshots();
                _canFireDNA = false;// Prevents multiple DNA fires
            }
        }

        CheckBounds();
    }

    private void CheckBounds()
    {
        if (transform.position.y < -12f)
        {
            Destroy(this.gameObject);
        }
    }

    private void NormalFiring()
    {
        if  (Time.time > _nextFire)
        {
            GameObject enemyLaser = Instantiate(_normalLaserPrefab, _frontLaserSpawnPoint.position, Quaternion.identity);

            Laser laser = enemyLaser.GetComponent<Laser>();
            if (laser != null)
            {
                laser.AssignEnemyLaser();
            }

            _nextFire = Time.time + _fireRate;
        }
    }

    private void FireDNAshots()
    {
        float startAngle = 180f; // Left
        float endAngle = 0f;     // Right
        float angleStep = (startAngle - endAngle) / (_dnaShotCount - 1);

        for (int i = 0; i < _dnaShotCount; i++)
        {
            float angle = startAngle - i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            GameObject dnaShot = Instantiate(_dnaShotPrefab, _backLaserSpawnPoint.position, Quaternion.identity);

            Rigidbody2D rb = dnaShot.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * _dnaShotSpeed;
            }

            dnaShot.transform.rotation = Quaternion.Euler(0, 0, angle); // For visual spin
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerLaser"))
        {
            Laser laser = other.GetComponent<Laser>();

            if (laser != null && laser.HasHit() == false)
            {
                Destroy(other.gameObject); // Destroy the laser
                laser.MarkAsHit(); // Optional, if you want to prevent double hits
                Damage(); // Take 1 damage
            }
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            Damage(); // SmartEnemy takes damage if collides with player directly
        }
    }


    private void MoveDownward()
    {
        transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
    }


    public void Damage()
    {
        _health--;
        if (_health <= 0)
        {
            if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }
}


