using UnityEngine;

public class Breakdown : MonoBehaviour
{
    [Header("Repair Animation")]
    public string repairAnimationName = "Repair";

    [Header("Repair Indicator")]
    public GameObject repairIndicator;

    private Animator animator;

    private bool playerNearby;
    private bool repaired;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BreakdownCreated();
        }
    }

    private void Update()
    {
        if (repaired)
            return;

        if (playerNearby && Input.GetKeyDown(KeyCode.Return))
        {
            Repair();
        }
    }

    private void Repair()
    {
        repaired = true;

        // Сразу убираем сигнал о поломке
        if (repairIndicator != null)
        {
            repairIndicator.SetActive(false);
        }

        // Запускаем анимацию ремонта
        if (animator != null)
        {
            animator.Play(repairAnimationName);
        }
        else
        {
            FinishRepair();
        }

        // Учитываем починку
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BreakdownRepaired();
        }
    }

    // Вызывается Animation Event
    // в конце анимации Repair
    public void FinishRepair()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
