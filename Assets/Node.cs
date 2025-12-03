using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Vector3 positionOffset;
    private Renderer rend;
    private Color startColor;
    public GameObject turret;
    buildmanager buildmanager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        buildmanager = buildmanager.instance;
    }
    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if(buildmanager.GetTurretToBuild() == null)
        {
            return;
        }
        rend.material.color = hoverColor;
    }
    void OnMouseExit(){
        rend.material.color = startColor;
    }
    void OnMouseDown(){
        if (EventSystem.current.IsPointerOverGameObject())
            return;
            
        if(buildmanager == null)
        {
            Debug.LogError("BuildManager is null!");
            return;
        }
        
        // 타워가 이미 설치되어 있으면 선택만 하기
        if(turret != null)
        {
            // Unity에서 파괴된 오브젝트는 참조가 남아있을 수 있으므로 실제 존재 여부 확인
            try
            {
                if(turret.gameObject == null)
                {
                    turret = null;
                }
                else
                {
                    // 타워가 있으면 선택만 하기 (X키로 삭제)
                    buildmanager.SelectTurret(this);
                    Debug.Log("Turret selected! Press X to delete.");
                    return;
                }
            }
            catch
            {
                // 오브젝트가 파괴된 경우 참조 정리
                turret = null;
            }
        }
        
        // 타워를 설치하려고 할 때
        if(buildmanager.GetTurretToBuild() == null)
        {
            Debug.Log("No turret selected! Please select a turret first.");
            return;
        }
        
        // 최대 타워 개수 체크
        if(!buildmanager.CanBuildTurret())
        {
            Debug.Log("Maximum turret limit reached! (" + buildmanager.GetCurrentTurretCount() + "/" + buildmanager.GetMaxTurrets() + ")");
            return;
        }
    
        GameObject turretToBuild = buildmanager.GetTurretToBuild();
        int turretCost = buildmanager.GetTurretCost(turretToBuild);
        
        // 골드 확인 및 소모
        if(Gold.instance == null)
        {
            Debug.LogError("Gold instance is null!");
            return;
        }
        
        if(!Gold.instance.SpendGold(turretCost))
        {
            Debug.Log("Not enough gold to build turret! Need: " + turretCost);
            return;
        }
        
        turret = (GameObject)Instantiate(turretToBuild, transform.position + positionOffset, transform.rotation);
        buildmanager.OnTurretBuilt(); // 타워 개수 증가
        Debug.Log("Turret built successfully! Gold spent: " + turretCost + " (" + buildmanager.GetCurrentTurretCount() + "/" + buildmanager.GetMaxTurrets() + ")");
        
        // 포탑 건설 후 선택 해제 (선택사항)
        buildmanager.SetTurretToBuild(null);
    }
    
    public void SellTurret()
    {
        if(turret == null)
            return;
        
        // 타워 프리팹 찾기 (Turret 컴포넌트가 있다고 가정)
        // 실제로는 타워 오브젝트에서 어떤 타워인지 확인해야 함
        // 간단하게 buildmanager의 프리팹들과 비교
        GameObject turretPrefab = null;
        if(buildmanager.standardTurretPrefab != null && turret.name.Contains(buildmanager.standardTurretPrefab.name))
        {
            turretPrefab = buildmanager.standardTurretPrefab;
        }
        else if(buildmanager.missileLauncherPrefab != null && turret.name.Contains(buildmanager.missileLauncherPrefab.name))
        {
            turretPrefab = buildmanager.missileLauncherPrefab;
        }
        else if(buildmanager.artilleryPrefab != null && turret.name.Contains(buildmanager.artilleryPrefab.name))
        {
            turretPrefab = buildmanager.artilleryPrefab;
        }
        
        if(turretPrefab != null)
        {
            int refund = buildmanager.GetSellRefund(turretPrefab);
            
            // 골드 환불
            if(Gold.instance != null)
            {
                Gold.instance.AddGold(refund);
            }
            
            // 타워 파괴
            Destroy(turret);
            turret = null;
            buildmanager.OnTurretDestroyed(); // 타워 개수 감소
            buildmanager.DeselectTurret(); // 선택 해제
            
            Debug.Log("Turret sold! Refund: " + refund + " (" + buildmanager.GetCurrentTurretCount() + "/" + buildmanager.GetMaxTurrets() + ")");
        }
        else
        {
            // 타워 종류를 찾을 수 없으면 그냥 파괴 (환불 없음)
            Destroy(turret);
            turret = null;
            buildmanager.OnTurretDestroyed();
            buildmanager.DeselectTurret(); // 선택 해제
            Debug.Log("Turret removed! (No refund - unknown type)");
        }
    }
}

