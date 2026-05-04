using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer map;
    [SerializeField] private float padding = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;
    public float xPosLastFrame;

    [Header("Vida")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;

    [Header("Coleta")]
    public int tinta = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleMovement();
        ClampMovement();
        FlipCharacterX();
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
        }
    }

    private void UpdateAttackPoint()
    {
        float distance = 0.5f;
        attackPoint.localPosition = lastDirection * distance;
    }

    // =========================
    // VIDA
    // =========================

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player levou dano. Vida atual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player morreu");
        Destroy(gameObject);
    }

    // =========================
    // COLETA (TintaNarrativa)
    // =========================

    public void Coletar(int valor)
    {
        tinta += valor;
        Debug.Log("Tinta: " + tinta);
    }

    // =========================
    // GIZMOS (DEBUG)
    // =========================

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}