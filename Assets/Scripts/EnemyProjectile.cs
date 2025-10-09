using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 5f;
    public GameObject hitEffect;
    
    private Transform target;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    
    void Start()
    {
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        if (hasTarget)
        {
            // Move towards target
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                HitTarget();
            }
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetPosition = target.position;
        hasTarget = true;
    }
    
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if we hit the player
        if (other.CompareTag("Player"))
        {
            /*
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            */
            Debug.Log("EnemyProjectile hit Player");
            HitTarget();
        }
        // Check if we hit the ground or other obstacles
        else if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            HitTarget();
        }
    }
    
    void HitTarget()
    {
        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
}
