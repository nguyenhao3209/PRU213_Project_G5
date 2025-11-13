using UnityEngine;
using UnityEngine.UI; // Thêm dòng này
public class GalamothAI : MonoBehaviour
{
    // Sử dụng enum để quản lý trạng thái, sạch sẽ và dễ hiểu hơn
    private enum State
    {
        Sleeping,
        Fighting,
        Dead
    }
    private State currentState;
[Header("UI")]
public Slider healthBarSlider;
    [Header("Health Settings")]
    public int maxHealth = 3000;
    private int currentHealth;

    [Header("Core References")]
    public Transform player;
    public Animator anim;
public Collider2D sleepTriggerZone; // Thêm dòng này

    [Header("Staff Smash Attack (Tầm gần)")]
    public float attackRange = 3f;
    public int staffDamage = 50;
    public Transform staffAttackPoint;
    public float staffAttackArea = 2f;
    public float staffAttackCooldown = 5f;
    private float staffTimer;
    [Header("Orb Attack (Tầm xa)")]
    public GameObject orbPrefab;
    public Transform orbSpawnPoint;
    public float orbAttackCooldown = 3f;
    private float orbTimer;

    void Start()
    {
        currentState = State.Sleeping; // Bắt đầu ở trạng thái ngủ
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (healthBarSlider != null)
    {
        healthBarSlider.gameObject.SetActive(false);
    }
    }

    void Update()
    {
        // Chỉ chạy logic chiến đấu khi boss ở trạng thái Fighting
        if (currentState != State.Fighting)
        {
            return;
        }

        // Đếm ngược thời gian hồi chiêu
        staffTimer -= Time.deltaTime;
        orbTimer -= Time.deltaTime;

        // Tính khoảng cách tới người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Logic AI: Ưu tiên đập trượng nếu người chơi ở trong tầm
        if (distanceToPlayer <= attackRange && staffTimer <= 0f)
        {
            anim.SetTrigger("StartStaffSmash");
            staffTimer = staffAttackCooldown; // Reset timer
        }
        else if (orbTimer <= 0f) // Nếu không thì bắn cầu
        {
            anim.SetTrigger("StartOrbAttack");
            orbTimer = orbAttackCooldown; // Reset timer
        }
    }

    // Hàm này được Unity tự động gọi khi người chơi đi vào vùng Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == State.Sleeping && other.CompareTag("Player"))
        {
            WakeUp();
        }
    }

    void WakeUp()
{
    currentState = State.Fighting;
    anim.SetTrigger("PlayerInRange");

    // Hiện thanh máu khi boss thức
    if (healthBarSlider != null)
    {
        healthBarSlider.gameObject.SetActive(true);
        healthBarSlider.value = 1; // Đảm bảo thanh máu đầy khi bắt đầu
    }
if (sleepTriggerZone != null)
{
    sleepTriggerZone.enabled = true;
}
    // ... (vô hiệu hóa trigger) ...
}

    // --- CÁC HÀM ĐƯỢC GỌI BẰNG ANIMATION EVENT ---
// Hàm này được GameManager gọi để reset boss
public void ResetBoss()
{
    // 1. Trở về trạng thái ngủ
    currentState = State.Sleeping;

    // 2. Hồi đầy máu
    currentHealth = maxHealth;

    // 3. Ẩn thanh máu
    if (healthBarSlider != null)
    {
        healthBarSlider.gameObject.SetActive(false);
    }

    // 4. Bật lại trigger (nếu bạn đã tắt nó)
    // (Giả sử BoxCollider2D thứ hai là trigger)
    if (sleepTriggerZone != null)
{
    sleepTriggerZone.enabled = true;
}

    // 5. Kích hoạt lại animation ngủ (hoặc Idle)
    anim.Play("Sleeping"); // Đổi "Sleeping" thành tên state ngủ của bạn
}
    public void DealStaffDamage()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(staffAttackPoint.position, staffAttackArea);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                Debug.Log("Player trúng đòn đập trượng!");
                hitPlayer.GetComponent<AlucardController>().TakeDamage(staffDamage);
            }
        }
    }

    public void FireProjectile()
    {
        Instantiate(orbPrefab, orbSpawnPoint.position, Quaternion.identity);
    }

    // --- CÁC HÀM KHÁC ---
public void TakeDamage(int damage)
{
    if (currentState == State.Dead) return;

    currentHealth -= damage;

    // GỌI HÀM CẬP NHẬT THANH MÁU
    UpdateHealthBar();

    if (currentHealth <= 0)
    {
        Die();
    }
}
    
       void UpdateHealthBar()
{
    if (healthBarSlider != null)
    {
        // Tính toán tỉ lệ máu còn lại (từ 0.0 đến 1.0)
        float healthPercentage = (float)currentHealth / (float)maxHealth;

        // Gán giá trị đó vào thanh trượt
        healthBarSlider.value = healthPercentage;
    }
}
    

    void Die()
{
    currentState = State.Dead;
    anim.SetTrigger("Die");

    // Ẩn thanh máu khi boss chết
    if (healthBarSlider != null)
    {
        healthBarSlider.gameObject.SetActive(false);
    }

    // ... (code vô hiệu hóa boss) ...
}

    // Vẽ Gizmo trong Editor để dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        if (staffAttackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(staffAttackPoint.position, staffAttackArea);
    }
}