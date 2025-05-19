using System.Collections;
using UnityEngine;

public class BossOrbPhaseOne : MonoBehaviour
{
    private BossHelmet _bossHelmet;

    [Header("References")]
    [SerializeField] private GameObject _pickupEffectPrefab;

    [Header("Movement Settings")]
    [SerializeField] private Vector3 _targetPosition = new Vector3(3f, -7f, 0f);
    [SerializeField] private float _moveSpeed = 2f;

    [Header("Pulse Animation")]
    [SerializeField] private float _pulseSpeed = 2f;
    [SerializeField] private float _pulseScaleAmount = 0.2f;

    private Vector3 _baseScale;
    private bool _reachedTarget = false;
    private Transform _player;

    public void InitializeHelmet(BossHelmet helmet)
    {
        _bossHelmet = helmet;
    }

    private void Start()
    {
        

        Debug.Log("Orb spawned at: " + transform.position);
        _baseScale = transform.localScale;
        _player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (!_reachedTarget)
        {
            // Move toward the target position
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

            // Pulse while moving
            float pulse = Mathf.Sin(Time.time * _pulseSpeed) * _pulseScaleAmount;
            transform.localScale = _baseScale + Vector3.one * pulse;

            // Stop when close enough
            if (Vector3.Distance(transform.position, _targetPosition) <= 0.01f)
            {
                _reachedTarget = true;
                transform.position = _targetPosition;
                transform.localScale = _baseScale; // Reset scale
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_reachedTarget && other.CompareTag("Player"))
        {
            Debug.Log("Orb collected!");

            if (_pickupEffectPrefab != null)
            {
                Instantiate(_pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            if (_bossHelmet != null)
            {
                _bossHelmet.OnOrbSequenceComplete();
            }

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.DisableInputAndFireIce();
            }

            Destroy(gameObject);
        }
    }
}
