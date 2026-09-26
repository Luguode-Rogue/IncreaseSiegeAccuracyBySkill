using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;
using MathF = TaleWorlds.Library.MathF;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
namespace IncreaseSiegeAccuracyBySkill
{
    [HarmonyPatch(typeof(RangedSiegeWeapon), "GetBallisticErrorAppliedDirection")]
    static class GetBallisticErrorAppliedDirection_Patch
    {
        static bool Prefix(ref Vec3 __result, RangedSiegeWeapon __instance,ref float BallisticErrorAmount)
        {
            string s = "远程工程器，熟练度加精度\r\n0-100：负精度，\r\n100-200：弩车加到满精度，多发投石器精度加到1散布\r\n200-250：单发投石器加到满精度\r\n250-300：配重投石器加到满精度";
            // 提前获取_lastShooterAgent（关键修改点）
            Agent lastShooterAgent = Traverse.Create(__instance)
                .Field<Agent>("_lastShooterAgent").Value;
            int duiwugongcheng = 0;//队伍的工程师的攻城等级
            // 确认agent和其起源信息有效
            if (lastShooterAgent?.Origin?.BattleCombatant is PartyBase partyBase && partyBase.MapEvent is MapEvent mapEvent)
            {
                // 获取移动部队
                MobileParty mobileParty = partyBase.IsMobile ? partyBase.MobileParty : null;

                // 检查是否满足围攻战且为攻击方的条件
                if (mobileParty != null && mapEvent.AttackerSide == partyBase.MapEventSide && mapEvent.EventType == MapEvent.BattleTypes.Siege)
                {

                    if (mobileParty.EffectiveEngineer != null)
                    {
                        duiwugongcheng= mobileParty.EffectiveEngineer.GetSkillValue(DefaultSkills.Engineering);
                    }
                }
            }

            // 获取工程技能值
            int engineeringSkill = lastShooterAgent.Character.GetSkillValue(DefaultSkills.Engineering);
            engineeringSkill += duiwugongcheng /2;
            BallisticErrorAmount = GetBaseErrorModified(__instance, engineeringSkill);
            return true; 
        }
        private static float GetBaseErrorModified(RangedSiegeWeapon weapon, int skill)
        {
            return weapon switch
            {
                FireBallista _ => CalculateModifiedError(0.5f,skill),
                Ballista _ => CalculateModifiedError(0.5f, skill),
                FireMangonel _ => CalculateModifiedError(1.5f, skill),
                Mangonel _ => CalculateModifiedError(2.5f, skill),
                Trebuchet _ => CalculateModifiedError(1.0f, skill),
                _ => CalculateModifiedError(1.0f, skill) // 默认值
            };
        }
        private static float CalculateModifiedError(float baseError, int skill)
        {
            float retFloat =1;
            if (skill <= 100)
            {
                retFloat= baseError - (skill - 100) * 0.005f;
            }
            else if (skill <=200) 
            {
                if (baseError == 0.5f)//攻城弩
                {
                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.005f, 0, 3);//

                }
                else if (baseError == 1.5f)//火焰投石车
                {

                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.001f, 0, 3);
                }
                else if (baseError == 2.5f)//散弹投石车
                {

                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.015f, 1, 3);
                }
                else if (baseError == 1f)//配重投石车和其他
                {

                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.005f, 0, 3);
                }
            }
            else if (skill <=250)
            {
                if (baseError == 0.5f)//攻城弩
                {
                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.005f, 0, 3);//

                }
                else if (baseError == 1.5f)//火焰投石车
                {

                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.001f, 0, 3);
                }
                else if (baseError == 2.5f)//散弹投石车
                {

                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.015f, 1, 3);
                }
                else if (baseError == 1f)//配重投石车和其他
                {

                    retFloat = MathF.Clamp(baseError - (skill - 100) * 0.005f, 0, 3);
                }
            }
            else if (skill <=300) 
            {
                if (baseError == 2.5)
                {
                    retFloat = 0.5f;
                }
                else
                {
                    retFloat = 0;
                }
            }
            return retFloat;
        }
    }

    
}
