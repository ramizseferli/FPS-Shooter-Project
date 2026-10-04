using UnityEngine;

public class WorkbenchSlot : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private GameObject weaponOnSlot;

    private bool isPlayerInRange = false;
    private WeaponController playerWeaponController;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Dəyən obyektin özündə VƏ YA onun ana obyektdə (Parent) WeaponController varmı yoxlayırıq
        WeaponController controller = other.GetComponentInParent<WeaponController>();

        if (controller == null)
        {
            controller = other.GetComponentInChildren<WeaponController>();
        }

        if (controller != null)
        {
            playerWeaponController = controller;
            isPlayerInRange = true;
            Debug.Log("[WORKBENCH] Oyunçu masanın zonasına girdi.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WeaponController controller = other.GetComponentInParent<WeaponController>();
        if (controller == null)
        {
            controller = other.GetComponentInChildren<WeaponController>();
        }

        if (controller != null && controller == playerWeaponController)
        {
            isPlayerInRange = false;
            playerWeaponController = null;
            Debug.Log("[WORKBENCH] Oyunçu masanın zonasından çıxdı.");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithTable();
        }
    }

    private void InteractWithTable()
    {
        if (playerWeaponController == null) return;

        Debug.Log("[WORKBENCH] E düyməsi basıldı. Silah aktivləşdirilir.");

        // Oyunçuya silahı veririk
        playerWeaponController.SetArmedState(true);

        // Masadakı silahı gizlədirik
        if (weaponOnSlot != null)
        {
            weaponOnSlot.SetActive(false);
        }
    }
}