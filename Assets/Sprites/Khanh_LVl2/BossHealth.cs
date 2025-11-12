using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;
    [SerializeField] private Image healthFill; // Kéo thanh Fill (Image) vô đây trong Inspector

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log("🔥 Boss spawned with HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Ngăn HP xuống âm
        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log("💥 Boss hit! Damage taken: " + damage + " | Current HP: " + currentHealth);

        // Cập nhật thanh máu ngay lập tức
        UpdateHealthBar();

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Debug.Log("☠️ Boss defeated!");

            // Gọi lại lần cuối để chắc chắn thanh máu update đầy đủ
            UpdateHealthBar();

            // Xoá boss sau 0.3 giây để UI kịp hiển thị thay đổi
            Destroy(gameObject, 0.3f);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = (float)currentHealth / maxHealth;
            Debug.Log("🩸 Boss health bar updated: " + healthFill.fillAmount);
        }
        else
        {
            Debug.LogWarning("⚠️ HealthFill (Image) chưa được gán trong Inspector!");
        }
    }
}
