using UnityEngine;

public class CarMovemetn : MonoBehaviour
{
    [Header("Config do Carro")]
    public float DeSlow = 0;
    public bool DirecaoContraria = false;

    [Header("Spawn dos Players")]
    public Transform SpawnPlayer1;
    public Transform SpawnPlayer2;

    private Vector3 initialPosition;

    void Start()
    {
        // Salva a posição inicial do carro
        initialPosition = transform.position;
    }

    void Update()
    {
        float direction = DirecaoContraria ? -1f : 1f;

        // Movimento do carro
        transform.Translate(transform.right * Time.deltaTime * DeSlow * direction);

        RaycastHit hit;

        // RAYCAST na direção correta
        if (Physics.Raycast(transform.position, transform.right * direction, out hit, 15))
        {
            // 🛑 Semáforo
            if (hit.transform.CompareTag("Stop Car"))
            {
                if (BotaoInterativo.StateSemaforo)
                {
                    DeSlow = 0;
                }
                else
                {
                    DeSlow = Vector3.Distance(transform.position, hit.transform.position) * 0.5f;
                }
            }

            // 👤 PLAYER
            if (hit.transform.CompareTag("Player"))
            {
                // velocidade alta e muito perto -> atropela
                if (DeSlow > 5 && Vector3.Distance(transform.position, hit.transform.position) < 5)
                {
                    Transform playerHit = hit.transform;

                    // Detecta qual player encostou
                    if (playerHit.name == "Player1" && SpawnPlayer1 != null)
                    {
                        playerHit.position = SpawnPlayer1.position;
                    }
                    else if (playerHit.name == "Player2" && SpawnPlayer2 != null)
                    {
                        playerHit.position = SpawnPlayer2.position;
                    }
                }
            }

            // 🚗 Aceleração natural
            if (DeSlow < 15 &&
                !hit.transform.CompareTag("Player") &&
                !hit.transform.CompareTag("Respawn") &&
                !hit.transform.CompareTag("Stop Car"))
            {
                DeSlow += 0.1f;
            }
        }
        else
        {
            if (DeSlow < 15)
                DeSlow += 0.1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 🔄 Respawn dos carros
        if (other.CompareTag("Respawn"))
        {
            transform.position = initialPosition;
            DeSlow = Random.Range(5f, 15f);
        }

        // 👤 Player bate diretamente no trigger do carro (em vez do Ray)
        if (other.CompareTag("Player"))
        {
            Transform playerHit = other.transform;

            if (playerHit.name == "Player_1" && SpawnPlayer1 != null)
            {
                playerHit.position = SpawnPlayer1.position;
            }
            else if (playerHit.name == "Player_2" && SpawnPlayer2 != null)
            {
                playerHit.position = SpawnPlayer2.position;
            }
        }
    }
}
