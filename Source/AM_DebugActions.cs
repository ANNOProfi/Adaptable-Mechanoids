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

            foreach(string name in component.mechList.Keys)
            {
                Log.Message("AM_Debug: Resetting armor for "+name);
                component.mechList[name].ResetArmor(true);
            }
        }
    }
}