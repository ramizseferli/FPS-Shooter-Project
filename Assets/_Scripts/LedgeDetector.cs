using System.Collections;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float wallCheckDistance = 1.2f;
    public float ledgeCheckHeight = 1.6f;
    public float maxLedgeDepth = 1.5f;
    public LayerMask climbableLayer;

    [Header("Offset Settings")]
    [Tooltip("Xarakter asılanda əllərinin divar kənarına dəyməsi üçün aşağı düşmə məsafəsi")]
    public float hangOffsetDown = 1.4f;
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
        if (_animator !=null)
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
        Vector3 origin = transform.position + Vector3.up * 0.1f;
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

                    // Divarın tam kənarına uyğun asılma mövqeyi
                    _hangPosition = _topPosition
                                    - (transform.forward * hangOffsetForward)
                                    - (Vector3.up * hangOffsetDown);

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
            // Move/Speed parametrlərini sıfırla ki, yerimə animasiyası dayansın
            _animator.SetFloat("Speed", 0f);
            _animator.SetFloat("MotionSpeed", 0f);
            _animator.SetBool("Grounded", false); // Havada olduğunu bildiririk
            _animator.SetBool("FreeFall", false);

            int upperLayer = _animator.GetLayerIndex("UpperBody");
            if (upperLayer != -1) _animator.SetLayerWeight(upperLayer, 0f);

            _animator.applyRootMotion = false;

            // Hanging animasiyasını aktivləşdir
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
            // YALNIZ parametrləri dəyişirik, Play() çağırmırıq!
            _animator.SetBool("isHanging", false);
            _animator.SetTrigger("Climb");
        }

        Vector3 startPosition = transform.position;
        Vector3 midPoint = new Vector3(startPosition.x, _topPosition.y, startPosition.z);
        Vector3 finalPosition = _topPosition + (transform.forward * 0.4f);

        float elapsedTime = 0f;

        while (elapsedTime < climbDuration)
        {
            float t = elapsedTime / climbDuration;

            if (t < 0.5f)
            {
                transform.position = Vector3.Lerp(startPosition, midPoint, t / 0.5f);
            }
            else
            {
                transform.position = Vector3.Lerp(midPoint, finalPosition, (t - 0.5f) / 0.5f);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;

        // FİZİKA VƏ ANIMATORU SIFIRLAYIRIQ
        if (_animator != null)
        {
            _animator.SetBool("isHanging", false);
            _animator.ResetTrigger("Climb"); // Trigger-i sıfırlayırıq
            _animator.SetBool("Grounded", true);
        }

        if (_characterController != null)
            _characterController.enabled = true;

        _isClimbing = false;
    }
}