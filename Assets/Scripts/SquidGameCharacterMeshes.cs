using System.Collections.Generic;
using UnityEngine;

public static class SquidGameCharacterMeshes
{
    private static Mesh suitJacketMesh;
    private static Mesh playerParkaMesh;
    private static Mesh suitUpperArmMesh;
    private static Mesh suitForearmMesh;
    private static Mesh playerUpperArmMesh;
    private static Mesh playerForearmMesh;
    private static Mesh suitThighMesh;
    private static Mesh suitShinMesh;
    private static Mesh playerThighMesh;
    private static Mesh playerShinMesh;
    private static Mesh dressShoeMesh;
    private static Mesh sneakerMesh;
    private static Mesh salesmanHeadMesh;
    private static Mesh playerHeadMesh;
    private static Mesh detailedHandMesh;
    private static Mesh slapHandMesh;
    private static Mesh pinkGuardTorsoMesh;
    private static Mesh pinkGuardMaskMesh;
    private static Mesh subwayBenchMesh;
    private static Mesh trainTrackMesh;
    private static Mesh platformPillarMesh;
    private static Mesh entranceCanopyMesh;

    // Public Accessors
    public static Mesh GetSuitJacket() => suitJacketMesh ??= BuildSuitJacketMesh();
    public static Mesh GetPlayerJacket() => playerParkaMesh ??= BuildPlayerParkaMesh();
    public static Mesh GetSuitUpperArm() => suitUpperArmMesh ??= BuildLimbSegmentMesh(0.38f, 0.082f, 0.072f, true, false);
    public static Mesh GetSuitForearm() => suitForearmMesh ??= BuildLimbSegmentMesh(0.36f, 0.070f, 0.058f, true, true);
    public static Mesh GetPlayerUpperArm() => playerUpperArmMesh ??= BuildLimbSegmentMesh(0.38f, 0.092f, 0.080f, false, false);
    public static Mesh GetPlayerForearm() => playerForearmMesh ??= BuildLimbSegmentMesh(0.36f, 0.078f, 0.065f, false, true);
    public static Mesh GetSuitThigh() => suitThighMesh ??= BuildLimbSegmentMesh(0.48f, 0.115f, 0.092f, true, false);
    public static Mesh GetSuitShin() => suitShinMesh ??= BuildLimbSegmentMesh(0.46f, 0.090f, 0.075f, true, true);
    public static Mesh GetPlayerThigh() => playerThighMesh ??= BuildLimbSegmentMesh(0.48f, 0.120f, 0.098f, false, false);
    public static Mesh GetPlayerShin() => playerShinMesh ??= BuildLimbSegmentMesh(0.46f, 0.096f, 0.080f, false, true);
    public static Mesh GetDressShoe() => dressShoeMesh ??= BuildShoeMesh(true);
    public static Mesh GetSneaker() => sneakerMesh ??= BuildShoeMesh(false);
    public static Mesh GetSalesmanHead() => salesmanHeadMesh ??= BuildRealisticHeadMesh(true);
    public static Mesh GetPlayerHead() => playerHeadMesh ??= BuildRealisticHeadMesh(false);
    public static Mesh GetDetailedHand() => detailedHandMesh ??= BuildDetailedHandMesh(false);
    public static Mesh GetSlapHand() => slapHandMesh ??= BuildDetailedHandMesh(true);
    public static Mesh GetPinkGuardTorso() => pinkGuardTorsoMesh ??= BuildPinkGuardTorsoMesh();
    public static Mesh GetPinkGuardMask() => pinkGuardMaskMesh ??= BuildPinkGuardMaskMesh();
    public static Mesh GetSubwayBench() => subwayBenchMesh ??= BuildSubwayBenchMesh();
    public static Mesh GetTrainTracks() => trainTrackMesh ??= BuildTrainTracksMesh();
    public static Mesh GetPlatformPillar() => platformPillarMesh ??= BuildPlatformPillarMesh();
    public static Mesh GetEntranceCanopy() => entranceCanopyMesh ??= BuildEntranceCanopyMesh();

    // Legacy backwards-compatible accessors
    public static Mesh GetSuitPants() => GetSuitThigh();
    public static Mesh GetPlayerJeans() => GetPlayerThigh();
    public static Mesh GetHumanHead() => GetPlayerHead();

    // =========================================================================
    // 1. REALISTIC HUMAN HEAD & FACIAL ANATOMY (EYES, NOSE, LIPS, JAW, HAIR)
    // =========================================================================
    private static Mesh BuildRealisticHeadMesh(bool isSalesman)
    {
        var builder = new CustomMeshBuilder();

        // 1. Cranium / Skull Vault (Smooth 16x16 ellipsoid)
        builder.AddSmoothEllipsoid(new Vector3(0f, 0.06f, -0.015f), new Vector3(0.125f, 0.145f, 0.142f), 12, 16);

        // 2. Forehead & Brow Ridge
        builder.AddSmoothEllipsoid(new Vector3(0f, 0.075f, 0.095f), new Vector3(0.105f, 0.045f, 0.055f), 8, 12);

        // 3. Eye Sockets & Eyeballs
        foreach (float sx in new[] { -0.048f, 0.048f })
        {
            // Eyeball dome
            builder.AddSmoothEllipsoid(new Vector3(sx, 0.038f, 0.118f), new Vector3(0.024f, 0.018f, 0.020f), 6, 8);
            // Pupil & Iris
            builder.AddBox(new Vector3(sx, 0.038f, 0.136f), new Vector3(0.012f, 0.012f, 0.005f), Quaternion.identity);
            // Eyebrow arch
            builder.AddBox(new Vector3(sx, 0.065f, 0.125f), new Vector3(0.042f, 0.010f, 0.018f), Quaternion.Euler(0f, 0f, sx > 0 ? -12f : 12f));
        }

        // 4. Sculpted Nose (Bridge, Dorsum, and Nostril flare)
        builder.AddSmoothCylinder(new Vector3(0f, 0.060f, 0.110f), new Vector3(0f, -0.012f, 0.155f), 0.015f, 0.024f, 8);
        builder.AddSmoothEllipsoid(new Vector3(0f, -0.012f, 0.155f), new Vector3(0.026f, 0.016f, 0.022f), 6, 8); // Tip

        // 5. Mouth & Lips (Upper & Lower vermilion borders)
        builder.AddSmoothEllipsoid(new Vector3(0f, -0.052f, 0.130f), new Vector3(0.038f, 0.010f, 0.018f), 6, 10); // Upper lip
        builder.AddSmoothEllipsoid(new Vector3(0f, -0.070f, 0.128f), new Vector3(0.035f, 0.012f, 0.018f), 6, 10); // Lower lip

        // 6. Cheekbones & Cheeks
        foreach (float sx in new[] { -0.082f, 0.082f })
        {
            builder.AddSmoothEllipsoid(new Vector3(sx, 0.015f, 0.075f), new Vector3(0.038f, 0.045f, 0.048f), 8, 8);
        }

        // 7. Tapered Jawline & Sculpted Chin
        builder.AddSmoothCylinder(new Vector3(0f, -0.025f, 0.040f), new Vector3(0f, -0.135f, 0.085f), 0.098f, 0.045f, 12);
        builder.AddSmoothEllipsoid(new Vector3(0f, -0.138f, 0.092f), new Vector3(0.046f, 0.032f, 0.040f), 8, 10); // Chin button

        // 8. Sculpted Ears (Left & Right)
        foreach (float sx in new[] { -0.128f, 0.128f })
        {
            builder.AddSmoothEllipsoid(new Vector3(sx, 0.025f, -0.015f), new Vector3(0.016f, 0.052f, 0.032f), 8, 8);
        }

        // 9. Hairstyle
        if (isSalesman)
        {
            // Gong Yoo / Salesman: Sharp, side-parted polished gentleman's cut
            builder.AddSmoothEllipsoid(new Vector3(0f, 0.165f, -0.015f), new Vector3(0.138f, 0.065f, 0.155f), 10, 16);
            builder.AddSmoothEllipsoid(new Vector3(0.025f, 0.135f, 0.092f), new Vector3(0.125f, 0.045f, 0.068f), 8, 12);
            // Sideburns
            builder.AddBox(new Vector3(-0.122f, 0.045f, 0.035f), new Vector3(0.018f, 0.085f, 0.042f), Quaternion.Euler(0f, 10f, 0f));
            builder.AddBox(new Vector3( 0.122f, 0.045f, 0.035f), new Vector3(0.018f, 0.085f, 0.042f), Quaternion.Euler(0f, -10f, 0f));
        }
        else
        {
            // Seong Gi-hun (Player): Disheveled, wavy street fringe bangs and layered hair
            builder.AddSmoothEllipsoid(new Vector3(0f, 0.170f, -0.010f), new Vector3(0.145f, 0.075f, 0.160f), 10, 16);
            // Front messy wavy fringe hanging over forehead
            builder.AddSmoothEllipsoid(new Vector3(-0.035f, 0.120f, 0.115f), new Vector3(0.085f, 0.048f, 0.055f), 8, 10);
            builder.AddSmoothEllipsoid(new Vector3( 0.042f, 0.112f, 0.118f), new Vector3(0.075f, 0.042f, 0.052f), 8, 10);
            // Side curls
            builder.AddSmoothEllipsoid(new Vector3(-0.125f, 0.055f, 0.015f), new Vector3(0.032f, 0.075f, 0.075f), 8, 8);
            builder.AddSmoothEllipsoid(new Vector3( 0.125f, 0.055f, 0.015f), new Vector3(0.032f, 0.075f, 0.075f), 8, 8);
        }

        return builder.ToMesh(isSalesman ? "Salesman_Realistic_Head" : "Player_Realistic_Head");
    }

    // =========================================================================
    // 2. TAILORED SUIT JACKET TORSO (SALESMAN)
    // =========================================================================
    private static Mesh BuildSuitJacketMesh()
    {
        var builder = new CustomMeshBuilder();

        // Main Chest & Torso Barrel (Smooth contoured rings)
        float[] ringY = { 0.32f, 0.18f, 0.02f, -0.16f, -0.32f };
        float[] ringW = { 0.24f, 0.23f, 0.20f, 0.21f,  0.22f };
        float[] ringD = { 0.13f, 0.14f, 0.12f, 0.125f, 0.135f };

        int ringCount = ringY.Length;
        int segments = 16;
        int baseIdx = builder.VertexCount;

        for (int r = 0; r < ringCount; r++)
        {
            float y = ringY[r];
            float w = ringW[r];
            float d = ringD[r];

            for (int s = 0; s < segments; s++)
            {
                float angle = s * Mathf.PI * 2f / segments;
                float x = Mathf.Cos(angle) * w;
                float z = Mathf.Sin(angle) * d;

                // Broad chest structure
                if (z > 0f && (r == 0 || r == 1)) z *= 1.15f;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 norm = new Vector3(Mathf.Cos(angle), 0.1f, Mathf.Sin(angle)).normalized;
                builder.AddVertex(pos, norm, new Vector2((float)s / segments, (float)r / (ringCount - 1)));
            }
        }

        for (int r = 0; r < ringCount - 1; r++)
        {
            int rStart = baseIdx + r * segments;
            int nextStart = baseIdx + (r + 1) * segments;
            for (int s = 0; s < segments; s++)
            {
                int sNext = (s + 1) % segments;
                builder.AddQuad(rStart + s, rStart + sNext, nextStart + sNext, nextStart + s);
            }
        }

        // Structured Shoulder Pads
        foreach (float sx in new[] { -0.22f, 0.22f })
        {
            builder.AddSmoothEllipsoid(new Vector3(sx, 0.29f, 0f), new Vector3(0.085f, 0.055f, 0.125f), 8, 10);
        }

        // 3D Peak Suit Lapels
        builder.AddBox(new Vector3(-0.075f, 0.16f, 0.145f), new Vector3(0.065f, 0.24f, 0.025f), Quaternion.Euler(0f, -8f, 14f));
        builder.AddBox(new Vector3( 0.075f, 0.16f, 0.145f), new Vector3(0.065f, 0.24f, 0.025f), Quaternion.Euler(0f, 8f, -14f));

        // Breast pocket & folded white pocket square
        builder.AddBox(new Vector3(0.12f, 0.16f, 0.140f), new Vector3(0.065f, 0.018f, 0.015f), Quaternion.identity);
        builder.AddBox(new Vector3(0.12f, 0.178f, 0.142f), new Vector3(0.045f, 0.012f, 0.008f), Quaternion.identity);

        return builder.ToMesh("Suit_Jacket_Sculpt");
    }

    // =========================================================================
    // 3. LAYERED WINTER PARKA JACKET (GI-HUN)
    // =========================================================================
    private static Mesh BuildPlayerParkaMesh()
    {
        var builder = new CustomMeshBuilder();

        float[] ringY = { 0.32f, 0.16f, -0.02f, -0.20f, -0.34f };
        float[] ringW = { 0.25f, 0.24f, 0.22f,  0.23f,  0.24f };
        float[] ringD = { 0.16f, 0.17f, 0.16f,  0.165f, 0.17f };

        int ringCount = ringY.Length;
        int segments = 16;
        int baseIdx = builder.VertexCount;

        for (int r = 0; r < ringCount; r++)
        {
            float y = ringY[r];
            float w = ringW[r];
            float d = ringD[r];

            for (int s = 0; s < segments; s++)
            {
                float angle = s * Mathf.PI * 2f / segments;
                float x = Mathf.Cos(angle) * w;
                float z = Mathf.Sin(angle) * d;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 norm = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
                builder.AddVertex(pos, norm, new Vector2((float)s / segments, (float)r / (ringCount - 1)));
            }
        }

        for (int r = 0; r < ringCount - 1; r++)
        {
            int rStart = baseIdx + r * segments;
            int nextStart = baseIdx + (r + 1) * segments;
            for (int s = 0; s < segments; s++)
            {
                int sNext = (s + 1) % segments;
                builder.AddQuad(rStart + s, rStart + sNext, nextStart + sNext, nextStart + s);
            }
        }

        // Puffy Winter Hood Fold draped around collar & back
        builder.AddSmoothEllipsoid(new Vector3(0f, 0.32f, -0.12f), new Vector3(0.20f, 0.08f, 0.11f), 8, 12);

        // Front zipper baffle running down center
        builder.AddBox(new Vector3(0f, 0.0f, 0.172f), new Vector3(0.038f, 0.65f, 0.022f), Quaternion.identity);

        // Twin Front Cargo Pockets
        foreach (float sx in new[] { -0.13f, 0.13f })
        {
            builder.AddBox(new Vector3(sx, -0.18f, 0.165f), new Vector3(0.085f, 0.10f, 0.035f), Quaternion.identity);
        }

        return builder.ToMesh("Player_Parka_Sculpt");
    }

    // =========================================================================
    // 4. ANATOMICAL LIMB SEGMENTS (SMOOTH ARMS, FOREARMS, THIGHS, SHINS)
    // =========================================================================
    private static Mesh BuildLimbSegmentMesh(float length, float rTop, float rBottom, bool isSuit, bool isLower)
    {
        var builder = new CustomMeshBuilder();

        int segments = 12;
        int rings = 6;
        int baseIdx = builder.VertexCount;

        for (int r = 0; r < rings; r++)
        {
            float t = (float)r / (rings - 1);
            float y = Mathf.Lerp(0f, -length, t);
            float radius = Mathf.Lerp(rTop, rBottom, t);

            // Muscle bulge contouring
            if (isLower && !isSuit && t > 0.15f && t < 0.6f)
            {
                radius *= 1.12f; // Calf / Forearm bulge
            }

            for (int s = 0; s < segments; s++)
            {
                float angle = s * Mathf.PI * 2f / segments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius * (isSuit ? 1.12f : 1.0f); // Creased oval for pressed trousers

                Vector3 pos = new Vector3(x, y, z);
                Vector3 norm = new Vector3(x, 0.1f, z).normalized;
                builder.AddVertex(pos, norm, new Vector2((float)s / segments, t));
            }
        }

        for (int r = 0; r < rings - 1; r++)
        {
            int rStart = baseIdx + r * segments;
            int nextStart = baseIdx + (r + 1) * segments;
            for (int s = 0; s < segments; s++)
            {
                int sNext = (s + 1) % segments;
                builder.AddQuad(rStart + s, rStart + sNext, nextStart + sNext, nextStart + s);
            }
        }

        // Cuff rim at distal end
        builder.AddSmoothCylinder(new Vector3(0f, -length + 0.02f, 0f), new Vector3(0f, -length, 0f), rBottom * 1.08f, rBottom * 1.08f, 12);

        return builder.ToMesh(isSuit ? "Suit_Limb_Segment" : "Street_Limb_Segment");
    }

    // =========================================================================
    // 5. ARTICULATED DETAILED HAND WITH 5 FINGERS & KNUCKLES
    // =========================================================================
    private static Mesh BuildDetailedHandMesh(bool isOpenSlapHand)
    {
        var builder = new CustomMeshBuilder();

        // Palm Base (Contoured organic palm)
        builder.AddSmoothEllipsoid(new Vector3(0f, -0.045f, 0f), new Vector3(0.048f, 0.052f, 0.022f), 8, 10);

        // 4 Articulated Fingers (Index, Middle, Ring, Little)
        float[] fingerX = { 0.032f, 0.011f, -0.011f, -0.030f };
        float[] fingerLen = { 0.072f, 0.082f, 0.076f, 0.062f };
        float curlAngle = isOpenSlapHand ? 6f : 28f; // Open palm for slapping!

        for (int i = 0; i < 4; i++)
        {
            float x = fingerX[i];
            float len = fingerLen[i];
            float halfLen = len * 0.5f;

            // Proximal phalanx (Knuckle to middle joint)
            Vector3 p0 = new Vector3(x, -0.075f, 0.005f);
            Vector3 p1 = p0 + Quaternion.Euler(curlAngle, 0f, 0f) * new Vector3(0f, -halfLen, 0f);
            builder.AddSmoothCylinder(p0, p1, 0.011f, 0.0095f, 6);

            // Distal phalanx (Finger tip)
            Vector3 p2 = p1 + Quaternion.Euler(curlAngle * 1.8f, 0f, 0f) * new Vector3(0f, -halfLen, 0f);
            builder.AddSmoothCylinder(p1, p2, 0.0095f, 0.0075f, 6);
            builder.AddSmoothEllipsoid(p2, new Vector3(0.008f, 0.008f, 0.008f), 4, 6);
        }

        // Thumb (Opposable at 45 degree angle)
        Vector3 t0 = new Vector3(0.042f, -0.032f, 0.012f);
        Vector3 t1 = t0 + Quaternion.Euler(15f, 0f, -38f) * new Vector3(0.035f, -0.035f, 0f);
        builder.AddSmoothCylinder(t0, t1, 0.012f, 0.009f, 6);
        builder.AddSmoothEllipsoid(t1, new Vector3(0.009f, 0.009f, 0.009f), 4, 6);

        return builder.ToMesh(isOpenSlapHand ? "Slap_Hand_Sculpt" : "Detailed_Hand_Sculpt");
    }

    // =========================================================================
    // 6. REALISTIC FOOTWEAR (OXFORD DRESS SHOE & STREET SNEAKER)
    // =========================================================================
    private static Mesh BuildShoeMesh(bool isOxford)
    {
        var builder = new CustomMeshBuilder();

        // 1. Outsole Platform
        builder.AddSmoothCylinder(new Vector3(0f, 0.01f, 0.04f), new Vector3(0f, 0.035f, 0.04f), 0.068f, 0.065f, 12);
        builder.AddBox(new Vector3(0f, 0.02f, 0.04f), new Vector3(0.125f, 0.035f, 0.28f), Quaternion.identity);

        if (isOxford)
        {
            // Raised Oxford Heel Block
            builder.AddBox(new Vector3(0f, 0.035f, -0.075f), new Vector3(0.115f, 0.045f, 0.085f), Quaternion.identity);
            // Sleek leather vamp & toe cap
            builder.AddSmoothEllipsoid(new Vector3(0f, 0.075f, 0.06f), new Vector3(0.062f, 0.052f, 0.125f), 8, 12);
            // Lace facing flaps
            builder.AddBox(new Vector3(0f, 0.095f, 0.02f), new Vector3(0.055f, 0.025f, 0.085f), Quaternion.identity);
        }
        else
        {
            // Sneaker thick rubber midsole rand
            builder.AddBox(new Vector3(0f, 0.028f, 0.04f), new Vector3(0.135f, 0.045f, 0.29f), Quaternion.identity);
            // Padded sneaker upper & rounded toe bumper
            builder.AddSmoothEllipsoid(new Vector3(0f, 0.082f, 0.055f), new Vector3(0.068f, 0.058f, 0.135f), 8, 12);
            // Ankle padded collar
            builder.AddSmoothCylinder(new Vector3(0f, 0.085f, -0.04f), new Vector3(0f, 0.135f, -0.04f), 0.058f, 0.052f, 10);
        }

        return builder.ToMesh(isOxford ? "Oxford_Shoe_Mesh" : "Sneaker_Mesh");
    }

    // =========================================================================
    // 7. PINK GUARD TACTICAL JUMPSUIT TORSO & VISOR MASK
    // =========================================================================
    private static Mesh BuildPinkGuardTorsoMesh()
    {
        var builder = new CustomMeshBuilder();

        // Jumpsuit Torso Body
        builder.AddSmoothEllipsoid(new Vector3(0f, 0.0f, 0f), new Vector3(0.24f, 0.32f, 0.16f), 10, 14);

        // Tactical Webbing Chest Harness (Diagonal X-straps)
        builder.AddBox(new Vector3(0f, 0.06f, 0.162f), new Vector3(0.048f, 0.42f, 0.015f), Quaternion.Euler(0f, 0f, 32f));
        builder.AddBox(new Vector3(0f, 0.06f, 0.162f), new Vector3(0.048f, 0.42f, 0.015f), Quaternion.Euler(0f, 0f, -32f));

        // Black Utility Duty Belt around waist
        builder.AddSmoothCylinder(new Vector3(0f, -0.22f, 0f), new Vector3(0f, -0.28f, 0f), 0.225f, 0.225f, 14);

        // Ammo pouches & holster attached to belt
        builder.AddBox(new Vector3(0.12f, -0.25f, 0.18f), new Vector3(0.065f, 0.09f, 0.045f), Quaternion.identity);
        builder.AddBox(new Vector3(-0.12f, -0.25f, 0.18f), new Vector3(0.065f, 0.09f, 0.045f), Quaternion.identity);
        builder.AddBox(new Vector3(-0.21f, -0.28f, 0.0f), new Vector3(0.055f, 0.14f, 0.085f), Quaternion.identity); // Holster

        return builder.ToMesh("Pink_Guard_Torso_Mesh");
    }

    private static Mesh BuildPinkGuardMaskMesh()
    {
        var builder = new CustomMeshBuilder();

        // Convex Fencing Mask Dome (Smooth aerodynamic curved mesh)
        builder.AddSmoothEllipsoid(new Vector3(0f, 0f, 0f), new Vector3(0.128f, 0.155f, 0.140f), 12, 16);

        // Front Faceplate Rim
        builder.AddSmoothEllipsoid(new Vector3(0f, 0.01f, 0.085f), new Vector3(0.105f, 0.135f, 0.075f), 10, 14);

        // Pristine White Square (□) printed on front faceplate
        float sqSize = 0.09f;
        float barThick = 0.016f;
        float zPos = 0.158f;
        builder.AddBox(new Vector3(0f,  sqSize * 0.5f, zPos), new Vector3(sqSize, barThick, 0.01f), Quaternion.identity); // Top
        builder.AddBox(new Vector3(0f, -sqSize * 0.5f, zPos), new Vector3(sqSize, barThick, 0.01f), Quaternion.identity); // Bottom
        builder.AddBox(new Vector3(-sqSize * 0.5f, 0f, zPos), new Vector3(barThick, sqSize, 0.01f), Quaternion.identity); // Left
        builder.AddBox(new Vector3( sqSize * 0.5f, 0f, zPos), new Vector3(barThick, sqSize, 0.01f), Quaternion.identity); // Right

        return builder.ToMesh("Pink_Guard_Mask_Mesh");
    }

    // =========================================================================
    // 8. SEOUL METRO ENTRANCE CANOPY (EXIT 2 TO STREET)
    // =========================================================================
    private static Mesh BuildEntranceCanopyMesh()
    {
        var builder = new CustomMeshBuilder();

        // Modern Steel Truss Archway over the subway entrance
        float width = 7.4f;
        float length = 18.0f;
        float height = 4.2f;

        // Side Support Columns
        foreach (float sx in new[] { -width * 0.5f, width * 0.5f })
        {
            builder.AddBox(new Vector3(sx, height * 0.5f, 0f), new Vector3(0.24f, height, length), Quaternion.identity);
            builder.AddBox(new Vector3(sx, height * 0.5f, 0f), new Vector3(0.08f, height * 0.85f, length - 0.4f), Quaternion.identity);
        }

        // Angled Glass Roof Canopy
        builder.AddBox(new Vector3(0f, height + 0.15f, 0f), new Vector3(width + 0.6f, 0.12f, length + 0.8f), Quaternion.identity);

        return builder.ToMesh("Subway_Entrance_Canopy_Mesh");
    }

    // =========================================================================
    // 9. PROPS: SUBWAY BENCH, TRAIN TRACKS, PLATFORM PILLAR
    // =========================================================================
    private static Mesh BuildSubwayBenchMesh()
    {
        var builder = new CustomMeshBuilder();

        for (int i = -1; i <= 1; i++)
        {
            float x = i * 0.68f;
            builder.AddBox(new Vector3(x, 0.42f, 0f), new Vector3(0.58f, 0.05f, 0.48f), Quaternion.identity);
            builder.AddBox(new Vector3(x, 0.70f, -0.22f), new Vector3(0.58f, 0.48f, 0.05f), Quaternion.Euler(12f, 0f, 0f));
        }

        foreach (float lx in new[] { -0.88f, 0.88f })
        {
            builder.AddSmoothCylinder(new Vector3(lx, 0f, -0.16f), new Vector3(lx, 0.42f, -0.16f), 0.032f, 0.032f, 8);
            builder.AddSmoothCylinder(new Vector3(lx, 0f,  0.16f), new Vector3(lx, 0.42f,  0.16f), 0.032f, 0.032f, 8);
            builder.AddSmoothCylinder(new Vector3(lx, 0.03f, -0.18f), new Vector3(lx, 0.03f, 0.18f), 0.028f, 0.028f, 8);
        }

        return builder.ToMesh("Subway_Bench_Mesh");
    }

    private static Mesh BuildTrainTracksMesh()
    {
        var builder = new CustomMeshBuilder();
        float length = 120f;

        foreach (float rx in new[] { -0.72f, 0.72f })
        {
            builder.AddBox(new Vector3(rx, 0.18f, 0f), new Vector3(0.07f, 0.04f, length), Quaternion.identity);
            builder.AddBox(new Vector3(rx, 0.10f, 0f), new Vector3(0.025f, 0.12f, length), Quaternion.identity);
            builder.AddBox(new Vector3(rx, 0.02f, 0f), new Vector3(0.12f, 0.03f, length), Quaternion.identity);
        }

        for (float z = -length * 0.5f; z < length * 0.5f; z += 0.68f)
        {
            builder.AddBox(new Vector3(0f, -0.03f, z), new Vector3(2.4f, 0.12f, 0.24f), Quaternion.identity);
        }

        return builder.ToMesh("Train_Tracks_Mesh");
    }

    private static Mesh BuildPlatformPillarMesh()
    {
        var builder = new CustomMeshBuilder();
        builder.AddBox(new Vector3(0f, 2.5f, 0f), new Vector3(1.1f, 5.0f, 1.1f), Quaternion.identity);
        builder.AddBox(new Vector3(0f, 0.2f, 0f), new Vector3(1.25f, 0.4f, 1.25f), Quaternion.identity);
        builder.AddBox(new Vector3(0f, 4.85f, 0f), new Vector3(1.22f, 0.3f, 1.22f), Quaternion.identity);
        return builder.ToMesh("Platform_Pillar_Mesh");
    }

    // =========================================================================
    // ENHANCED PROCEDURAL MESH BUILDER (WITH SMOOTH CYLINDERS, SPHERES, CAPSULES)
    // =========================================================================
    private sealed class CustomMeshBuilder
    {
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<Vector2> uvs = new List<Vector2>();
        private readonly List<int> triangles = new List<int>();

        public int VertexCount => vertices.Count;

        public void AddVertex(Vector3 pos, Vector3 norm, Vector2 uv)
        {
            vertices.Add(pos);
            normals.Add(norm);
            uvs.Add(uv);
        }

        public void AddQuad(int a, int b, int c, int d)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);

            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(d);
        }

        public void AddBox(Vector3 center, Vector3 size, Quaternion rotation)
        {
            Vector3 half = size * 0.5f;
            Vector3[] localCorners = new Vector3[]
            {
                new Vector3(-half.x, -half.y, -half.z),
                new Vector3( half.x, -half.y, -half.z),
                new Vector3( half.x,  half.y, -half.z),
                new Vector3(-half.x,  half.y, -half.z),
                new Vector3(-half.x, -half.y,  half.z),
                new Vector3( half.x, -half.y,  half.z),
                new Vector3( half.x,  half.y,  half.z),
                new Vector3(-half.x,  half.y,  half.z),
            };

            for (int i = 0; i < 8; i++)
            {
                localCorners[i] = center + (rotation * localCorners[i]);
            }

            AddFace(localCorners[0], localCorners[1], localCorners[2], localCorners[3], rotation * Vector3.back);
            AddFace(localCorners[5], localCorners[4], localCorners[7], localCorners[6], rotation * Vector3.forward);
            AddFace(localCorners[4], localCorners[0], localCorners[3], localCorners[7], rotation * Vector3.left);
            AddFace(localCorners[1], localCorners[5], localCorners[6], localCorners[2], rotation * Vector3.right);
            AddFace(localCorners[3], localCorners[2], localCorners[6], localCorners[7], rotation * Vector3.up);
            AddFace(localCorners[4], localCorners[5], localCorners[1], localCorners[0], rotation * Vector3.down);
        }

        private void AddFace(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal)
        {
            int start = vertices.Count;
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            uvs.Add(new Vector2(0f, 0f));
            uvs.Add(new Vector2(1f, 0f));
            uvs.Add(new Vector2(1f, 1f));
            uvs.Add(new Vector2(0f, 1f));

            triangles.Add(start + 0);
            triangles.Add(start + 1);
            triangles.Add(start + 2);

            triangles.Add(start + 0);
            triangles.Add(start + 2);
            triangles.Add(start + 3);
        }

        public void AddSmoothCylinder(Vector3 bottom, Vector3 top, float rBottom, float rTop, int segments)
        {
            int baseIdx = vertices.Count;
            Vector3 axis = (top - bottom);
            float length = axis.magnitude;
            if (length < 0.0001f) return;
            Vector3 dir = axis.normalized;

            Vector3 right = Vector3.Cross(dir, Vector3.up);
            if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(dir, Vector3.right);
            right.Normalize();
            Vector3 fwd = Vector3.Cross(right, dir).normalized;

            for (int r = 0; r < 2; r++)
            {
                float t = r;
                Vector3 center = (r == 0) ? bottom : top;
                float radius = (r == 0) ? rBottom : rTop;

                for (int s = 0; s < segments; s++)
                {
                    float angle = s * Mathf.PI * 2f / segments;
                    Vector3 radial = (right * Mathf.Cos(angle) + fwd * Mathf.Sin(angle));
                    Vector3 pos = center + radial * radius;
                    Vector3 norm = radial;
                    AddVertex(pos, norm, new Vector2((float)s / segments, t));
                }
            }

            for (int s = 0; s < segments; s++)
            {
                int sNext = (s + 1) % segments;
                AddQuad(baseIdx + s, baseIdx + sNext, baseIdx + segments + sNext, baseIdx + segments + s);
            }
        }

        public void AddSmoothEllipsoid(Vector3 center, Vector3 radii, int latSegments, int lonSegments)
        {
            int baseIdx = vertices.Count;

            for (int lat = 0; lat <= latSegments; lat++)
            {
                float v = (float)lat / latSegments;
                float theta = v * Mathf.PI;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= lonSegments; lon++)
                {
                    float u = (float)lon / lonSegments;
                    float phi = u * Mathf.PI * 2f;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    Vector3 norm = new Vector3(sinTheta * cosPhi, cosTheta, sinTheta * sinPhi);
                    Vector3 pos = center + new Vector3(norm.x * radii.x, norm.y * radii.y, norm.z * radii.z);
                    AddVertex(pos, norm.normalized, new Vector2(u, v));
                }
            }

            for (int lat = 0; lat < latSegments; lat++)
            {
                for (int lon = 0; lon < lonSegments; lon++)
                {
                    int cur = baseIdx + lat * (lonSegments + 1) + lon;
                    int next = cur + lonSegments + 1;
                    AddQuad(cur, cur + 1, next + 1, next);
                }
            }
        }

        public Mesh ToMesh(string name)
        {
            var mesh = new Mesh
            {
                name = name,
                vertices = vertices.ToArray(),
                normals = normals.ToArray(),
                uv = uvs.ToArray(),
                triangles = triangles.ToArray()
            };
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
