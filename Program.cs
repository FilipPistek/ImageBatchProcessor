using ImageBatchProcessor2.Components;

namespace ImageBatchProcessor2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Image Batch Processor (Parallel) ===");

            string inputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input");
            string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
            string logFile = Path.Combine(outputFolder, "process_log.csv");

            // Počet vláken nastavíme podle počtu jader procesoru
            int threadCount = Environment.ProcessorCount;

            // Ověření existence složek
            if (!Directory.Exists(inputFolder))
            {
                Directory.CreateDirectory(inputFolder);
                Console.WriteLine($"[INFO] Vytvořena složka pro vstup: {inputFolder}");
                Console.WriteLine("Prosím vložte do této složky nějaké obrázky (.jpg) a spusťte program znovu.");
                return;
            }
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);

            Console.WriteLine($"Vstup: {inputFolder}");
            Console.WriteLine($"Výstup: {outputFolder}");
            Console.WriteLine($"Počet pracovních vláken: {threadCount}");
            Console.WriteLine("---------------------------------------------");

            // 2. INICIALIZACE KOMPONENT
            // Vytvoření loggeru (řeší konflikty o zápis do souboru)
            var logger = new ThreadSafeLogger(logFile);

            // Vytvoření hlavního procesoru (řídí paralelizaci)
            var processor = new Processor(logger);

            // 3. SPUŠTĚNÍ ZPRACOVÁNÍ
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                processor.Start(inputFolder, outputFolder, threadCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHYBA] Kritická chyba aplikace: {ex.Message}");
            }

            watch.Stop();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"Hotovo. Celkový čas: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("Stiskněte libovolnou klávesu pro ukončení...");
            Console.ReadKey();
        }
    }
}
