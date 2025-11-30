using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingTravel : MonoBehaviour
{
    [Header("Referências")]
    public Image barra;                // Image Filled
    public RectTransform aviao;        // Avião
    public RectTransform pontoInicial; // Onde o avião começa
    public RectTransform pontoFinal;   // Onde ele termina

    [Header("Configuração")]
    public float duracao = 4f;         // Tempo para encher a barra
    public string cenaDestino = "MinhaCena";

    private float tempo;

    void Start()
    {
        tempo = 0f;
        barra.fillAmount = 0f;
    }

    void Update()
    {
        tempo += Time.deltaTime;
        
        // Progresso de 0 a 1 em 4 segundos
        float t = Mathf.Clamp01(tempo / duracao);

        // Atualiza a barra
        barra.fillAmount = t;

        // Move o avião conforme a barra
        aviao.position = Vector3.Lerp(pontoInicial.position, pontoFinal.position, t);

        // Quando terminar → troca de cena
        if (t >= 1f)
        {
            SceneManager.LoadScene(cenaDestino);
        }
    }
}
