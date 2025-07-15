using Raizen.UniCad.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Raizen.UniCad.BLL.Util
{
    public class Imagem  : IDisposable
    {
        private Bitmap b = null;

        public Image resizeImage(Image imgToResize, float Width, float Height)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (Width / (float)sourceWidth);
            nPercentH = (Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            b = new Bitmap(destWidth, destHeight);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();
            }

            return (Image)b;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                b.Dispose();
                
            }
            
        }
    }
}
