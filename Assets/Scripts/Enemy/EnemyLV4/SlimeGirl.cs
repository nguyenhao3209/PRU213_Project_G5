using UnityEngine;

public class SlimeGirl : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 5;                // số phát đạn chịu được
    public float flashDuration = 0.1f;       // thời gian nhấp nháy khi trúng
    public Color flashColor = Color.red;     // màu nhấp nháy

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // Hàm gọi khi bị trúng đạn
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(spriteRenderer != null)
            StartCoroutine(Flash());

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    // Coroutine nhấp nháy khi trúng
    private System.Collections.IEnumerator Flash()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // Khi Slime chết
    private void Die()
    {
        // Optional: thêm animation hoặc particle effect
        Destroy(gameObject);
    }
}
