using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;
    public float damage = 10f;
    public GameObject impactEffect;

    public void Seek(Transform _target)
    {
        target = _target;
    }
    
    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    // Update is called once per frame
    void Update()
    {
        if(target ==null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFram = speed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFram)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFram, Space.World);
    }

    void HitTarget()
    {
        if(impactEffect != null)
        {
            GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        Enemy enemy = target.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }

}