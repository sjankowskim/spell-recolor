using ThunderRoad;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;
using System.Linq;

/*
 * GRAVITY
 * - Color
 * - Color 1
 * - Color 2
 * - Color 3
 * - Intensity
 * 
 * FIRE
 * - Color
 * - Intensity
 * 
 * LIGHTNING
 * - Color
 * - Color 1
 * - Color 2
 * - Intensity
 * - Size
 */
namespace SpellRecolor
{
    public class GravityColors : MonoBehaviour
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public float intensity;
    }

    public class GravityRecolorLevelModule : LevelModule
    {
        public static GravityColors gravityColors;
        [Tooltip("Sets the red value for the Gravity spell's color.")]
        [Range(0, 255)]
        public float r = 0f;
        [Tooltip("Sets the green value for the Gravity spell's color.")]
        [Range(0, 255)]
        public float g = 0f;
        [Tooltip("Sets the blue value for the Gravity spell's color.")]
        [Range(0, 255)]
        public float b = 0f;
        [Tooltip("Sets the alpha value for the Gravity spell's color.")]
        [Range(0, 1)]
        public float a = 1f;
        [Tooltip("Sets the intensity value for the Gravity spell's color.")]
        [Range(-10, 10)]
        public float intensity = 0f;

        public override IEnumerator OnLoadCoroutine()
        {
            gravityColors = GameManager.local.gameObject.AddComponent<GravityColors>();
            new Harmony("Fire").PatchAll();
            return base.OnLoadCoroutine();
        }

        public override void Update()
        {
            base.Update();
            gravityColors.r = r / 255;
            gravityColors.g = g / 255;
            gravityColors.b = b / 255;
            gravityColors.a = a;
            gravityColors.intensity = intensity;
        }

        [HarmonyPatch(typeof(SpellCastGravity), "FindAndLockItem")]
        class GravityStaffPatch
        {
            public static void Postfix()
            {
                float r = gravityColors.r;
                float g = gravityColors.g;
                float b = gravityColors.b;
                float a = gravityColors.a;
                EffectInstance woosh = IllegalStuff.GetField<EffectInstance>(Player.currentCreature.equipment.GetHeldWeapon(Side.Right).imbues[0].spellCastBase, "staffWhooshEffect");
                RecolorWooshParticles(woosh, r, g, b, a);
            }

            private static void RecolorWooshParticles(EffectInstance instance, float r, float g, float b, float a)
            {
                instance.Stop();
                foreach (Effect effect in instance.effects)
                {
                    if (effect is EffectParticle effectParticle)
                    {
                        RecolorUtils.ChangeParticleColor(ref effectParticle.rootParticleSystem, r, g, b, a);
                        foreach (var child in effectParticle.childs)
                        {
                            RecolorUtils.ChangeParticleColor(ref child.particleSystem, r, g, b, a);
                        }
                    }
                }
                instance.Play();
            }
        }

        [HarmonyPatch(typeof(SpellCastProjectile), "Fire")]
        class FireSpellPatch
        {
            public static void Postfix(bool active)
            {
                float r = gravityColors.r;
                float g = gravityColors.g;
                float b = gravityColors.b;
                float a = gravityColors.a;
                float intensity = gravityColors.intensity;

                if (active)
                {
                    EffectInstance charge = IllegalStuff.GetField<EffectInstance>(Player.currentCreature.mana.casterRight.spellInstance, "chargeEffectInstance");
                    RecolorChargeInstance(charge, r, g, b, a, intensity);

                    foreach (EffectInstance fingerInstance in Player.currentCreature.mana.casterRight.fingerEffectInstances)
                        RecolorFingerEffectInstance(fingerInstance, r, g, b, a, intensity);
                }
            }

            private static void RecolorChargeInstance(EffectInstance instance, float r, float g, float b, float a, float intensity)
            {
                instance.Stop();
                Debug.LogWarning("Stand back, I'm about to do some magic...");
                foreach(var particleEffect in instance.effects.OfType<EffectParticle>())
                {
                    Debug.LogError(particleEffect);
                }
                foreach (Effect effect in instance.effects)
                {
                    Debug.Log(effect);
                    if (effect is EffectVfx effectVfx)
                    {
                    
                        effectVfx.vfx.SetVector4("Color", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetFloat("Intensity", intensity);
                        var list = new List<string>();
                        effectVfx.vfx.GetParticleSystemNames(list);
                        var vfx = effectVfx.vfx;

                        List<VFXExposedProperty> list2 = new List<VFXExposedProperty>();
                        vfx.visualEffectAsset.GetExposedProperties(list2);
                        foreach (VFXExposedProperty property in list2)
                            Debug.LogWarning(property.type + ": " + property.name);

                        foreach (var entry in list)
                        {
                            Debug.LogWarning(entry);
                            Debug.Log(effectVfx.vfx.GetParticleSystemInfo(entry).sleeping);
                        }
                    }
                }
                instance.Play();
            }

            private static void RecolorFingerEffectInstance(EffectInstance instance, float r, float g, float b, float a, float intensity)
            {
                instance.Stop();
                foreach (Effect effect in instance.effects)
                {
                    if (effect is EffectVfx effectVfx)
                    {
                        effectVfx.vfx.SetVector4("Main Color", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetFloat("Intensity", intensity);
                        effectVfx.vfx.SetGradient("MainGradient", RecolorUtils.CreateGradient(r, g, b, a));
                    }
                }
                instance.Play();
            }
        }

        [HarmonyPatch(typeof(SpellCastGravity), "Fire")]
        class GravitySpellPatch
        {
            public static void Postfix(bool active)
            {
                float r = gravityColors.r;
                float g = gravityColors.g;
                float b = gravityColors.b;
                float a = gravityColors.a;
                float intensity = gravityColors.intensity;

                if (active)
                {
                    EffectInstance charge = IllegalStuff.GetField<EffectInstance>(Player.currentCreature.mana.casterRight.spellInstance, "chargeEffectInstance");
                    RecolorChargeInstance(charge, r, g, b, a, intensity);

                    foreach (EffectInstance fingerInstance in Player.currentCreature.mana.casterRight.fingerEffectInstances)
                    {
                        RecolorFingerEffectInstance(fingerInstance, r, g, b, a, intensity);
                    }

                    EffectInstance hover = IllegalStuff.GetField<EffectInstance>(Player.currentCreature.mana.casterRight.spellInstance, "hoverEffectInstance");
                    RecolorHoverParticles(hover, r, g, b, a);

                    EffectData push = IllegalStuff.GetField<EffectData>(Player.currentCreature.mana.casterRight.spellInstance, "pushEffectData");
                    RecolorPushParticles(push, r, g, b, a);
                }
            }

            private static void RecolorPushParticles(EffectData data, float r, float g, float b, float a)
            {
                foreach (EffectModule module in data.modules)
                {
                    if (module is EffectModuleParticle particleModule)
                    {
                        // particleModule.effectParticlePrefab.SetMainGradient(RecolorUtils.CreateGradient(r, g, b, a));
                        // particleModule.effectParticlePrefab.SetSecondaryGradient(RecolorUtils.CreateGradient(r, g, b, a))
                        // Debug.Log(particleModule.effectParticlePrefab.containingInstance);
                        // particleModule.effectParticlePrefab.childs.Count
                        // particleModule.effectParticlePrefab.currentMainGradient
                        // particleModule.effectParticlePrefab.currentSecondaryGradient
                        // ParticleSystem.MinMaxCurve curve = IllegalStuff.GetField<ParticleSystem.MinMaxCurve>(particleModule.effectParticlePrefab, "minMaxCurve");
                        // Debug.Log(curve.constant);
                        // Debug.Log(curve.constantMin);
                        // Debug.Log(curve.constantMax);

/*                        RecolorUtils.ChangeParticleColor(particleModule.effectParticlePrefab.rootParticleSystem, r, g, b, a);
                        foreach (var child in particleModule.effectParticlePrefab.childs)
                        {
                            RecolorUtils.ChangeParticleColor(child.particleSystem, r, g, b, a);
                        }*/
                    }
                }
            }

            private static void RecolorHoverParticles(EffectInstance instance, float r, float g, float b, float a)
            {
                instance.Stop();
                foreach (Effect effect in instance.effects)
                {
                    Debug.Log(effect);
                    if (effect is EffectParticle effectParticle)
                    {
                        RecolorUtils.ChangeParticleColor(ref effectParticle.rootParticleSystem, r, g, b, a);
                        foreach (var child in effectParticle.childs)
                        {
                            RecolorUtils.ChangeParticleColor(ref child.particleSystem, r, g, b, a);
                        }
                    }
                }
                instance.Play();
            }

            private static void RecolorChargeInstance(EffectInstance instance, float r, float g, float b, float a, float intensity)
            {
                instance.Stop();
                foreach (Effect effect in instance.effects)
                {
                    Debug.Log(effect);
                    if (effect is EffectVfx effectVfx)
                    {


                        effectVfx.vfx.SetVector4("Color", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetVector4("Color 1", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetVector4("Color 2", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetVector4("Color 3", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetFloat("Intensity", intensity);
                    } 
                    else if (effect is EffectParticle effectParticle) 
                    {
                        RecolorUtils.ChangeParticleColor(ref effectParticle.rootParticleSystem, r, g, b, a);
                        foreach (var child in effectParticle.childs)
                        {
                            RecolorUtils.ChangeParticleColor(ref child.particleSystem, r, g, b, a);
                        }
                    }
                }
                instance.Play();
            }

            private static void RecolorFingerEffectInstance(EffectInstance instance, float r, float g, float b, float a, float intensity)
            {
                instance.Stop();
                foreach (Effect effect in instance.effects)
                {
                    if (effect is EffectVfx effectVfx)
                    {
                        effectVfx.vfx.SetVector4("Main Color", new Vector4(r, g, b, a));
                        effectVfx.vfx.SetFloat("Intensity", intensity);
                        effectVfx.vfx.SetGradient("MainGradient", RecolorUtils.CreateGradient(r, g, b, a));
                    }
                }
                instance.Play();
            }
        }
    }
}
