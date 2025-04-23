using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotatespeed = 20.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    [SerializeField] private AudioClip _asteroidDestroyedSound;
    [SerializeField] private AudioSource _audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {  
        transform.Rotate(Vector3.forward * _rotatespeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser") 
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);      
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Debug.Log("Asteroid, destroyed!");
            Destroy(this.gameObject, 0.25f);
        }
    }


}
