using UnityEngine;

[RequireComponent(typeof(RagdollCharacter))]
[RequireComponent(typeof(GroundDetector))]
public class MovementController : MonoBehaviour
{
    [SerializeField] PlayerInputHandler _input;
    [SerializeField] Transform _cameraTransform;

    RagdollCharacter _character;
    GroundDetector _groundDetector;

    float _moveForce;
    float _maxSpeed;

    void Awake()
    {
        _character = GetComponent<RagdollCharacter>();
        _groundDetector = GetComponent<GroundDetector>();
    }

    void Start()
    {
        var def = _character.definition;
        _moveForce = def.moveForce;
        _maxSpeed = def.maxSpeed;
    }

    void FixedUpdate()
    {
        if (!_groundDetector.IsGrounded) return;

        var input = _input.MoveInput;
        if (input.sqrMagnitude < 0.01f) return;

        // Camera-relative direction
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

        var pelvisRb = _character.pelvis.Rb;
        Vector3 horizontalVel = pelvisRb.linearVelocity;
        horizontalVel.y = 0f;

        if (horizontalVel.magnitude < _maxSpeed)
        {
            pelvisRb.AddForce(moveDir * _moveForce, ForceMode.Force);
        }
    }
}
