using UnityEngine;

public class DodgeLaser : MonoBehaviour
{

    [SerializeField]
    private float _speed = 8.0f;
    private bool _isEnemyLaser = false;
    private bool _hasHit = false; // Prevents double hit
    [SerializeField] private float _pickupHitRange = 2f;
    private Vector3 _startPosition;
    // Stores the position where the laser was originally spawned
    // This is used to check if the laser was fired close enough to a pickup

    private void Start()
    {
        // Record the initial position of the laser when it spawns
        // This will later be used to calculate the distance to a pickup
        _startPosition = transform.position;


    }



    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y > 17f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -12f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            _hasHit = true;
            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Enemy") && _isEnemyLaser == false)
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.OnDeath();
            }

            _hasHit = true;
            Destroy(this.gameObject);
        }
        // If the laser collides with a Pickup AND it's an enemy laser
        else if (other.CompareTag("Pickup") && _isEnemyLaser)
        {   // Calculate the distance between where the laser was spawned and the pickup's position
            // This ensures that only lasers fired from close range can destroy the pickup
            float distanceToPickup = Vector3.Distance(_startPosition, other.transform.position);

            // If the laser was spawned close enough to the pickup, destroy the pickup
            if (distanceToPickup <= _pickupHitRange)
            {
                Destroy(other.gameObject); // Destroy the pickup GameObject
            }
            // Mark this laser as having already hit something, so it won't trigger again
            _hasHit = true;
            Destroy(this.gameObject); // Destroy the laser itself, whether or not it destroyed the pickup
        }


    }


    public void MarkAsHit()
    {
        _hasHit = true;
    }

    public bool HasHit()
    {
        return _hasHit;
    }

}