using UnityEngine;


public enum FireMode
{
    SemiAuto,
    FullyAuto,
    Burst
}
[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapon System/Weapon Data")]

public class WeaponData : ScriptableObject
{
    [Header("Weapon Identity")]
    public string weaponName;
    public FireMode fireMode = FireMode.FullyAuto;

    [Header("Shooting Settings")]
    public int maxAmmo = 30;
    public float fireRate = 0.1f;
    public float range = 100f;
    public float damage = 20f;

    [Header("Reload Settings")]
    public float reloadTime = 2.0f;

    [Header("Visual & Audio Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHolePrefab;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    
}
