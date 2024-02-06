<Cabbage>
form caption("Terrain Sound"), size(450, 880), guiMode("polling"), colour(0, 0, 0), pluginId("ter1")

    label bounds(-9, 16, 90, 15) text("Mode:") colour(0, 0, 0)
    combobox bounds(60, 15, 100, 18) channel("terrainType") items("Grass", "Wood", "Stone", "Mud","Snow", "Water", "Custom") colour(100, 100, 100) 

    button bounds(10, 50, 80, 30), channel("toggleOneStep"), identChannel("toggleOneStepID"), text("OneStep"), latched(0), colour(150, 0, 0), fontColour(255, 255, 255)
    button bounds(90, 50, 80, 30), channel("toggleSteps"), identChannel("toggleStepsID"), text("Steps: OFF", "Steps: ON"), latched(1), colour(0, 150, 0), fontColour(255, 255, 255)
    button bounds(170, 50, 80, 30), channel("toggleSneak"), identChannel("toggleSneakID"), text("Sneak: OFF" , "Sneak: ON"), latched(1), colour(150, 150, 0), fontColour(255, 255, 255)
    button bounds(250, 50, 80, 30), channel("toggleDrone"), identChannel("toggleDroneID"), text("Drone: OFF" , "Drone: ON"), latched(1), colour(0, 0, 150), fontColour(255, 255, 255)
    
    groupbox bounds(15, 90, 325, 145) text("ADSR") colour(35, 35, 35)
    hslider bounds(16, 110, 325, 25) channel("attack") range(0.01, 1, 0.1, 1) text("Attack") colour(145, 145, 145) trackerColour(170, 170, 255)
    hslider bounds(16, 140, 325, 25) channel("decay") range(0.01, 1, 0.1, 1) text("Decay") colour(145, 145, 145) trackerColour(170, 170, 255)
    hslider bounds(16, 170, 325, 25) channel("sustain") range(0, 1, 0.5, 1) text("Sustain") colour(145, 145, 145) trackerColour(170, 170, 255)
    hslider bounds(16, 200, 325, 25) channel("release") range(0.01, 1, 0.5, 1) text("Release") colour(145, 145, 145) trackerColour(170, 170, 255)

    groupbox bounds(345, 45, 85, 190) text("Steps") colour(35, 35, 35)
    vslider bounds(345, 70, 50, 160), channel("speed"), identChannel("speedID"), text(".Vel"), range(0, 1, 0.5), colour(145, 145, 145), fontColour(255, 255, 255), trackerColour(0, 150, 0)
    vslider bounds(380, 70, 50, 160), channel("stepRand"), text(".Rnd"), range(0, 1, 0.5), colour(145, 145, 145), fontColour(255, 255, 255), trackerColour(0, 150, 0)
    
    groupbox bounds(10, 240, 420, 120) text("EQ") colour(35, 35, 35)
    hslider bounds(20, 260, 360, 25) channel("bassEQ") range(-1, 1, 0) text("Bass EQ") colour(145, 145, 145) trackerColour(255, 170, 255) 
    hslider bounds(20, 290, 360, 25) channel("midEQ") range(-1, 1, 0) text("Mid EQ") colour(145, 145, 145) trackerColour(255, 170, 255)
    hslider bounds(20, 320, 360, 25) channel("trebleEQ") range(-1, 1, 0) text("Treble EQ") colour(145, 145, 145) trackerColour(255, 170, 255)
    vslider bounds(370, 260, 58, 85) channel("eqBandwidth") range(0, 2, 1) text("Bandwidth") colour(145, 145, 145) trackerColour(100, 100, 255)

    groupbox bounds(10, 365, 420, 180) text("Textures") colour(35, 35, 35)
    hslider bounds(20, 385, 400, 25) channel("grassMix") range(0, 1, 0.02) text("Grass") colour(145, 145, 145) trackerColour(0, 200, 0)
    hslider bounds(20, 410, 400, 25) channel("woodMix") range(0, 1, 0) text("Wood") colour(145, 145, 145) trackerColour(250, 155, 0)
    hslider bounds(20, 435, 400, 25) channel("stoneMix") range(0, 1, 0) text("Stone") colour(145, 145, 145) trackerColour(180, 180, 180)
    hslider bounds(20, 460, 400, 25) channel("mudMix") range(0, 1, 0) text("Mud") colour(145, 145, 145) trackerColour(205, 133, 63)
    hslider bounds(20, 485, 400, 25) channel("waterMix") range(0, 1, 0) text("Water") colour(145, 145, 145) trackerColour(85, 153, 178)
    hslider bounds(20, 510, 400, 25) channel("snowMix") range(0, 1, 0) text("Snow") colour(145, 145, 145) trackerColour(255, 255, 255)  // Added Snow Slider

    groupbox bounds(10, 550, 430, 130) text("Articulation") colour(35, 35, 35)   // Adjusted height for snug fit
    hslider bounds(20, 570, 400, 25) channel("pitch") range(-1, 1, 0) text("Pitch") colour(145, 145, 145) trackerColour(85, 153, 178)
    hslider bounds(20, 595, 400, 25) channel("stepForce") range(0, 1, 0.25) text("Force") colour(145, 145, 145) trackerColour(85, 153, 178)
    hslider bounds(20, 620, 400, 25) channel("heel") range(0.01, 1, 0.25) text("Heel") colour(145, 145, 145) trackerColour(85, 153, 178)
    rslider bounds(0, 0, 0) channel("heelMixer") visible(0)
    hslider bounds(20, 645, 400, 25) channel("heelRandom") range(0.01, 1, 0.25) text("HeelRand") colour(145, 145, 145) trackerColour(85, 153, 178)
    
    groupbox bounds(10, 685, 430, 75) text("Material") colour(35, 35, 35)
    hslider bounds(20, 705, 400, 25) channel("wetness") range(0, 1, 0.5) text("Wetness") colour(145, 145, 145) trackerColour(85, 153, 178)
    hslider bounds(20, 730, 400, 25) channel("roughness") range(0, 1, 0.5) text("Roughness") colour(145, 145, 145) trackerColour(85, 153, 178)

    groupbox bounds(10, 765, 430, 50) text("Grass") colour(35, 35, 35)
    hslider bounds(20, 785, 400, 25) channel("grassLength") range(0, 1, 0.5) text("Grass Length") colour(145, 145, 145) trackerColour(0, 200, 0)  // Green for Grass

    groupbox bounds(10, 820, 430, 50) text("Water") colour(35, 35, 35)
    hslider bounds(20, 840, 400, 25) channel("waterDepth") range(0, 1, 0.5) text("Depth") colour(145, 145, 145) trackerColour(85, 153, 178)


</Cabbage>

<CsoundSynthesizer>
<CsOptions>
-n -d
</CsOptions>
<CsInstruments>

sr = 44100
ksmps = 16
nchnls = 2
0dbfs = 1

; ADSR and targetHeel for Grass
giGrassA = 0.01
giGrassD = 0.1
giGrassS = 0
giGrassR = 0.15
giGrassTargetHeel = 0.01  ; Grass has standard attack

; ADSR and targetHeel for Wood
giWoodA = 0.001
giWoodD = 0.05
giWoodS = 0
giWoodR = 0.05
giWoodTargetHeel = -0.12  ; Wood should have less attack

; ADSR and targetHeel for Stone
giStoneA = 0.01
giStoneD = 0.05
giStoneS = 0
giStoneR = 0.1
giStoneTargetHeel = 0.1  ; Adjust as per requirement

; ADSR and targetHeel for Mud
giMudA = 0.001
giMudD = 0.25
giMudS = 0
giMudR = 0.3
giMudTargetHeel = 0.1  ; Adjust as per requirement

; ADSR and targetHeel for Water
giWaterA = 0.1
giWaterD = 0.1
giWaterS = 0
giWaterR = 0.5
giWaterTargetHeel = 0.5  ; Adjust as per requirement

; ADSR and targetHeel for Snow
giSnowA = 0.02        
giSnowD = 0.12        
giSnowS = 0.0
giSnowR = 0.2
giSnowTargetHeel = 0.2

gkDroneActive = 0
gkStepsActive init 0

instr DetectTriggers

    kToggleOneStep chnget "toggleOneStep"
    kTrigOneStep changed kToggleOneStep
    
    kToggleSteps chnget "toggleSteps"
    kTrigSteps changed kToggleSteps
    
    kToggleSneak chnget "toggleSneak"
    kTrigSneak changed kToggleSneak
    
    kToggleDrone chnget "toggleDrone"
    kTrigDrone changed kToggleDrone
    
    ; OneStep Button
    if kTrigOneStep == 1 then
        if kToggleOneStep == 1 then
            
            chnset "colour(150, 0, 0)", "toggleOneStepID"          
            
            event "i", "Footstep", 0, 0
            event "i", "Footstep", 0, -1
                   
        else 
            chnset "colour(0, 0, 0)", "toggleOneStepID" 
        endif
    endif
    
    ; Drone Button
    if kTrigDrone == 1 then
        if kToggleDrone == 1 then
            
            gkDroneActive = 1
            
            chnset "colour(0, 0, 150)", "toggleDroneID"      
            
            event "i", "Footstep", 0, 0
            event "i", "Footstep", 0, -1
                   
        else 
            gkDroneActive = 0
            
            event "i", "Footstep", 0, 0
            event "i", "Footstep", 0, -1
            chnset "colour(0, 0, 0)", "toggleDroneID" 
        endif
    endif

    ; Steps Button
    if kTrigSteps == 1 then
        if kToggleSteps == 1 then
            
            chnset 0, "toggleInstrument"
            
            chnset "colour(0, 0, 0)", "toggleDroneID" 
            chnset "text(\"Steps: 0\", \"Steps: 1\")", "toggleStepsID"
            chnset "colour(0, 150, 0)", "toggleStepsID" 

            event "i", "Footstep", 0, 0
            event "i", "Footstep", 0, -1
            
            event "i", "ControlSteps", 0, 0
            event "i", "ControlSteps", 0, -1
            
            else
                chnset "text(\"Steps: 1\", \"Steps: 0\")", "toggleStepsID"
                chnset "colour(0, 0, 0)", "toggleStepsID"
                event "i", "ControlSteps", 0, 0
        endif
    endif
    
    ; Sneak Button
    if kTrigSneak == 1 then
        if kToggleSneak == 1 then
            chnset "text(\"Sneak: ON\")", "toggleSneakID"
        else
            chnset "text(\"Sneak: OFF\")", "toggleSneakID"
        endif
    endif 
    
endin

instr UpdateMixer
    
    ; Get the terrain mixtures and other parameters
    kGrassMix chnget "grassMix"
    kWoodMix chnget "woodMix"
    kStoneMix chnget "stoneMix"
    kMudMix chnget "mudMix"
    kWater chnget "waterMix"
    kSnowMix chnget "snowMix" ; Added Snow Mix
    kHeelMixer chnget "heelMixer"
    kWaterDepth chnget "waterDepth"
    
    ; Get the force value and grass length from the channel
    kForce chnget "stepForce"
    kGrassLength chnget "grassLength"
    kGrassLength = kGrassLength * 2.5
    
    kSum max 0.1, (kGrassMix + kWoodMix + kStoneMix + kMudMix + kWater + kSnowMix)

    kDecayBase = (kGrassMix * giGrassD + kWoodMix * giWoodD + kStoneMix * giStoneD + kMudMix * giMudD + kWater * giWaterD + kSnowMix * giSnowD) / kSum
    kAttackBase = (kGrassMix * giGrassA + kWoodMix * giWoodA + kStoneMix * giStoneA + kMudMix * giMudA + kWater * giWaterA + kSnowMix * giSnowA) / kSum
    kReleaseBase = (kGrassMix * giGrassR + kWoodMix * giWoodR + kStoneMix * giStoneR + kMudMix * giMudR + kWater * giWaterR + kSnowMix * giSnowR) / kSum
    kSustainBase = (kGrassMix * giGrassS + kWoodMix * giWoodS + kStoneMix * giStoneS + kMudMix * giMudS + kWater * giWaterS + kSnowMix * giSnowS) / kSum
    
    ; Scaling the ADSR values based on kForce and iGrassLength
    kAttack = kAttackBase * (1 + kForce * 0.5 + kGrassLength * 0.5)
    kDecay = kDecayBase * (1 + kForce * 0.5 + kGrassLength * 0.25)
    kRelease = kReleaseBase * (1 + kForce * 0.5 + kGrassLength * 0.5)
    kSustain = kSustainBase * (1 + kForce * 0.5 + kGrassLength * 0.5)


    ; Handle special drone active case
    if (gkDroneActive == 1) then 
        kAttack = 1
        kSustain = 1 
    endif

    ; Heel mixer calculation
    kHeelMixer = (kGrassMix * giGrassTargetHeel + kWoodMix * giWoodTargetHeel + kStoneMix * giStoneTargetHeel + kMudMix * giMudTargetHeel + kWater * giWaterTargetHeel + kSnowMix * giSnowTargetHeel) / kSum

    ; Update the UI sliders with these values
    chnset kAttack, "attack"
    chnset kDecay, "decay"
    chnset kSustain, "sustain"
    chnset kRelease, "release"
    chnset kHeelMixer, "heelMixer"
    
endin

instr ControlSteps
    
    kRawSpeed chnget "speed"
    kStepRand chnget "stepRand"
    
    kLastTriggerTime init 0  
    
    kMinInterval = 0.2  ; Minimum interval between steps
    kCounter init 0  
    
    kRawSpeed = kRawSpeed * 0.6 ; Map speed range to 0 - 0.6

    kExpFactor = 5  ; Exponential curve factor

    kStepSpeed = kMinInterval + (10 - kMinInterval) * pow(kRawSpeed, kExpFactor)

    kCounter = kCounter + ksmps / sr
    kCurrentTime = kCounter

    kRandRange = (1 - kRawSpeed) * (kStepSpeed - 0.1) * kStepRand ; Define range for randomness, scale inversely with kRawSpeed
    
    kRandModifier randh 2 * kRandRange - kRandRange, 0.4 ; Generate a random interval using randh. The range is [-kRandRange, kRandRange]

    kWaitTime = kStepSpeed + kRandModifier ; Compute wait time 

    kMinWaitTime = 0.15  ; Ensure wait time doesn't go below a threshold
    if kWaitTime < kMinWaitTime then
        kWaitTime = kMinWaitTime
    endif

    if kCurrentTime - kLastTriggerTime >= kWaitTime then  ; Wait interval and trigger footstep
        event "i", "Footstep", 0.1, kWaitTime
        kLastTriggerTime = kCurrentTime
    endif
endin

instr Footstep

    iType chnget "terrainType"
    iTrigger chnget "trigger"
    iAttack chnget "attack"
    iDecay chnget "decay"
    iSustain chnget "sustain"
    iRelease chnget "release"
    iGrassMix chnget "grassMix"
    iWoodMix chnget "woodMix"
    iStoneMix chnget "stoneMix"
    iMudMix chnget "mudMix"
    iSnowMix chnget "snowMix"
    iWater chnget "waterMix"
    kRawSpeed chnget "speed"
    
    iGrassLength chnget "grassLength"
    iWaterDepth chnget "waterDepth"
    
    iSum max 0.1, (iGrassMix + iWoodMix + iStoneMix + iMudMix + iWater + iSnowMix)
    iRoughness chnget "roughness"
    aGrassSound init 0
    aWood init 0
    aStone init 0
    aMud init 0
    aWaterSound init 0
    aSnow init 0

    kHeel chnget "heel"
    kHeelRand chnget "heelRand"
    kHeelMixer chnget "heelMixer"
    
    ; Generate a deviation only once
    kDeviation random 0.1, 0.2
    
    iPitch chnget "pitch"
    iForce chnget "stepForce"
    kForceMappedPitch = iForce * 7 
    kPitchMultiplierForce = pow(2, kForceMappedPitch / 12)
    
    iPitchMapped = 0.5 + (iPitch + 1) * 0.5 ; Maps -1 to 0.5 and 1 to 1.5

    ; Calculate the new heel value
    kHeel = (kHeelRand + kDeviation)
    kHeel = (kHeel < kHeelRand + 0.1 ? kHeelRand + 0.1 : (kHeel > kHeelRand + 0.2 ? kHeelRand + 0.2 : kHeel))
    kHeel = (kHeel < 0 ? 0 : (kHeel > 1 ? 1 : kHeel))
    kHeel = kHeel * kRawSpeed + 0.09
    kHeel = 0.5 * (kHeelMixer + kHeel)

    ; Update the heel channel
    chnset kHeel, "heel"
    
    ; === Texture Generators ===
    iSineTable ftgen 0, 0, 16384, 10, 1

    aNoise pinker
    aWhiteNoise noise .3, 1
    
if iGrassMix > 0 then
    
    iForce chnget "stepForce"
    iRoughness chnget "roughness"
    
    iForce = 0.1 + iForce * 0.8

    iRoughness = 0.1 + iRoughness * 0.8
    iRoughness = iRoughness * 0.005

    ; Roughness Modulation
    aFilteredNoise butlp aNoise, 100
    kVCOFreq = aFilteredNoise * (50000 * iRoughness)
    aVCOOut oscili 1, kVCOFreq
        
    ; Grass Modal Synthesis Parameters
    kGrassFreqs[] fillarray 3000, 3500, 4000, 6000, 8000, 11000, 14000  ; Start with higher frequencies
    kGrassFreqs = kGrassFreqs * kPitchMultiplierForce
    kGrassQs[] fillarray 3, 3, 3, 4, 16, 16, 16  ; Slightly broader frequency response
    kGrassAmp = 0.1 

    aGrass1 reson aNoise, kGrassFreqs[0], kGrassFreqs[0]/kGrassQs[0]
    aGrass2 reson aNoise, kGrassFreqs[1], kGrassFreqs[1]/kGrassQs[1]
    aGrass3 reson aNoise, kGrassFreqs[2], kGrassFreqs[2]/kGrassQs[2]
    aGrass4 reson aNoise, kGrassFreqs[3], kGrassFreqs[3]/kGrassQs[3]
    aGrass5 reson aNoise, kGrassFreqs[4], kGrassFreqs[4]/kGrassQs[4]
    aGrass6 reson aNoise, kGrassFreqs[5], kGrassFreqs[5]/kGrassQs[5]
    aGrass7 reson aNoise, kGrassFreqs[6], kGrassFreqs[6]/kGrassQs[6]
    aGrassModal = (aGrass1 + aGrass2 + aGrass3 + aGrass4 + aGrass5 + aGrass6 + aGrass7) * kGrassAmp
    
    aGrassModal = aGrassModal * (1 + aVCOOut)
    
    kNoiseFilterFreq = 15000  
    kNoiseFilterBW = 8000     
    kNoiseAmp = 0.1 * iForce 
    aFilteredGrassNoise reson aNoise, kNoiseFilterFreq, kNoiseFilterBW 
    aFilteredGrassNoise = aFilteredGrassNoise * kNoiseAmp           

    aGrassModalEnhanced = aGrassModal + aFilteredGrassNoise 

    aGrassCombined = aGrassModalEnhanced * iForce 

    kGrassRMS rms aGrassCombined
    kSafeGrassRMS = max(kGrassRMS, 0.00001)
    kTargetGrassRMS = 0.1
    aGrassSound = aGrassCombined * (kTargetGrassRMS / kSafeGrassRMS)
 
    aGrassSound limit aGrassSound, -1, 1
    
    aGrassHP buthp aGrassSound, 1000  
    
    aGrassSound = aGrassSound * aGrassHP
endif

if iWoodMix > 0 then

    iForce chnget "stepForce"
    iRoughness chnget "roughness"
    
    iForce = 0.1 + iForce * 0.8
    iForce = iForce * 0.1
    
    iRoughness = 0.1 + iRoughness * 0.8
    iRoughness = iRoughness * 0.01
    
    kForceFactor = 0.1 + (1 / (1 + exp(-10 * (iForce - 0.1))))
    kResonanceFactor = 0.2 + (pow(iForce, 1.5) * (1 - 0.2))
    kFreqShift = 1000 - (pow(iForce, 1.5) * 100)

    kSemitoneChange = iPitch * 12  
    kPitchMultiplier = pow(2, kSemitoneChange / 12)
    kWoodFreqs[] fillarray 250, 450, 650, 850
    kWoodFreqs = kWoodFreqs * kPitchMultiplierForce + kFreqShift

    kWoodQs[] fillarray 5, 5, 5, 5
    kWoodAmp = 0.03
    

    ; Roughness Modulation
    
    aFilteredNoise butlp aNoise, 150
    kVCOFreq = aFilteredNoise * (50000 * iRoughness)
    aVCOOut oscili 1, kVCOFreq
    
    ; Resonant Bandwidth
    
    kWoodResBW = 3  
    kWoodResFreqs[] fillarray 220, 390, 580, 760, 950

    aWoodRes1 reson aNoise, (kWoodResFreqs[0] + aVCOOut * 20) * iPitchMapped, kWoodResBW * kResonanceFactor, iForce
    aWoodRes2 reson aNoise, (kWoodResFreqs[1] + aVCOOut * 20), kWoodResBW * kResonanceFactor, iForce
    aWoodRes3 reson aNoise, (kWoodResFreqs[2] + aVCOOut * 20), kWoodResBW * kResonanceFactor, iForce
    aWoodRes4 reson aNoise, (kWoodResFreqs[3] + aVCOOut * 20), kWoodResBW * kResonanceFactor, iForce
    aWoodRes5 reson aNoise, (kWoodResFreqs[4] + aVCOOut * 20), kWoodResBW * kResonanceFactor, iForce

    aWood1 reson aNoise, kWoodFreqs[0] + aVCOOut * 20, kWoodFreqs[0]/kWoodQs[0]
    aWood2 reson aNoise, kWoodFreqs[1] + aVCOOut * 20, kWoodFreqs[1]/kWoodQs[1]
    aWood3 reson aNoise, kWoodFreqs[2] + aVCOOut * 20, kWoodFreqs[2]/kWoodQs[2]
    aWood4 reson aNoise, kWoodFreqs[3] + aVCOOut * 20, kWoodFreqs[3]/kWoodQs[3]
    aWoodModal = (aWood1 + aWood2 + aWood3 + aWood4) * kWoodAmp * iForce

    aWoodResSum = (aWoodRes1 + aWoodRes2 + aWoodRes3 + aWoodRes4 + aWoodRes5) * 0.2

    ; Combined Wood Sound
    aWoodCombined = aWoodModal + aWoodResSum * iForce * aVCOOut

    ; RMS Normalization for Wood
    kRMS rms aWoodCombined
    kSafeRMS = max(kRMS, 0.00001)
    kTargetRMS = 0.1
    aWood = aWoodCombined * (kTargetRMS / kSafeRMS)
    aWood clip aWood, -1, 1

    aWoodClipped limit aWood, -1, 1
    aWood = aWood * kForceFactor * 0.3
endif

if iStoneMix > 0 then
    
    iForce chnget "stepForce"
    iRoughness chnget "roughness"
    
    iForce = 0.01 + iForce * 0.95
    
    iRoughness = 0.1 + iRoughness * 0.95

    ; Stone texture
    
    kStoneFreqs[] fillarray 2500, 2900, 3300, 3700, 4100, 4500, 4900, 5300
    kStoneFreqs = kStoneFreqs * iForce 
    kStoneQs[] fillarray 10, 10, 10, 5, 5, 5, 15, 15
    kStoneAmp[] fillarray 0.095, 0.09, 0.085, 0.08, 0.075, 0.07, 0.1, 0.1
    
    aFilteredNoise butlp aNoise, iRoughness * 25 + 0.01

    iForce = (iForce * (0.7 - 0.01)) + 0.25
    iRoughness = (iRoughness * (0.02 - 0.01)) + 0.0001
    
    iRoughnessMultiplier = iRoughness * 12500 + 0.0001
    kVCOFreq = aFilteredNoise * (1000 * iRoughness + 0.01)
    
    aVCOOut oscili 1, kVCOFreq

    ; White noise generation and modulation
    aWhiteNoise randi 6000, (iForce * 10000 * 2)

    ; Update modulated noise calculations
    aScaledNoise = aWhiteNoise * iRoughness * 0.001
    aModulatedNoise = aScaledNoise * (1 + aVCOOut * iForce) * iRoughness / 10

    aStone1 reson aNoise + aModulatedNoise, kStoneFreqs[0] + aVCOOut * 100, kStoneFreqs[0]/kStoneQs[0]
    aStone2 reson aNoise + aModulatedNoise, kStoneFreqs[1] + aVCOOut * 200, kStoneFreqs[1]/kStoneQs[1]
    aStone3 reson aNoise + aModulatedNoise, kStoneFreqs[2] + aVCOOut * 300, kStoneFreqs[2]/kStoneQs[2]
    aStone4 reson aNoise + aModulatedNoise, kStoneFreqs[3] + aVCOOut * 400, kStoneFreqs[3]/kStoneQs[3]
    aStone5 reson aNoise + aModulatedNoise, kStoneFreqs[4] + aVCOOut * 500, kStoneFreqs[4]/kStoneQs[4]
    aStone6 reson aNoise + aModulatedNoise, kStoneFreqs[5] + aVCOOut * 600, kStoneFreqs[5]/kStoneQs[5]
    aStone7 reson aNoise + aModulatedNoise, kStoneFreqs[6] + aVCOOut * 700, kStoneFreqs[6]/kStoneQs[6]
    aStone8 reson aNoise + aModulatedNoise, kStoneFreqs[7] + aVCOOut * 800, kStoneFreqs[7]/kStoneQs[7]

    aStoneModal = aStone1*kStoneAmp[0] + aStone2*kStoneAmp[1] + aStone3*kStoneAmp[2] + aStone4*kStoneAmp[3] + 
                  aStone5*kStoneAmp[4] + aStone6*kStoneAmp[5] + aStone7*kStoneAmp[6] + aStone8*kStoneAmp[7]
               
    aRingMod = aStoneModal * aVCOOut

    aAmpMod = aStoneModal * (0.5 + 0.5 * aVCOOut)
    
    ; Subtractive
    aSub1 butbp aRingMod + aAmpMod, 2800, 50 
    aSub2 butbp aRingMod + aAmpMod, 3200, 50
    aSub3 butbp aRingMod + aAmpMod, 3600, 300
    aSub4 butbp aRingMod + aAmpMod, 4300, 350
    aSubSum = aSub1 + aSub2 + aSub3 + aSub4
    
        
    ; Slapping Sound
    aSlapNoise randi 19000, 20000
    aSlapSignal = aSlapNoise * 0.00025 * (iForce / 2)  ; Doubling the amplitude for more prominence
    
    aPebbleNoise randi 18000, 20000
    aPebbleSignal = aPebbleNoise * 0.0000001 * iForce
    aPebbleHP buthp aPebbleSignal, 18000
    aPebbleSignal = aPebbleSignal + aPebbleHP
    
    ; Add some frequency modulation to the slap sound for more complexity
   ; kModFreq = 20  ; Modulation frequency
    ;kModDepth randi 0.5, 4  ; Random modulation depth between 0.5 and 4 Hz
    ;aSlapSignal = aSlapSignal * oscil(kModDepth, kModFreq, 1)
    
    ; Low frequency "thud" associated with foot impact
    iThudFreq = 20  ; Adjusted to a slightly lower frequency for a deeper thud
    aThud oscili 1, iThudFreq, 1  ; Doubling the amplitude for more prominence
    aThudSignal = aThud

    ; Apply some simple low-pass filtering to the thud to emphasize its low end
    kCutoff = 50 * iForce  ; Cutoff frequency for the low-pass filter
    kQ = 1 * iForce  ; Quality factor, adjust for resonance
    aThudSignal = moogladder(aThudSignal, kCutoff, kQ)

    ; Combine all signals but exclude the thud for now
    aStone = aAmpMod * iForce * aModulatedNoise * aSubSum * aSlapSignal + aPebbleSignal

    ; Apply high-pass filter to the combination
    kRoughnessHPCutoff = (1 - iRoughness) * 900 + 100

    aStoneHP buthp aStone, kRoughnessHPCutoff

    ; Now add the thud back to the filtered signal
    aStoneCombined = aStoneHP + aThudSignal

    ; Clip the combined signal
    aStoneClipped limit aStoneCombined, -1, 1

    kRMSStone rms aStone
    kSafeRMSStone = max(kRMSStone, 0.00001)
    aStone = aStone * (kTargetRMS / kSafeRMSStone)
    aStone limit aStone, -1, 1

    aStone = aStoneClipped * 0.1

    
endif

if iMudMix > 0 then
    
    iForce chnget "stepForce"
    iRoughness chnget "roughness"
    
    iForce = 0.1 + iForce * 0.98
    iRoughness = 0.1 + iRoughness * 0.98
    iRoughness = iRoughness * 0.02

    aFilteredNoise butlp aNoise, 1 

    kVCOFreq = aFilteredNoise * (iForce * 100 * iRoughness)
    aVCOOut oscili 1, kVCOFreq
        
    ; Mud Modal Synthesis Parameters
    kMudFreqs[] fillarray 200, 600, 2000, 8000, 12500, 15000, 18000  ; Frequencies array
    kMudAmps[] fillarray 0, 0, 0, 0, 0, 0.3, 0.4  ; Amplitudes array for each frequency
    kPitchMultiplierForce = kPitchMultiplierForce + 0.1 * 0.001
    kMudQs[] fillarray 16, 6, 3, 4, 16, 16, 16  ; Q values for each frequency
    
    kMudFreqs[0] = kMudFreqs[0] + 100 + linrand(5) - linrand(5)
    kMudFreqs[1] = kMudFreqs[1] + 50 + linrand(100) - linrand(100)
    kMudFreqs[2] = kMudFreqs[2] + linrand(200) - linrand(200) * kPitchMultiplierForce 
    kMudFreqs[3] = kMudFreqs[3] + linrand(2000) - linrand(2000) * kPitchMultiplierForce 
    kMudFreqs[4] = kMudFreqs[4] + linrand(2000) - linrand(2000) * kPitchMultiplierForce 
    kMudFreqs[5] = kMudFreqs[5] + linrand(2000) - linrand(2000) * kPitchMultiplierForce 
    kMudFreqs[6] = kMudFreqs[6] + linrand(500) - linrand(500) * kPitchMultiplierForce 
    
    aMud1 reson aNoise, kMudFreqs[0] + linrand(100) - linrand(50), kMudFreqs[0]/kMudQs[0] * kMudAmps[0]
    aMud2 reson aNoise, kMudFreqs[1] + linrand(100) - linrand(50), kMudFreqs[1]/kMudQs[1] * kMudAmps[1]
    aMud3 reson aNoise, kMudFreqs[2] + linrand(100) - linrand(50), kMudFreqs[2]/kMudQs[2] * kMudAmps[2]
    aMud4 reson aNoise, kMudFreqs[3] + linrand(100) - linrand(50), kMudFreqs[3]/kMudQs[3] * kMudAmps[3]
    aMud5 reson aNoise, kMudFreqs[4] + linrand(100) - linrand(50), kMudFreqs[4]/kMudQs[4] * kMudAmps[4]
    aMud6 reson aNoise, kMudFreqs[5] + linrand(100) - linrand(50), kMudFreqs[5]/kMudQs[5] * kMudAmps[5]
    aMud7 reson aNoise, kMudFreqs[6] + linrand(100) - linrand(50), kMudFreqs[6]/kMudQs[6] * kMudAmps[6]


    aMudModal = aMud1 + aMud2 + aMud3 + aMud4 + aMud5 + aMud6 + aMud7

    aMudModalEnhanced = aMudModal 

    aMudCombined = aMudModalEnhanced 

    kMudRMS rms aMudCombined
    kSafeMudRMS = max(kMudRMS, 0.00001)
    kTargetMudRMS = 0.5
    aMud = aMudCombined * (kTargetMudRMS / kSafeMudRMS)
 
    aMud limit aMud, -1, 1
    
    iStartFreq = 200 + linrand(50)
    iEndFreq = iStartFreq + (linrand(10000))
    iDuration = 1 + linrand(5) ; Duration of the event is a random value between 5 and 10
    
    kSweepFreq line iStartFreq, iEndFreq, iDuration
    aMudSweep butlp aMud, kSweepFreq
    
    aHP buthp aMud, 50  ; High-pass filter starting at 50 Hz

    
    aMud = aMud * aMudSweep

endif


if iSnowMix > 0 then
    
    iForce chnget "stepForce"
    iRoughness chnget "roughness"
    
    iForce = 0.01 + iForce * 0.98
    iRoughness = 0.01 + iRoughness * 0.98
    iRoughness = iRoughness * 0.004

    ; Roughness Modulation
    aFilteredNoise butlp aNoise, 200
    kVCOFreq = aFilteredNoise * (iForce * 20000 * iRoughness)
    aVCOOut oscili 1, kVCOFreq
        
    ; Snow Modal Synthesis Parameters
    kSnowFreqs[] fillarray 3000, 3500, 4000, 6000, 8000, 11000, 14000  ; Start with higher frequencies
    kSnowFreqs = kSnowFreqs * kPitchMultiplierForce
    kSnowQs[] fillarray 3, 3, 3, 4, 8, 8, 16  ; Slightly broader frequency response
    kSnowAmp = 0.1 

    aSnow1 reson aNoise, kSnowFreqs[0], kSnowFreqs[0]/kSnowQs[0]
    aSnow2 reson aNoise, kSnowFreqs[1], kSnowFreqs[1]/kSnowQs[1]
    aSnow3 reson aNoise, kSnowFreqs[2], kSnowFreqs[2]/kSnowQs[2]
    aSnow4 reson aNoise, kSnowFreqs[3], kSnowFreqs[3]/kSnowQs[3]
    aSnow5 reson aNoise, kSnowFreqs[4], kSnowFreqs[4]/kSnowQs[4]
    aSnow6 reson aNoise, kSnowFreqs[5], kSnowFreqs[5]/kSnowQs[5]
    aSnow7 reson aNoise, kSnowFreqs[6], kSnowFreqs[6]/kSnowQs[6]
    aSnowModal = (aSnow1 + aSnow2 + aSnow3 + aSnow4 + aSnow5 + aSnow6 + aSnow7) * kSnowAmp
    
    aSnowModal = aSnowModal * (1 + aVCOOut)
    
    kNoiseFilterFreq = 6500  
    kNoiseFilterBW = 1500     
    kNoiseAmp = 0.05 * iForce 

    aFilteredSnowNoise reson aNoise, kNoiseFilterFreq, kNoiseFilterBW 
    aFilteredSnowNoise = aFilteredSnowNoise * kNoiseAmp              

    aSnowModalEnhanced = aSnowModal + aFilteredSnowNoise

    aSnowCombined = aSnowModalEnhanced * iForce

    kSnowRMS rms aSnowCombined
    kSafeSnowRMS = max(kSnowRMS, 0.00001)
    kTargetSnowRMS = 0.1
    aSnow = aSnowCombined * (kTargetSnowRMS / kSafeSnowRMS)
 
    aSnow limit aSnow, -1, 1
    
    aSnowHP buthp aSnow, max(500, 1000 * iRoughness) 

    
    aSnow = aSnow * aSnowHP

endif

    
if iWater > 0 then
   iForce chnget "stepForce"
   iRoughness chnget "roughness"
   
    iFreqMultiplier = pow(2, (12 * iPitch) / 12)  
    kOriginalRippleFreq = 3  

    kMinRippleFreq = 0.01 
    kRippleFreq = max(kOriginalRippleFreq * iFreqMultiplier, kMinRippleFreq)

    aRipple oscili 0.3, kRippleFreq * 0.4
    aWhiteSplash rand 1
    aSplash = (aNoise + aWhiteSplash) * 0.01
    aWaterSound = (aRipple + aSplash) * iWater
    iSafeSum = max(iSum, 0.00001)
endif

    aOut = 0
    iSumDenominator = 0

    if iGrassMix > 0 then
        aOut += aGrassSound * iGrassMix
        iSumDenominator += iGrassMix
    endif

    if iWoodMix > 0 then
        aOut += aWood * iWoodMix
        iSumDenominator += iWoodMix
    endif

    if iStoneMix > 0 then
        aOut += aStone * iStoneMix
        iSumDenominator += iStoneMix
    endif

    if iMudMix > 0 then
        aOut += aMud * iMudMix
        iSumDenominator += iMudMix
    endif

    if iWater > 0 then
        aOut += aWaterSound * iWater
        iSumDenominator += iWater
    endif
    
    if iSnowMix > 0 then
        aOut += aSnow * iSnowMix
        iSumDenominator += iSnowMix
    endif

    ; Avoid division by zero
    if iSumDenominator != 0 then
        aOut = aOut / iSumDenominator
    else
        aOut = 0
    endif

    aEnv madsr iAttack, iDecay, iSustain, iRelease
    aOut *= aEnv

    kLowFreq chnget "bassFreq"
    kMidFreq chnget "midFreq"
    kHighFreq chnget "trebleFreq"

    kLowGainRaw chnget "bassEQ"
    kMidGainRaw chnget "midEQ"
    kHighGainRaw chnget "trebleEQ"
    
    kEQBandwidth chnget "eqBandwidth"
    
    kLowGain = kLowGainRaw + 1
    kMidGain = kMidGainRaw + 1
    kHighGain = kHighGainRaw + 1

    kBandwidth = kEQBandwidth * 1000 + 0.1

    aTemp1 eqfil aOut, kLowFreq, kBandwidth, kLowGain
    aTemp2 eqfil aTemp1, kMidFreq, kBandwidth, kMidGain
    aOut eqfil aTemp2, kHighFreq, kBandwidth, kHighGain

    aOut limit aOut, -1, 1
    aOut *= 0.9

    outs aOut, aOut

endin

</CsInstruments>
<CsScore>
f1 0 16384 10 1 ; Sine Wave
i "UpdateMixer" 0 z 
i "DetectTriggers" 0 z 

</CsScore>
</CsoundSynthesizer>
