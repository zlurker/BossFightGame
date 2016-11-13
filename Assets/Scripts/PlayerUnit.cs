using UnityEngine;
using System.Collections;

public class PlayerUnit : UnitBase {

    public float playerMoveSpeed;
    public Transform target;

    [Header("Player Weapons")]
    public UnitAttacks[] playerWeapons;
    [Header("Player Abilities")]
    public UnitAbilities[] playerAbilities;

    public int currentWeapSelected;

    #region Movement
    Vector3 destination;
    Vector3 normalizedDist;
    float initialY;
    Ray ray;
    RaycastHit hit;
    #endregion

    void Start() {
        destination = transform.position;
        initialY = transform.position.y;
    }

    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            if (Input.GetMouseButtonDown(0)) {
                destination = hit.point;
                destination.y = initialY;
                normalizedDist = Vector3.Normalize(destination - transform.position);
                Debug.DrawLine(ray.origin, hit.point, Color.red);
            }
        }

        if (!target)
            HuntForTarget();

        playerMoveSpeed = StatCompiler(2);

        if ((destination - transform.position).sqrMagnitude >= playerMoveSpeed * playerMoveSpeed) {
            transform.position += normalizedDist * playerMoveSpeed;
            if (target)
                if (playerWeapons[currentWeapSelected].fireOnTheMove)
                    playerWeapons[currentWeapSelected].timer = FireWeapon(playerWeapons[currentWeapSelected], target);
        } else
            if (target)
            playerWeapons[currentWeapSelected].timer = FireWeapon(playerWeapons[currentWeapSelected], target);

        if (target)
            if ((target.position - transform.position).sqrMagnitude > playerWeapons[currentWeapSelected].range * playerWeapons[currentWeapSelected].range || playerWeapons[currentWeapSelected].rangeType == RangeType.Normal)
                target = null;

        for (var i = 0; i < playerAbilities.Length; i++) {
            playerAbilities[i].timer = UseAbility(playerAbilities[i]);
        }
    }

    void HuntForTarget() {
        float dist = Mathf.Infinity;
        Collider temp = null;
        Collider[] targets = Physics.OverlapSphere(transform.position, playerWeapons[currentWeapSelected].range);
        foreach (Collider tar in targets)
            if (tar.CompareTag(playerWeapons[currentWeapSelected].targetTag)) {
                if ((tar.transform.position - transform.position).sqrMagnitude < dist) {
                    dist = (tar.transform.position - transform.position).sqrMagnitude;
                    temp = tar;
                }
            }
        if (temp != null)
            target = temp.transform;
    }
}
