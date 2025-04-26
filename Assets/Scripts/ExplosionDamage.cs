using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] private float explosionDuration = 0.5f;

    void Start()
    {
        Destroy(gameObject, explosionDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(); // ✅ Calls your built-in damage logic
            }
        }
    }
}
