using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ManagerGamer : MonoBehaviourPunCallbacks
{
    public static ManagerGamer Instancia { get; private set; }

    [SerializeField] private GameObject _prefabJogador; // arraste o prefab aqui pelo Inspector
    [SerializeField] private Transform[] _spawns;

    private int _jogadoresEmJogo = 0;
    private List<Player_Move> _jogadores;
    public List<Player_Move> Jogadores { get => _jogadores; private set => _jogadores = value; }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        photonView.RPC("AdicionaJogador", RpcTarget.AllBuffered);
        _jogadores = new List<Player_Move>();
    }

    [PunRPC]
    private void AdicionaJogador()
    {
        _jogadoresEmJogo++;
        if (_jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
            CriarJogador();
        }
    }

    private void CriarJogador()
    {
        // Verifica se o prefab está corretamente atribuído
        if (_prefabJogador == null)
        {
            Debug.LogError("O prefab do jogador não está atribuído no Inspector.");
            return;
        }

        // Instancia usando o nome do prefab (Photon exige que esteja na pasta Resources)
        GameObject jogadorObj = PhotonNetwork.Instantiate(_prefabJogador.name, _spawns[Random.Range(0, _spawns.Length)].position, Quaternion.identity);
        Player_Move jogador = jogadorObj.GetComponent<Player_Move>();

        jogador.photonView.RPC("Inicializa", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
}
