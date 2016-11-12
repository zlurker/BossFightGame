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

    // Update is called once per frame
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

        if ((destination - transform.position).sqrMagnitude >= playerMoveSpeed * playerMoveSpeed) {
            transform.position += normalizedDist * playerMoveSpeed;
            if (playerWeapons[currentWeapSelected].fireOnTheMove)
                playerWeapons[currentWeapSelected].timer = FireWeapon(playerWeapons[currentWeapSelected],target);
        } else
            playerWeapons[currentWeapSelected].timer = FireWeapon(playerWeapons[currentWeapSelected], target);
    }    
}
