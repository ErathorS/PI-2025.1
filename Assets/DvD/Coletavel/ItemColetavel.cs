using UnityEngine;
using Photon.Pun;

public class ItemColetavel : MonoBehaviourPun
{
  public float rotacaoVelocidade = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotacaoVelocidade * Time.deltaTime);
    }
}