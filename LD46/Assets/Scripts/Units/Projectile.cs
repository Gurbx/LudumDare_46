using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject deathExplosionPrefab;
    [SerializeField] private ParticleSystem trail;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float timeElapsed;
    private float duration;
    private int damage;
    private Vector2Int damagePoint;

    public void Initiaize(Vector2Int startPos, Vector2Int endPos, Vector2Int range, int damage)
    {
        this.startPosition = startPos;
        startPosition.x += 0.5f;
        startPosition.y += 0.5f;
        
        this.targetPosition = endPos;
        targetPosition.x += 0.5f;
        targetPosition.y += 0.5f;

        damagePoint = endPos;

        timeElapsed = 0;
        duration = range.magnitude/6f;
        this.damage = damage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(startPosition, targetPosition, timeElapsed*2/duration);
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            World.instance.DamageObjectAtPoint(damagePoint, damage);
            DeathEffect();
            Destroy(gameObject);
        }
    }

    private void DeathEffect()
    {
        ScreenShakeHandler.AddScreenShake(3, 3, 0.2f);
        trail.transform.parent = transform.parent;
        trail.Stop();
        Destroy(trail, 4f);
        var effect = Instantiate(deathExplosionPrefab, transform);
        effect.transform.parent = transform.parent;
        Destroy(effect, 6f);
    }
}
