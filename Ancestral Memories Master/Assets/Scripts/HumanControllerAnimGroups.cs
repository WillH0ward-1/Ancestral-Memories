using UnityEngine;
using System.Collections.Generic;
public class HumanControllerAnimGroups : MonoBehaviour
{
    public List<string> Action = new List<string> { "Action_BentKnees_ForageGround", "Action_Fatigue_WipeSweat", "Action_Item_PickUp", "Action_Item_PullFromGround", "Action_KneesToStanding_FinishPray", "Action_OnKnee_ForageGround", "Action_OnKnees_DigAndPlant", "Action_OnKnees_PlantSeeds", "Action_OnKnees_PrayToGround", "Action_OnKnees_PrayToSky", "Action_PetAnimal", "Action_Pose_PickedUpByBack", "Action_PrayTowardsSky_Floating", "Action_Standing_Equip", "Action_Standing_HarvestTree", "Action_Standing_Prayer", "Action_Standing_ScatterSeeds01", "Action_Standing_ScatterSeeds02", "Action_StandingToKnees_ToPrayer", "Action_StandToKnee_HealGround", "Action_Zombie_Bite_Forward", "Action_Zombie_Eat_FromGround" };
    public List<string> Attack = new List<string> { "Attack_AssassinateHuman", "Attack_Kick", "Attack_Neanderthal_Punch01", "Attack_Neanderthal_Punch02", "Attack_PunchHuman", "Attack_RitualExecutionStab", "Attack_Slash01", "Attack_Slash02", "Attack_Slash03", "Attack_Slash04", "Attack_Stab01", "Attack_Stab02", "Attack_Stab03", "Attack_Stomp" };
    public List<string> AttackReact = new List<string> { "AttackReact_Standing_SuckerPunched" };
    public List<string> Climb = new List<string> { "Climb_ClimbUp01", "Climb_ClimbUp02", "Climb_ClimbUp03" };
    public List<string> Climbing = new List<string> { "Climbing" };
    public List<string> Dance = new List<string> { "Dance_Egyptian", "Dance_HandsInTheAir", "Dance_Rambunctious", "Dance_Shuffle", "Dance_ShuffleRun", "Dance_Skipping01", "Dance_Skipping02", "Dance_SuperShuffle", "Dance_Vibe" };
    public List<string> Death = new List<string> { "Death_Pose_EmbraceDeath", "Death_Standing_Assassinated", "Death_Standing_Electrocution", "Death_Standing_FallBackwards", "Death_Standing_FallForwards01", "Death_Standing_FallForwards02", "Death_Standing_Insanity" };
    public List<string> Emotion = new List<string> { "Emotion_Angry_FrustratedMax", "Emotion_Angry_FrustratedMin", "Emotion_Angry_Intimidate", "Emotion_Angry_ThrowMud", "Emotion_Angry_YellAt", "Emotion_Confused_Dazed", "Emotion_Curious_LookAround", "Emotion_Disgust_CoverEars", "Emotion_Disgust_CoverFace", "Emotion_Elder_Clapping", "Emotion_Elder_ListenCloser", "Emotion_Fatigue", "Emotion_Happy_Cheering", "Emotion_Happy_Glee", "Emotion_MidSapien_Confusion", "Emotion_Neaderthal_Happy_Clapping", "Emotion_Sapien_Happy_Clapping", "Emotion_Scared_Anxious", "Emotion_Scared_LookAround" };
    public List<string> Falling = new List<string> { "Falling_BadTrip_Flailing", "Falling_GoodTrip_Floating" };
    public List<string> FallToGroundBack = new List<string> { "FallToGroundBack_BlownAway", "FallToGroundBack_BlownAwayAwe", "FallToGroundBack_KnockedBack", "FallToGroundBack_KnockedOut01", "FallToGroundBack_KnockedOut02", "FallToGroundBack_KnockedOut03", "FallToGroundBack_KnockedOut04" };
    public List<string> Idle = new List<string> { "Idle_Crouch", "Idle_Elder", "Idle_MidSapien01", "Idle_MidSapien02", "Idle_MidSapien03", "Idle_Neanderthal", "Idle_Neanderthal_ItchSelf01", "Idle_Neanderthal_ItchSelf02", "Idle_Sad", "Idle_Sapien_InspectSelf01", "Idle_Sapien_InspectSelf02", "Idle_Sapien01", "Idle_Sapien02", "Idle_Sapien03" };
    public List<string> KneesToStand = new List<string> { "KneesToStand_StandUp" };
    public List<string> OnBack = new List<string> { "OnBack_Death_Stabbed", "OnBack_FranticCrawlBackwards", "OnBack_GetUp01", "OnBack_GetUp02", "OnBack_Scared_LookAround", "OnBack_Seizure", "OnBack_Sleeping", "OnBack_WakeUp" };
    public List<string> OnFloor = new List<string> { "OnFloor_Dazed" };
    public List<string> OnKnees = new List<string> { "OnKnees_Crawl_Critical", "OnKnees_Crawl_Medium" };
    public List<string> Run = new List<string> { "Run_MidSapien_Jog", "Run_Neanderthal_Jog01", "Run_Neanderthal_Jog02", "Run_Sapien_Jog", "Run_Sapien_Sprint", "Run_Scared", "Run_Scared_Terrified" };
    public List<string> Seat = new List<string> { "Seat_Sitting_Death_Seizure", "Seat_Sitting_Distress", "Seat_Sitting_LaughingMax", "Seat_Sitting_LaughingMid", "Seat_Sitting_LaughingMin", "Seat_StandToSit_Exhausted", "Seat_StandToSit_Neutral" };
    public List<string> Sitting = new List<string> { "Sitting_OnGround_Casually", "Sitting_OnGround_Meditate" };
    public List<string> Sleeping = new List<string> { "Sleeping_Disturb_RollOver", "Sleeping_Idle" };
    public List<string> Swimming = new List<string> { "Swimming" };
    public List<string> Talk = new List<string> { "Talk_TellStory01", "Talk_TellStory02" };
    public List<string> Torch = new List<string> { "Torch_Attack", "Torch_LightFire_FromFront", "Torch_LightFire_FromGround" };
    public List<string> Walk = new List<string> { "Walk_CarryFront", "Walk_Crouch01", "Walk_Crouch02", "Walk_Crouch03", "Walk_Elderly", "Walk_Intoxicated", "Walk_MidSapien_Hungry", "Walk_MidSapien01", "Walk_MidSapien02", "Walk_MidSapien03", "Walk_MidSapien04", "Walk_MidSapien05", "Walk_Neanderthal01", "Walk_Neanderthal02", "Walk_Sad", "Walk_Sapien", "Walk_Scared", "Walk_Sneak" };
    public List<string> Zombie = new List<string> { "Zombie_Crawling" };
}
