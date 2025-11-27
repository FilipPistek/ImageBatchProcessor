using ImageBatchProcessor2.Components;

namespace ImageBatchProcessor.Tests
{
    [TestClass]
    public class FileValidatorTests
    {
        [TestMethod]
        public void IsValidImage_ReturnsTrue_ForJpgExtension()
        {
            // Arrange (Pøíprava)
            string fileName = "image.jpg";

            // Act (Akce)
            bool result = FileValidator.IsValidImage(fileName);

            // Assert (Ovìøení)
            Assert.IsTrue(result, "Soubor s koncovkou .jpg by mìl projít.");
        }

        [TestMethod]
        public void IsValidImage_ReturnsTrue_ForUpperCaseJPG()
        {
            // Testujeme, že to funguje i pro velká písmena
            string fileName = "IMAGE.JPG";

            bool result = FileValidator.IsValidImage(fileName);

            Assert.IsTrue(result, "Validace by nemìla øešit velikost písmen.");
        }

        [TestMethod]
        public void IsValidImage_ReturnsFalse_ForTextFile()
        {
            // Testujeme neplatný soubor
            string fileName = "dokument.txt";

            bool result = FileValidator.IsValidImage(fileName);

            Assert.IsFalse(result, "Soubor .txt by nemìl být oznaèen jako obrázek.");
        }
    }
}