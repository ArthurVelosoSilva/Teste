using UnityEngine;

public class TintaNarrativa : MonoBehaviour
{
    [Header("Configuração")]
    public int valor = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem encostou é o Player
        if (other.CompareTag("Player"))
        {
            // Pega o script unificado do player
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Coletar(valor);
            }

            // Destroi o item após coletar
            Destroy(gameObject);
        }
    }
}