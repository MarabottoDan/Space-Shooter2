using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private float _lifetime = 0f; //Counter for how long Black holes have been "alive"
    private float _logTimer = 0f; //Counter for how long Black holes have been "alive"


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {    //Counter for how long Black holes have been "alive"
        _lifetime += Time.deltaTime;
        _logTimer += Time.deltaTime;

        if (_logTimer >= 1f)
        {
            Debug.Log($"{gameObject.name} has been alive for {Mathf.FloorToInt(_lifetime)} seconds.");
            _logTimer = 0f;
        }
        gameObject.name = $"BlackHole_{Mathf.FloorToInt(_lifetime)}s";

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.EnterBlackHole(this);

                // Find all active black holes for teleport chain
                GameObject[] allBHs = GameObject.FindGameObjectsWithTag("BlackHole");
                List<Transform> positions = new List<Transform>();

                foreach (GameObject bh in allBHs)
                {
                    positions.Add(bh.transform);
                }

                // Sort black holes by X to control teleport order (optional)
                positions.Sort((a, b) => a.position.x.CompareTo(b.position.x));

                player.StartBlackHoleChain(positions);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ExitBlackHole();
                Debug.Log("Player EXITED Black Hole");
            }
        }
    }

    private void OnDestroy()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Player player = playerObj.GetComponent<Player>();
            if (player != null)
            {
                player.ExitBlackHole();
                Debug.Log("Black Hole destroyed — forcing player to exit slow mode.");
            }
        }
    }

}
