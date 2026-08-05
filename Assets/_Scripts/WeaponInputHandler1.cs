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
        if (weaponController.GetFireMode() == FireMode.SemiAuto || weaponController.GetFireMode() == FireMode.Burst)
        {
            if (Input.GetMouseButton(0))
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

        if (Input.GetButton("Fire1"))
        {
            weaponController.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            weaponController.TryReload();
        }
    }



}
