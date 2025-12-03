using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color selectedColor;
    public Vector3 positionOffset;

    private Renderer rend;
    private Color startColor;
    private bool isSelected = false;   // ➜ 클릭으로 선택 여부 관리
    public GameObject rangeIndicatorPrefab; // 사거리 원 프리팹
    private GameObject rangeIndicatorInstance; // 인스턴스
    public GameObject turret;
    buildmanager buildmanager;

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

        // 건설 모드일 때만 호버 색 표시 (원하면 이 부분도 빼도 됨)
        if (buildmanager.GetTurretToBuild() == null)
            return;

        // 이미 선택된 노드는 호버 색으로 바꾸지 않음
        if (isSelected)
            return;

        rend.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        // 선택된 상태면 선택 색 유지, 아니면 원래 색
        rend.material.color = isSelected ? selectedColor : startColor;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (buildmanager == null)
        {
            Debug.LogError("BuildManager is null!");
            return;
        }

        // 1) 이미 타워가 설치되어 있으면 클릭으로 '선택'
        if (turret != null)
        {
            try
            {
                if (turret.gameObject == null)
                {
                    turret = null;
                }
                else
                {
                    // 여기서만 '선택' 발생
                    buildmanager.SelectTurret(this);
                    Debug.Log("Turret selected! Press X to delete.");
                    return;
                }
            }
            catch
            {
                turret = null;
            }
        }

        // 2) 타워 설치 로직은 그대로
        if (buildmanager.GetTurretToBuild() == null)
        {
            Debug.Log("No turret selected! Please select a turret first.");
            return;
        }

        if (!buildmanager.CanBuildTurret())
        {
            Debug.Log("Maximum turret limit reached! (" +
                      buildmanager.GetCurrentTurretCount() + "/" +
                      buildmanager.GetMaxTurrets() + ")");
            return;
        }

        GameObject turretToBuild = buildmanager.GetTurretToBuild();
        int turretCost = buildmanager.GetTurretCost(turretToBuild);

        if (Gold.instance == null)
        {
            Debug.LogError("Gold instance is null!");
            return;
        }

        if (!Gold.instance.SpendGold(turretCost))
        {
            Debug.Log("Not enough gold to build turret! Need: " + turretCost);
            return;
        }

        turret = (GameObject)Instantiate(
            turretToBuild,
            transform.position + positionOffset,
            transform.rotation
        );

        buildmanager.OnTurretBuilt();
        Debug.Log("Turret built successfully! Gold spent: " + turretCost +
                  " (" + buildmanager.GetCurrentTurretCount() + "/" +
                  buildmanager.GetMaxTurrets() + ")");

        buildmanager.SetTurretToBuild(null);
    }

    public void SetSelected(bool isSelected)
    {
        if (rend == null) return;
        rend.material.color = isSelected ? selectedColor : startColor;

        // 사거리 표시 관리
        if (isSelected && turret != null && rangeIndicatorPrefab != null)
        {
            float range = 0f;

            // Turret 또는 MissileTurret 컴포넌트에서 range 값 가져오기
            Turret turretComp = turret.GetComponent<Turret>();
            if (turretComp != null)
            {
                range = turretComp.range;
            }
            else
            {
                MissileTurret missileComp = turret.GetComponent<MissileTurret>();
                if (missileComp != null)
                {
                    range = missileComp.range;
                }
            }

            if (range > 0f)
            {
                rangeIndicatorInstance = Instantiate(
                    rangeIndicatorPrefab,
                    turret.transform.position,
                    Quaternion.Euler(90f, 0f, 0f), // x축 90도 회전
                    turret.transform
                );
                float scale = range * 2f;
                rangeIndicatorInstance.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else
        {
            if (rangeIndicatorInstance != null)
            {
                Destroy(rangeIndicatorInstance);
                rangeIndicatorInstance = null;
            }
        }
    }
    public void SellTurret()
    {
        if (turret == null)
            return;

        GameObject turretPrefab = null;
        if (buildmanager.standardTurretPrefab != null &&
            turret.name.Contains(buildmanager.standardTurretPrefab.name))
        {
            turretPrefab = buildmanager.standardTurretPrefab;
        }
        else if (buildmanager.missileLauncherPrefab != null &&
                 turret.name.Contains(buildmanager.missileLauncherPrefab.name))
        {
            turretPrefab = buildmanager.missileLauncherPrefab;
        }
        else if (buildmanager.artilleryPrefab != null &&
                 turret.name.Contains(buildmanager.artilleryPrefab.name))
        {
            turretPrefab = buildmanager.artilleryPrefab;
        }

        if (turretPrefab != null)
        {
            int refund = buildmanager.GetSellRefund(turretPrefab);

            if (Gold.instance != null)
            {
                Gold.instance.AddGold(refund);
            }

            Destroy(turret);
            turret = null;
            buildmanager.OnTurretDestroyed();
            buildmanager.DeselectTurret(); // 선택 해제

            Debug.Log("Turret sold! Refund: " + refund +
                      " (" + buildmanager.GetCurrentTurretCount() + "/" +
                      buildmanager.GetMaxTurrets() + ")");
        }
        else
        {
            Destroy(turret);
            turret = null;
            buildmanager.OnTurretDestroyed();
            buildmanager.DeselectTurret();
            Debug.Log("Turret removed! (No refund - unknown type)");
        }
    }
}