using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;
using Verse.Sound;
using System.Reflection;

namespace FirefoamBelt
{

    [StaticConstructorOnStartup]
    class Patch
    {
        static Patch()
        {

            Log.Message("Patching FirefoamBelt");
            HarmonyInstance _Harmony = HarmonyInstance.Create("FirefoamBelt");
            
            //Get the Method
            MethodInfo _RimWorld_CompExplosive_Detonate = typeof(RimWorld.CompExplosive).GetMethod("Detonate", BindingFlags.NonPublic | BindingFlags.Instance);
            Patch.LogNULL(_RimWorld_CompExplosive_Detonate, "_RimWorld_CompExplosive_Detonate", true);

            //Get the Prefix Patch
            MethodInfo _CompExplosiveFix_DetonateFixed = typeof(CompExplosive_Fix).GetMethod("Detonate_Prefix", BindingFlags.NonPublic | BindingFlags.Static);
            Patch.LogNULL(_CompExplosiveFix_DetonateFixed, "Detonate_Prefix", true);

            //Apply the Prefix Patch
            _Harmony.Patch(_RimWorld_CompExplosive_Detonate, new HarmonyMethod(_CompExplosiveFix_DetonateFixed), null);
            
            Log.Message("Patching FirefoamBelt Complete");
        }


        /// <summary>
        /// Debug Logging Helper
        /// </summary>
        /// <param name="objectToTest"></param>
        /// <param name="name"></param>
        /// <param name="logSucess"></param>
        public static void LogNULL(object objectToTest, String name, bool logSucess = false)
        {
            if (objectToTest == null)
            {
                Log.Error(name + " Is NULL.");
            }
            else
            {
                if (logSucess)
                {
                    Log.Message(name + " Is Not NULL.");
                }
            }
        }
    }


    public class CompExplosive_Fix
    {

        protected static Boolean Detonate_Prefix(ref CompExplosive __instance, Map map)
        {

            Log.Message("Detonate");
            if (__instance.parent.SpawnedOrAnyParentSpawned)
            {

                //Cache Position before Killing parent.
                IntVec3 _Position = __instance.parent.PositionHeld;

                CompProperties_Explosive props = __instance.Props;
                float num = __instance.ExplosiveRadius();
                if (props.explosiveExpandPerFuel > 0f && __instance.parent.GetComp<CompRefuelable>() != null)
                {
                    __instance.parent.GetComp<CompRefuelable>().ConsumeFuel(__instance.parent.GetComp<CompRefuelable>().Fuel);
                }
                if (props.destroyThingOnExplosionSize <= num && !__instance.parent.Destroyed)
                {
                    __instance.destroyedThroughDetonation = true;
                    __instance.parent.Kill(null, null);
                }
                //__instance.EndWickSustainer();
                //EndWickSustainer is Private so use Reflection to invoke the method.
                MethodInfo _EndWickSustainer = typeof(CompExplosive).GetMethod("EndWickSustainer", BindingFlags.NonPublic | BindingFlags.Instance);
                Patch.LogNULL(_EndWickSustainer, "_EndWickSustainer");
                if (_EndWickSustainer != null)
                {
                    _EndWickSustainer.Invoke(__instance, null);
                }

                __instance.wickStarted = false;
                if (map == null)
                {
                    Log.Warning("Tried to detonate CompExplosive in a null map.", false);
                }
                else
                {
                    if (props.explosionEffect != null)
                    {
                        Effecter effecter = props.explosionEffect.Spawn();
                       //effecter.Trigger(new TargetInfo(__instance.parent.PositionHeld, map, false), new TargetInfo(__instance.parent.PositionHeld, map, false));
                        effecter.Trigger(new TargetInfo(_Position, map, false), new TargetInfo(_Position, map, false));
                        effecter.Cleanup();
                    }
                    IntVec3 positionHeld = __instance.parent.PositionHeld;
                    float radius = num;
                    DamageDef explosiveDamageType = props.explosiveDamageType;
                    
                    //Get the instigator unsing Reflection because it is private
                    FieldInfo _Instigator = typeof(CompExplosive).GetField("instigator", BindingFlags.NonPublic | BindingFlags.Instance);
                    Patch.LogNULL(_Instigator, "_Instigator");
                    Thing _InstigatorValue = (Thing)_Instigator.GetValue(__instance);

                    Thing thing = _InstigatorValue ?? __instance.parent;

                    int damageAmountBase = props.damageAmountBase;
                    float armorPenetrationBase = props.armorPenetrationBase;
                    SoundDef explosionSound = props.explosionSound;
                    ThingDef postExplosionSpawnThingDef = props.postExplosionSpawnThingDef;
                    float postExplosionSpawnChance = props.postExplosionSpawnChance;
                    int postExplosionSpawnThingCount = props.postExplosionSpawnThingCount;
                  //  GenExplosion.DoExplosion(positionHeld, map, radius, explosiveDamageType, thing, damageAmountBase, armorPenetrationBase, explosionSound, null, null, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, props.applyDamageToExplosionCellsNeighbors, props.preExplosionSpawnThingDef, props.preExplosionSpawnChance, props.preExplosionSpawnThingCount, props.chanceToStartFire, props.damageFalloff);

                    GenExplosion.DoExplosion(_Position, map, radius, explosiveDamageType, thing, damageAmountBase, armorPenetrationBase, explosionSound, null, null, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, props.applyDamageToExplosionCellsNeighbors, props.preExplosionSpawnThingDef, props.preExplosionSpawnChance, props.preExplosionSpawnThingCount, props.chanceToStartFire, props.damageFalloff);
                }
            }

            return false;
        }

    }


}
