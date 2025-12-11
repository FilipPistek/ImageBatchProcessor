using System;
using System.IO;
using System.Text.Json;
using ImageBatchProcessor2.Components;

namespace ImageBatchProcessor2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Image Batch Processor (Parallel) ===");
            Console.WriteLine("---------------------------------------------");

            // 1. NAČTENÍ KONFIGURACE (config.json)
            // Hledáme soubor vedle .exe souboru
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            AppConfig config = new AppConfig(); // Vytvoříme instanci s výchozími hodnotami (800px, 0px, text)

            if (File.Exists(configPath))
            {
                try
                {
                    string jsonString = File.ReadAllText(configPath);
                    // Deserializace: Převod textu z JSONu na C# objekt
                    // Operátor ?? zajistí, že když se to nepovede (null), použije se 'new AppConfig()'
                    config = JsonSerializer.Deserialize<AppConfig>(jsonString) ?? new AppConfig();

                    Console.WriteLine($"[Config] Načteno nastavení:");
                    Console.WriteLine($" - Max Šířka: {config.TargetWidth} px");
                    Console.WriteLine($" - Max Výška: {config.TargetHeight} px (0 = automaticky)");
                    Console.WriteLine($" - Vodoznak: '{config.WatermarkText}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Config VAROVÁNÍ] Chyba při čtení config.json: {ex.Message}");
                    Console.WriteLine("Používám výchozí nastavení.");
                }
            }
            else
            {
                Console.WriteLine("[Config] Soubor config.json nenalezen. Používám výchozí hodnoty.");
            }
            Console.WriteLine("---------------------------------------------");

            // 2. NASTAVENÍ CEST A SLOŽEK
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string inputFolder = Path.Combine(baseDir, "input");
            string outputFolder = Path.Combine(baseDir, "output");
            string logFile = Path.Combine(outputFolder, "log.csv");

            // Počet vláken podle jader procesoru (Paralelizace)
            int threadCount = Environment.ProcessorCount;

            // Ověření a vytvoření složek
            bool inputExists = Directory.Exists(inputFolder);
            if (!inputExists)
            {
                Directory.CreateDirectory(inputFolder);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[INFO] Byla vytvořena složka 'input'.");
                Console.WriteLine($"Cesta: {inputFolder}");
                Console.WriteLine("Vložte do ní prosím obrázky (.jpg) a spusťte program znovu.");
                Console.ResetColor();
                Console.WriteLine("\nStiskněte libovolnou klávesu pro ukončení...");
                Console.ReadKey();
                return; // Ukončí program, protože není co zpracovávat
            }

            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);

            Console.WriteLine($"Vstupní složka: {inputFolder}");
            Console.WriteLine($"Výstupní složka: {outputFolder}");
            Console.WriteLine($"Počet pracovních vláken: {threadCount}");
            Console.WriteLine("---------------------------------------------");

            // 3. INICIALIZACE KOMPONENT
            // ThreadSafeLogger řeší konflikty při zápisu do souboru (používá lock)
            var logger = new ThreadSafeLogger(logFile);

            // Processor řídí logiku Producer-Consumer a předáváme mu načtenou konfiguraci
            var processor = new Processor(logger, config);

            // 4. SPUŠTĚNÍ ZPRACOVÁNÍ
            Console.WriteLine("Spouštím zpracování...");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Hlavní metoda, která spustí vlákna
                processor.Start(inputFolder, outputFolder, threadCount);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[CHYBA] Kritická chyba aplikace: {ex.Message}");
                Console.ResetColor();
            }

            watch.Stop();
            Console.WriteLine("---------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"HOTOVO. Celkový čas: {watch.ElapsedMilliseconds} ms");
            Console.ResetColor();
            Console.WriteLine("Výsledky najdete ve složce output.");
            Console.WriteLine("Stiskněte libovolnou klávesu pro ukončení...");
            Console.ReadKey();
        }
    }
}
