using UnityEngine;
using TMPro;
public class buildmanager : MonoBehaviour
{
    public static buildmanager instance;   
    public GameObject standardTurretPrefab;
    public GameObject missileLauncherPrefab;
    public GameObject artilleryPrefab;

    public TMP_Text turretCountText;
    [Header("Turret Costs")]
    public int standardTurretCost = 30;
    public int missileLauncherCost = 50;
    public int artilleryCost = 70;
    
    [Header("Turret Limits")]
    public int maxTurrets = 8; // 최대 설치 가능한 타워 개수
    public float sellRefundRate = 0.5f; // 타워 판매 시 환불 비율 (50%)
    
    private GameObject turretToBuild;
    private int currentTurretCount = 0; // 현재 설치된 타워 개수
    
    // 선택된 타워 관리
    private Node selectedNode = null; // 선택된 타워가 있는 Node

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one buildmanager in scene!");
            return;
        }
        instance = this;
    }

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }
    private void UpdateTurretCountUI()
    {
        if (turretCountText != null)
            turretCountText.text = $" {currentTurretCount} / {maxTurrets}";
    }

    public void SetTurretToBuild(GameObject turret)
    {    
        turretToBuild = turret;
    }
    
    public int GetTurretCost(GameObject turretPrefab)
    {
        if(turretPrefab == standardTurretPrefab)
            return standardTurretCost;
        else if(turretPrefab == missileLauncherPrefab)
            return missileLauncherCost;
        else if(turretPrefab == artilleryPrefab)
            return artilleryCost;
        else
            return 0;
    }

    void Start()
    {
        UpdateTurretCountUI();
    }

    public bool CanBuildTurret()
    {
        return currentTurretCount < maxTurrets;
    }
    
    public void OnTurretBuilt()
    {
        currentTurretCount++;
        UpdateTurretCountUI();
    }
    
    public void OnTurretDestroyed()
    {
        if(currentTurretCount > 0)
            currentTurretCount--;
        UpdateTurretCountUI();
    }
    
    public int GetCurrentTurretCount()
    {
        return currentTurretCount;
    }
    
    public int GetMaxTurrets()
    {
        return maxTurrets;
    }
    
    // 타워를 판매할 때 환불 금액 계산
    public int GetSellRefund(GameObject turretPrefab)
    {
        int cost = GetTurretCost(turretPrefab);
        return Mathf.RoundToInt(cost * sellRefundRate);
    }
    
    // 타워 선택
    public void SelectTurret(Node node)
    {
        if (selectedNode != null)
            selectedNode.SetSelected(false); // 이전 선택 해제

        selectedNode = node;
        selectedNode.SetSelected(true); // 새 선택 노드 
    }
    
    // 선택된 타워 해제
    public void DeselectTurret()
    {
        if (selectedNode != null)
        {
            selectedNode.SetSelected(false); // 선택 해제
            selectedNode = null;
        }
    }
    
    // 선택된 타워 삭제 (X키로 호출)
    public void DeleteSelectedTurret()
    {
        if(selectedNode != null)
        {
            selectedNode.SellTurret();
            selectedNode = null;
        }
    }
    
    void Update()
    {
        // X키를 누르면 선택된 타워 삭제
        if(Input.GetKeyDown(KeyCode.X))
        {
            DeleteSelectedTurret();
        }
        
        // ESC키를 누르면 선택 해제
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectTurret();
            SetTurretToBuild(null); // 건설 선택도 해제
        }
    }
} 
