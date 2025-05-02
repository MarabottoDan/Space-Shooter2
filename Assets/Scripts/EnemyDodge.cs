using System.Collections;
using UnityEngine;

public class EnemyDodge : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed = 3.5f;
    private Vector3 _startPosition;

    [Header("Firing")]
    [SerializeField] private GameObject _laserEnemyPrefab;
    [SerializeField] private float _fireRate = 3.0f;
    private float _nextFire = -1f;

    
    [Header("Dodge Settings")]
    [SerializeField] private float _detectionRange = 5f;// How far the enemy can "see" lasers
    [SerializeField] private float _dodgeCooldown = 3f;// Time between dodge attempts
    [SerializeField] private float _dodgeSpeed = 7f; // Speed of the dodge movement
    [SerializeField] private float _dodgeChance = 0.5f;  // 50% chance to dodge if detected
    [SerializeField] private float _dodgeDuration = 0.4f; // How long the dodge lasts

    private float _lastDodgeTime = -Mathf.Infinity;// Tracks the last time the enemy dodged to enforce cooldown
    private bool _isDodging = false; // Flag to check if the enemy is currently dodging
    private Vector3 _dodgeDirection;  // Stores the direction (left or right) of the current dodge

    [Header("Other")]
    [SerializeField] private GameObject _explosionPrefab;
    private bool _hasHit = false;

    private Player _player;
    private Animator _anim;
    private AudioSource _audio;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy.cs: Player reference is NULL");
        }
        else
        {
            Debug.Log("Enemy.cs: Player reference is OK");
        }

        _anim = GetComponent<Animator>();
       
        _startPosition = transform.position;

        if (_player == null) Debug.LogError("Player is NULL");
        if (_anim == null) Debug.LogError("Animator is NULL");
        
        if (_laserEnemyPrefab == null) Debug.LogError("LaserEnemyPrefab is NULL");

        _nextFire = Time.time + Random.Range(1f, 3f); // Delay first shot
    }

    private void Update()
    {
        if (_isDodging)// If currently dodging, move in the stored dodge direction at dodge speed
        {
            transform.Translate(_dodgeDirection * _dodgeSpeed * Time.deltaTime);
        }
        else
        {// If not dodging, move downward normally
            transform.Translate(Vector3.down * _speed * Time.deltaTime);       
            TryDodgeLaser();// Check if the enemy should dodge an incoming player laser
            TryToShoot();// Attempt to fire a laser at the player
        }

        if (transform.position.y < -11f)
        {
            float randomX = Random.Range(-20f, 20f);
            transform.position = new Vector3(randomX, 13f, 0);
        }
    }

    private void TryToShoot()
    {
        if (Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            Vector3 laserOffset = new Vector3(0, -2.5f, 0);
            GameObject enemyLaser = Instantiate(_laserEnemyPrefab, transform.position + laserOffset, Quaternion.identity);

            // Mark the laser as an enemy laser so it can hurt the player
            DodgeLaser laserScript = enemyLaser.GetComponent<DodgeLaser>();
            if (laserScript != null)
            {
                laserScript.AssignEnemyLaser();
            }

        }
    }

    private void TryDodgeLaser()// Checks for nearby player lasers and decides whether to dodge
    {   // Find all active player lasers in the scene
        GameObject[] playerLasers = GameObject.FindGameObjectsWithTag("PlayerLaser");

        foreach (GameObject laser in playerLasers)
        {   // Calculate distance between this enemy and the current laser
            float distance = Vector3.Distance(transform.position, laser.transform.position);
            // Check if laser is within dodge range and cooldown has passed
            if (distance < _detectionRange && Time.time - _lastDodgeTime > _dodgeCooldown)
            {
                if (Random.value < _dodgeChance)
                {   // Update last dodge time
                    _lastDodgeTime = Time.time;
                    // Start dodging
                    _isDodging = true;
                    // Randomly pick left or right direction to dodge
                    _dodgeDirection = Random.value < 0.5f ? Vector3.left : Vector3.right;
                    //Start coroutine to stop dodging after a short duration
                    StartCoroutine(EndDodge());
                    break;
                }
            }
        }
    }

    private IEnumerator EndDodge()
    {
        yield return new WaitForSeconds(_dodgeDuration);
        _isDodging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            _hasHit = true;
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("PlayerLaser"))
        {
            _hasHit = true;
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.AddScore(20);
            }
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        
    }
}
