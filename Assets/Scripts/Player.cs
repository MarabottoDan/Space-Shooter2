using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour

{
    [SerializeField] private float _speed = 5.6f;
    private float _speedMultiplier = 2;
    [SerializeField] float _shiftSpeed = 8.5f;
    private bool _isSpeedBoostActive = false;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    [SerializeField] private bool _isShieldsActive = false;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _rightEngine, _leftEngine;
    [SerializeField] private int _score;
    private UIManager _uiManager;
    private string[] _hitMessages =
    {
        "I've been hit!",
        "I think my spaceship has a dent now!",
        "Why can't we be friends?",
        "Oh you're going to pay for that!"
     };
    private bool _isGameOver = false;
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _playerDestroyedSound;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the Player is Null");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Calculatemovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void Calculatemovement()

    {
        // write these with the other variables (floats)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 _direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed = _shiftSpeed;
        }
        else if (!_isSpeedBoostActive)
        {
            _speed = 5.6f;
        }

        transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }

        if (transform.position.y <= -9)
        {
            transform.position = new Vector3(transform.position.x, -9, 0);
        }

        if (transform.position.x < -17f)
        {
            transform.position = new Vector3(-17f, transform.position.y, 0);
        }
        if (transform.position.x >= 17f)
        {
            transform.position = new Vector3(17f, transform.position.y, 0);
        }

     
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        }

        _audioSource.Play();

    }

    public void Damage()
    {
        if (_isShieldsActive == true) 
        {
            _isShieldsActive = false;
            _shieldVisualizer.SetActive(false);
            Debug.Log("Shields down! Panic mode engaged!");
            return;
        }
        _lives -= 1;
        //_lives = _lives -1;
        // _lives--;

        if(_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        int randomIndex = Random.Range(0, _hitMessages.Length);
        Debug.Log(_hitMessages[randomIndex]);
        _uiManager.UpdateLives(_lives);

        

        if (_lives < 1)
        {          
            _spawnManager.OnPlayerDeath();
            _audioSource.Play();
            Destroy(this.gameObject);
            Debug.Log("I'm space dust now :(");
            

        }

    }
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
        
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        if (!_isSpeedBoostActive)
        {
            _isSpeedBoostActive = true;
            _speed *= _speedMultiplier;
            StartCoroutine(SpeedBoostPowerDownRoutine());
        }
    }

     IEnumerator SpeedBoostPowerDownRoutine()
    {
      yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;

    Debug.Log("SNAIL SPEED Activated");
        _speed /= _speedMultiplier;
    }
    public void ShieldsActive()
    {
       _isShieldsActive = true;
       _shieldVisualizer.SetActive(true);
        
        
    }

    public void AddScore(int points)
    {
        _score += points;
        Debug.Log("Points..I need more POOOOIIIINTSSS!");
        _uiManager.UpdateScore(_score);
    }
    


}