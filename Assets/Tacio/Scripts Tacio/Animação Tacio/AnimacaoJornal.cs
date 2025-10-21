using UnityEngine;

public class AnimacaoJornal : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    public float velocidadeRotacao = 50f; // Velocidade de rotação (graus por segundo)

    [Header("Configurações de Flutuação")]
    public float amplitude = 0.25f;  // Altura máxima da flutuação
    public float velocidadeFlutuacao = 2f; // Velocidade do movimento para cima e para baixo

    private Vector3 posicaoInicial;

    void Start()
    {
        // Armazena a posição inicial para basear o movimento senoidal
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // 🌀 Rotação suave no eixo Y
        transform.Rotate(Vector3.up * velocidadeRotacao * Time.deltaTime, Space.World);

        // 🌊 Movimento de flutuação usando seno (sobe e desce suavemente)
        float novaAltura = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutuacao) * amplitude;

        // Atualiza apenas o eixo Y
        transform.position = new Vector3(transform.position.x, novaAltura, transform.position.z);
    }
}
