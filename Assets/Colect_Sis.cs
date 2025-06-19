using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Colect_Sis : MonoBehaviour
{   
    public int To_Collect = 0;
    public static int Cur_Collect = 0;

    public TextMeshProUGUI TextCount;
    // Update is called once per frame
    void Update()
    {
        TextCount.text = Cur_Collect + "/" + To_Collect + " Coletaveis";
    }
    private void OnTriggerEnter(Collider other)
    {
        print("Ass");
        if (other.gameObject.tag == "Coletaveis") 
        {
            Destroy(other.gameObject);
            Cur_Collect++;
        }
        
    }
}
