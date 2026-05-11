using UnityEngine;
using UnityEngine.SceneManagement;

public class VoltarCena : MonoBehaviour
{
    public void Voltar()
    {
        // ativa retorno da posição
        PlayerPrefs.SetInt("RetornarPosicao", 1);

        string ultimaCena = PlayerPrefs.GetString("UltimaCena");

        SceneManager.LoadScene(ultimaCena);
    }
}