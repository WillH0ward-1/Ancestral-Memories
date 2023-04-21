 SHAMANIC
SYNESTHESIA

Technical Sound Design and Implementation
Audio For Games 2022

Shamanic Synesthesia is a novel, audio-rich ambient environment in which a natural ecosystem thrives or dies as the result of the players actions. The theme of the game is a synthesis of Neanderthalic, Shamanic and Biblical themes. The graphical style is a combination of retro aesthetic qualities from various 1990s/Early 2000s 3D graphics (Microsoft DOS,PS1, Dreamcast, N64) while not strictly adhering to one particular look.
The world contains a variety of trees and plants with individual growth cycles of life and death. The player may play their shell-flute, which plays alongside the generative music composition. The world also contains interactable NPC’s each with their own distinct audio design and implementation methods.

Recommended Versions:

Unity: 2022.2.5f1
Unity FMOD Integration: 2.02.11
FMOD Studio: 2.02.11

Across the project you may notice the disparity between the file names ‘Ancestral Memories’ and the title of the game 'Shamanic Synesthesia'. ‘Ancestral Memories’ was a working title, and in a bid to not potentially mess anything up before submitting my work to my uni, it hasn’t been renamed. (This has caused problems in the past).

When the project is opened, you will receive a warning regarding the Input System. Select ‘No’. This project uses the old input system.
When you open the project, you will need to open the relevant scene. The name of the scene is ‘MainIsland’.

You’ll find the FMOD Studio project in the Master directory, in the same place you find the ‘Assets’ and ‘Library’ folder

Disclaimers:

-	The audio reliably loads every other time the game is loaded (when you enter play mode). When the audio isn't present during play mode, restart the game. However, make preparations for sudden spike in volume. All of the sounds that accumulate while the game is running seem to load at once, that means that the longer the game is left running, the more severe this effect. This can be very loud given enough sounds over time. So, in the case you are in play mode and there is no sound, perhaps lower the volume of your device before re-entering edit mode, particularly if wearing headphones.

-	Using the flute inside the cave reliably causes the player movement to break. Think this is some strange Boolean issue where the movement input goes out of sync as it’s disabled/enabled during ‘Flute mode’.

-	Generative music may sometimes fail to update the musical mode and get stuck in one mode (rare, possibly related to using the editor/other applications simultaneously)

-	Because of the unique camera type (reverse perspective cam), selecting things can be unreliable as the frustrum dilates as the player rotates the mouse wheel, which affects the scripts responsible for selecting items. A custom camera projection matrix fed to the RayCast would be required to fix this, this is beyond the scope of this project.

-	In the StudioEventEmitter class (FMOD script) – a modification has been made to check if an activeEmitter is null before updating its playing status. This was causing a really bad issue before, I think to do with the execution of my emitter generation. This fixed it mostly, as it’s far more reliable now than it was (No sound would work until Unity was opened/re-opened.)

-	The method to change the footstep parameter is quite confusing, as there are two separate methods. One uses ray casting, and the other uses the AreaManager to set directly via a ‘currentRoom’ variable. 

Notable assets/resources:

-	Sebastian Lague’s Random Terrain Generation Series.
The Map Generator script is entirely written by Sebastian Lague, this project is in fact a fork of Episode 21 of Sebastian Lague’s Procedural Landmass Generation. As this terrain was not originally made with URP materials, there have been many modifications to control the visual appearance of the mesh using URP Shadergraph.
https://github.com/SebLague/Procedural-Landmass-Generation/tree/master/Proc%20Gen%20E21. Adaptations were made where required

URP shader for the terrain was based on this tutorial!:
https://www.youtube.com/watch?v=uJSxqr3a0cA
(Check out 'Skullborn', the game made by the dev who made this tutorial: https://store.steampowered.com/app/1841200/Skullborn/)

-	Gregory Schlomoff’s Poisson Disc sampling method. http://gregschlom.com/devlog/2014/06/29/Poisson-disc-sampling-Unity.html
Found in ‘PoissonDiscSampler.cs’

-	SharpCoderBlog’s Deer AI:
https://sharpcoderblog.com/blog/unity-3d-deer-ai-tutorial
This was the base for the Animal AI script, heavily modified in the ‘AnimalAI’ script to fit the needs of the project:

-	Alessandro Fama’s website for very useful FMOD API examples:  
Fisher-Yates Shuffle algorithm – implanted in ‘MusicManager’ script –
https://alessandrofama.com/tutorials/fmod/unity/shuffle-playlist

-	The FMOD forum for even more useful API examples
https://qa.fmod.com/


