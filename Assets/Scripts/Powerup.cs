using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID;
    [SerializeField] private GameObject _slimeMessPrefab;
    [SerializeField] private float _slimeSlowAmount = 2.0f;
    [SerializeField] private float _slimeDuration = 5.0f;

    private AudioSource _audioSource;
    //0 = Triple Shot 1 = Speed 2 = Shields

    private string[] _tripleShotText =
   {
        "Triple shot BABY!!!",
        "Say hello to my little friends!",
        "Time to light you up!"
   };

    [SerializeField] private AudioClip _clip; 

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {       
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        if (transform.position.y < -11.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

           
               switch(_powerupID)
                {
                    case 0:
                        AudioSource.PlayClipAtPoint(_clip, transform.position);
                        player.TripleShotActive();
                        int randomIndex = Random.Range(0, _tripleShotText.Length);
                        Debug.Log(_tripleShotText[randomIndex]);
                        Destroy(this.gameObject);
                    break;
                    case 1:
                        AudioSource.PlayClipAtPoint(_clip, transform.position);
                        player.SpeedBoostActive();
                        Debug.Log("I’m faster than my Wi-Fi!");
                    Destroy(this.gameObject);
                    break;
                    case 2:
                        AudioSource.PlayClipAtPoint(_clip, transform.position);
                        player.ShieldsActive();
                    Destroy(this.gameObject);
                    break;
                    case 3:
                        AudioSource.PlayClipAtPoint(_clip, transform.position, 1.5f);
                        player.AddAmmo(10);//Adds 10 ammo 
                        player.UpdateUIAmmo();// Update the UI to reflect new ammo count
                        Debug.Log("Ammo Loaded");
                    Destroy(this.gameObject);
                    break;
                    case 4:
                        if (player.GetCurrentLives() < player.GetMaxLives())// Check if the player currently has fewer than the maximum allowed lives
                    {
                            AudioSource.PlayClipAtPoint(_clip, transform.position);// Play the health pickup audio clip
                            player.AddLife();// Add one life to the player and update relevant visuals (like engine fire and UI)
                        Debug.Log("Life Added");
                        
                    }
                        else
                        {
                        // If the player already has max lives, play a different audio
                            player.PlayNoMoreLivesForYouClip();
                            Debug.Log("Max lives already, No more lives for you");
                        }
                    Destroy(this.gameObject);
                    break;
                    case 5:
         
                    _audioSource.PlayOneShot(_clip);
                    player.ActivateChainLightning();// Activates the chain lightning ability on the player for a limited time
                    Debug.Log("⚡BOLT STORM ONLINE!");// Debug message for confirmation
                                                     // In case 5
                    SpriteRenderer srBolt = GetComponent<SpriteRenderer>();
                    if (srBolt != null) srBolt.enabled = false;

                    Collider2D colBolt = GetComponent<Collider2D>();
                    if (colBolt != null) colBolt.enabled = false;

                    Destroy(this.gameObject, _clip.length);
                    break;
                    case 6:
                    AudioSource.PlayClipAtPoint(_clip, transform.position, 1.0f); // Slime splat sound

                    // Instantiate slime mess splat at the player's position
                    GameObject slimeMess = Instantiate(_slimeMessPrefab, other.transform.position, Quaternion.identity);
                    slimeMess.transform.SetParent(other.transform); // Stick it to the player
                    // Apply the movement slow debuff to the player (custom method in Player script)
                    player.ApplySlimeDebuff(_slimeSlowAmount, _slimeDuration, slimeMess);
                    Debug.Log("Slimed! Movement Slowed.");

                    // Hide all SpriteRenderers (parent + children)
                    foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                    {
                        sr.enabled = false;
                    }

                    // Disable the power-up's collider so it can't be picked up again
                    Collider2D col = GetComponent<Collider2D>();
                    if (col != null) col.enabled = false;

                    // Destroy the power-up after the audio finishes playing
                    Destroy(this.gameObject, _clip.length);
                    break;
                   default:
                        Debug.Log("Default Value");
                        break;
                }
            
        }
    }  
}

