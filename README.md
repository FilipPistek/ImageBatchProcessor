# ImageBatchProcessor

## Autor
**Jméno:** Filip Pištěk

**Předmět:** Programové vybavení

**Rok:** 2025

## Popis projektu
ImageBatchProcessor je konzolová aplikace demonstrující **paralelní zpracování dat**. Řeší reálný problém hromadné úpravy fotografií (změna velikosti + vodoznak), kde využití více vláken výrazně zrychluje proces oproti sériovému zpracování.

Projekt využívá návrhový vzor **Producer-Consumer** a bezpečně řeší konflikty o sdílené zdroje (zápis do logovacího souboru).

## Struktura repozitáře
Projekt dodržuje striktní oddělení zdrojového kódu, testů a dokumentace:

* `Components/` - Třídy zapouzdřující logiku (Logika procesoru, Editor grafiky, Loger).
* `ImageBatchProcessorTests/` - Unit testy ověřující validaci souborů (MSTest).

---

## Návod k instalaci a spuštění

### Spuštění hotové aplikace (Bez nutnosti IDE)
Tato varianta je určena pro školní PC nebo běžného uživatele.

1.  **Stažení:**
    * Přejděte do sekce **[Releases](../../releases)** v tomto repozitáři.
    * Stáhněte `ImageBatchProcessor_v1.0.zip` archiv.

2.  **Příprava:**
    * Rozbalte ZIP archiv.

3.  **Spuštění:**
    * Spusťte `ImageBatchProcessor.exe`.
    * Aplikace při prvním startu vytvoří složky `input` a `output`.
    * Vložte obrázky (`.jpg`) do nově vzniklé složky `input`.
    * Spusťte aplikaci znovu – proběhne zpracování.

## Ovládání a Konfigurace
Aplikace funguje plně automaticky bez nutnosti zásahu uživatele.

* **Vstup:** Obrázky formátu `.jpg` ve složce `/input`.
* **Výstup:** Upravené obrázky ve složce `/output` + soubor `process_log.csv` (auditní log).
* **Konfigurace vláken:** Aplikace automaticky detekuje počet logických jader procesoru (`Environment.ProcessorCount`) a podle toho vytvoří optimální počet pracovních vláken (Konzumentů).
