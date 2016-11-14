using UnityEngine;
using System.Collections;

public class PlayerUnit : UnitBase {

    public enum InputType {
        TapToMove, KeyDown
    }

    public float playerMoveSpeed;
    public Transform target;

    [Header("Player Weapons")]
    public UnitAttacks[] playerWeapons;
    [Header("Player Abilities")]
    public UnitAbilities[] playerAbilities;
    [Header("Input Type")]
    public bool engageMode;
    public InputType inputType;

    [Header("KeyDown Input Settings")]
    public string horizontalMovement;
    public string verticalMovement;
    public string rotatePlayer;
    public string fireGun;
    public string switchWeapF;
    public string switchWeapB;

    [Header("Camera Settings")]
    public bool firstPerson;

    public int currentWeapSelected;

    #region Movement
    Ray ray;
    RaycastHit hit;
    #endregion

    void Start() {
        destination = transform.position;
        initialY = transform.position.y;
        //engageMode = false;
    }

    void Update() {
        playerMoveSpeed = StatCompiler(2);

        switch (inputType) {
            case InputType.TapToMove:
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                    if (Input.GetMouseButtonDown(0)) {
                        destination = hit.point;
                        destination.y = initialY;
                        normalizedDist = Vector3.Normalize(destination - transform.position);
                        //Debug.DrawLine(ray.origin, hit.point, Color.red);
                    }


                if ((destination - transform.position).sqrMagnitude >= playerMoveSpeed * playerMoveSpeed)
                    transform.position += normalizedDist * playerMoveSpeed;

                break;

            case InputType.KeyDown:
                transform.position += transform.TransformDirection(playerMoveSpeed * Input.GetAxis(horizontalMovement), 0, playerMoveSpeed * Input.GetAxis(verticalMovement));
                transform.position = new Vector3(transform.position.x, initialY, transform.position.z);


                if (Input.GetButtonDown(fireGun))
                    engageMode = engageMode ? false : true;

                if (Input.GetButtonDown(switchWeapF))
                    CycleWeapon(1);
                if (Input.GetButtonDown(switchWeapB))
                    CycleWeapon(-1);

                if (firstPerson)
                    if (target && engageMode) {
                        transform.LookAt(target);
                    }
                break;
        }

        if (engageMode) {
            if (!target)
                HuntForTarget();

            if (target) {
                playerWeapons[currentWeapSelected].timer = FireWeapon(playerWeapons[currentWeapSelected], target);
                if ((target.position - transform.position).sqrMagnitude > playerWeapons[currentWeapSelected].range * playerWeapons[currentWeapSelected].range && playerWeapons[currentWeapSelected].rangeType == RangeType.Normal)
                    target = null;
            }
        }

        for (var i = 0; i < playerAbilities.Length; i++) {
            playerAbilities[i].timer = UseAbility(playerAbilities[i]);
        }
        Debug.Log(playerWeapons[currentWeapSelected].attackName);
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

    void CycleWeapon(int value) {
        currentWeapSelected += value;

        if (currentWeapSelected < 0)
            currentWeapSelected = playerWeapons.Length - 1;

        if (currentWeapSelected >= playerWeapons.Length)
            currentWeapSelected = 0;
    }
}
