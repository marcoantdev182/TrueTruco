using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public static class RuntimeMaterialFactory
    {
        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public static Material GetMaterial(string key, Color color)
        {
            if (Materials.TryGetValue(key, out Material material))
            {
                return material;
            }

            Shader shader = Shader.Find("Standard");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            material = new Material(shader);
            material.name = key;
            material.color = color;
            Materials.Add(key, material);
            return material;
        }
    }
}
