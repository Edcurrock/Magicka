using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class Damagable : MonoBehaviour
{
    Rigidbody2D rb;
    EnvironmentController environment;
    Renderer renderer;
    [SerializeField] int totalHealth = 5;
    int currentHealth;

    public ParticleSystem explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        environment = FindObjectOfType<EnvironmentController>();
        renderer = GetComponent<Renderer>();
        currentHealth = totalHealth;
    }

    // Update is called once per frame
    public void Damage(int amount)
    {
        if (Camera.main.GetComponent<CinemachineImpulseSource>())
            Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        currentHealth -= amount;

        if (GetComponentInChildren<ParticleSystem>())
            GetComponentInChildren<ParticleSystem>().Play();

        if (currentHealth <= 0)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        DOTween.Kill(renderer.material);

        if (currentHealth <= 0)
            return;

        renderer.material.DOFloat(1, "HitAmount", .05f).OnComplete(() => renderer.material.DOFloat(0, "HitAmount", .15f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && !environment.canUse)
        {
            Damage(10);
            rb.AddForce(Vector2.up, ForceMode2D.Impulse);
        }
    }
}
