using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    [System.Serializable]

    public enum BulletPattern {
        DirectionFacing, Seeking, Stationary, FollowSpawner, Rain
    }

    public BulletPattern bulletPattern;

    [Header("Projectile Attributes")]
    public float damage;
    public float projectileSpeed;
    public bool timeBasedProjectile;
    public float lifetime;
    public bool piercing;

    [Header("Seeking Attributes")]
    public float rateOfTargetUpdate;

    [Header("Other Attributes")]
    public Vector3 rotation;
    public bool warnPlayer;

    [Header("On Projectile Hit")]
    public float aoeSize;
    public bool stops;

    [Header("On Projectile Expiry")]
    public GameObject[] spawns;

    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Vector3 normDirection;
    [HideInInspector]
    public float targetUpdateTimer;
    [HideInInspector]
    public Transform spawner;
    [HideInInspector]
    public string affectsTag;
    [HideInInspector]
    public bool contact;

    void Start() {
        lifetime += Time.time;
    }

    void Update() {
        if (!contact || !stops) {
            if (bulletPattern == BulletPattern.Seeking)
                if (targetUpdateTimer < Time.time) {
                    targetUpdateTimer = Time.time + rateOfTargetUpdate;
                    normDirection = Vector3.Normalize(target.position - transform.position);
                }

            switch (bulletPattern) {
                case BulletPattern.DirectionFacing:
                case BulletPattern.Seeking:
                    transform.position += normDirection * projectileSpeed;
                    break;

                case BulletPattern.FollowSpawner:
                    transform.position = spawner.position;
                    break;

                case BulletPattern.Rain:
                    transform.position -= new Vector3(0, projectileSpeed, 0);
                    break;
            }
        }

        if (rotation != Vector3.zero)
            transform.eulerAngles += rotation;

        if (warnPlayer) { //Show marking.

        }

        if (timeBasedProjectile)
            if (Time.time > lifetime)
                ProjectileExpired();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag(affectsTag) || collision.transform.CompareTag("Floor")) {
            contact = true;
            if (aoeSize > 0) {
                Collider[] inAoe;
                inAoe = Physics.OverlapSphere(collision.transform.position, aoeSize);

                foreach (Collider caughtInAoe in inAoe)
                    if (caughtInAoe.transform.CompareTag(affectsTag))
                        caughtInAoe.transform.GetComponent<UnitBase>().health -= damage;
            } else
                if (collision.transform.CompareTag(affectsTag))
                collision.transform.GetComponent<UnitBase>().health -= damage;

            if (!piercing) 
                ProjectileExpired();            
        }
    }

    void ProjectileExpired() {
        foreach (GameObject spawn in spawns)
            Instantiate(spawn, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
