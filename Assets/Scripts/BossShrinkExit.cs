using System.Collections;
using UnityEngine;

public class BossShrinkExit : MonoBehaviour
{
    [SerializeField] private float _shrinkDuration = 10f;
    [SerializeField] private Vector3 _targetScale = default;
    [SerializeField] private bool _destroyAfterShrink = true;

    private Vector3 _startScale;

    private void Awake()
    {
        _startScale = transform.localScale;

        if (_targetScale == default)
        {
            _targetScale = Vector3.one * 0.01f;
        }
    }

    public void StartShrinkExit()
    {
        StartCoroutine(SmoothRoutine());
    }

    private IEnumerator SmoothRoutine()
    {
        Debug.Log("BossShinkExit: Starting Boss shrink");

        float elapsed = 0f;

        while (elapsed < _shrinkDuration)
        {
            float progress = elapsed / _shrinkDuration;
            transform.localScale = Vector3.Lerp(_startScale, _targetScale, progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _targetScale;
        Debug.Log(" BossShrinkExit: Shrinking complete");

        if (_destroyAfterShrink)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
