using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class AudioReactive : MonoBehaviour
{
    [SerializeField] private FMOD.DSP mFFT;
    [SerializeField] private LineRenderer mLineRenderer;
    [SerializeField] private float[] mFFTSpectrum;
    [SerializeField] const int WindowSize = 1024;

    void Start()
    {
        mLineRenderer = gameObject.AddComponent<LineRenderer>();
        mLineRenderer.positionCount = WindowSize;
        mLineRenderer.startWidth = mLineRenderer.endWidth = .1f;

        // Create a DSP of DSP_TYPE.FFT
        if (FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out mFFT) == FMOD.RESULT.OK)
        {
            mFFT.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
            mFFT.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WindowSize * 2);
            FMODUnity.RuntimeManager.StudioSystem.flushCommands();

            // Get the master bus (or any other bus for that matter)
            FMOD.Studio.Bus selectedBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            if (selectedBus.hasHandle())
            {
                // Get the channel group
                FMOD.ChannelGroup channelGroup;
                if (selectedBus.getChannelGroup(out channelGroup) == FMOD.RESULT.OK)
                {
                    // Add fft to the channel group
                    if (channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, mFFT) != FMOD.RESULT.OK)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add mFFT to the master channel group");
                    }
                }
                else
                {
                    Debug.LogWarningFormat("FMOD: Unable to get Channel Group from the selected bus");
                }
            }
            else
            {
                Debug.LogWarningFormat("FMOD: Unable to get the selected bus");
            }
        }
        else
        {
            Debug.LogWarningFormat("FMOD: Unable to create FMOD.DSP_TYPE.FFT");
        }
    }

    void OnDestroy()
    {
        FMOD.Studio.Bus selectedBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        if (selectedBus.hasHandle())
        {
            FMOD.ChannelGroup channelGroup;
            if (selectedBus.getChannelGroup(out channelGroup) == FMOD.RESULT.OK)
            {
                if (mFFT.hasHandle())
                {
                    channelGroup.removeDSP(mFFT);
                }
            }
        }
    }

    [SerializeField] const float WIDTH = 10.0f;
    [SerializeField] const float HEIGHT = 0.1f;

    void Update()
    {
        if (mFFT.hasHandle())
        {
            IntPtr unmanagedData;
            uint length;
            if (mFFT.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length) == FMOD.RESULT.OK)
            {
                FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
                if (fftData.numchannels > 0)
                {
                    if (mFFTSpectrum == null)
                    {
                        // Allocate the fft spectrum buffer once
                        for (int i = 0; i < fftData.numchannels; ++i)
                        {
                            mFFTSpectrum = new float[fftData.length];
                        }
                    }
                    fftData.getSpectrum(0, ref mFFTSpectrum);

                    var pos = Vector3.zero;
                    pos.x = WIDTH * -0.5f;

                    for (int i = 0; i < WindowSize; ++i)
                    {
                        pos.x += (WIDTH / WindowSize);

                        float level = lin2dB(mFFTSpectrum[i]);
                        pos.y = (80 + level) * HEIGHT;

                        mLineRenderer.SetPosition(i, pos);
                    }
                }
            }
        }
    }

    private float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }
    
}
