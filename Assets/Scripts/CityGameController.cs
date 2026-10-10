using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public sealed class CityGameController : MonoBehaviour
{
    public enum GamePhase
    {
        ExploringSubway,
        SalesmanApproaching,
        SalesmanDialogue,
        BriefcaseOpening,
        DdakjiAiming,
        DdakjiThrowing,
        SlapCinematic,
        VictoryWon,
        InspectCard,
        GoToShuttle,
        ShuttleBoarding,
        IslandTransitionComplete
    }

    // World Coordinates (Subway Platform & Street Above)
    private const float SubwayFloorY = 0.05f;
    private const float StreetFloorY = 4.5f;
    private const float PlayerStartPlatformZ = 2.0f;
    private const float SalesmanStartPlatformZ = 13.0f;
    private const float ShuttleZ = 68.0f;

    // References
    [SerializeField] private Transform playerRoot;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Transform salesmanRoot;
    [SerializeField] private Transform islandVanRoot;
    [SerializeField] private Camera gameCamera;

    // Player Multi-Joint Skeletal Hierarchy
    private Transform playerHips;
    private Transform playerTorso;
    private Transform playerHead;
    private Transform playerRightUpperArm;
    private Transform playerRightForearm;
    private Transform playerRightHand;
    private Transform playerLeftUpperArm;
    private Transform playerLeftForearm;
    private Transform playerLeftHand;
    private Transform playerRightThigh;
    private Transform playerRightShin;
    private Transform playerRightFoot;
    private Transform playerLeftThigh;
    private Transform playerLeftShin;
    private Transform playerLeftFoot;

    // Salesman Multi-Joint Skeletal Hierarchy
    private Transform salesmanHips;
    private Transform salesmanTorso;
    private Transform salesmanHead;
    private Transform salesmanRightUpperArm;
    private Transform salesmanRightForearm;
    private Transform salesmanRightHand;
    private Transform salesmanLeftUpperArm;
    private Transform salesmanLeftForearm;
    private Transform salesmanLeftHand;
    private Transform salesmanRightThigh;
    private Transform salesmanRightShin;
    private Transform salesmanRightFoot;
    private Transform salesmanLeftThigh;
    private Transform salesmanLeftShin;
    private Transform salesmanLeftFoot;
    private Transform salesmanBriefcase;
    private Transform briefcaseLid;
    private Transform cashStack;
    private Transform redDdakjiBriefcase;
    private Transform blueDdakjiBriefcase;
    private float salesmanWalkCycle = 0f;

    // Pink Guard Skeletal Hierarchy
    private Transform pinkGuardRoot;
    private Transform pinkGuardHips;
    private Transform pinkGuardTorso;
    private Transform pinkGuardHead;
    private Transform pinkGuardRightArm;
    private Transform pinkGuardLeftArm;

    // Ddakji Minigame World Objects
    private Transform redDdakjiGround;
    private Transform blueDdakjiHand;
    private Transform blueDdakjiThrown;
    private Transform squidCardHand;

    // Game State
    private GamePhase currentPhase = GamePhase.ExploringSubway;
    private int slapsEndured = 0;
    private int wonCashKRW = 0;
    private bool hasWonCard = false;
    private string currentDialogueText = "";
    private string dialogueSpeaker = "";
    private float dialogueTimer = 0f;

    // Ddakji Gauge
    private float ddakjiPower = 0f;
    private float ddakjiPowerDirection = 1.7f;
    private const float SweetSpotStart = 0.60f;
    private const float SweetSpotEnd = 0.82f;

    // Slap Cinematic State
    private float slapTimer = 0f;
    private float screenFlashAlpha = 0f;
    private float cameraShakeIntensity = 0f;
    private float headSlapAngle = 0f;

    // Third Person Camera
    private float camYaw = 0f;
    private float camPitch = 14f;
    private float camDistance = 4.2f;
    private Vector3 currentCamLookTarget;

    // Procedural Locomotion State
    private float walkCycle = 0f;
    private Vector3 playerVelocity;
    private AudioSource audioSource;
    private AudioSource rainAudioSource;

    // Materials Cache
    private Material wetAsphaltMat;
    private Material sidewalkMat;
    private Material subwayTileMat;
    private Material tactileYellowMat;
    private Material seoulTaxiMat;
    private Material carBlackMat;
    private Material carGlassMat;
    private Material suitMat;
    private Material briefcaseMat;
    private Material ddakjiRedMat;
    private Material ddakjiBlueMat;
    private Material squidCardFrontMat;
    private Material squidCardBackMat;
    private Material cashMat;
    private Material neonConvenienceMat;
    private Material neonNoraebangMat;
    private Material neonSubwayMat;
    private Material billboardSquidMat;
    private Material warmGlowMat;
    private Material pinkGuardMat;
    private Material metalBenchMat;
    private Material trainTracksMat;
    private Material skinMat;
    private Material shirtMat;
    private Material tieMat;
    private Material parkaMat;
    private Material jeansMat;
    private Material sneakerMat;
    private Material hairMat;
    private Material guardMaskMat;

    // 3D Card Inspection Rotation
    private Vector2 cardInspectRot = new Vector2(0f, 0f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartGameIfMissing()
    {
        if (FindAnyObjectByType<CityGameController>() != null) return;
        var city = new GameObject("City Squid Game");
        var audio = city.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        city.AddComponent<CityGameController>().BuildFullCity();
    }

    private void Awake()
    {
        EnsureAudioSource();
    }

    private void Start()
    {
        EnsureAudioSource();
        BuildFullCity();
        InitGameFlow();
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        if (rainAudioSource == null)
        {
            var child = transform.Find("Rain Audio Loop");
            GameObject rainObj;
            if (child == null)
            {
                rainObj = new GameObject("Rain Audio Loop");
                rainObj.transform.SetParent(transform, false);
            }
            else
            {
                rainObj = child.gameObject;
            }

            rainAudioSource = rainObj.GetComponent<AudioSource>();
            if (rainAudioSource == null)
            {
                rainAudioSource = rainObj.AddComponent<AudioSource>();
            }

            rainAudioSource.loop = true;
            rainAudioSource.volume = 0.28f;
            rainAudioSource.clip = SquidGameAudio.GetRain();
            if (Application.isPlaying && !rainAudioSource.isPlaying)
            {
                rainAudioSource.Play();
            }
        }
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        EnsureAudioSource();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void BuildScene() => BuildFullCity();

    public void BuildFullCity()
    {
        PurgeLegacyObjects();
        InitMaterials();
        BuildSeoulSubwayStationPlatform();
        BuildStreetLevelAndCity();
        BuildIslandShuttleVan();
        BuildPlayerCharacter();
        BuildSalesmanCharacter();
        BuildDdakjiArena();
        SetupRainAndLighting();
    }

    private void PurgeLegacyObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            if (child.name == "Rain Audio Loop") continue;
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }

        playerRoot = null;
        playerController = null;
        salesmanRoot = null;
        islandVanRoot = null;

        var allObjects = FindObjectsByType<GameObject>();
        foreach (var go in allObjects)
        {
            if (go == null || go == this.gameObject || go.transform.IsChildOf(this.transform)) continue;
            if (go.GetComponent<Camera>() != null || go.GetComponent<Light>() != null) continue;
            string n = go.name;
            if (n.StartsWith("Main asphalt") || n.StartsWith("Apartment") || n.StartsWith("Crosswalk") ||
                n.StartsWith("Car ") || n.StartsWith("Tree ") || n.StartsWith("Street") ||
                n.StartsWith("Right sidewalk") || n.StartsWith("Left sidewalk") || n.StartsWith("Center lane") ||
                n.StartsWith("Shop") || n.StartsWith("Parked") || n.StartsWith("Island transfer") ||
                n.StartsWith("Finish") || n.StartsWith("Signal") || n.StartsWith("Shoe") || n.StartsWith("Arm") ||
                n.StartsWith("Leg") || n.StartsWith("Eye") || n.StartsWith("Waiting") || n.StartsWith("Bus shelter") ||
                n.StartsWith("Roadside curb") || n.StartsWith("City ground") || n.StartsWith("Shuttle") ||
                n.StartsWith("Ddakji Recruiter") || n.StartsWith("Player") || n.StartsWith("Briefcase") ||
                n.StartsWith("Subway") || n.StartsWith("Station") || n.StartsWith("Platform") ||
                n.StartsWith("Seoul") || n.StartsWith("The Recruiter") || n.StartsWith("Masked Pink"))
            {
                if (Application.isPlaying) Destroy(go);
                else DestroyImmediate(go);
            }
        }
    }

    private void InitMaterials()
    {
        wetAsphaltMat = MakeMaterial(new Color(0.12f, 0.13f, 0.14f), 0.85f, 0.15f, SquidGameTextures.GetAsphalt(), SquidGameTextures.GetAsphaltNormal());
        sidewalkMat = MakeMaterial(new Color(0.68f, 0.67f, 0.65f), 0.35f, 0.05f, SquidGameTextures.GetSidewalk(), SquidGameTextures.GetSidewalkNormal());
        subwayTileMat = MakeMaterial(new Color(0.92f, 0.94f, 0.96f), 0.78f, 0.08f, SquidGameTextures.GetSubwayWall());
        tactileYellowMat = MakeMaterial(new Color(0.92f, 0.78f, 0.12f), 0.45f, 0.05f, SquidGameTextures.GetTactileYellow());
        seoulTaxiMat = MakeMaterial(new Color(0.95f, 0.45f, 0.05f), 0.9f, 0.7f);
        carBlackMat = MakeMaterial(new Color(0.08f, 0.08f, 0.09f), 0.92f, 0.8f);
        carGlassMat = MakeMaterial(new Color(0.1f, 0.14f, 0.18f), 0.95f, 0.1f);
        suitMat = MakeMaterial(new Color(0.14f, 0.15f, 0.18f), 0.25f, 0.05f, SquidGameTextures.GetSuitWool());
        briefcaseMat = MakeMaterial(new Color(0.12f, 0.09f, 0.08f), 0.65f, 0.15f, SquidGameTextures.GetBriefcaseLeather());
        ddakjiRedMat = MakeMaterial(Color.white, 0.4f, 0.0f, SquidGameTextures.GetDdakjiRed());
        ddakjiBlueMat = MakeMaterial(Color.white, 0.4f, 0.0f, SquidGameTextures.GetDdakjiBlue());
        squidCardFrontMat = MakeMaterial(Color.white, 0.5f, 0.0f, SquidGameTextures.GetCardFront());
        squidCardBackMat = MakeMaterial(Color.white, 0.5f, 0.0f, SquidGameTextures.GetCardBack());
        cashMat = MakeMaterial(Color.white, 0.25f, 0.0f, SquidGameTextures.GetCash());
        pinkGuardMat = MakeMaterial(new Color(0.98f, 0.12f, 0.48f), 0.3f, 0.05f);
        guardMaskMat = MakeMaterial(new Color(0.06f, 0.06f, 0.07f), 0.45f, 0.25f);
        metalBenchMat = MakeMaterial(new Color(0.72f, 0.74f, 0.78f), 0.85f, 0.85f);
        trainTracksMat = MakeMaterial(new Color(0.32f, 0.34f, 0.36f), 0.6f, 0.7f);

        skinMat = MakeMaterial(new Color(0.86f, 0.71f, 0.58f), 0.3f, 0.0f);
        shirtMat = MakeMaterial(new Color(0.95f, 0.95f, 0.96f), 0.25f, 0.0f);
        tieMat = MakeMaterial(new Color(0.55f, 0.08f, 0.12f), 0.55f, 0.08f);
        parkaMat = MakeMaterial(new Color(0.15f, 0.26f, 0.22f), 0.3f, 0.0f);
        jeansMat = MakeMaterial(new Color(0.17f, 0.22f, 0.32f), 0.25f, 0.0f);
        sneakerMat = MakeMaterial(new Color(0.88f, 0.88f, 0.86f), 0.35f, 0.0f);
        hairMat = MakeMaterial(new Color(0.08f, 0.07f, 0.06f), 0.4f, 0.05f);

        neonConvenienceMat = MakeEmissive(SquidGameTextures.GetConvenienceSign(), new Color(0.1f, 0.85f, 0.95f) * 2.2f);
        neonNoraebangMat = MakeEmissive(SquidGameTextures.GetNoraebangSign(), new Color(1.0f, 0.15f, 0.6f) * 2.2f);
        neonSubwayMat = MakeEmissive(SquidGameTextures.GetSubwaySign(), new Color(0.3f, 0.7f, 1.2f) * 2.0f);
        billboardSquidMat = MakeEmissive(SquidGameTextures.GetSquidBillboard(), new Color(1.0f, 0.15f, 0.55f) * 2.5f);
        warmGlowMat = MakeEmissive(null, new Color(1.0f, 0.88f, 0.65f) * 1.9f);
    }

    private void InitGameFlow()
    {
        currentPhase = GamePhase.ExploringSubway;
        slapsEndured = 0;
        wonCashKRW = 0;
        hasWonCard = false;
        SetDialogue("", "");

        // Player begins standing on the Subway Station Platform
        if (playerRoot != null && playerController != null)
        {
            playerController.enabled = false;
            playerRoot.position = new Vector3(1.5f, SubwayFloorY, PlayerStartPlatformZ);
            playerRoot.rotation = Quaternion.identity;
            playerController.enabled = true;
        }

        // Salesman stands down the platform holding his briefcase
        if (salesmanRoot != null)
        {
            salesmanRoot.position = new Vector3(1.5f, SubwayFloorY, SalesmanStartPlatformZ);
            salesmanRoot.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if (salesmanBriefcase != null && salesmanLeftHand != null)
        {
            salesmanBriefcase.SetParent(salesmanLeftHand, false);
            salesmanBriefcase.localPosition = new Vector3(-0.06f, -0.22f, 0.08f);
            salesmanBriefcase.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }

        if (briefcaseLid != null) briefcaseLid.localRotation = Quaternion.identity;
        if (cashStack != null) cashStack.gameObject.SetActive(false);
        if (redDdakjiBriefcase != null) redDdakjiBriefcase.gameObject.SetActive(false);
        if (blueDdakjiBriefcase != null) blueDdakjiBriefcase.gameObject.SetActive(false);
        if (redDdakjiGround != null) redDdakjiGround.gameObject.SetActive(false);
        if (blueDdakjiHand != null) blueDdakjiHand.gameObject.SetActive(false);
        if (blueDdakjiThrown != null) blueDdakjiThrown.gameObject.SetActive(false);
        if (squidCardHand != null) squidCardHand.gameObject.SetActive(false);

        camYaw = 0f;
        camPitch = 14f;
        camDistance = 4.2f;
    }

    private void Update()
    {
        EnsurePlayerStanding();
        HandleKeyboardInput();
        UpdatePhaseLogic();
        UpdateCameraOrbit();
        UpdateVisualEffects();
        AnimateSalesman(Time.deltaTime);
        AnimatePinkGuard();
    }

    private void EnsurePlayerStanding()
    {
        if (playerRoot == null) return;

        // Catch falling below subway platform
        if (playerRoot.position.y < -0.05f && playerRoot.position.z < 35f)
        {
            if (playerController != null) playerController.enabled = false;
            playerRoot.position = new Vector3(playerRoot.position.x, SubwayFloorY, playerRoot.position.z);
            if (playerController != null) playerController.enabled = true;
            playerVelocity.y = -1.0f;
        }
        // Catch falling below street level when on street
        else if (playerRoot.position.z >= 42f && playerRoot.position.y < StreetFloorY - 0.2f)
        {
            if (playerController != null) playerController.enabled = false;
            playerRoot.position = new Vector3(playerRoot.position.x, StreetFloorY + 0.05f, playerRoot.position.z);
            if (playerController != null) playerController.enabled = true;
            playerVelocity.y = -1.0f;
        }
    }

    private void HandleKeyboardInput()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null) return;

        // Camera Orbit Controls
        if (mouse != null && (mouse.rightButton.isPressed || (mouse.leftButton.isPressed && currentPhase == GamePhase.InspectCard)))
        {
            Vector2 delta = mouse.delta.ReadValue();
            if (currentPhase == GamePhase.InspectCard)
            {
                cardInspectRot += delta * 0.45f;
            }
            else
            {
                camYaw += delta.x * 0.22f;
                camPitch = Mathf.Clamp(camPitch - delta.y * 0.18f, -15f, 65f);
            }
        }

        // Mouse scroll zoom
        if (mouse != null)
        {
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                camDistance = Mathf.Clamp(camDistance - Mathf.Sign(scroll) * 0.6f, 2.0f, 7.5f);
            }
        }

        bool interactPressed = kb.eKey.wasPressedThisFrame || (mouse != null && mouse.leftButton.wasPressedThisFrame && currentPhase != GamePhase.InspectCard);
        bool spacePressed = kb.spaceKey.wasPressedThisFrame;

        // Fast Stairs Ascent Assist (Press E near stairs at Z >= 21m to ascend immediately to street)
        if (playerRoot != null && interactPressed && playerRoot.position.z >= 20f && playerRoot.position.z <= 32f && playerRoot.position.y < 2.5f)
        {
            AscendPlayerToStreet();
            return;
        }

        // State Machine Actions
        switch (currentPhase)
        {
            case GamePhase.SalesmanApproaching:
                if (interactPressed || spacePressed)
                {
                    StartDialoguePhase();
                }
                break;

            case GamePhase.SalesmanDialogue:
                if (interactPressed || spacePressed)
                {
                    StartBriefcaseOpenPhase();
                }
                break;

            case GamePhase.BriefcaseOpening:
                if (interactPressed || spacePressed)
                {
                    StartDdakjiAimingPhase();
                }
                break;

            case GamePhase.DdakjiAiming:
                if (interactPressed || spacePressed)
                {
                    ExecuteDdakjiThrow();
                }
                break;

            case GamePhase.SlapCinematic:
                if (slapTimer >= 2.2f && (interactPressed || spacePressed))
                {
                    StartDdakjiAimingPhase();
                }
                break;

            case GamePhase.VictoryWon:
                if (interactPressed || spacePressed)
                {
                    currentPhase = GamePhase.InspectCard;
                    cardInspectRot = Vector2.zero;
                    if (squidCardHand != null) squidCardHand.gameObject.SetActive(true);
                    SetDialogue("3D CARD INSPECT", "Click & Drag mouse to rotate the Squid Game Business Card. Press [E] to pocket card.");
                }
                break;

            case GamePhase.InspectCard:
                if (interactPressed || kb.escapeKey.wasPressedThisFrame || spacePressed)
                {
                    currentPhase = GamePhase.GoToShuttle;
                    if (squidCardHand != null) squidCardHand.gameObject.SetActive(false);
                    SetDialogue("ASCEND TO STREET LEVEL", "Head up the subway stairs (Z: 24m->42m) to the rainy street above. The Black VIP Van awaits!");
                    PlaySound(SquidGameAudio.GetThemeMotif(), 0.9f);
                }
                break;

            case GamePhase.GoToShuttle:
                float distToVan = Vector3.Distance(playerRoot.position, islandVanRoot.position);
                if (distToVan <= 6.5f && interactPressed)
                {
                    StartShuttleBoarding();
                }
                break;

            case GamePhase.IslandTransitionComplete:
                if (interactPressed || kb.rKey.wasPressedThisFrame || spacePressed)
                {
                    InitGameFlow();
                }
                break;
        }

        // Player Locomotion in Exploration & Walking phases
        if (currentPhase == GamePhase.ExploringSubway || currentPhase == GamePhase.SalesmanApproaching || currentPhase == GamePhase.GoToShuttle)
        {
            UpdatePlayerMovement(kb);
        }
    }

    private void AscendPlayerToStreet()
    {
        if (playerRoot == null) return;
        if (playerController != null) playerController.enabled = false;
        playerRoot.position = new Vector3(2.0f, StreetFloorY + 0.05f, 44.0f);
        if (playerController != null) playerController.enabled = true;
        SetDialogue("STREET LEVEL REACHED", "You have ascended to the street! Head forward to board the Black VIP Island Shuttle Van.");
        PlaySound(SquidGameAudio.GetThemeMotif(), 0.85f);
    }

    private void UpdatePlayerMovement(Keyboard kb)
    {
        if (playerRoot == null || playerController == null || gameCamera == null) return;

        float moveX = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float moveZ = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        bool isSprinting = kb.leftShiftKey.isPressed;

        Vector3 camForward = Vector3.ProjectOnPlane(gameCamera.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(gameCamera.transform.right, Vector3.up).normalized;
        Vector3 moveDir = (camForward * moveZ + camRight * moveX);

        float targetSpeed = isSprinting ? 6.2f : 3.4f;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 targetVel = moveDir * targetSpeed;
        playerVelocity.x = Mathf.Lerp(playerVelocity.x, targetVel.x, Time.deltaTime * 12f);
        playerVelocity.z = Mathf.Lerp(playerVelocity.z, targetVel.z, Time.deltaTime * 12f);

        if (playerController.isGrounded)
        {
            playerVelocity.y = -2.5f; // Solid grounding snap
        }
        else
        {
            playerVelocity.y = Mathf.Max(-25f, playerVelocity.y - 18f * Time.deltaTime);
        }

        playerController.Move(playerVelocity * Time.deltaTime);

        // Turn character towards moving direction
        if (moveDir.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            playerRoot.rotation = Quaternion.Slerp(playerRoot.rotation, targetRot, Time.deltaTime * 12f);
            walkCycle += Time.deltaTime * (isSprinting ? 12f : 7.2f);

            if (Mathf.Sin(walkCycle) > 0.95f)
            {
                PlaySound(SquidGameAudio.GetFootstep(), 0.35f);
            }
        }
        else
        {
            walkCycle = Mathf.Lerp(walkCycle, 0f, Time.deltaTime * 6f);
        }

        AnimatePlayerLimbs(walkCycle, moveDir.magnitude, isSprinting);
    }

    private void AnimatePlayerLimbs(float cycle, float speedNorm, bool isSprinting)
    {
        if (playerHips == null || playerRightThigh == null || playerLeftThigh == null) return;

        float time = Time.time;

        if (speedNorm < 0.05f)
        {
            // LIVING IDLE ANIMATION
            float breathe = Mathf.Sin(time * 2.2f);
            float weightShift = Mathf.Sin(time * 0.9f);

            playerHips.localPosition = new Vector3(weightShift * 0.015f, 0.95f + breathe * 0.008f, 0f);
            playerHips.localRotation = Quaternion.Euler(0f, 0f, weightShift * 0.8f);
            playerTorso.localRotation = Quaternion.Euler(breathe * 1.5f, 0f, -weightShift * 0.5f);

            if (currentPhase != GamePhase.InspectCard && currentPhase != GamePhase.DdakjiAiming)
            {
                playerRightUpperArm.localRotation = Quaternion.Euler(4f + breathe * 1.0f, 0f, -8f);
                playerRightForearm.localRotation = Quaternion.Euler(18f, 0f, 0f);
                playerLeftUpperArm.localRotation = Quaternion.Euler(4f + breathe * 1.0f, 0f, 8f);
                playerLeftForearm.localRotation = Quaternion.Euler(18f, 0f, 0f);
            }

            playerRightThigh.localRotation = Quaternion.Euler(0f, 0f, -weightShift * 0.6f);
            playerRightShin.localRotation = Quaternion.identity;
            playerRightFoot.localRotation = Quaternion.identity;

            playerLeftThigh.localRotation = Quaternion.Euler(0f, 0f, -weightShift * 0.6f);
            playerLeftShin.localRotation = Quaternion.identity;
            playerLeftFoot.localRotation = Quaternion.identity;
        }
        else
        {
            // DYNAMIC LOCOMOTION (WALK & SPRINT KINEMATICS WITH KNEE FLEXION)
            float strideAmp = isSprinting ? 40f : 28f;
            float legSwingR = Mathf.Sin(cycle) * strideAmp * speedNorm;
            float legSwingL = -legSwingR;

            float bounce = -Mathf.Abs(Mathf.Sin(cycle)) * (isSprinting ? 0.055f : 0.035f);
            float hipSway = Mathf.Sin(cycle) * (isSprinting ? 0.030f : 0.018f);
            playerHips.localPosition = new Vector3(hipSway, 0.95f + bounce, 0f);
            playerHips.localRotation = Quaternion.Euler(0f, Mathf.Sin(cycle) * 3.5f, -hipSway * 35f);

            float torsoLean = isSprinting ? 12f : 6f;
            playerTorso.localRotation = Quaternion.Euler(torsoLean, -Mathf.Sin(cycle) * 3f, 0f);

            // Right Leg
            playerRightThigh.localRotation = Quaternion.Euler(legSwingR, 0f, 0f);
            float kneeR = (legSwingR < 0f) ? Mathf.Sin(-legSwingR * Mathf.Deg2Rad) * (isSprinting ? 68f : 48f) : 4f;
            playerRightShin.localRotation = Quaternion.Euler(kneeR, 0f, 0f);
            float footR = (legSwingR > 0f) ? 14f : -18f;
            playerRightFoot.localRotation = Quaternion.Euler(footR, 0f, 0f);

            // Left Leg
            playerLeftThigh.localRotation = Quaternion.Euler(legSwingL, 0f, 0f);
            float kneeL = (legSwingL < 0f) ? Mathf.Sin(-legSwingL * Mathf.Deg2Rad) * (isSprinting ? 68f : 48f) : 4f;
            playerLeftShin.localRotation = Quaternion.Euler(kneeL, 0f, 0f);
            float footL = (legSwingL > 0f) ? 14f : -18f;
            playerLeftFoot.localRotation = Quaternion.Euler(footL, 0f, 0f);

            // Arms Counter-Swing with Elbow Flexion
            if (currentPhase != GamePhase.InspectCard && currentPhase != GamePhase.DdakjiAiming)
            {
                float armSwingR = -legSwingR * (isSprinting ? 1.2f : 0.95f);
                float armSwingL = -legSwingL * (isSprinting ? 1.2f : 0.95f);

                playerRightUpperArm.localRotation = Quaternion.Euler(armSwingR, 0f, -8f);
                float elbowR = Mathf.Clamp(armSwingR * 0.7f + 25f, 15f, 75f);
                playerRightForearm.localRotation = Quaternion.Euler(elbowR, 0f, 0f);

                playerLeftUpperArm.localRotation = Quaternion.Euler(armSwingL, 0f, 8f);
                float elbowL = Mathf.Clamp(armSwingL * 0.7f + 25f, 15f, 75f);
                playerLeftForearm.localRotation = Quaternion.Euler(elbowL, 0f, 0f);
            }
        }
    }

    private void AnimateSalesman(float dt)
    {
        if (salesmanHips == null || salesmanTorso == null) return;

        float time = Time.time;

        if (currentPhase == GamePhase.SalesmanApproaching)
        {
            salesmanWalkCycle += dt * 7.5f;
            float swing = Mathf.Sin(salesmanWalkCycle) * 26f;

            salesmanHips.localPosition = new Vector3(0f, 0.96f - Mathf.Abs(Mathf.Sin(salesmanWalkCycle)) * 0.030f, 0f);

            salesmanRightThigh.localRotation = Quaternion.Euler(swing, 0f, 0f);
            salesmanRightShin.localRotation = Quaternion.Euler((swing < 0f) ? -swing * 1.5f : 4f, 0f, 0f);
            salesmanRightFoot.localRotation = Quaternion.Euler((swing > 0f) ? 12f : -14f, 0f, 0f);

            salesmanLeftThigh.localRotation = Quaternion.Euler(-swing, 0f, 0f);
            salesmanLeftShin.localRotation = Quaternion.Euler((-swing < 0f) ? swing * 1.5f : 4f, 0f, 0f);
            salesmanLeftFoot.localRotation = Quaternion.Euler((-swing > 0f) ? 12f : -14f, 0f, 0f);

            // Right arm swings naturally; left arm holds briefcase firmly
            salesmanRightUpperArm.localRotation = Quaternion.Euler(-swing * 0.85f, 0f, -8f);
            salesmanRightForearm.localRotation = Quaternion.Euler(20f, 0f, 0f);

            salesmanLeftUpperArm.localRotation = Quaternion.Euler(8f, 0f, 8f);
            salesmanLeftForearm.localRotation = Quaternion.Euler(14f, 0f, 0f);
        }
        else if (currentPhase != GamePhase.SlapCinematic)
        {
            // IDLE & GESTURING
            float breathe = Mathf.Sin(time * 2.0f);
            salesmanHips.localPosition = new Vector3(0f, 0.96f + breathe * 0.006f, 0f);
            salesmanTorso.localRotation = Quaternion.Euler(breathe * 1.2f, Mathf.Sin(time * 0.8f) * 2f, 0f);

            salesmanRightUpperArm.localRotation = Quaternion.Euler(6f + breathe * 1.5f, 0f, -10f);
            salesmanRightForearm.localRotation = Quaternion.Euler(20f, 0f, 0f);

            salesmanLeftUpperArm.localRotation = Quaternion.Euler(8f, 0f, 8f);
            salesmanLeftForearm.localRotation = Quaternion.Euler(15f, 0f, 0f);

            salesmanRightThigh.localRotation = Quaternion.identity;
            salesmanRightShin.localRotation = Quaternion.identity;
            salesmanLeftThigh.localRotation = Quaternion.identity;
            salesmanLeftShin.localRotation = Quaternion.identity;
        }
    }

    private void AnimatePinkGuard()
    {
        if (pinkGuardTorso == null) return;
        float breathe = Mathf.Sin(Time.time * 1.8f);
        pinkGuardTorso.localRotation = Quaternion.Euler(breathe * 1.2f, 0f, 0f);
    }

    private void UpdatePhaseLogic()
    {
        float distToSalesman = Vector3.Distance(playerRoot.position, salesmanRoot.position);

        switch (currentPhase)
        {
            case GamePhase.ExploringSubway:
                if (distToSalesman <= 9.0f)
                {
                    currentPhase = GamePhase.SalesmanApproaching;
                }
                break;

            case GamePhase.SalesmanApproaching:
                // Salesman steps smoothly toward player along the platform
                Vector3 toPlayer = (playerRoot.position - salesmanRoot.position);
                toPlayer.y = 0f;
                if (toPlayer.magnitude > 2.2f)
                {
                    salesmanRoot.position += toPlayer.normalized * (1.8f * Time.deltaTime);
                }
                if (toPlayer.sqrMagnitude > 0.01f)
                {
                    salesmanRoot.rotation = Quaternion.Slerp(salesmanRoot.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * 6f);
                }

                if (distToSalesman <= 3.6f)
                {
                    SetDialogue("THE RECRUITER (SALESMAN)", "Excuse me, sir. Do you have a minute to play a game? [Press E to Talk]");
                }
                break;

            case GamePhase.DdakjiAiming:
                ddakjiPower += ddakjiPowerDirection * Time.deltaTime;
                if (ddakjiPower >= 1f)
                {
                    ddakjiPower = 1f;
                    ddakjiPowerDirection = -Mathf.Abs(ddakjiPowerDirection);
                }
                else if (ddakjiPower <= 0f)
                {
                    ddakjiPower = 0f;
                    ddakjiPowerDirection = Mathf.Abs(ddakjiPowerDirection);
                }
                break;

            case GamePhase.SlapCinematic:
                slapTimer += Time.deltaTime;
                UpdateSlapAnimation();
                break;

            case GamePhase.InspectCard:
                if (squidCardHand != null && gameCamera != null)
                {
                    squidCardHand.position = gameCamera.transform.position + gameCamera.transform.forward * 0.48f + gameCamera.transform.up * -0.04f;
                    squidCardHand.rotation = gameCamera.transform.rotation * Quaternion.Euler(cardInspectRot.y, -cardInspectRot.x, 0f);
                }
                break;
        }
    }

    private void StartDialoguePhase()
    {
        currentPhase = GamePhase.SalesmanDialogue;
        PlaySound(SquidGameAudio.GetSubwayBell(), 0.75f);

        Vector3 pToS = (salesmanRoot.position - playerRoot.position); pToS.y = 0f;
        playerRoot.rotation = Quaternion.LookRotation(pToS);
        salesmanRoot.rotation = Quaternion.LookRotation(-pToS);

        SetDialogue("THE RECRUITER (SALESMAN)",
            "\"Sir, I can see you are deep in debt. How about we play a round of Ddakji right here on the platform?\nWin and earn ₩100,000 Won. Lose and you pay ₩100,000. No cash? You can pay with your body.\" [Press E / Space to Accept]");
    }

    private void StartBriefcaseOpenPhase()
    {
        currentPhase = GamePhase.BriefcaseOpening;
        PlaySound(SquidGameAudio.GetBriefcase(), 0.9f);

        // Place briefcase right on the subway platform floor between them
        Vector3 midPoint = (playerRoot.position + salesmanRoot.position) * 0.5f;
        midPoint.y = SubwayFloorY + 0.02f;
        salesmanBriefcase.SetParent(transform, true);
        salesmanBriefcase.position = midPoint;
        salesmanBriefcase.rotation = Quaternion.LookRotation(playerRoot.position - salesmanRoot.position);

        if (briefcaseLid != null)
        {
            briefcaseLid.localRotation = Quaternion.Euler(-105f, 0f, 0f);
        }

        if (cashStack != null) cashStack.gameObject.SetActive(true);
        if (redDdakjiBriefcase != null) redDdakjiBriefcase.gameObject.SetActive(true);
        if (blueDdakjiBriefcase != null) blueDdakjiBriefcase.gameObject.SetActive(true);

        PlaySound(SquidGameAudio.GetCash(), 0.8f);

        SetDialogue("THE RECRUITER (SALESMAN)",
            "\"Here is ₩100,000 won in crisp bills, and two Ddakji tiles: Red and Blue.\"\n\"Pick your tile. Slam it onto mine to flip it!\" [Press E / Space to Start Challenge]");
    }

    private void StartDdakjiAimingPhase()
    {
        currentPhase = GamePhase.DdakjiAiming;
        ddakjiPower = 0f;
        ddakjiPowerDirection = 2.0f;

        Vector3 floorPos = (playerRoot.position + salesmanRoot.position) * 0.5f + new Vector3(0f, 0f, 0.4f);
        floorPos.y = SubwayFloorY + 0.015f;
        redDdakjiGround.position = floorPos;
        redDdakjiGround.rotation = Quaternion.Euler(0f, 25f, 0f);
        redDdakjiGround.gameObject.SetActive(true);

        blueDdakjiHand.gameObject.SetActive(true);
        playerRightUpperArm.localRotation = Quaternion.Euler(-80f, -15f, 35f);
        playerRightForearm.localRotation = Quaternion.Euler(45f, 0f, 0f);

        SetDialogue("DDAKJI CHALLENGE - SUBWAY PLATFORM",
            "Time your throw! Slam when the moving gauge enters the GREEN SWEET SPOT [Press SPACE or E]");
    }

    private void ExecuteDdakjiThrow()
    {
        currentPhase = GamePhase.DdakjiThrowing;
        blueDdakjiHand.gameObject.SetActive(false);

        bool isSuccess = (ddakjiPower >= SweetSpotStart && ddakjiPower <= SweetSpotEnd);
        StartCoroutine(AnimateThrowSequence(isSuccess));
    }

    private IEnumerator AnimateThrowSequence(bool isSuccess)
    {
        playerRightUpperArm.localRotation = Quaternion.Euler(75f, 10f, 15f);
        playerRightForearm.localRotation = Quaternion.Euler(15f, 0f, 0f);

        Vector3 startThrowPos = playerRightUpperArm.position + new Vector3(0f, 0.4f, 0.2f);
        Vector3 landPos = redDdakjiGround.position + new Vector3(0.04f, 0.005f, -0.05f);

        blueDdakjiThrown.position = startThrowPos;
        blueDdakjiThrown.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6.5f;
            blueDdakjiThrown.position = Vector3.Lerp(startThrowPos, landPos, t);
            yield return null;
        }

        PlaySound(SquidGameAudio.GetDdakjiSlam(), 1.0f);
        cameraShakeIntensity = 0.35f;

        yield return new WaitForSeconds(0.2f);

        if (isSuccess)
        {
            PlaySound(SquidGameAudio.GetDdakjiFlip(), 1.0f);

            float flipT = 0f;
            Vector3 redStartPos = redDdakjiGround.position;
            while (flipT < 1f)
            {
                flipT += Time.deltaTime * 4.0f;
                float height = Mathf.Sin(flipT * Mathf.PI) * 0.45f;
                redDdakjiGround.position = redStartPos + Vector3.up * height;
                redDdakjiGround.rotation = Quaternion.Euler(flipT * 180f, 25f, 0f);
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);
            ExecuteVictorySequence();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            StartSlapCinematicSequence();
        }
    }

    private void StartSlapCinematicSequence()
    {
        currentPhase = GamePhase.SlapCinematic;
        slapsEndured++;
        slapTimer = 0f;
        screenFlashAlpha = 0f;

        Vector3 pToS = (salesmanRoot.position - playerRoot.position); pToS.y = 0f;
        playerRoot.rotation = Quaternion.LookRotation(pToS);
        salesmanRoot.rotation = Quaternion.LookRotation(-pToS);

        SetDialogue("THE RECRUITER (SALESMAN)", "\"Tsk, you missed. You know our deal: pay ₩100,000 Won... or pay with your body!\"");
    }

    private void UpdateSlapAnimation()
    {
        if (salesmanRightUpperArm == null || salesmanRightForearm == null || playerHead == null || playerTorso == null) return;

        if (slapTimer < 0.85f)
        {
            // WIND-UP: Salesman coils torso and draws right arm high behind him
            float windUp = Mathf.SmoothStep(0f, 1f, slapTimer / 0.85f);
            salesmanTorso.localRotation = Quaternion.Euler(-5f * windUp, -35f * windUp, 0f);
            salesmanRightUpperArm.localRotation = Quaternion.Euler(-115f * windUp, 55f * windUp, 35f * windUp);
            salesmanRightForearm.localRotation = Quaternion.Euler(85f * windUp, 0f, 0f);
            headSlapAngle = 0f;
        }
        else if (slapTimer < 1.02f)
        {
            // EXPLOSIVE STRIKE: Snaps forward with whip force!
            float strikeT = (slapTimer - 0.85f) / 0.17f;
            salesmanTorso.localRotation = Quaternion.Euler(8f * strikeT, 25f * strikeT, 0f);
            salesmanRightUpperArm.localRotation = Quaternion.Euler(35f * strikeT, -65f * strikeT, 15f);
            salesmanRightForearm.localRotation = Quaternion.Euler(15f, 0f, 0f);

            if (slapTimer >= 0.94f && screenFlashAlpha <= 0.05f)
            {
                PlaySound(SquidGameAudio.GetSlap(), 1.0f);
                PlaySound(SquidGameAudio.GetGroan(), 0.95f);
                screenFlashAlpha = 0.95f;
                cameraShakeIntensity = 0.85f;
                headSlapAngle = -68f;
            }
        }
        else
        {
            // RECOVERY: Smooth return, player clutching cheek / stunned
            salesmanTorso.localRotation = Quaternion.Slerp(salesmanTorso.localRotation, Quaternion.identity, Time.deltaTime * 3.5f);
            salesmanRightUpperArm.localRotation = Quaternion.Slerp(salesmanRightUpperArm.localRotation, Quaternion.identity, Time.deltaTime * 4f);
            salesmanRightForearm.localRotation = Quaternion.Slerp(salesmanRightForearm.localRotation, Quaternion.identity, Time.deltaTime * 4f);
            headSlapAngle = Mathf.Lerp(headSlapAngle, 0f, Time.deltaTime * 3.5f);

            if (slapTimer >= 2.0f)
            {
                SetDialogue("THE RECRUITER (SALESMAN)",
                    $"*SMACK!* \"That settles the ₩100,000 won debt. Total slaps taken: {slapsEndured}.\nShall we try another round? You can still win!\" [Press E to Play Again]");
            }
        }

        playerHead.localRotation = Quaternion.Euler(6f, headSlapAngle, -headSlapAngle * 0.35f);
        playerTorso.localRotation = Quaternion.Euler(-headSlapAngle * 0.15f, headSlapAngle * 0.2f, 0f);
    }

    private void ExecuteVictorySequence()
    {
        currentPhase = GamePhase.VictoryWon;
        wonCashKRW += 100000;
        hasWonCard = true;

        PlaySound(SquidGameAudio.GetCash(), 1.0f);
        PlaySound(SquidGameAudio.GetThemeMotif(), 0.95f);

        SetDialogue("VICTORY! ₩100,000 CASH WON",
            "\"Splendid throw, sir! Here is your ₩100,000 Won.\"\n\"You have a natural gift for games. There is a place where you can win billions... take this card.\" [Press E to Inspect Card]");
    }

    private void StartShuttleBoarding()
    {
        currentPhase = GamePhase.ShuttleBoarding;
        PlaySound(SquidGameAudio.GetThemeMotif(), 1.0f);

        StartCoroutine(AnimateShuttleBoardingSequence());
    }

    private IEnumerator AnimateShuttleBoardingSequence()
    {
        SetDialogue("SQUID GAME SHUTTLE", "Entering the VIP Island Shuttle Van with Pink Guard...");
        yield return new WaitForSeconds(1.8f);

        currentPhase = GamePhase.IslandTransitionComplete;
        SetDialogue("SQUID GAME COMMENCED", "You fall unconscious inside the shuttle...\nWelcome to the Island. Red Light, Green Light awaits.");
    }

    private void UpdateCameraOrbit()
    {
        if (gameCamera == null || playerRoot == null) return;

        Vector3 targetPoint;
        if (currentPhase == GamePhase.SalesmanDialogue || currentPhase == GamePhase.BriefcaseOpening || currentPhase == GamePhase.SlapCinematic)
        {
            // Cinematic 2-Shot dialogue framing
            Vector3 midPoint = (playerRoot.position + salesmanRoot.position) * 0.5f;
            midPoint.y += 1.45f;
            targetPoint = midPoint;
        }
        else if (currentPhase == GamePhase.DdakjiAiming || currentPhase == GamePhase.DdakjiThrowing)
        {
            // Low angle over-the-shoulder framing for Ddakji aiming
            Vector3 overShoulder = playerRoot.position + playerRoot.right * 0.65f + Vector3.up * 1.35f;
            targetPoint = overShoulder;
        }
        else
        {
            // Standard Third Person Gameplay Camera
            targetPoint = playerRoot.position + Vector3.up * 1.55f;
        }

        currentCamLookTarget = Vector3.Lerp(currentCamLookTarget, targetPoint, Time.deltaTime * 8f);

        Quaternion rot = Quaternion.Euler(camPitch, camYaw, 0f);
        Vector3 camDesiredPos = currentCamLookTarget - (rot * Vector3.forward * camDistance);

        // Screen Shake offset on impact
        Vector3 shakeOffset = Vector3.zero;
        if (cameraShakeIntensity > 0.01f)
        {
            shakeOffset = Random.insideUnitSphere * cameraShakeIntensity;
            cameraShakeIntensity = Mathf.Max(0f, cameraShakeIntensity - Time.deltaTime * 2.8f);
        }

        gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, camDesiredPos, Time.deltaTime * 10f);
        gameCamera.transform.LookAt(currentCamLookTarget + shakeOffset);
    }

    private void UpdateVisualEffects()
    {
        if (screenFlashAlpha > 0f)
        {
            screenFlashAlpha = Mathf.Max(0f, screenFlashAlpha - Time.deltaTime * 2.6f);
        }

        if (dialogueTimer > 0f)
        {
            dialogueTimer -= Time.deltaTime;
            if (dialogueTimer <= 0f)
            {
                currentDialogueText = "";
            }
        }
    }

    private void SetDialogue(string speaker, string text, float duration = 0f)
    {
        dialogueSpeaker = speaker;
        currentDialogueText = text;
        dialogueTimer = duration;
    }

    // =========================================================================
    // ON GUI - AAA GTA / SQUID GAME HUD
    // =========================================================================
    private void OnGUI()
    {
        var oldColor = GUI.color;

        // 1. Red Impact Screen Flash (On Slap)
        if (screenFlashAlpha > 0.01f)
        {
            GUI.color = new Color(0.85f, 0.05f, 0.05f, screenFlashAlpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        // 2. Squid Game Top Bar HUD
        DrawTopStatusBar();

        // 3. Mini Controls Helper (Bottom-Left)
        DrawControlsHelper();

        // 4. Dialogue Box
        if (!string.IsNullOrEmpty(currentDialogueText))
        {
            DrawDialogueBox();
        }

        // 5. Ddakji Power Meter (During Ddakji minigame)
        if (currentPhase == GamePhase.DdakjiAiming)
        {
            DrawDdakjiPowerMeter();
        }

        // 6. 3D Card Inspection Overlay (During InspectCard)
        if (currentPhase == GamePhase.InspectCard)
        {
            DrawCardInspectOverlay();
        }

        // 7. Fullscreen Fade Out (On Island Shuttle Departure)
        if (currentPhase == GamePhase.IslandTransitionComplete)
        {
            DrawIslandTransitionOverlay();
        }

        GUI.color = oldColor;
    }

    private void DrawTopStatusBar()
    {
        float barWidth = 520f;
        float barHeight = 92f;
        Rect barRect = new Rect(24f, 24f, barWidth, barHeight);

        GUI.color = new Color(0.06f, 0.07f, 0.09f, 0.94f);
        GUI.Box(barRect, GUIContent.none);

        GUI.color = new Color(0.98f, 0.12f, 0.48f, 1f);
        GUI.DrawTexture(new Rect(barRect.x, barRect.y, barRect.width, 3f), Texture2D.whiteTexture);

        GUI.color = Color.white;
        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        titleStyle.normal.textColor = new Color(0.98f, 0.85f, 0.25f);

        string locText = (playerRoot != null && playerRoot.position.z >= 38f)
            ? "SEOUL DISTRICT - RAINY STREET (쌍문동 대로)"
            : "SEOUL METRO - SSANGMUN PLATFORM (쌍문역 플랫폼)";
        GUI.Label(new Rect(barRect.x + 18f, barRect.y + 10f, 480f, 22f), locText, titleStyle);

        var cashStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 17,
            fontStyle = FontStyle.Bold
        };
        cashStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(barRect.x + 18f, barRect.y + 34f, 240f, 24f), $"CASH: ₩{wonCashKRW:N0} KRW", cashStyle);

        var slapStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };
        slapStyle.normal.textColor = (slapsEndured > 0) ? new Color(1.0f, 0.35f, 0.35f) : new Color(0.7f, 0.7f, 0.7f);
        GUI.Label(new Rect(barRect.x + 18f, barRect.y + 60f, 240f, 20f), $"SLAPS ENDURED: {slapsEndured}", slapStyle);

        var cardStatusStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleRight
        };
        cardStatusStyle.normal.textColor = hasWonCard ? new Color(0.2f, 0.95f, 0.5f) : new Color(0.6f, 0.6f, 0.6f);
        string cardText = hasWonCard ? "CARD: ACQUIRED ○ △ □" : "CARD: NONE";
        GUI.Label(new Rect(barRect.x + barWidth - 190f, barRect.y + 35f, 172f, 22f), cardText, cardStatusStyle);

        // Distance to Island Van / Stairs
        if (islandVanRoot != null && playerRoot != null)
        {
            float distToVan = Vector3.Distance(playerRoot.position, islandVanRoot.position);
            var distStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold
            };
            distStyle.normal.textColor = new Color(0.98f, 0.12f, 0.48f);
            GUI.Label(new Rect(barRect.x + barWidth - 210f, barRect.y + 60f, 192f, 20f), $"VAN DISTANCE: {distToVan:F1}m", distStyle);
        }
    }

    private void DrawControlsHelper()
    {
        float w = 460f;
        float h = 64f;
        Rect r = new Rect(24f, Screen.height - h - 24f, w, h);

        GUI.color = new Color(0.06f, 0.07f, 0.09f, 0.88f);
        GUI.Box(r, GUIContent.none);

        GUI.color = Color.white;
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft
        };
        style.normal.textColor = new Color(0.85f, 0.88f, 0.92f);

        string hint = "[WASD] Walk | [Shift] Sprint | [Right-Click Drag] Orbit Cam\n" +
                      "[E] Interact / Talk / Play / Ascend Stairs / Board Van";
        GUI.Label(new Rect(r.x + 14f, r.y + 8f, r.width - 28f, r.height - 16f), hint, style);
    }

    private void DrawDialogueBox()
    {
        float boxWidth = Mathf.Min(820f, Screen.width - 48f);
        float boxHeight = 110f;
        Rect boxRect = new Rect((Screen.width - boxWidth) * 0.5f, Screen.height - boxHeight - 96f, boxWidth, boxHeight);

        GUI.color = new Color(0.05f, 0.06f, 0.08f, 0.95f);
        GUI.Box(boxRect, GUIContent.none);

        GUI.color = new Color(0.98f, 0.12f, 0.48f, 1f);
        GUI.DrawTexture(new Rect(boxRect.x, boxRect.y, 4f, boxRect.height), Texture2D.whiteTexture);

        GUI.color = Color.white;
        var spkStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };
        spkStyle.normal.textColor = new Color(0.98f, 0.85f, 0.25f);
        GUI.Label(new Rect(boxRect.x + 18f, boxRect.y + 10f, boxRect.width - 36f, 22f), dialogueSpeaker, spkStyle);

        var txtStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            wordWrap = true
        };
        txtStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(boxRect.x + 18f, boxRect.y + 34f, boxRect.width - 36f, boxRect.height - 42f), currentDialogueText, txtStyle);
    }

    private void DrawDdakjiPowerMeter()
    {
        float barWidth = 360f;
        float barHeight = 28f;
        float x = (Screen.width - barWidth) * 0.5f;
        float y = Screen.height - 245f;

        // Background Track
        GUI.color = new Color(0.1f, 0.1f, 0.12f, 0.95f);
        GUI.DrawTexture(new Rect(x - 4f, y - 4f, barWidth + 8f, barHeight + 8f), Texture2D.whiteTexture);

        // Sweet spot region (Green)
        float sweetX = x + barWidth * SweetSpotStart;
        float sweetW = barWidth * (SweetSpotEnd - SweetSpotStart);
        GUI.color = new Color(0.15f, 0.85f, 0.25f, 0.95f);
        GUI.DrawTexture(new Rect(sweetX, y, sweetW, barHeight), Texture2D.whiteTexture);

        // Moving Indicator (Red pointer)
        float indicatorX = x + barWidth * ddakjiPower;
        GUI.color = new Color(0.98f, 0.15f, 0.15f, 1f);
        GUI.DrawTexture(new Rect(indicatorX - 3f, y - 6f, 6f, barHeight + 12f), Texture2D.whiteTexture);

        GUI.color = Color.white;
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 13
        };
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y - 28f, barWidth, 22f), "POWER GAUGE - HIT THE GREEN SWEET SPOT!", style);
    }

    private void DrawCardInspectOverlay()
    {
        float w = 460f;
        float h = 60f;
        Rect r = new Rect((Screen.width - w) * 0.5f, 30f, w, h);

        GUI.color = new Color(0.06f, 0.07f, 0.09f, 0.92f);
        GUI.Box(r, GUIContent.none);

        GUI.color = Color.white;
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = new Color(0.98f, 0.85f, 0.25f);
        GUI.Label(r, "SQUID GAME INVITATION CARD (3D INSPECT)\n[Click & Drag] Rotate Card | [E] Pocket Card", style);
    }

    private void DrawIslandTransitionOverlay()
    {
        GUI.color = new Color(0.04f, 0.04f, 0.05f, 0.98f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);

        GUI.color = Color.white;
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 28
        };
        style.normal.textColor = new Color(0.98f, 0.12f, 0.48f);
        GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width, 48f), "○  △  □", style);

        var subStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18
        };
        subStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(0f, Screen.height * 0.48f, Screen.width, 36f), "YOU HAVE ENTERED THE SQUID GAME", subStyle);
        GUI.Label(new Rect(0f, Screen.height * 0.54f, Screen.width, 30f), "Press [SPACE / E / R] to Replay City Chapter", subStyle);
    }

    // =========================================================================
    // 1. PROCEDURAL BUILDER: SEOUL SUBWAY STATION PLATFORM (SSANGMUN LINE 4)
    // =========================================================================
    private void BuildSeoulSubwayStationPlatform()
    {
        var stationRoot = new GameObject("Seoul Metro Ssangmun Platform").transform;
        stationRoot.SetParent(transform, false);

        // 1. Platform Base Slab (walkable platform at Y = 0)
        CreatePrimitive(PrimitiveType.Cube, "Subway Platform Slab", stationRoot,
            new Vector3(14f, 0.6f, 52f), new Vector3(2.0f, -0.3f, 4.0f), Quaternion.identity, subwayTileMat, true);

        // 2. Yellow Tactile Warning Edge Strip (with raised safety bumps)
        CreatePrimitive(PrimitiveType.Cube, "Platform Edge Tactile Strip", stationRoot,
            new Vector3(0.6f, 0.02f, 52f), new Vector3(-4.6f, 0.015f, 4.0f), Quaternion.identity, tactileYellowMat);

        // 3. Recessed Train Track Trench (depth Y = -1.15m)
        CreatePrimitive(PrimitiveType.Cube, "Track Ballast Trench", stationRoot,
            new Vector3(8f, 0.6f, 75f), new Vector3(-8.8f, -1.45f, 4.0f), Quaternion.identity, trainTracksMat, true);

        // 4. Dual Steel I-Beam Rails & Concrete Sleepers
        CreateMeshObject("Subway Rails and Ties", stationRoot, SquidGameCharacterMeshes.GetTrainTracks(),
            trainTracksMat, new Vector3(-8.8f, -1.12f, 4.0f), Quaternion.identity, Vector3.one);

        // 5. Far Tunnel Concrete Wall
        CreatePrimitive(PrimitiveType.Cube, "Tunnel Wall", stationRoot,
            new Vector3(0.6f, 7.5f, 75f), new Vector3(-13.0f, 2.2f, 4.0f), Quaternion.identity, carBlackMat, true);

        // 6. Platform Back Wall with Advertising Lightboxes & Metro Signs
        CreatePrimitive(PrimitiveType.Cube, "Platform Back Wall", stationRoot,
            new Vector3(0.6f, 6.2f, 52f), new Vector3(9.2f, 2.8f, 4.0f), Quaternion.identity, subwayTileMat, true);

        // Station Advertising Lightboxes
        foreach (float bz in new[] { -12f, 4f, 16f })
        {
            CreatePrimitive(PrimitiveType.Cube, "Station Ad Lightbox", stationRoot,
                new Vector3(0.2f, 2.4f, 4.8f), new Vector3(8.85f, 2.4f, bz), Quaternion.identity, billboardSquidMat);
        }

        // 7. Giant Platform Support Pillars (with Line 4 route maps)
        foreach (float pz in new[] { -14f, -2f, 10f })
        {
            CreateMeshObject($"Platform Pillar {pz}", stationRoot, SquidGameCharacterMeshes.GetPlatformPillar(),
                subwayTileMat, new Vector3(1.8f, 0.0f, pz), Quaternion.identity, Vector3.one, true);

            CreatePrimitive(PrimitiveType.Cube, "Pillar Metro Sign", stationRoot,
                new Vector3(1.28f, 0.65f, 0.12f), new Vector3(1.8f, 2.4f, pz + 0.58f), Quaternion.identity, neonSubwayMat);
        }

        // 8. Stainless Steel Platform Benches (where Gi-hun waits!)
        foreach (float bz in new[] { -8f, 4f, 14f })
        {
            CreateMeshObject($"Subway Bench {bz}", stationRoot, SquidGameCharacterMeshes.GetSubwayBench(),
                metalBenchMat, new Vector3(4.5f, 0.0f, bz), Quaternion.Euler(0f, -90f, 0f), Vector3.one, true);
        }

        // 9. Overhead Metro Signage
        CreatePrimitive(PrimitiveType.Cube, "Overhead Metro Sign", stationRoot,
            new Vector3(0.18f, 0.85f, 4.2f), new Vector3(-2.8f, 4.2f, 6.0f), Quaternion.identity, neonSubwayMat);

        // Fluorescent Station Lighting
        for (float lz = -16f; lz <= 22f; lz += 10f)
        {
            var lightObj = new GameObject($"Station Light {lz}");
            lightObj.transform.SetParent(stationRoot, false);
            lightObj.transform.localPosition = new Vector3(2.0f, 4.2f, lz);
            var pl = lightObj.AddComponent<Light>();
            pl.type = LightType.Point;
            pl.color = new Color(0.92f, 0.95f, 1.0f);
            pl.range = 14f;
            pl.intensity = 2.4f;

            CreatePrimitive(PrimitiveType.Cube, "Tube Fixture", lightObj.transform,
                new Vector3(0.35f, 0.08f, 2.4f), Vector3.zero, Quaternion.identity, warmGlowMat);
        }

        // 10. Staircase Ascending to Street Level (from Z = 24 to Z = 42, Y = 0 to Y = 4.5)
        BuildSubwayExitStairs(stationRoot);
    }

    private void BuildSubwayExitStairs(Transform parent)
    {
        var stairsRoot = new GameObject("Subway Exit Stairs to Street").transform;
        stairsRoot.SetParent(parent, false);
        stairsRoot.position = new Vector3(2.0f, 0f, 24f);

        int totalSteps = 24;
        float rampLengthZ = 18f; // Z from 24m to 42m
        float stepDepth = rampLengthZ / totalSteps; // 0.75m
        float stepHeight = StreetFloorY / totalSteps; // ~0.187m

        for (int i = 0; i < totalSteps; i++)
        {
            CreatePrimitive(PrimitiveType.Cube, $"Stair {i}", stairsRoot,
                new Vector3(6.5f, stepHeight, stepDepth),
                new Vector3(0f, stepHeight * 0.5f + i * stepHeight, i * stepDepth + stepDepth * 0.5f),
                Quaternion.identity, subwayTileMat, true);
        }

        // Smooth ramp collider covering the stairs perfectly so walking up is 100% fluid!
        var ramp = new GameObject("Stair Smooth Ramp Collider");
        ramp.transform.SetParent(stairsRoot, false);
        ramp.transform.localPosition = new Vector3(0f, StreetFloorY * 0.5f, rampLengthZ * 0.5f);
        float angle = Mathf.Atan2(StreetFloorY, rampLengthZ) * Mathf.Rad2Deg; // ~14.0 degrees
        ramp.transform.localRotation = Quaternion.Euler(-angle, 0f, 0f); // Negative angle slopes UPWARDS along +Z
        var bc = ramp.AddComponent<BoxCollider>();
        float hyp = Mathf.Sqrt(StreetFloorY * StreetFloorY + rampLengthZ * rampLengthZ);
        bc.size = new Vector3(6.6f, 0.15f, hyp);

        // Side Handrails
        foreach (float sx in new[] { -3.2f, 3.2f })
        {
            var rail = new GameObject($"Handrail {sx}");
            rail.transform.SetParent(stairsRoot, false);
            rail.transform.localPosition = new Vector3(sx, StreetFloorY * 0.5f + 0.85f, rampLengthZ * 0.5f);
            rail.transform.localRotation = Quaternion.Euler(-angle, 0f, 0f);
            var rCol = rail.AddComponent<BoxCollider>();
            rCol.size = new Vector3(0.08f, 0.08f, hyp);
            CreatePrimitive(PrimitiveType.Cube, "Rail Visual", rail.transform, new Vector3(0.06f, 0.06f, hyp), Vector3.zero, Quaternion.identity, metalBenchMat);
        }

        // Entrance Canopy at street level exit (Z = 42m)
        var canopy = new GameObject("Seoul Metro Station Entrance Canopy").transform;
        canopy.SetParent(stairsRoot, false);
        canopy.localPosition = new Vector3(0f, StreetFloorY, rampLengthZ);
        CreateMeshObject("Canopy Glass Truss", canopy, SquidGameCharacterMeshes.GetEntranceCanopy(), carGlassMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Signboard: Ssangmun Station Exit 2 (쌍문역 2번 출구)
        CreatePrimitive(PrimitiveType.Cube, "Metro Exit 2 Sign", canopy,
            new Vector3(6.2f, 0.9f, 0.25f), new Vector3(0f, 3.4f, 0f), Quaternion.identity, neonSubwayMat);
    }

    // =========================================================================
    // 2. PROCEDURAL BUILDER: STREET LEVEL DISTRICT ABOVE (Z >= 42m ONLY!)
    // =========================================================================
    private void BuildStreetLevelAndCity()
    {
        var streetRoot = new GameObject("Street Level District").transform;
        streetRoot.SetParent(transform, false);
        streetRoot.position = new Vector3(0f, StreetFloorY, 42f);

        // Wet Asphalt Avenue (Street level starts strictly at Z = 42m; no ceiling over stairs!)
        CreatePrimitive(PrimitiveType.Cube, "Street Avenue Slab", streetRoot,
            new Vector3(22f, 0.4f, 100f), new Vector3(0f, -0.2f, 50f), Quaternion.identity, wetAsphaltMat, true);

        // Sidewalks
        CreatePrimitive(PrimitiveType.Cube, "Left Sidewalk", streetRoot,
            new Vector3(8f, 0.4f, 100f), new Vector3(-15f, -0.1f, 50f), Quaternion.identity, sidewalkMat, true);
        CreatePrimitive(PrimitiveType.Cube, "Right Sidewalk", streetRoot,
            new Vector3(8f, 0.4f, 100f), new Vector3(15f, -0.1f, 50f), Quaternion.identity, sidewalkMat, true);

        // Street Foundation Slab
        CreatePrimitive(PrimitiveType.Cube, "Street Foundation", streetRoot,
            new Vector3(140f, 1f, 100f), new Vector3(0f, -0.7f, 50f), Quaternion.identity, wetAsphaltMat, true);

        // Crosswalks & Lane Lines
        for (float z = 8f; z < 90f; z += 8f)
        {
            CreatePrimitive(PrimitiveType.Cube, "Lane Dash", streetRoot,
                new Vector3(0.18f, 0.015f, 3.8f), new Vector3(0f, 0.012f, z), Quaternion.identity, sidewalkMat);
        }

        // Multi-story Commercial Buildings lining street
        for (int side = -1; side <= 1; side += 2)
        {
            for (int i = 0; i < 3; i++)
            {
                float z = 12f + i * 26f;
                float bX = side * 24.5f;
                Material sign = (i == 0) ? neonConvenienceMat : (i == 1) ? neonNoraebangMat : billboardSquidMat;

                CreatePrimitive(PrimitiveType.Cube, $"Street Building {side}_{i}", streetRoot,
                    new Vector3(14f, 18f, 22f), new Vector3(bX, 9f, z), Quaternion.identity, sidewalkMat, true);
                CreatePrimitive(PrimitiveType.Cube, "Shop Sign", streetRoot,
                    new Vector3(0.3f, 1.4f, 9.5f), new Vector3(bX - side * 7.2f, 4.8f, z), Quaternion.identity, sign);
            }
        }

        // Parked Seoul Taxi
        CreateDetailedCar(streetRoot, new Vector3(8.5f, 0f, 12f), 0f, seoulTaxiMat, true);

        // Streetlights casting warm light
        for (float z = 8f; z < 80f; z += 22f)
        {
            var lightObj = new GameObject("Street Light");
            lightObj.transform.SetParent(streetRoot, false);
            lightObj.transform.localPosition = new Vector3(-11f, 7.5f, z);
            var spot = lightObj.AddComponent<Light>();
            spot.type = LightType.Spot;
            spot.spotAngle = 75f;
            spot.range = 18f;
            spot.intensity = 3.5f;
            spot.color = new Color(1.0f, 0.88f, 0.72f);
            spot.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Light Pole", streetRoot,
                new Vector3(0.18f, 3.8f, 0.18f), new Vector3(-11f, 3.8f, z), Quaternion.identity, carBlackMat);
        }
    }

    private void CreateDetailedCar(Transform parent, Vector3 pos, float rotY, Material paintMat, bool isTaxi)
    {
        var car = new GameObject(isTaxi ? "Seoul Orange Taxi" : "Sedan").transform;
        car.SetParent(parent, false);
        car.localPosition = pos;
        car.localRotation = Quaternion.Euler(0f, rotY, 0f);

        CreatePrimitive(PrimitiveType.Cube, "Body", car, new Vector3(2.1f, 0.75f, 4.6f), new Vector3(0f, 0.65f, 0f), Quaternion.identity, paintMat);
        CreatePrimitive(PrimitiveType.Cube, "Cabin", car, new Vector3(1.75f, 0.7f, 2.4f), new Vector3(0f, 1.35f, -0.2f), Quaternion.identity, carGlassMat);

        foreach (float lx in new[] { -0.7f, 0.7f })
        {
            CreatePrimitive(PrimitiveType.Cube, "Headlight", car, new Vector3(0.38f, 0.2f, 0.08f), new Vector3(lx, 0.75f, 2.34f), Quaternion.identity, warmGlowMat);
        }

        if (isTaxi)
        {
            CreatePrimitive(PrimitiveType.Cube, "Taxi Roof Sign", car, new Vector3(0.55f, 0.22f, 0.28f), new Vector3(0f, 1.82f, -0.1f), Quaternion.identity, tactileYellowMat);
        }
    }

    // =========================================================================
    // 3. PROCEDURAL BUILDER: ISLAND VIP SHUTTLE VAN & ARTICULATED PINK GUARD
    // =========================================================================
    private void BuildIslandShuttleVan()
    {
        islandVanRoot = new GameObject("Squid Game Island VIP Shuttle Van").transform;
        islandVanRoot.SetParent(transform, false);
        islandVanRoot.position = new Vector3(2.0f, StreetFloorY, ShuttleZ);

        CreatePrimitive(PrimitiveType.Cube, "Van Body", islandVanRoot, new Vector3(2.5f, 2.2f, 6.8f), new Vector3(0f, 1.45f, 0f), Quaternion.identity, carBlackMat, true);
        CreatePrimitive(PrimitiveType.Cube, "Van Tinted Windows", islandVanRoot, new Vector3(2.54f, 0.85f, 4.8f), new Vector3(0f, 1.75f, -0.4f), Quaternion.identity, carGlassMat);
        CreatePrimitive(PrimitiveType.Cube, "Windshield", islandVanRoot, new Vector3(2.2f, 0.95f, 0.1f), new Vector3(0f, 1.75f, 2.6f), Quaternion.Euler(-25f, 0f, 0f), carGlassMat);

        foreach (float lx in new[] { -0.85f, 0.85f })
        {
            CreatePrimitive(PrimitiveType.Cube, "Van Headlight", islandVanRoot, new Vector3(0.42f, 0.22f, 0.1f), new Vector3(lx, 0.9f, 3.42f), Quaternion.identity, warmGlowMat);

            var spot = new GameObject("Headlight Beam").AddComponent<Light>();
            spot.transform.SetParent(islandVanRoot, false);
            spot.transform.localPosition = new Vector3(lx, 0.9f, 3.5f);
            spot.transform.rotation = Quaternion.Euler(15f, lx * 5f, 0f);
            spot.type = LightType.Spot;
            spot.spotAngle = 65f;
            spot.range = 22f;
            spot.intensity = 4.0f;
            spot.color = new Color(0.95f, 0.98f, 1.0f);
        }

        // Glowing Pink Squid Game Ground Beacon at the Van Door
        var beacon = CreatePrimitive(PrimitiveType.Cylinder, "Van Boarding Beacon", islandVanRoot,
            new Vector3(2.4f, 0.02f, 2.4f), new Vector3(-2.0f, 0.02f, -0.4f), Quaternion.identity, pinkGuardMat);
        _ = beacon;

        // Masked Pink Guard standing beside the sliding door
        pinkGuardRoot = new GameObject("Masked Pink Guard (Square)").transform;
        pinkGuardRoot.SetParent(islandVanRoot, false);
        pinkGuardRoot.localPosition = new Vector3(-2.0f, 0f, -0.4f);
        pinkGuardRoot.localRotation = Quaternion.Euler(0f, 85f, 0f);

        pinkGuardHips = new GameObject("Guard Hips").transform;
        pinkGuardHips.SetParent(pinkGuardRoot, false);
        pinkGuardHips.localPosition = new Vector3(0f, 0.95f, 0f);

        pinkGuardTorso = new GameObject("Guard Torso").transform;
        pinkGuardTorso.SetParent(pinkGuardHips, false);
        pinkGuardTorso.localPosition = new Vector3(0f, 0.28f, 0f);
        CreateMeshObject("Guard Jumpsuit Sculpt", pinkGuardTorso, SquidGameCharacterMeshes.GetPinkGuardTorso(), pinkGuardMat, Vector3.zero, Quaternion.identity, Vector3.one);

        pinkGuardHead = new GameObject("Guard Mask Head").transform;
        pinkGuardHead.SetParent(pinkGuardTorso, false);
        pinkGuardHead.localPosition = new Vector3(0f, 0.48f, 0f);
        CreateMeshObject("Guard Fencing Mask", pinkGuardHead, SquidGameCharacterMeshes.GetPinkGuardMask(), guardMaskMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Arms holding tactical SMG across chest
        pinkGuardRightArm = new GameObject("Guard Right Arm").transform;
        pinkGuardRightArm.SetParent(pinkGuardTorso, false);
        pinkGuardRightArm.localPosition = new Vector3(0.32f, 0.18f, 0f);
        pinkGuardRightArm.localRotation = Quaternion.Euler(38f, -25f, 0f);
        CreateMeshObject("Arm Segment", pinkGuardRightArm, SquidGameCharacterMeshes.GetSuitUpperArm(), pinkGuardMat, Vector3.zero, Quaternion.identity, Vector3.one);

        pinkGuardLeftArm = new GameObject("Guard Left Arm").transform;
        pinkGuardLeftArm.SetParent(pinkGuardTorso, false);
        pinkGuardLeftArm.localPosition = new Vector3(-0.32f, 0.18f, 0f);
        pinkGuardLeftArm.localRotation = Quaternion.Euler(42f, 30f, 0f);
        CreateMeshObject("Arm Segment", pinkGuardLeftArm, SquidGameCharacterMeshes.GetSuitUpperArm(), pinkGuardMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Tactical SMG Gun prop
        var smg = CreatePrimitive(PrimitiveType.Cube, "Tactical SMG", pinkGuardTorso,
            new Vector3(0.08f, 0.18f, 0.55f), new Vector3(0f, 0.05f, 0.28f), Quaternion.Euler(15f, 0f, 0f), carBlackMat);
        _ = smg;

        // Legs in combat boots
        foreach (float sx in new[] { -0.16f, 0.16f })
        {
            var leg = new GameObject($"Guard Leg {sx}").transform;
            leg.SetParent(pinkGuardHips, false);
            leg.localPosition = new Vector3(sx, 0f, 0f);
            CreateMeshObject("Leg Pants", leg, SquidGameCharacterMeshes.GetSuitThigh(), pinkGuardMat, Vector3.zero, Quaternion.identity, Vector3.one);
            CreateMeshObject("Combat Boot", leg, SquidGameCharacterMeshes.GetDressShoe(), carBlackMat, new Vector3(0f, -0.48f, 0.04f), Quaternion.identity, Vector3.one);
        }
    }

    // =========================================================================
    // 4. REALISTIC PLAYER MODEL (GI-HUN) WITH ARTICULATED ANATOMY
    // =========================================================================
    private void BuildPlayerCharacter()
    {
        playerRoot = new GameObject("Player (Gi-hun)").transform;
        playerRoot.SetParent(transform, false);
        playerRoot.position = new Vector3(1.5f, SubwayFloorY, PlayerStartPlatformZ);

        playerController = playerRoot.gameObject.AddComponent<CharacterController>();
        playerController.height = 1.8f;
        playerController.radius = 0.35f;
        playerController.center = new Vector3(0f, 0.9f, 0f);
        playerController.stepOffset = 0.40f;
        playerController.slopeLimit = 55f;
        playerController.minMoveDistance = 0f;

        // 1. Pelvis / Hips Root
        playerHips = new GameObject("Player Hips").transform;
        playerHips.SetParent(playerRoot, false);
        playerHips.localPosition = new Vector3(0f, 0.95f, 0f);

        // 2. Spine / Torso
        playerTorso = new GameObject("Player Torso").transform;
        playerTorso.SetParent(playerHips, false);
        playerTorso.localPosition = new Vector3(0f, 0.28f, 0f);
        CreateMeshObject("Parka Jacket Sculpt", playerTorso, SquidGameCharacterMeshes.GetPlayerJacket(), parkaMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // 3. Realistic Anatomical Head
        playerHead = new GameObject("Player Head").transform;
        playerHead.SetParent(playerTorso, false);
        playerHead.localPosition = new Vector3(0f, 0.48f, 0f);
        CreateMeshObject("Gi-hun Head Sculpt", playerHead, SquidGameCharacterMeshes.GetPlayerHead(), skinMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // 4. Arms with 2-Bone Kinematics & 5-Finger Hands
        // Right Arm
        playerRightUpperArm = new GameObject("Player Right Upper Arm").transform;
        playerRightUpperArm.SetParent(playerTorso, false);
        playerRightUpperArm.localPosition = new Vector3(0.34f, 0.24f, 0f);
        CreateMeshObject("Upper Arm Parka", playerRightUpperArm, SquidGameCharacterMeshes.GetPlayerUpperArm(), parkaMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerRightForearm = new GameObject("Player Right Forearm").transform;
        playerRightForearm.SetParent(playerRightUpperArm, false);
        playerRightForearm.localPosition = new Vector3(0f, -0.36f, 0f);
        CreateMeshObject("Forearm Parka", playerRightForearm, SquidGameCharacterMeshes.GetPlayerForearm(), parkaMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerRightHand = new GameObject("Player Right Hand").transform;
        playerRightHand.SetParent(playerRightForearm, false);
        playerRightHand.localPosition = new Vector3(0f, -0.34f, 0f);
        CreateMeshObject("Right Hand Sculpt", playerRightHand, SquidGameCharacterMeshes.GetDetailedHand(), skinMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Left Arm
        playerLeftUpperArm = new GameObject("Player Left Upper Arm").transform;
        playerLeftUpperArm.SetParent(playerTorso, false);
        playerLeftUpperArm.localPosition = new Vector3(-0.34f, 0.24f, 0f);
        CreateMeshObject("Upper Arm Parka", playerLeftUpperArm, SquidGameCharacterMeshes.GetPlayerUpperArm(), parkaMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerLeftForearm = new GameObject("Player Left Forearm").transform;
        playerLeftForearm.SetParent(playerLeftUpperArm, false);
        playerLeftForearm.localPosition = new Vector3(0f, -0.36f, 0f);
        CreateMeshObject("Forearm Parka", playerLeftForearm, SquidGameCharacterMeshes.GetPlayerForearm(), parkaMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerLeftHand = new GameObject("Player Left Hand").transform;
        playerLeftHand.SetParent(playerLeftForearm, false);
        playerLeftHand.localPosition = new Vector3(0f, -0.34f, 0f);
        CreateMeshObject("Left Hand Sculpt", playerLeftHand, SquidGameCharacterMeshes.GetDetailedHand(), skinMat, Vector3.zero, Quaternion.Euler(0f, 180f, 0f), Vector3.one);

        // Blue Ddakji in Hand
        blueDdakjiHand = CreatePrimitive(PrimitiveType.Cube, "Blue Ddakji in Hand", playerRightHand,
            new Vector3(0.35f, 0.035f, 0.35f), new Vector3(0f, -0.08f, 0.08f), Quaternion.identity, ddakjiBlueMat).transform;
        blueDdakjiHand.gameObject.SetActive(false);

        // Squid Game Business Card
        var cardObj = new GameObject("Squid Card 3D Pivot");
        cardObj.transform.SetParent(playerRoot, false);
        cardObj.transform.localPosition = new Vector3(0f, 1.35f, 0.55f);
        squidCardHand = cardObj.transform;

        CreatePrimitive(PrimitiveType.Quad, "Card Front", squidCardHand,
            new Vector3(0.32f, 0.18f, 1f), new Vector3(0f, 0f, 0.003f), Quaternion.identity, squidCardFrontMat);
        CreatePrimitive(PrimitiveType.Quad, "Card Back", squidCardHand,
            new Vector3(0.32f, 0.18f, 1f), new Vector3(0f, 0f, -0.003f), Quaternion.Euler(0f, 180f, 0f), squidCardBackMat);
        squidCardHand.gameObject.SetActive(false);

        // 5. Legs with 2-Bone Kinematics & Street Sneakers
        // Right Leg
        playerRightThigh = new GameObject("Player Right Thigh").transform;
        playerRightThigh.SetParent(playerHips, false);
        playerRightThigh.localPosition = new Vector3(0.16f, 0f, 0f);
        CreateMeshObject("Right Thigh Jeans", playerRightThigh, SquidGameCharacterMeshes.GetPlayerThigh(), jeansMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerRightShin = new GameObject("Player Right Shin").transform;
        playerRightShin.SetParent(playerRightThigh, false);
        playerRightShin.localPosition = new Vector3(0f, -0.46f, 0f);
        CreateMeshObject("Right Shin Jeans", playerRightShin, SquidGameCharacterMeshes.GetPlayerShin(), jeansMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerRightFoot = new GameObject("Player Right Foot").transform;
        playerRightFoot.SetParent(playerRightShin, false);
        playerRightFoot.localPosition = new Vector3(0f, -0.44f, 0.04f);
        CreateMeshObject("Right Sneaker", playerRightFoot, SquidGameCharacterMeshes.GetSneaker(), sneakerMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Left Leg
        playerLeftThigh = new GameObject("Player Left Thigh").transform;
        playerLeftThigh.SetParent(playerHips, false);
        playerLeftThigh.localPosition = new Vector3(-0.16f, 0f, 0f);
        CreateMeshObject("Left Thigh Jeans", playerLeftThigh, SquidGameCharacterMeshes.GetPlayerThigh(), jeansMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerLeftShin = new GameObject("Player Left Shin").transform;
        playerLeftShin.SetParent(playerLeftThigh, false);
        playerLeftShin.localPosition = new Vector3(0f, -0.46f, 0f);
        CreateMeshObject("Left Shin Jeans", playerLeftShin, SquidGameCharacterMeshes.GetPlayerShin(), jeansMat, Vector3.zero, Quaternion.identity, Vector3.one);

        playerLeftFoot = new GameObject("Player Left Foot").transform;
        playerLeftFoot.SetParent(playerLeftShin, false);
        playerLeftFoot.localPosition = new Vector3(0f, -0.44f, 0.04f);
        CreateMeshObject("Left Sneaker", playerLeftFoot, SquidGameCharacterMeshes.GetSneaker(), sneakerMat, Vector3.zero, Quaternion.identity, Vector3.one);
    }

    // =========================================================================
    // 5. REALISTIC SALESMAN MODEL (GONG YOO) IN TAILORED SUIT
    // =========================================================================
    private void BuildSalesmanCharacter()
    {
        salesmanRoot = new GameObject("The Recruiter (Gong Yoo)").transform;
        salesmanRoot.SetParent(transform, false);
        salesmanRoot.position = new Vector3(1.5f, SubwayFloorY, SalesmanStartPlatformZ);
        salesmanRoot.rotation = Quaternion.Euler(0f, 180f, 0f);

        // 1. Pelvis / Hips
        salesmanHips = new GameObject("Salesman Hips").transform;
        salesmanHips.SetParent(salesmanRoot, false);
        salesmanHips.localPosition = new Vector3(0f, 0.96f, 0f);

        // 2. Spine / Torso
        salesmanTorso = new GameObject("Salesman Torso").transform;
        salesmanTorso.SetParent(salesmanHips, false);
        salesmanTorso.localPosition = new Vector3(0f, 0.28f, 0f);
        CreateMeshObject("Suit Jacket Sculpt", salesmanTorso, SquidGameCharacterMeshes.GetSuitJacket(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        CreatePrimitive(PrimitiveType.Cube, "Shirt Collar", salesmanTorso, new Vector3(0.18f, 0.35f, 0.05f), new Vector3(0f, 0.35f, 0.15f), Quaternion.identity, shirtMat);
        CreatePrimitive(PrimitiveType.Cube, "Silk Tie", salesmanTorso, new Vector3(0.08f, 0.45f, 0.04f), new Vector3(0f, 0.18f, 0.18f), Quaternion.identity, tieMat);

        // 3. Head & Handsome Facial Sculpt
        salesmanHead = new GameObject("Salesman Head").transform;
        salesmanHead.SetParent(salesmanTorso, false);
        salesmanHead.localPosition = new Vector3(0f, 0.48f, 0f);
        CreateMeshObject("Gong Yoo Head Sculpt", salesmanHead, SquidGameCharacterMeshes.GetSalesmanHead(), skinMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // 4. Arms with 2-Bone Kinematics
        // Right Arm (Slapping Arm)
        salesmanRightUpperArm = new GameObject("Salesman Right Upper Arm").transform;
        salesmanRightUpperArm.SetParent(salesmanTorso, false);
        salesmanRightUpperArm.localPosition = new Vector3(0.34f, 0.24f, 0f);
        CreateMeshObject("Suit Upper Arm", salesmanRightUpperArm, SquidGameCharacterMeshes.GetSuitUpperArm(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanRightForearm = new GameObject("Salesman Right Forearm").transform;
        salesmanRightForearm.SetParent(salesmanRightUpperArm, false);
        salesmanRightForearm.localPosition = new Vector3(0f, -0.36f, 0f);
        CreateMeshObject("Suit Forearm", salesmanRightForearm, SquidGameCharacterMeshes.GetSuitForearm(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanRightHand = new GameObject("Salesman Right Hand").transform;
        salesmanRightHand.SetParent(salesmanRightForearm, false);
        salesmanRightHand.localPosition = new Vector3(0f, -0.34f, 0f);
        CreateMeshObject("Slapping Hand", salesmanRightHand, SquidGameCharacterMeshes.GetSlapHand(), skinMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Left Arm (Holding Briefcase)
        salesmanLeftUpperArm = new GameObject("Salesman Left Upper Arm").transform;
        salesmanLeftUpperArm.SetParent(salesmanTorso, false);
        salesmanLeftUpperArm.localPosition = new Vector3(-0.34f, 0.24f, 0f);
        CreateMeshObject("Suit Upper Arm", salesmanLeftUpperArm, SquidGameCharacterMeshes.GetSuitUpperArm(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanLeftForearm = new GameObject("Salesman Left Forearm").transform;
        salesmanLeftForearm.SetParent(salesmanLeftUpperArm, false);
        salesmanLeftForearm.localPosition = new Vector3(0f, -0.36f, 0f);
        CreateMeshObject("Suit Forearm", salesmanLeftForearm, SquidGameCharacterMeshes.GetSuitForearm(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanLeftHand = new GameObject("Salesman Left Hand").transform;
        salesmanLeftHand.SetParent(salesmanLeftForearm, false);
        salesmanLeftHand.localPosition = new Vector3(0f, -0.34f, 0f);
        CreateMeshObject("Left Hand", salesmanLeftHand, SquidGameCharacterMeshes.GetDetailedHand(), skinMat, Vector3.zero, Quaternion.Euler(0f, 180f, 0f), Vector3.one);

        // 5. Legs with Tailored Suit Trousers & Oxford Dress Shoes
        // Right Leg
        salesmanRightThigh = new GameObject("Salesman Right Thigh").transform;
        salesmanRightThigh.SetParent(salesmanHips, false);
        salesmanRightThigh.localPosition = new Vector3(0.16f, 0f, 0f);
        CreateMeshObject("Suit Thigh", salesmanRightThigh, SquidGameCharacterMeshes.GetSuitThigh(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanRightShin = new GameObject("Salesman Right Shin").transform;
        salesmanRightShin.SetParent(salesmanRightThigh, false);
        salesmanRightShin.localPosition = new Vector3(0f, -0.46f, 0f);
        CreateMeshObject("Suit Shin", salesmanRightShin, SquidGameCharacterMeshes.GetSuitShin(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanRightFoot = new GameObject("Salesman Right Foot").transform;
        salesmanRightFoot.SetParent(salesmanRightShin, false);
        salesmanRightFoot.localPosition = new Vector3(0f, -0.44f, 0.04f);
        CreateMeshObject("Oxford Shoe", salesmanRightFoot, SquidGameCharacterMeshes.GetDressShoe(), carBlackMat, Vector3.zero, Quaternion.identity, Vector3.one);

        // Left Leg
        salesmanLeftThigh = new GameObject("Salesman Left Thigh").transform;
        salesmanLeftThigh.SetParent(salesmanHips, false);
        salesmanLeftThigh.localPosition = new Vector3(-0.16f, 0f, 0f);
        CreateMeshObject("Suit Thigh", salesmanLeftThigh, SquidGameCharacterMeshes.GetSuitThigh(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanLeftShin = new GameObject("Salesman Left Shin").transform;
        salesmanLeftShin.SetParent(salesmanLeftThigh, false);
        salesmanLeftShin.localPosition = new Vector3(0f, -0.46f, 0f);
        CreateMeshObject("Suit Shin", salesmanLeftShin, SquidGameCharacterMeshes.GetSuitShin(), suitMat, Vector3.zero, Quaternion.identity, Vector3.one);

        salesmanLeftFoot = new GameObject("Salesman Left Foot").transform;
        salesmanLeftFoot.SetParent(salesmanLeftShin, false);
        salesmanLeftFoot.localPosition = new Vector3(0f, -0.44f, 0.04f);
        CreateMeshObject("Oxford Shoe", salesmanLeftFoot, SquidGameCharacterMeshes.GetDressShoe(), carBlackMat, Vector3.zero, Quaternion.identity, Vector3.one);

        BuildSalesmanBriefcase();
    }

    private void BuildSalesmanBriefcase()
    {
        salesmanBriefcase = new GameObject("Leather Briefcase").transform;
        salesmanBriefcase.SetParent(salesmanLeftHand, false);
        salesmanBriefcase.localPosition = new Vector3(-0.06f, -0.22f, 0.08f);
        salesmanBriefcase.localRotation = Quaternion.Euler(0f, 90f, 0f);

        CreatePrimitive(PrimitiveType.Cube, "Case Body", salesmanBriefcase, new Vector3(0.55f, 0.14f, 0.42f), new Vector3(0f, 0.07f, 0f), Quaternion.identity, briefcaseMat);

        var lidObj = new GameObject("Briefcase Lid");
        lidObj.transform.SetParent(salesmanBriefcase, false);
        lidObj.transform.localPosition = new Vector3(0f, 0.14f, -0.21f);
        briefcaseLid = lidObj.transform;
        CreatePrimitive(PrimitiveType.Cube, "Lid Shell", briefcaseLid, new Vector3(0.55f, 0.035f, 0.42f), new Vector3(0f, 0.018f, 0.21f), Quaternion.identity, briefcaseMat);

        CreatePrimitive(PrimitiveType.Cube, "Brass Handle", salesmanBriefcase, new Vector3(0.16f, 0.06f, 0.035f), new Vector3(0f, 0.16f, 0.22f), Quaternion.identity, tactileYellowMat);

        cashStack = CreatePrimitive(PrimitiveType.Cube, "Cash Bundles 100k", salesmanBriefcase,
            new Vector3(0.24f, 0.06f, 0.14f), new Vector3(-0.12f, 0.11f, 0f), Quaternion.identity, cashMat).transform;
        cashStack.gameObject.SetActive(false);

        redDdakjiBriefcase = CreatePrimitive(PrimitiveType.Cube, "Red Ddakji in Case", salesmanBriefcase,
            new Vector3(0.14f, 0.025f, 0.14f), new Vector3(0.12f, 0.09f, -0.06f), Quaternion.identity, ddakjiRedMat).transform;
        redDdakjiBriefcase.gameObject.SetActive(false);

        blueDdakjiBriefcase = CreatePrimitive(PrimitiveType.Cube, "Blue Ddakji in Case", salesmanBriefcase,
            new Vector3(0.14f, 0.025f, 0.14f), new Vector3(0.12f, 0.09f, 0.08f), Quaternion.identity, ddakjiBlueMat).transform;
        blueDdakjiBriefcase.gameObject.SetActive(false);
    }

    private void BuildDdakjiArena()
    {
        var redObj = CreatePrimitive(PrimitiveType.Cube, "Red Ddakji on Ground", transform,
            new Vector3(0.35f, 0.035f, 0.35f), Vector3.zero, Quaternion.identity, ddakjiRedMat);
        redDdakjiGround = redObj.transform;
        redDdakjiGround.gameObject.SetActive(false);

        var blueObj = CreatePrimitive(PrimitiveType.Cube, "Blue Ddakji Thrown", transform,
            new Vector3(0.35f, 0.035f, 0.35f), Vector3.zero, Quaternion.identity, ddakjiBlueMat);
        blueDdakjiThrown = blueObj.transform;
        blueDdakjiThrown.gameObject.SetActive(false);
    }

    private void SetupRainAndLighting()
    {
        var sunLight = FindAnyObjectByType<Light>();
        if (sunLight != null)
        {
            sunLight.color = new Color(0.22f, 0.28f, 0.38f);
            sunLight.intensity = 0.85f;
            sunLight.shadows = LightShadows.Soft;
        }

        gameCamera = Camera.main;
        if (gameCamera == null)
        {
            var camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            gameCamera = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
        }
        gameCamera.fieldOfView = 62f;
        gameCamera.nearClipPlane = 0.15f;

        BuildRainParticles();
    }

    private void BuildRainParticles()
    {
        var rainObj = new GameObject("Falling Rain System");
        rainObj.transform.SetParent(transform, false);
        rainObj.transform.localPosition = new Vector3(0f, StreetFloorY + 16f, 55f);

        var ps = rainObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 1.0f;
        main.startSpeed = 26f;
        main.startSize = 0.06f;
        main.maxParticles = 2500;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 1400f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(38f, 1f, 85f);

        var psRenderer = rainObj.GetComponent<ParticleSystemRenderer>();
        var rainMat = MakeMaterial(new Color(0.85f, 0.92f, 1.0f, 0.65f), 0.9f, 0.0f, SquidGameTextures.GetRainDrop());
        psRenderer.material = rainMat;
        psRenderer.renderMode = ParticleSystemRenderMode.Stretch;
        psRenderer.lengthScale = 2.4f;
    }

    // =========================================================================
    // FACTORY HELPERS
    // =========================================================================
    private static GameObject CreateMeshObject(string name, Transform parent, Mesh mesh, Material material,
        Vector3 localPos, Quaternion localRot, Vector3 localScale, bool withCollider = false)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = localRot;
        obj.transform.localScale = localScale;

        var mf = obj.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        var mr = obj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;

        if (withCollider)
        {
            var mc = obj.AddComponent<MeshCollider>();
            mc.sharedMesh = mesh;
        }

        return obj;
    }

    private static Shader GetDefaultShader()
    {
        var s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        if (s == null) s = Shader.Find("Diffuse");
        return s;
    }

    private static Material MakeMaterial(Color color, float smoothness, float metallic, Texture2D baseTex = null, Texture2D normalTex = null)
    {
        var shader = GetDefaultShader();
        var mat = new Material(shader) { color = color };

        if (baseTex != null)
        {
            mat.mainTexture = baseTex;
            mat.SetTexture("_BaseMap", baseTex);
        }

        if (normalTex != null)
        {
            mat.EnableKeyword("_NORMALMAP");
            mat.SetTexture("_BumpMap", normalTex);
            mat.SetFloat("_BumpScale", 1.0f);
        }

        mat.SetFloat("_Smoothness", smoothness);
        mat.SetFloat("_Metallic", metallic);
        return mat;
    }

    private static Material MakeEmissive(Texture2D baseTex, Color emissionColor)
    {
        var shader = GetDefaultShader();
        var mat = new Material(shader) { color = Color.white };

        if (baseTex != null)
        {
            mat.mainTexture = baseTex;
            mat.SetTexture("_BaseMap", baseTex);
            mat.SetTexture("_EmissionMap", baseTex);
        }

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emissionColor);
        mat.SetFloat("_Smoothness", 0.85f);
        return mat;
    }

    private static GameObject CreatePrimitive(PrimitiveType type, string objectName, Transform parent,
        Vector3 scale, Vector3 localPosition, Quaternion localRotation, Material material, bool withCollider = false)
    {
        var instance = GameObject.CreatePrimitive(type);
        instance.name = objectName;
        instance.transform.SetParent(parent, false);
        instance.transform.localScale = scale;
        instance.transform.localPosition = localPosition;
        instance.transform.localRotation = localRotation;

        var rend = instance.GetComponent<Renderer>();
        if (rend != null) rend.sharedMaterial = material;

        var col = instance.GetComponent<Collider>();
        if (col != null) col.enabled = withCollider;

        return instance;
    }
}
