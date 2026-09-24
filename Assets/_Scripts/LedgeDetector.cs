using System.Collections;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float wallCheckDistance = 1.2f;
    public float ledgeCheckHeight = 1.6f;
    public float maxLedgeDepth = 1.5f;
    public LayerMask climbableLayer;

    [Header("Reach & Height Safety Limits")]
    [Tooltip("Personajın tullanıb tuta biləcəyi maksimum hündürlük (metrlə)")]
    public float maxJumpReachHeight = 2.5f;
    [Tooltip("Çiyin/Əl oxundan tilə qədər maksimum çatışma məsafəsi")]
    public float maxArmLength = 0.9f;
    [Tooltip("Çiyin və ya əlin çıxış nöqtəsi (Boş qalsa Player transform istifadə edəcək)")]
    public Transform shoulderPoint;

    [Header("Offset Settings")]
    [Tooltip("Xarakter asılanda əllərinin divar kənarına dəyməsi üçün aşağı düşmə məsafəsi")]
    public float hangOffsetDown = 0f;
    [Tooltip("Xarakterin divardan irəli/geri məsafəsi")]
    public float hangOffsetForward = 0.25f;
    public float climbDuration = 1.0f;

    [Header("References")]
    public Animator _animator;
    private CharacterController _characterController;

    private bool _isHanging = false;
    private bool _isClimbing = false;
    private Vector3 _hangPosition;
    private Vector3 _topPosition;

    private void Start()
    {
        if (_animator != null)
        {
            _animator.SetBool("isHanging", false);
            _animator.SetBool("Grounded", true);
            _animator.SetBool("FreeFall", false);
        }
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (shoulderPoint == null)
            shoulderPoint = transform;
    }

    private void Update()
    {
        if (_isClimbing) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isHanging)
            {
                TryHang();
            }
            else
            {
                StartCoroutine(ClimbUpRoutine());
            }
        }
    }

    private void TryHang()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        RaycastHit wallHit;

        if (Physics.Raycast(origin, transform.forward, out wallHit, wallCheckDistance, climbableLayer))
        {
            Vector3 highOrigin = transform.position + Vector3.up * ledgeCheckHeight;
            RaycastHit ledgeHit;

            if (Physics.Raycast(highOrigin, transform.forward, out ledgeHit, wallCheckDistance + maxLedgeDepth, climbableLayer))
            {
                Vector3 downOrigin = ledgeHit.point + Vector3.up * 0.5f;
                RaycastHit topHit;

                if (Physics.Raycast(downOrigin, Vector3.down, out topHit, 1f, climbableLayer))
                {
                    _topPosition = topHit.point;

                    // --- SÜZGƏC 1: MAX HÜNDÜRLÜK YOXLAMASI ---
                    float heightDifference = _topPosition.y - transform.position.y;
                    if (heightDifference > maxJumpReachHeight) return;

                    // --- SÜZGƏC 2: ƏLİN / QOLUN ÇATMA MƏSAFƏSİ YOXLAMASI ---
                    float distanceToLedge = Vector3.Distance(shoulderPoint.position, _topPosition);
                    if (distanceToLedge > maxArmLength) return;

                    // --- DƏQİQ ASILMA HESABLAMASI ---
                    float handYOffsetFromPivot = 1.95f;
                    float targetY = _topPosition.y - handYOffsetFromPivot - hangOffsetDown;

                    _hangPosition = new Vector3(
                        _topPosition.x,
                        targetY,
                        _topPosition.z
                    ) - (transform.forward * hangOffsetForward);

                    StartCoroutine(HangRoutine());
                }
            }
        }
    }

    private IEnumerator HangRoutine()
    {
        _isClimbing = true;

        if (_characterController != null)
            _characterController.enabled = false;

        if (_animator != null)
        {
            _animator.SetFloat("Speed", 0f);
            _animator.SetFloat("MotionSpeed", 0f);
            _animator.SetBool("Grounded", false);
            _animator.SetBool("FreeFall", false);

            int upperLayer = _animator.GetLayerIndex("UpperBody");
            if (upperLayer != -1) _animator.SetLayerWeight(upperLayer, 0f);

            _animator.applyRootMotion = false;

            _animator.SetBool("isHanging", true);
            _animator.Play("Hanging", 0, 0f);
        }

        float elapsedTime = 0f;
        Vector3 startPos = transform.position;

        while (elapsedTime < 0.25f)
        {
            transform.position = Vector3.Lerp(startPos, _hangPosition, elapsedTime / 0.25f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = _hangPosition;
        _isHanging = true;
        _isClimbing = false;
    }

    private IEnumerator ClimbUpRoutine()
    {
        _isHanging = false;
        _isClimbing = true;

        if (_animator != null)
        {
            _animator.SetBool("isHanging", false);
            _animator.SetTrigger("Climb");
            _animator.applyRootMotion = false; // Kodla idarə etdiyimiz üçün Root Motion bağlanır
        }

        Vector3 startPosition = transform.position;

        // Final position: Xarakterin tam çıxacağı yer (Tilin üstü + bir az irəli)
        Vector3 finalPosition = _topPosition + (transform.forward * 0.4f);

        // MidPoint: Asıldığı Y dəyərindən birbaşa final hündürlüyə (səthə) olan Y qalxma nöqtəsi
        Vector3 midPoint = new Vector3(startPosition.x, finalPosition.y, startPosition.z);

        float elapsedTime = 0f;

        while (elapsedTime < climbDuration)
        {
            float t = elapsedTime / climbDuration;

            if (t < 0.6f)
            {
                // Vaxtın 60%-də bədəni tədricən divarın üst hündürlüyünə qaldırır
                transform.position = Vector3.Lerp(startPosition, midPoint, t / 0.6f);
            }
            else
            {
                // Qalan 40%-də bədəni irəli — divarın üstünə keçirir
                transform.position = Vector3.Lerp(midPoint, finalPosition, (t - 0.6f) / 0.4f);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;

        if (_animator != null)
        {
            _animator.SetBool("isHanging", false);
            _animator.ResetTrigger("Climb");
            _animator.SetBool("Grounded", true);
        }

        if (_characterController != null)
            _characterController.enabled = true;

        _isClimbing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = shoulderPoint != null ? shoulderPoint : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(point.position, maxArmLength);
    }
}