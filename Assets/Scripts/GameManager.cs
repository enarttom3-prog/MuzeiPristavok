using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Сколько поломок сейчас находится на карте
    public int activeBreakdowns = 0;

    // Сколько поломок игрок починил
    public int repairedBreakdowns = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BreakdownCreated()
    {
        activeBreakdowns++;
    }

    public void BreakdownRepaired()
    {
        activeBreakdowns--;
        repairedBreakdowns++;
    }
}
