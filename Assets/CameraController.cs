using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool doMovement = true;
    public float scrollSpeed = 5f;
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;
    
    [Header("Camera Limits")]
    public float minY = 10f;
    public float maxY = 80f;
    public float minX = -50f; // 왼쪽 제한
    public float maxX = 50f;  // 오른쪽 제한
    public float minZ = -50f; // 뒤쪽 제한
    public float maxZ = 50f;  // 앞쪽 제한
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            doMovement = !doMovement;
        }
        if(!doMovement){
            return; 
        }
        
        Vector3 pos = transform.position;
        
        // 앞뒤 이동 (W/S 키 또는 마우스)
        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness){
            pos += Vector3.forward * panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness){
            pos += Vector3.back * panSpeed * Time.deltaTime;
        }
        
        // 좌우 이동 (A/D 키 또는 마우스)
        if(Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness){
            pos += Vector3.right * panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness){
            pos += Vector3.left * panSpeed * Time.deltaTime;
        }
        
        // 스크롤로 높이 조절
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;
        
        // 모든 축에 범위 제한 적용
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        
        transform.position = pos;
 
    }
}
