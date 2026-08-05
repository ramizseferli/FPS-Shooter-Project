using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.1f;

    private Animation gunAnimation;

    [Header("References")]
    [SerializeField] private Camera fpsCamera;
    private float nextTimeToFire = 0f;
    private int layerMask;

    void Start()
    {
        layerMask = LayerMask.GetMask("Wall", "Character");
        gunAnimation = GetComponent<Animation>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (gunAnimation != null)
        {
            gunAnimation.Rewind("Pistol_Shoot");
            gunAnimation.Play("Pistol_Shoot");
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range, layerMask))
        {
            Debug.Log("Vurulan Hedef: " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Debug.DrawLine(fpsCamera.transform.position, hit.point, Color.yellow, 0.5f);
        }
        else
        {
            Debug.DrawRay(fpsCamera.transform.position, fpsCamera.transform.forward * range, Color.white, 0.5f);
        }
    }

    public void OnAnimationEvent()
    {
        // Gələcəkdə istifadə edilə bilər
    }
}