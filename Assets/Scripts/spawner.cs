using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;

    bool startSpawning;
    bool isSpawning; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && spawnCount < numToSpawn && !isSpawning)
        {
            StartCoroutine(spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true; 

        }
    }
    IEnumerator spawn()
    {
        isSpawning = true;
        yield return new WaitForSeconds(timeBetweenSpawns);

        int spawnInt = Random.Range(0, spawnPos.Length);

        // public static Object Instantiate(Object original, Vector3 position, Quaternion rotation);
        Instantiate(objectToSpawn, spawnPos[spawnInt].position , spawnPos[spawnInt].rotation);

        spawnCount++;
        isSpawning = false;
    }
}
