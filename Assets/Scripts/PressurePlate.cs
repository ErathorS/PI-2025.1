using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PressurePlate : MonoBehaviourPun
{
    [Header("Objeto a ativar (opcional)")]
    public GameObject objetoParaAtivar;

    [Header("Cores da placa")]
    public Color corNormal = Color.gray;
    public Color corAtivada = Color.green;

    private Renderer rend;
    private HashSet<int> objetosNaPlaca = new HashSet<int>();

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = corNormal;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Está colidino");
        if (!photonView.IsMine) return;

        GameObject other = collision.gameObject;

        // Verifica se é o personagem (tag "Player")
        if (other.CompareTag("Player"))
        {
            int id = other.GetInstanceID();

            if (!objetosNaPlaca.Contains(id))
            {
                objetosNaPlaca.Add(id);
                VerificarEstadoPlaca();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (!photonView.IsMine) return;

        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            int id = other.GetInstanceID();

            if (objetosNaPlaca.Contains(id))
            {
                objetosNaPlaca.Remove(id);
                VerificarEstadoPlaca();
            }
        }
    }

    void VerificarEstadoPlaca()
    {
        bool ativada = objetosNaPlaca.Count > 0;

        rend.material.color = ativada ? corAtivada : corNormal;

        if (objetoParaAtivar != null)
            objetoParaAtivar.SetActive(ativada);
    }
}
