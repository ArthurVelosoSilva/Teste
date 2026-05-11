using UnityEngine;

public class Ogro : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaDePerseguicao = 5f;

    [Header("Ataque")]
    public float distanciaDeAtaque = 3f;
    public float tempoEntreAtaques = 2f;
    public int dano = 2;

    [Header("Vida")]
    public int vidaMaxima = 10;
    private int vidaAtual;

    [Header("Drop")]
    public GameObject itemDrop;

    [Range(0f, 1f)]
    public float chanceDrop = 1f;

    [Header("Referências")]
    public Transform player;

    private Player playerScript;
    private Vector2 direcao;
    private Animator anim;
    private float tempoUltimoAtaque;

    void Start()
    {
        anim = GetComponent<Animator>();

        vidaAtual = vidaMaxima;

        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(
            transform.position,
            player.position
        );

        // atacar
        if (distancia <= distanciaDeAtaque)
        {
            Atacar();
        }
        // perseguir
        else if (distancia <= distanciaDePerseguicao)
        {
            Perseguir();
        }
        // parado
        else
        {
            direcao = Vector2.zero;
        }

        AtualizarAnimacao();
    }

    // =========================
    // MOVIMENTO
    // =========================

    void Perseguir()
    {
        direcao = (
            player.position - transform.position
        ).normalized;

        transform.Translate(
            direcao * velocidade * Time.deltaTime,
            Space.World
        );
    }

    // =========================
    // ATAQUE
    // =========================

    void Atacar()
    {
        direcao = (
            player.position - transform.position
        ).normalized;

        if (Time.time >= tempoUltimoAtaque + tempoEntreAtaques)
        {
            tempoUltimoAtaque = Time.time;

            // animação
            anim.SetTrigger("Attack");

            // dano
            playerScript?.TakeDamage(dano);
        }
    }

    // =========================
    // ANIMAÇÃO
    // =========================

    void AtualizarAnimacao()
    {
        if (direcao != Vector2.zero)
        {
            anim.SetFloat("MoveX", direcao.x);
            anim.SetFloat("MoveY", direcao.y);
        }

        anim.SetFloat("Speed", direcao.magnitude);
    }

    // =========================
    // RECEBER DANO
    // =========================

    public void TomarDano(int danoRecebido)
    {
        vidaAtual -= danoRecebido;

        Debug.Log("Ogro tomou dano! Vida: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    // =========================
    // MORTE
    // =========================

    void Morrer()
    {
        TentarDrop();

        Destroy(gameObject);
    }

    // =========================
    // DROP
    // =========================

    void TentarDrop()
    {
        if (itemDrop == null) return;

        float chance = Random.value;

        if (chance <= chanceDrop)
        {
            Instantiate(
                itemDrop,
                transform.position,
                Quaternion.identity
            );
        }
    }
}