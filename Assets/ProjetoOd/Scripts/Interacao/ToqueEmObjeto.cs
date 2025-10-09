using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ToqueEmObjeto : MonoBehaviourPun
{
    private GameObject objetoTocavelAtual;

    private Button botaoInteragir;
    private GameObject painelInteracao;

    private void Start()
    {
        if (!photonView.IsMine) return;

        string tagCanvas = (photonView.Owner.ActorNumber == 1) ? "CanvasP1" : "CanvasP2";
        GameObject canvas = GameObject.FindGameObjectWithTag(tagCanvas);

        if (canvas != null)
        {
            painelInteracao = canvas.transform.Find("BotaoInteragir")?.gameObject;
            if (painelInteracao != null)
            {
                botaoInteragir = painelInteracao.GetComponent<Button>();
                painelInteracao.SetActive(false);
                botaoInteragir.onClick.AddListener(InteragirComObjeto);
            }
            else
            {
                Debug.LogError("Botão de interação não encontrado no Canvas!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("BotaoInterativo"))
        {
            objetoTocavelAtual = other.gameObject;
            painelInteracao?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("BotaoInterativo") && other.gameObject == objetoTocavelAtual)
        {
            objetoTocavelAtual = null;
            painelInteracao?.SetActive(false);
        }
    }

    private void InteragirComObjeto()
    {
        if (objetoTocavelAtual != null)
        {
            objetoTocavelAtual.GetComponent<BotaoInterativo>()?.ExecutarAcao();
            painelInteracao?.SetActive(false); // opcional: oculta botão após interação
        }
    }
}
