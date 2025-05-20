using UnityEngine;
using UnityEngine.UI;

public class BossEye : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 20;
    private int _currentHealth;
    [SerializeField] private Image _eyeHealthBarFill;
    [SerializeField] private GameObject _eyeExplosionPrefab;
    [SerializeField] private AudioClip _damageSound;

    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerLaser"))
        {
            Debug.Log($"{this.name} hit by PlayerLaser!");

            TakeDamage(1); // Apply 1 damage per laser hit (you can change this)

            Destroy(other.gameObject); // Destroy the laser
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{this.name} took {damage} damage");
        _currentHealth -= damage;

        if(_eyeHealthBarFill != null)
        {
            _eyeHealthBarFill.fillAmount = (float)_currentHealth / _maxHealth;
        }

        if (_damageSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_damageSound);
        }

        if (_currentHealth <=0)
        {
            Die();
        }
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
    public void AssignHealthBar(Image image)
    {
        _eyeHealthBarFill = image;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
