using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace HoeSkill
{
    public static class TextureLoader
    {
        public static Sprite LoadCustomTexture(string resourceName)
        {
            Texture2D texture2D = new Texture2D(0, 0);
            ImageConversion.LoadImage(texture2D, ExtractResource(resourceName));
            return Sprite.Create(texture2D, new Rect(0f, 0f, 32f, 32f), Vector2.zero);
        }

        private static byte[] ExtractResource(String filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream filestream = assembly.GetManifestResourceStream(filename))
            {
                if (filestream == null)
                {
                    return null;
                }
                byte[] byteArray = new byte[filestream.Length];
                filestream.Read(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
    }
}
