using UnityEngine;

public class GerenciadorMissoes : MonoBehaviour
{
    [SerializeField] private bool[] missoesConcluidas;

    public bool TodasMissoesCompletas()
    {
        foreach (bool m in missoesConcluidas)
        {
            if (!m) return false;
        }
        return true;
    }

    public void CompletarMissao(int index)
    {
        if (index >= 0 && index < missoesConcluidas.Length)
            missoesConcluidas[index] = true;
    }
}
