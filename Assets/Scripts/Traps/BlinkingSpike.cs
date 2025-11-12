using System.Collections;
using UnityEngine;

// Attach this to your spike GameObject to make it appear/disappear on a timer.
public class BlinkingSpike : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float onDuration = 1.5f;
    [SerializeField] private float offDuration = 1.5f;
    [SerializeField] private bool startOn = true;

    [Header("Animator (optional)")]
    [SerializeField] private bool useAnimator = false;
    [SerializeField] private string animatorActiveBool = "Active";

    [Header("References (optional)")]
    [SerializeField] private Collider2D damageCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyDamage enemyDamage; // Your existing damage script (optional)

    private Coroutine loopRoutine;
    private Animator cachedAnimator;

    private void Reset()
    {
        // Auto-wire common components when adding the script
        if (damageCollider == null)
            damageCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyDamage == null)
            enemyDamage = GetComponent<EnemyDamage>();
    }

    private void Awake()
    {
        if (useAnimator)
            cachedAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ApplyState(startOn);
        loopRoutine = StartCoroutine(BlinkLoop(startOn));
    }

    private void OnDisable()
    {
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }
    }

    private IEnumerator BlinkLoop(bool currentState)
    {
        bool state = currentState;
        while (true)
        {
            float wait = state ? onDuration : offDuration;
            yield return new WaitForSeconds(wait);
            state = !state;
            ApplyState(state);
        }
    }

    private void ApplyState(bool isOn)
    {
        if (damageCollider != null)
            damageCollider.enabled = isOn;

        if (enemyDamage != null)
            enemyDamage.enabled = isOn;

        if (spriteRenderer != null)
            spriteRenderer.enabled = isOn;

        if (useAnimator && cachedAnimator != null && !string.IsNullOrEmpty(animatorActiveBool))
            cachedAnimator.SetBool(animatorActiveBool, isOn);
    }
}


