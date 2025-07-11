using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_Hediff_AdaptiveArmor : Hediff
    {
        private readonly AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

        public HediffStage curStage;

        private bool registered = false;

        private Dictionary<StatDef, float> statModifiers;

        public override HediffStage CurStage
        {
            get
            {
                if(curStage == null)
                {
                    if(registered)
                    {
                        curStage = new HediffStage();
                        curStage.statOffsets = new List<StatModifier>();

                        if(statModifiers == null)
                        {
                            statModifiers = new Dictionary<StatDef, float>();
                        }

                        foreach(AM_ArmorTypes armorTypes in this.def.GetModExtension<AM_AdaptableArmor>().armorTypes)
                        {
                            StatModifier modifier = new StatModifier();
                            modifier.stat = armorTypes.armorType;
                            
                            if(statModifiers.ContainsKey(armorTypes.armorType) == false)
                            {
                                statModifiers.Add(armorTypes.armorType, component.RequestAdaptation(pawn.def.defName, armorTypes.damageType, pawn.Faction.def));
                            }

                            modifier.value = statModifiers[armorTypes.armorType];

                            curStage.statOffsets.Add(modifier);
                        }
                    }
                }
                return curStage;
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if(!registered)
            {
                Log.Warning("AM_Warning: Mech "+ pawn.def.defName +" has requested adaptation before being registered.");
            }

            if(!this.pawn.Dead && (!this.pawn.Faction.def.humanlikeFaction || pawn.Faction.IsPlayer) && totalDamageDealt > 0f)
            {
                component.AddDamage(pawn.def.defName, pawn.Faction.def, dinfo.Def.armorCategory, totalDamageDealt);
            }
        }

        public void Register()
        {
            //Log.Message("AM_Message: Registering "+pawn.def.defName+" of faction "+pawn.Faction.def);
            component.Register(pawn.def.defName, pawn.Faction.def, pawn, def.GetModExtension<AM_AdaptableArmor>());

            registered = true;
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            //Registering new mech type
            if(!registered)
            {
                Register();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref registered, "registered", defaultValue: false);
            Scribe_Collections.Look(ref statModifiers, "statModifiers", LookMode.Def, LookMode.Value);
        }
    }
}