using UnityEngine;

public class HomingMissile : MonoBehaviour
{

    [SerializeField] private float _speed = 5f; // Missile movement speed
    [SerializeField] private float _fallbackSpeed = 3f;          // Speed if no target
    [SerializeField] private float _rotateSpeed = 200f; // How fast it turns toward the target
    [SerializeField] private float _detectionRadius = 10f;// How far it can "see"
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _explosionClip;

    private Transform _target;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        FindClosestTarget();
        _audioSource = Camera.main.GetComponent<AudioSource>(); // Play sound from main camera
        Destroy(this.gameObject, 5f);// Failsafe: self-destruct after 5 seconds
    }

    // Update is called once per frame
    void Update()
    {
   
       if (_target == null)
       {
          FindNewTarget(); // Try to find a new one
          if (_target == null)
          {
                transform.Translate(Vector3.up * _fallbackSpeed * Time.deltaTime);
            return; 
          }
       }

            Vector3 direction = (_target.position - transform.position).normalized;
            Vector3 newDirection = Vector3.RotateTowards(transform.up, direction, _rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, newDirection);

            // Move forward
            transform.Translate(Vector3.up * _speed * Time.deltaTime);


    }

    void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= _detectionRadius)
            {
                shortestDistance = distance;
                closestEnemy = enemy;
            }
        }
         if (closestEnemy != null)
        {
            _target = closestEnemy.transform;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if  (other.CompareTag("Player"))
        {
            return;
        }
        Debug.Log("Homing missile hit: " + other.tag);

        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("BossHelmet"))
        {
            BossHelmet helmet = other.GetComponent<BossHelmet>();
            if (helmet !=null)
            {
                helmet.TakeDamage(5);
            }
            Debug.Log("Missile collided with: " + other.name + " (Tag: " + other.tag + ")");

        }

        if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }
            if (_explosionClip != null)
            {
                AudioSource camAudio = Camera.main.GetComponent<AudioSource>();
                camAudio.PlayOneShot(_explosionClip, 2.0f); // Adjust volume boost as needed
            }

            Destroy(this.gameObject,0.1f);
        
    }
    private void FindNewTarget()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("BossHelmet");
        if (boss != null)
        {
            float bossDistance = Vector3.Distance(transform.position, boss.transform.position);
            if (bossDistance <= _detectionRadius)
            {
                _target = boss.transform;
                Debug.Log("MISSILE: Locked onto BossHelmet manually.");
                return;
            }
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        _target = nearestEnemy;
    }

}
