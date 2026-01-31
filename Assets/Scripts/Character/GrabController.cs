using UnityEngine;

public class GrabController : MonoBehaviour
{
    [SerializeField] PlayerInputHandler _input;
    [SerializeField] RagdollCharacter _character;

    FixedJoint _leftGrabJoint;
    FixedJoint _rightGrabJoint;
    float _grabBreakForce;
    float _grabRange;

    void Start()
    {
        var def = _character.definition;
        _grabBreakForce = def.grabBreakForce;
        _grabRange = def.grabRange;
    }

    void FixedUpdate()
    {
        HandleGrab(true, _input.GrabLeftHeld, _character.leftHand, ref _leftGrabJoint);
        HandleGrab(false, _input.GrabRightHeld, _character.rightHand, ref _rightGrabJoint);
    }

    void HandleGrab(bool isLeft, bool held, BodyPart hand, ref FixedJoint grabJoint)
    {
        if (held && grabJoint == null)
        {
            TryGrab(hand, ref grabJoint);
        }
        else if (!held && grabJoint != null)
        {
            Release(ref grabJoint);
        }
    }

    void TryGrab(BodyPart hand, ref FixedJoint grabJoint)
    {
        var cols = UnityEngine.Physics.OverlapSphere(hand.transform.position, _grabRange);
        foreach (var col in cols)
        {
            // Can grab Grabbable objects or static geometry (walls, etc.)
            var grabbable = col.GetComponent<Grabbable>();
            var rb = col.attachedRigidbody;

            // Skip own body parts
            if (col.GetComponent<BodyPart>() != null) continue;

            if (grabbable != null || col.gameObject.isStatic || rb == null)
            {
                grabJoint = hand.gameObject.AddComponent<FixedJoint>();
                if (rb != null)
                    grabJoint.connectedBody = rb;

                grabJoint.breakForce = _grabBreakForce;
                grabJoint.breakTorque = _grabBreakForce;
                return;
            }
        }
    }

    void Release(ref FixedJoint grabJoint)
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
        }
    }

    void OnJointBreak(float breakForce)
    {
        // Clean up references when joints break naturally
        if (_leftGrabJoint == null) _leftGrabJoint = null;
        if (_rightGrabJoint == null) _rightGrabJoint = null;
    }
}
