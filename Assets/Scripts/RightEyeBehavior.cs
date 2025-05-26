using System.Collections;
using UnityEngine;

public class RightEyeBehavior : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private SpriteRenderer _eyeSprite;
    [SerializeField] private Color _flashColor = Color.red;

    [SerializeField] private GameObject _leftEye;



    private Color _originalColor;
    private bool _isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }

        if  (_eyeSprite != null)
        {
            _originalColor = Color.white; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null) return;


        Vector3 direction = _player.position - transform.position;

        // Get angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void StartFlashLoop()
    {
        if(!_isFlashing)
        {
            Debug.Log("StartFlashLoop() triggered.");
            StartCoroutine(FlashLoopRoutine());
            _isFlashing = true;
        }
    }

    private IEnumerator FlashLoopRoutine()
    {
        yield return new WaitForSeconds(10f);

        while (true)
        {
            // Phase 1: Flash 2x per second (0.25s) for 2 seconds
            yield return StartCoroutine(FlashPattern(0.25f, 2f));

            // Phase 2: Flash 3x per second (~0.17s) for 2 seconds
            yield return StartCoroutine(FlashPattern(0.17f, 2f));

            if (_leftEye != null)
            {
                LeftEyeBehavior leftEye = _leftEye.GetComponent<LeftEyeBehavior>();
                if (leftEye != null)
                {
                    leftEye.StartBlackHoleAttack();
                }
                else
                {
                    Debug.LogWarning("LeftEyeBehavior not found on _leftEye.");
                }
            }


            yield return new WaitForSeconds(20f);
        }
    }

    private IEnumerator FlashPattern(float interval, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            _eyeSprite.color = _flashColor;
            yield return new WaitForSeconds(interval / 2f);
            _eyeSprite.color = _originalColor;
            yield return new WaitForSeconds(interval / 2f);

            elapsed += interval;

        }
    }
}
