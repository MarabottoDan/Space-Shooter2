using UnityEngine;

public class Laser : MonoBehaviour
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
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

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
        Debug.Log("Laser hit: " + other.name + ", Tag: " + other.tag);

        //Debug.Log("Laser hit: " + other.name);
        if (_hasHit) return;

        // Laser hits the player
        if (other.CompareTag("Player") && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            _hasHit = true;
            Destroy(this.gameObject);
        }

        // Laser hits an enemy
        else if (other.CompareTag("Enemy") && !_isEnemyLaser)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDeath();
            }
            else
            {
                SmartEnemy smartEnemy = other.GetComponent<SmartEnemy>();
                if (smartEnemy != null)
                {
                    smartEnemy.Damage();
                }
            }

            _hasHit = true;
            Destroy(this.gameObject);
        }

        // Laser hits a pickup (only if it's an enemy laser)
        else if (other.CompareTag("Pickup") && _isEnemyLaser)
        {
            float distanceToPickup = Vector3.Distance(_startPosition, other.transform.position);

            if (distanceToPickup <= _pickupHitRange)
            {
                Destroy(other.gameObject);
            }

            _hasHit = true;
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("BossHelmet") && !_isEnemyLaser)
        {
            BossHelmet helmet = other.GetComponent<BossHelmet>();
            if (helmet != null)
            {
                helmet.TakeDamage(1); // or however much damage your laser does
            }
            Debug.Log("Laser collided with: " + other.name + " | Tag: " + other.tag);
            _hasHit = true;
            Destroy(this.gameObject); // Destroy the laser after hitting the helmet
            
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