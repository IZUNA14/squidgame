using UnityEngine;

public static class SquidGameTextures
{
    private static Texture2D asphaltTex;
    private static Texture2D asphaltBump;
    private static Texture2D sidewalkTex;
    private static Texture2D sidewalkBump;
    private static Texture2D tactileTex;
    private static Texture2D subwayWallTex;
    private static Texture2D subwaySignTex;
    private static Texture2D convenienceSignTex;
    private static Texture2D noraebangSignTex;
    private static Texture2D billboardSquidTex;
    private static Texture2D ddakjiRedTex;
    private static Texture2D ddakjiBlueTex;
    private static Texture2D cardFrontTex;
    private static Texture2D cardBackTex;
    private static Texture2D cashTex;
    private static Texture2D briefcaseLeatherTex;
    private static Texture2D suitWoolTex;
    private static Texture2D rainDropTex;

    public static Texture2D GetAsphalt() => asphaltTex ??= CreateAsphalt(512, 512);
    public static Texture2D GetAsphaltNormal() => asphaltBump ??= CreateAsphaltNormal(256, 256);
    public static Texture2D GetSidewalk() => sidewalkTex ??= CreateSidewalk(512, 512);
    public static Texture2D GetSidewalkNormal() => sidewalkBump ??= CreateSidewalkNormal(256, 256);
    public static Texture2D GetTactileYellow() => tactileTex ??= CreateTactileYellow(256, 256);
    public static Texture2D GetSubwayWall() => subwayWallTex ??= CreateSubwayWall(256, 256);
    public static Texture2D GetSubwaySign() => subwaySignTex ??= CreateSubwaySign(512, 128);
    public static Texture2D GetConvenienceSign() => convenienceSignTex ??= CreateConvenienceSign(512, 256);
    public static Texture2D GetNoraebangSign() => noraebangSignTex ??= CreateNoraebangSign(512, 256);
    public static Texture2D GetSquidBillboard() => billboardSquidTex ??= CreateSquidBillboard(512, 256);
    public static Texture2D GetDdakjiRed() => ddakjiRedTex ??= CreateDdakji(256, new Color(0.85f, 0.16f, 0.16f));
    public static Texture2D GetDdakjiBlue() => ddakjiBlueTex ??= CreateDdakji(256, new Color(0.12f, 0.35f, 0.88f));
    public static Texture2D GetCardFront() => cardFrontTex ??= CreateCardFront(512, 280);
    public static Texture2D GetCardBack() => cardBackTex ??= CreateCardBack(512, 280);
    public static Texture2D GetCash() => cashTex ??= CreateCash(256, 128);
    public static Texture2D GetBriefcaseLeather() => briefcaseLeatherTex ??= CreateBriefcaseLeather(256, 256);
    public static Texture2D GetSuitWool() => suitWoolTex ??= CreateSuitWool(256, 256);
    public static Texture2D GetRainDrop() => rainDropTex ??= CreateRainStreak(64, 128);

    private static Texture2D CreateAsphalt(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = x * 0.08f;
                float ny = y * 0.08f;
                float gravelNoise = Mathf.PerlinNoise(nx, ny) * 0.15f + Mathf.PerlinNoise(nx * 4f, ny * 4f) * 0.05f;
                float puddleNoise = Mathf.PerlinNoise(x * 0.012f, y * 0.012f);
                float isPuddle = Mathf.SmoothStep(0.55f, 0.68f, puddleNoise);

                float baseLum = Mathf.Lerp(0.14f + gravelNoise, 0.05f + gravelNoise * 0.2f, isPuddle);
                pixels[y * width + x] = new Color(baseLum, baseLum * 1.02f, baseLum * 1.05f, 1f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    private static Texture2D CreateAsphaltNormal(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float hL = Mathf.PerlinNoise((x - 1) * 0.15f, y * 0.15f);
                float hR = Mathf.PerlinNoise((x + 1) * 0.15f, y * 0.15f);
                float hD = Mathf.PerlinNoise(x * 0.15f, (y - 1) * 0.15f);
                float hU = Mathf.PerlinNoise(x * 0.15f, (y + 1) * 0.15f);
                Vector3 norm = new Vector3((hL - hR) * 0.7f, (hD - hU) * 0.7f, 1f).normalized;
                pixels[y * width + x] = new Color(norm.x * 0.5f + 0.5f, norm.y * 0.5f + 0.5f, norm.z * 0.5f + 0.5f, 1f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    private static Texture2D CreateSidewalk(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        int tileSize = width / 4;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tx = x % tileSize;
                int ty = y % tileSize;
                bool isGrout = tx <= 3 || ty <= 3 || tx >= tileSize - 2 || ty >= tileSize - 2;
                float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f) * 0.08f;
                float baseLum = isGrout ? 0.35f : 0.62f + noise;
                pixels[y * width + x] = new Color(baseLum, baseLum * 0.98f, baseLum * 0.95f, 1f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    private static Texture2D CreateSidewalkNormal(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        int tileSize = width / 4;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tx = x % tileSize;
                int ty = y % tileSize;
                float nx = 0f;
                float ny = 0f;
                if (tx < 5) nx = 0.5f - (tx / 5f) * 0.5f;
                else if (tx > tileSize - 6) nx = -0.5f + ((tileSize - 1 - tx) / 5f) * 0.5f;
                if (ty < 5) ny = 0.5f - (ty / 5f) * 0.5f;
                else if (ty > tileSize - 6) ny = -0.5f + ((tileSize - 1 - ty) / 5f) * 0.5f;
                Vector3 norm = new Vector3(nx, ny, 1f).normalized;
                pixels[y * width + x] = new Color(norm.x * 0.5f + 0.5f, norm.y * 0.5f + 0.5f, norm.z * 0.5f + 0.5f, 1f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    private static Texture2D CreateTactileYellow(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        int dotSpacing = width / 8;
        float radius = dotSpacing * 0.28f;
        Color bgYellow = new Color(0.88f, 0.72f, 0.08f);
        Color dotYellow = new Color(0.98f, 0.86f, 0.18f);
        Color shadowYellow = new Color(0.68f, 0.54f, 0.04f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int cx = (x / dotSpacing) * dotSpacing + dotSpacing / 2;
                int cy = (y / dotSpacing) * dotSpacing + dotSpacing / 2;
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                if (dist < radius)
                {
                    float angle = Mathf.Atan2(y - cy, x - cx);
                    Color dotCol = angle > 0f ? dotYellow : shadowYellow;
                    pixels[y * width + x] = Color.Lerp(dotCol, bgYellow, dist / radius);
                }
                else
                {
                    pixels[y * width + x] = bgYellow;
                }
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    private static Texture2D CreateSubwayWall(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        int tileW = width / 4;
        int tileH = height / 8;
        Color tileColor = new Color(0.92f, 0.94f, 0.95f);
        Color groutColor = new Color(0.35f, 0.38f, 0.42f);

        for (int y = 0; y < height; y++)
        {
            int row = y / tileH;
            int shift = (row % 2 == 1) ? tileW / 2 : 0;
            for (int x = 0; x < width; x++)
            {
                int tx = (x + shift) % tileW;
                int ty = y % tileH;
                bool isGrout = tx <= 2 || ty <= 2;
                pixels[y * width + x] = isGrout ? groutColor : tileColor;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    private static Texture2D CreateSubwaySign(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color blueBg = new Color(0.04f, 0.28f, 0.72f);
        Color white = Color.white;
        Color yellow = new Color(0.95f, 0.82f, 0.1f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isBorder = (x < 6 || x > width - 7 || y < 6 || y > height - 7);
                if (isBorder)
                {
                    pixels[y * width + x] = white;
                    continue;
                }

                bool isCircle4 = (Vector2.Distance(new Vector2(x, y), new Vector2(width * 0.15f, height * 0.5f)) < height * 0.32f);
                if (isCircle4)
                {
                    pixels[y * width + x] = yellow;
                    continue;
                }

                bool isBanner = (y > height * 0.35f && y < height * 0.65f && x > width * 0.28f && x < width * 0.94f);
                pixels[y * width + x] = isBanner ? white : blueBg;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateConvenienceSign(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color darkBlue = new Color(0.05f, 0.12f, 0.28f);
        Color brightCyan = new Color(0.0f, 0.85f, 0.95f);
        Color brightOrange = new Color(1.0f, 0.45f, 0.05f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y < height * 0.2f)
                {
                    pixels[y * width + x] = brightOrange;
                }
                else if (y > height * 0.8f)
                {
                    pixels[y * width + x] = brightCyan;
                }
                else
                {
                    bool isTextBar = (x > width * 0.1f && x < width * 0.9f && y > height * 0.38f && y < height * 0.62f);
                    pixels[y * width + x] = isTextBar ? Color.white : darkBlue;
                }
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateNoraebangSign(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color darkViolet = new Color(0.12f, 0.03f, 0.22f);
        Color hotPink = new Color(1.0f, 0.08f, 0.55f);
        Color neonCyan = new Color(0.05f, 0.95f, 0.9f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isBorder = (x < 8 || x > width - 9 || y < 8 || y > height - 9);
                if (isBorder)
                {
                    pixels[y * width + x] = hotPink;
                }
                else
                {
                    bool isInnerBar = (x > width * 0.15f && x < width * 0.85f && y > height * 0.3f && y < height * 0.7f);
                    pixels[y * width + x] = isInnerBar ? neonCyan : darkViolet;
                }
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateSquidBillboard(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color black = new Color(0.06f, 0.06f, 0.08f);
        Color squidPink = new Color(1.0f, 0.12f, 0.54f);
        Color gold = new Color(0.98f, 0.82f, 0.24f);

        float cy = height * 0.52f;
        float r = height * 0.26f;

        // Circle at x = 0.24, Triangle at x = 0.5, Square at x = 0.76
        Vector2 circleCenter = new Vector2(width * 0.24f, cy);
        Vector2 squareCenter = new Vector2(width * 0.76f, cy);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 p = new Vector2(x, y);

                // Circle outline
                float dCircle = Vector2.Distance(p, circleCenter);
                bool inCircleRing = dCircle >= r - 8f && dCircle <= r + 8f;

                // Triangle outline
                float tx = x - width * 0.5f;
                float ty = y - (cy - r * 0.2f);
                float halfBase = r * 1.15f;
                float triHeight = r * 2f;
                bool inTriangle = false;
                if (ty >= -r * 0.8f && ty <= r * 1.2f)
                {
                    float expectedHalf = (1f - (ty + r * 0.8f) / triHeight) * halfBase;
                    float distFromEdge = Mathf.Abs(Mathf.Abs(tx) - expectedHalf);
                    bool onBottom = Mathf.Abs(ty - (-r * 0.8f)) < 8f && Mathf.Abs(tx) <= halfBase;
                    if (distFromEdge < 7f || onBottom) inTriangle = true;
                }

                // Square outline
                float dx = Mathf.Abs(x - squareCenter.x);
                float dy = Mathf.Abs(y - squareCenter.y);
                bool inSquare = (dx <= r && dy <= r) && (dx >= r - 12f || dy >= r - 12f);

                // Bottom gold banner
                bool isGoldBar = y < height * 0.14f && x > width * 0.08f && x < width * 0.92f;

                if (inCircleRing || inTriangle || inSquare)
                {
                    pixels[y * width + x] = squidPink;
                }
                else if (isGoldBar)
                {
                    pixels[y * width + x] = gold;
                }
                else
                {
                    pixels[y * width + x] = black;
                }
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateDdakji(int size, Color baseColor)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        var pixels = new Color[size * size];
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        Color darkFold = baseColor * 0.65f;
        Color brightFold = baseColor * 1.25f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center.x;
                float dy = y - center.y;

                // Fold diagonals
                bool isDiag1 = Mathf.Abs(dx - dy) < 3.5f;
                bool isDiag2 = Mathf.Abs(dx + dy) < 3.5f;
                bool isBorder = x < 5 || x > size - 6 || y < 5 || y > size - 6;

                // 4 folded quadrant flaps
                Color quadrantColor;
                if (dy > dx && dy > -dx) quadrantColor = brightFold; // Top
                else if (dx > dy && dx > -dy) quadrantColor = baseColor; // Right
                else if (dy < dx && dy < -dx) quadrantColor = darkFold; // Bottom
                else quadrantColor = baseColor * 0.85f; // Left

                if (isBorder || isDiag1 || isDiag2)
                {
                    pixels[y * widthOrSize(size) + x] = darkFold * 0.7f;
                }
                else
                {
                    float paperNoise = Mathf.PerlinNoise(x * 0.2f, y * 0.2f) * 0.08f;
                    pixels[y * size + x] = quadrantColor + new Color(paperNoise, paperNoise, paperNoise);
                }
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static int widthOrSize(int s) => s;

    private static Texture2D CreateCardFront(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color kraftBase = new Color(0.76f, 0.64f, 0.49f);
        Color debossColor = new Color(0.24f, 0.18f, 0.13f);
        Color goldShimmer = new Color(0.85f, 0.73f, 0.38f);

        float cy = height * 0.5f;
        float r = height * 0.26f;
        Vector2 cCenter = new Vector2(width * 0.23f, cy);
        Vector2 sCenter = new Vector2(width * 0.77f, cy);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float paperFiber = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.05f - 0.025f;
                Color col = kraftBase + new Color(paperFiber, paperFiber, paperFiber * 0.8f);

                Vector2 p = new Vector2(x, y);

                // Circle outline
                float dC = Vector2.Distance(p, cCenter);
                bool inC = dC >= r - 9f && dC <= r + 9f;

                // Triangle outline
                float tx = x - width * 0.5f;
                float ty = y - (cy - r * 0.25f);
                float halfBase = r * 1.15f;
                float triHeight = r * 2f;
                bool inT = false;
                if (ty >= -r * 0.85f && ty <= r * 1.15f)
                {
                    float expHalf = (1f - (ty + r * 0.85f) / triHeight) * halfBase;
                    float dist = Mathf.Abs(Mathf.Abs(tx) - expHalf);
                    bool onBottom = Mathf.Abs(ty - (-r * 0.85f)) < 9f && Mathf.Abs(tx) <= halfBase;
                    if (dist < 8f || onBottom) inT = true;
                }

                // Square outline
                float sx = Mathf.Abs(x - sCenter.x);
                float sy = Mathf.Abs(y - sCenter.y);
                bool inS = (sx <= r && sy <= r) && (sx >= r - 15f || sy >= r - 15f);

                if (inC || inT || inS)
                {
                    // Debossed gold/dark symbol
                    col = Color.Lerp(debossColor, goldShimmer, 0.25f);
                }

                pixels[y * width + x] = col;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateCardBack(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color kraftBase = new Color(0.76f, 0.64f, 0.49f);
        Color printDark = new Color(0.18f, 0.14f, 0.1f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float paperFiber = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.05f - 0.025f;
                Color col = kraftBase + new Color(paperFiber, paperFiber, paperFiber * 0.8f);

                // Phone number bar (010-034-1988)
                bool isPhoneBar = (y > height * 0.42f && y < height * 0.58f && x > width * 0.18f && x < width * 0.82f);
                // Barcode lines at bottom
                bool isBarcode = (y > height * 0.18f && y < height * 0.32f && x > width * 0.25f && x < width * 0.75f && (x % 6 < 3));

                if (isPhoneBar && (x % 14 > 3))
                {
                    col = printDark;
                }
                else if (isBarcode)
                {
                    col = printDark;
                }

                pixels[y * width + x] = col;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateCash(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color billGreen = new Color(0.68f, 0.78f, 0.62f);
        Color bandWhite = new Color(0.95f, 0.95f, 0.92f);
        Color borderGreen = new Color(0.35f, 0.55f, 0.38f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isBand = (x > width * 0.42f && x < width * 0.58f);
                bool isBorder = (x < 5 || x > width - 6 || y < 5 || y > height - 6);
                if (isBand) pixels[y * width + x] = bandWhite;
                else if (isBorder) pixels[y * width + x] = borderGreen;
                else pixels[y * width + x] = billGreen;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateBriefcaseLeather(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color leatherBlack = new Color(0.08f, 0.08f, 0.09f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.4f, y * 0.4f) * 0.05f;
                pixels[y * width + x] = leatherBlack + new Color(noise, noise, noise);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateSuitWool(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        Color darkCharcoal = new Color(0.12f, 0.13f, 0.16f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float weave = ((x % 2 == 0) ^ (y % 2 == 0)) ? 0.02f : -0.02f;
                pixels[y * width + x] = darkCharcoal + new Color(weave, weave, weave);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }

    private static Texture2D CreateRainStreak(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            float alpha = Mathf.Sin((y / (float)height) * Mathf.PI);
            for (int x = 0; x < width; x++)
            {
                float dx = Mathf.Abs(x - width * 0.5f) / (width * 0.5f);
                float xAlpha = Mathf.Clamp01(1f - dx * 2f);
                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha * xAlpha * 0.7f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(true, false);
        return tex;
    }
}

