using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MagoDialogo : MonoBehaviour
{
    public GameObject textoPressioneR;
    public string nomeCenaDialogo = "Dialogo Mago";

    private bool playerPerto = false;

    private void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.R))
        {
            // salva posição do player
            PlayerPrefs.SetFloat("PlayerX", GameObject.FindGameObjectWithTag("Player").transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", GameObject.FindGameObjectWithTag("Player").transform.position.y);

            // salva cena atual
            PlayerPrefs.SetString("UltimaCena", SceneManager.GetActiveScene().name);

            SceneManager.LoadScene(nomeCenaDialogo);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
            textoPressioneR.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
            textoPressioneR.SetActive(false);
        }
    }
}