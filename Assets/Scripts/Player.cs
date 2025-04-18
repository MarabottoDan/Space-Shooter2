﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class Player : MonoBehaviour

{
    [SerializeField] private float _speed = 5.6f;
    private float _speedMultiplier = 2;
    [SerializeField] float _shiftSpeed = 10f;
    private bool _isSpeedBoostActive = false;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;

    [Header("Chain Lightning")]
    [SerializeField] private GameObject _chainLightningShotPrefab;// Reference to the bolt projectile prefab (assign in Inspector)
    [SerializeField] private bool _isChainLightningActive = false;// Tracks whether chain lightning is currently active

    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    
    private float _canFire = -1f;
    private float _addLife = 1f;//Adds 1 live to the player
    private int _maxLives = 3;
   


    [Header("Ammo Settings")]
    
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
    [SerializeField] private AudioClip _noMoreLivesForYouClip;// Clip for when player has full lives and collects Health PowerUp



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
    private CameraShake _cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        // Cache the CameraShake script from the main camera once at the start
        _currentAmmo = _maxAmmo;//Set current ammo to max at game start
        UpdateAmmoUI();//Update the UI with the initial ammo value
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
            _canFire = Time.time + _fireRate;

            if (_isChainLightningActive)
            {
                Instantiate(_chainLightningShotPrefab, transform.position + new Vector3(0, 0.8f,0), Quaternion.identity);
                // Spawns the lightning shot slightly above the player for better visual alignment
                transform.Translate(Vector3.down * _speed * Time.deltaTime);// This creates a small downward nudge when
                                                                            //firing, giving the effect of recoil or power pushback
            }
            else
            {
                FireLaser();
            }     
        }
      
    }

    public void ActivateChainLightning()
    {
        _isChainLightningActive = true; // Enable the power-up for the player
        StartCoroutine(ChainLightningCooldown()); // Begin countdown to disable the ability
    }

    private IEnumerator ChainLightningCooldown()
    {
        yield return new WaitForSeconds(5f);// Allow firing for 5 seconds
        _isChainLightningActive = false;// Disable the power-up
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
        _ammoText.text = _currentAmmo.ToString();//Displays the number of ammo

    }
    void FireLaser()
    {
        if (_currentAmmo <= 0)//Check if player has ammo
        {
            Debug.Log("Playing no ammo clip");
            _audioSource.PlayOneShot(_noAmmoClip);//Play no ammo sound
            return;
        }
        _currentAmmo--;//Decrease Ammo when firing
        UpdateAmmoUI();//Update UI with the new ammo count
        _canFire = Time.time + _fireRate;//How fast we can fire

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
        _cameraShake.StartCoroutine(_cameraShake.Shake(1f, 0.5f));
        // Trigger a short, subtle camera shake when the player takes damage

        if (_isShieldsActive) 
        {
            _shieldHits--; 

            if (_shieldHits == 2)
            {
                SetShieldColor(Color.green); 
                return; 
            }
            else if (_shieldHits == 1)
            {
                SetShieldColor(Color.red); 
                return; 
            }
            else if (_shieldHits <= 0)
            {
                SetShieldColor(Color.white);
                _isShieldsActive = false; 
                _shieldVisualizer.SetActive(false); 
                Debug.Log("Shields down! Panic mode engaged!"); 
                return; 
            }
            
         }
            _lives -= 1;
        //_lives = _lives -1;// _lives--;

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

    // Method to add a life to the player. Returns true if a life was added, false otherwise.
    public void AddLife()
    {
        if (_lives < _maxLives)// Check if the current lives are less than the maximum allowed
        {
            _lives += (int)_addLife;// Increase the lives by 1
            _uiManager.UpdateLives(_lives); //Update the UI to reflect the new lives count

            if (_lives == 2)// If player now has 2 lives, turn off one of the two engine fires randomly
            {
                if  (_leftEngineOn &&_rightEngineOn)// Only proceed if both engines are currently on fire
                {
                    if (Random.value < 0.5f)// Randomly choose to turn off either the left or right engine fire
                    {
                        _leftEngine.SetActive(false);
                        _leftEngineOn = false;
                    }
                    else
                    {
                        _rightEngine.SetActive(false);
                        _rightEngineOn = false;
                    }
                }

            }
            else if (_lives == 3)// If player now has full health (3 lives), turn off both engine fires
            {
                if (_leftEngineOn)
                {
                    _leftEngine.SetActive(false);
                    _leftEngineOn = false;
                }
                if (_rightEngineOn)
                {
                    _rightEngine.SetActive(false);
                    _rightEngineOn = false;
                }
            }
            return;
        }
        else
        {
            Debug.Log("Max lives reached");
            // Play a sound indicating no more lives can be added
            _audioSource.PlayOneShot(_noMoreLivesForYouClip);
        }
    }

    public int GetCurrentLives()// Returns the player's current number of lives
    {
        return _lives;
    }

    public int GetMaxLives()// Returns the maximum number of lives the player can have
    {
        return _maxLives;
    }
    public void PlayNoMoreLivesForYouClip()//Plays clip
    {
        _audioSource.PlayOneShot(_noMoreLivesForYouClip);
    }
    public void UpdateUIAmmo()
    {
        _ammoText.text =  _currentAmmo.ToString();  // Update the UI text to show current ammo
    }
    public void AddAmmo(int amount)
    {
        _currentAmmo += amount; // Increase _currentAmmo by the amount passed into the AddAmmo method
        _currentAmmo = Mathf.Min(_currentAmmo, _maxAmmo); // Prevents ammo from going over max
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