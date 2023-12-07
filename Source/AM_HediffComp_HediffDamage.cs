using System;
using RimWorld;
using Verse;
using AthenaFramework;
using CF;

namespace AdaptableMechanoids
{
    public class AM_HediffComp_HediffDamage : HediffComp, IDamageResponse
    {
        private AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

        public virtual void PreApplyDamage(ref DamageInfo dinfo, ref bool absorbed)
        {
            if(!this.Pawn.Dead && !absorbed && (this.Pawn.RaceProps.IsMechanoid && !this.Pawn.IsColonyMech))
            {
                component.mechList[Pawn.def.defName].damageInstances[dinfo.Def.armorCategory]++;
                component.mechList[Pawn.def.defName].damageAmounts[dinfo.Def.armorCategory] += dinfo.Amount;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            if(!component.mechList.ContainsKey(this.Pawn.def.defName))
            {
                component.mechList.Add(this.Pawn.def.defName, new AM_MechArmorStats(Pawn.GetStatValue(StatDefOf.ArmorRating_Blunt), Pawn.GetStatValue(StatDefOf.ArmorRating_Sharp), Pawn.GetStatValue(StatDefOf.ArmorRating_Heat), Pawn.def.defName));
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            AthenaCache.AddCache(this, ref AthenaCache.responderCache, Pawn.thingIDNumber);
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            AthenaCache.RemoveCache(this, AthenaCache.responderCache, Pawn.thingIDNumber);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                AthenaCache.AddCache(this, ref AthenaCache.responderCache, Pawn.thingIDNumber);
            }
        }
    }
}