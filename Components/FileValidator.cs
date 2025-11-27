namespace ImageBatchProcessor2.Components
{
    public static class FileValidator
    {
        /// <summary>
        /// Ověří, zda má soubor podporovanou koncovku (.jpg).
        /// </summary>
        public static bool IsValidImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            string extension = Path.GetExtension(filePath);

            // Ověříme, že koncovka je .jpg (ignoruje velikost písmen)
            return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
