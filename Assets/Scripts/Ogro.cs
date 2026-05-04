using UnityEngine;

public class Ogro : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaDePerseguicao = 5f;

    [Header("Ataque")]
    public float distanciaDeAtaque = 1.2f;
    public float tempoEntreAtaques = 2f;
    public int dano = 1;

    [Header("Vida")]
    public int vidaMaxima = 5;
    private int vidaAtual;

    [Header("Drop")]
    public GameObject itemDrop; // prefab
    [Range(0f, 1f)] public float chanceDrop = 1f; // 100% por padrão

    [Header("Referências")]
    public Transform player;

    private PlayerHealth playerHealth;
    private Vector2 direcao;
    private Animator anim;
    private float tempoUltimoAtaque;

    void Start()
    {
        anim = GetComponent<Animator>();
        vidaAtual = vidaMaxima;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= distanciaDeAtaque)
            Atacar();
        else if (distancia <= distanciaDePerseguicao)
            Perseguir();
        else
            direcao = Vector2.zero;

        AtualizarAnimacao();
    }

    void Perseguir()
    {
        direcao = (player.position - transform.position).normalized;
        transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }

    void Atacar()
    {
        direcao = (player.position - transform.position).normalized;

        if (Time.time >= tempoUltimoAtaque + tempoEntreAtaques)
        {
            tempoUltimoAtaque = Time.time;

            anim.SetTrigger("Attack");
            playerHealth?.TakeDamage(dano);
        }
    }

    void AtualizarAnimacao()
    {
        if (direcao != Vector2.zero)
        {
            anim.SetFloat("MoveX", direcao.x);
            anim.SetFloat("MoveY", direcao.y);
        }

        anim.SetFloat("Speed", direcao.magnitude);
    }

    // 💥 RECEBER DANO
    public void TomarDano(int danoRecebido)
    {
        vidaAtual -= danoRecebido;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    // ☠️ MORTE + DROP
    void Morrer()
    {
        TentarDrop();

        Destroy(gameObject);
    }

    void TentarDrop()
    {
        if (itemDrop == null) return;

        float chance = Random.value;

        if (chance <= chanceDrop)
        {
            Instantiate(itemDrop, transform.position, Quaternion.identity);
        }
    }
}