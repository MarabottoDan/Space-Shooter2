using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _lifetime = 5f;
    public static int TotalSawsRemaining = 0;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] public GameObject _icedHelmet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        transform.Rotate(0, 360, 0 * Time.deltaTime); // Spin
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BossHelmetIced"))
        {
            SawProjectile.TotalSawsRemaining--;

            if (SawProjectile.TotalSawsRemaining == 0) // This is the last saw!
            {
                if (_explosionPrefab != null)
                {
                    Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                }

                if (_icedHelmet != null)
                {
                    _icedHelmet.SetActive(false); // Disable IcedHelmet
                    Debug.Log("BossHelmetIced DISABLED after final explosion.");
                }
            }

            Destroy(gameObject); // Always destroy the saw
        }
    }

    

    public void SetIcedHelmet(GameObject helmet)
    {
        _icedHelmet = helmet;
    }




}
