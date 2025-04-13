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

            if (_powerupID == 3)  
            {
                AudioSource.PlayClipAtPoint(_clip, transform.position, 1.5f); 
            }
            else
                AudioSource.PlayClipAtPoint(_clip, transform.position); 
            {
               switch(_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        int randomIndex = Random.Range(0, _tripleShotText.Length);
                        Debug.Log(_tripleShotText[randomIndex]);
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        Debug.Log("I’m faster than my Wi-Fi!");
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AddAmmo(10);//Adds 10 ammo 
                        player.UpdateUIAmmo();// Update the UI to reflect new ammo count
                        Debug.Log("Ammo Loaded");
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
            
            Destroy(this.gameObject);
        }
    }  
}

