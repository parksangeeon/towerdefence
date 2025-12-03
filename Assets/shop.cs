using UnityEngine;

public class shop : MonoBehaviour
{ 
    buildmanager buildmanager;    
    void Start()
    {
        buildmanager = buildmanager.instance;
    }
    public void PurchaseStandardTurret(){
        if(buildmanager == null)
        {
            Debug.LogError("BuildManager is null in shop!");
            return;
        }
        if(buildmanager.standardTurretPrefab == null)
        {
            Debug.LogError("Standard Turret Prefab is not assigned!");
            return;
        }
        buildmanager.SetTurretToBuild(buildmanager.standardTurretPrefab);
        Debug.Log("Standard Turret selected!");
    }
    public void PurchaseMissileLauncher(){
        if(buildmanager == null)
        {
            Debug.LogError("BuildManager is null in shop!");
            return;
        }
        if(buildmanager.missileLauncherPrefab == null)
        {
            Debug.LogError("Missile Launcher Prefab is not assigned!");
            return;
        }
        buildmanager.SetTurretToBuild(buildmanager.missileLauncherPrefab);
        Debug.Log("Missile Launcher selected!");
    }
    public void PurchaseArtillery(){
        if(buildmanager == null)
        {
            Debug.LogError("BuildManager is null in shop!");
            return;
        }
        if(buildmanager.artilleryPrefab == null)
        {
            Debug.LogError("Artillery Prefab is not assigned!");
            return;
        }
        buildmanager.SetTurretToBuild(buildmanager.artilleryPrefab);
        Debug.Log("Artillery selected!");
    }
    
}
