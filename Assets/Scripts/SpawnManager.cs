using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour

{
    [SerializeField]private GameObject _enemyprefab;
    [SerializeField] private GameObject _horizontalEnemyPrefab;
    [SerializeField] private GameObject _whiteEnemyPrefab;
    [SerializeField]private GameObject _enemyContainer;
    [SerializeField]private GameObject[] _powerups;
   

    [Header("Wave system")]
    [SerializeField] private int _startRegularEnemyCount = 3;// Number of regular enemies to spawn in the first wave
    [SerializeField] private int _startHorizontalEnemyCount = 1;// Number of horizontal enemies to spawn in the first wave
    [SerializeField] private float _timeBetweenWaves = 5f;// Delay (in seconds) between waves
    [SerializeField] private UIManager _uiManager;// Reference to the UIManager script to update wave UI and countdown

    private int _currentWave = 1; // Tracks the current wave number
    private bool _stopSpawning = false;// Flag to stop spawning when the player dies


    
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyWaves());
        StartCoroutine(SpawnPowerupRoutine());
        _uiManager.UpdateWaveText(_currentWave);
    }

    IEnumerator SpawnEnemyWaves()
    {
        yield return new WaitForSeconds(3.0f);// Wait a few seconds before the first wave starts
        while (!_stopSpawning)
        {
            // Calculate how many enemies to spawn this wave
            int regularEnemies = _startRegularEnemyCount + (_currentWave - 1); //Increases the number of regular enemies by 1 more each wave.
            int horizontalEnemies = _startHorizontalEnemyCount + Mathf.FloorToInt((_currentWave - 1) / 2);
            //Increases the number of horizontal enemies every 2 waves.

            for (int i = 0; i < regularEnemies; i++) // Spawn regular enemies in random range location
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);
                GameObject newEnemy = Instantiate(_enemyprefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;

                yield return new WaitForSeconds(0.5f);// Small delay between spawns
            }
            // Spawn 1 WhiteEnemy per wave
            Vector3 whiteEnemyPos = new Vector3(0, 12f, 0); // It'll decide L or R in its own Start()
            GameObject whiteEnemy = Instantiate(_whiteEnemyPrefab, whiteEnemyPos, Quaternion.identity);
            whiteEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(0.5f); // Optional delay

            for (int i =0; i < horizontalEnemies; i ++)// Spawn horizontal enemies in random range location
            {
                Vector3 horizontalPos = new Vector3(-24, Random.Range(1f, 6f), 0);
                GameObject horizontalEnemy = Instantiate(_horizontalEnemyPrefab, horizontalPos, Quaternion.identity);
                horizontalEnemy.transform.parent = _enemyContainer.transform;

                yield return new WaitForSeconds(0.5f);
            }
            
            yield return new WaitUntil(() => _enemyContainer.transform.childCount == 0);// Wait until all enemies are destroyed


            Debug.Log("Wave " + _currentWave + " completed");

            yield return _uiManager.ShowWaveCountDown(5); // Show countdown before next wave
            // Update wave count and wave UI
            _currentWave++;
            _uiManager.UpdateWaveText(_currentWave);
          
        } 
    }



    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);// Wait 3 seconds before starting to spawn power-ups
        while (_stopSpawning == false)// Continue spawning as long as the player is alive
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);
            // Choose a random X position within screen bounds, Y = 12 to spawn from the top
            int randomPowerUP = Random.Range(0, 7);// Randomly select one of the 6 power-ups

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

