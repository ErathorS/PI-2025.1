using System.Collections;
using UnityEngine;

public class PostePiscando : MonoBehaviour
{
    public Light posteLuz;
    public float intervalo = 1f;
    public float duracaoPiscar = 0.1f;

    private float timer;
    private bool piscando;
    private bool devePiscar = true;
    private Coroutine coroutinePiscar;

    void Start()
    {
        if (posteLuz == null)
            posteLuz = GetComponentInChildren<Light>();
            
        if (posteLuz == null)
            Debug.LogError($"[PostePiscando] {gameObject.name} não tem Light atribuída!");
    }

    void Update()
    {
        if (!devePiscar) return;
        
        if (!piscando && coroutinePiscar == null)
        {
            timer += Time.deltaTime;

            if (timer >= intervalo)
            {
                coroutinePiscar = StartCoroutine(Piscar());
                timer = 0f;
            }
        }
    }

    IEnumerator Piscar()
    {
        piscando = true;

        // Apaga a luz
        if (posteLuz != null)
            posteLuz.enabled = false;

        yield return new WaitForSeconds(duracaoPiscar);

        // Acende a luz
        if (posteLuz != null)
            posteLuz.enabled = true;

        piscando = false;
        coroutinePiscar = null;
    }

    public void PararDePiscar()
    {
        devePiscar = false;
        
        if (coroutinePiscar != null)
        {
            StopCoroutine(coroutinePiscar);
            coroutinePiscar = null;
        }

        if (posteLuz != null)
            posteLuz.enabled = true;

        piscando = false;
        
        Debug.Log($"[PostePiscando] {gameObject.name} parou de piscar");
    }

    public void VoltarAPiscar()
    {
        devePiscar = true;
        piscando = false;
        timer = 0f;
        coroutinePiscar = null;
    }
}