using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Fonts;


namespace UI.Converters
{

    public class SimpleFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            // Return the bytes of your embedded font here.
            // You could load from resources or a file.
            throw new NotImplementedException();
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Return the info for your font.
            return new FontResolverInfo("MyFont");
        }
    }
}
