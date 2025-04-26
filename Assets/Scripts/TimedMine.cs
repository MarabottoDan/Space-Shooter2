using UnityEngine;

public class TimedMine : MonoBehaviour
{
    [SerializeField] private float minTime = 2f;
    [SerializeField] private float maxTime = 5f;
    [SerializeField] private GameObject explosionEffect;

    private void Start()
    {
        float delay = Random.Range(minTime, maxTime);
        Invoke(nameof(Explode), delay);
    }

    private void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
