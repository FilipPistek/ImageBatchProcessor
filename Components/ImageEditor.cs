using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace ImageBatchProcessor2.Components
{
    public class ImageEditor
    {
        /// <summary>
        /// Načte obrázek, zmenší ho a přidá vodoznak.
        /// </summary>
        public static void ProcessImage(string inputPath, string outputPath)
        {
            // Použití 'using' zajistí správné uvolnění paměti (Dispose)
            using (var originalImage = Image.FromFile(inputPath))
            {
                // 1. Zmenšení obrázku (Resize) na max šířku 800px
                int newWidth = 800;
                int newHeight = (int)((double)originalImage.Height / originalImage.Width * newWidth);

                // Pokud je originál menší, nezvětšujeme ho
                if (originalImage.Width < newWidth)
                {
                    newWidth = originalImage.Width;
                    newHeight = originalImage.Height;
                }

                using (var resizedBitmap = new Bitmap(newWidth, newHeight))
                using (var graphics = Graphics.FromImage(resizedBitmap))
                {
                    // Nastavení kvality vykreslování
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    // Vykreslení zmenšeného obrázku
                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                    // 2. Přidání Vodoznaku (Watermark)
                    string watermarkText = "Image Processor - " + DateTime.Now.Year;
                    Font font = new Font("Arial", 20, FontStyle.Bold);
                    Color color = Color.FromArgb(128, 255, 255, 255); // Poloprůhledná bílá
                    SolidBrush brush = new SolidBrush(color);

                    // Umístění vpravo dole
                    SizeF textSize = graphics.MeasureString(watermarkText, font);
                    PointF position = new PointF(newWidth - textSize.Width - 10, newHeight - textSize.Height - 10);

                    graphics.DrawString(watermarkText, font, brush, position);

                    // 3. Uložení
                    resizedBitmap.Save(outputPath, ImageFormat.Jpeg);
                }
            }
        }
    }
}
