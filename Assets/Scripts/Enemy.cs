using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _verticalspeed = 4.0f;
    [SerializeField] private GameObject _laserPrefab;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("the Player is NULL");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if(Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {

        transform.Translate(Vector3.down * _verticalspeed * Time.deltaTime);

        if (transform.position.y < -11.5f)
        {
            float randomX = Random.Range(-16.7f, 13.5f);
            transform.position = new Vector3(randomX, 16.5f, 0);
        }
    }

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

     if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            OnDeath();
        }

     if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if(_player != null)
            {
                _player.AddScore(10);
            }
            OnDeath();
        }

    }

    void OnDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _verticalspeed = 1.5f;
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.8f);
    }

}
