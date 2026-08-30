using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private Vector3 recoilRotation = new Vector3(-2f, 0.5f, 0.5f);
    [SerializeField] private Vector3 recoilKickback = new Vector3(0f, 0f, -0.1f);

    [Header("Slide Recoil Settings")]
    [SerializeField] private Transform slideTransform;

    [Tooltip("Slide-ın normalda durduğu X koordinatı")]
    [SerializeField] private float slideDefaultX = -0.069f;

    [Tooltip("Atəş anında arxaya gedəcəyi X kordinatı")]
    [SerializeField] private float slideRecoilX = -0.3f;
    

    [Header("Speed & Snappiness")]
    [SerializeField] private float snappiness = 20f; // Slide kəskinliyi üçün artırıldı
    [SerializeField] private float returnSpeed = 12f; // Sürətli geri qayıtma üçün artırıldı
    [SerializeField] private WeaponController weaponController;

    [Header("Transform Offset")]
    [SerializeField] private Vector3 defaultRotationOffset = new Vector3(-90f, 0f, 0f);

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 currentPosition;
    private Vector3 targetPosition;

    private float slideCurrentX;
    private float slideTargetX;

    private void Start()
    {
        if (slideTransform != null)
        {
            slideCurrentX = slideDefaultX;
            slideTargetX = slideDefaultX;

            Vector3 currentLocalPos = slideTransform.localPosition;
            slideTransform.localPosition = new Vector3(slideDefaultX, currentLocalPos.y, currentLocalPos.z);
        }
    }

    private void OnEnable()
    {
        if (weaponController != null)
        {
            weaponController.OnFired += FireRecoil;
        }
    }

    private void OnDisable()
    {
        if (weaponController != null)
        {
            weaponController.OnFired -= FireRecoil;
        }
    }

    void Update()
    {
        //Parent recoil
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, returnSpeed * Time.deltaTime);

        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, snappiness * Time.deltaTime);


        transform.localRotation = Quaternion.Euler(defaultRotationOffset + currentRotation);
        transform.localPosition = currentPosition;

        // 2. Slide Recoil
        if (slideTransform != null)
        {
            slideTargetX = Mathf.Lerp(slideTargetX, slideDefaultX, returnSpeed * Time.deltaTime);
            slideCurrentX = Mathf.Lerp(slideCurrentX, slideTargetX, snappiness * Time.deltaTime);

            Vector3 localPos = slideTransform.localPosition;
            slideTransform.localPosition = new Vector3(slideCurrentX, localPos.y, localPos.z);
        }
    }

    private void FireRecoil()
    {
        // Silahın ümumi təpməsi
        targetRotation += new Vector3(recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        targetPosition += recoilKickback;

        // Atəş anında slideTargetX birbaşa -0.3-ə atılır
        if (slideTransform != null)
        {
            slideTargetX = slideRecoilX;
        }
    }
}