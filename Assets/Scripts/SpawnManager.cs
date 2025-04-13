using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour

{
    [SerializeField]private GameObject _enemyprefab;
    [SerializeField]private GameObject _enemyContainer;
    [SerializeField]private GameObject[] _powerups;
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
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

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-17.0f, 17.0f), 12f, 0);
            int randomPowerUP = Random.Range(0, 4);
            Instantiate(_powerups[randomPowerUP], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5, 15));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}

