using System.Collections.Generic;
using System.IO;

namespace MultimAPP
{
    /// <summary>
    /// Podsjetnik za mene da ti objasnim ako se nisi susreo s ovim. Skroz kul stvar...
    /// </summary>
    public static class ExtensionMethods
    {
        public static bool IsImage(this string fileName)
        {
            List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

            if (ImageExtensions.Contains(Path.GetExtension(fileName).ToUpperInvariant()))
            {
                return true;
            }
            else return false;
        }
    }
}
