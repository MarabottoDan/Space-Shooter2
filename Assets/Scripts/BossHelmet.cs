using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class BossHelmet : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 50;
    private int _currentHealth;
    [SerializeField] private GameObject _helmetDamageEffect;
    [SerializeField] private AudioClip _damageSound;
    private AudioSource _audioSource;
    private Image _healthBarFill;// Drag your health bar fill here in the Inspector

    [Header("Phase One Transition")]
    [SerializeField] private GameObject _bossOrbPhase1Prefab;
    private Vector3 _orbSpawnPosition;

    private bool _hasSpawnedOrb = false;
    [SerializeField] private GameObject _helmetIcedVersion;

    [SerializeField] private SpriteRenderer _helmetSprite;
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;
    private Color _originalColor;

    private BossFirstAndSecondPhase _bossPhaseController;




    // Start is called before the first frame update
    private void Start()
    {

        _bossPhaseController = GetComponentInParent<BossFirstAndSecondPhase>();

        if (_bossPhaseController == null)
        {
            Debug.LogWarning("BossPhaseController not found on parent!");
        }

        _currentHealth = _maxHealth;
        _audioSource = GetComponent<AudioSource>();

        _orbSpawnPosition = new Vector3(-12f, 13f, 0f);

        // Lock the Z position to 0 in case it's inherited from parent
        Vector3 fixedZ = transform.position;
        fixedZ.z = 0f;
        transform.position = fixedZ;
        _originalColor = _helmetSprite.color;


    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if(_helmetDamageEffect != null)
        {
            Instantiate(_helmetDamageEffect, transform.position, Quaternion.identity);
        }

        if (_damageSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_damageSound);
        }

        if (_helmetSprite != null)
        {
            StartCoroutine(FlashDamageEffect());
        }

        UpdateHealthBar();
        Debug.Log("Helmet health: " + _currentHealth);


        if (_currentHealth <= 0 && !_hasSpawnedOrb)
        {
            _hasSpawnedOrb = true; //Prevents multiple spawning

            if (_bossPhaseController != null)
            {
                _bossPhaseController.MoveToPhaseTwoStart();
            }
            StartCoroutine(HandleHelmetDestruction());
        }
    }

    private IEnumerator HandleHelmetDestruction()
    {
        Debug.Log("BossHelmet will spawn orb at: " + _orbSpawnPosition);
        Debug.Log("Orb prefab is: " + _bossOrbPhase1Prefab?.name);

        Debug.Log("Helmet destroyed");

        if (_bossOrbPhase1Prefab != null)
        {
            Instantiate(_bossOrbPhase1Prefab, _orbSpawnPosition, Quaternion.identity);
            Debug.Log("Boss Phase 1 Orb Spawned");
        }
        else
        {
            Debug.LogWarning("Orb prefab is not assigned!");
        }

        yield return null; 
    }


    private void UpdateHealthBar()
    {
        _healthBarFill.fillAmount = (float)_currentHealth / _maxHealth;
    }

    public void AssignHealthBar(Image image)
    {
        _healthBarFill = image;
        Debug.Log("BossHelmet: Health bar assigned!");
    }

    public void SetOrbPrefab(GameObject orb)
    {
        _bossOrbPhase1Prefab = orb;
        Debug.Log("Orb prefab set from SpawnManager.");
    }
    public void OnOrbSequenceComplete()
    {
        Debug.Log("Orb sequence complete. Now disabling or transitioning helmet...");

        
    }

    public void ApplyIceBlast()
    {
        Debug.Log("Helmet hit by IceBlast!");

        gameObject.SetActive(false); // Hide/dismiss current helmet

        SpawnManager spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.RevealBossEyesUI(); // This hides Alien bar and shows both eye bars
        }
        else
        {
            Debug.LogWarning("SpawnManager not found in ApplyIceBlast()");
        }

        // Re-enable Player Input
        Player player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        if (player != null)
        {
            player.EnableInput();
        }
        else
        {
            Debug.LogWarning("Player not found to re-enable input.");
        }
    }

    private IEnumerator FlashDamageEffect()
    {
        _helmetSprite.color = _damageColor;
        yield return new WaitForSeconds(_flashDuration);
        _helmetSprite.color = _originalColor;
    }


}
