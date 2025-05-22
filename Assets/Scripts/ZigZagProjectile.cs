using UnityEngine;

public class ZigZagProjectile : MonoBehaviour
{

    private Vector3 _moveDirection = Vector3.zero;
    private float _lifetime = 3f;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _zigzagAmplitude = 0.03f;
    [SerializeField] private float _zigzagFrequency = 20f;

    private Vector3 _perpendicular;

    private float _timeAlive = 0f;

    public void SetDirection(Vector3 dir)
    {
        _moveDirection = dir.normalized;

        //Get Perpendicular direction for ZigZag
        _perpendicular = new Vector3(-_moveDirection.y, _moveDirection.x, 0f);
    }

    public void SetLifetime(float time)
    {
        _lifetime = time;
    }
    

    // Update is called once per frame
    void Update()
    {
        _timeAlive += Time.deltaTime;

        //zigzag offset using sine wave
        float offset = Mathf.Sin(_timeAlive * _zigzagFrequency) * _zigzagAmplitude;

        //Final position = forward movement + zigzag
        Vector3 moveStep = _moveDirection * _speed * Time.deltaTime;
        Vector3 zigzagStep = _perpendicular * offset;
        transform.position += moveStep + zigzagStep;

        if (_timeAlive >= _lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            Destroy(gameObject);
        }
    }
}
