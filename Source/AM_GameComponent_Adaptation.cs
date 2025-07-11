using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_GameComponent_Adaptation : GameComponent
    {
        //public Dictionary<bool, Dictionary<string, AM_MechArmorStats>> mechFactionList = new Dictionary<bool, Dictionary<string, AM_MechArmorStats>>();

        //public Dictionary<bool, HashSet<string>> mechList = new Dictionary<bool, HashSet<string>>();

        public Dictionary<string, AM_MechArmorSorting> mechList = new Dictionary<string, AM_MechArmorSorting>();

        private int ticksToUpdate = AM_Utilities.Settings.adaptationTime;

        private bool useHeatInitialised = false;

        private bool useMaxInitialised = false;

        public AM_GameComponent_Adaptation(Game game)
        {
        }

        public void Register(string name, FactionDef mechFaction, Pawn pawn, AM_AdaptableArmor armorTypes)
        {
            if(mechList.ContainsKey(name))
            {
                if(mechList[name].CheckMech(mechFaction) == null)
                {
                    mechList[name].MakeNewMech(pawn, armorTypes, mechFaction);
                }
            }
            else
            {
                mechList.Add(name, new AM_MechArmorSorting());

                mechList[name].MakeNewMech(pawn, armorTypes, mechFaction);
            }
        }

        public float RequestAdaptation(string name, DamageArmorCategoryDef armor, FactionDef mechFaction)
        {
            if(mechList.ContainsKey(name))
            {
                return mechList[name].CheckMech(mechFaction).armorOffsets[armor];
            }
            else
            {
                Log.Warning("AM_Warning: Adaptation requested for "+ name +" before mech could be registered.");
                return 0f;
            }
        }

        public void AddDamage(string name, FactionDef mechFaction, DamageArmorCategoryDef damage, float damageAmount)
        {
            if(mechList.ContainsKey(name))
            {
                mechList[name].AddDamage(mechFaction, damage, damageAmount);
            }
            else
            {
                Log.Warning("AM_Warning: Mech "+ name +" tried to add damage before being registered");
            }
        }

        public void ResetArmor(bool debug)
        {
            foreach(string name in mechList.Keys)
            {
                mechList[name].ResetArmor(debug);
            }
        }

        public void ResetMax()
        {
            foreach(string name in mechList.Keys)
            {
                mechList[name].ResetMax();
            }
        }

        public void CalculateArmor(bool hardmode)
        {
            foreach(string name in mechList.Keys)
            {
                mechList[name].AdaptMechs(hardmode);
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if(ticksToUpdate != 0)
            {
                ticksToUpdate--;
                return;
            }
            else
            {
                //Resetting if heat armor is enabled mid-game
                if(!useHeatInitialised && AM_Utilities.Settings.useHeat)
                {
                    useHeatInitialised = true;

                    ResetArmor(false);
                }

                if(!useMaxInitialised && AM_Utilities.Settings.useMax)
                {
                    useMaxInitialised = true;

                    ResetMax();
                }

                CalculateArmor(AM_Utilities.Settings.hardMode);

                if(useHeatInitialised && !AM_Utilities.Settings.useHeat)
                {
                    useHeatInitialised = false;
                }

                if(useMaxInitialised && !AM_Utilities.Settings.useMax)
                {
                    useMaxInitialised = false;

                    ResetMax();
                }

                ticksToUpdate = AM_Utilities.Settings.adaptationTime;
            }
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            if(AM_Utilities.Settings.useHeat)
            {
                useHeatInitialised = true;
            }

            if(AM_Utilities.Settings.useMax)
            {
                useMaxInitialised = true;
            }
        }

        public void CheckArmorTypes()
        {
            foreach(string name in mechList.Keys)
            {
                mechList[name].CheckArmorTypes();
            }
        }

        public void CheckFactions()
        {
            foreach(string name in mechList.Keys)
            {
                mechList[name].CheckFactions();
            }
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            CheckFactions();

            CheckArmorTypes();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mechList, "mechList", LookMode.Value, LookMode.Deep);
            Scribe_Values.Look(ref useHeatInitialised, "useHeatInitialised", false);
            Scribe_Values.Look(ref useMaxInitialised, "useMaxInitialised", false);
        }
    }
}