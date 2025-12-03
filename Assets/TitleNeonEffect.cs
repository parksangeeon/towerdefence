using UnityEngine;
using TMPro;

public class TitleNeonEffect : MonoBehaviour
{
    [Header("Neon Effect Settings")]
    public TMP_Text titleText; // OTSD 텍스트
    public Color baseColor = Color.cyan; // 기본 색상
    public Color glowColor = Color.white; // 빛나는 색상
    public float glowSpeed = 2f; // 빛나는 속도
    public float glowIntensity = 0.5f; // 빛나는 강도 (0.0 ~ 1.0)
    
    private float time = 0f;
    private Color originalColor;
    
    void Start()
    {
        if(titleText == null)
        {
            titleText = GetComponent<TMP_Text>();
        }
        
        if(titleText != null)
        {
            originalColor = titleText.color;
        }
    }
    
    void Update()
    {
        if(titleText == null)
            return;
        
        // 시간에 따라 빛나는 효과 생성
        time += Time.deltaTime * glowSpeed;
        
        // 사인파를 사용해서 부드럽게 빛나게 함
        float glow = (Mathf.Sin(time) + 1f) * 0.5f; // 0.0 ~ 1.0 사이 값
        
        // 색상 보간 (기본 색상 <-> 빛나는 색상)
        Color currentColor = Color.Lerp(baseColor, glowColor, glow * glowIntensity);
        
        // 텍스트 색상 적용
        titleText.color = currentColor;
        
        // 추가 효과: 아웃라인 색상도 변경 (더 네온 느낌)
        if(titleText.fontSharedMaterial != null)
        {
            // Outline 효과가 있다면 색상도 변경 가능
        }
    }
}

