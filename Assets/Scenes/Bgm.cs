using UnityEngine;

public class Bgm : MonoBehaviour
{
    public static Bgm instance;
    
    [Header("BGM Settings")]
    public AudioClip bgmClip; // 재생할 BGM 오디오 클립
    public float volume = 0.5f; // 볼륨 (0.0 ~ 1.0)
    public bool loop = true; // 반복 재생 여부
    
    private AudioSource audioSource;
    
    void Awake()
    {
        // 싱글톤 패턴 - 씬이 바뀌어도 BGM이 계속 재생되도록
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 오브젝트 유지
            
            // AudioSource 컴포넌트 가져오기 또는 추가
            audioSource = GetComponent<AudioSource>();
            if(audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // AudioSource 설정
            audioSource.clip = bgmClip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.playOnAwake = true;
        }
        else
        {
            // 이미 BGM이 있으면 중복 생성 방지
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // BGM 재생
        if(audioSource != null && bgmClip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
    // 볼륨 조절 메서드 (필요시 사용)
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume); // 0.0 ~ 1.0 사이로 제한
        if(audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    
    // BGM 정지 메서드 (필요시 사용)
    public void StopBGM()
    {
        if(audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    // BGM 재생 메서드 (필요시 사용)
    public void PlayBGM()
    {
        if(audioSource != null && bgmClip != null)
        {
            audioSource.Play();
        }
    }
}
