using UnityEngine;

public class WhiteEnemy : MonoBehaviour
{
    [SerializeField] private Transform mineSpawnPoint;// Where the mines will spawn from
    [SerializeField] private GameObject minePrefab; // Prefab of the mine to spawn
    [SerializeField] private float mineDropInterval = 0.6f; // Time between each mine drop
    [SerializeField] private float moveSpeed = 5f;// Movement speed of the enemy

    [SerializeField] private Vector3 _topLeft = new Vector3(-8f, 5f, 0f); // Top-left starting position
    [SerializeField] private Vector3 _topRight = new Vector3(8f, 5f, 0f); // Top-right starting position

    // Direction to move if spawned from the left
    [SerializeField] private Vector3 _fromLeftDirection = new Vector3(1f, -1f, 0f);

    // Direction to move if spawned from the right
    [SerializeField] private Vector3 _fromRightDirection = new Vector3(-1f, -1f, 0f);


    private Vector3 _moveDirection;
    private float _nextMineDropTime;
    private bool _canDropMines = false;

    void Start()
    {        // Randomly decide if the enemy spawns from left or right
        bool fromLeft = Random.value > 0.5f;

        if (fromLeft)
        {
            transform.position = _topLeft;
            _moveDirection = _fromLeftDirection.normalized;
            transform.rotation = Quaternion.Euler(0, 0, 40.568f);// Tilt facing the center
        }
        else
        {
            transform.position = _topRight;
            _moveDirection = _fromRightDirection.normalized;
            transform.rotation = Quaternion.Euler(0, 0, -40.568f);// Tilt facing the center
        }
    }

    void Update()
    {
        // Move enemy along its chosen direction
        transform.position += _moveDirection * moveSpeed * Time.deltaTime;

        // Enable mine dropping once below y = 0
        if (!_canDropMines && transform.position.y <= 0f)
        {
            _canDropMines = true;
            _nextMineDropTime = Time.time; // Start dropping immediately after passing y = 0
        }

        // Drop mines while enemy is still above y = -9 (still inside Bounds)
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
