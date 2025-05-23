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

            BossFirstAndSecondPhase _bossPhaseController = helmet.GetComponentInParent<BossFirstAndSecondPhase>();
            if (_bossPhaseController != null)
            {
                _bossPhaseController.ContinueAfterOrb();
            }
            else
            {
                Debug.LogWarning("BossPhaseController not found from IceBlast.");
            }

            Destroy(gameObject); // Destroy IceBlast after hit
        }
    }

    

}
