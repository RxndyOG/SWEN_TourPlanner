using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace UI.Converters
{

    public class SimpleFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            // Pfad zur Schriftartdatei (TTF)
            string fontPath = Path.Combine("Fonts", $"{faceName}.ttf");

            if (File.Exists(fontPath))
            {
                return File.ReadAllBytes(fontPath);
            }

            throw new FileNotFoundException($"Font file not found: {fontPath}");
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                return new FontResolverInfo("Arial");
            }

            // Fallback to a default font
            return new FontResolverInfo("FallbackFont");
        }

    }
}
