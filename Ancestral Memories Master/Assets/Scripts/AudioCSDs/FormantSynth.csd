<Cabbage>
form caption("FormantSynth"), size(765, 650), colour(129, 129, 129), pluginId("talk")

label bounds(10, 10, 200, 20), text("Formant Synthesizer"), fontColour(255, 255, 255, 255), font("default", 16)

groupbox bounds(5, 85, 80, 400), text("Master"), colour(200, 200, 200)
vslider bounds(20, 105, 50, 375), channel("kMasterVolume"), range(0, 1, 0.5), text("Vol.") trackerColour(0, 0, 255, 255)
    
button bounds(155, 80, 90, 50), channel("toggleDrone"), identChannel("toggleDroneID"), text("Loop: OFF"), latched(1), colour(255, 0, 0), fontColour(255, 255, 255)
button bounds(90, 80, 60, 50), channel("toggleSpeak"), identChannel("toggleSpeakID"), text("Speak"), latched(0), colour(255, 0, 0), fontColour(255, 255, 255)

rslider bounds(240, 70, 60, 60), channel("tune"), range(-24, 24, 0), text("Tune"), trackerColour(0, 0, 255, 255)
rslider bounds(300, 70, 60, 60), channel("fineTune"), range(-1, 1, 0), text("Fine-Tune"), trackerColour(0, 0, 255, 255)  
rslider bounds(360, 70, 60, 60), channel("lfoFrequency"), range(0.1, 10, 3), text("LFO Frequency"), trackerColour(0, 0, 255, 255)
rslider bounds(420, 70, 60, 60), channel("lfoDepth"), range(0, 1, 0.1), text("LFO Depth"), trackerColour(0, 0, 255, 255)
rslider bounds(480, 70, 60, 60), channel("lfoWaveform"), range(0, 1, 0), text("LFO Waveform"), trackerColour(0, 0, 255, 255)
hslider bounds(90, 135, 220, 20), channel("waveformShapeSlider"), range(0, 1, 0.5), text("Waveform Shape"), trackerColour(0, 0, 255, 255)
hslider bounds(90, 155, 220, 20), channel("waveformSkew"), range(0, 1, 0.5), text("Waveform Skew"), trackerColour(0, 0, 255, 255)
hslider bounds(90, 175, 220, 20), channel("harmonicSlider"), range(0, 1, 0.8), text("Harmonics")

xypad bounds(325, 190, 200, 160), channel("xCoordinate", "yCoordinate") 
hslider bounds(325, 355, 200, 20), channel("xCoord"), range(0, 1, 0.5, 1) 
vslider bounds(530, 190, 20, 160), channel("yCoord"), range(0, 1, 0.5, 1) 


button bounds(325, 140, 80, 25), channel("autoVowel"), identChannel("autoVowelID"), text("AutoVowel"), colour(100, 100, 255), fontColour(255, 255, 255)
button bounds(415, 140, 80, 25), channel("splatter"), identChannel("splatterID"), text("Splatter"), colour(255, 100, 100), fontColour(255, 255, 255)
button bounds(415, 170, 80, 15), channel("hardSplat"), identChannel("hardSplatID"), text("HardSplat"), colour(100, 255, 100), fontColour(255, 255, 255)
button bounds(575, 345, 150, 25), channel("toggleFilter"), identChannel("toggleFilterID"), text("Filter"), colour(100, 100, 255), fontColour(255, 255, 255)


rslider bounds(495, 140, 54, 54) range(0, 1, 0.5) channel("splatSpeed") text("SplatSpeed")


hslider bounds(325, 370, 210, 25), channel("autoSpeed"), range(-1, 1, 0), text("AutoSpeed") trackerColour(0, 0, 255, 255)
hslider bounds(325, 395, 210, 25), channel("autoRadius"), range(-1, 1, 0), text("AutoRadius") trackerColour(0, 0, 255, 255)
hslider bounds(325, 420, 210, 25), channel("autoRandom"), range(-1, 1, 0), text("AutoRandom") trackerColour(0, 0, 255, 255)
hslider bounds(325, 445, 210, 25), channel("autoRandomRadius"), range(-1, 1, 0), text("AutoRandomRadius") trackerColour(0, 0, 255, 255)

plant("Bandwidth") {
    groupbox bounds(325, 475, 210, 125), text("Bandwidth"), colour(200, 200, 200)
    hslider bounds(325, 495, 210, 25), channel("kBW1"), range(0.001, 500, 60), text("BW1") trackerColour(0, 0, 255, 255)
    hslider bounds(325, 515, 210, 25), channel("kBW2"), range(0.001, 500, 90), text("BW2") trackerColour(0, 0, 255, 255)
    hslider bounds(325, 535, 210, 25), channel("kBW3"), range(0.001, 500, 200), text("BW3") trackerColour(0, 0, 255, 255)
    hslider bounds(325, 555, 210, 25), channel("kBW4"), range(0.001, 500, 300), text("BW4") trackerColour(0, 0, 255, 255)
    hslider bounds(325, 575, 210, 25), channel("kBW5"), range(0.001, 500, 400), text("BW5") trackerColour(0, 0, 255, 255)
}



plant("NoiseAmount") {
    hslider bounds(550, 100, 200, 20), channel("noiseAmountSlider"), range(0, 1, 0.1), text("Breathiness") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 130, 200, 20), channel("toneVolumeSlider"), range(0, 1, 0.5), text("Tone") trackerColour(0, 0, 255, 255)
}

;hslider bounds(100, 350, 200, 20), channel("blendFactor"), range(0, 1, 0.5), text("Blend"), trackerColour(150, 150, 150, 255) textColour(255, 255, 255, 255)
;hslider bounds(100, 370, 200, 20), channel("dampingFactor"), range(0, 1, 0.5), text("Damping"), trackerColour(150, 150, 150, 255) textColour(255, 255, 255, 255)

plant("Formants") {
    groupbox bounds(90, 365, 220, 190), text("Formants"), colour(200, 200, 200)
    
    vslider bounds(106, 390, 25, 150), channel("kFreq1"), range(0.1, 4000, 300, 1, 0.1), text("F1") trackerColour(0, 67, 210, 255) textColour(255, 255, 255, 255)
    vslider bounds(136, 390, 25, 150), channel("kFreq2"), range(0.1, 4000, 870, 1, 0.1), text("F2") trackerColour(0, 0, 255, 255) textColour(255, 255, 255, 255)
    vslider bounds(166, 390, 25, 150), channel("kFreq3"), range(0.1, 4000, 2250, 1, 0.1), text("F3") trackerColour(0, 0, 255, 255) textColour(255, 255, 255, 255)
    vslider bounds(196, 390, 25, 150), channel("kFreq4"), range(0.1, 4000, 3500, 1, 0.1), text("F4") trackerColour(0, 0, 255, 255) textColour(255, 255, 255, 255)
    vslider bounds(226, 390, 25, 150), channel("kFreq5"), range(0.1, 4000, 4500, 1, 0.1), text("F5") trackerColour(0, 0, 255, 255) textColour(255, 255, 255, 255)
}




plant("AMPADSR") {
    groupbox bounds(552, 150, 200, 145), text("AMP ENVELOPE"), colour(200, 200, 200)
    hslider bounds(550, 170, 200, 25), channel("ampAttack"), range(0.1, 1, 0.01), text("Attack") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 200, 200, 25), channel("ampDecay"), range(0.3, 1, 0.01), text("Decay") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 230, 200, 25), channel("ampSustain"), range(0, 1, 0.5), text("Sustain") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 260, 200, 25), channel("ampRelease"), range(0.1, 2, 0.5), text("Release") trackerColour(0, 0, 255, 255)
}

plant("FILTERADSR") {
    groupbox bounds(540, 375, 220, 150), text("FILTER ENVELOPE"), colour(200, 200, 200)

    hslider bounds(550, 395, 200, 25), channel("filterAttack"), range(0.001, 1, 0.5), text("Attack") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 425, 200, 25), channel("filterDecay"), range(0.001, 1, 0.5), text("Decay") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 455, 200, 25), channel("filterSustain"), range(0.001, 1, 0.5), text("Sustain") trackerColour(0, 0, 255, 255)
    hslider bounds(550, 485, 200, 25), channel("filterRelease"), range(0.001, 1, 0.5), text("Release") trackerColour(0, 0, 255, 255)
}

</Cabbage>

<CsoundSynthesizer>
<CsOptions>
-n -odac --displays -d -m0d -+rtmidi=NULL -M0 --midi-key-cps=4 --midi-velocity-amp=5
</CsOptions>
<CsInstruments>

ksmps = 32
nchnls = 1
0dbfs = 1

giDuration = 0.01

instr UpdateGUI

    ; Drone ON/OFF Toggle
    
    kToggleSpeak chnget "toggleSpeak"
    kTrigSpeak changed kToggleSpeak 
    
    if kTrigSpeak == 1 then
        if kToggleSpeak == 1 then
            event "i", "LoopSound", 0, giDuration ; Start the LoopSound
            event "i", "InitXYPad", 0, 1
            turnoff2 "XYPad", 0, 0.01 ; Stop the XYPad
            event "i", "SetOneShotColourON", 0, 0
            event "i", "SetOneShotColourOFF", 0.1, 0.1
        elseif kToggleSpeak == 0 then
            turnoff2 "LoopSound", 0, 0.01 ; Stop the LoopSound 
        endif
    endif
        
    kToggleDrone chnget "toggleDrone"
    kTrigDrone changed kToggleDrone 

    if kTrigDrone == 1 then
        if kToggleDrone == 1 then
            chnset "text(\"Drone: ON\")", "toggleDroneID"
            event "i", "LoopSound", 0, -1 ; Start the LoopSound
            event "i", "XYPad", 0, -1 ; Start the XYPad
        elseif kToggleDrone == 0 then
            chnset "text(\"Drone: OFF\")", "toggleDroneID"
            turnoff2 "LoopSound", 0, 0.01 ; Stop the LoopSound 
            event "i", "InitXYPad", 0, 1
            turnoff2 "XYPad", 0, 0.01 ; Stop the XYPad
        endif
    endif
    
    kToggleFilter chnget "toggleFilter"
    kTrigFilter changed kToggleFilter 

    if kTrigFilter == 1 then
        if kToggleFilter == 1 then
            chnset "text(\"Filter: ON\")", "toggleFilterID"
        elseif kToggleFilter == 0 then
            chnset "text(\"Filter: OFF\")", "toggleFilterID"
        endif
    endif
    
    ; Button Colouring 
    
    iNumButtons = 5
    SChannels[] init iNumButtons
    SIdentChannels[] init iNumButtons
    gkPrevButtonStates[] init iNumButtons
    kButtonStates[] init iNumButtons

    SChannels[0] = "toggleDrone"
    SChannels[1] = "autoVowel"
    SChannels[2] = "splatter"
    SChannels[3] = "hardSplat"
    SChannels[4] = "toggleEnvelope"

    SIdentChannels[0] = "toggleDroneID"
    SIdentChannels[1] = "autoVowelID"
    SIdentChannels[2] = "splatterID"
    SIdentChannels[3] = "hardSplatID"
    SIdentChannels[4] = "toggleEnvelopeID"
    
    kIndexInit = 0
    until kIndexInit == iNumButtons do
        gkPrevButtonStates[kIndexInit] = -1
        kIndexInit += 1
    od

    kIndex = 0
    until kIndex == iNumButtons do
        kButtonStates[kIndex] chnget SChannels[kIndex] ; Fetch button state

        if kButtonStates[kIndex] != gkPrevButtonStates[kIndex] then ; Compare with previous state
            if kButtonStates[kIndex] == 1 then
                if strlen(SIdentChannels[kIndex]) > 0 then
                    chnset sprintfk("colour(0,255,0)", SIdentChannels[kIndex]), SIdentChannels[kIndex] ; Set colour (ON)
                endif
            else
                if strlen(SIdentChannels[kIndex]) > 0 then
                    chnset sprintfk("colour(0,0,0)", SIdentChannels[kIndex]), SIdentChannels[kIndex] ; Set colour (OFF)
                endif
            endif
            
            gkPrevButtonStates[kIndex] = kButtonStates[kIndex] ; Update state
        endif
        kIndex += 1
    od

endin

instr SetOneShotColourOFF
    chnset "colour(0, 0, 0)", "toggleSpeakID"
endin

instr SetOneShotColourON
    chnset "colour(0, 255, 0)", "toggleSpeakID"
endin

instr LoopSound
    
    ; === AMP ADSR ENV ===
    
    iAmpAttack chnget "ampAttack"
    iAmpDecay chnget "ampDecay"
    iAmpSustain chnget "ampSustain"
    iAmpRelease chnget "ampRelease"

    aEnv madsr iAmpAttack, iAmpDecay, iAmpSustain, iAmpRelease 
    
    ; === FREQ ADSR ENV ===
    
    kFilterToggle chnget "toggleFilter"

    iFilterAttack chnget "filterAttack"
    iFilterDecay chnget "filterDecay"
    iFilterSustain chnget "filterSustain"
    iFilterRelease chnget "filterRelease"
    
    if kFilterToggle == 1 then
        aFilterEnv madsr iFilterAttack, iFilterDecay, iFilterSustain, iFilterRelease ; Filter Envelope
    endif
    
    ; === TUNING CONTROL ===
    
    kTune chnget "tune"
    kFineTune chnget "fineTune"
    kTune = lag(kTune, 1)
    kOrigLFOFreq chnget "lfoFrequency"
    kOrigLFODepth chnget "lfoDepth"
    
    ; Add randomness to LFO
    
    kLFOFreq = kOrigLFOFreq 
    kLFODepth = kOrigLFODepth 

    kLFOWaveform chnget "lfoWaveform"

    chnset kLFOFreq, "lfoFrequency"
    chnset kLFODepth, "lfoDepth"
    
    ; Generate LFO waveform (WIP)
    
    if kLFOWaveform == 0 then
        kLFO oscili kLFODepth, kLFOFreq
    elseif kLFOWaveform == 1 then
        kLFO oscili kLFODepth, kLFOFreq, 10
    elseif kLFOWaveform == 2 then
        kLFOSignal phasor kLFOFreq
        kLFO = (kLFOSignal > 0.5) ? kLFODepth : -kLFODepth
    elseif kLFOWaveform == 3 then
        kLFOSignal phasor kLFOFreq
        kLFO = (kLFOSignal * 2 - 1) * kLFODepth
    endif

    ; === TONE SIGNAL GENERATION ===
    
    kMasterVolume chnget "kMasterVolume"
    kWaveformShape chnget "waveformShapeSlider"
    kWaveformSkew chnget "waveformSkew"
    kHarmonics chnget "harmonicSlider"

    kDrive = 1 + kHarmonics^2 * 5  ; Saturation formula

    kTuneFreq = 220 * pow(2, (kTune + kLFO)/12 + kFineTune/100)
    asigTuneFreq = a(kTuneFreq)
    aWaveformShape = a(kWaveformShape)
    aWaveformSkew = a(kWaveformSkew)

    kModIndex = 3 + kHarmonics^2 * 5 

    ; Generate waveform
    aModulator oscil kModIndex, asigTuneFreq * 2
    aSquin squinewave asigTuneFreq + aModulator, aWaveformShape, aWaveformSkew, 0, 80 
    
    aSaturation = tanh(aSquin * kDrive) 
    
    aLoopSig = aSaturation * aEnv * 0.01
    aLoopSig = butterhp(aLoopSig, 200)

    ; === FILTER SETTINGS ===
    
    ; Formant Frequencies
    kFreq1 chnget "kFreq1"
    kFreq2 chnget "kFreq2"
    kFreq3 chnget "kFreq3"
    kFreq4 chnget "kFreq4"
    kFreq5 chnget "kFreq5"

    ; Formants Modulation depth (0-1)
    kModDepth1 = 0.5  
    kModDepth2 = 0.5  
    kModDepth3 = 0.5  
    kModDepth4 = 0.5  
    kModDepth5 = 0.5  
    
    ; Apply Freq Filter to Formants
    
    if kFilterToggle == 1 then ; Freq Filter: ON            
        kFreqShift1 = kFreq1 * kModDepth1 * aFilterEnv
        kFreqShift2 = kFreq2 * kModDepth2 * aFilterEnv
        kFreqShift3 = kFreq3 * kModDepth3 * aFilterEnv
        kFreqShift4 = kFreq4 * kModDepth4 * aFilterEnv
        kFreqShift5 = kFreq5 * kModDepth5 * aFilterEnv
    else ; Freq Filter: OFF
        kFreqShift1 = kFreq1 * kModDepth1
        kFreqShift2 = kFreq2 * kModDepth2
        kFreqShift3 = kFreq3 * kModDepth3
        kFreqShift4 = kFreq4 * kModDepth4
        kFreqShift5 = kFreq5 * kModDepth5 
    endif

    kFreq1Modulated = kFreq1 + kFreqShift1
    kFreq2Modulated = kFreq2 + kFreqShift2
    kFreq3Modulated = kFreq3 + kFreqShift3
    kFreq4Modulated = kFreq4 + kFreqShift4
    kFreq5Modulated = kFreq5 + kFreqShift5
    
    kFreq1 = kFreq1Modulated * pow(2, kTune/12)
    kFreq2 = kFreq2Modulated * pow(2, kTune/12)
    kFreq3 = kFreq3Modulated * pow(2, kTune/12)
    kFreq4 = kFreq4Modulated * pow(2, kTune/12)
    kFreq5 = kFreq5Modulated * pow(2, kTune/12)
    
    ; === BANDWIDTH ===
    
    kBW1 chnget "kBW1"
    kBW2 chnget "kBW2"
    kBW3 chnget "kBW3"
    kBW4 chnget "kBW4"
    kBW5 chnget "kBW5"
    
    kBW1 = kBW1 * pow(2, kTune/12)
    kBW2 = kBW2 * pow(2, kTune/12)
    kBW3 = kBW3 * pow(2, kTune/12)
    kBW4 = kBW4 * pow(2, kTune/12)
    kBW5 = kBW5 * pow(2, kTune/12)
    
    kFormantLFOScale = 0.05  ; LFO Strength Scaler
    
    ; Apply LFO to Formants and Bandwith
    
    kFreq1 += kFreq1 * kLFO * kFormantLFOScale
    kFreq2 += kFreq2 * kLFO * kFormantLFOScale
    kFreq3 += kFreq3 * kLFO * kFormantLFOScale
    kFreq4 += kFreq4 * kLFO * kFormantLFOScale
    kFreq5 += kFreq5 * kLFO * kFormantLFOScale
    
    kBW1 += kBW1 * kLFO * kFormantLFOScale
    kBW2 += kBW2 * kLFO * kFormantLFOScale
    kBW3 += kBW3 * kLFO * kFormantLFOScale
    kBW4 += kBW4 * kLFO * kFormantLFOScale
    kBW5 += kBW5 * kLFO * kFormantLFOScale
    
    kFreq1 = limit(kFreq1, 20, 20000)
    kFreq2 = limit(kFreq2, 20, 20000)
    kFreq3 = limit(kFreq3, 20, 20000)
    kFreq4 = limit(kFreq4, 20, 20000)
    kFreq5 = limit(kFreq5, 20, 20000)
    
    kBW1 = limit(kBW1, 0.1, 20000)
    kBW2 = limit(kBW2, 0.1, 20000)
    kBW3 = limit(kBW3, 0.1, 20000)
    kBW4 = limit(kBW4, 0.1, 20000)
    kBW5 = limit(kBW5, 0.1, 20000)
    
    ; Resonant filters
    
    a1 resonz aLoopSig, kFreq1Modulated, kBW1
    a2 resonz aLoopSig, kFreq2, kBW2
    a3 resonz aLoopSig, kFreq3, kBW3
    a4 resonz aLoopSig, kFreq4, kBW4
    a5 resonz aLoopSig, kFreq5, kBW5
    
    ; === NOISE/BREATH GENERATION ===
    
    kNoiseAmount chnget "noiseAmountSlider"
    aPink pinker
    aWhite rand 1
    aBreathNoise = aPink * 0.01 + aWhite * 0.01
    aBreathNoise *= kNoiseAmount * aEnv * (1 + 0.2 * kLFO)
    aBreathNoise = butterhp(aBreathNoise, 300)

    ; Process breath with formant filters
    aBreath1 reson aBreathNoise, kFreq1, kBW1
    aBreath2 reson aBreathNoise, kFreq2, kBW2
    aBreath3 reson aBreathNoise, kFreq3, kBW3
    aBreath4 reson aBreathNoise, kFreq4, kBW4
    aBreath5 reson aBreathNoise, kFreq5, kBW5

    ; Normalize each breath formant filter
    kNormBreath1 = 0.5
    kNormBreath2 = 0.5
    kNormBreath3 = 0.5
    kNormBreath4 = 0.5
    kNormBreath5 = 0.5
    
    aBreath = (aBreath1 * kNormBreath1 + aBreath2 * kNormBreath2 + aBreath3 * kNormBreath3 + aBreath4 * kNormBreath4 + aBreath5 * kNormBreath5)

    ; === SUMMING SIGNALS ===
    
    kToneVolume chnget "toneVolumeSlider"
    aTone = kToneVolume * (a1 + a2 + a3)
    aMix = 0.5 * aTone + 0.5 * aBreath
    aMix = limit(aMix, -1, 1)
    aMix *= kMasterVolume
    
    outs aMix

endin

instr XYPad

    ; UI Slider XY Coords
    
    kxCoord chnget "xCoord"
    kyCoord chnget "yCoord"

    ; XYPad Coords
    kx chnget "xCoordinate"
    ky chnget "yCoordinate"
    
    ; Check sliders moved
    if changed(kxCoord) == 1 then
        chnset kxCoord, "xCoordinate"
        kx = kxCoord
    endif

    if changed(kyCoord) == 1 then
        chnset kyCoord, "yCoordinate"
        ky = kyCoord
    endif

    ; Check if XYPad Coords have moved
    if changed(kx) == 1 && changed(kxCoord) == 0 then
        chnset kx, "xCoord"
    endif

    if changed(ky) == 1 && changed(kyCoord) == 0 then
        chnset ky, "yCoord"
    endif

    ; Process vowel formants
    kTune chnget "tune"
    kFineTune chnget "fineTune"
    kNormTune = (kTune + 24) / 48

    ; Define formant values for midrange vowels
    iF1_i_Mid = 270
    iF2_i_Mid = 2300
    iF3_i_Mid = 3000
    iF4_i_Mid = 3000
    iF5_i_Mid = 3000

    iF1_u_Mid = 300
    iF2_u_Mid = 870
    iF3_u_Mid = 2240
    iF4_u_Mid = 2240
    iF5_u_Mid = 2240

    iF1_e_Mid = 390
    iF2_e_Mid = 2000
    iF3_e_Mid = 2800
    iF4_e_Mid = 2800
    iF5_e_Mid = 2800
    
    iF1_o_Mid = 460
    iF2_o_Mid = 920
    iF3_o_Mid = 2420
    iF4_o_Mid = 2420
    iF5_o_Mid = 2420
    
    iF1_a_Mid = 730
    iF2_a_Mid = 1090
    iF3_a_Mid = 2440
    iF4_a_Mid = 2440
    iF5_a_Mid = 2440
    
    ; Calculate the mean frequency for all vowels
    iF1_Mid = (iF1_i_Mid + iF1_u_Mid + iF1_e_Mid + iF1_o_Mid + iF1_a_Mid) / 5
    iF2_Mid = (iF2_i_Mid + iF2_u_Mid + iF2_e_Mid + iF2_o_Mid + iF2_a_Mid) / 5
    iF3_Mid = (iF3_i_Mid + iF3_u_Mid + iF3_e_Mid + iF3_o_Mid + iF3_a_Mid) / 5
    iF4_Mid = (iF4_i_Mid + iF4_u_Mid + iF4_e_Mid + iF4_o_Mid + iF4_a_Mid) / 5
    iF5_Mid = (iF5_i_Mid + iF5_u_Mid + iF5_e_Mid + iF5_o_Mid + iF5_a_Mid) / 5
    
    ; Adjust formants based on normalized tuning
    kF1_i = iF1_i_Mid + (iF1_Mid - iF1_i_Mid) * kNormTune
    kF2_i = iF2_i_Mid + (iF2_Mid - iF2_i_Mid) * kNormTune
    kF3_i = iF3_i_Mid + (iF3_Mid - iF3_i_Mid) * kNormTune
    kF4_i = iF4_i_Mid + (iF4_Mid - iF4_i_Mid) * kNormTune
    kF5_i = iF5_i_Mid + (iF5_Mid - iF5_i_Mid) * kNormTune

    kF1_u = iF1_u_Mid + (iF1_Mid - iF1_u_Mid) * kNormTune
    kF2_u = iF2_u_Mid + (iF2_Mid - iF2_u_Mid) * kNormTune
    kF3_u = iF3_u_Mid + (iF3_Mid - iF3_u_Mid) * kNormTune
    kF4_u = iF4_u_Mid + (iF4_Mid - iF4_u_Mid) * kNormTune
    kF5_u = iF5_u_Mid + (iF5_Mid - iF5_u_Mid) * kNormTune

    kF1_e = iF1_e_Mid + (iF1_Mid - iF1_e_Mid) * kNormTune
    kF2_e = iF2_e_Mid + (iF2_Mid - iF2_e_Mid) * kNormTune
    kF3_e = iF3_e_Mid + (iF3_Mid - iF3_e_Mid) * kNormTune
    kF4_e = iF4_e_Mid + (iF4_Mid - iF4_e_Mid) * kNormTune
    kF5_e = iF5_e_Mid + (iF5_Mid - iF5_e_Mid) * kNormTune
    
    kF1_o = iF1_o_Mid + (iF1_Mid - iF1_o_Mid) * kNormTune
    kF2_o = iF2_o_Mid + (iF2_Mid - iF2_o_Mid) * kNormTune
    kF3_o = iF3_o_Mid + (iF3_Mid - iF3_o_Mid) * kNormTune
    kF4_o = iF4_o_Mid + (iF4_Mid - iF4_o_Mid) * kNormTune
    kF5_o = iF5_o_Mid + (iF5_Mid - iF5_o_Mid) * kNormTune

    ; 2D Interpolation for 'i' and 'u' and between 'e' and 'o'
    kF1_i_u = kF1_i + (kF1_u - kF1_i) * ky
    kF2_i_u = kF2_i + (kF2_u - kF2_i) * ky
    kF3_i_u = kF3_i + (kF3_u - kF3_i) * ky
    kF4_i_u = kF4_i + (kF4_u - kF4_i) * ky
    kF5_i_u = kF5_i + (kF5_u - kF5_i) * ky

    kF1_e_o = kF1_e + (kF1_o - kF1_e) * ky
    kF2_e_o = kF2_e + (kF2_o - kF2_e) * ky
    kF3_e_o = kF3_e + (kF3_o - kF3_e) * ky
    kF4_e_o = kF4_e + (kF4_o - kF4_e) * ky
    kF5_e_o = kF5_e + (kF5_o - kF5_e) * ky

    kF1 = kF1_i_u + (kF1_e_o - kF1_i_u) * kx
    kF2 = kF2_i_u + (kF2_e_o - kF2_i_u) * kx
    kF3 = kF3_i_u + (kF3_e_o - kF3_i_u) * kx
    kF4 = kF4_i_u + (kF4_e_o - kF4_i_u) * kx
    kF5 = kF5_i_u + (kF5_e_o - kF5_i_u) * kx

    ; Final frequency adjustments based on tuning
    kF1 = kF1 * pow(2, kTune/12 + kFineTune/100)
    kF2 = kF2 * pow(2, kTune/12 + kFineTune/100)
    kF3 = kF3 * pow(2, kTune/12 + kFineTune/100)
    kF4 = kF4 * pow(2, kTune/12 + kFineTune/100)
    kF5 = kF5 * pow(2, kTune/12 + kFineTune/100)

    chnset kF1, "kFreq1"
    chnset kF2, "kFreq2"
    chnset kF3, "kFreq3"
    chnset kF4, "kFreq4"
    chnset kF5, "kFreq5"
    
    ; Get status of UI variables
    kActivate chnget "autoVowel"
    kAutoSpeed chnget "autoSpeed"
    kAutoRadius chnget "autoRadius"
    kAutoRandom chnget "autoRandom"
    kAutoRandomRadius chnget "autoRandomRadius"
    gkPrevSplatter init 0  
    gkTargetX init 0       
    gkTargetY init 0     
    kSplatter chnget "splatter"
    kSplatSpeed chnget "splatSpeed"
    kHardSplat chnget "hardSplat"

    gkSplatCounter init 0

    if kSplatter == 1 then
    if kHardSplat == 1 then
    
        gkSplatCounter = gkSplatCounter + 1

        if gkSplatCounter > (100 /* Splat Effect Scaler */ / kSplatSpeed) then
            gkTargetX random 0, 1
            gkTargetY random 0, 1
            kx = gkTargetX
            ky = gkTargetY
            gkSplatCounter = 0 
        endif
    else
        ; Generate a random target point if just starting or active and  close enough to the previous target
        if gkPrevSplatter == 0 || abs(kx - gkTargetX) < 0.05 && abs(ky - gkTargetY) < 0.05 then
            gkTargetX random 0, 1
            gkTargetY random 0, 1
        endif

            ; Lerp towards the target.
            kx = kx + (gkTargetX - kx) * kSplatSpeed / 20
            ky = ky + (gkTargetY - ky) * kSplatSpeed / 20
        endif

        ; Update the X and Y coordinate channels
        chnset kx, "xCoordinate"
        chnset ky, "yCoordinate"
        
    endif

    kPrevSplatter = kSplatter 

    if kActivate == 1 then
    
        ; Circular motion for X and Y coordinates 
        kTheta phasor kAutoSpeed
        kRadius = abs(kAutoRadius)
        kDirection = signum(kAutoRadius) 

        kx = 0.5 + kRadius*cos(kTheta*2*$M_PI*kDirection) 
        ky = 0.5 + kRadius*sin(kTheta*2*$M_PI*kDirection)

        ; Introduce a shiver effect based on AutoRandomRadius
        kRandomX = kAutoRandom * kAutoRandomRadius * randh(1, -1, 10)
        kRandomY = kAutoRandom * kAutoRandomRadius * randh(1, -1, 10)

        kx = kx + kRandomX
        ky = ky + kRandomY

        kx limit kx, 0, 1
        ky limit ky, 0, 1

        chnset kx, "xCoordinate"
        chnset ky, "yCoordinate"
    endif
    
endin

instr InitXYPad

    kx chnget "xCoord"
    ky chnget "yCoord"
    kxCoord chnget "xCoordinate"
    kyCoord chnget "yCoordinate"
    
    iXinit = 0.5
    iYinit = 0.5
    
    chnset iXinit, "xCoord"
    chnset iYinit, "yCoord"
    chnset iXinit, "xCoordinate"
    chnset iYinit, "yCoordinate"
    
endin

</CsInstruments>
<CsScore>
i "InitXYPad" 0 1
i "UpdateGUI" 0 z

f 10 0 4096 10 1 ; Sine wave for LFO

</CsScore>




</CsoundSynthesizer>