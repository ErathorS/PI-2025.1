using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostePiscando : MonoBehaviour
{
    public Light posteLuz;
    public float intervalo = 1f;
    public float duracaoPiscar = 0.1f;

    private float timer;
    private bool piscando;
    private bool devePiscar = true;

    void Start()
    {
        if (posteLuz == null)
            posteLuz = GetComponentInChildren<Light>();
    }

    void Update()
    {
        if (!devePiscar) return;
        
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

    // Novo método para parar de piscar
    public void PararDePiscar()
    {
        devePiscar = false;
        posteLuz.enabled = true; // Garante que a luz fique acesa
        StopAllCoroutines();
    }

    // Novo método para voltar a piscar (se necessário)
    public void VoltarAPiscar()
    {
        devePiscar = true;
        piscando = false;
        timer = 0f;
    }
}