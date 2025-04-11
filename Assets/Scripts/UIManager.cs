using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Image _livesImg;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    private GameManager _gameManager;


    public Image fillImage; // Image used for the fill (foreground) of the thruster bar
    public float fillAmount = 1f; // Initial amount of fill for the bar (1 = full)

    public float _drainRate = 0.2f; // Rate at which the thruster bar drains when used
    public float _refillRate = 0.1f; // Rate at which the bar refills after use
    [SerializeField]public float _refillDelay = 3.0f; // Delay before refill starts after letting go of the key

    private float _currentFill = 1f; // Current fill level of the bar
    private bool _isDraining = false; // Whether the bar is currently draining
    private bool _isRefilling = false;  // Whether the bar is currently refilling
    private float _refillTimer = 0f; // Timer to track when to start refilling
    public bool Thrusters_Bar_Fill => _currentFill > 0f; // Public read-only property to check if thrusters have any fuel left.
                                                         //Lets other scripts (like the Player) check if the bar still has energy left.

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score:";
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.Log("GameManager is NULL.");
        }
    }

    public void UpdateScore(int playerscore)
    {
        _scoreText.text = "Score:" + playerscore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
        {
            return;
        }

        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _currentFill > 0f) //If LeftShift key is pressed and thruster has energy
        {
            _isDraining = true; //Set _isDraining to true (start draining energy)
            _isRefilling = false; //Set _isRefilling to false (stop refilling energy)
            _refillTimer = 0f; // Reset the _refillTimer
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) //If LeftShift key is released
        {
            _isDraining = false; //Stop draining energy
            _refillTimer = 0f; // Reset _refillTimer
        }

        if (_isDraining) //If energy is being drained
        {
            _currentFill -= _drainRate * Time.deltaTime; //Decrease the _currentFill based on the drain rate and time
            _currentFill = Mathf.Clamp01(_currentFill); //Ensure the fill value stays between 0 and 1
        }

        if (!_isDraining && _currentFill < 1f) //If not draining and truster is not full
        {
            _refillTimer += Time.deltaTime; //Increase the _refillTimer

            if (_refillTimer >= _refillDelay) //If the _refillDelay has passed
            {
                _isRefilling = true; //Start energy refilling
            }
        }

        if (_isRefilling) //If energy is being refilled
        {
            _currentFill += _refillRate * Time.deltaTime; //Increase the _currentFill based on the _refillRate and Time
            _currentFill = Mathf.Clamp01(_currentFill); //Ensure the _currentFill value stays between 0 and 1

            if (_currentFill >= 1f) //If the _currentFill reaches 1 (full bar)
            {
                _isRefilling = false; //Stop refilling
                _refillTimer = 0f; // Reset _refillTimer
            }
        }

        fillImage.fillAmount = _currentFill; //Updates the fillImage of the fillAmount of the Thruster based on the _currentFill
    }
}
