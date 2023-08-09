using UnityEngine;
using System.Collections.Generic;
public class HumanControllerAnimGroups : MonoBehaviour
{
    public List<string> Action = new List<string> { "Action_EmbraceDeath", "Action_Pray_LoopStanding", "Action_PrayTowardSkyStanding", "Action_Sapien_PetAnimal", "Action_SitDown_LowEnergy", "Action_SitDown_StableEnergy", "Action_WipeSweat" };
    public List<string> Attack = new List<string> { "Attack_ExecuteRitualStab", "Attack_Kick", "Attack_NeanderthalicSlash", "Attack_Slash01", "Attack_Slash02", "Attack_Slash03", "Attack_Stab01", "Attack_Stab02", "Attack_StompOnGround" };
    public List<string> Climb = new List<string> { "Climb_ClimbUp01", "Climb_ClimbUp02", "Climb_ClimbUp03" };
    public List<string> Climbing = new List<string> { "Climbing" };
    public List<string> Crawl = new List<string> { "Crawl_CriticalHealth", "Crawl_LowHealth", "Crawl_OnBack_Backwards" };
    public List<string> Dance = new List<string> { "Dance_Cheering", "Dance_Egyptian", "Dance_Rambunctious", "Dance_SpotSkip01", "Dance_SpotSkip02", "Dance_Stepping", "Dance_Vibe" };
    public List<string> Death = new List<string> { "Death_AssasinationFromFront", "Death_Electrocution", "Death_Insanity", "Death_OldAge_FallBack", "Death_OldAge_FallFront" };
    public List<string> Emotion = new List<string> { "Emotion_Angry_BattleCry", "Emotion_Angry_Intimidate", "Emotion_Angry_ThrowMud", "Emotion_Angry_YellTowards ", "Emotion_Arguing", "Emotion_Curious", "Emotion_Curious_LookAroundSapien", "Emotion_Elder_StandingListenCloser", "Emotion_Happy_BriefGlee", "Emotion_Happy_ClapNeanderthalic", "Emotion_Happy_ClapSapien", "Emotion_Sapien_Confused", "Emotion_Scared_Anxious02", "Emotion_Scared_LookAroundSapien" };
    public List<string> Falling = new List<string> { "Falling_BadTripFlailing", "Falling_GoodTripFalling" };
    public List<string> GroundFaceUp = new List<string> { "GroundFaceUp_Convulsing", "GroundFaceUp_StandUp" };
    public List<string> Idle = new List<string> { "Idle_Crouch", "Idle_Elderly", "Idle_Intoxicated01", "Idle_MidSapien_LookAround", "Idle_MidSapien_LookAround02", "Idle_Neanderthal_ItchSelf02", "Idle_Neanderthal_ItchSelf03", "Idle_Neanderthalic", "Idle_Neanderthalic02", "Idle_RunningFatigue", "Idle_Sad", "Idle_Sapien01", "Idle_Sapien02", "Idle_Sapien03", "Idle_SittingCasually", "Idle_SittingMeditating" };
    public List<string> Item = new List<string> { "Item_ForageGround", "Item_PickUp", "Item_RightHand_Equip", "Item_RightHand_Sheathe", "Item_ScatterSeeds", "Item_Torch_Attack", "Item_Torch_LightFromCampfire", "Item_Torch_LightOnFire", "Item_WaterPlants" };
    public List<string> Knees = new List<string> { "Knees_DigAndPlant", "Knees_KneesToStand", "Knees_PrayToGround", "Knees_PrayToSky" };
    public List<string> OnGround = new List<string> { "OnGround_Intoxicated" };
    public List<string> Pain = new List<string> { "Pain_InsanityStanding" };
    public List<string> Pose = new List<string> { "Pose_PickedUpByBack" };
    public List<string> Reaction = new List<string> { "Reaction_BlownBackAwe", "Reaction_KnockedBackShoulder", "Reaction_Knockout01" };
    public List<string> Run = new List<string> { "Run_JoggingSapien01", "Run_JoggingSapien02", "Run_Neanderthalic01", "Run_Neanderthalic02", "Run_Sneak_SapienImmortal" };
    public List<string> Running = new List<string> { "Running_TerrifiedPanic", "Running_TerrifiedYelling" };
    public List<string> Sitting = new List<string> { "Sitting_LaughingHigh", "Sitting_LaughingLow", "Sitting_LaughingMedium" };
    public List<string> Sleeping = new List<string> { "Sleeping_BackOnGround", "Sleeping_Disturbance", "Sleeping_Idle" };
    public List<string> StandToKnees = new List<string> { "StandToKnees" };
    public List<string> Swimming = new List<string> { "Swimming" };
    public List<string> Talk = new List<string> { "Talk_TellStory01", "Talk_TellStory02" };
    public List<string> Walk = new List<string> { "Walk_Carry", "Walk_Crouch_Sneak01", "Walk_Crouch_Sneak02", "Walk_Elderly01", "Walk_Intoxicated", "Walk_MidSapien01", "Walk_MidSapien02", "Walk_Neanderthalic01", "Walk_Nervously", "Walk_Sad", "Walk_SapienImmortal01", "Walk_SapienImortal02", "Walk_Sneak", "Walk_Starving" };
    public List<string> Zombie = new List<string> { "Zombie_Crawl", "Zombie_EatFromGround", "Zombie_EatFront" };
}
