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

    private Dictionary<(int grupoID, int lugarID), int> ativacoes = new Dictionary<(int, int), int>();

    private ProgressaoFaseController progressoController;

    void Awake()
    {
        ativacoes = new Dictionary<(int, int), int>();
    }

    void Start()
    {
        Debug.Log("[LugarVisitadoManager] Start iniciado");

        progressoController = ProgressaoFaseController.instancia;
        if (progressoController == null)
        {
            progressoController = FindObjectOfType<ProgressaoFaseController>();
        }

        if (progressoController == null)
        {
            Debug.LogError("[LugarVisitadoManager] ProgressaoFaseController não encontrado!");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            VerificarEAtualizarProgresso();
        }
    }

    public void MarcarLugar(int grupoID, int lugarID, int playerID)
    {
        Debug.Log($"[LugarVisitadoManager] MarcarLugar chamado - Grupo: {grupoID}, Lugar: {lugarID}, Jogador: {playerID}");

        // Verificações de segurança
        if (ativacoes == null) 
        {
            ativacoes = new Dictionary<(int, int), int>();
        }

        // 🔴 CORREÇÃO: Verificar se jogador já ativou outro lugar no MESMO grupo
        if (JogadorJaAtivouLugarNoGrupo(playerID, grupoID))
        {
            Debug.Log($"[LugarVisitadoManager] ❌ Jogador {playerID} JÁ ativou outro lugar no grupo {grupoID}. IMPOSSÍVEL chegar aqui!");
            return;
        }

        // Verificar se este lugar específico já foi ativado
        if (ativacoes.ContainsKey((grupoID, lugarID)))
        {
            Debug.Log($"[LugarVisitadoManager] Lugar {grupoID}-{lugarID} já foi ativado anteriormente pelo jogador {ativacoes[(grupoID, lugarID)]}");
            return;
        }

        // Adicionar ao dicionário
        ativacoes[(grupoID, lugarID)] = playerID;
        
        Debug.Log($"[LugarVisitadoManager] ✅ Lugar marcado - Grupo: {grupoID}, Lugar: {lugarID}, Jogador: {playerID}");

        // Sincronizar com outros jogadores
        if (photonView != null)
        {
            photonView.RPC("RPC_SincronizarLugar", RpcTarget.Others, grupoID, lugarID, playerID);
        }

        // Verificar progresso do grupo
        VerificarProgressoGrupo(grupoID);
    }

    // 🔴 CORREÇÃO: Método para verificar se jogador já ativou lugar no grupo
    public bool JogadorJaAtivouLugarNoGrupo(int playerID, int grupoID)
    {
        if (ativacoes == null)
        {
            Debug.LogError("[LugarVisitadoManager] Dicionário 'ativacoes' não inicializado!");
            return false;
        }

        foreach (var entry in ativacoes)
        {
            if (entry.Key.grupoID == grupoID && entry.Value == playerID)
            {
                Debug.Log($"[LugarVisitadoManager] 🔍 Jogador {playerID} já ativou o lugar {entry.Key.lugarID} no grupo {grupoID}");
                return true;
            }
        }
        
        Debug.Log($"[LugarVisitadoManager] 🔍 Jogador {playerID} NÃO ativou nenhum lugar no grupo {grupoID}");
        return false;
    }

    [PunRPC]
    private void RPC_SincronizarLugar(int grupoID, int lugarID, int playerID)
    {
        Debug.Log($"[LugarVisitadoManager] Cliente sincronizando lugar - Grupo: {grupoID}, Lugar: {lugarID}");

        if (ativacoes == null)
        {
            ativacoes = new Dictionary<(int, int), int>();
        }

        if (!ativacoes.ContainsKey((grupoID, lugarID)))
        {
            ativacoes[(grupoID, lugarID)] = playerID;
            Debug.Log($"[LugarVisitadoManager] ✅ Lugar sincronizado no cliente");
        }
    }

    private void VerificarProgressoGrupo(int grupoID)
    {
        Debug.Log($"[LugarVisitadoManager] Verificando progresso do grupo {grupoID}");

        List<LugarVisitado> grupo = grupoID == 1 ? grupo1 : grupo2;
        ref bool grupoConcluido = ref (grupoID == 1 ? ref grupo1Concluido : ref grupo2Concluido);

        if (grupo == null)
        {
            Debug.LogError($"[LugarVisitadoManager] Grupo {grupoID} é nulo!");
            return;
        }

        if (grupoConcluido)
        {
            Debug.Log($"[LugarVisitadoManager] Grupo {grupoID} já estava concluído");
            return;
        }

        // Verificar se TODOS os lugares do grupo foram visitados
        bool todosVisitados = true;
        HashSet<int> jogadoresNoGrupo = new HashSet<int>();

        foreach (var lugar in grupo)
        {
            if (lugar != null && !lugar.FoiVisitado())
            {
                todosVisitados = false;
                break;
            }
            else if (lugar != null && lugar.FoiVisitado())
            {
                // Encontrar qual jogador ativou este lugar
                foreach (var ativacao in ativacoes)
                {
                    if (ativacao.Key.grupoID == grupoID && ativacao.Key.lugarID == lugar.lugarID)
                    {
                        jogadoresNoGrupo.Add(ativacao.Value);
                        break;
                    }
                }
            }
        }

        Debug.Log($"[LugarVisitadoManager] Grupo {grupoID} - Todos visitados: {todosVisitados}, Jogadores únicos: {jogadoresNoGrupo.Count}");

        // Grupo só é concluído se TODOS os lugares foram visitados
        if (todosVisitados && !grupoConcluido)
        {
            grupoConcluido = true;

            Debug.Log($"[LugarVisitadoManager] ✅✅✅ GRUPO {grupoID} CONCLUÍDO! " +
                     $"Todos os {grupo.Count} lugares foram visitados por {jogadoresNoGrupo.Count} jogadores diferentes. Notificando progresso...");

            // Notificar progresso
            if (progressoController != null)
            {
                progressoController.LugarVisitadoConcluido();
                Debug.Log($"[LugarVisitadoManager] ✅ Progresso de lugar notificado com sucesso!");
            }
            else
            {
                Debug.LogError("[LugarVisitadoManager] ProgressaoFaseController não encontrado!");
            }
        }
    }

    public void VerificarEAtualizarProgresso()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("[LugarVisitadoManager] Verificando e atualizando progresso de todos os grupos...");
        
        VerificarProgressoGrupo(1);
        VerificarProgressoGrupo(2);
    }

    // 🔴 MÉTODO PARA DEBUG
    public void DebugEstadoAtual()
    {
        Debug.Log($"[LugarVisitadoManager] === DEBUG LUGARES ===");
        Debug.Log($"ProgressoController: {(progressoController != null ? "✅ OK" : "❌ NULO")}");
        Debug.Log($"Grupo1 Concluído: {grupo1Concluido}");
        Debug.Log($"Grupo2 Concluído: {grupo2Concluido}");
        Debug.Log($"Ativações registradas: {ativacoes?.Count}");
        
        if (ativacoes != null)
        {
            // Agrupar por jogador e grupo para debug
            Dictionary<int, List<(int grupo, int lugar)>> ativacoesPorJogador = new Dictionary<int, List<(int, int)>>();
            
            foreach (var ativacao in ativacoes)
            {
                int jogador = ativacao.Value;
                if (!ativacoesPorJogador.ContainsKey(jogador))
                {
                    ativacoesPorJogador[jogador] = new List<(int, int)>();
                }
                ativacoesPorJogador[jogador].Add((ativacao.Key.grupoID, ativacao.Key.lugarID));
            }

            foreach (var jogador in ativacoesPorJogador)
            {
                Debug.Log($"  Jogador {jogador.Key}:");
                foreach (var lugar in jogador.Value)
                {
                    Debug.Log($"    - Grupo {lugar.grupo}, Lugar {lugar.lugar}");
                }
            }
        }

        // Verificar estado atual dos grupos
        Debug.Log($"--- ESTADO DOS GRUPOS ---");
        VerificarEstadoGrupo(1, grupo1);
        VerificarEstadoGrupo(2, grupo2);
        Debug.Log($"=========================");
    }

    private void VerificarEstadoGrupo(int grupoID, List<LugarVisitado> grupo)
    {
        if (grupo == null)
        {
            Debug.Log($"Grupo {grupoID}: ❌ NULO");
            return;
        }

        int visitados = 0;
        HashSet<int> jogadoresNoGrupo = new HashSet<int>();

        foreach (var lugar in grupo)
        {
            if (lugar != null && lugar.FoiVisitado())
            {
                visitados++;
                
                // Encontrar jogador que ativou este lugar
                foreach (var ativacao in ativacoes)
                {
                    if (ativacao.Key.grupoID == grupoID && ativacao.Key.lugarID == lugar.lugarID)
                    {
                        jogadoresNoGrupo.Add(ativacao.Value);
                        break;
                    }
                }
            }
        }

        Debug.Log($"Grupo {grupoID}: {visitados}/{grupo.Count} lugares visitados por {jogadoresNoGrupo.Count} jogadores");
    }
    // 🔴 NOVO: Método para verificar se jogador pode ativar lugar
    public bool JogadorPodeAtivarLugar(int playerID, int grupoID)
    {
        return !JogadorJaAtivouLugarNoGrupo(playerID, grupoID);
    }
}