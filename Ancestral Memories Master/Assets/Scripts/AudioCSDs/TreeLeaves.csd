<Cabbage>
form caption("Tree Leaves") size(400, 640), colour(25, 25, 25), pluginId("leaf")
label bounds(10, 10, 380, 20), text("Tree Leaves"), fontColour(255, 255, 255), font("default", 16)
button bounds(160, 40, 80, 25) channel("ONOFF") text("OFF", "ON")
hslider bounds(10, 70, 380, 40) channel("Amplitude") range(0, 1, 0.5, 1, 0.01) text("Amplitude") trackerColour("green")
hslider bounds(10, 120, 380, 40) channel("Frequency") range(0, 1000, 80, 1, 0.01) text("Frequency") trackerColour("green")
hslider bounds(10, 170, 380, 40) channel("HighPass") range(100, 20000, 100, 1, 0.01) text("HighPass") trackerColour("green")
hslider bounds(10, 220, 380, 40) channel("LowPass") range(20, 20000, 20000, 1, 0.01) text("LowPass") trackerColour("green")
hslider bounds(10, 270, 380, 40) channel("Noise") range(0, 1, 1, 1, 0.01) text("Noise") trackerColour("green")
hslider bounds(10, 320, 380, 40) channel("Sizzle") range(0, 1, 1, 1, 0.01) text("Sizzle") trackerColour("green")
hslider bounds(10, 370, 380, 40) channel("Blossom") range(0, 1, 0, 1, 0.01) text("Blossom") trackerColour("green")
hslider bounds(10, 420, 380, 40) channel("Disturbance") range(0, 1, 0, 1, 0.01) text("Disturbance") trackerColour("green")

signaldisplay bounds(10, 470, 380, 160), colour("green") displayType("waveform"), backgroundColour(0, 0, 0), zoom(-1), signalVariable("aOut"), channel("display")
</Cabbage>


<CsoundSynthesizer>
<CsOptions>
-odac -n -d -+rtmidi=NULL -M0 -m0d --displays
</CsOptions>
<CsInstruments>

sr = 44100
ksmps = 512
nchnls = 2
0dbfs  = 1

instr DetectTriggers

    kOnOff chnget "ONOFF"
    
    if changed(kOnOff) == 1 then
        if kOnOff == 1 then
            event "i", 1, 0, -1 ; Start instr 1 indefinitely
        else
            turnoff2 1, 0, 0 ; Turn off instr 1 immediately
        endif
    endif
    
endin

instr 1

kDisturbance chnget "Disturbance"
kAmp chnget "Amplitude"
kSizzle chnget "Sizzle"
kNoise chnget "Noise"
kBlossom chnget "Blossom"

kCurveFactorFreq = 1 ; Adjust for dynamic evolution
kCurveFactorLPF = 1 ; Adjust for LPF max value quicker
kCurveFactorHPF = 0.2 ; Adjust for HPF

kFreqExp = pow(kDisturbance, kCurveFactorFreq) * 1000 ; Exponential scale from 0 to 100
kSizzleExp = pow(kDisturbance, kCurveFactorFreq) * 100 
kNoiseExp = pow(kDisturbance, kCurveFactorFreq) * 100 / 2
kBlossomExp = pow(kBlossom, kCurveFactorFreq) * 100 / 2

kLPFExp = pow(kDisturbance, kCurveFactorLPF) * 20000 ; Exponential scale from 0 to 20000, faster
kHPFExp = 20000 - pow(kDisturbance, kCurveFactorHPF) * 20000 ; Inverse exponential scale from 20000 down to 0, adjusted

chnset kFreqExp, "Frequency"
chnset kHPFExp, "HighPass"
chnset kLPFExp, "LowPass"

; NOISE

; Noise source for whistle 
aLeafNoise pinker

; Initial conditions for high-pass and low-pass filters
iLeafNoiseHP = 500
iLeafNoiseHPfactor = 2
iLeafNoiseLP = 4000

; High-pass filter to remove low frequencies based on Wind Whistle parameter
kLeafNoiseHPFFreq = iLeafNoiseHP + (1 - kNoise) * iLeafNoiseHPfactor
aLeafNoiseHPF butterhp aLeafNoise, kLeafNoiseHPFFreq

; Low-pass filter to remove high frequencies, setting an upper limit
kLeafNoiseLPFFreq = iLeafNoiseLP
aLeafNoiseLPF butterlp aLeafNoiseHPF, kLeafNoiseLPFFreq

; LFO-modulated randomness to determine center frequency for resonant peaks
kCenterFreq = 15000

; Resonant peaks modulated more by Wind Strength
aRes1 = reson(aLeafNoiseLPF, kCenterFreq, 500) * kDisturbance
aRes2 = reson(aLeafNoiseLPF, kCenterFreq * 1.1, 500) * kDisturbance
aRes3 = reson(aLeafNoiseLPF, kCenterFreq * 1.2, 100) * kDisturbance
aRes4 = reson(aLeafNoiseLPF, kCenterFreq * 1.5, 50) * kDisturbance
aRes5 = reson(aLeafNoiseLPF, kCenterFreq * 2, 50) * kDisturbance
aRes6 = reson(aLeafNoiseLPF, kCenterFreq * 2.5, 50) * kDisturbance

; Combine all resonant peaks, multiplied by Wind Strength.
aLeafNoiseComb = (aRes1 + aRes2) * kNoise * 0.02

; LEAF SIZZLE 1

kLfoSizzlingRate randi 0.15,  0.35, 0.01 ; Random LFO rate between 30 and 70 Hz for Creaking

kLfoSizzling lfo 50, kLfoSizzlingRate 
kSizzlingFreq = 16000 + kLfoSizzling 
kSizzlingBandwidth = kLfoSizzling + 200

kSizzlingAmpScaled = kAmp * 0.02 * kDisturbance
aSizzlingSrc dust2 kSizzlingAmpScaled * kSizzle, kSizzleExp * 4
aSizzling reson aSizzlingSrc, kSizzlingFreq, kSizzlingBandwidth

; LEAF SIZZLE 2

kSizzlingFreq2 = 5000 
kSizzlingBandwidth2 = 500 

aSizzlingSrc2 dust2 kSizzlingAmpScaled * kSizzle * 0.05, kSizzleExp * 4
aSizzling2 reson aSizzlingSrc2, kSizzlingFreq2, kSizzlingBandwidth2

; BLOSSOM FX

kBlossomingFreq = 17000 ; Center frequency for the new band
kBlossomingBandwidth = 300 ; Bandwidth for the new band

kBlossomingAmpScaled = kAmp * 0.5 * kBlossom
aBlossomingSrc dust2 kBlossomingAmpScaled * kBlossom * 0.1, kBlossomExp * 4
aBlossoming reson aBlossomingSrc, kBlossomingFreq, kBlossomingBandwidth

; Combine both sizzling sounds with the leaf noise
aOut = aBlossoming + aSizzling + aSizzling2 + aLeafNoiseComb

outs aOut, aOut


display aOut, .1, 1

endin






</CsInstruments>
<CsScore>
i"DetectTriggers" 0 z

</CsScore>
</CsoundSynthesizer>
