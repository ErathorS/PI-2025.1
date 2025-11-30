using UnityEngine;

public class CarMovemetn : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float DeSlow = 0;
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
        // Direção do movimento
        float moveDirection = MoverParaFrente ? 1f : -1f;
        Vector3 moveVector = transform.right * moveDirection;
        
        // Direção do raycast (independente do movimento)
        float rayDirection = RaycastParaFrente ? 1f : -1f;
        Vector3 raycastVector = transform.right * rayDirection;

        // Verifica semáforo e carro da frente
        VerificarSemaforo(raycastVector);
        VerificarCarroFrente(raycastVector);

        // Se semáforo detectado OU carro da frente detectado, para o carro
        if (semaforoDetectado || carroFrenteDetectado)
        {
            DeSlow = 0;
        }
        else
        {
            // Movimento do carro
            transform.Translate(moveVector * Time.deltaTime * DeSlow);

            // Aceleração natural
            if (DeSlow < 15)
            {
                DeSlow += 0.1f;
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
                CarMovemetn carroFrente = hitInfo.transform.GetComponent<CarMovemetn>();
                
                if (carroFrente != null)
                {
                    float distancia = hitInfo.distance;
                    
                    if (carroFrente.DeSlow == 0 || distancia < stoppingDistance)
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
            DeSlow = Random.Range(5f, 15f);
            semaforoDetectado = false;
            carroFrenteDetectado = false;
        }
    }
}