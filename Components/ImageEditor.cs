using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace ImageBatchProcessor2.Components
{
    public class ImageEditor
    {
        public static void ProcessImage(string inputPath, string outputPath, int targetWidth, int targetHeight, string watermarkText)
        {
            using (var originalImage = Image.FromFile(inputPath))
            {
                // LOGIKA PRO VÝPOČET NOVÝCH ROZMĚRŮ (Aspect Ratio)
                int newWidth, newHeight;
                double ratioX = (double)targetWidth / originalImage.Width;
                double ratioY = (double)targetHeight / originalImage.Height;
                double ratio;

                // 1. Uživatel zadal Šířku i Výšku -> Vybereme menší poměr (aby se vešel)
                if (targetWidth > 0 && targetHeight > 0)
                {
                    ratio = Math.Min(ratioX, ratioY);
                }
                // 2. Uživatel zadal jen Šířku
                else if (targetWidth > 0)
                {
                    ratio = ratioX;
                }
                // 3. Uživatel zadal jen Výšku
                else if (targetHeight > 0)
                {
                    ratio = ratioY;
                }
                // 4. Nezadal nic? Necháme originál (nebo default)
                else
                {
                    ratio = 1;
                }

                newWidth = (int)(originalImage.Width * ratio);
                newHeight = (int)(originalImage.Height * ratio);

                // Ochrana proti zvětšování (volitelné - pokud je originál menší než cíl)
                if (originalImage.Width < newWidth && originalImage.Height < newHeight)
                {
                    newWidth = originalImage.Width;
                    newHeight = originalImage.Height;
                }

                // Vykreslení
                using (var resizedBitmap = new Bitmap(newWidth, newHeight))
                using (var graphics = Graphics.FromImage(resizedBitmap))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                    // Vodoznak
                    Font font = new Font("Arial", 20, FontStyle.Bold);
                    Color color = Color.FromArgb(128, 255, 255, 255);
                    SolidBrush brush = new SolidBrush(color);

                    SizeF textSize = graphics.MeasureString(watermarkText, font);

                    // Pozice vpravo dole
                    float x = newWidth - textSize.Width - 10;
                    float y = newHeight - textSize.Height - 10;

                    // Ochrana: Kdyby byl obrázek moc malý a text se nevešel, dáme ho na 0,0
                    if (x < 0) x = 0;
                    if (y < 0) y = 0;

                    graphics.DrawString(watermarkText, font, brush, x, y);

                    resizedBitmap.Save(outputPath, ImageFormat.Jpeg);
                }
            }
        }
    }
}
