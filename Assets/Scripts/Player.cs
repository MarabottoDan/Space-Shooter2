using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class Player : MonoBehaviour

{
    [SerializeField] private float _speed = 5.6f;
    private float _speedMultiplier = 2;
    [SerializeField] float _shiftSpeed = 10f;
    private bool _isSpeedBoostActive = false;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    

    [Header("Ammo Settings")]
    // [SerializeField] private int _fullAmmo;
    [SerializeField] private int _maxAmmo = 15;//Max Ammo
    public int _currentAmmo;//Current Ammo
    public Text _ammoText;//Reference to the Text component that displays ammo count



    [Header("Shields")]
    [SerializeField] private bool _isShieldsActive = false;
    [SerializeField] private GameObject _shieldVisualizer;// Blue shield visualizer
    [SerializeField] private GameObject _shieldVisualizer1;//Green shield visualizer
    [SerializeField] private GameObject _shieldVisualizer2;//Red shield visualizer
    private int _shieldHits;// Hits the shield can take

    [Header("Thrusters")]
    [SerializeField] private GameObject _rightEngine, _leftEngine;
    [SerializeField] private GameObject _thrustersCharge; 
    private bool _leftEngineOn = false;
    private bool _rightEngineOn = false;

    [Header("Audio")]
    [SerializeField] private AudioClip _noAmmoClip;
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _playerDestroyedSound;



    [SerializeField] private int _score;
    private UIManager _uiManager; //This reference connects the UIManager script (where the thruster bar is managed) to the Player script.
    private string[] _hitMessages =
    {
        "I've been hit!",
        "I think my spaceship has a dent now!",
        "Why can't we be friends?",
        "Oh you're going to pay for that!"
     };
   // private bool _isGameOver = false;
  

    // Start is called before the first frame update
    void Start()
    {
        _currentAmmo = _maxAmmo;
        UpdateAmmoUI();
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
        ThrusterSpeed();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();            
        }
      
    }

    void Calculatemovement()

    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 _direction = new Vector3(horizontalInput, verticalInput, 0);

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

    void ThrusterSpeed()

    {
           
         if (Input.GetKey(KeyCode.LeftShift) && !_isSpeedBoostActive && _uiManager.Thrusters_Bar_Fill)
        //This checks if the Left Shift key is pressed, whether the speed boost is already active, and whether there is fuel left in the thruster bar
        {
            _speed = _shiftSpeed;
         }
         else if (!_isSpeedBoostActive)
         {
             _speed = 5.6f;
         }
        
    }

    void UpdateAmmoUI()
    {
        _ammoText.text = _currentAmmo.ToString();

    }
    void FireLaser()
    {
        if (_currentAmmo <= 0)
        {
            Debug.Log("Playing no ammo clip");
            _audioSource.PlayOneShot(_noAmmoClip);

            //AudioSource.PlayClipAtPoint(_noAmmoClip, transform.position);
            return;
        }
        _currentAmmo--;//Decrease Ammo when firing
        UpdateAmmoUI();//Update UI with the new ammo count
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
    // This method sets the color of the shield's material
    private void SetShieldColor(Color color)
    {
        // Assumes the shield GameObject has a Renderer with a material you can change
        _shieldVisualizer.GetComponent<Renderer>().material.color = color;
    }


    public void ShieldsActive()
    {
        if (_isShieldsActive)
        {
            Debug.Log("No Extra Shields for YOU");
            return;//If shield is active, prevents from another shield powerup being activated
        }
        _shieldHits = 3;
        _isShieldsActive = true; 
        _shieldVisualizer.SetActive(true); //Blue on

    }
    public void Damage()
    {

        if (_isShieldsActive) // Check if the shield is active
        {
            _shieldHits--; // Decrease shield hit count by 1

            if (_shieldHits == 2)
            {
                SetShieldColor(Color.green); // Change shield color to green after 1 hit
                return; // Exit the method early
            }
            else if (_shieldHits == 1)
            {
                SetShieldColor(Color.red); // Change shield color to red after 2 hits
                return; // Exit the method early
            }
            else if (_shieldHits <= 0)
            {
                SetShieldColor(Color.white);//Changes shield color back to original color of the sprite
                _isShieldsActive = false; // Mark the shield as inactive
                _shieldVisualizer.SetActive(false); // Hide the shield visual
                Debug.Log("Shields down! Panic mode engaged!"); // Log message
                return; // Exit the method
            }
            
         }
            _lives -= 1;
        //_lives = _lives -1;
        // _lives--;

        if (_lives == 2)
        {
          
            if (Random.value < 0.5f)
            {
                _leftEngine.SetActive(true);
                _leftEngineOn = true;
            }
            else
            {
                _rightEngine.SetActive(true);
                _rightEngineOn = true;
            }
        }
        else if (_lives == 1)
        {
            // Show the engine that hasn’t already been shown
            if (!_leftEngineOn)
            {
                _leftEngine.SetActive(true);
                _leftEngineOn = true;
            }
            else if (!_rightEngineOn)
            {
                _rightEngine.SetActive(true);
                _rightEngineOn = true;
            }
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

    public void UpdateUIAmmo()
    {
        _ammoText.text =  _currentAmmo.ToString();  // Update the UI text to show current ammo
    }
    public void AddAmmo(int amount)
    {
        _currentAmmo += amount;
        _currentAmmo = Mathf.Min(_currentAmmo, _maxAmmo);
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





    public void AddScore(int points)
    {
        _score += points;
        Debug.Log("Points..I need more POOOOIIIINTSSS!");
        _uiManager.UpdateScore(_score);
    }
    


}