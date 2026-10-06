using UnityEngine;

public class WorkbenchSlot : MonoBehaviour
{
    // YENİ: Ən son hansı slotdan silah götürüldüyünü yadda saxlayır
    private static WorkbenchSlot lastPickedSlot;

    [Header("Visual Reference")]
    [SerializeField] private GameObject weaponOnSlot; // Bu konkret slotdakı vizual silah

    private bool isPlayerInRange = false;
    private WeaponController playerWeaponController;

    private void OnTriggerEnter(Collider other)
    {
        WeaponController controller = other.GetComponentInParent<WeaponController>();
        if (controller == null)
        {
            controller = other.GetComponentInChildren<WeaponController>(true);
        }

        if (controller != null)
        {
            playerWeaponController = controller;
            isPlayerInRange = true;
            Debug.Log($"[WORKBENCH] {gameObject.name} zonasında oyunçu var.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WeaponController controller = other.GetComponentInParent<WeaponController>();
        if (controller == null)
        {
            controller = other.GetComponentInChildren<WeaponController>(true);
        }

        if (controller != null && controller == playerWeaponController)
        {
            isPlayerInRange = false;
            playerWeaponController = null;
            Debug.Log($"[WORKBENCH] {gameObject.name} zonasından oyunçu çıxdı.");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithSlot();
        }
    }

    private void InteractWithSlot()
    {
        if (playerWeaponController == null) return;

        // Slotda silah yoxdursa və ya artıq götürülübsə qarşısını alırıq
        if (weaponOnSlot != null && !weaponOnSlot.activeSelf) return;

        // --- YENİ: SWAP (Yerdəyişmə) MƏNTİQİ ---
        // Əgər əvvəl başqa slotdan silah götürmüşdüksə, həmin silahı masaya qaytarırıq
        if (lastPickedSlot != null && lastPickedSlot != this)
        {
            if (lastPickedSlot.weaponOnSlot != null)
            {
                lastPickedSlot.weaponOnSlot.SetActive(true);
            }
        }
        // ----------------------------------------

        Debug.Log($"[WORKBENCH] {gameObject.name} üçün E basıldı. Silah verilir.");

        // Oyunçunun silahını aktivləşdiririk
        playerWeaponController.SetArmedState(true);

        // Yalnız BU slotdakı vizual silahı söndürürük
        if (weaponOnSlot != null)
        {
            weaponOnSlot.SetActive(false);
        }

        // YENİ: Bu slotu son istifadə olunan kimi yadda saxlayırıq
        lastPickedSlot = this;
    }
}