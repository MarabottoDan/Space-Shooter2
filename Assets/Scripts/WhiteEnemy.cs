using UnityEngine;

public class WhiteEnemy : MonoBehaviour
{
    [SerializeField] private Transform mineSpawnPoint;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private float mineDropInterval = 0.6f;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Vector3 _topLeft = new Vector3(-8f, 5f, 0f);
    [SerializeField] private Vector3 _topRight = new Vector3(8f, 5f, 0f);

    [SerializeField] private Vector3 _fromLeftDirection = new Vector3(1f, -1f, 0f);
    [SerializeField] private Vector3 _fromRightDirection = new Vector3(-1f, -1f, 0f);


    private Vector3 _moveDirection;
    private float _nextMineDropTime;
    private bool _canDropMines = false;

    void Start()
    {
        bool fromLeft = Random.value > 0.5f;

        if (fromLeft)
        {
            transform.position = _topLeft;
            _moveDirection = _fromLeftDirection.normalized;
            transform.rotation = Quaternion.Euler(0, 0, 40.568f);
        }
        else
        {
            transform.position = _topRight;
            _moveDirection = _fromRightDirection.normalized;
            transform.rotation = Quaternion.Euler(0, 0, -40.568f);
        }
    }

    void Update()
    {
        transform.position += _moveDirection * moveSpeed * Time.deltaTime;

        // Enable mine dropping once below y = 0
        if (!_canDropMines && transform.position.y <= 0f)
        {
            _canDropMines = true;
            _nextMineDropTime = Time.time; // Start dropping immediately after passing y = 0
        }

        if (_canDropMines && transform.position.y >= -9f)
        {
            if (Time.time >= _nextMineDropTime)
            {
                Instantiate(minePrefab, mineSpawnPoint.position, Quaternion.identity);
                _nextMineDropTime = Time.time + mineDropInterval;
            }
        }

        // Destroy the enemy at y = -12
        if (transform.position.y <= -12f)
        {
            Destroy(gameObject);
        }
    }
}
