SHAMANIC
SYNESTHESIA
Technical Sound Design and Implementation
Audio For Games 2022

// ============================================================================================================================================

You’ll immediately notice the disparity between the file names ‘Ancestral Memories’ and the title of the game. ‘Ancestral Memories’ was a working title, and in a bid to not potentially mess anything up before the submission, it hasn’t been renamed. (This has caused problems in the past).

// ============================================================================================================================================

Shamanic SYNESTHESIA is a novel, audio-focused ambient environment in which a natural ecosystem deteriorates or blooms as the result of player-driven actions. The theme of the game is a synthesis of Neanderthalic, Shamanic and Biblical themes. The graphical style utilises warped graphics to evoke and embrace certain distinct imperfections of early-1990’s 3D, while not strictly adhering to their limitations. The audio asset design also considers this aesthetic. The world contains a variety of trees and plants with individual growth cycles of life and death. The player may play their shell-flute, which plays alongside the generative music composition. The world also contains interactable NPC’s each with their own distinct audio design and implementation methods.

// ============================================================================================================================================

Opening the project:

Recommended Versions:

Unity: 2022.2.5f1
Unity FMOD Integration: 2.02.11
FMOD Studio: 2.02.11

When the project is opened, you will receive the following warning regarding the Input System:

Select ‘No’. This project uses the old input system.

When you open the project, you will need to open the relevant scene. The name of the scene is ‘MainIsland’.


You’ll find the FMOD Studio project in the Master directory, in the same place you find the ‘Assets’ and ‘Library’ folder

// ============================================================================================================================================

Disclaimers:

	In the editor, the audio quite often loads every other time the game is loaded. If there is no sound upon loading, just stop play mode and then hit play again.

	Stopping in play mode may produce overly loud sounds

	Using the flute inside the cave reliably causes the player movement to break. Think this is some strange Boolean issue where the movement input goes out of sync as it’s disabled/enabled during ‘Flute mode’.

	Generative music may sometimes fail to update the musical mode and get stuck in one mode (rare, possibly related to using the editor/other applications simultaneously)

	Because of the unique camera type (reverse perspective cam), selecting things can be unreliable as the frustrum dilates as the player rotates the mouse wheel, which affects the scripts responsible for selecting items. A custom camera projection matrix fed to the RayCast would be required to fix this, this is beyond the scope of this project.

	The full development of this game spanned approximately 7 months. I was as involved with the entire codebase as I possibly could be. Its development brought about many technical challenges and learning curves that required extensive research in all manner of areas of game development to overcome them. 

	In the StudioEventEmitter class (FMOD script) – a modification has been made to check if an activeEmitter is null before updating its playing status. This was causing a really bad issue before, I think to do with the execution of my emitter generation. This fixed it mostly, as it’s far more reliable now than it was (No sound would work until Unity was opened/re-opened.)

	The method to change the footstep parameter is quite confusing, as there are two separate methods. One uses ray casting, and the other uses the AreaManager to set directly via a ‘currentRoom’ variable. 

// ============================================================================================================================================

Notable assets/resources:

	Sebastian Lague’s Random Terrain Generation Series.
The Map Generator script is entirely written by Sebastian Lague, this project is in fact a fork of Episode 21 of Sebastian Lague’s Procedural Landmass Generation, everything is built on top:
https://github.com/SebLague/Procedural-Landmass-Generation/tree/master/Proc%20Gen%20E21. Adaptations were made where required

	Gregory Schlomoff’s Poisson Disc sampling method. http://gregschlom.com/devlog/2014/06/29/Poisson-disc-sampling-Unity.html
Found in ‘PoissonDiscSampler.cs’

	SharpCoderBlog’s Deer AI:
https://sharpcoderblog.com/blog/unity-3d-deer-ai-tutorial
This was the base for the Animal AI script, heavily modified in the ‘AnimalAI’ script to fit the needs of the project:

	Alessandro Fama’s website for very useful FMOD API examples:  
Fisher-Yates Shuffle algorithm – implanted in ‘MusicManager’ script –
https://alessandrofama.com/tutorials/fmod/unity/shuffle-playlist

	The FMOD forum was invaluable in formulating solutions
https://qa.fmod.com/

// ============================================================================================================================================


