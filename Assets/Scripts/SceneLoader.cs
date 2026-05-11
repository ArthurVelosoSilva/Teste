using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void CarregarCena(string nomeCena)
    {
        // Se estiver indo do Menu para Vilarejo
        if (
            SceneManager.GetActiveScene().name == "Menu" &&
            nomeCena == "Vilarejo"
        )
        {
            // zera leitores
            PlayerPrefs.SetInt(
                "PlayerLeitores",
                0
            );
        }

        SceneManager.LoadScene(nomeCena);
    }

    public void SairJogo()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}