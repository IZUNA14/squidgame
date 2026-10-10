using UnityEngine;

public static class SquidGameAudio
{
    private static AudioClip slapClip;
    private static AudioClip ddakjiSlamClip;
    private static AudioClip ddakjiFlipClip;
    private static AudioClip cashClip;
    private static AudioClip briefcaseClip;
    private static AudioClip themeMotifClip;
    private static AudioClip rainClip;
    private static AudioClip footstepClip;
    private static AudioClip subwayBellClip;
    private static AudioClip groanClip;

    public static AudioClip GetSlap() => slapClip ??= CreateSlapSound();
    public static AudioClip GetDdakjiSlam() => ddakjiSlamClip ??= CreateDdakjiSlamSound();
    public static AudioClip GetDdakjiFlip() => ddakjiFlipClip ??= CreateDdakjiFlipSound();
    public static AudioClip GetCash() => cashClip ??= CreateCashSound();
    public static AudioClip GetBriefcase() => briefcaseClip ??= CreateBriefcaseSound();
    public static AudioClip GetThemeMotif() => themeMotifClip ??= CreateSquidThemeMotif();
    public static AudioClip GetRain() => rainClip ??= CreateRainAmbience();
    public static AudioClip GetFootstep() => footstepClip ??= CreateFootstepSound();
    public static AudioClip GetSubwayBell() => subwayBellClip ??= CreateSubwayBell();
    public static AudioClip GetGroan() => groanClip ??= CreateGroanSound();

    private static AudioClip CreateSlapSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.45f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;

            // Sharp high-frequency transient skin crack (0 - 0.05s)
            float crack = (Random.value * 2f - 1f) * Mathf.Exp(-t * 85f);

            // Flesh impact mid thump (~180Hz decaying rapidly)
            float thump = Mathf.Sin(2f * Mathf.PI * 180f * t) * Mathf.Exp(-t * 28f) * 0.85f;

            // Low end body punch (~75Hz bass resonance)
            float punch = Mathf.Sin(2f * Mathf.PI * 75f * t) * Mathf.Exp(-t * 16f) * 0.7f;

            // Ear ringing / sting (~2400Hz subtle ring)
            float ring = Mathf.Sin(2f * Mathf.PI * 2400f * t) * Mathf.Exp(-t * 12f) * 0.15f;

            samples[i] = Mathf.Clamp(crack * 1.2f + thump + punch + ring, -1f, 1f);
        }

        var clip = AudioClip.Create("Slap_FX", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateDdakjiSlamSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.35f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            // Thick cardboard paper crack (sharp bandpassed pop)
            float crack = (Random.value * 2f - 1f) * Mathf.Exp(-t * 90f) * 0.9f;
            // Tile floor impact thud (~140Hz decaying)
            float thud = Mathf.Sin(2f * Mathf.PI * 140f * t) * Mathf.Exp(-t * 32f) * 0.75f;

            samples[i] = Mathf.Clamp(crack + thud, -1f, 1f);
        }

        var clip = AudioClip.Create("Ddakji_Slam", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateDdakjiFlipSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.5f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            // Initial lift snap + air flutter whoosh + landing slap
            float whoosh = Mathf.Sin(2f * Mathf.PI * (200f + 300f * Mathf.Sin(t * 20f)) * t) * Mathf.Exp(-t * 6f) * 0.3f;
            float landing = 0f;
            if (t > 0.22f)
            {
                float lt = t - 0.22f;
                landing = (Random.value * 2f - 1f) * Mathf.Exp(-lt * 80f) * 0.8f +
                          Mathf.Sin(2f * Mathf.PI * 160f * lt) * Mathf.Exp(-lt * 30f) * 0.6f;
            }

            samples[i] = Mathf.Clamp(whoosh + landing, -1f, 1f);
        }

        var clip = AudioClip.Create("Ddakji_Flip", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateCashSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.4f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            // Multiple rapid paper flickers
            float flickers = 0f;
            for (int f = 0; f < 5; f++)
            {
                float ft = t - f * 0.05f;
                if (ft > 0f)
                {
                    flickers += (Random.value * 2f - 1f) * Mathf.Exp(-ft * 70f) * 0.25f;
                }
            }
            samples[i] = Mathf.Clamp(flickers, -1f, 1f);
        }

        var clip = AudioClip.Create("Cash_Flick", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateBriefcaseSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.35f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            // Twin brass latch clicks
            float click1 = Mathf.Sin(2f * Mathf.PI * 1800f * t) * Mathf.Exp(-t * 90f) * 0.7f;
            float click2 = 0f;
            if (t > 0.09f)
            {
                float t2 = t - 0.09f;
                click2 = Mathf.Sin(2f * Mathf.PI * 2200f * t2) * Mathf.Exp(-t2 * 90f) * 0.8f;
            }
            samples[i] = Mathf.Clamp(click1 + click2, -1f, 1f);
        }

        var clip = AudioClip.Create("Briefcase_Latch", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateSquidThemeMotif()
    {
        // Iconic chilling 4-note Squid Game marimba/flute motif
        int sampleRate = 44100;
        float noteDur = 0.45f;
        float[] notes = new float[] { 329.63f, 392.00f, 440.00f, 493.88f, 440.00f }; // E4, G4, A4, B4, A4
        int length = (int)(sampleRate * noteDur * notes.Length);
        float[] samples = new float[length];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq = notes[n];
            int startIdx = (int)(n * noteDur * sampleRate);
            int endIdx = (int)((n + 1) * noteDur * sampleRate);

            for (int i = startIdx; i < endIdx && i < length; i++)
            {
                float t = (float)(i - startIdx) / sampleRate;
                float env = Mathf.Sin(Mathf.Clamp01(t / noteDur) * Mathf.PI);

                // Flute / bell harmonic spectrum
                float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.7f +
                             Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.25f +
                             Mathf.Sin(2f * Mathf.PI * freq * 3f * t) * 0.1f;

                samples[i] = tone * env * 0.6f;
            }
        }

        var clip = AudioClip.Create("Squid_Theme_Motif", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateRainAmbience()
    {
        int sampleRate = 44100;
        int length = sampleRate * 3; // 3-second seamless loop
        float[] samples = new float[length];
        float lastVal = 0f;

        for (int i = 0; i < length; i++)
        {
            // Low-pass filtered pink noise for gentle continuous rain
            float white = Random.value * 2f - 1f;
            lastVal = Mathf.Lerp(lastVal, white, 0.12f);
            float drip = (Random.value < 0.003f) ? (Random.value * 2f - 1f) * 0.3f : 0f;
            samples[i] = (lastVal * 0.22f + drip);
        }

        var clip = AudioClip.Create("Rain_Ambience", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateFootstepSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.18f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            float tap = (Random.value * 2f - 1f) * Mathf.Exp(-t * 60f) * 0.35f;
            float squelch = Mathf.Sin(2f * Mathf.PI * 340f * t) * Mathf.Exp(-t * 40f) * 0.2f;
            samples[i] = tap + squelch;
        }

        var clip = AudioClip.Create("Footstep_Wet", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateSubwayBell()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.9f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            float chime1 = Mathf.Sin(2f * Mathf.PI * 880f * t) * Mathf.Exp(-t * 4f) * 0.5f;
            float chime2 = 0f;
            if (t > 0.25f)
            {
                float t2 = t - 0.25f;
                chime2 = Mathf.Sin(2f * Mathf.PI * 1174.66f * t2) * Mathf.Exp(-t2 * 4f) * 0.5f;
            }
            samples[i] = chime1 + chime2;
        }

        var clip = AudioClip.Create("Subway_Bell", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateGroanSound()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.38f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / sampleRate;
            float freq = Mathf.Lerp(160f, 95f, t / 0.38f);
            float voice = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-t * 9f) * 0.55f;
            float breath = (Random.value * 2f - 1f) * Mathf.Exp(-t * 14f) * 0.25f;
            samples[i] = voice + breath;
        }

        var clip = AudioClip.Create("Player_Groan", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}

