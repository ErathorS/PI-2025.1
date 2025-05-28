using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class ToqueEmObjeto : MonoBehaviour
{
 public GameObject botaoInteragir; // Botão que aparece quando estiver perto
    private GameObject objetoTocavelAtual;

    void Start()
    {
        botaoInteragir.SetActive(false);
        botaoInteragir.GetComponent<Button>().onClick.AddListener(AoClicarBotao);
    }

    void AoClicarBotao()
    {
        if (objetoTocavelAtual != null)
        {
            objetoTocavelAtual.GetComponent<BotaoInterativo>()?.ExecutarAcao();
        }

        botaoInteragir.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BotaoInterativo"))
        {
            objetoTocavelAtual = other.gameObject;
            botaoInteragir.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BotaoInterativo"))
        {
            if (other.gameObject == objetoTocavelAtual)
            {
                botaoInteragir.SetActive(false);
                objetoTocavelAtual = null;
            }
        }
    }
}

