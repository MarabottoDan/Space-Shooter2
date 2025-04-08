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
    public Image fillImage;
    public float fillAmount = 1f;

    public float _drainRate = 0.2f;
    public float _refillRate = 0.1f;
    [SerializeField]public float _refillDelay = 3.0f;

    private float _currentFill = 1f;
    private bool _isDraining = false;
    private bool _isRefilling = false;
    private float _refillTimer = 0f;
    public bool Thrusters_Bar_Fill => _currentFill > 0f;

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
        if (Input.GetKey(KeyCode.LeftShift) && _currentFill > 0f)
        {
            _isDraining = true;
            _isRefilling = false;
            _refillTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isDraining = false;
            _refillTimer = 0f;
        }

        if (_isDraining)
        {
            _currentFill -= _drainRate * Time.deltaTime;
            _currentFill = Mathf.Clamp01(_currentFill);
        }

        if (!_isDraining && _currentFill < 1f)
        {
            _refillTimer += Time.deltaTime;

            if (_refillTimer >= _refillDelay)
            {
                _isRefilling = true;
            }
        }

        if (_isRefilling)
        {
            _currentFill += _refillRate * Time.deltaTime;
            _currentFill = Mathf.Clamp01(_currentFill);

            if (_currentFill >= 1f)
            {
                _isRefilling = false;
                _refillTimer = 0f;
            }
        }

        fillImage.fillAmount = _currentFill;
    }
}
