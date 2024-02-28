<Cabbage>
form caption("Tree Grow") size(400, 750), colour(25, 25, 25), pluginId("grow")
label bounds(10, 10, 380, 20), text("Wood Growth"), fontColour(255, 255, 255), font("default", 16)
button bounds(160, 40, 80, 25) channel("ONOFF") text("OFF", "ON")
hslider bounds(10, 80, 380, 40) channel("Amplitude") range(0, 1, 0.5, 1, 0.01) text("Amplitude") trackerColour("green")
hslider bounds(10, 130, 380, 40) channel("Frequency") range(0, 100, 80, 1, 0.01) text("Frequency") trackerColour("green")
hslider bounds(10, 180, 380, 40) channel("HighPass") range(100, 20000, 100, 1, 0.01) text("HighPass") trackerColour("green")
hslider bounds(10, 230, 380, 40) channel("LowPass") range(20, 20000, 20000, 1, 0.01) text("LowPass") trackerColour("green")
hslider bounds(10, 280, 380, 40) channel("BandPass") range(20, 20000, 20, 1, 0.01) text("BandPass") trackerColour("green")
hslider bounds(10, 330, 380, 40) channel("Snap") range(0, 1, 1, 1, 0.01) text("Snap") trackerColour("green")
hslider bounds(10, 380, 380, 40) channel("Crackle") range(0, 1, 1, 1, 0.01) text("Crackle") trackerColour("green")
hslider bounds(10, 430, 380, 40) channel("Creak") range(0, 1, 1, 1, 0.01) text("Creak") trackerColour("green")
hslider bounds(10, 480, 380, 40) channel("Rumble") range(0, 1, 1, 1, 0.01) text("Rumble") trackerColour("green")
hslider bounds(10, 530, 380, 40) channel("GrowthStage") range(0, 1, 0, 1, 0.01) text("Growth") trackerColour("green")

signaldisplay bounds(10, 580, 380, 160), colour("green") displayType("waveform"), backgroundColour(0, 0, 0), zoom(-1), signalVariable("aOut"), channel("display")
</Cabbage>

<CsoundSynthesizer>
<CsOptions>
-odac -n -d -+rtmidi=NULL -M0 -m0d --displays
</CsOptions>
<CsInstruments>

sr = 44100
ksmps = 32
nchnls = 1
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

kGrowthStage chnget "GrowthStage"
kAmp chnget "Amplitude"
kSnap chnget "Snap"
kCrackle chnget "Crackle"
kCreaking chnget "Creak"
kRumble chnget "Rumble"

kCurveFactorFreq = 1 ; Adjust for dynamic evolution
kCurveFactorLPF = 1 ; Adjust for LPF max value quicker
kCurveFactorHPF = 0.2 ; Adjust for HPF

kFreqExp = pow(kGrowthStage, kCurveFactorFreq) * 100 ; Exponential scale from 0 to 100
kRumbleExp = pow(kGrowthStage, kCurveFactorFreq) * 100 / 0.4
kCreakExp = pow(kGrowthStage, kCurveFactorFreq) * 100 / 2
kCrackleExp = pow(kGrowthStage, kCurveFactorFreq) * 200 
kSnapExp = pow(kGrowthStage, kCurveFactorFreq) * 100 / 50

kLPFExp = pow(kGrowthStage, kCurveFactorLPF) * 20000 ; Exponential scale from 0 to 20000, faster
kHPFExp = 20000 - pow(kGrowthStage, kCurveFactorHPF) * 20000 ; Inverse exponential scale from 20000 down to 0, adjusted

chnset kFreqExp, "Frequency"
chnset kHPFExp, "HighPass"
chnset kLPFExp, "LowPass"

; WOOD FREQUENCIES

kLfoRumbleRate = 0.01
kLfoCreakingRate randi 0.15,  0.35, 0.01 ; Random LFO rate between 30 and 70 Hz for Creaking
kLfoCracklingRate randi 0.2, 0.5, 0.01 ; Random LFO rate between 150 and 250 Hz for Crackling
kLfoSnappingRate randi 0.5, 1, 0.01 ; Random LFO rate between 800 and 1200 Hz for Snapping

kLfoRumble lfo 20, kLfoRumbleRate ; Modulate Rumble frequency
kRumbleFreq = 10 + kLfoRumble 
kRumbleBandwidth = kRumbleFreq + 10 

kLfoCreaking lfo 50, kLfoCreakingRate ; Modulate Creaking frequency
kCreakingFreq = 500 + kLfoCreaking 
kCreakingBandwidth = kLfoCreaking + kLfoCreaking + 200

kLfoCrackling lfo 200, kLfoCracklingRate ; Modulate Crackling frequency
kCracklingFreq = 15000 + kLfoCrackling 
kCracklingBandwidth = kCracklingFreq + kLfoCrackling + 50 

kLfoSnapping lfo 1000, kLfoSnappingRate ; Modulate Snapping frequency
kSnappingFreq = 3500 + kLfoSnapping 
kSnappingBandwidth = 2000  

; AMP SCALING

kRumbleAmpScaled = kAmp * 0.004 
kCreakingAmpScaled = kAmp * 0.01
kCracklingAmpScaled = kAmp * 0.3
kSnappingAmpScaled = kAmp * 0.6

; ==== RNDM IMPULSES =====

aRumbleSrc dust2 kRumbleAmpScaled * kRumble, kRumbleExp 
aRumble reson aRumbleSrc, kRumbleFreq, kRumbleBandwidth

aCreakingSrc dust2 kCreakingAmpScaled * kCreaking, kCreakExp
aCreaking reson aCreakingSrc, kCreakingFreq, kCreakingBandwidth

aCracklingSrc dust2 kCracklingAmpScaled * kCrackle, kCrackleExp 
aCrackling reson aCracklingSrc, kCracklingFreq, kCracklingBandwidth

aSnappingSrc dust2 kSnappingAmpScaled * kSnap, kSnapExp 
aSnapping reson aSnappingSrc, kSnappingFreq, kSnappingBandwidth

aOut = aSnapping + aCrackling + aCreaking + aRumble

outs aOut

display aOut, .1, 1

endin






</CsInstruments>
<CsScore>
i"DetectTriggers" 0 z

</CsScore>
</CsoundSynthesizer>
