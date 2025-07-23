using Unity.Netcode;
using UnityEngine;

public class FishSpawner : NetworkBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 5f;
    public Transform[] spawnPoints;
    public GameObject peixeNormalPrefab;
    public GameObject peixeDouradoPrefab;

    private float timer;

    void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnFish();
            timer = 0f;
        }
    }

    void SpawnFish()
    {
        int index = Random.Range(0, spawnPoints.Length);
        GameObject prefab = Random.value < 0.2f ? peixeDouradoPrefab : peixeNormalPrefab; // 20% de chance
        GameObject fish = Instantiate(prefab, spawnPoints[index].position, Quaternion.identity);
        fish.GetComponent<NetworkObject>().Spawn();
    }
    
}
