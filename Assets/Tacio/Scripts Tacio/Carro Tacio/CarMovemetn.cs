using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float speed = 0f;
    public bool MoverParaFrente = true;

    [Header("Configurações de Raycast")]
    public bool RaycastParaFrente = true;
    public float raycastDistance = 20f;
    public float stoppingDistance = 3f;

    [Header("Referência do Gerenciador")]
    public GerenciadorTransito gerenciadorTransito;

    private Vector3 initialPosition;
    private bool semaforoDetectado = false;
    private bool carroFrenteDetectado = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // CORREÇÃO: Direção do movimento baseada em MoverParaFrente
        float moveDirection = MoverParaFrente ? 1f : -1f;
        Vector3 moveVector = transform.right * moveDirection;

        // CORREÇÃO: Direção do raycast DEVE SER A MESMA do movimento
        float rayDirection = MoverParaFrente ? 1f : -1f;
        Vector3 raycastVector = transform.right * rayDirection;

        // Verifica semáforo e carro da frente
        VerificarSemaforo(raycastVector);
        VerificarCarroFrente(raycastVector);

        // Se semáforo detectado OU carro da frente detectado, para o carro
        if (semaforoDetectado || carroFrenteDetectado)
        {
            speed = 0f;
        }
        else
        {
            // Movimento do carro
            transform.Translate(moveVector * Time.deltaTime * speed);

            // Aceleração natural
            if (speed < 15f)
            {
                speed += 0.1f;
            }
        }
    }

    private void VerificarSemaforo(Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance))
        {
            // Verifica se é um trigger de semáforo
            if (hit.collider.isTrigger && gerenciadorTransito != null)
            {
                // Consulta o gerenciador se deve parar neste semáforo
                if (gerenciadorTransito.DevePararNoSemaforo(hit.collider))
                {
                    semaforoDetectado = true;
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
                    return;
                }
            }
        }

        semaforoDetectado = false;
    }

    private void VerificarCarroFrente(Vector3 direction)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, raycastDistance);

        foreach (RaycastHit hitInfo in hits)
        {
            if (hitInfo.transform == this.transform)
                continue;

            if (hitInfo.transform.CompareTag("Car"))
            {
                CarMovement carroFrente = hitInfo.transform.GetComponent<CarMovement>();

                if (carroFrente != null)
                {
                    float distancia = hitInfo.distance;

                    if (carroFrente.speed == 0f || distancia < stoppingDistance)
                    {
                        carroFrenteDetectado = true;
                        Debug.DrawRay(transform.position, direction * distancia, Color.yellow);
                        return;
                    }
                }
            }
        }

        carroFrenteDetectado = false;
        Debug.DrawRay(transform.position, direction * raycastDistance, Color.green);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            transform.position = initialPosition;
            speed = Random.Range(5f, 15f);
            semaforoDetectado = false;
            carroFrenteDetectado = false;
        }
    }

    // NOVO: Método para debug
    public void DebugEstado()
    {
        Debug.Log($"[CarMovement] {gameObject.name} - " +
            $"Speed: {speed}, " +
            $"MoverParaFrente: {MoverParaFrente}, " +
            $"SemaforoDetectado: {semaforoDetectado}, " +
            $"CarroFrenteDetectado: {carroFrenteDetectado}");
    }
}