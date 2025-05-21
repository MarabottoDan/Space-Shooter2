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

    void Start()
    {
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
        if (_canFire && Time.time >= _nextFireTime)
        {
            FireLaser();
            _nextFireTime = Time.time + _fireRate;
        }
    }

    private void FireLaser()
    {
        if (_laserPrefab != null && _laserSpawnPoint != null)
        {
            GameObject laser = Instantiate(_laserPrefab, _laserSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.down * _laserSpeed;
            }
        }
    }
}
