using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float startSpeed = 10f;
    public float startHealth = 100f;
    public int goldReward = 10; // 적 처치 시 획득하는 골드
    
    [HideInInspector]
    public float speed;
    
    private float health;
    private Transform target;
    private int wavepointIndex = 0;
    
    void Start()
    {
        speed = startSpeed;
        health = startHealth;
        target = Waypoints.points[0];
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        
        if(health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // 적 처치 시 골드 획득
        if(Gold.instance != null)
        {
            Gold.instance.AddGold(goldReward);
        }
        
        Destroy(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 dir = (target.position - transform.position);
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3. Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWaypoint();
        }
    }
    private void GetNextWaypoint()
    {
        if(wavepointIndex >= Waypoints.points.Length - 1)
        {
            ReachedEnd();
            return;
        }
        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];
    }
    
    void ReachedEnd()
    {
        if(Life.instance != null)
        {
            Life.instance.LoseLife();
        }
        Destroy(gameObject);
    }
}
