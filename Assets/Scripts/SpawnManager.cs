using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour

{
    [SerializeField]private GameObject _enemyprefab;
    [SerializeField] private GameObject _horizontalEnemyPrefab;
    [SerializeField]private GameObject _enemyContainer;
    [SerializeField]private GameObject[] _powerups;
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnHorizontalEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine ()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {          
            Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);
            GameObject newEnemy = Instantiate(_enemyprefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }     
    }


   IEnumerator SpawnHorizontalEnemyRoutine()
    {
        yield return new WaitForSeconds(15f);
        while (_stopSpawning == false)
        {
            Vector3 horizontalPos = new Vector3(-24f,Random.Range(1f, 6f), 0);
            GameObject horizontalEnemy = Instantiate(_horizontalEnemyPrefab, horizontalPos, Quaternion.identity);
            horizontalEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(35.0f);

        }
    }



    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);// Wait 3 seconds before starting to spawn power-ups
        while (_stopSpawning == false)// Continue spawning as long as the player is alive
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);
            // Choose a random X position within screen bounds, Y = 12 to spawn from the top
            int randomPowerUP = Random.Range(0, 6);// Randomly select one of the 6 power-ups

            if (randomPowerUP == 5)// Check if the selected power-up is the Bolt Power-Up
            {
                float _rarityRoll = Random.Range(0f, 1f);// Generate a float between 0 and 1

                if (_rarityRoll <= 0.50f)// Only spawn Bolt Power-Up 15% of the time
                {
                    Instantiate(_powerups[randomPowerUP], posToSpawn, Quaternion.identity);
                    // Spawn rare Bolt Power-Up
                }
            }
            else
            { 
            Instantiate(_powerups[randomPowerUP], posToSpawn, Quaternion.identity);
                // Spawn all other power-ups with normal chance
            }
            yield return new WaitForSeconds(Random.Range(5, 10));
            // Wait a random amount of time before spawning the next power-up (between 5–10 seconds)
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}

