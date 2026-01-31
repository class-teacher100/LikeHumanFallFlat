using UnityEngine;

[RequireComponent(typeof(RagdollCharacter))]
[RequireComponent(typeof(GroundDetector))]
public class JumpController : MonoBehaviour
{
    [SerializeField] PlayerInputHandler _input;

    RagdollCharacter _character;
    GroundDetector _groundDetector;

    float _jumpImpulse;

    void Awake()
    {
        _character = GetComponent<RagdollCharacter>();
        _groundDetector = GetComponent<GroundDetector>();
    }

    void Start()
    {
        _jumpImpulse = _character.definition.jumpImpulse;
    }

    void FixedUpdate()
    {
        if (!_input.JumpPressed) return;
        if (!_groundDetector.IsGrounded) return;

        _input.ConsumeJump();

        Vector3 impulse = Vector3.up * _jumpImpulse;

        // Apply to pelvis and both feet for a natural jump
        _character.pelvis.ApplyForce(impulse * 0.6f, ForceMode.Impulse);
        _character.leftFoot.ApplyForce(impulse * 0.2f, ForceMode.Impulse);
        _character.rightFoot.ApplyForce(impulse * 0.2f, ForceMode.Impulse);
    }
}
