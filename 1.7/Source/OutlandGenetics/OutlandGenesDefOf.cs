using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OutlandGenes
{
    [DefOf]
    public static class OutlandGenesDefOf
    {
        static OutlandGenesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OutlandGenesDefOf));
        }

        public static ThingDef Outland_HumanoidEgg;

        public static ThoughtDef Outland_Empathic;

        public static HediffDef 
            Outland_EndogermLossShock,
            Outland_EndogermReplicating,
            Outland_EndogerminationComa,
            Outland_XenotypeAscender,
            Outland_EggPouch,
            Outland_EggFatigue;

        public static GeneDef 
            Outland_CalmingPheromones,
            Outland_DarklightAffinity,
            Outland_KindEyes,
            Outland_FamiliarScent,
            Outland_EggLayer,
            Outland_AllMale,
            Outland_AllFemale,
            Outland_EasyEarnedAscension,
            Outland_EarnedAscension,
            Outland_HardEarnedAscension;

        public static SoundDef 
            ReimplantXenogerm_Cast;

        public static EffecterDef 
            Implant_Xenogerm;

        public static JobDef 
            CastAbilityOnThingMelee,
            Outland_LayEgg,
            Outland_FrozenInIce;

        public static ThingCategoryDef 
            Outland_HumanoidEggs;

        public static DamageDef 
            Outland_IceFreeze;
    }
}
