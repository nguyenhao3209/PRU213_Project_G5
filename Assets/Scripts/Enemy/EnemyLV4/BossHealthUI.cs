using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Health bossHealth;   // Gán script Health.cs của Boss
    [SerializeField] private Image fillImage;     // Gán hình thanh máu (HealthBar_Fill)
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (bossHealth != null)
        {
           float targetFill = bossHealth.currentHealth / bossHealth.StartingHealth;
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * 5f);
        }

        if (mainCam != null)
            transform.LookAt(transform.position + mainCam.transform.forward);
    }
}
