using System.Collections;
using UnityEngine;

public class BossLeftHand : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Transform _laserSpawnPoint;
    [SerializeField] private float _laserSpeed = 5f;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private float _startDelay = 5f; // Delay before firing starts

    private float _nextFireTime = 0f;
    private bool _canFire = false;

    private Transform _playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        StartCoroutine(StartFiringWithDelay());
    }

    IEnumerator StartFiringWithDelay()
    {
        yield return new WaitForSeconds(_startDelay);
        _canFire = true;
        _nextFireTime = Time.time + _fireRate;
    }

    void Update()
    {
        if (_canFire && Time.time >= _nextFireTime && _playerTransform != null)
        {
            FireLaser();
            _nextFireTime = Time.time + _fireRate;
        }

        
    }

    private void FireLaser()
    {
        if (_laserPrefab != null && _laserSpawnPoint != null && _playerTransform != null)
        {
            GameObject laser = Instantiate(_laserPrefab, _laserSpawnPoint.position, Quaternion.identity);

            Vector2 direction = (_playerTransform.position - _laserSpawnPoint.position).normalized;

            Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = direction * _laserSpeed;     
            }
            

        }
    }
    public void StopFiring()
    {
        Debug.Log("BossLeftHand: Stopping firing.");
        _canFire = false;
    }

}
