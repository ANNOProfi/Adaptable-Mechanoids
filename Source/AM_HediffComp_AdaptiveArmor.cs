using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_Hediff_AdaptiveArmor : Hediff
    {
        private readonly AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

        public HediffStage curStage;

        public override HediffStage CurStage
        {
            get
            {
                if(curStage == null)
                {
                    if(component.mechList.ContainsKey(pawn.IsColonyMech) && component.mechList[pawn.IsColonyMech].Contains(this.pawn.def.defName))
                    {
                        curStage = new HediffStage();
                        curStage.statOffsets = new List<StatModifier>();

                        foreach(AM_ArmorTypes armorTypes in this.def.GetModExtension<AM_AdaptableArmor>().armorTypes)
                        {
                            StatModifier statModifier = new StatModifier();
                            statModifier.stat = armorTypes.armorType;
                            statModifier.value = component.mechFactionList[pawn.IsColonyMech].TryGetValue(pawn.def.defName).armorOffsets[armorTypes.damageType];

                            curStage.statOffsets.Add(statModifier);
                        }
                    }
                }
                return curStage;
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if((AM_Utilities.Settings.adaptAIMech && !AM_Utilities.Settings.adaptColonyMech && pawn.IsColonyMech) || (!AM_Utilities.Settings.adaptAIMech && AM_Utilities.Settings.adaptColonyMech && !pawn.IsColonyMech) || !this.pawn.RaceProps.IsMechanoid)
            {
                pawn.health.RemoveHediff(this);
            }

            if(!this.pawn.Dead && (!this.pawn.Faction.def.humanlikeFaction || pawn.Faction.IsPlayer) && totalDamageDealt > 0f)
            {
                component.mechFactionList[pawn.IsColonyMech].TryGetValue(pawn.def.defName).damageAmounts[dinfo.Def.armorCategory] += totalDamageDealt;
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if((AM_Utilities.Settings.adaptAIMech && !AM_Utilities.Settings.adaptColonyMech && pawn.IsColonyMech) || (!AM_Utilities.Settings.adaptAIMech && AM_Utilities.Settings.adaptColonyMech && !pawn.IsColonyMech) || !this.pawn.RaceProps.IsMechanoid)
            {
                pawn.health.RemoveHediff(this);
            }
            //Registering new mech type
            if(!component.mechFactionList.ContainsKey(pawn.IsColonyMech))
            {
                component.mechFactionList.Add(pawn.IsColonyMech, new Dictionary<string, AM_MechArmorStats>());
            }

            if(!component.mechList.ContainsKey(pawn.IsColonyMech))
            {
                component.mechList.Add(pawn.IsColonyMech, new HashSet<string>());
            }

            if(!component.mechList[pawn.IsColonyMech].Contains(this.pawn.def.defName) && this.pawn.RaceProps.IsMechanoid)
            {
                component.mechFactionList[pawn.IsColonyMech].Add(pawn.def.defName, new AM_MechArmorStats(this.pawn, this.def.GetModExtension<AM_AdaptableArmor>()));
                component.mechList[pawn.IsColonyMech].Add(this.pawn.def.defName);
            }
        }
    }
}