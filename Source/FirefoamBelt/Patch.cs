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
            //HarmonyInstance.Create("EnhancedDevelopment.WarningOptions")PatchAll(Assembly.GetExecutingAssembly());
            HarmonyInstance _Harmony = HarmonyInstance.Create("EnhancedDevelopment.WarningOptions");


            ////Get the Origional Resting Property
            //PropertyInfo _RimWorld_Plant_Resting = typeof(RimWorld.Plant).GetProperty("Resting", BindingFlags.NonPublic | BindingFlags.Instance);
            //Patch.LogNULL(_RimWorld_Plant_Resting, "RimWorld_Plant_Resting", true);

            //Get the Resting Property Getter Method
            MethodInfo _RimWorld_CompExplosive_Detonate = typeof(RimWorld.CompExplosive).GetMethod("Detonate",BindingFlags.NonPublic | BindingFlags.Instance);
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


    public class CompExplosive_Fix : CompExplosive
    {

        protected static Boolean Detonate_Prefix(ref CompExplosive __instance, Map map)
        {
            Log.Message("Executing Detonate_Prefix");
            Log.Message("Map: " + (map != null).ToString());
            if (!__instance.parent.SpawnedOrAnyParentSpawned)
            {
                return false; //Dont run origional
            }

            //Cache Position before Killing parent.
            IntVec3 _Position = __instance.parent.PositionHeld;

            CompProperties_Explosive props = __instance.Props;
            float radius = __instance.ExplosiveRadius();
            if ((double)props.explosiveExpandPerFuel > 0.0 && __instance.parent.GetComp<CompRefuelable>() != null)
                __instance.parent.GetComp<CompRefuelable>().ConsumeFuel(__instance.parent.GetComp<CompRefuelable>().Fuel);
            if ((double)props.destroyThingOnExplosionSize <= (double)radius && !__instance.parent.Destroyed)
            {
                __instance.destroyedThroughDetonation = true;
                __instance.parent.Kill(new DamageInfo?(), (Hediff)null);
            }

            //this.EndWickSustainer();

            //EndWickSustainer is Provate so use Reflection to invoke the method.
            MethodInfo _EndWickSustainer = typeof(CompExplosive).GetMethod("EndWickSustainer", BindingFlags.NonPublic | BindingFlags.Instance);
            //Patch.LogNULL(_EndWickSustainer, "_EndWickSustainer", true);
            if (_EndWickSustainer != null)
            {
                _EndWickSustainer.Invoke(__instance, null);
            }

            __instance.wickStarted = false;
            if (map == null)
            {
                Log.Warning("Tried to detonate CompExplosive in a null map.");
            }
            else
            {
                if (props.explosionEffect != null)
                {
                    Effecter effecter = props.explosionEffect.Spawn();
                    //effecter.Trigger(new TargetInfo(__instance.parent.PositionHeld, map, false), new TargetInfo(__instance.parent.PositionHeld, map, false));
                    //Use the Cached Position
                    effecter.Trigger(new TargetInfo(_Position, map, false), new TargetInfo(_Position, map, false));
                    effecter.Cleanup();
                }

                //Get the instigator unsing Reflection because it is private
                FieldInfo _Instigator = typeof(CompExplosive).GetField("instigator", BindingFlags.NonPublic | BindingFlags.Instance);
                //Patch.LogNULL(_Instigator, "_Instigator", true);
                Thing _InstigatorValue = (Thing)_Instigator.GetValue(__instance);

                //GenExplosion.DoExplosion(this.parent.PositionHeld, map, radius, props.explosiveDamageType, this.instigator ?? (Thing)this.parent, props.damageAmountBase, props.explosionSound, (ThingDef)null, (ThingDef)null, props.postExplosionSpawnThingDef, props.postExplosionSpawnChance, props.postExplosionSpawnThingCount, props.applyDamageToExplosionCellsNeighbors, props.preExplosionSpawnThingDef, props.preExplosionSpawnChance, props.preExplosionSpawnThingCount, props.chanceToStartFire, props.dealMoreDamageAtCenter);
                GenExplosion.DoExplosion(_Position, map, radius, props.explosiveDamageType, _InstigatorValue ?? (Thing)__instance.parent, props.damageAmountBase, props.explosionSound, (ThingDef)null, (ThingDef)null, props.postExplosionSpawnThingDef, props.postExplosionSpawnChance, props.postExplosionSpawnThingCount, props.applyDamageToExplosionCellsNeighbors, props.preExplosionSpawnThingDef, props.preExplosionSpawnChance, props.preExplosionSpawnThingCount, props.chanceToStartFire, props.dealMoreDamageAtCenter);
            }

            return false;
        }

    }


}
