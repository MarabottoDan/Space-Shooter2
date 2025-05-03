using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class Player : MonoBehaviour

{
    [SerializeField] private float _speed = 5.6f;
    private float _speedMultiplier = 2;
    [SerializeField] float _shiftSpeed = 10f;
    private bool _isSpeedBoostActive = false;
    [SerializeField] private GameObject _laserPlayerPrefab;
   
    [SerializeField] private GameObject _tripleShotPrefab;

    [Header("Homing Missile")]
    [SerializeField] private GameObject _homingMissilePrefab;
    [SerializeField] private Transform _missileSpawnPoint;
    


    [Header("Chain Lightning")]
    [SerializeField] private GameObject _chainLightningShotPrefab;// Reference to the bolt projectile prefab (assign in Inspector)
    [SerializeField] private bool _isChainLightningActive = false;// Tracks whether chain lightning is currently active

    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;

    private bool _isSlowed = false; // Tracks whether slime debuff is active
    private bool _isDamaged = false; // To prevent multiple damage hits at once

    [Header("PickupMagnetBehavior")]
    [SerializeField] private float _pickupPullSpeed = 10f;
    [SerializeField] private float _pickPullRadius = 5f;
    [SerializeField] private float _pullDuration = 3f;
    [SerializeField] private float _pullCooldown = 50f;

    [SerializeField] private Image _magnetBar; //assign the magnet bar image here
    [SerializeField] private GameObject _magneticWavesPrefab; //MagneticWavesPrefab here

    private bool _isPullingPickups = false;
    private bool _canPull = true;
 
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

        if (Input.GetKeyDown(KeyCode.C) && _canPull)
        {
            StartCoroutine(PullRoutine());// This will be called...
        }

        if (_isPullingPickups)
        {
            PullPickups();//We'll define this function next.
        }
      
    }

    private IEnumerator PullRoutine()
    {
        _isPullingPickups = true;
        _canPull = false;

        
        _magneticWavesPrefab.SetActive(true);//Turn on visual effects  
        _magnetBar.fillAmount = 1f;//Start with the full magnet image

        float timer = _pullDuration;

        // DRAIN BAR DURING PULL
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            _magnetBar.fillAmount = timer / _pullDuration;//Drain the bar over time
            yield return null;
        }

        //End pull mode
        _isPullingPickups = false;
        _magneticWavesPrefab.SetActive(false);
        _magnetBar.fillAmount = 0f;

        // REFILL BAR DURING COOLDOWN
        float cooldownTimer = _pullCooldown;
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            _magnetBar.fillAmount = 1f - (cooldownTimer / _pullCooldown);
            yield return null;
        }
        
        _canPull = true;
    }
    // This method finds nearby pickups and pulls them toward the player while the magnet is active
    private void PullPickups()
    {   // Find all GameObjects in the scene tagged as "Pickup"
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");

        foreach (GameObject pickup in pickups)// Loop through each pickup
        {
            // Calculate the distance between the player and the pickup
            float distance = Vector3.Distance(transform.position, pickup.transform.position);

            // If the pickup is within the defined pull radius
            if (distance <= _pickPullRadius)
            {
                // Calculate the direction from the pickup to the player
                Vector3 direction = (transform.position - pickup.transform.position).normalized;
                // Move the pickup toward the player at the defined speed
                pickup.transform.position += direction * _pickupPullSpeed * Time.deltaTime;
            }
        }
    }

    public void ActivateHomingMissiles()
    {
        
        StartCoroutine(HomingMissileRoutine());// Fire every 2 seconds
    }

    private IEnumerator HomingMissileRoutine()
    {
        float duration = 5f;
        float fireRate = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Instantiate(_homingMissilePrefab, _missileSpawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(fireRate);
            elapsed += fireRate;
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
        if (_isSlowed)
        {
            return;//Don't override Slimedebuff while it's active
        }
           
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

    public void ApplySlimeDebuff(float slowAmount, float duration, GameObject slimeMess)
    {
        // Start a coroutine to handle the slowdown and timed recovery
        StartCoroutine(SlimeDebuffRoutine(slowAmount, duration, slimeMess));
    }

    private IEnumerator SlimeDebuffRoutine(float slowAmount, float duration, GameObject slimeMess)
    {
        _isSlowed = true;// Flag to prevent ThrusterSpeed from overriding
        _speed -= slowAmount;
        Debug.Log("Speed reduced by slime!");

        yield return new WaitForSeconds(duration);

        _speed += slowAmount;
        _isSlowed = false; // Reset the flag after debuff ends
        Debug.Log("Speed restored.");

        if (slimeMess != null)
        {
            Destroy(slimeMess);// Remove the visual after effect ends
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
            Instantiate(_laserPlayerPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
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
        if (_isDamaged == true)
        {
            return; //Already damaged recently, ignore extra hits
        }

        _isDamaged = true; // Mark as damaged

        _cameraShake.StartCoroutine(_cameraShake.Shake(1f, 0.5f));
        // Trigger a short, subtle camera shake when the player takes damage

        if (_isShieldsActive) 
        {
            _shieldHits--; 

            if (_shieldHits == 2)
            {
                SetShieldColor(Color.green); 
                
            }
            else if (_shieldHits == 1)
            {
                SetShieldColor(Color.red); 
                
            }
            else if (_shieldHits <= 0)
            {
                SetShieldColor(Color.white);
                _isShieldsActive = false; 
                _shieldVisualizer.SetActive(false); 
                Debug.Log("Shields down! Panic mode engaged!"); 
              
            }

            StartCoroutine(DamageCooldown());
            return;
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

        StartCoroutine(DamageCooldown());
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
        Debug.Log("Enemy destroyed!");
        _uiManager.UpdateScore(_score);
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(0.2f); // Tiny invincibility window
        _isDamaged = false; // Allow taking damage again after cooldown
    }

    public void LoseLife()
    {
        _lives--;
        _uiManager.UpdateLives(_lives);

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _audioSource.Play();
            Destroy(this.gameObject);
            _cameraShake.StartCoroutine(_cameraShake.Shake(1f, 0.5f));
        }
    }

    public void ApplyRandomPowerup()
    {
        int random = Random.Range(0, 3);// At least Triple Shot, Speed Boost, and Ammo refills.

        switch (random)
        {
            case 0:
                TripleShotActive();
                Debug.Log("Random Powerup: Triple Shot!");
                break;

            case 1:
                SpeedBoostActive();
                Debug.Log("Random Powerup : SpeedBoost!");
                break;

            case 2:
                AddAmmo(5);
                UpdateAmmoUI();
                Debug.Log("Random Powerup: Ammo Boost!");
                break;


        }
    }

    
}