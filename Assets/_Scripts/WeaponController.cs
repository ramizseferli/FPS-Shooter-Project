using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Data Connection")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private int currentAmmo;

    [Header("Weapon toggle settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject weaponMesh;
    [SerializeField] private bool isArmed = false;

    [Header("Raycast / Shooting Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private float range = 100f;
    [SerializeField] private float damage = 20f;


    // Events
    public event Action OnFired;
    public event Action<int, int> OnAmmoChanged;

    // Private Variables
    private int maxAmmo;
    private float fireRate;
    private float nextTimeToFire;
    private float reloadTime;
    private bool isReloading = false;
    private FireMode fireMode;
    // 1. UNITY LIFECYCLE (İlk işə düşənlər)
    public void Awake()
    {
        InitializeWeaponData();
    }

    public void InitializeWeaponData()
    {
        if(weaponData !=null)
        {
            maxAmmo = weaponData.maxAmmo;
            fireRate = weaponData.fireRate;
            range=weaponData.range;
            damage = weaponData.damage;
            reloadTime = weaponData.reloadTime;
            fireMode = weaponData.fireMode;

            currentAmmo = maxAmmo;
        }

        else
        {
            Debug.LogError($"{gameObject.name} üzərində WeaponData təyin olunmayıb");
        }
    }

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    // 2. PUBLIC METHODS (Kənardan çağırılanlar)
    public void TryShoot()
    {
        if (Time.time >= nextTimeToFire && currentAmmo > 0 && !isReloading)
        {
            if (fireMode == FireMode.Burst)
            {
                StartCoroutine(BurstShootCorotine());
            }
            else
            {
                ExecuteSingleShot();
            }

        }
    }

    public void TryReload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }
    private void ExecuteSingleShot()
    {
        currentAmmo -= 1;
        nextTimeToFire = Time.time + fireRate;
        ShootRaycast();

        OnFired?.Invoke();
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }
    private IEnumerator BurstShootCorotine()
    {
        nextTimeToFire = Time.time + fireRate;

        for (int i = 0; i<3; i++)
        {
            if (currentAmmo>0 && !isReloading)
            {
                ExecuteSingleShot();
                yield return new WaitForSeconds(0.08f);
            }
        }
    }
    private void ShootRaycast()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Kamera Təyin olunmayıb! Raycast atıla bilmir.");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range, ~ignoreLayers))
        {
            Debug.Log($"[SHOOT] Dəydiyi obyekt: {hit.transform.name}");
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
    // 3. PRIVATE METHODS / COROUTINES (Daxili köməkçilər)
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        isReloading = false;
    }
    // 4. Weapon Shoot Animation arrangement
    public void ToggleWeapon()
    {
        isArmed = !isArmed;

        if (animator !=null)
        {
            animator.SetBool("IsArmed", isArmed);
        }

        if(weaponMesh != null)
        {
            weaponMesh.SetActive(isArmed);
        }
    }
    public FireMode GetFireMode() => fireMode;
}