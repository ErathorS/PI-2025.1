using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotaoInterativo : MonoBehaviour
{
    public GameObject Parada;
    public static bool StateSemaforo = false;
    private void Start()
    {
        StateSemaforo = false;
    }
    public void ExecutarAcao()
    {
        Debug.Log("Ação do objeto interativo executada: " + name);
        if (gameObject.name == "Semaforo") 
        {
            switch (StateSemaforo) 
            {
                case false:
                    print("ligando");
                    Parada.SetActive(true);
                    StateSemaforo = true;
                    break;
                case true:
                    print("Desligando");
                    Parada.SetActive(false);
                    StateSemaforo = false;
                    break;
            }
        }
    }
}
