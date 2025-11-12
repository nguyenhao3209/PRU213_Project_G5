using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;
    public Slider healthBar;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.value = currentHealth;

        anim.SetTrigger("Hit");

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1.5f); // boss biến mất sau 1.5 giây
    }
}
