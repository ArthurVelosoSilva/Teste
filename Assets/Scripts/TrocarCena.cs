using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocarCena : MonoBehaviour
{
    [Header("Cena")]
    public string nomeDaCena;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // verifica se é o player
        if (other.CompareTag("Player"))
        {
            // procura inimigos vivos na layer Enemy
            GameObject[] inimigos =
                GameObject.FindGameObjectsWithTag("Enemy");

            // se não houver inimigos
            if (inimigos.Length == 0)
            {
                SceneManager.LoadScene(nomeDaCena);
            }
            else
            {
                Debug.Log(
                    "Derrote todos os inimigos primeiro!"
                );
            }
        }
    }
}