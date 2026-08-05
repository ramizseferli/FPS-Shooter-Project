using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponController weaponController;

    [SerializeField] private string shootAnimationStateName = "Pistol_Shoot";

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }

        if (weaponController == null)
        {
            weaponController = GetComponent<WeaponController>();
        }
    }

    private void OnEnable()
    {
        if (weaponController != null)
        {
            weaponController.OnFired += PlayShootAnimation;
        }
    }

    private void PlayShootAnimation()
    {
        if (animator != null)
        {
            animator.Play(shootAnimationStateName, -1, 0f);
        }
    }
}