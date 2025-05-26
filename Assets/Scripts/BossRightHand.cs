using UnityEngine;

public class BossRightHand : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _projectileLifetime = 3f;

    [Header("Attack Settings")]
    [SerializeField] private int _minProjectiles = 4;
    [SerializeField] private int _maxProjectiles = 8;

    private readonly float[] _clockAngles = { 180f, 210f, 240f, 270f, 300f, 330f, 360f };


    public void FireZigZagProjectiles()
    {
        int count = Random.Range(_minProjectiles, _maxProjectiles + 1);

        for (int i = 0; i < count; i++)
        {
            float angle = _clockAngles[Random.Range(0, _clockAngles.Length)];
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);


            GameObject proj = Instantiate(_projectilePrefab, _spawnPoint.position, Quaternion.identity);
            ZigZagProjectile zz = proj.GetComponent<ZigZagProjectile>();
            if (zz != null)
            {
                zz.SetDirection(direction);
                zz.SetLifetime(_projectileLifetime);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireZigZagProjectiles();
        }
    }
}
