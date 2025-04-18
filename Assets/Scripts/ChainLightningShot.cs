using System.Collections;
using UnityEngine;

public class ChainLightningShot : MonoBehaviour
{
    [SerializeField] private GameObject[] _boltPrefabs; //Drag your Bolt1Prefab, Bolt2Prefab, and Bolt3Prefab into this array in the Inspector
    [SerializeField] private float _boltLifeTime = 2f; // How long each lightning bolt lasts before disappearing  
    [SerializeField] private float _speed = 5f; // Speed at which the ChainLightningShot travels before it stops and explodes  
    [SerializeField] private GameObject _boltGlowPrefab;//Drag your BoltGlow prefab here(the glow effect that appears at the center of the explosion)
    [SerializeField] private AudioClip _thunderClip;//Drag your thunder sound effect here
    private AudioSource _audioSource;// Will be fetched at runtime
    private Rigidbody2D _rb;// Used to move the shot forward before exploding

    // Called when the shot is instantiated
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();    // Get the Rigidbody2D on this object    
        _audioSource = GetComponent<AudioSource>();// Get the AudioSource component
        Invoke(nameof(StartLightningStorm), 1f);// Start the storm 1 second after firing
        Invoke(nameof(PlayThunderSound), .2f); // Play a thunder sound shortly after firing
    }

    void PlayThunderSound()
    {
        _audioSource.PlayOneShot(_thunderClip);// Play the thunder audio once
    }
    // Update is called once per frame
    void Update()
    {
        if(_rb != null)
        {
            // Move the bolt upward while it's still active (before it stops and explodes)
            _rb.MovePosition(_rb.position + Vector2.up * _speed * Time.deltaTime);
        }      
    }

    void StartLightningStorm()
    {
        _rb.velocity = Vector2.zero;// Stop movement
        _rb = null;// Prevent further movement logic
        if (_audioSource != null && _thunderClip != null)
        {
            _audioSource.PlayOneShot(_thunderClip);// Play explosion thunder sound
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or ThunderClip on ChainLightningShot");
        }


        StartCoroutine(LightningStormSequence());// Begin the bolt spawning effect
    }

   IEnumerator LightningStormSequence()
    {
        yield return StartCoroutine(SpawnBoltsOverTime(2f, 2));// Spawn first wave: 2 bolts/sec for 2 seconds = 4 bolts
        yield return StartCoroutine(SpawnBoltsOverTime(2f, 3));// Spawn second wave: 3 bolts/sec for 2 seconds = 6 bolts

        Destroy(gameObject);// Destroy the main ChainLightningShot after the effect ends
    }

    IEnumerator SpawnBoltsOverTime(float duration, int boltsPreSecond)
    // Coroutine that spawns a set number of lightning bolts per second over a given duration.
    // 'duration' is how long the wave lasts.
    // 'boltsPerSecond' determines how frequently bolts are spawned within that time.
    {
        float interval = 1f / boltsPreSecond;// Time between each bolt spawn
        float timer = 0f;// Keeps track of how much time has passed since the bolt wave started

        while (timer < duration)
        {
            SpawnRandomBolt();// Spawn one bolt
            timer += interval;// Increment the timer by the interval after each bolt is spawned
            //This ensures the loop progresses toward the total duration correctly, spacing each bolt out evenly.
            //Let me know if you want similar comments for the rest of the method!
            yield return new WaitForSeconds(interval); // Wait before spawning the next
        }
    }

    void SpawnRandomBolt()
    {
        // Instantiate a glow effect at the center of the explosion
        Instantiate(_boltGlowPrefab, transform.position, Quaternion.identity);

        int randomIndex = Random.Range(0, _boltPrefabs.Length);// Pick a random bolt

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
