
using UnityEngine;

public class Laser2 : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -11)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                Debug.Log("Damaging Player with Laser2");
                player.Damage();
            }
            Destroy(this.gameObject);
        }
    }


}
