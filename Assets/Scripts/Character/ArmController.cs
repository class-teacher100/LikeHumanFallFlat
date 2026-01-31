using UnityEngine;

public class ArmController : MonoBehaviour
{
    [SerializeField] PlayerInputHandler _input;
    [SerializeField] RagdollCharacter _character;
    [SerializeField] ArmReachTarget _reachTarget;
    [SerializeField] float _activeSpring = 2000f;
    [SerializeField] float _activeDamper = 100f;

    void FixedUpdate()
    {
        UpdateArm(true, _input.GrabLeftHeld);
        UpdateArm(false, _input.GrabRightHeld);
    }

    void UpdateArm(bool isLeft, bool isActive)
    {
        BodyPart upperArm = isLeft ? _character.leftUpperArm : _character.rightUpperArm;
        BodyPart lowerArm = isLeft ? _character.leftLowerArm : _character.rightLowerArm;

        if (!isActive)
        {
            // Return to rest pose
            var def = _character.definition;
            upperArm.SetJointDrive(def.shoulderSpring, def.shoulderDamper);
            upperArm.SetJointTargetRotation(Quaternion.identity);
            lowerArm.SetJointDrive(def.elbowSpring, def.elbowDamper);
            lowerArm.SetJointTargetRotation(Quaternion.identity);
            return;
        }

        // Increase drive strength when actively reaching
        upperArm.SetJointDrive(_activeSpring, _activeDamper);
        lowerArm.SetJointDrive(_activeSpring, _activeDamper);

        // Calculate target rotation for upper arm to point toward reach target
        Vector3 reachDir = _reachTarget.GetReachDirection(isLeft);
        Vector3 shoulderForward = upperArm.Joint.connectedBody.transform.up; // torso up

        Quaternion worldTarget = Quaternion.LookRotation(reachDir, Vector3.up);
        Quaternion connectedRotation = upperArm.Joint.connectedBody.transform.rotation;
        Quaternion localTarget = Quaternion.Inverse(connectedRotation) * worldTarget;

        // ConfigurableJoint target rotation is relative to initial pose
        upperArm.SetJointTargetRotation(Quaternion.Inverse(localTarget));

        // Straighten lower arm when reaching
        lowerArm.SetJointTargetRotation(Quaternion.identity);
    }
}
