using UnityEngine;
using UnityEngine.UI;

public class BossEye : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 20;
    private int _currentHealth;

    [SerializeField] private GameObject _eyeExplosionPrefab;
    [SerializeField] private AudioClip _damageSound;

    private AudioSource _audioSource;
    private Image _healthBarFill; // This will be assigned dynamically

    void Start()
    {
        _currentHealth = _maxHealth;
        _audioSource = GetComponent<AudioSource>();

        if (_healthBarFill == null)
        {
            Debug.LogWarning($"{this.name}: Health bar fill not assigned.");
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"{this.name} took {damage} damage. Current: {_currentHealth}");

        if (_damageSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_damageSound);
        }

        UpdateHealthBar();

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

    private void Die()
    {
        if (_eyeExplosionPrefab != null)
        {
            Instantiate(_eyeExplosionPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log($"{this.name} destroyed!");
        Destroy(this.gameObject);
    }
}
