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
                TryAdjustOtherHediff(hediff);
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            if(!Props.affectsExistingHediffs)
            {
                return;
            }

            foreach (Hediff otherHediff in Pawn.health.hediffSet.hediffs)
            {
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
                foreach (StatModifier modifier in other.CurStage.statOffsets)
                {
                    if(modifier.stat == StatDefOf.ArmorRating_Blunt)
                    {
                        modifier.value = component.mechList[Pawn.def.defName].Armor_Blunt;
                    }
                    else if(modifier.stat == StatDefOf.ArmorRating_Sharp)
                    {
                        modifier.value = component.mechList[Pawn.def.defName].Armor_Sharp;
                    }
                    else if(modifier.stat == StatDefOf.ArmorRating_Heat)
                    {
                        modifier.value = component.mechList[Pawn.def.defName].Armor_Heat;
                    }
                }
            }
        }
    }
}