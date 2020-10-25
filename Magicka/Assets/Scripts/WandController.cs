using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class WandController : MonoBehaviour
{
    public class Projectile
    {
        public GameObject gameObject;
        public Transform transform;
        public float lifeTime;
        public Projectile(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
        }
    }
    [SerializeField] float projectileSpeed = 5;
    [SerializeField] float totalTime = 5;
    [SerializeField] LayerMask collisionMask = default;
    [SerializeField] GameObject fireballPrefab = default;
    [SerializeField] Light2D wandLight;

    Transform poolParent;

    List<Projectile> objectPool = new List<Projectile>();
    List<Projectile> inUsePool = new List<Projectile>();
    RaycastHit2D hitInfo;
    private void Start()
    {
        poolParent = new GameObject("Fireball Pool").transform;
        for (int i = 0; i < 25; i++)
        {
            objectPool.Add(NewObject());
        }
    }

    Projectile NewObject()
    {
        GameObject obj = Instantiate(fireballPrefab, poolParent.transform);
        obj.SetActive(false);
        Projectile projectile = new Projectile(obj);
        return projectile;
    }
    Projectile GetObject()
    {
        Projectile obj = null;
        if (objectPool.Count > 0)
        {
            obj = objectPool[0];
            objectPool.RemoveAt(0);
        }
        else
        {
            obj = NewObject();
        }
        inUsePool.Add(obj);
        return obj;
    }
    public void ReturnObject(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        inUsePool.Remove(projectile);
        objectPool.Add(projectile);
    }

    public void ShootFireball(Vector2 aimDirection)
    {
        if (GetComponentInChildren<ParticleSystem>() != null)
            GetComponentInChildren<ParticleSystem>().Play();

        if (Camera.main.GetComponent<CinemachineImpulseSource>())
            Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        Projectile projectile = GetObject();
        projectile.gameObject.SetActive(true);
        projectile.transform.position = transform.position;
        projectile.transform.right = aimDirection;
        projectile.lifeTime = 0;
    }

    private void Update()
    {

        for (int i = inUsePool.Count - 1; i >= 0; i--)
        {
            Projectile projectile = inUsePool[i];
            Vector3 startPosition = projectile.transform.position;
            projectile.transform.position += projectile.transform.right * projectileSpeed * Time.deltaTime;
            //projectile.transform.position += Physics.gravity * Time.deltaTime;
            projectile.lifeTime += Time.deltaTime;

            hitInfo = Physics2D.Linecast(startPosition, projectile.transform.position, collisionMask.value);

            if (hitInfo && hitInfo.collider.CompareTag("Enemy"))
            {
                if (hitInfo.transform.TryGetComponent(out Damagable damageable))
                {
                    damageable.Damage(1);
                    ReturnObject(projectile);
                }
            }


            if (projectile.lifeTime > totalTime)
            {
                ReturnObject(projectile);
            }
        }
    }

}
