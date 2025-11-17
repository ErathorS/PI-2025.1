using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LugarVisitadoManager : MonoBehaviourPun
{
    [Header("Grupos de Lugares")]
    public List<LugarVisitado> grupo1;
    public List<LugarVisitado> grupo2;

    private bool grupo1Concluido = false;
    private bool grupo2Concluido = false;

    // Guarda qual player ativou qual lugar
    private Dictionary<(int grupoID, int lugarID), int> ativacoes
        = new Dictionary<(int, int), int>();

    // 🔎 Verifica se jogador já ativou algum lugar no grupo
    public bool JogadorJaAtivouLugarNoGrupo(int playerID, int grupoID)
    {
        foreach (var entry in ativacoes)
        {
            if (entry.Key.grupoID == grupoID && entry.Value == playerID)
                return true;
        }
        return false;
    }

    public void MarcarLugar(int grupoID, int lugarID, int playerID)
    {
        ativacoes[(grupoID, lugarID)] = playerID;

        switch (grupoID)
        {
            case 1:
                VerificarGrupo(grupo1, ref grupo1Concluido);
                break;

            case 2:
                VerificarGrupo(grupo2, ref grupo2Concluido);
                break;
        }
    }

    private void VerificarGrupo(List<LugarVisitado> grupo, ref bool grupoConcluido)
    {
        if (grupoConcluido)
            return;

        bool todosVisitados = true;

        foreach (var lugar in grupo)
        {
            if (!lugar.FoiVisitado())
            {
                todosVisitados = false;
                break;
            }
        }

        if (todosVisitados)
        {
            grupoConcluido = true;

            if (PhotonNetwork.IsMasterClient)
            {
                ProgressaoFaseController prog = FindObjectOfType<ProgressaoFaseController>();
                prog?.LugarVisitadoConcluido();
            }
        }
    }
}
