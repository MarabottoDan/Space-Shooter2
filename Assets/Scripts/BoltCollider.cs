using UnityEngine;

public class BoltCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDeath(); // Call the proper method
            }

            Destroy(gameObject); // Destroy the bolt
        }
    }
}
