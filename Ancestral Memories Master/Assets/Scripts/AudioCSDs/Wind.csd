<Cabbage>
form caption("Wind Generator"), size(400, 800), colour(25, 25, 25), pluginId("wind")
label bounds(10, 10, 380, 20), text("Wind Generator"), fontColour(255, 255, 255), font("default", 16)
button bounds(160, 40, 80, 25), channel("ONOFF"), text("OFF", "ON"), latched(1)
hslider bounds(10, 80, 380, 40), channel("WindStrength"), range(0, 1, 1, 1, 0.01), text("Wind Strength"), trackerColour("blue")
hslider bounds(10, 130, 380, 40), channel("WindWhistle"), range(0, 1, 1, 1, 0.01), text("Wind Whistle"), trackerColour("blue")
hslider bounds(10, 180, 380, 40), channel("ResonanceStrength"), range(0, 1, 0.5, 1, 0.01), text("Resonance Strength"), trackerColour("blue")
hslider bounds(10, 230, 380, 40), channel("WindRumble"), range(0, 1, 1, 1, 0.01), text("Wind Rumble"), trackerColour("blue")
hslider bounds(10, 280, 380, 40), channel("LFO"), range(0, 1, 0.5, 1, 0.01), text("LFO Speed"), trackerColour("blue")
hslider bounds(10, 330, 380, 40), channel("LFODepth"), range(0, 1, 0.5, 1, 0.01), text("LFO Depth"), trackerColour("blue")
hslider bounds(10, 380, 380, 40), channel("WindRandomness"), range(0, 1, 1, 1, 0.01), text("Wind Randomness"), trackerColour("blue")
; Amp ADSR
groupbox bounds(10, 430, 380, 165), text("Amp ADSR"), outlineThickness(2)
hslider bounds(20, 460, 350, 40), channel("AmpAttack"), range(0.05, 5, 5, 1, 0.01), text("Attack"), trackerColour("blue")
hslider bounds(20, 490, 350, 40), channel("AmpSustain"), range(0.2, 1, 1, 1, 0.01), text("Sustain"), trackerColour("blue")
hslider bounds(20, 520, 350, 40), channel("AmpDecay"), range(0.2, 1, 1, 1, 0.01), text("Decay"), trackerColour("blue")
hslider bounds(20, 550, 350, 40), channel("AmpRelease"), range(0.05, 5, 5, 1, 0.01), text("Release"), trackerColour("blue")

signaldisplay bounds(10, 610, 380, 180), colour("green") displayType("waveform"), backgroundColour(0, 0, 0), zoom(-1), signalVariable("aOut"), channel("display")
</Cabbage>

<CsoundSynthesizer>
<CsOptions>
-n -d -+rtmidi=NULL -M0 -m0d --displays
</CsOptions>
<CsInstruments>

; Global variables
ksmps = 512
nchnls = 2
0dbfs = 1

instr DetectTriggers

    iAmpRelease chnget "AmpRelease"
    
    kToggleWind chnget "ONOFF"
    kTrigWind changed kToggleWind 
    
    if kTrigWind == 1 then
        if kToggleWind == 1 then
            event "i", "Wind", 0, -1 ; Start instr 1 indefinitely
        elseif kToggleWind == 0 then
             turnoff2 "Wind", 0, iAmpRelease ; Stop the XYPad
        endif
    endif
    
endin

instr Wind

; Retrieve control values
kOnOff chnget "ONOFF"
kWindStrength chnget "WindStrength"
kWindWhistle chnget "WindWhistle"
kResonanceStrength chnget "ResonanceStrength"
kWindRumble chnget "WindRumble"
kLFOSpeed chnget "LFO"
kLFODepth chnget "LFODepth"
kWindRandomness chnget "WindRandomness"

iAmpAttack chnget "AmpAttack"
iAmpDecay chnget "AmpDecay"
iAmpSustain chnget "AmpSustain"
iAmpRelease chnget "AmpRelease"

aAmpEnv madsr iAmpAttack, iAmpDecay, iAmpSustain, iAmpRelease  
    
; ========== LFO ==========

; Adjust the LFO speed modulation based on WindRandomness
kSpeedModRange = 0.5 + (kWindRandomness * 0.5) ; Scale from min (0.5) to max (1.0)
kSpeedFluctuation = randi(-kSpeedModRange, kSpeedModRange, 10) ; Fluctuation rate based on randomness
kLFOSpeedModulated = kLFOSpeed + (kLFOSpeed * kSpeedFluctuation)

; Adjust the LFO depth modulation based on WindRandomness
kDepthModRange = 0.5 + (kWindRandomness * 0.5) ; Scale from min (0.5) to max (1.0)
kDepthFluctuation = randi(-kDepthModRange, kDepthModRange, 10) ; Apply fluctuation to depth
kLFODepthModulated = kLFODepth + (kLFODepth * kDepthFluctuation)

; Ensure the modulated values stay within bounds
kLFOSpeedModulated = max(min(kLFOSpeedModulated, 20), 0.01) ; Speed
kLFODepthModulated = max(min(kLFODepthModulated, 1), 0.1) ; Depth

; Generate LFO + modulate with calculated speed and depth
kLFO lfo 1, kLFOSpeedModulated
kLFO = kLFO * kLFODepthModulated

; ========== RUMBLE ==========

; Noise source for rumble 
aRumbleNoise pinker  ; Generate pink noise as the basis for the rumble sound.

; High-pass filter to remove lowest frequencies.
aRumbleHPF butterhp aRumbleNoise, 20

; Modulate the LPF cutoff frequency using the LFO.
kRumbleLPFModDepth = 100  ; Strength of LPF cutoff variation.
kRumbleLPFBaseCutoff = 500  ; Base cutoff frequency for the LP filter.
kRumbleLPFVariedCutoff = kRumbleLPFBaseCutoff + (kLFO * kRumbleLPFModDepth * kLFODepthModulated)

; Apply the modulated low-pass filter to shape the noise into a more dynamic rumble.
aRumbleLPF butterlp aRumbleHPF, kRumbleLPFVariedCutoff

; Attenuate the amplitude of the rumble based on the 'Wind Rumble' control for volume control.
aRumbleAttenuated = aRumbleLPF * kWindRumble


; ========== WHISTLE ==========

; Noise source for whistle 
aWhistleNoise pinker

; Initial conditions for high-pass and low-pass filters
iWhistleHP = 500
iWhistleHPfactor = 50
iWhistleLP = 18000

; High-pass filter to remove low frequencies based on Wind Whistle parameter
kWhistleHPFFreq = iWhistleHP + (1 - kWindWhistle) * iWhistleHPfactor
aWhistleHPF butterhp aWhistleNoise, kWhistleHPFFreq

; Low-pass filter to remove high frequencies, setting an upper limit
kWhistleLPFFreq = iWhistleLP
aWhistleLPF butterlp aWhistleHPF, kWhistleLPFFreq

; LFO-modulated randomness to determine center frequency for resonant peaks
kCenterFreq = (kLFO * 100) + iWhistleHP / 2

; Resonant peaks modulated more by Wind Strength
aRes1 = reson(aWhistleLPF, kCenterFreq, 100) * (kWindStrength * kWindWhistle)
aRes2 = reson(aWhistleLPF, kCenterFreq * 1.1, 100) * (kWindStrength * kWindWhistle)
aRes3 = reson(aWhistleLPF, kCenterFreq * 0.9, 100) * (kWindStrength * kWindWhistle)
aRes4 = reson(aWhistleLPF, kCenterFreq * 1.5, 50) * (kWindStrength * kWindWhistle)
aRes5 = reson(aWhistleLPF, kCenterFreq * 2, 50) * (kWindStrength * kWindWhistle)
aRes6 = reson(aWhistleLPF, kCenterFreq * 2.5, 50) * (kWindStrength * kWindWhistle)

; Combine all resonant peaks, multiplied by Wind Strength.
aWhistleComb = (aRes1 + aRes2 + aRes3 + aRes4 + aRes5 + aRes6) * kWindStrength * 0.002

; ========== OUTPUT ==========

; Combine rumble and whistle components
aOut = aRumbleAttenuated + aWhistleComb ; Make sure to include aWhistleBPF in the mix if needed

; Control overall amplitude
aOut *= aAmpEnv * kWindStrength  * 0.2

outs aOut, aOut  ; Output the audio signal

display aOut, .1, 1

endin

</CsInstruments>
<CsScore>
; Always run the DetectTriggers instrument to monitor the ON/OFF state
i "DetectTriggers" 0 z
</CsScore>
</CsoundSynthesizer>
