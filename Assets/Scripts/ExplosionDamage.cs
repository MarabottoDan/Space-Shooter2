using System.Collections; 
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    // Total time before the explosion object is destroyed
    [SerializeField] private float explosionDuration = 0.5f;

    private CircleCollider2D _collider;

    private void Start()
    {
        // Get the CircleCollider2D attached to this object
        _collider = GetComponent<CircleCollider2D>();

        // Start coroutine to disable the collider early (so it only damages once)
        StartCoroutine(DisableColliderEarly());

        // Destroy the explosion GameObject after the full explosion duration
        Destroy(gameObject, explosionDuration);
    }

    private IEnumerator DisableColliderEarly()
    {
        yield return new WaitForSeconds(0.2f); // Wait a short time to allow the player to be damaged
        if (_collider != null)
        {
            _collider.enabled = false;// Disable collider to prevent multiple hits
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))// Check if the object entering the collider is the Player
        {
            Player player = collision.GetComponent<Player>(); // Try to get the Player script from the collided object
            if (player != null)
            {
                player.Damage();// Call the Damage method on the Player
            }
        }
    }
}
