using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBase : MonoBehaviour {

    public enum ModifierValueType {
        Value, Percentage
    }

    public enum AttackType {
        InFrontOfUnit, GlobalRandomLocations
    }

    public enum RangeType {
        Normal, EndAtRange, IgnoreRange
    }

    public enum FrontAttackType {
        Arc, StraightLine
    }

    public enum AbilityType {
        StatsBuff, Environment
    }

    [System.Serializable]
    public struct Stat {
        public string statName;
        public float statValue;
        public bool negativeAllowed;
    }

    [System.Serializable]
    public struct UnitStats {
        public ModifierValueType valueModifierType;
        public bool timed;
        public float lifetime;
        public Stat[] stats;
        //5 stats - 1: DamageMod
        //2: ArmorMod
        //3: MovespeedMod
        //4: AttackSpeedMod
        //5: LifeRegnMod
    }

    [System.Serializable]
    public struct UnitAttacks {
        [HideInInspector]
        public float timer;

        public string attackName;
        [Header("Attack Attributes")]
        public KeyCode keyTrigger;
        public string targetTag;
        public float attackCooldown;
        //public bool fireOnTheMove;
        public bool selfDestructOnAttack;

        [Header("Range Attributes")]
        public float range;
        public RangeType rangeType;

        [Header("Projectile Attributes")]
        public Projectile attack;
        public int amountToSpawn;
        public AttackType typeOfAttack;

        [Header("In Front Of Unit Attributes")]
        public FrontAttackType typeOfFrontAttack;
        public float arc;

        [Header("Global Random Locations Attribute")]
        public bool overrideTargetAsThis;

        [Header("Y Attributes")]       
        public float setY;
        public float minForRandomY;
        public bool useUnitY;
        public bool randomY;
        public bool ignoreY;

        [Header("Debug")]
        public bool disableAttack;
    }

    [System.Serializable]
    public struct UnitAbilities {
        [HideInInspector]
        public float timer;

        public string abilityName;

        [Header("Ability Attribute")]
        //public AbilityType abilityClass;
        public KeyCode keyTrigger;
        public float abilityTimer;

        [Header("Stats Attribute")]
        public UnitStats statModifier;

        //[Header("Environment Attribute")]
        //public float affectedTimeScale;

        [Header("Debug")]
        public bool disableAbility;
    }

    [Header("Unit Attribute")]
    public new string name;
    public float health;

    [Header("Unit Stats")]
    public List<UnitStats> unitStat = new List<UnitStats>();

    #region Movement
    protected Vector3 destination;
    protected Vector3 normalizedDist;
    protected float initialY;
    #endregion

    void Awake() {
    }

    public float FireWeapon(UnitAttacks weapInst, Transform target) {
        if (!weapInst.disableAttack) {
            if (weapInst.timer < Time.time)
                if (weapInst.range * weapInst.range >= (target.position - transform.position).sqrMagnitude || weapInst.rangeType != RangeType.Normal) {
                    ProjectileTypeSorting(weapInst, target);
                    if (weapInst.selfDestructOnAttack)
                        Destroy(gameObject);
                    else {
                        weapInst.timer = Time.time + (weapInst.attackCooldown * StatCompiler(3));
                    }
                }
        }
        return weapInst.timer;
    }

    public float UseAbility(UnitAbilities abilityInst) {
        if (!abilityInst.disableAbility)
            if (abilityInst.timer < Time.time) {
                abilityInst.timer = Time.time + (abilityInst.abilityTimer * StatCompiler(3));
                UnitStats temp = abilityInst.statModifier;
                temp.lifetime += Time.time;
                unitStat.Add(temp);
            }

        return abilityInst.timer;
    }

    public void ProjectileTypeSorting(UnitAttacks reference, Transform target) {
        Projectile inst = null;
        Vector3 unitPos = Vector3.zero;
        Vector3 normDist = Vector3.zero;
        Vector3 targetPos = target.position;

        if (reference.rangeType == RangeType.EndAtRange) {
            targetPos = transform.position + (Vector3.Normalize(target.position - transform.position)) * reference.range;
        }

        switch (reference.typeOfAttack) {
            case AttackType.InFrontOfUnit:
                if (reference.useUnitY)
                    reference.setY = transform.position.y;
                if (reference.randomY)
                    reference.setY = Random.Range(reference.minForRandomY, reference.setY);

                unitPos = new Vector3(transform.position.x, reference.setY, transform.position.z);

                if (reference.ignoreY) 
                    targetPos.y = reference.setY;
                

                normDist = Vector3.Normalize(targetPos - unitPos);

                switch (reference.typeOfFrontAttack) {
                    case FrontAttackType.Arc:
                        for (var i = -5; i < 5; i++) { //This needs a fix
                            //Debug.DrawRay(transform.position + (normDist + (new Vector3(reference.arc, 0, reference.arc) * i)) * reference.range, new Vector3(0, 10, 0), Color.red, 2);
                            inst = (Projectile)Instantiate(reference.attack, unitPos, Quaternion.identity);
                            inst.damage *= StatCompiler(0);
                            inst.normDirection = (normDist + (new Vector3(reference.arc, 0, reference.arc) * i));
                            InitialisingProjectile(inst, target, reference.targetTag);
                        }
                        break;
                    case FrontAttackType.StraightLine:
                        for (var i = 1; i < reference.amountToSpawn + 1; i++) {
                            //Debug.DrawRay(transform.position + ((normDist * reference.range * i) / reference.amountToSpawn), new Vector3(0, 10, 0), Color.red, 2);
                            inst = (Projectile)Instantiate(reference.attack, unitPos, Quaternion.identity);
                            inst.damage *= StatCompiler(0);
                            inst.normDirection = (normDist * i) / reference.amountToSpawn;
                            InitialisingProjectile(inst, target, reference.targetTag);
                        }
                        break;
                }
                break;

            case AttackType.GlobalRandomLocations:
                for (var i = 0; i < reference.amountToSpawn; i++) {
                    if (reference.useUnitY)
                        reference.setY = transform.position.y;
                    if (reference.randomY)
                        reference.setY = Random.Range(reference.minForRandomY, reference.setY);

                    inst = (Projectile)Instantiate(reference.attack, new Vector3(Random.Range(transform.position.x - reference.range / 2, transform.position.x + reference.range / 2), reference.setY, Random.Range(transform.position.z - reference.range / 2, transform.position.z + reference.range / 2)), Quaternion.identity);
                    inst.damage *= StatCompiler(0);
                    inst.normDirection = Vector3.Normalize(targetPos - inst.transform.position);
                    InitialisingProjectile(inst, target, reference.targetTag);
                }
                break;

        }
    }

    void InitialisingProjectile(Projectile inst, Transform firstTar, string targetTag) {
        inst.target = firstTar;
        inst.spawner = transform;
        inst.affectsTag = targetTag;
    }

    public void DamageUnit(float damage) {
        damage -= StatCompiler(1);
        health -= damage;

        if (health <= 0)
            Destroy(gameObject);
    }

    public float StatCompiler(int stat) {
        float temp = 0;
        for (var i = 0; i < unitStat.Count; i++) {
            if (!unitStat[i].timed || Time.time < unitStat[i].lifetime) {
                switch (unitStat[i].valueModifierType) {
                    case ModifierValueType.Value:
                        temp += unitStat[i].stats[stat].statValue;
                        break;
                    case ModifierValueType.Percentage:
                        temp *= unitStat[i].stats[stat].statValue;
                        break;
                }

                if (temp <= 0 && !unitStat[0].stats[0].negativeAllowed) {
                    temp = 0;
                    return temp;
                }
            } else {
                unitStat.Remove(unitStat[i]);
            }
        }
        return temp;
    }
}
