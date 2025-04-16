using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningShot : MonoBehaviour
{
    [SerializeField] private GameObject[] _boltPrefabs;
    
    [SerializeField] private float _boltLifeTime = 2f;   
    [SerializeField] private float _speed = 5f; // Adjust the speed as needed
   
    [SerializeField] private GameObject _boltGlowPrefab;
    [SerializeField] private AudioClip _thunderClip;
    private AudioSource _audioSource;


    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();       
        _audioSource = GetComponent<AudioSource>();
        Invoke(nameof(StartLightningStorm), 1f);
        Invoke(nameof(PlayThunderSound), .2f);
        


    }

    void PlayThunderSound()
    {
        _audioSource.PlayOneShot(_thunderClip);
    }
    // Update is called once per frame
    void Update()
    {
        if(_rb != null)
        {
            _rb.MovePosition(_rb.position + Vector2.up * _speed * Time.deltaTime);
        }
        
    }

    void StartLightningStorm()
    {
        _rb.velocity = Vector2.zero;
        _rb = null;
        if (_audioSource != null && _thunderClip != null)
        {
            _audioSource.PlayOneShot(_thunderClip);
        }
        else
        {
            Debug.LogWarning("⚠️Missing AudioSource or ThunderClip on ChainLightningShot");
        }


        StartCoroutine(LightningStormSequence());
    }

   IEnumerator LightningStormSequence()
    {
        yield return StartCoroutine(SpawnBoltsOverTime(2f, 2));

        yield return StartCoroutine(SpawnBoltsOverTime(2f, 3));

        Destroy(gameObject);
    }
    IEnumerator SpawnBoltsOverTime(float duration, int boltsPreSecond)
    {
        float interval = 1f / boltsPreSecond;
        float timer = 0f;

        while (timer < duration)
        {
            SpawnRandomBolt();
            timer += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnRandomBolt()
    {
        Instantiate(_boltGlowPrefab, transform.position, Quaternion.identity);

        int randomIndex = Random.Range(0, _boltPrefabs.Length);

        // Angle between 90 (left) and -90 (right) in degrees — this covers ONLY the upper arc
        float angle = Random.Range(-90f, 90f);

        // Apply angle directly to rotation
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        // Instantiate bolt with rotation (no velocity)
        GameObject bolt = Instantiate(_boltPrefabs[randomIndex], transform.position, rotation);

        // Destroy bolt after 2 seconds (or your _boltLifetime value)
        Destroy(bolt, _boltLifeTime);
    }



}
