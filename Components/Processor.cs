using System.Collections.Concurrent;

namespace ImageBatchProcessor2.Components
{
    /// <summary>
    /// Třída řídící paralelní zpracování úloh pomocí vzoru Producer-Consumer.
    /// </summary>
    public class Processor
    {
        // Thread-safe kolekce (fronta), která blokuje konzumenty, pokud je prázdná,
        // a blokuje producenta, pokud je přeplněná (kapacita 100).
        private BlockingCollection<string> _tasks = new BlockingCollection<string>(100);

        private readonly ThreadSafeLogger _logger;

        public Processor(ThreadSafeLogger logger)
        {
            _logger = logger;
        }

        public void Start(string inputFolder, string outputFolder, int threadCount)
        {
            // 1. Spustíme KONZUMENTY (Dělníky)
            // Vytvoříme pole tasků (vláken), které budou běžet na pozadí
            Task[] consumers = new Task[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                int workerId = i; // Lokální kopie proměnné pro lambda výraz
                consumers[i] = Task.Run(() => ConsumerWorker(outputFolder, workerId));
            }
            Console.WriteLine($"[System] Spuštěno {threadCount} konzumních vláken.");

            // 2. Spustíme PRODUCENTA
            // Běží v hlavním vlákně (nebo bychom mohli dát taky do Task.Run)
            ProducerWorker(inputFolder);

            // 3. Čekáme na dokončení všech konzumentů
            // Konzumenti skončí sami, jakmile Producent zahlásí "CompleteAdding" a fronta se vyprázdní.
            Task.WaitAll(consumers);
        }

        /// <summary>
        /// Producent: Prohledá disk a plní frontu práce.
        /// </summary>
        private void ProducerWorker(string inputFolder)
        {
            try
            {
                var allFiles = Directory.GetFiles(inputFolder);
                Console.WriteLine($"[Producent] Nalezeno {allFiles.Length} souborů ke zpracování.");

                foreach (var file in allFiles)
                {
                    if (FileValidator.IsValidImage(file))
                    {
                        _tasks.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Producent Chyba] {ex.Message}");
            }
            finally
            {
                _tasks.CompleteAdding();
                Console.WriteLine("[Producent] Všechny soubory přidány do fronty.");
            }
        }

        /// <summary>
        /// Konzument: Bere práci z fronty a volá ImageEditor.
        /// </summary>
        private void ConsumerWorker(string outputFolder, int id)
        {
            // GetConsumingEnumerable() chytře iteruje:
            // - Pokud je něco ve frontě, vezme to.
            // - Pokud je prázdná, ale producent ještě neskončil, čeká (sleep).
            // - Pokud je prázdná a producent skončil (CompleteAdding), cyklus skončí.
            foreach (var inputFile in _tasks.GetConsumingEnumerable())
            {
                string fileName = Path.GetFileName(inputFile);
                string outputPath = Path.Combine(outputFolder, fileName);
                string status = "OK";

                try
                {
                    Console.WriteLine($"[Vlákno {id}] Zpracovávám: {fileName}");

                    // REÁLNÁ PRÁCE: Volání logiky pro úpravu obrázku
                    ImageEditor.ProcessImage(inputFile, outputPath);

                }
                catch (Exception ex)
                {
                    status = $"ERROR: {ex.Message}";
                    Console.WriteLine($"[Vlákno {id}] Chyba u {fileName}: {ex.Message}");
                }

                // SYNCHRONIZACE: Zápis do logu (zde by nastal konflikt bez zámku)
                _logger.Log(fileName, status, id);
            }

            Console.WriteLine($"[Vlákno {id}] Končí práci.");
        }
    }
}
