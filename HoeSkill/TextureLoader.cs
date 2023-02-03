using System.IO;
using System.Reflection;
using UnityEngine;

namespace HoeSkill
{
    public static class TextureLoader
    {
        /// <summary>
        /// Loads a custom texture embedded in the current assembly.
        /// </summary>
        /// <param name="resourceFullPath">Fully qualified path + name of resource</param>
        public static Sprite LoadCustomTexture(string resourceFullPath)
        {
            byte[] rawBytes = ExtractResource(resourceFullPath);

            // Create empty texture, then load raw image data into it
            var texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(rawBytes);
            
            return Sprite.Create(texture2D, new Rect(0f, 0f, 32f, 32f), Vector2.zero);
        }

        private static byte[] ExtractResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream fileStream = assembly.GetManifestResourceStream(filename))
            {
                if (fileStream == null)
                {
                    return null;
                }

                var loadedBytes = new byte[fileStream.Length];
                fileStream.Read(loadedBytes, 0, loadedBytes.Length);
                return loadedBytes;
            }
        }
    }
}