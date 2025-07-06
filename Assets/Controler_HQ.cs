using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class QuadroHQ
{
    [TextArea(2, 4)]
    public List<string> falas;
}

public class Controler_HQ : MonoBehaviour
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

    void Start()
    {
        canvas.SetActive(true);
        AtualizarQuadro();
        AtualizarFala();
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            AvancarDialogo();
        }
    }

    void AvancarDialogo()
    {
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
                // Fim da HQ
                falaHQ.text = "";
                canvas.SetActive(false);
            }
        }
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
        falaHQ.text = falasPorQuadro[quadroAtual].falas[linhaAtual];
    }
}