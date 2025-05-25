using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossEye : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 20;
    private int _currentHealth;

    [SerializeField] private GameObject _eyeExplosionPrefab;
    [SerializeField] private AudioClip _damageSound;

    [Header("Flash damage Eyes")]
    [SerializeField] private SpriteRenderer _eyeSprite;
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;

    private Color _originalColor;

    private AudioSource _audioSource;

    [SerializeField] private Image _healthBarFill; // <-- Use this ONE for both Inspector and SpawnManager

    [SerializeField] private string _eyeSide = "Left"; 


    void Start()
    {
        _currentHealth = _maxHealth;
        _audioSource = GetComponent<AudioSource>();
        _originalColor = _eyeSprite.color;

        if (_healthBarFill == null)
        {
            Debug.LogWarning($"{this.name}: Health bar fill not assigned.");
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{this.name} took {damage} damage");
        _currentHealth -= damage;

        UpdateHealthBar();

        if (_damageSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_damageSound);
        }

        if (_eyeSprite != null)
        {
            StartCoroutine(FlashDamageEffect());
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (_healthBarFill != null)
        {
            _healthBarFill.fillAmount = (float)_currentHealth / _maxHealth;
        }
        else
        {
            Debug.LogWarning($"{this.name}: Health bar image is still null.");
        }
    }

    public void AssignHealthBar(Image image)
    {
        _healthBarFill = image;
        Debug.Log($"{this.name}: Health bar assigned = {image.name}");
    }

    private IEnumerator FlashDamageEffect()
    {
        _eyeSprite.color = _damageColor;
        yield return new WaitForSeconds(_flashDuration);
        _eyeSprite.color = _originalColor;
    }

    private void Die()
    {
        Debug.Log($"{this.name} -> DIE() CALLED");

        if (_eyeExplosionPrefab != null)
        {
            Transform bossTransform = transform.parent; // Get the boss (eye's parent)

            if (bossTransform != null)
            {
                Instantiate(_eyeExplosionPrefab, transform.position, Quaternion.identity, bossTransform);
            }
            else
            {
                // Fallback: no parent, spawn in world space
                Instantiate(_eyeExplosionPrefab, transform.position, Quaternion.identity);
            }


        }

        if (_damageSound != null)
        {
            GameObject audioObj = new GameObject("EyeDeathSound");
            audioObj.transform.position = transform.position;

            AudioSource tempAudio = audioObj.AddComponent<AudioSource>();
            tempAudio.clip = _damageSound;
            tempAudio.volume = 1f;
            tempAudio.Play();

            Destroy(audioObj, _damageSound.length);
        }

        Debug.Log($"{this.name} destroyed!");
            // Notify the BossEyeDeathHandler which eye was destroyed

            FindObjectOfType<BossEyeDeathHandler>()?.OnEyeDeath(_eyeSide);

        Destroy(this.gameObject);
    }

}
