using System;
using RimWorld;
using Verse;
using CF;

namespace AdaptableMechanoids
{
    public class AM_HediffComp_OffsetOtherHediff : HediffComp, IHediffComp_OnHediffAdded
    {
        public AM_HediffCompProperties_OffsetOtherHediff Props => (AM_HediffCompProperties_OffsetOtherHediff)props;

        private AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

        public void OnHediffAdded(ref Hediff hediff)
        {
            if(Props.affectsNewHediffs)
            {
                Log.Message("Trying to modify hediffs");
                TryAdjustOtherHediff(hediff);
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            if(!component.mechList.ContainsKey(this.Pawn.def.defName))
            {
                Log.Message("Generating new entry");
                component.mechList.Add(this.Pawn.def.defName, new AM_MechArmorStats(Pawn.GetStatValue(StatDefOf.ArmorRating_Blunt), Pawn.GetStatValue(StatDefOf.ArmorRating_Sharp), Pawn.GetStatValue(StatDefOf.ArmorRating_Heat), Pawn.def.defName));
            }

            if(!Props.affectsExistingHediffs)
            {
                return;
            }

            foreach (Hediff otherHediff in Pawn.health.hediffSet.hediffs)
            {
                Log.Message("Trying to modify hediffs");
                TryAdjustOtherHediff(otherHediff);
            }
        }

        private void TryAdjustOtherHediff(Hediff other)
        {
            if(other == parent)
            {
                return;
            }
            
            if(Props.affectedHediffs.Contains(other.def))
            {
                Log.Message("Modifying armor");
                component.mechList[Pawn.def.defName].CalculateArmor();
                foreach (StatModifier modifier in other.CurStage.statOffsets)
                {
                    if(modifier.stat == StatDefOf.ArmorRating_Blunt)
                    {
                        //modifier.value = component.mechList[Pawn.def.defName].Armor_Blunt;
                        modifier.value = component.mechList[Pawn.def.defName].armorOffsets[AM_DefOf.Blunt];
                    }
                    else if(modifier.stat == StatDefOf.ArmorRating_Sharp)
                    {
                        //modifier.value = component.mechList[Pawn.def.defName].Armor_Sharp;
                        modifier.value = component.mechList[Pawn.def.defName].armorOffsets[DamageArmorCategoryDefOf.Sharp];
                    }
                    else if(modifier.stat == StatDefOf.ArmorRating_Heat)
                    {
                        //modifier.value = component.mechList[Pawn.def.defName].Armor_Heat;
                        modifier.value = component.mechList[Pawn.def.defName].armorOffsets[AM_DefOf.Heat];
                    }
                }
                Log.Message("Modification complete");
            }
        }
    }
}