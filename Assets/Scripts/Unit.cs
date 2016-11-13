using UnityEngine;
using System.Collections;

public class Unit : UnitBase {

    [System.Serializable]
    public struct UnitPhases {
        public string phaseName;
        public float speed;
        public float armor;
        public float healthRegen;
    }

    public bool bossStatus;
    public Transform target;
    [Header("Unit Behavior")]
    public UnitPhases[] phases;
    public UnitAttacks[] attacks;
    public UnitAbilities[] abilities;

    void Start() {

    }

    void Update() {
        for (var i = 0; i < attacks.Length; i++) {
            Transform tempTarget = null;

            if (attacks[i].overrideTargetAsThis)
                tempTarget = transform;
            else
                tempTarget = target;

            attacks[i].timer = FireWeapon(attacks[i], tempTarget);
        }

        for (var i = 0; i < abilities.Length; i++) {
            abilities[i].timer = UseAbility(abilities[i]);
        }
    }
}

