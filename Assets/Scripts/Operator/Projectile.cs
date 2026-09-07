using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask blockingLayers;
    public bool alive;
    public float speed;
    public int damage;
    public Vector3 lastPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void ResetValues()
    {
        alive = default;
        speed = default;
        damage = default;
        lastPosition = default;
    }

    void Move()
    {
        if (!alive)
            return;

        transform.position = transform.position + transform.forward * speed * Time.deltaTime;

        RaycastHit hit;
        if (Physics.Linecast(lastPosition, transform.position, out hit, blockingLayers))
        {
            //Debug.Log(hit.collider.gameObject.name);
            Hit(hit);
        }


        lastPosition = transform.position;
    }

    private void Hit(RaycastHit hit)
    {
        OperatorHitbox hitBox = hit.collider.gameObject.GetComponent<OperatorHitbox>();
        if(hitBox != null)
        {
            hitBox._operator.ProjectileHit(damage);
        }

        ProjectilePool.Instance.ReturnToPool(this);
    }
}
