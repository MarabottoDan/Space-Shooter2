
using UnityEngine;

public class BossHandRoundProjectile : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -12)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x >= 23)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x <= -23)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
