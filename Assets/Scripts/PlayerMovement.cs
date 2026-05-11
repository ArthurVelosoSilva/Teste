using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer map;
    [SerializeField] private float padding = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;
    public float xPosLastFrame;

    [Header("Vida")]
    public int maxHealth = 150;
    private int currentHealth;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI vidaTexto;
    [SerializeField] private TextMeshProUGUI tintaTexto;
    [SerializeField] private TextMeshProUGUI leitoresTexto;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;

    [Header("Coleta")]
    public int tinta = 0;

    [Header("Leitores")]
    public int leitores = 0;

    [Header("Quebra Roteiro")]
    public int custoQuebraRoteiro = 10;

    [Header("Porta Secreta")]
    [SerializeField] private GameObject portaSecreta;
    [SerializeField] private string cenaPorta = "Final";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        string cenaAtual =
            SceneManager.GetActiveScene().name;

        // =========================
        // VILAREJO = RESET
        // =========================

        if (cenaAtual == "Vilarejo")
        {
            // vida máxima
            currentHealth = maxHealth;

            // zera tinta
            tinta = 0;

            // salva
            PlayerPrefs.SetInt(
                "PlayerHealth",
                currentHealth
            );

            PlayerPrefs.SetInt(
                "PlayerTinta",
                tinta
            );
        }
        else
        {
            // =========================
            // CARREGAR VIDA E TINTA
            // =========================

            currentHealth = PlayerPrefs.GetInt(
                "PlayerHealth",
                maxHealth
            );

            tinta = PlayerPrefs.GetInt(
                "PlayerTinta",
                0
            );
        }

        // =========================
        // CARREGAR LEITORES
        // =========================

        leitores = PlayerPrefs.GetInt(
            "PlayerLeitores",
            0
        );

        AtualizarUIVida();
        AtualizarUITinta();
        AtualizarUILeitores();

        VerificarPortaSecreta();

        // =========================
        // POSIÇÃO SALVA
        // =========================

        // só restaura posição se veio da cena de diálogo
        if (PlayerPrefs.GetInt("RetornarPosicao", 0) == 1)
        {
            if (PlayerPrefs.HasKey("PlayerX"))
            {
                float x = PlayerPrefs.GetFloat("PlayerX");
                float y = PlayerPrefs.GetFloat("PlayerY");

                transform.position = new Vector2(x, y);
            }

            // desativa após usar
            PlayerPrefs.SetInt("RetornarPosicao", 0);
        }
    }

    void Update()
    {
        HandleMovement();
        ClampMovement();
        FlipCharacterX();

        VerificarQuebraRoteiro();
        VerificarPortaSecreta();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
    }

    // =========================
    // MOVIMENTO
    // =========================

    private void HandleMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        bool isWalking = movement != Vector2.zero;

        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);

        if (isWalking)
        {
            lastDirection = movement;

            animator.SetFloat("lastX", lastDirection.x);
            animator.SetFloat("lastY", lastDirection.y);

            UpdateAttackPoint();
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("attack");
            Attack();
        }
    }

    private void FlipCharacterX()
    {
        float input = Input.GetAxis("Horizontal");

        if (input > 0 && transform.position.x > xPosLastFrame)
        {
            spriteRenderer.flipX = false;
        }
        else if (input < 0 && transform.position.x < xPosLastFrame)
        {
            spriteRenderer.flipX = true;
        }

        xPosLastFrame = transform.position.x;
    }

    private void ClampMovement()
    {
        if (map == null) return;

        Bounds bounds = map.bounds;

        float minX = bounds.min.x + padding;
        float maxX = bounds.max.x - padding;

        float minY = bounds.min.y + padding;
        float maxY = bounds.max.y - padding;

        float x = Mathf.Clamp(transform.position.x, minX, maxX);
        float y = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector2(x, y);
    }

    // =========================
    // ATAQUE
    // =========================

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Ogro>()?.TomarDano(attackDamage);
            enemy.GetComponent<Raposa>()?.TomarDano(attackDamage);
            enemy.GetComponent<Dragao>()?.TomarDano(attackDamage);
        }
    }

    private void UpdateAttackPoint()
    {
        float distance = 0.5f;

        attackPoint.localPosition =
            lastDirection * distance;
    }

    // =========================
    // VIDA
    // =========================

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // salva vida
        PlayerPrefs.SetInt(
            "PlayerHealth",
            currentHealth
        );

        AtualizarUIVida();

        Debug.Log(
            "Player levou dano. Vida atual: "
            + currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void AtualizarUIVida()
    {
        if (vidaTexto != null)
        {
            vidaTexto.text =
                "Vida: " + currentHealth;
        }
    }

    // =========================
    // MORTE
    // =========================

    private void Die()
    {
        Debug.Log("Player morreu");

        // reseta vida
        PlayerPrefs.SetInt(
            "PlayerHealth",
            maxHealth
        );

        // reseta tinta
        PlayerPrefs.SetInt(
            "PlayerTinta",
            0
        );

        // reseta leitores
        PlayerPrefs.SetInt(
            "PlayerLeitores",
            0
        );

        // limpa posição salva
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");

        PlayerPrefs.SetInt(
            "RetornarPosicao",
            0
        );

        // vai para GameOver
        SceneManager.LoadScene("GameOver");
    }

    // =========================
    // COLETA
    // =========================

    public void Coletar(int valor)
    {
        tinta += valor;

        // salva tinta
        PlayerPrefs.SetInt(
            "PlayerTinta",
            tinta
        );

        AtualizarUITinta();

        Debug.Log(
            "Tinta Narrativa: " + tinta
        );
    }

    private void AtualizarUITinta()
    {
        if (tintaTexto != null)
        {
            tintaTexto.text =
                "Tinta Narrativa: " + tinta;
        }
    }

    // =========================
    // LEITORES
    // =========================

    private void AtualizarUILeitores()
    {
        if (leitoresTexto != null)
        {
            leitoresTexto.text =
                "Leitores: " + leitores;
        }
    }

    // =========================
    // PORTA SECRETA
    // =========================

    private void VerificarPortaSecreta()
    {
        string cenaAtual =
            SceneManager.GetActiveScene().name;

        if (
            cenaAtual == "Vilarejo" &&
            leitores >= 10000 &&
            portaSecreta != null
        )
        {
            portaSecreta.SetActive(true);
        }
    }

    // =========================
    // QUEBRA ROTEIRO
    // =========================

    private void VerificarQuebraRoteiro()
    {
        // apertou P
        if (Input.GetKeyDown(KeyCode.P))
        {
            string cenaAtual =
                SceneManager.GetActiveScene().name;

            string proximaCena = "";

            // =========================
            // QUAL CENA VAI ABRIR
            // =========================

            if (cenaAtual == "Floresta")
            {
                proximaCena = "Caverna1";
            }
            else if (cenaAtual == "Castelo")
            {
                proximaCena = "Caverna2";
            }
            else if (cenaAtual == "Dragao (Batalha)")
            {
                proximaCena = "Caverna3";
            }

            // se não houver destino
            if (proximaCena == "")
                return;

            // =========================
            // VERIFICA TINTA
            // =========================

            if (tinta >= custoQuebraRoteiro)
            {
                // desconta tinta
                tinta -= custoQuebraRoteiro;

                // ganha leitores
                leitores += 1000;

                // salva tinta
                PlayerPrefs.SetInt(
                    "PlayerTinta",
                    tinta
                );

                // salva leitores
                PlayerPrefs.SetInt(
                    "PlayerLeitores",
                    leitores
                );

                AtualizarUITinta();
                AtualizarUILeitores();

                Debug.Log(
                    "Quebra Roteiro ativada!"
                );

                // troca cena
                SceneManager.LoadScene(
                    proximaCena
                );
            }
            else
            {
                Debug.Log(
                    "Tinta Narrativa insuficiente!"
                );
            }
        }
    }

    // =========================
    // DEBUG
    // =========================

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}