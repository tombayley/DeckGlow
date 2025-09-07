using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Windows.Media;

namespace DeckGlow
{
    internal class Util
    {
        public static ImageSource? LoadSvg(string svgPath)
        {
            var converter = new FileSvgReader(new WpfDrawingSettings());
            try
            {
                var drawing = converter.Read(svgPath);
                if (drawing != null)
                {
                    return new DrawingImage(drawing);
                }
            }
            catch (ArgumentException e)
            {
                return null;
            }
            return null;
        }
    }
}
