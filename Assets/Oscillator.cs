using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public double frequency = 440.0;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000.0;

    public float gain;
    public float volume = 0.1f;

    static string[] tones = { "C", "C#", "D", "D#", "E", "F",
         "F#", "G", "G#", "A", "A#", "B" };
    static double[] frequencies = { 261.63, 277.18, 293.66, 311.13, 329.63, 349.23,
         369.99, 392.00, 415.30, 440.00, 466.16, 493.88 };

    public static double[] current_scale;
    public static int scale_index;

    static int FindTone(string key)
    {
        for (int i = 0; i < tones.GetLength(0); i++)
        {
            if (key == tones[i])
                return i;
        }
        return -1;
    }

    static double GetFrequency(string key)
    {
        return frequencies[FindTone(key)];
    }

    static double[] ComputeScale(string key, int[] steps)
    {
        double[] scale = new double[steps.Length];

        int tonePosition = 0;
        int startTone;

        startTone = FindTone(key);


        if (startTone < 0)
            return null;
        if (steps.GetLength(0) != scale.GetLength(0))
            return null;

        tonePosition = startTone;
        for (int i = 0; i < steps.GetLength(0); i++)
        {
            scale[i] = frequencies[tonePosition % tones.GetLength(0)];
            tonePosition += steps[i];
        }

        return scale;
    }

    static void SetScale(double[] scale)
    {
        current_scale = scale;
        scale_index = 0;
    }

    void Start()
    {
        Debug.Log("START");

        int[] major = { 2, 2, 1, 2, 2, 2, 1 };
        int[] minor = { 2, 1, 2, 2, 1, 2, 2 };

        int[] test = { 2, 2, 1, 2, 2, 2, 1, -1, -2, -2, -2, -1, -2, -2 };
        int[] test2 = { 2, 2, -1, -2, 2, 2, -5, -1 };

        double[] scale = ComputeScale("A", test2);

        Debug.Log(string.Join(", ", scale));

        SetScale(scale);
    }

    public void PlayScale(string key, int[] pattern)
    {
        double[] scale = ComputeScale(key, pattern);
        SetScale(scale);

        gain = 0;
    }

    public void PlayNote()
    {
        // Play next note in current scale
        gain = volume;

        Debug.Log("Play note!");

        frequency = current_scale[scale_index];
        scale_index = (scale_index + 1) % current_scale.Length;
    }

    public void Stop()
    {
        gain = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNote();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gain = 0;
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i+= channels)
        {
            phase += increment;
            data[i] = (float)(gain * Mathf.Sin((float)phase));
            data[i] += (float)(gain * (double)Mathf.PingPong((float)phase, 1.0f));

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase = 0.0;
            }
        }
    }
}
