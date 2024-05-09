using UnityEngine;

namespace SpellRecolor
{
    public class RecolorUtils
    {
        public static Gradient CreateGradient(float r, float g, float b, float a)
        {
            Gradient gradient = new Gradient();

            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(r, g, b), 0.0f), new GradientColorKey(new Color(r, g, b), 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(a, 0.0f), new GradientAlphaKey(a, 1.0f) }
                );

            return gradient;
        }

        public static void PrintGradient(Gradient gradient)
        {
            foreach (GradientColorKey key in gradient.colorKeys)
                Debug.Log("Color: " + key.color + ", " + key.time);

            foreach (GradientAlphaKey key in gradient.alphaKeys)
                Debug.Log("Alpha: " + key.alpha + ", " + key.time);
        }

        public static void ChangeParticleColor(ref ParticleSystem system, float r, float g, float b, float a)
        {
            // Uses color, colorMin, colorMax, gradient, and gradientMax
/*            ParticleSystem.ColorOverLifetimeModule colorModule = system.colorOverLifetime;
            colorModule.color = new ParticleSystem.MinMaxGradient { 
                color = new Color(r, g, b, a), 
                colorMin = colorModule.color.colorMin, 
                colorMax = new Color(r, g, b, a), 
                gradient = colorModule.color.gradient, 
                gradientMax = colorModule.color.gradientMax 
            };*/
            ParticleSystem.MainModule main = system.main;
            main.startColor = new ParticleSystem.MinMaxGradient
            {
                color = new Color(r, g, b, a),
                colorMin = main.startColor.colorMin,
                colorMax = new Color(r, g, b, a),
                gradient = main.startColor.gradient,
                gradientMin = main.startColor.gradientMin,
                gradientMax = main.startColor.gradientMax
            };
        /*            settings.startColor = new ParticleSystem.MinMaxGradient
                    {
                        color =new Color(r, g, b, a);
                        colorMin = colorModule.color.colorMin,
                        colorMax = new Color(r, g, b, a),
                        gradient = colorModule.color.gradient,
                        gradientMax = colorModule.color.gradientMax
                    };*/
    }

/*        public static void ChangeAllParticleColor(ParticleSystem system, float r, float g, float b, float a)
        {
            ChangeParticleColor(system, r, g, b, a);

            // Uses color, colorMin, colorMax; does NOT use gradient, gradientMin, gradientMax
            // Keep colorMin; replace color & colorMax
            ParticleSystem.MainModule main = system.main;
            main.startColor = new ParticleSystem.MinMaxGradient { 
                color = new Color(r, g, b, a), 
                colorMin = main.startColor.colorMin, 
                colorMax = new Color(r, g, b, a)
            };
        }*/
    }
}
