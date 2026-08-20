using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private Vector3 recoilRotation = new Vector3(-2f, 0.5f, 0.5f); //Kickback angles(bucaqlarý)
    [SerializeField] private Vector3 recoilKickback = new Vector3(0f, 0f, -0.1f);

    [Header("Speed & Snappiness")]
    [SerializeField] private float snappiness = 10f;
    [SerializeField] private float returnSpeed = 6f;
    [SerializeField] private WeaponController weaponController;

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 currentPosition;
    private Vector3 targetPosition;

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
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, returnSpeed * Time.deltaTime);

        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, snappiness * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
        transform.localPosition = currentPosition;
    }

    private void FireRecoil()
    {
        targetRotation += new Vector3(recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y),Random.Range(-recoilRotation.z, recoilRotation.z));
        targetPosition += recoilKickback; 

    }
}