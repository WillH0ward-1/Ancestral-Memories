<CsoundSynthesizer>
<CsOptions>
-n -d 
</CsOptions>
<CsInstruments>
; Initialize the global variables. 
ksmps = 32
nchnls = 2
0dbfs = 1

instr 1 ; Formant Voice instrument
    iFreq1 chnget "iFreq1"
    iFreq2 chnget "iFreq2"
    iFreq3 chnget "iFreq3"
    iBW1 chnget "iBW1"
    iBW2 chnget "iBW2"
    iBW3 chnget "iBW3"

    ; If not provided, default to previous values
    if (iBW1 == 0) then
        iBW1 = 80
    endif

    if (iBW2 == 0) then
        iBW2 = 100
    endif

    if (iBW3 == 0) then
        iBW3 = 120
    endif

    ; Simple envelope for our sound
    kEnv linsegr 0, 0.1, 1, 0.8, 0.7, 0.1, 0

    ; Here we'll use resonant filters to simulate the formants
    aSig white 1    ; White noise source

    aF1 reson aSig, iFreq1, iBW1
    aF2 reson aSig, iFreq2, iBW2
    aF3 reson aSig, iFreq3, iBW3

    aMix = (aF1 + aF2 + aF3) * kEnv

    outs aMix, aMix
endin

</CsInstruments>
<CsScore>
i1 0 10
</CsScore>
</CsoundSynthesizer>
