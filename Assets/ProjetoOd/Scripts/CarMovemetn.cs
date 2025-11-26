using UnityEngine;

public class CarMovemetn : MonoBehaviour
{
    public float DeSlow = 0;
    public Vector3 SpawnPlayer = new Vector3(-2.5f, -0.001f, -7.4f);

    public bool DirecaoContraria = false;  // 👈 NOVO

    void Update()
    {
        // 👇 Escolhe a direção (1 normal, -1 invertida)
        float direction = DirecaoContraria ? -1f : 1f;

        transform.Translate(transform.right * Time.deltaTime * DeSlow * direction);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right * direction, out hit, 15))
        {
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

            if (hit.transform.CompareTag("Respawn"))
            {
                transform.position = new Vector3(
                    transform.position.x - Random.Range(120, 150) * direction,
                    transform.position.y,
                    transform.position.z
                );
            }

            if (hit.transform.CompareTag("Player") && DeSlow > 5 &&
                Vector3.Distance(transform.position, hit.transform.position) < 5)
            {
                hit.collider.gameObject.transform.position = SpawnPlayer;
            }

            if (DeSlow < 15 && !hit.transform.CompareTag("Player") &&
                !hit.transform.CompareTag("Respawn") &&
                !hit.transform.CompareTag("Stop Car"))
            {
                DeSlow += 0.1f;
            }
        }
        else
        {
            if (DeSlow < 15)
            {
                DeSlow += 0.1f;
            }
        }
    }
}
