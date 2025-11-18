using UnityEngine;

public class CarMovemet : MonoBehaviour
{
    public float DeSlow = 0;
    public Vector3 SpawnPlayer = new Vector3(-2.5f, -0.001f, -7.4f);

    void Update()
    {
        // Movimento do carro
        transform.Translate(transform.right * Time.deltaTime * DeSlow);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, 15))
        {
            // 🚦 STOP CAR → para 100%
            if (hit.transform.CompareTag("Stop Car"))
            {
                if (BotaoInterativo.StateSemaforo)
                {
                    DeSlow = 0; // semáforo fechado → carro para
                }
                else
                {
                    // sem desaceleração gradual, apenas mantém velocidade normal
                    DeSlow = 15; 
                }
            }

            // 🔁 Respawn do carro
            if (hit.transform.CompareTag("Respawn"))
            {
                transform.position = new Vector3(
                    transform.position.x - Random.Range(120, 150),
                    transform.position.y,
                    transform.position.z
                );
            }

            // 🧍 PLAYER atropelado → somente teleporta, sem desacelerar
            if (hit.transform.CompareTag("Player"))
            {
                hit.collider.gameObject.transform.position = SpawnPlayer;
            }

            // 🚗 Aceleração normal quando não está vendo nada importante
            if (!hit.transform.CompareTag("Player") &&
                !hit.transform.CompareTag("Respawn") &&
                !hit.transform.CompareTag("Stop Car"))
            {
                if (DeSlow < 15)
                    DeSlow += 0.1f;
            }
        }
        else
        {
            // Nada à frente → acelera normalmente
            if (DeSlow < 15)
                DeSlow += 0.1f;
        }
    }
}
