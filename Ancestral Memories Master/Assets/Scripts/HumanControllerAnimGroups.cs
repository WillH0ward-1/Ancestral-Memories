using UnityEngine;
using System.Collections.Generic;
public class HumanControllerAnimGroups : MonoBehaviour
{
    public List<string> Action = new List<string> { "Action_EmbraceDeath", "Action_HarvestTree", "Action_Pose_PickedUpByBack", "Action_Pose_PrayUpStandingFloating", "Action_Pray_LoopStanding", "Action_Sapien_PetAnimal", "Action_WipeSweat" };
    public List<string> Attack = new List<string> { "Attack_ExecuteRitualStab", "Attack_Kick", "Attack_NeanderthalicSlash", "Attack_NeanderthalicSmash", "Attack_Slash01", "Attack_Slash02", "Attack_Slash03", "Attack_Slash04", "Attack_Stab01", "Attack_Stab02", "Attack_Stab03", "Attack_StompOnGround" };
    public List<string> Attacked = new List<string> { "Attacked_Punched" };
    public List<string> Climb = new List<string> { "Climb_Climbing", "Climb_ClimbUp01", "Climb_ClimbUp02", "Climb_ClimbUp03" };
    public List<string> Crawl = new List<string> { "Crawl_CriticalHealth", "Crawl_LowHealth", "Crawl_OnBack_Backwards" };
    public List<string> Dance = new List<string> { "Dance_Cheering", "Dance_Egyptian", "Dance_HandsInAir", "Dance_Rambunctious", "Dance_Shuffle", "Dance_SpotSkip01", "Dance_SpotSkip02", "Dance_Stepping", "Dance_SuperShuffle", "Dance_Vibe" };
    public List<string> Death = new List<string> { "Death_Insanity", "Death_Standing_AssasinatedFromFront", "Death_Standing_Electrocution", "Death_Standing_FallFront01", "Death_Standing_FallFront02", "Death_Standing_FlyBackwards", "Death_Standing_OldAge_FallBack", "Death_Standing_OldAgeFallFront" };
    public List<string> Emotion = new List<string> { "Emotion_Angry_Arguing", "Emotion_Angry_BattleCry", "Emotion_Angry_Frustrated01", "Emotion_Angry_Frustrated02", "Emotion_Angry_Intimidate", "Emotion_Angry_ThrowMud", "Emotion_Angry_YellTowards ", "Emotion_Curious", "Emotion_Curious_LookAround", "Emotion_Disgust_FliesDisgust", "Emotion_Elder_Curious", "Emotion_Happy_BriefGlee", "Emotion_Neanderthal_Happy_Clap", "Emotion_Sapien_Confused", "Emotion_Sapien_Happy_Clap", "Emotion_Scared_Anxious", "Emotion_Scared_LookAround" };
    public List<string> Falling = new List<string> { "Falling_BadTripFlailing", "Falling_GoodTripFalling" };
    public List<string> Idle = new List<string> { "Idle_Crouch", "Idle_Elderly", "Idle_Intoxicated01", "Idle_MidSapien_LookAround", "Idle_MidSapien_LookAround02", "Idle_MidSapien01", "Idle_Neanderthal_ItchSelf02", "Idle_Neanderthal_ItchSelf03", "Idle_Neanderthal01", "Idle_Neanderthalic", "Idle_RunningFatigue", "Idle_Sad", "Idle_Sapien_LookAround01", "Idle_Sapien_LookAround02", "Idle_Sapien01", "Idle_Sapien02", "Idle_Sapien03", "Idle_SittingCasually", "Idle_SittingMeditating" };
    public List<string> Item = new List<string> { "Item_ForageGround", "Item_PickUp", "Item_RightHand_Equip", "Item_RightHand_Sheathe", "Item_ScatterSeeds", "Item_Torch_Attack", "Item_Torch_LightFromCampfire", "Item_Torch_LightOnFire", "Item_WaterPlants" };
    public List<string> Knees = new List<string> { "Knees_DigAndPlant", "Knees_KneelToHeal", "Knees_KneesToStand", "Knees_OnOneKnee_Forage", "Knees_PrayToGround", "Knees_PrayToSky", "Knees_StandToKnees" };
    public List<string> OnBack = new List<string> { "OnBack_Convulsing", "OnBack_Death_Stabbed", "OnBack_GetUpFromBack_Mid", "OnBack_GetUpFromBack_Min", "OnBack_ScaredLookAround" };
    public List<string> OnGround = new List<string> { "OnGround_Dazed" };
    public List<string> Run = new List<string> { "Run_JoggingSapien01", "Run_JoggingSapien02", "Run_Neanderthalic01", "Run_Neanderthalic02", "Run_Sneak_SapienImmortal", "Run_SprintingSapien01" };
    public List<string> Running = new List<string> { "Running_TerrifiedPanic", "Running_TerrifiedYelling" };
    public List<string> Sit = new List<string> { "Sit_Death_Seizure", "Sit_SitDown_LowEnergy", "Sit_SitDown_StableEnergy", "Sit_Sitting_Distress", "Sit_Sitting_LaughingMax", "Sit_Sitting_LaughingMid", "Sit_Sitting_LaughingMin" };
    public List<string> Sleeping = new List<string> { "Sleeping_BackOnGround_WakeUp", "Sleeping_OnBack_Idle", "Sleeping_Onside_DisturbedRollOver", "Sleeping_OnSide_Idle" };
    public List<string> StandingToBack = new List<string> { "StandingToBack_BlownAway", "StandingToBack_KnockedBack_Shoulder", "StandingToBack_KnockedOut01", "StandingToBack_KnockedOut02", "StandingToBack_KnockedOut03", "StandingToBack_KnockedOut04" };
    public List<string> Swimming = new List<string> { "Swimming" };
    public List<string> Talk = new List<string> { "Talk_TellStory01", "Talk_TellStory02" };
    public List<string> Walk = new List<string> { "Walk_Carry", "Walk_Crouch_Sneak01", "Walk_Crouch_Sneak02", "Walk_Elderly", "Walk_Intoxicated", "Walk_MidSapien01", "Walk_MidSapien02", "Walk_MidSapien03", "Walk_Neanderthalic01", "Walk_Nervously", "Walk_Sad", "Walk_Sapien01", "Walk_Sapien02", "Walk_Sneak", "Walk_Starving" };
    public List<string> Zombie = new List<string> { "Zombie_Crawl", "Zombie_EatFromGround", "Zombie_EatFront" };
}
