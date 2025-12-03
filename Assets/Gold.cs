using TMPro;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public static Gold instance;
    
    [Header("Gold Settings")]
    public int startGold = 60;
    private int currentGold;
    
    [Header("UI")]
    public TMP_Text goldText;
    
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one Gold in scene!");
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        currentGold = startGold;
        UpdateGoldText();
    }
    
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldText();
    }
    
    public bool SpendGold(int amount)
    {
        if(currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldText();
            return true;
        }
        else
        {
            Debug.Log("Not enough gold! Need: " + amount + ", Have: " + currentGold);
            return false;
        }
    }
    
    public int GetGold()
    {
        return currentGold;
    }
    
    void UpdateGoldText()
    {
        if(goldText != null)
        {
            goldText.text = " " + currentGold.ToString();
        }
    }
}
