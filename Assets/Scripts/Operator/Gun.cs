using System.Threading.Tasks;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int maxAmmo;
    public float ergonomics;
    public float cyclingSpeed;
    public int damage;
    public float velocity;

    [HideInInspector]public bool cycling;
    [HideInInspector]public int ammo;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammo = maxAmmo;
    }

    private void FixedUpdate()
    {
        
    }

    public async void Fire(Vector3 startPosition, Vector3 direction)
    {
        if (ammo == 0)
            return;

        cycling = true;

        ammo--;

        Debug.Log(direction);

        Projectile projectile = ProjectilePool.Instance.PopFromPool();
        projectile.transform.position = startPosition;
        projectile.damage = damage;
        projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        projectile.speed = velocity;
        projectile.lastPosition = startPosition;
        projectile.alive = true;

        await Task.Delay((int)(cyclingSpeed*1000.0f));
        cycling = false;
    }


}
