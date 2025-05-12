using UnityEngine;

public class IceBlast : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;

    private void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BossHelmet"))
        {
            BossHelmet helmet = other.GetComponent<BossHelmet>();
            if (helmet != null)
            {
                helmet.ApplyIceBlast();
            }

            Destroy(gameObject); // Destroy IceBlast after hit
        }
    }
}
