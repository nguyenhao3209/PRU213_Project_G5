using UnityEngine;
using System.Collections; // Cần thiết cho Coroutine

public class AlucardController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float moveSpeed = 8f;
    private float moveInput;
    private bool isFacingRight = true;

    [Header("Jumping")]
    public float jumpForce = 16f;
    public Transform groundCheck; // Vị trí để kiểm tra đất
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround; // Layer của mặt đất
    private bool isGrounded;

    [Header("Backdash")]
    public float backdashSpeed = 20f;
    public float backdashDuration = 0.2f;
    public float backdashCooldown = 1f;
    private bool isBackdashing = false;
    private float backdashCooldownTimer;

    [Header("Attack")]
    public float attackCooldown = 0.5f; // Thời gian hồi chiêu
    private float attackTimer;          // Biến đếm ngược
    private bool isAttacking = false;   // Biến cờ kiểm soát trạng thái tấn công

    [Header("Attack Hitbox & Damage")]
    public Transform attackPoint;      // Kéo GameObject AttackPoint vào đây
    public float attackRange = 0.5f;   // Bán kính vùng tấn công
    public LayerMask enemyLayers;      // Layer của kẻ địch (Gán cho Galamoth layer "Enemy")
    public int attackDamage = 20;      // Sát thương gây ra


    [Header("Combat (Health/Damage)")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    
    [Header("Invincibility")]
    public float invincibilityDuration = 1f; // Thời gian bất tử sau khi trúng đòn
    private bool isInvincible = false;


    // ---------------------------------------------------------------- //
    //                      UNITY LIFECYCLE METHODS                     //
    // ---------------------------------------------------------------- //

    void Awake()
    {
        // Lấy các component cần thiết
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Nếu đã chết, không làm gì cả
        if (isDead) return;

        // Nếu đang lướt lùi hoặc đang tấn công, hạn chế một số hành động
        if (isBackdashing || isAttacking)
        {
            // Cập nhật các timer
            backdashCooldownTimer -= Time.deltaTime;
            attackTimer -= Time.deltaTime;
            UpdateAnimations(); // Vẫn cập nhật animation
            return; // Thoát khỏi Update để không nhận input di chuyển
        }

        // --- NHẬN INPUT ---
        moveInput = Input.GetAxisRaw("Horizontal"); // Dùng GetAxisRaw để nhân vật dừng ngay lập tức

        // Kiểm tra có đang đứng trên mặt đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // Cập nhật các timer
        backdashCooldownTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        // --- XỬ LÝ HÀNH ĐỘNG ---

        // 1. Tấn công
        if (Input.GetButtonDown("Fire1") && attackTimer <= 0)
        {
            StartAttack();
        }
        
        // 2. Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 3. Backdash
        if (Input.GetButtonDown("Fire2") && backdashCooldownTimer <= 0) // Fire2 thường là chuột phải
        {
            StartBackdash();
        }

        // Cập nhật tất cả animation
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // Nếu đã chết, đang lướt lùi hoặc đang tấn công, không di chuyển
        if (isDead || isBackdashing || isAttacking)
        {
            return;
        }

        // Áp dụng lực di chuyển
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Lật mặt nhân vật
        if (!isFacingRight && moveInput > 0)
        {
            Flip();
        }
        else if (isFacingRight && moveInput < 0)
        {
            Flip();
        }
    }

    // ---------------------------------------------------------------- //
    //                      HELPER & COMBAT METHODS                     //
    // ---------------------------------------------------------------- //

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown; // Reset thời gian hồi chiêu
        anim.SetTrigger("Attack"); // Kích hoạt animation tấn công

        // Gọi hàm kiểm tra va chạm sau một khoảng thời gian (khi animation vung kiếm tới giữa)
        Invoke("CheckAttackHitbox", 0.15f); // Tinh chỉnh thời gian này cho khớp animation

        // Sau khi animation tấn công gần kết thúc, reset trạng thái
        Invoke("EndAttack", 0.3f); // Tinh chỉnh thời gian này cho khớp animation
    }

    void CheckAttackHitbox()
    {
        if (attackPoint == null) return;

        // Tạo một vòng tròn kiểm tra va chạm tại attackPoint
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Tìm component GalamothAI trên kẻ địch
            GalamothAI boss = enemy.GetComponent<GalamothAI>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
            }

            // Sinh ra hiệu ứng tia lửa/va chạm (nếu có)
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void StartBackdash()
    {
        isBackdashing = true;
        backdashCooldownTimer = backdashCooldown;

        rb.gravityScale = 0; // Tạm thời tắt trọng lực
        
        // Lực lướt lùi sẽ ngược với hướng đang nhìn
        float dashDirection = isFacingRight ? -1 : 1;
        rb.velocity = new Vector2(backdashSpeed * dashDirection, 0);

        anim.SetTrigger("Backdash"); // Kích hoạt animation lướt lùi

        StartCoroutine(BackdashCoroutine());
    }

    IEnumerator BackdashCoroutine()
    {
        yield return new WaitForSeconds(backdashDuration);
        isBackdashing = false;
        rb.gravityScale = 4; // Trả lại trọng lực (thay 4 bằng giá trị gốc của bạn)
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hurt");
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
        
        // Vô hiệu hóa nhân vật
        this.enabled = false; // Tắt script này
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // Không bị tác động bởi vật lý nữa
        GetComponent<Collider2D>().enabled = false;
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        // Có thể thêm hiệu ứng nhấp nháy ở đây (ví dụ: tắt/mở SpriteRenderer)
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    // ---------------------------------------------------------------- //
    //                      ANIMATION & GIZMOS                        //
    // ---------------------------------------------------------------- //

    void UpdateAnimations()
    {
        // Truyền tốc độ di chuyển vào Animator
        // Dùng Mathf.Abs để giá trị luôn dương
        anim.SetFloat("moveSpeed", Mathf.Abs(moveInput)); 

        // Animation nhảy/rơi
        anim.SetBool("isJumping", !isGrounded);
    }

    // Vẽ Gizmo trong Editor để dễ dàng căn chỉnh
    void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn kiểm tra đất
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Vẽ vòng tròn tấn công
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}