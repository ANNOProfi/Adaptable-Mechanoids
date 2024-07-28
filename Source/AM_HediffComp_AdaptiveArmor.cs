using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_Hediff_AdaptiveArmor : Hediff
    {
        private readonly AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

        private HediffStage curStage;

        public override HediffStage CurStage
        {
            get
            {
                if(curStage == null)
                {
                    if(component.mechList.Contains(this.pawn.def.defName))
                    {
                        StatModifier armorBlunt = new StatModifier();
                        armorBlunt.stat = StatDefOf.ArmorRating_Blunt;
                        armorBlunt.value = component.mechArmorList[pawn.def.defName].armorOffsets[AM_DefOf.Blunt];

                        StatModifier armorSharp = new StatModifier();
                        armorSharp.stat = StatDefOf.ArmorRating_Sharp;
                        armorSharp.value = component.mechArmorList[pawn.def.defName].armorOffsets[DamageArmorCategoryDefOf.Sharp];

                        StatModifier armorHeat = new StatModifier();
                        armorHeat.stat = StatDefOf.ArmorRating_Heat;
                        armorHeat.value = component.mechArmorList[pawn.def.defName].armorOffsets[AM_DefOf.Heat];

                        curStage = new HediffStage();
                        curStage.statOffsets = new List<StatModifier>{armorBlunt, armorSharp, armorHeat};
                    }
                }
                return curStage;
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            //Making triple sure pawn is a hostile mech
            if(!this.pawn.Dead && this.pawn.RaceProps.IsMechanoid && !this.pawn.IsColonyMech && !this.pawn.Faction.def.humanlikeFaction)
            {
                component.mechArmorList[pawn.def.defName].damageAmounts[dinfo.Def.armorCategory] += totalDamageDealt;
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            //Registering new mech type
            if(!component.mechList.Contains(this.pawn.def.defName) && this.pawn.RaceProps.IsMechanoid && !this.pawn.IsColonyMech && !this.pawn.Faction.def.humanlikeFaction)
            {
                component.mechArmorList.Add(this.pawn.def.defName, new AM_MechArmorStats(pawn.GetStatValue(StatDefOf.ArmorRating_Blunt), pawn.GetStatValue(StatDefOf.ArmorRating_Sharp), pawn.GetStatValue(StatDefOf.ArmorRating_Heat), pawn.def.defName));
                component.mechList.Add(this.pawn.def.defName);
            }
        }
    }
}