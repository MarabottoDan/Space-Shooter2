using UnityEngine;

public class HorizontalEnemy : MonoBehaviour
{
    [SerializeField] private float _horizontalSpeed = 5; // Speed at which the enemy moves horizontally
    [SerializeField] private GameObject _laser2Prefab; // Prefab for the laser this enemy shoots
    [SerializeField] private GameObject _explosionPrefab;// Prefab for the explosion effect when destroyed   
    [SerializeField] private AudioSource _audioSource;// Reference to the AudioSource for explosion sound
    [SerializeField] private float _fireRate = 6f;// How often this enemy can shoot
    private float _canFire = -1f;// Time when the enemy can fire again
    private Player _player;// Reference to the Player script
    private bool _isDead = false;// Tracks whether the enemy has been destroyed
    private bool _isMovingRight = true;// Tracks the direction the enemy is moving (right or left)

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();// Get reference to the player in the scene
        _audioSource = GetComponent<AudioSource>(); // Get audio source component on this enemy
    }

    // Update is called once per frame
    void Update()
    {
        Move();// Handles movement
        FireLaser();// Handles shooting
    }

    void Move()
    {
        if (_isDead) return;// Do nothing if the enemy is dead

        if (_isMovingRight)
        {
            transform.Translate(Vector3.right * _horizontalSpeed * Time.deltaTime); // Move right

            if (transform.position.x >= 24f)// Change direction if it hits the right limit
            {
                _isMovingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * _horizontalSpeed * Time.deltaTime); // Move left

            if (transform.position.x <= -28f) // Destroy if it exits the left side of the screen
            {
                Destroy(this.gameObject);
            }
        }
    }

    void FireLaser()// Shoots a laser at random intervals
    {
        if (Time.time > _canFire &&  !_isDead) // Only fire if the time has passed and the enemy is not dead
        {
            _fireRate = Random.Range(2f, 5f);// Randomize next fire interval
            _canFire = Time.time + _fireRate;
            Instantiate(_laser2Prefab, transform.position, Quaternion.identity); // Spawn the laser

        }
    }

    private void OnTriggerEnter2D(Collider2D other)// Detects collision with other objects
    {
        Debug.Log("HorizontalEnemy HIT" + other.name);
        if (other.CompareTag("PlayerLaser"))// If hit by a laser, take damage
        {
            Destroy(other.gameObject);// Destroy the laser
            Damage();// Apply damage logic
        }
    }

    public void Damage() // Logic for when the enemy is destroyed
    {
        if (_isDead) return;// Skip if already dead

        _isDead = true;

        if (_explosionPrefab != null) // Spawn explosion if assigned
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Explosion Spawned");
        }

        else
        {
            Debug.LogWarning("Explosion Prefab is not assigned");
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();  // Hide the sprite

        if (sr != null) sr.enabled = false;

        _horizontalSpeed = 0;// Stop movement

        if (_audioSource != null) // Play explosion sound if assigned
        {
            _audioSource.Play();
        }

        if (_player != null) // Add score to player if available
        {
            _player.AddScore(20);
        }
       
        Destroy(this.gameObject, 2.8f); // Destroy this object after a short delay to let effects play
    }


}
