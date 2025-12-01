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
        float moveDirection = MoverParaFrente ? 1f : -1f;
        Vector3 moveVector = transform.right * moveDirection;

        float rayDirection = MoverParaFrente ? 1f : -1f;
        Vector3 raycastVector = transform.right * rayDirection;

        VerificarSemaforo(raycastVector);
        VerificarCarroFrente(raycastVector);

        if (semaforoDetectado || carroFrenteDetectado)
        {
            speed = 0f;
        }
        else
        {
            transform.Translate(moveVector * Time.deltaTime * speed);

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
            if (hit.collider.isTrigger && gerenciadorTransito != null)
            {
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

    public void DebugEstado()
    {
        Debug.Log($"[CarMovement] {gameObject.name} - " +
            $"Speed: {speed}, " +
            $"MoverParaFrente: {MoverParaFrente}, " +
            $"SemaforoDetectado: {semaforoDetectado}, " +
            $"CarroFrenteDetectado: {carroFrenteDetectado}");
    }
}