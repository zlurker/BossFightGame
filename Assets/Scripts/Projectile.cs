using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public bool lookAtTarget;
    public bool piercing;
    public bool healing;

    [Header("Seeking Attributes")]
    public float rateOfTargetUpdate;

    [Header("Other Attributes")]
    public Vector3 rotation;
    public Vector3 expand;
    public bool warnPlayer;

    [Header("On Projectile Hit")]
    public float aoeSize;
    public bool stops;
    public UnitBase.UnitStats statusEffectCaused;

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
    //[HideInInspector]
    public string affectsTag;
    [HideInInspector]
    public bool contact;

    void Start() {
        lifetime += Time.time;
        if (healing)
            damage *= -1;
    }

    void Update() {
        if (rotation != Vector3.zero)
            transform.eulerAngles += rotation;

        if (expand != Vector3.zero)
            transform.localScale += expand;

        if (lookAtTarget)
            transform.LookAt(target);

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


        if (warnPlayer) { //Show marking.

        }

        if (timeBasedProjectile)
            if (Time.time > lifetime)
                ProjectileExpired();
    }

    void OnCollisionEnter(Collision collision) {
        UnitBase inst = null;
        UnitBase.UnitStats temp;
        if (collision.transform.CompareTag(affectsTag) || collision.transform.CompareTag("Floor")) {
            contact = true;

            if (collision.transform.CompareTag(affectsTag)) {
                inst = collision.transform.GetComponent<UnitBase>();
            }

            if (aoeSize > 0) {
                Collider[] inAoe;
                inAoe = Physics.OverlapSphere(collision.transform.position, aoeSize);

                foreach (Collider caughtInAoe in inAoe)
                    if (inst) {
                        inst.DamageUnit(damage);
                        if (statusEffectCaused.stats.Length > 0) {
                            temp = statusEffectCaused;
                            temp.lifetime += Time.time;

                            inst.unitStat.Add(temp);                           
                        }
                    }
            } else {
                if (inst) {
                    inst.DamageUnit(damage);
                    if (statusEffectCaused.stats.Length > 0) {
                        temp = statusEffectCaused;
                        temp.lifetime += Time.time;

                        inst.unitStat.Add(temp);                     
                    }
                }
            }

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
