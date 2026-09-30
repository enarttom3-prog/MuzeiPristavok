using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] points;

    public float moveSpeed = 2f;
    public float maxstopTime = 5f;
    public float minstopTime = 1f;

    [Header("Breakdowns")]
    public GameObject breakdownPrefab;

    // Только эти точки могут создавать поломки
    public Transform[] breakdownPoints;

    // Вероятность создания поломки при остановке
    [Range(0f, 1f)]
    public float breakdownChance = 0.5f;

    private int currentPoint;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool waiting;

    private float lastMoveX;
    private float lastMoveY;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (waiting || points.Length == 0)
            return;

        Vector2 direction =
            points[currentPoint].position - transform.position;

        float distance = direction.magnitude;

        direction.Normalize();

        transform.position +=
            (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (direction != Vector2.zero)
        {
            lastMoveX = direction.x;
            lastMoveY = direction.y;
        }

        animator.SetFloat("MoveX", lastMoveX);
        animator.SetFloat("MoveY", lastMoveY);
        animator.SetFloat("Speed", 1f);

        if (distance < 0.1f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint()
    {
        waiting = true;

        animator.SetFloat("Speed", 0f);

        // NPC стоит в точке
        yield return new WaitForSeconds(
            Random.Range(minstopTime, maxstopTime)
        );

        // После ожидания проверяем,
        // является ли эта точка точкой поломки
        TryCreateBreakdown(points[currentPoint]);

        // Переходим к следующей точке
        currentPoint++;

        if (currentPoint >= points.Length)
        {
            currentPoint = 0;
        }

        waiting = false;
    }

    private void TryCreateBreakdown(Transform currentPointTransform)
    {
        // Если prefab не назначен
        if (breakdownPrefab == null)
            return;

        // Проверяем, входит ли текущая точка
        // в список точек для поломок
        if (!IsBreakdownPoint(currentPointTransform))
            return;

        // Проверяем вероятность
        if (Random.value > breakdownChance)
            return;

        // Проверяем, нет ли уже поломки в этой точке
        if (currentPointTransform.childCount > 0)
            return;

        // Создаём поломку дочерним объектом точки
        GameObject breakdown = Instantiate(
            breakdownPrefab,
            currentPointTransform.position,
            Quaternion.identity,
            currentPointTransform
        );

    }

    private bool IsBreakdownPoint(Transform point)
    {
        if (breakdownPoints == null)
            return false;

        foreach (Transform breakdownPoint in breakdownPoints)
        {
            if (breakdownPoint == point)
                return true;
        }

        return false;
    }
}
