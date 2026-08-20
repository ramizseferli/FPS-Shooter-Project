using UnityEngine;

public class WeaponInputHandler : MonoBehaviour
{
    private WeaponController weaponController;
    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    private void Update()
    {
        if (weaponController == null) return;

        FireMode mode = weaponController.GetFireMode();

        if (mode == FireMode.SemiAuto || mode == FireMode.Burst)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                weaponController.TryShoot();
            }
        }
        else if (mode == FireMode.FullyAuto)
        {
             if (Input.GetButton("Fire1"))
             {
                weaponController.TryShoot();
             }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            weaponController.TryReload();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            weaponController.ToggleWeapon();
        }
    }
}
