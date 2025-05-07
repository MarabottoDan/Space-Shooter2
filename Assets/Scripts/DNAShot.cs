
using UnityEngine;

public class DNAShot : MonoBehaviour
{

    [Header("Settings")]
    //[SerializeField] private float _speed = 5f;// Speed of DNA shot
    [SerializeField] private float _lifetime = 5f;// How long the DNA shot exists if no collision
    

    [SerializeField] private float _rotationSpeed = 90f; // Degrees per second

    private bool _hasBounced = false; // Tracks if the DNA shot has already bounced

    private Rigidbody2D _rb;

    [Header("Screen Bounds")]
    [SerializeField] private float _leftBound = -17f;
    [SerializeField] private float _rightBound = 17f;
    [SerializeField] private float _topBound = 9f;
    [SerializeField] private float _bottomBound = -9f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();// Cache Rigidbody2D for velocity control

        Destroy(this.gameObject, _lifetime);// Auto-destroy DNA shot after its lifetime expires
    }

    // Update is called once per frame
    void Update()
    {
        CheckBounds();
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);// Spin the DNA shot as it flies (ex. twisting projectile effect)

    }

    private void CheckBounds()
    {
        Vector3 pos = transform.position;

        if(!_hasBounced)
        {    // If DNA shot hits left or right edges
            if (pos.x < _leftBound || pos.x > _rightBound)
            {
                BounceHorizontal();// Reverse X direction
            }
            // If DNA shot hits top or bottom edges
            else if (pos.y < _bottomBound || pos.y > _topBound)
            {
                BounceVertical();// Reverse Y direction
            }
        }
        else
        {
            // Already bounced once, now destroy if fully off screen
            if (pos.x < _leftBound - 2f || pos.x > _rightBound  + 2f || pos.y <_bottomBound - 2f || pos.y > _topBound + 2f)
            {
                Destroy(this.gameObject);
            }

        }
    }

    private void BounceHorizontal()
    {
        if ( _rb != null)
        {
            _rb.velocity = new Vector2(-_rb.velocity.x, _rb.velocity.y);// Reverse X direction
            //Only flip(reverse) the left-right movement, but don't touch the up/down movement
            //-_rb.velocity.x ➔ Flip the X value(left<-> right).
         // _rb.velocity.y ➔ Leave Y value unchanged(still go up or down as before).
            _hasBounced = true;// Mark as bounced
        }
    }

    private void BounceVertical()
    {
        if (_rb != null)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, -_rb.velocity.y);/// Reverse only Y velocity
            _hasBounced = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply DNA effect to player
            float roll = Random.Range(0f, 1f);// Random roll between 0 and 1

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (roll <= 0.3f)
                {
                    // 30% chance to lose life
                    player.LoseLife();
                }
                else
                {
                    // 70% chance to give random powerup
                    player.ApplyRandomPowerup();
                }
            }

            Destroy(this.gameObject); // Destroy DNA shot after hitting player
        }
        else if (other.CompareTag("Enemy"))
        {
            // Apply DNA effect to enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float roll = Random.Range(0f, 1f);

                if (roll <= 0.3f)
                {
                    // 30% chance to kill the enemy
                    enemy.OnDeath(); //Instead of Destroy(other.gameObject) — triggers explosion
                }
                else
                {
                    // 70% chance to mutate enemy (if not already mutated)
                    if (!enemy.IsMutated()) //New protection against double mutation
                    {
                        enemy.ApplyRandomMutation();
                        enemy.MarkAsMutated(); //Mark the enemy so it can't mutate again
                    }
                }
            }

            Destroy(this.gameObject); // Destroy DNA shot after hitting enemy
        }
    }
}
