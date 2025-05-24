using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftEyeBehavior : MonoBehaviour
{
    [SerializeField] private GameObject[] _blackHolePrefabs;
    [SerializeField] private float _minX = -16f;
    [SerializeField] private float _maxX = 16f;
    [SerializeField] private float _minY = -9f;
    [SerializeField] private float _maxY = 0f;
    [SerializeField] private float _blackHoleLifeTime = 9f;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBlackHoleAttack()
    {
        

        foreach (GameObject prefab in _blackHolePrefabs)
        {
            Vector3 spawnPos = new Vector3(Random.Range(_minX, _maxX),Random.Range (_minY, _maxY), 0f);

            GameObject bh = Instantiate(prefab, spawnPos, Quaternion.identity);
            Destroy(bh, _blackHoleLifeTime);
        }
        Debug.Log("Black Holes spawned by left eye");
    }
}
