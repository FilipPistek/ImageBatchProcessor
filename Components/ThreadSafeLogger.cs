namespace ImageBatchProcessor2.Components
{
    public class ThreadSafeLogger
    {
        private readonly string _filePath;

        // Zámek - objekt, který slouží jako "klíč" ke kritické sekci
        private readonly object _lockObject = new object();

        public ThreadSafeLogger(string filePath)
        {
            _filePath = filePath;
            // Inicializace hlavičky CSV souboru
            // Toto se děje v konstruktoru (běží jen jednou), takže zde zámek není nutný,
            // pokud instanci tvoříme před spuštěním vláken.
            File.WriteAllText(_filePath, "Cas;VlaknoID;Soubor;Status\n");
        }

        public void Log(string fileName, string status, int threadId)
        {
            // KRITICKÁ SEKCE
            // Klíčové slovo 'lock' zajistí, že dovnitř tohoto bloku
            // vstoupí v daný okamžik pouze jedno vlákno. Ostatní musí čekat.
            lock (_lockObject)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string line = $"{timestamp};{threadId};{fileName};{status}\n";

                // Zápis na disk (sdílený zdroj)
                File.AppendAllText(_filePath, line);
            }
        }
    }
}
