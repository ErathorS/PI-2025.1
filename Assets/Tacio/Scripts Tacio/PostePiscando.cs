using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PostePiscando : MonoBehaviour
{
    public Light posteLuz;               // arraste a luz do poste aqui (ou Light2D se quiser adaptar)
    public float intervalo = 1f;         // tempo entre cada piscada
    public float duracaoPiscar = 0.1f;   // quanto tempo a luz fica apagada durante a piscada

    private float timer;
    private bool piscando;

    void Start()
    {
        if (posteLuz == null)
            posteLuz = GetComponentInChildren<Light>();  
    }

    void Update()
    {
        if (!piscando)
        {
            timer += Time.deltaTime;

            if (timer >= intervalo)
            {
                StartCoroutine(Piscar());
                timer = 0f;
            }
        }
    }

    IEnumerator Piscar()
    {
        piscando = true;

        // Apaga a luz
        posteLuz.enabled = false;

        yield return new WaitForSeconds(duracaoPiscar);

        // Acende a luz
        posteLuz.enabled = true;

        piscando = false;
    }
}
