
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerLaser"))// Check if the object that entered is a PlayerLaser

        {
            // Try to get the Laser script attached to the laser GameObject
            Laser laser = other.GetComponent<Laser>();

            if (laser != null)
            {
                // Mark the laser as "already hit" so it won't damage anything else
                laser.MarkAsHit();
            }
            Destroy(other.gameObject); // Destroy the laser that collided with the shield

            Destroy(this.gameObject);// Destroy the shield itself after blocking the hit
        }
        
    }
}
