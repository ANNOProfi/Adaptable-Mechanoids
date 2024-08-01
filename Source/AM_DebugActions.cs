using LudeonTK;
using Verse;

namespace AdaptableMechanoids
{
    public static class AM_DebugActions
    {
        [DebugAction("Adaptable Mechanoids", "ResetArmor", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ResetArmor()
        {
            AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

            foreach(bool colonyMech in component.mechList.Keys)
            {
                foreach(string name in component.mechList[colonyMech])
                {
                    Log.Message("AM_Debug: Resetting armor for "+name+", colony mech: "+colonyMech);
                    component.mechFactionList[colonyMech].TryGetValue(name).ResetArmor(true);
                }
            }
        }
    }
}