using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _verticalspeed = 4.0f;
    [SerializeField] private GameObject _laserEnemyPrefab;
    [SerializeField] private GameObject _enemyShield;
    private GameObject _laserEnemyContainer;
    [SerializeField] private float _aggressionDistance = 7.0f;// Aggression distance
    [SerializeField] private float _aggressionSpeed = 5.5f; // Aggression speed
    [SerializeField] private float _aggressionDuration = 2f;// How long to chase
    private float _aggressionTimer; // InternalTimer

    



    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private bool _isDead = false;
    private bool _isAggressive;
    
    // Start is called before the first frame update
    void Start()
    {
        _aggressionTimer = _aggressionDuration;
        _isAggressive = Random.value < 0.4f;// 40% chance to become aggresive
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _laserEnemyContainer = GameObject.Find("LaserEnemyContainer");

        //Find the shield child automatically
        _enemyShield = transform.Find("EnemyShield")?.gameObject;

        if (_enemyShield != null)
        {
            if (Random.value < 0.5f) // 50% chance to spawn with shield
            {
                _enemyShield.SetActive(true);
            }
            else
            {
                _enemyShield.SetActive(false);
            }
        }

       

        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL!");
        }

        if (_laserEnemyContainer == null)
        {
            Debug.LogWarning("LaserEnemyContainer not found in the Scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        CalculateMovement();

        if(Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        if (_player != null)
        {
            if (_isAggressive)
            {
                // Calculate distance between enemy and player
                float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

                // Check if player is within aggression distance
                if (distanceToPlayer <= _aggressionDistance && _aggressionTimer > 0)
                {
                    //Aggressive behaviour: Move directly towards the player
                    Vector3 directiontoPlayer = (_player.transform.position - transform.position).normalized;
                    transform.Translate(directiontoPlayer * _aggressionSpeed * Time.deltaTime);

                    _aggressionTimer -= Time.deltaTime;// Countdown the aggression timer
                    return;
                }
            }
                // Normal downward movement
                transform.Translate(Vector3.down * _verticalspeed * Time.deltaTime);
              
        }  

        if (transform.position.y < -11.5f)
        {
            float randomX = Random.Range(-16.7f, 13.5f);
            transform.position = new Vector3(randomX, 16.5f, 0);
        }
    }

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserEnemyPrefab, transform.position, Quaternion.identity);

        if (_laserEnemyContainer == null)
        {
            _laserEnemyContainer = GameObject.Find("LaserEnemyContainer");
        }

        if (_laserEnemyContainer != null)
        {
            enemyLaser.transform.parent = _laserEnemyContainer.transform;
        }
       
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            if (_enemyShield != null && _enemyShield.activeSelf == true)
            {
                Destroy(_enemyShield);
                return;// Shield absorbs the first hit, enemy survives
            }

           
            OnDeath();
        }
        else if (other.CompareTag("PlayerLaser"))
        {
            Laser laser = other.GetComponent<Laser>();

            if (laser != null && laser.HasHit() == false) // 🔵 Only allow fresh lasers
            {
                Destroy(other.gameObject);

                if (_player != null)
                {
                    _player.AddScore(10);
                }
                OnDeath();
            }
        }
    }


    public void OnDeath()
    {
        _isDead = true;

        // Play enemy death animation
        _anim.SetTrigger("OnEnemyDeath");

        // Slow down movement during death animation
        _verticalspeed = 1.5f;

        // Play enemy death sound
        _audioSource.Play();

        // Destroy the shield if it still exists
        if (_enemyShield != null)
        {
            Destroy(_enemyShield);
        }

        // Remove the enemy's collider so it can't interact anymore
        Destroy(GetComponent<Collider2D>());

        // Destroy the enemy GameObject after a short delay (for animation and sound to finish)
        Destroy(this.gameObject, 2.8f);
    }

    public void ApplyRandomMutation()
    {
        int random = Random.Range(0, 3);// picks a random buff

        switch (random)
        {
            case 0:
                // Increase movement speed
                _verticalspeed *= 1.5f;
                Debug.Log("Mutation: Speed Boost!");
                StartCoroutine(ResetMutation());
                break;

            case 1:
                // Fire lasers faster
                _fireRate *= 0.5f; // Halve the fire rate to shoot more often
                Debug.Log("Mutation: Rapid Fire!");
                StartCoroutine(ResetMutation());
                break;

            case 2:
                //Get temporary shield
                if (_enemyShield != null)
                {
                    _enemyShield.SetActive(true);
                    Debug.Log("Mutation: Temporary Shield activated!");
                    StartCoroutine(ResetMutation());
                }
                break;
        }
    }

    private IEnumerator ResetMutation()
    {
        yield return new WaitForSeconds(5f); // Mutation lasts for 5 seconds

        //Reset speed and fire rate back to normal
        _verticalspeed = 4.0f;
        _fireRate = 3.0f;
        if (_enemyShield != null)
        {
            // Optionally disable the shield if it was enabled by mutation
            _enemyShield.SetActive(false);
        }
    }

    private bool _isMutated = false;

    public bool IsMutated()
    {
        return _isMutated;
    }

    public void MarkAsMutated()
    {
        _isMutated = true;
    }


}
