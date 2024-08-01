using System.Collections.Generic;
using System.Configuration;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_GameComponent_Adaptation : GameComponent
    {
        //public Dictionary<string, AM_MechArmorStats> mechArmorList = new Dictionary<string, AM_MechArmorStats>();

        public Dictionary<bool, Dictionary<string, AM_MechArmorStats>> mechFactionList = new Dictionary<bool, Dictionary<string, AM_MechArmorStats>>();

        public Dictionary<bool, HashSet<string>> mechList = new Dictionary<bool, HashSet<string>>();

        //public HashSet<string> mechList = new HashSet<string>();

        private int ticksToUpdate = AM_Utilities.Settings.adaptationTime;

        private bool useHeatInitialised = false;

        private bool useMaxInitialised = false;

        public AM_GameComponent_Adaptation(Game game)
        {
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

                    foreach(bool colonyMech in mechFactionList.Keys)
                    {
                        foreach(string name in mechList[colonyMech])
                        {
                            mechFactionList[colonyMech].TryGetValue(name).ResetArmor(false);
                        }
                    }
                }

                if(!useMaxInitialised && AM_Utilities.Settings.useMax)
                {
                    useMaxInitialised = true;

                    foreach(bool colonyMech in mechFactionList.Keys)
                    {
                        foreach(string name in mechList[colonyMech])
                        {
                            mechFactionList[colonyMech].TryGetValue(name).ResetMax();
                        }
                    }
                }

                foreach(bool colonyMech in mechFactionList.Keys)
                {
                    foreach(string name in mechList[colonyMech])
                    {
                        mechFactionList[colonyMech].TryGetValue(name).CalculateArmor(AM_Utilities.Settings.hardMode);
                    }
                }

                if(useHeatInitialised && !AM_Utilities.Settings.useHeat)
                {
                    useHeatInitialised = false;
                }

                if(useMaxInitialised && !AM_Utilities.Settings.useMax)
                {
                    useMaxInitialised = false;

                    foreach(bool colonyMech in mechFactionList.Keys)
                    {
                        foreach(string name in mechList[colonyMech])
                        {
                            mechFactionList[colonyMech].TryGetValue(name).ResetMax();
                        }
                    }
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

            mechFactionList.Add(true, new Dictionary<string, AM_MechArmorStats>());
            mechFactionList.Add(false, new Dictionary<string, AM_MechArmorStats>());

            mechList.Add(true, new HashSet<string>());
            mechList.Add(false, new HashSet<string>());
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mechList, "mechList", LookMode.Deep, LookMode.Deep);
            Scribe_Collections.Look(ref mechFactionList, "mechFactionList", LookMode.Deep, LookMode.Deep);
        }
    }
}