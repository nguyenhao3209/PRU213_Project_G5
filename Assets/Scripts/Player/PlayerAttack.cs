using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;


    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // ✅ Kết hợp cả hai logic: cooldown, TimeScale và điều kiện tấn công
        bool canAttackInput = Input.GetMouseButton(0);
        bool cooldownReady = cooldownTimer > attackCooldown;
        // Cho phép tấn công ngay cả khi đang nhảy
        bool canAttackState = true;
        bool gameRunning = Time.timeScale > 0;

        if (canAttackInput && cooldownReady && canAttackState && gameRunning)
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        // ✅ Lấy viên đạn trống duy nhất và tái sử dụng
        int fireballIndex = FindFireball();
        GameObject fireball = fireballs[fireballIndex];

        // Đặt vị trí và hướng trước khi kích hoạt
        fireball.transform.position = firePoint.position;
        fireball.SetActive(true);
        fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0; // Nếu tất cả đạn đang bay, dùng lại viên đầu tiên
    }
}
