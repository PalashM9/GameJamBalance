using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int balance = 0;  

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddBalance(int amount)
    {
        balance += amount;
        Debug.Log("Balance = " + balance);
    }
}
