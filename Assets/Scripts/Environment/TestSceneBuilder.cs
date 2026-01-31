using UnityEngine;

public class TestSceneBuilder : MonoBehaviour
{
    [SerializeField] CharacterDefinition _characterDefinition;

    void Start()
    {
        SetupLayers();
        CreateGround();
        CreateSteps();
        CreateGrabbables();
        CreateClimbingWall();
        CreateCharacter();
    }

    void SetupLayers()
    {
        // Layers must be set up in Project Settings manually:
        // Layer 6: Ground
        // Layer 7: Character
        // Layer 8: Grabbable
        // Disable Character-Character collisions in Physics settings
        int charLayer = LayerMask.NameToLayer("Character");
        if (charLayer >= 0)
            UnityEngine.Physics.IgnoreLayerCollision(charLayer, charLayer);
    }

    void CreateGround()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(10f, 1f, 10f);
        ground.layer = LayerMask.NameToLayer("Ground");
        ground.isStatic = true;
    }

    void CreateSteps()
    {
        float[] heights = { 0.5f, 1.0f, 1.5f };
        for (int i = 0; i < heights.Length; i++)
        {
            var step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.name = $"Step_{heights[i]}m";
            step.transform.position = new Vector3(3f + i * 2f, heights[i] / 2f, 0f);
            step.transform.localScale = new Vector3(1.5f, heights[i], 1.5f);
            step.layer = LayerMask.NameToLayer("Ground");
            step.isStatic = true;
        }
    }

    void CreateGrabbables()
    {
        int grabbableLayer = LayerMask.NameToLayer("Grabbable");

        // Boxes
        for (int i = 0; i < 3; i++)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = $"GrabbableBox_{i}";
            box.transform.position = new Vector3(-3f + i * 1.5f, 0.5f, 3f);
            box.transform.localScale = Vector3.one * 0.5f;
            box.layer = grabbableLayer;

            var rb = box.AddComponent<Rigidbody>();
            rb.mass = 5f;
            box.AddComponent<Grabbable>();
        }

        // Spheres
        for (int i = 0; i < 2; i++)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = $"GrabbableSphere_{i}";
            sphere.transform.position = new Vector3(-2f + i * 2f, 0.5f, 5f);
            sphere.transform.localScale = Vector3.one * 0.4f;
            sphere.layer = grabbableLayer;

            var rb = sphere.AddComponent<Rigidbody>();
            rb.mass = 3f;
            sphere.AddComponent<Grabbable>();
        }
    }

    void CreateClimbingWall()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");

        // Wall
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "ClimbingWall";
        wall.transform.position = new Vector3(0f, 3f, -5f);
        wall.transform.localScale = new Vector3(4f, 6f, 0.3f);
        wall.layer = groundLayer;
        wall.isStatic = true;

        // Ledges/protrusions for grabbing
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var ledge = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ledge.name = $"Ledge_{row}_{col}";
                ledge.transform.position = new Vector3(
                    -1.2f + col * 1.2f,
                    1f + row * 1.3f,
                    -4.7f
                );
                ledge.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
                ledge.layer = groundLayer;
                ledge.isStatic = true;
            }
        }
    }

    void CreateCharacter()
    {
        var charObj = new GameObject("Player");
        charObj.transform.position = new Vector3(0f, 2f, 0f);

        var ragdoll = charObj.AddComponent<RagdollCharacter>();
        ragdoll.definition = _characterDefinition;
        ragdoll.BuildRagdoll();

        charObj.AddComponent<BalanceController>();

        var groundDetector = charObj.AddComponent<GroundDetector>();

        // Input
        var inputObj = new GameObject("InputHandler");
        var inputHandler = inputObj.AddComponent<PlayerInputHandler>();

        // Movement
        var movement = charObj.AddComponent<MovementController>();
        // Camera is set up below, we'll wire it via serialized fields at runtime

        var jump = charObj.AddComponent<JumpController>();

        // Camera
        var camObj = new GameObject("ThirdPersonCamera");
        var cam = camObj.AddComponent<UnityEngine.Camera>();
        var tpCam = camObj.AddComponent<ThirdPersonCamera>();
        camObj.AddComponent<CameraCollision>();

        // Arm control
        var armReach = charObj.AddComponent<ArmReachTarget>();
        var armCtrl = charObj.AddComponent<ArmController>();
        var grabCtrl = charObj.AddComponent<GrabController>();

        // Debugger
        var debugger = charObj.AddComponent<PhysicsDebugger>();

        // Wire references via reflection (since SerializeField can't be set at runtime normally)
        // Use a helper to set private serialized fields
        SetField(movement, "_input", inputHandler);
        SetField(movement, "_cameraTransform", camObj.transform);
        SetField(jump, "_input", inputHandler);
        SetField(tpCam, "_input", inputHandler);
        SetField(tpCam, "_target", ragdoll);
        SetField(camObj.GetComponent<CameraCollision>(), "_target", ragdoll);
        SetField(armReach, "_cameraTransform", camObj.transform);
        SetField(armReach, "_character", ragdoll);
        SetField(armCtrl, "_input", inputHandler);
        SetField(armCtrl, "_character", ragdoll);
        SetField(armCtrl, "_reachTarget", armReach);
        SetField(grabCtrl, "_input", inputHandler);
        SetField(grabCtrl, "_character", ragdoll);
        SetField(debugger, "_character", ragdoll);
        SetField(debugger, "_groundDetector", groundDetector);
        SetField(groundDetector, "_groundLayer", LayerMask.GetMask("Ground"));

        // Destroy the default Main Camera if it exists
        var mainCam = UnityEngine.Camera.main;
        if (mainCam != null && mainCam.gameObject != camObj)
            Destroy(mainCam.gameObject);
    }

    static void SetField(object target, string fieldName, object value)
    {
        var type = target.GetType();
        while (type != null)
        {
            var field = type.GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }
            type = type.BaseType;
        }
        Debug.LogWarning($"Field '{fieldName}' not found on {target.GetType().Name}");
    }
}
