using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]private float _speed = 3.0f;
    [SerializeField]private int _powerupID;
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
                        break;
                    case 1:
                        AudioSource.PlayClipAtPoint(_clip, transform.position);
                        player.SpeedBoostActive();
                        Debug.Log("I’m faster than my Wi-Fi!");
                        break;
                    case 2:
                        AudioSource.PlayClipAtPoint(_clip, transform.position);
                        player.ShieldsActive();
                        break;
                    case 3:
                        AudioSource.PlayClipAtPoint(_clip, transform.position, 1.5f);
                        player.AddAmmo(10);//Adds 10 ammo 
                        player.UpdateUIAmmo();// Update the UI to reflect new ammo count
                        Debug.Log("Ammo Loaded");
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
                        break;
                    case 5:
                    AudioSource.PlayClipAtPoint(_clip, transform.position, 1.5f);
                    player.ActivateChainLightning();
                    Debug.Log("⚡BOLT STORM ONLINE!");
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            
            
            Destroy(this.gameObject);
        }
    }  
}

