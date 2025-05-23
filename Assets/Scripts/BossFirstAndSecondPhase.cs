using System.Collections;
using UnityEngine;

public class BossFirstAndSecondPhase : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _xMin = -11f;
    [SerializeField] private float _xMax = 8f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _delayBeforeMovement = 10f;
    [SerializeField] private float _timeBetweenMoves = 6f;
    [SerializeField] private Vector3 _phaseTwoStartPosition = new Vector3(0, 3, 0);
    [Header("Phase 2 start")]
    
    [SerializeField] private float _phaseTwoMoveSpeed = 10f;

    [Header("Spit Settings")]
    [SerializeField] private GameObject _spitPrefab;
    [SerializeField] private Transform _spitSpawnPoint;
    [SerializeField] private Vector3 _spitTargetScale = new Vector3(5f, 3f, .3f);
    [SerializeField] private Vector3 _spitTargetPosition = new Vector3(0f, 0f, 0f);
    [SerializeField] private float _spitGrowSpeed = .5f;
    [SerializeField] private float _spitLifeTime = 3f;
    [SerializeField] private float _delayBeforeSpitting = 10f;
    [SerializeField] private AudioClip _spitSound;
    [SerializeField] private float _spitSlideSpeed = 3f;
   
    private AudioSource _cameraAudioSource;
    [SerializeField] private AudioSource _bossAudioSource;




    private bool _isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginMovementRoutine());

        _cameraAudioSource = Camera.main?.GetComponent<AudioSource>();

        if (_cameraAudioSource == null)
        {
            Debug.LogWarning("Main Camera or its AudioSource is missing!");
        }

    }

    private IEnumerator BeginMovementRoutine()
    {
        yield return new WaitForSeconds(_delayBeforeMovement);

        while (true)
        {
            yield return StartCoroutine(MoveToRandomX());
            yield return new WaitForSeconds(_timeBetweenMoves);
        }
    }

    private IEnumerator MoveToRandomX()
    {
        float targetX = Random.Range(_xMin, _xMax);
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        _isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
            yield return null;
        }
        _isMoving = false;
    }

    public void MoveToPhaseTwoStart()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPhaseTwoStartRoutine());
    }

    private IEnumerator MoveToPhaseTwoStartRoutine()
    {
        while (Vector3.Distance(transform.position, _phaseTwoStartPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _phaseTwoStartPosition, _phaseTwoMoveSpeed * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(DelayedSpitAttack());

        StartCoroutine(BeginMovementRoutine());
    }

    private IEnumerator DelayedSpitAttack()
    {
        yield return new WaitForSeconds(_delayBeforeSpitting);
        DoSpitAttack();
    }

    public void DoSpitAttack()
    {
       if (_spitPrefab == null || _spitSpawnPoint == null)
        {
            Debug.LogWarning("Spit prefab not assigned");
            return;
        }

        GameObject spitInstance = Instantiate(_spitPrefab, _spitSpawnPoint.position, Quaternion.identity);

        if (_bossAudioSource != null && _spitSound != null)
        {
            _bossAudioSource.PlayOneShot(_spitSound, 1.5f);
            Debug.Log("Spit sound played via boss AudioSource.");
        }
        else
        {
            Debug.LogWarning("Boss AudioSource or spit sound missing.");
        }


        StartCoroutine(GrowSpit(spitInstance));
    }

    private IEnumerator GrowSpit(GameObject spit)
    {
        Vector3 initialScale = spit.transform.localScale;
        Vector3 initialPosition = spit.transform.position;

        float elapsed = 0f;
        float duration = _spitGrowSpeed;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            spit.transform.localScale = Vector3.Lerp(initialScale, _spitTargetScale, t);
            spit.transform.position = Vector3.Lerp(initialPosition, _spitTargetPosition, t);
            elapsed += Time.deltaTime;
          
            yield return null;
        }

        spit.transform.localScale = _spitTargetScale;
        spit.transform.position = _spitTargetPosition;


        yield return new WaitForSeconds(5f);

        StartCoroutine(SlideSpitDownAndDestroy(spit));
    }

    private IEnumerator SlideSpitDownAndDestroy(GameObject spit)
    {
        if (spit == null)
        {
            Debug.LogWarning("Spit object is null when trying to slide.");
            yield break;
        }

        Debug.Log("Starting spit slide down from Y = " + spit.transform.position.y);

        while (spit != null && spit.transform.position.y > -30f)
        {
            spit.transform.position += Vector3.down * _spitSlideSpeed * Time.deltaTime;
            yield return null;
        }

        Debug.Log("Spit reached below -30 Y. Destroying.");
        Destroy(spit);
    }

}
