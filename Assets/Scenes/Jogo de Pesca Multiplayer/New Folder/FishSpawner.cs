using Unity.Netcode;
using UnityEngine;

public class FishSpawner : NetworkBehaviour
{
    public GameObject[] fishPrefabs;
    public Transform[] spawnPoints;
    public float tempoSpawn = 5f;
    private float timer;

    void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;
        if (timer >= tempoSpawn)
        {
            timer = 0f;
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        int indexPeixe = Random.Range(0, fishPrefabs.Length);
        int indexPos = Random.Range(0, spawnPoints.Length);

        GameObject peixe = Instantiate(fishPrefabs[indexPeixe], spawnPoints[indexPos].position, Quaternion.identity);
        peixe.GetComponent<NetworkObject>().Spawn();
    }
}
