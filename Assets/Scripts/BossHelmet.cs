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

    // Start is called before the first frame update
    private void Start()
    {
        _currentHealth = _maxHealth;
        _audioSource = GetComponent<AudioSource>();
        // Lock the Z position to 0 in case it's inherited from parent
        Vector3 fixedZ = transform.position;
        fixedZ.z = 0f;
        transform.position = fixedZ;    

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

        UpdateHealthBar();

        if (_currentHealth <= 0)
        {
            DestroyHelmet();
        }
    }

    private void DestroyHelmet()
    {
        Debug.Log("Helmet destroyed");
        gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        _healthBarFill.fillAmount = (float)_currentHealth / _maxHealth;
    }

    public void AssignHealthBar(Image image)
    {
        _healthBarFill = image;
        Debug.Log("✅ BossHelmet: Health bar assigned!");
    }

}
