using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


namespace IncreaseSiegeAccuracyBySkill
{
    public class SubModule : MBSubModuleBase
    {
        protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
        {

            new Harmony("IncreaseSiegeAccuracyBySkill").PatchAll(Assembly.GetExecutingAssembly());

        }
    }
}