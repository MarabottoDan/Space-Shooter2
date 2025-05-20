using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SpawnManager : MonoBehaviour

{
    [SerializeField]private GameObject _enemyprefab;
    [SerializeField] private GameObject _horizontalEnemyPrefab;
    [SerializeField] private GameObject _whiteEnemyPrefab;
    [SerializeField]private GameObject _enemyContainer;
    [SerializeField] private GameObject _dodgeEnemyPrefab;
    [SerializeField] private float _dodgeEnemySpawnChance = 0.2f; // 20% chance instead of regular enemy

    [Header("Boss")]
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private int _startWave = 1; // Set this to 5 in Inspector to test boss
    [SerializeField] private float _bossIntroClipDelay = 2f;
    [SerializeField] private float _bossSpawnDelay = 10f;
    [SerializeField] private float _bossGrowDuration = 3f;
    [SerializeField] private AudioClip _bossIntroClip1;
    [SerializeField] private AudioClip _bossIntroClip2;
    [SerializeField] private Vector3 _bossFinalScale = Vector3.one; //// Final visible scale for boss
    [SerializeField] private float _bossDropDistance = 1f;
    [SerializeField] private float _bossDropSpeed = 2f;
    private bool _bossSpawned = false;
    [SerializeField] private GameObject _bossOrbPhase1Prefab;
    


    [Header("Boss UI")]
    [SerializeField] private GameObject _bossHealthBarFrame; // Drag the HealthBarFrame here
    [SerializeField] private float _bossHealthBarDelay = 1f;
    [SerializeField] private Image _bossHealthBarFill;
    [SerializeField] private GameObject _bossHealthBarFillGO;
    [SerializeField] private GameObject _leftEyeHealthBarGO;
    [SerializeField] private GameObject _rightEyeHealthBarGO;




    [Header("Testing")]
    [SerializeField] private bool _testBossMode = false;
    [SerializeField] private float _testBossDelay = 3f;// how long after Start to spawn the boss

    [Header("Powerups")]
    [SerializeField]private GameObject[] _powerups;

    [Header("SmartEnemy")]
    [SerializeField] private GameObject _smartEnemyPrefab;
    [SerializeField] private float _smartEnemySpawnChance = .2f;
   

    [Header("Wave system")]
    [SerializeField] private int _startRegularEnemyCount = 3;// Number of regular enemies to spawn in the first wave
    [SerializeField] private int _startHorizontalEnemyCount = 1;// Number of horizontal enemies to spawn in the first wave
   
    [SerializeField] private UIManager _uiManager;// Reference to the UIManager script to update wave UI and countdown

    private int _currentWave;
    //private bool _stopSpawning = false;// Flag to stop spawning when the player dies
    private bool _stopEnemySpawning = false;
    private bool _stopPowerupSpawning = false;




    public void StartSpawning()
    {
        if (_testBossMode)
        {
            StartCoroutine(TestBossSpawn());
        }
        else
        {
            _currentWave = _startWave;//Start from selected wave
            _uiManager.UpdateWaveText(_currentWave);
            _stopEnemySpawning = false;
            _stopPowerupSpawning = false;
            StartCoroutine(SpawnEnemyWaves());
            StartCoroutine(SpawnPowerupRoutine());
        }
    }

    private IEnumerator TestBossSpawn()
    {
        Debug.Log("TEST MODE: Boss will spawn in" + _testBossDelay + "seconds");
        yield return new WaitForSeconds(_testBossDelay);

        //Skip Wave logic and directly trigger boss logic
        _currentWave = 5;
        _bossSpawned = false;

        //Manually call just the boss spawn from SpawnEnemyWaves
        yield return StartCoroutine(SpawnBossDirectly());
    }

    private IEnumerator SpawnBossDirectly()
    {
        _bossSpawned = true;
        Debug.Log("Wave 5 complete. Spawning Boss!");

        yield return new WaitForSeconds(_bossIntroClipDelay); // Optional delay before boss spawns

        //Play first clip
        if (_bossIntroClip1 != null)
        {
            AudioSource.PlayClipAtPoint(_bossIntroClip1, Camera.main.transform.position);
            yield return new WaitForSeconds(_bossIntroClip1.length);
        }

        //Play second clip
        if (_bossIntroClip2 != null)
        {
            AudioSource.PlayClipAtPoint(_bossIntroClip2, Camera.main.transform.position);
            yield return new WaitForSeconds(_bossIntroClip2.length);
        }



        // wait remaining delay
        float totalClipTime = (_bossIntroClip1?.length ?? 0) + (_bossIntroClip2?.length ?? 0);
        float remainingDelay = _bossSpawnDelay - _bossIntroClipDelay - totalClipTime;
        if (remainingDelay > 0)
            yield return new WaitForSeconds(remainingDelay);

        //Spawn the boss at a Tiny scale
        Vector3 bossSpawnPosition = new Vector3(0, 2f, 0); 
        GameObject boss = Instantiate(_bossPrefab, bossSpawnPosition, Quaternion.identity);
        // Assign the health bar directly to the helmet script
        BossHelmet helmet = boss.GetComponentInChildren<BossHelmet>(true);
        if (helmet != null && _bossHealthBarFill != null)
        {
            helmet.AssignHealthBar(_bossHealthBarFill);
            helmet.SetOrbPrefab(_bossOrbPhase1Prefab);
            Debug.Log("Orb prefab assigned to BossHelmet from SpawnManager.");
        }
        else
        {
            Debug.LogWarning("BossHelmet or HealthBarFill is missing!");
        }

        boss.transform.localScale = Vector3.one * 0.01f;// Very tiny

        //smoothly grow over time
        float elapsed = 0f;
        // Vector3 targetScale = Vector3.one;

        while (elapsed < _bossGrowDuration)
        {
            float t = elapsed / _bossGrowDuration;
            boss.transform.localScale = Vector3.Lerp(Vector3.one * 0.01f, _bossFinalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.localScale = _bossFinalScale; //ensure final scale is exact

        //Move boss downward by _bossDropDistance
        Vector3 startPos = boss.transform.position;
        Vector3 targetPos = startPos - new Vector3(0, _bossDropDistance, 0);
        float dropProgress = 0f;

        while (dropProgress < 1f)
        {
            dropProgress += Time.deltaTime * _bossDropSpeed;
            boss.transform.position = Vector3.Lerp(startPos, targetPos, dropProgress);
            yield return null;
        }
        boss.transform.position = targetPos; //make sure it lands exactly
        _uiManager.UpdateWaveText(-1); // Hide or update to "Boss Fight" if you want

        yield return new WaitForSeconds(_bossHealthBarDelay);

        if(_bossHealthBarFrame != null)
        {
            _bossHealthBarFrame.SetActive(true);
        }
        if (_bossHealthBarFillGO != null)
        {
            _bossHealthBarFillGO.SetActive(true); // ✅ Enable it on boss spawn
        }

        if (!_stopPowerupSpawning)
        {
            Debug.Log("✅ Starting powerup spawner for boss wave.");
            StartCoroutine(SpawnPowerupRoutine());
        }

        yield break; // Exit the coroutine, no more regular waves



    }
    IEnumerator SpawnEnemyWaves()
    {
        yield return new WaitForSeconds(3.0f);// Wait a few seconds before the first wave starts
        while (!_stopEnemySpawning)
        {
            // Calculate how many enemies to spawn this wave
            int regularEnemies = _startRegularEnemyCount + (_currentWave - 1); //Increases the number of regular enemies by 1 more each wave.
            int horizontalEnemies = _startHorizontalEnemyCount + Mathf.FloorToInt((_currentWave - 1) / 2);
            //Increases the number of horizontal enemies every 2 waves.

            for (int i = 0; i < regularEnemies; i++) // Spawn regular enemies in random range location
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);
                //GameObject newEnemy = Instantiate(_enemyprefab, posToSpawn, Quaternion.identity);
                //newEnemy.transform.parent = _enemyContainer.transform;

                float roll = Random.Range(0f, 1f);

                if (roll <= _smartEnemySpawnChance)
                {
                    GameObject smartEnemy = Instantiate(_smartEnemyPrefab, posToSpawn, _smartEnemyPrefab.transform.rotation);
                    smartEnemy.transform.parent = _enemyContainer.transform;
                }
                else if (roll <= _smartEnemySpawnChance + _dodgeEnemySpawnChance)
                {
                    GameObject dodgeEnemy = Instantiate(_dodgeEnemyPrefab, posToSpawn, Quaternion.identity);
                    dodgeEnemy.transform.parent = _enemyContainer.transform;
                }
                else
                {
                    GameObject newEmemy = Instantiate(_enemyprefab, posToSpawn, Quaternion.identity);
                    newEmemy.transform.parent = _enemyContainer.transform;
                }

                yield return new WaitForSeconds(0.5f);// Small delay between spawns
            }
            // Spawn 1 WhiteEnemy per wave
            Vector3 whiteEnemyPos = new Vector3(0, 12f, 0); // It'll decide L or R in its own Start()
            GameObject whiteEnemy = Instantiate(_whiteEnemyPrefab, whiteEnemyPos, Quaternion.identity);
            // Set the WhiteEnemy's parent to the EnemyContainer to keep the Hierarchy organized
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

            //HERE STARTS "WAVE 5" LOGIC
            // If we just finished wave 5 and haven't spawned the boss yet
            if (_currentWave == 5 && !_bossSpawned)
            {
                yield return StartCoroutine(SpawnBossDirectly());
                _stopEnemySpawning = true;

                yield break;
            }
            else
            {
                yield return _uiManager.ShowWaveCountDown(5); // Show countdown before next wave
                _currentWave++;
                _uiManager.UpdateWaveText(_currentWave);
            }


        }
    }
    public void RevealBossEyesUI()
    {
        if (_bossHealthBarFrame != null)
        {
            _bossHealthBarFrame.SetActive(false); // Hide the helmet health bar
            Debug.Log("Evil Alien Health Bar hidden");
        }

        if (_leftEyeHealthBarGO != null)
        {
            _leftEyeHealthBarGO.SetActive(true); // Show left eye health bar
            Debug.Log("Left Eye Health bar revealed");
        }

        if (_rightEyeHealthBarGO != null)
        {
            _rightEyeHealthBarGO.SetActive(true); // Show right eye health bar
            Debug.Log("Right Eye Health bar revealed");
        }
    }




    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);// Wait 3 seconds before starting to spawn power-ups

       while(!_stopPowerupSpawning)// Keep spawning while the player is alive
        {
            // Choose a random X position at the top of the screen
            Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);

            // Roll a random number between 0 and 1 to decide which rarity will spawn
            float roll = Random.Range(0f, 1f);
            Powerup.Rarity selectedRarity;

            if(roll <= 0.6f)
            {
                selectedRarity = Powerup.Rarity.Common; // 60% chance to spawn a Common powerup
            }

            else if (roll <=0.9f)
            {
                selectedRarity = Powerup.Rarity.Uncommon; // 30% chance to spawn an Uncommon powerup
            }

            else
            {
                selectedRarity = Powerup.Rarity.Rare; // 10% chance to spawn a Rare powerup
            }

            // Find all powerups in the array that match the selected rarity
            GameObject[] matchingPowerups = System.Array.FindAll(_powerups, p => p.GetComponent<Powerup>().GetRarity() == selectedRarity);

            if (matchingPowerups.Length >0)
            {
                // Pick a random matching powerup from the list
                int randomIndex = Random.Range(0, matchingPowerups.Length);

                // Spawn the selected powerup at the chosen position
                Instantiate(matchingPowerups[randomIndex], posToSpawn, Quaternion.identity);
            }
            // Wait a random time (between 5 to 10 seconds) before spawning the next powerup
            yield return new WaitForSeconds(Random.Range(5, 10));
        }
    }

    public void OnPlayerDeath()
    {
        _stopPowerupSpawning = true;
        _stopEnemySpawning = true;
    }


}

