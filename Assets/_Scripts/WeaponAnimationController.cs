using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private string shootAnimationTriggerName = "Pistol_Shoot";

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

    private void OnDisable()
    {
        if (weaponController != null)
        {
            weaponController.OnFired -= PlayShootAnimation;
        }
    }

    private void PlayShootAnimation()
    {
        if (animator != null)
        {
            // Qalan trigger-ləri sıfırlayırıq ki, təkrarlanan atəşdə animasiya ilişib qalmasın
            animator.ResetTrigger(shootAnimationTriggerName);

            // Xarakterin üst bədən animasiyasını işə salır
            animator.SetTrigger(shootAnimationTriggerName);
        }
    }
}