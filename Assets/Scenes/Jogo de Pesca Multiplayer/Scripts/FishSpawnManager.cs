using Unity.Netcode;
using UnityEngine;

public class FishSpawnManager : NetworkBehaviour
{
    [Header("Prefabs e Configurações")]
    public GameObject prefabPeixeDourado;    
    public AudioClip somPeixeDourado;       
    private int contadorFalhas = 0;          
    private int falhasPeixeDourado = 5; 
    public static FishSpawnManager Instancia { get; private set; }

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// verifica se deve spawnar peixes
    public void RegistrarFalha()
    {
        if (!IsServer) return;

        contadorFalhas++;
        
        // spawnar peixe dourado
        if (contadorFalhas >= falhasPeixeDourado)
        {
            SpawnarPeixeDourado();
            contadorFalhas = 0; 
        }
    }

    /// peixe dourado em posição aleatória
    private void SpawnarPeixeDourado()
    {
        Vector3 posicaoSpawn = ObterPosicaoSpawnAleatoria();
        SpawnarPeixe(prefabPeixeDourado, posicaoSpawn);
        TocarSomPeixeDouradoClientRpc(posicaoSpawn);
    }

    /// Spawna um peixe aleatório do tipo especificado
    private void SpawnarPeixeAleatorio(GameObject prefabPeixe)
    {
        Vector3 posicaoSpawn = ObterPosicaoSpawnAleatoria();
        SpawnarPeixe(prefabPeixe, posicaoSpawn);
    }

    /// spawna um peixe na rede
    private void SpawnarPeixe(GameObject prefabPeixe, Vector3 posicao)
    {
        GameObject peixe = Instantiate(prefabPeixe, posicao, Quaternion.identity);
        NetworkObject networkObject = peixe.GetComponent<NetworkObject>();
        networkObject.Spawn(); 
    }

    /// posição aleatória para spawn
    private Vector3 ObterPosicaoSpawnAleatoria()
    {
        float x = Random.Range(-5f, 5f);
        float z = Random.Range(-5f, 5f);
        return new Vector3(x, 0f, z);
    }

    /// tocar o som do peixe dourado
    [ClientRpc]
    private void TocarSomPeixeDouradoClientRpc(Vector3 posicao)
    {
        if (somPeixeDourado != null)
        {
            AudioSource.PlayClipAtPoint(somPeixeDourado, posicao);
        }
    }
}