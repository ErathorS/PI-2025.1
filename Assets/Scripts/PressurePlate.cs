using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PressurePlate : MonoBehaviourPun
{
    [Header("Objeto a ativar (opcional)")]
    public GameObject objetoParaAtivar;

    [Header("Cores da placa")]
    public Color corDesativada = Color.red;
    public Color corAtivada = Color.green;

    [Header("Movimento da placa")]
    public float movimentoAltura = 0.1f;
    public float velocidadeMovimento = 5f;

    private Vector3 posicaoOriginal;
    private Vector3 posicaoPressionada;
    private Vector3 alvoAtual;

    private HashSet<int> playersSobre = new HashSet<int>();
    private bool caixaPresente = false;

    private Renderer rend;

    void Start()
    {
        posicaoOriginal = transform.position;
        posicaoPressionada = posicaoOriginal - new Vector3(0, movimentoAltura, 0);
        alvoAtual = posicaoOriginal;

        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            rend = GetComponentInChildren<Renderer>();
            Debug.LogWarning("Renderer não estava no objeto principal, buscando em filhos.");
        }

        if (rend != null)
        {
            rend.material.color = corDesativada;
        }
        else
        {
            Debug.LogError("Renderer da placa de pressão não encontrado!");
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, alvoAtual, Time.deltaTime * velocidadeMovimento);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null)
            {
                playersSobre.Add(pv.OwnerActorNr);
                VerificarAtivacao();
            }
        }

        if (other.CompareTag("Box"))
        {
            caixaPresente = true;
            VerificarAtivacao();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null)
            {
                playersSobre.Remove(pv.OwnerActorNr);
                VerificarAtivacao();
            }
        }

        if (other.CompareTag("Box"))
        {
            caixaPresente = false;
            VerificarAtivacao();
        }
    }

    void VerificarAtivacao()
    {
        bool ativar = playersSobre.Count >= 2 || caixaPresente;

        Debug.Log("Verificando ativação: " + ativar);

        photonView.RPC("RPC_AtivarObjeto", RpcTarget.AllBuffered, ativar);
    }

    [PunRPC]
    void RPC_AtivarObjeto(bool ativar)
    {
        Debug.Log("RPC_AtivarObjeto recebido - ativar: " + ativar);

        if (objetoParaAtivar != null)
            objetoParaAtivar.SetActive(ativar);

        alvoAtual = ativar ? posicaoPressionada : posicaoOriginal;

        if (rend != null)
        {
            rend.material.color = ativar ? corAtivada : corDesativada;
        }
    }
}