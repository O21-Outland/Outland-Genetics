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

        public static HediffDef Outland_EndogermLossShock;
        public static HediffDef Outland_EndogermReplicating;
        public static HediffDef Outland_EndogerminationComa;
        public static HediffDef Outland_XenotypeAscender;
        public static HediffDef Outland_EggPouch;
        public static HediffDef Outland_EggFatigue;

        public static GeneDef Outland_CalmingPheromones;
        public static GeneDef Outland_DarklightAffinity;
        public static GeneDef Outland_KindEyes;
        public static GeneDef Outland_FamiliarScent;
        public static GeneDef Outland_EggLayer;
        public static GeneDef Outland_AllMale, Outland_AllFemale;

        public static SoundDef ReimplantXenogerm_Cast;

        public static EffecterDef Implant_Xenogerm;

        public static JobDef CastAbilityOnThingMelee;
        public static JobDef Outland_LayEgg;
        public static JobDef Outland_FrozenInIce;

        public static ThingCategoryDef Outland_HumanoidEggs;

        public static DamageDef Outland_IceFreeze;
    }
}
