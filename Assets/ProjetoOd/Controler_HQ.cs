using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

[System.Serializable]
public class QuadroHQ
{
    [TextArea(2, 4)]
    public List<string> falas;
}

public class Controler_HQ : MonoBehaviourPun
{
    [Header("Componentes Visuais")]
    public GameObject canvas;
    public TMP_Text falaHQ;

    [Header("Quadros da HQ (ordem importa!)")]
    public GameObject[] quadrosHQ;

    [Header("Falas por Quadro")]
    public List<QuadroHQ> falasPorQuadro = new List<QuadroHQ>();

    private int quadroAtual = 0;
    private int linhaAtual = 0;
    private bool terminou = false;

    void Start()
    {
        if (canvas != null)
            canvas.SetActive(true);

        AtualizarQuadro();
        AtualizarFala();
    }

    void Update()
    {
        if (!photonView.IsMine && !PhotonNetwork.IsMasterClient) return; // Apenas Player 1 controla
        if (terminou) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            photonView.RPC("AvancarDialogoRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void AvancarDialogoRPC()
    {
        if (terminou) return;

        if (quadroAtual >= falasPorQuadro.Count || quadroAtual >= quadrosHQ.Length)
        {
            EncerrarHQ();
            return;
        }

        var falasDoQuadro = falasPorQuadro[quadroAtual].falas;

        if (linhaAtual + 1 < falasDoQuadro.Count)
        {
            linhaAtual++;
            AtualizarFala();
        }
        else
        {
            quadroAtual++;
            linhaAtual = 0;

            if (quadroAtual < quadrosHQ.Length && quadroAtual < falasPorQuadro.Count)
            {
                AtualizarQuadro();
                AtualizarFala();
            }
            else
            {
                EncerrarHQ();
            }
        }
    }

    void EncerrarHQ()
    {
        terminou = true;
        falaHQ.text = "";
        if (canvas != null)
            canvas.SetActive(false);

        //Debug.Log("HQ finalizada e canvas desativado.");
    }

    void AtualizarQuadro()
    {
        for (int i = 0; i < quadrosHQ.Length; i++)
        {
            quadrosHQ[i].SetActive(i == quadroAtual);
        }
    }

    void AtualizarFala()
    {
        if (quadroAtual < falasPorQuadro.Count && linhaAtual < falasPorQuadro[quadroAtual].falas.Count)
            falaHQ.text = falasPorQuadro[quadroAtual].falas[linhaAtual];
        else
            falaHQ.text = "";
    }
}
