using UnityEngine;

public class TimedMine : MonoBehaviour
{
    [SerializeField] private float minTime = 2f;// Minimum time before the mine explodes
    [SerializeField] private float maxTime = 5f; // Maximum time before the mine explodes
    [SerializeField] private GameObject explosionEffect; // Explosion visual effect prefab


    private void Start()
    {
        float delay = Random.Range(minTime, maxTime);  // Pick a random delay between minTime and maxTime
        Invoke(nameof(Explode), delay); // Call the Explode method after the delay
    }

    private void Explode()
    {   // If there is an explosion effect, spawn it at the mine's position
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject); // Destroy the mine object after exploding
    }
}
