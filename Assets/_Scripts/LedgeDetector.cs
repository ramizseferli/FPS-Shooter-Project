using System.Collections;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float wallCheckDistance = 1.2f;
    public float ledgeCheckHeight = 1.9f;
    public float maxLedgeDepth = 2.5f;
    public LayerMask climbableLayer;

    [Header("Reach & Height Safety Limits")]
    public float maxJumpReachHeight = 2.5f;

    [Header("Offset Settings")]
    [Tooltip("Əllərin divarın kənarına tam oturması üçün dikey offset")]
    public float handYOffsetFromPivot = 1.95f;
    [Tooltip("Asılarkən divarla bədən arasındakı məsafə")]
    public float hangOffsetForward = 0.1f;
    public float climbDuration = 1.8f;

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

                    // MAX HÜNDÜRLÜK SÜZGƏCİ
                    float heightDifference = _topPosition.y - transform.position.y;
                    if (heightDifference > maxJumpReachHeight) return;

                    // DƏQİQ ASILMA HESABLAMASI
                    float targetY = _topPosition.y - handYOffsetFromPivot;

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

        while (elapsedTime < 0.2f)
        {
            transform.position = Vector3.Lerp(startPos, _hangPosition, elapsedTime / 0.2f);
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

        if (_characterController != null)
            _characterController.enabled = false;

        if (_animator != null)
        {
            _animator.SetBool("isHanging", false);
            _animator.SetTrigger("Climb");
            _animator.applyRootMotion = false;
        }

        // Animasiyanın ən sonundakı o xətalı reset kadrolarına çatmamaq üçün 
        // gözləmə müddətini 0.3 saniyə tez bitiririk
        float targetWaitTime = Mathf.Max(0.1f, climbDuration - 0.3f);
        yield return new WaitForSeconds(targetWaitTime);

        // Xarakteri tam divarın üstünə oturduruq
        Vector3 finalPosition = _topPosition + (transform.forward * 0.35f);
        transform.position = finalPosition;

        if (_animator != null)
        {
            _animator.ResetTrigger("Climb");
            _animator.SetBool("Grounded", true);
            _animator.SetBool("isHanging", false);
        }

        if (_characterController != null)
        {
            _characterController.enabled = true;
            _characterController.Move(Vector3.down * 0.05f);
        }

        _isClimbing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_topPosition, 0.08f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_hangPosition, 0.08f);
    }
}