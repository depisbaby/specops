using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;
    private void Awake()
    {
        Instance = this;
    }

    public int poolSize;
    public GameObject projectilePrefab;

    Queue<Projectile> pool = new Queue<Projectile>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScaleUpThePool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ScaleUpThePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(projectilePrefab);
            Projectile projectile = instance.GetComponent<Projectile>();
            projectile.ResetValues();
            projectile.gameObject.SetActive(false);
            pool.Enqueue(projectile);
        }
    }

    public void ReturnToPool(Projectile projectile)
    {
        projectile.ResetValues();
        projectile.gameObject.SetActive(false);
        pool.Enqueue(projectile);
    }

    public Projectile PopFromPool()
    {
        if(pool.Count > 0)
        {
            Projectile projectile = pool.Dequeue();
            projectile.gameObject.SetActive(true);
            return projectile;
        }

        ScaleUpThePool();
        Projectile _projectile = pool.Dequeue();
        _projectile.gameObject.SetActive(true);
        return _projectile;
    }
}
