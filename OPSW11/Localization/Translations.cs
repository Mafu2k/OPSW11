namespace OPSW11.Localization;

/// <summary>
/// Central translation table. Every user-facing string lives here keyed by a
/// stable identifier, with one value per supported language. Missing keys or
/// languages fall back to English, then to the key itself, so the UI degrades
/// gracefully and a typo is visible rather than blank.
/// </summary>
internal static class Translations
{
    private static readonly HashSet<string> SupportedLanguages =
        new(StringComparer.OrdinalIgnoreCase) { "pl", "en", "de", "es", "fr", "uk" };

    public static bool IsSupported(string? code)
        => code is not null && SupportedLanguages.Contains(code);

    public static string Get(string lang, string key)
    {
        if (Table.TryGetValue(key, out var byLang))
        {
            if (byLang.TryGetValue(lang, out var value)) return value;
            if (byLang.TryGetValue("en", out var english)) return english;
        }
        return key;
    }

    // pl, en, de, es, fr, uk
    private static Dictionary<string, string> E(
        string pl, string en, string de, string es, string fr, string uk)
        => new()
        {
            ["pl"] = pl, ["en"] = en, ["de"] = de,
            ["es"] = es, ["fr"] = fr, ["uk"] = uk
        };

    private static readonly Dictionary<string, Dictionary<string, string>> Table = new()
    {
        // Global / MainWindow
        ["App_Subtitle"] = E(
            "Optymalizator systemu", "System Optimizer", "System-Optimierer",
            "Optimizador del sistema", "Optimiseur système", "Оптимізатор системи"),
        ["Nav_Dashboard"] = E("Panel główny", "Dashboard", "Übersicht", "Panel", "Tableau de bord", "Панель"),
        ["Nav_QuickFix"] = E("Szybka naprawa", "Quick Fix", "Schnellreparatur", "Reparación rápida", "Réparation rapide", "Швидке виправлення"),
        ["Nav_AdvancedFix"] = E("Zaawansowana", "Advanced", "Erweitert", "Avanzada", "Avancée", "Розширена"),
        ["Nav_CustomFix"] = E("Naprawa własna", "Custom Fix", "Benutzerdefiniert", "Personalizada", "Personnalisée", "Власна"),
        ["Nav_Logs"] = E("Dziennik", "Logs", "Protokoll", "Registro", "Journal", "Журнал"),
        ["Admin_Yes"] = E("Tryb administratora", "Administrator mode", "Administratormodus", "Modo administrador", "Mode administrateur", "Режим адміністратора"),
        ["Admin_No"] = E("Brak uprawnień admina", "No admin privileges", "Keine Adminrechte", "Sin permisos de administrador", "Pas de droits admin", "Немає прав адміністратора"),
        ["Theme_Light"] = E("Jasny motyw", "Light theme", "Helles Design", "Tema claro", "Thème clair", "Світла тема"),
        ["Theme_Dark"] = E("Ciemny motyw", "Dark theme", "Dunkles Design", "Tema oscuro", "Thème sombre", "Темна тема"),
        ["Lang_Label"] = E("JĘZYK", "LANGUAGE", "SPRACHE", "IDIOMA", "LANGUE", "МОВА"),
        ["Status_Ready"] = E("Gotowy", "Ready", "Bereit", "Listo", "Prêt", "Готово"),
        ["Status_View"] = E("Widok: {0}", "View: {0}", "Ansicht: {0}", "Vista: {0}", "Vue : {0}", "Вигляд: {0}"),

        // Dashboard
        ["Dash_Title"] = E("Panel główny", "Dashboard", "Übersicht", "Panel principal", "Tableau de bord", "Головна панель"),
        ["Dash_Loading"] = E("Pobieranie danych systemowych...", "Loading system data...", "Systemdaten werden geladen...", "Cargando datos del sistema...", "Chargement des données système...", "Завантаження даних системи..."),
        ["Dash_Computer"] = E("KOMPUTER", "COMPUTER", "COMPUTER", "EQUIPO", "ORDINATEUR", "КОМП'ЮТЕР"),
        ["Dash_Processor"] = E("PROCESOR", "PROCESSOR", "PROZESSOR", "PROCESADOR", "PROCESSEUR", "ПРОЦЕСОР"),
        ["Dash_CpuUsage"] = E("UŻYCIE CPU", "CPU USAGE", "CPU-AUSLASTUNG", "USO DE CPU", "UTILISATION CPU", "ВИКОРИСТАННЯ CPU"),
        ["Dash_Ram"] = E("PAMIĘĆ RAM", "RAM MEMORY", "ARBEITSSPEICHER", "MEMORIA RAM", "MÉMOIRE RAM", "ПАМ'ЯТЬ RAM"),
        ["Dash_Disk"] = E("DYSK (C:)", "DISK (C:)", "DATENTRÄGER (C:)", "DISCO (C:)", "DISQUE (C:)", "ДИСК (C:)"),
        ["Dash_Uptime"] = E("CZAS PRACY", "UPTIME", "BETRIEBSZEIT", "TIEMPO ACTIVO", "TEMPS DE FONCTIONNEMENT", "ЧАС РОБОТИ"),
        ["Dash_CpuLoad"] = E("Obciążenie procesora", "Processor load", "Prozessorauslastung", "Carga del procesador", "Charge du processeur", "Навантаження процесора"),
        ["Dash_SinceBoot"] = E("od ostatniego startu", "since last boot", "seit dem letzten Start", "desde el último inicio", "depuis le dernier démarrage", "від останнього запуску"),
        ["Dash_QuickActions"] = E("SZYBKIE AKCJE", "QUICK ACTIONS", "SCHNELLAKTIONEN", "ACCIONES RÁPIDAS", "ACTIONS RAPIDES", "ШВИДКІ ДІЇ"),
        ["Dash_QuickActionsDesc"] = E(
            "Użyj panelu bocznego aby uruchomić naprawę: Szybka naprawa, Zaawansowana lub własne opcje.",
            "Use the sidebar to run a repair: Quick Fix, Advanced, or custom options.",
            "Nutze die Seitenleiste, um eine Reparatur zu starten: Schnellreparatur, Erweitert oder eigene Optionen.",
            "Usa la barra lateral para ejecutar una reparación: Rápida, Avanzada u opciones personalizadas.",
            "Utilisez la barre latérale pour lancer une réparation : Rapide, Avancée ou options personnalisées.",
            "Скористайтеся бічною панеллю, щоб запустити виправлення: Швидке, Розширене або власні параметри."),

        // Shared
        ["Progress_Title"] = E("POSTĘP", "PROGRESS", "FORTSCHRITT", "PROGRESO", "PROGRESSION", "ПРОГРЕС"),
        ["Btn_Cancel"] = E("Anuluj", "Cancel", "Abbrechen", "Cancelar", "Annuler", "Скасувати"),

        // Quick Fix
        ["QF_Title"] = E("Szybka naprawa", "Quick Fix", "Schnellreparatur", "Reparación rápida", "Réparation rapide", "Швидке виправлення"),
        ["QF_Subtitle"] = E("Wykonuje bezpieczne operacje bez konieczności restartu.", "Runs safe operations with no restart required.", "Führt sichere Vorgänge ohne Neustart aus.", "Ejecuta operaciones seguras sin necesidad de reiniciar.", "Exécute des opérations sûres sans redémarrage.", "Виконує безпечні операції без потреби перезапуску."),
        ["QF_WillDo"] = E("WYKONA:", "WILL DO:", "WIRD AUSGEFÜHRT:", "REALIZARÁ:", "EFFECTUERA :", "ВИКОНАЄ:"),
        ["QF_TempTitle"] = E("Czyszczenie plików tymczasowych", "Clean temporary files", "Temporäre Dateien bereinigen", "Limpiar archivos temporales", "Nettoyer les fichiers temporaires", "Очищення тимчасових файлів"),
        ["QF_TempDesc"] = E("Usuwa zawartość %TEMP% i C:\\Windows\\Temp.", "Removes the contents of %TEMP% and C:\\Windows\\Temp.", "Entfernt den Inhalt von %TEMP% und C:\\Windows\\Temp.", "Elimina el contenido de %TEMP% y C:\\Windows\\Temp.", "Supprime le contenu de %TEMP% et C:\\Windows\\Temp.", "Видаляє вміст %TEMP% і C:\\Windows\\Temp."),
        ["QF_PrefetchTitle"] = E("Czyszczenie Prefetch", "Clean Prefetch", "Prefetch bereinigen", "Limpiar Prefetch", "Nettoyer Prefetch", "Очищення Prefetch"),
        ["QF_PrefetchDesc"] = E("Opróżnia pamięć podręczną Prefetch dla szybszego startu.", "Clears the Prefetch cache for a faster startup.", "Leert den Prefetch-Cache für einen schnelleren Start.", "Vacía la caché de Prefetch para un inicio más rápido.", "Vide le cache Prefetch pour un démarrage plus rapide.", "Очищає кеш Prefetch для швидшого запуску."),
        ["QF_DnsTitle"] = E("Reset cache DNS", "Flush DNS cache", "DNS-Cache zurücksetzen", "Restablecer caché DNS", "Vider le cache DNS", "Скидання кешу DNS"),
        ["QF_DnsDesc"] = E("Czyści cache resolwera DNS — naprawia błędne adresy.", "Clears the DNS resolver cache — fixes bad addresses.", "Leert den DNS-Resolver-Cache — behebt fehlerhafte Adressen.", "Borra la caché del resolvedor DNS — corrige direcciones erróneas.", "Vide le cache du résolveur DNS — corrige les adresses erronées.", "Очищає кеш DNS-резолвера — виправляє помилкові адреси."),
        ["QF_WuTitle"] = E("Cache Windows Update", "Windows Update cache", "Windows-Update-Cache", "Caché de Windows Update", "Cache Windows Update", "Кеш Windows Update"),
        ["QF_WuDesc"] = E("Zatrzymuje usługi WU i czyści folder pobierania.", "Stops WU services and clears the download folder.", "Stoppt WU-Dienste und leert den Download-Ordner.", "Detiene los servicios de WU y limpia la carpeta de descargas.", "Arrête les services WU et vide le dossier de téléchargement.", "Зупиняє служби WU та очищає теку завантажень."),
        ["QF_RunButton"] = E("URUCHOM SZYBKĄ NAPRAWĘ", "RUN QUICK FIX", "SCHNELLREPARATUR STARTEN", "EJECUTAR REPARACIÓN RÁPIDA", "LANCER LA RÉPARATION RAPIDE", "ЗАПУСТИТИ ШВИДКЕ ВИПРАВЛЕННЯ"),
        ["QF_Starting"] = E("Uruchamianie...", "Starting...", "Wird gestartet...", "Iniciando...", "Démarrage...", "Запуск..."),
        ["QF_ConfirmMsg"] = E(
            "Szybka naprawa wyczyści pliki tymczasowe, Prefetch, opróżni cache DNS i wyczyści pamięć podręczną Windows Update.\n\nKontynuować?",
            "Quick Fix will clean temporary files, Prefetch, flush the DNS cache and clear the Windows Update cache.\n\nContinue?",
            "Die Schnellreparatur bereinigt temporäre Dateien, Prefetch, den DNS-Cache und den Windows-Update-Cache.\n\nFortfahren?",
            "La reparación rápida limpiará archivos temporales, Prefetch, la caché DNS y la caché de Windows Update.\n\n¿Continuar?",
            "La réparation rapide nettoiera les fichiers temporaires, Prefetch, le cache DNS et le cache Windows Update.\n\nContinuer ?",
            "Швидке виправлення очистить тимчасові файли, Prefetch, кеш DNS та кеш Windows Update.\n\nПродовжити?"),
        ["QF_ConfirmTitle"] = E("Potwierdź szybką naprawę", "Confirm Quick Fix", "Schnellreparatur bestätigen", "Confirmar reparación rápida", "Confirmer la réparation rapide", "Підтвердіть швидке виправлення"),
        ["QF_StepUserTemp"] = E("Czyszczenie plików tymczasowych użytkownika...", "Cleaning user temp files...", "Benutzer-Temp-Dateien werden bereinigt...", "Limpiando archivos temporales del usuario...", "Nettoyage des fichiers temporaires utilisateur...", "Очищення тимчасових файлів користувача..."),
        ["QF_StepWinTemp"] = E("Czyszczenie Windows\\Temp...", "Cleaning Windows\\Temp...", "Windows\\Temp wird bereinigt...", "Limpiando Windows\\Temp...", "Nettoyage de Windows\\Temp...", "Очищення Windows\\Temp..."),
        ["QF_StepPrefetch"] = E("Czyszczenie Prefetch...", "Cleaning Prefetch...", "Prefetch wird bereinigt...", "Limpiando Prefetch...", "Nettoyage de Prefetch...", "Очищення Prefetch..."),
        ["QF_StepDns"] = E("Reset cache DNS...", "Flushing DNS cache...", "DNS-Cache wird zurückgesetzt...", "Restableciendo caché DNS...", "Vidage du cache DNS...", "Скидання кешу DNS..."),
        ["QF_StepWu"] = E("Czyszczenie cache Windows Update...", "Cleaning Windows Update cache...", "Windows-Update-Cache wird bereinigt...", "Limpiando caché de Windows Update...", "Nettoyage du cache Windows Update...", "Очищення кешу Windows Update..."),
        ["QF_Cancelled"] = E("Anulowano przez użytkownika.", "Cancelled by the user.", "Vom Benutzer abgebrochen.", "Cancelado por el usuario.", "Annulé par l'utilisateur.", "Скасовано користувачем."),
        ["QF_CancelledItem"] = E("✕ Operacja anulowana.", "✕ Operation cancelled.", "✕ Vorgang abgebrochen.", "✕ Operación cancelada.", "✕ Opération annulée.", "✕ Операцію скасовано."),
        ["QF_DoneOk"] = E("Szybka naprawa zakończona pomyślnie.", "Quick Fix completed successfully.", "Schnellreparatur erfolgreich abgeschlossen.", "Reparación rápida completada con éxito.", "Réparation rapide terminée avec succès.", "Швидке виправлення успішно завершено."),
        ["QF_DoneWarn"] = E("Zakończono z ostrzeżeniami — sprawdź wyniki powyżej.", "Finished with warnings — check the results above.", "Mit Warnungen abgeschlossen — Ergebnisse oben prüfen.", "Finalizado con advertencias — revisa los resultados arriba.", "Terminé avec des avertissements — vérifiez les résultats ci-dessus.", "Завершено з попередженнями — перегляньте результати вище."),

        // Advanced Fix
        ["AF_Title"] = E("Zaawansowana naprawa", "Advanced Fix", "Erweiterte Reparatur", "Reparación avanzada", "Réparation avancée", "Розширене виправлення"),
        ["AF_Subtitle"] = E("Uruchamia SFC, DISM i resetuje składniki Windows Update.", "Runs SFC, DISM and resets Windows Update components.", "Führt SFC, DISM aus und setzt Windows-Update-Komponenten zurück.", "Ejecuta SFC, DISM y restablece los componentes de Windows Update.", "Exécute SFC, DISM et réinitialise les composants de Windows Update.", "Запускає SFC, DISM та скидає компоненти Windows Update."),
        ["AF_RestartWarnTitle"] = E("Wymagany restart po zakończeniu", "Restart required when finished", "Nach Abschluss ist ein Neustart erforderlich", "Se requiere reinicio al finalizar", "Redémarrage requis à la fin", "Після завершення потрібен перезапуск"),
        ["AF_RestartWarnDesc"] = E("Zapisz wszystkie otwarte pliki przed uruchomieniem tej operacji.", "Save all open files before starting this operation.", "Speichere alle geöffneten Dateien, bevor du diesen Vorgang startest.", "Guarda todos los archivos abiertos antes de iniciar esta operación.", "Enregistrez tous les fichiers ouverts avant de lancer cette opération.", "Збережіть усі відкриті файли перед запуском цієї операції."),
        ["AF_Operations"] = E("OPERACJE", "OPERATIONS", "VORGÄNGE", "OPERACIONES", "OPÉRATIONS", "ОПЕРАЦІЇ"),
        ["AF_Sfc_Title"] = E("SFC /scannow — Sprawdzenie plików systemowych", "SFC /scannow — System file check", "SFC /scannow — Systemdateiprüfung", "SFC /scannow — Comprobación de archivos del sistema", "SFC /scannow — Vérification des fichiers système", "SFC /scannow — Перевірка системних файлів"),
        ["AF_Sfc_Desc"] = E("Skanuje i naprawia uszkodzone pliki chronionych zasobów systemu Windows.", "Scans and repairs corrupted Windows protected system files.", "Scannt und repariert beschädigte geschützte Windows-Systemdateien.", "Escanea y repara archivos protegidos dañados de Windows.", "Analyse et répare les fichiers système protégés de Windows endommagés.", "Сканує та відновлює пошкоджені захищені системні файли Windows."),
        ["AF_Dism_Title"] = E("DISM /RestoreHealth — Naprawa obrazu systemu", "DISM /RestoreHealth — System image repair", "DISM /RestoreHealth — Systemabbild-Reparatur", "DISM /RestoreHealth — Reparación de imagen del sistema", "DISM /RestoreHealth — Réparation de l'image système", "DISM /RestoreHealth — Відновлення образу системи"),
        ["AF_Dism_Desc"] = E("Sprawdza i przywraca magazyn komponentów systemu Windows.", "Checks and restores the Windows component store.", "Prüft und stellt den Windows-Komponentenspeicher wieder her.", "Comprueba y restaura el almacén de componentes de Windows.", "Vérifie et restaure le magasin de composants Windows.", "Перевіряє та відновлює сховище компонентів Windows."),
        ["AF_Wu_Title"] = E("Reset składników Windows Update", "Reset Windows Update components", "Windows-Update-Komponenten zurücksetzen", "Restablecer componentes de Windows Update", "Réinitialiser les composants Windows Update", "Скидання компонентів Windows Update"),
        ["AF_Wu_Desc"] = E("Zatrzymuje usługi WU, czyści cache SoftwareDistribution i rejestruje DLL-e.", "Stops WU services, clears the SoftwareDistribution cache and re-registers DLLs.", "Stoppt WU-Dienste, leert den SoftwareDistribution-Cache und registriert DLLs neu.", "Detiene los servicios de WU, limpia la caché SoftwareDistribution y vuelve a registrar las DLL.", "Arrête les services WU, vide le cache SoftwareDistribution et réenregistre les DLL.", "Зупиняє служби WU, очищає кеш SoftwareDistribution і повторно реєструє DLL."),
        ["AF_Net_Title"] = E("Reset stosu sieciowego (netsh)", "Network stack reset (netsh)", "Netzwerkstack-Reset (netsh)", "Restablecer pila de red (netsh)", "Réinitialisation de la pile réseau (netsh)", "Скидання мережевого стека (netsh)"),
        ["AF_Net_Desc"] = E("Resetuje stos TCP/IP i ustawienia Winsock.", "Resets the TCP/IP stack and Winsock settings.", "Setzt den TCP/IP-Stack und die Winsock-Einstellungen zurück.", "Restablece la pila TCP/IP y la configuración de Winsock.", "Réinitialise la pile TCP/IP et les paramètres Winsock.", "Скидає стек TCP/IP та налаштування Winsock."),
        ["AF_Duration"] = E("Skanowanie SFC i DISM może potrwać 15–45 minut w zależności od rozmiaru dysku.", "SFC and DISM scans can take 15–45 minutes depending on disk size.", "SFC- und DISM-Scans können je nach Datenträgergröße 15–45 Minuten dauern.", "Los análisis de SFC y DISM pueden tardar de 15 a 45 minutos según el tamaño del disco.", "Les analyses SFC et DISM peuvent prendre 15 à 45 minutes selon la taille du disque.", "Сканування SFC та DISM може тривати 15–45 хвилин залежно від розміру диска."),
        ["AF_RestorePointNote"] = E("Przed uruchomieniem zostanie automatycznie utworzony punkt przywracania systemu.", "A system restore point is created automatically before starting.", "Vor dem Start wird automatisch ein Systemwiederherstellungspunkt erstellt.", "Antes de comenzar se crea automáticamente un punto de restauración del sistema.", "Un point de restauration système est créé automatiquement avant le démarrage.", "Перед запуском автоматично створюється точка відновлення системи."),
        ["AF_RunButton"] = E("URUCHOM ZAAWANSOWANĄ NAPRAWĘ", "RUN ADVANCED FIX", "ERWEITERTE REPARATUR STARTEN", "EJECUTAR REPARACIÓN AVANZADA", "LANCER LA RÉPARATION AVANCÉE", "ЗАПУСТИТИ РОЗШИРЕНЕ ВИПРАВЛЕННЯ"),
        ["AF_LiveOutput"] = E("WYNIK NA ŻYWO", "LIVE OUTPUT", "LIVE-AUSGABE", "SALIDA EN VIVO", "SORTIE EN DIRECT", "ЖИВИЙ ВИВІД"),
        ["AF_RestartBanner"] = E("Operacje zakończone — uruchom ponownie komputer, aby zastosować zmiany.", "Operations finished — restart your computer to apply the changes.", "Vorgänge abgeschlossen — starte den Computer neu, um die Änderungen anzuwenden.", "Operaciones finalizadas — reinicia el equipo para aplicar los cambios.", "Opérations terminées — redémarrez l'ordinateur pour appliquer les modifications.", "Операції завершено — перезавантажте комп'ютер, щоб застосувати зміни."),
        ["Btn_RestartNow"] = E("Uruchom ponownie", "Restart now", "Jetzt neu starten", "Reiniciar ahora", "Redémarrer maintenant", "Перезапустити зараз"),
        ["AF_ConfirmMsg"] = E(
            "Zaawansowana naprawa uruchomi SFC, DISM oraz zresetuje składniki Windows Update.\n\nOperacja może potrwać 15–45 minut. Wymagany będzie restart komputera.\n\nPrzed wykonaniem zostanie utworzony punkt przywracania.\n\nKontynuować?",
            "Advanced Fix will run SFC, DISM and reset Windows Update components.\n\nThe operation may take 15–45 minutes. A computer restart will be required.\n\nA restore point will be created beforehand.\n\nContinue?",
            "Die erweiterte Reparatur führt SFC, DISM aus und setzt Windows-Update-Komponenten zurück.\n\nDer Vorgang kann 15–45 Minuten dauern. Ein Neustart des Computers ist erforderlich.\n\nZuvor wird ein Wiederherstellungspunkt erstellt.\n\nFortfahren?",
            "La reparación avanzada ejecutará SFC, DISM y restablecerá los componentes de Windows Update.\n\nLa operación puede tardar de 15 a 45 minutos. Será necesario reiniciar el equipo.\n\nAntes se creará un punto de restauración.\n\n¿Continuar?",
            "La réparation avancée exécutera SFC, DISM et réinitialisera les composants Windows Update.\n\nL'opération peut prendre 15 à 45 minutes. Un redémarrage de l'ordinateur sera nécessaire.\n\nUn point de restauration sera créé au préalable.\n\nContinuer ?",
            "Розширене виправлення запустить SFC, DISM та скине компоненти Windows Update.\n\nОперація може тривати 15–45 хвилин. Знадобиться перезавантаження комп'ютера.\n\nПеред виконанням буде створено точку відновлення.\n\nПродовжити?"),
        ["AF_ConfirmTitle"] = E("Potwierdź naprawę zaawansowaną", "Confirm Advanced Fix", "Erweiterte Reparatur bestätigen", "Confirmar reparación avanzada", "Confirmer la réparation avancée", "Підтвердіть розширене виправлення"),
        ["AF_RestorePointDesc"] = E("OPSW11 — przed zaawansowaną naprawą", "OPSW11 — before Advanced Fix", "OPSW11 — vor der erweiterten Reparatur", "OPSW11 — antes de la reparación avanzada", "OPSW11 — avant la réparation avancée", "OPSW11 — перед розширеним виправленням"),
        ["AF_CreatingRestore"] = E("Tworzenie punktu przywracania systemu…", "Creating a system restore point…", "Systemwiederherstellungspunkt wird erstellt…", "Creando un punto de restauración del sistema…", "Création d'un point de restauration système…", "Створення точки відновлення системи…"),
        ["AF_StepSfc"] = E("Uruchamianie SFC /scannow…", "Running SFC /scannow…", "SFC /scannow wird ausgeführt…", "Ejecutando SFC /scannow…", "Exécution de SFC /scannow…", "Запуск SFC /scannow…"),
        ["AF_StepDism"] = E("Uruchamianie DISM /RestoreHealth…", "Running DISM /RestoreHealth…", "DISM /RestoreHealth wird ausgeführt…", "Ejecutando DISM /RestoreHealth…", "Exécution de DISM /RestoreHealth…", "Запуск DISM /RestoreHealth…"),
        ["AF_StepWu"] = E("Resetowanie składników Windows Update…", "Resetting Windows Update components…", "Windows-Update-Komponenten werden zurückgesetzt…", "Restableciendo componentes de Windows Update…", "Réinitialisation des composants Windows Update…", "Скидання компонентів Windows Update…"),
        ["AF_Cancelled"] = E("✕ Anulowano przez użytkownika.", "✕ Cancelled by the user.", "✕ Vom Benutzer abgebrochen.", "✕ Cancelado por el usuario.", "✕ Annulé par l'utilisateur.", "✕ Скасовано користувачем."),
        ["AF_StepDone"] = E("✓ Zakończono.", "✓ Done.", "✓ Fertig.", "✓ Completado.", "✓ Terminé.", "✓ Готово."),
        ["AF_AllDone"] = E("Wszystkie operacje zakończone", "All operations completed", "Alle Vorgänge abgeschlossen", "Todas las operaciones completadas", "Toutes les opérations terminées", "Усі операції завершено"),
        ["AF_RestartConfirmMsg"] = E(
            "Komputer zostanie uruchomiony ponownie. Zapisz wszystkie otwarte pliki.\n\nUruchomić ponownie?",
            "The computer will restart. Save all open files.\n\nRestart now?",
            "Der Computer wird neu gestartet. Speichere alle geöffneten Dateien.\n\nJetzt neu starten?",
            "El equipo se reiniciará. Guarda todos los archivos abiertos.\n\n¿Reiniciar ahora?",
            "L'ordinateur va redémarrer. Enregistrez tous les fichiers ouverts.\n\nRedémarrer maintenant ?",
            "Комп'ютер буде перезавантажено. Збережіть усі відкриті файли.\n\nПерезапустити зараз?"),
        ["AF_RestartConfirmTitle"] = E("Potwierdzenie restartu", "Confirm restart", "Neustart bestätigen", "Confirmar reinicio", "Confirmer le redémarrage", "Підтвердження перезапуску"),
        ["AF_ShutdownComment"] = E("OPSW11: restart po naprawie systemu.", "OPSW11: restart after system repair.", "OPSW11: Neustart nach Systemreparatur.", "OPSW11: reinicio tras la reparación del sistema.", "OPSW11 : redémarrage après réparation système.", "OPSW11: перезапуск після відновлення системи."),

        // Custom Fix
        ["CF_Title"] = E("Naprawa własna", "Custom Fix", "Benutzerdefinierte Reparatur", "Reparación personalizada", "Réparation personnalisée", "Власне виправлення"),
        ["CF_Subtitle"] = E("Zaznacz operacje do wykonania i kliknij Uruchom wybrane.", "Select the operations to run and click Run selected.", "Wähle die auszuführenden Vorgänge und klicke auf Ausgewählte starten.", "Selecciona las operaciones a ejecutar y haz clic en Ejecutar seleccionadas.", "Sélectionnez les opérations à exécuter et cliquez sur Exécuter la sélection.", "Виберіть операції для виконання та натисніть Запустити вибрані."),
        ["CF_Group_Cleanup"] = E("CZYSZCZENIE", "CLEANUP", "BEREINIGUNG", "LIMPIEZA", "NETTOYAGE", "ОЧИЩЕННЯ"),
        ["CF_Group_Network"] = E("SIEĆ", "NETWORK", "NETZWERK", "RED", "RÉSEAU", "МЕРЕЖА"),
        ["CF_Group_Repair"] = E("NAPRAWA SYSTEMU", "SYSTEM REPAIR", "SYSTEMREPARATUR", "REPARACIÓN DEL SISTEMA", "RÉPARATION SYSTÈME", "ВІДНОВЛЕННЯ СИСТЕМИ"),
        ["CF_Group_Services"] = E("USŁUGI", "SERVICES", "DIENSTE", "SERVICIOS", "SERVICES", "СЛУЖБИ"),
        ["CF_Group_Disk"] = E("DYSK", "DISK", "DATENTRÄGER", "DISCO", "DISQUE", "ДИСК"),
        ["CF_SafeOnly"] = E("Tylko bezpieczne operacje", "Safe operations only", "Nur sichere Vorgänge", "Solo operaciones seguras", "Opérations sûres uniquement", "Лише безпечні операції"),
        ["CF_SelectAll"] = E("Zaznacz wszystko", "Select all", "Alle auswählen", "Seleccionar todo", "Tout sélectionner", "Вибрати все"),
        ["CF_Count"] = E("{0} operacji wybranych", "{0} operation(s) selected", "{0} Vorgang/Vorgänge ausgewählt", "{0} operación(es) seleccionada(s)", "{0} opération(s) sélectionnée(s)", "Вибрано операцій: {0}"),
        ["Btn_RunSelected"] = E("Uruchom wybrane", "Run selected", "Ausgewählte starten", "Ejecutar seleccionadas", "Exécuter la sélection", "Запустити вибрані"),
        ["CF_ConfirmMsg"] = E("Zostanie uruchomionych {0} operacji.{1}\n\nKontynuować?", "{0} operation(s) will run.{1}\n\nContinue?", "{0} Vorgang/Vorgänge werden ausgeführt.{1}\n\nFortfahren?", "Se ejecutarán {0} operación(es).{1}\n\n¿Continuar?", "{0} opération(s) seront exécutées.{1}\n\nContinuer ?", "Буде запущено операцій: {0}.{1}\n\nПродовжити?"),
        ["CF_DangerNote"] = E("\n\n⚠ Niektóre wybrane operacje mogą być destrukcyjne.", "\n\n⚠ Some selected operations may be destructive.", "\n\n⚠ Einige ausgewählte Vorgänge können destruktiv sein.", "\n\n⚠ Algunas operaciones seleccionadas pueden ser destructivas.", "\n\n⚠ Certaines opérations sélectionnées peuvent être destructrices.", "\n\n⚠ Деякі вибрані операції можуть бути руйнівними."),
        ["CF_ConfirmTitle"] = E("Potwierdź naprawę własną", "Confirm Custom Fix", "Benutzerdefinierte Reparatur bestätigen", "Confirmar reparación personalizada", "Confirmer la réparation personnalisée", "Підтвердіть власне виправлення"),
        ["CF_RestorePointDesc"] = E("OPSW11 — przed naprawą własną", "OPSW11 — before Custom Fix", "OPSW11 — vor der benutzerdefinierten Reparatur", "OPSW11 — antes de la reparación personalizada", "OPSW11 — avant la réparation personnalisée", "OPSW11 — перед власним виправленням"),
        ["CF_Running"] = E("Uruchamianie: {0}", "Running: {0}", "Wird ausgeführt: {0}", "Ejecutando: {0}", "Exécution : {0}", "Виконується: {0}"),
        ["CF_Done"] = E("Zakończono", "Completed", "Abgeschlossen", "Completado", "Terminé", "Завершено"),
        ["CF_Cancelled"] = E("anulowano", "cancelled", "abgebrochen", "cancelado", "annulé", "скасовано"),
        ["CF_ErrorPrefix"] = E("błąd", "error", "Fehler", "error", "erreur", "помилка"),

        // Operations (name + description)
        ["Op_UserTemp_Name"] = E("Wyczyść %TEMP% (pliki użytkownika)", "Clean %TEMP% (user files)", "%TEMP% bereinigen (Benutzerdateien)", "Limpiar %TEMP% (archivos de usuario)", "Nettoyer %TEMP% (fichiers utilisateur)", "Очистити %TEMP% (файли користувача)"),
        ["Op_UserTemp_Desc"] = E("Usuwa zawartość %TEMP%. Pliki w użyciu są pomijane.", "Removes the contents of %TEMP%. Files in use are skipped.", "Entfernt den Inhalt von %TEMP%. Dateien in Benutzung werden übersprungen.", "Elimina el contenido de %TEMP%. Los archivos en uso se omiten.", "Supprime le contenu de %TEMP%. Les fichiers en cours d'utilisation sont ignorés.", "Видаляє вміст %TEMP%. Файли, що використовуються, пропускаються."),
        ["Op_WinTemp_Name"] = E("Wyczyść C:\\Windows\\Temp", "Clean C:\\Windows\\Temp", "C:\\Windows\\Temp bereinigen", "Limpiar C:\\Windows\\Temp", "Nettoyer C:\\Windows\\Temp", "Очистити C:\\Windows\\Temp"),
        ["Op_WinTemp_Desc"] = E("Usuwa tymczasowe pliki systemowe. Zablokowane pomijane.", "Removes temporary system files. Locked ones are skipped.", "Entfernt temporäre Systemdateien. Gesperrte werden übersprungen.", "Elimina archivos temporales del sistema. Los bloqueados se omiten.", "Supprime les fichiers système temporaires. Ceux verrouillés sont ignorés.", "Видаляє тимчасові системні файли. Заблоковані пропускаються."),
        ["Op_Prefetch_Name"] = E("Wyczyść Prefetch", "Clean Prefetch", "Prefetch bereinigen", "Limpiar Prefetch", "Nettoyer Prefetch", "Очистити Prefetch"),
        ["Op_Prefetch_Desc"] = E("Usuwa pliki .pf — zostaną odtworzone przy następnym użyciu.", "Removes .pf files — they are recreated on next use.", "Entfernt .pf-Dateien — sie werden bei der nächsten Nutzung neu erstellt.", "Elimina archivos .pf — se recrean en el próximo uso.", "Supprime les fichiers .pf — recréés à la prochaine utilisation.", "Видаляє файли .pf — вони відтворяться під час наступного використання."),
        ["Op_RecycleBin_Name"] = E("Opróżnij Kosz", "Empty Recycle Bin", "Papierkorb leeren", "Vaciar Papelera", "Vider la Corbeille", "Очистити Кошик"),
        ["Op_RecycleBin_Desc"] = E("Trwale usuwa wszystkie elementy z Kosza.", "Permanently removes all items from the Recycle Bin.", "Entfernt alle Elemente endgültig aus dem Papierkorb.", "Elimina permanentemente todos los elementos de la Papelera.", "Supprime définitivement tous les éléments de la Corbeille.", "Остаточно видаляє всі елементи з Кошика."),
        ["Op_WuCache_Name"] = E("Cache Windows Update", "Windows Update cache", "Windows-Update-Cache", "Caché de Windows Update", "Cache Windows Update", "Кеш Windows Update"),
        ["Op_WuCache_Desc"] = E("Zatrzymuje usługi WU i usuwa pobrane pliki aktualizacji.", "Stops WU services and removes downloaded update files.", "Stoppt WU-Dienste und entfernt heruntergeladene Update-Dateien.", "Detiene los servicios de WU y elimina los archivos de actualización descargados.", "Arrête les services WU et supprime les fichiers de mise à jour téléchargés.", "Зупиняє служби WU та видаляє завантажені файли оновлень."),
        ["Op_Dns_Name"] = E("Reset cache DNS", "Flush DNS cache", "DNS-Cache zurücksetzen", "Restablecer caché DNS", "Vider le cache DNS", "Скидання кешу DNS"),
        ["Op_Dns_Desc"] = E("Uruchamia ipconfig /flushdns. Naprawia błędne rozwiązania DNS.", "Runs ipconfig /flushdns. Fixes bad DNS resolutions.", "Führt ipconfig /flushdns aus. Behebt fehlerhafte DNS-Auflösungen.", "Ejecuta ipconfig /flushdns. Corrige resoluciones DNS erróneas.", "Exécute ipconfig /flushdns. Corrige les résolutions DNS erronées.", "Виконує ipconfig /flushdns. Виправляє помилкові DNS-запити."),
        ["Op_NetRestart_Name"] = E("Restart usług sieciowych", "Restart network services", "Netzwerkdienste neu starten", "Reiniciar servicios de red", "Redémarrer les services réseau", "Перезапуск мережевих служб"),
        ["Op_NetRestart_Desc"] = E("Restartuje DHCP, DNS Client, NLA i Network Profile Manager.", "Restarts DHCP, DNS Client, NLA and Network Profile Manager.", "Startet DHCP, DNS-Client, NLA und Netzwerkprofil-Manager neu.", "Reinicia DHCP, Cliente DNS, NLA y Administrador de perfiles de red.", "Redémarre DHCP, Client DNS, NLA et Gestionnaire de profils réseau.", "Перезапускає DHCP, DNS-клієнт, NLA та диспетчер мережевих профілів."),
        ["Op_NetReset_Name"] = E("Pełny reset stosu sieciowego (netsh)", "Full network stack reset (netsh)", "Vollständiger Netzwerkstack-Reset (netsh)", "Restablecimiento completo de la pila de red (netsh)", "Réinitialisation complète de la pile réseau (netsh)", "Повне скидання мережевого стека (netsh)"),
        ["Op_NetReset_Desc"] = E("Resetuje Winsock i TCP/IP. Wymagany restart.", "Resets Winsock and TCP/IP. Restart required.", "Setzt Winsock und TCP/IP zurück. Neustart erforderlich.", "Restablece Winsock y TCP/IP. Requiere reinicio.", "Réinitialise Winsock et TCP/IP. Redémarrage requis.", "Скидає Winsock і TCP/IP. Потрібен перезапуск."),
        ["Op_Sfc_Name"] = E("SFC /scannow — Sprawdzenie plików systemowych", "SFC /scannow — System file check", "SFC /scannow — Systemdateiprüfung", "SFC /scannow — Comprobación de archivos del sistema", "SFC /scannow — Vérification des fichiers système", "SFC /scannow — Перевірка системних файлів"),
        ["Op_Sfc_Desc"] = E("Skanuje i naprawia uszkodzone pliki Windows. Trwa 10–30 minut.", "Scans and repairs corrupted Windows files. Takes 10–30 minutes.", "Scannt und repariert beschädigte Windows-Dateien. Dauert 10–30 Minuten.", "Escanea y repara archivos dañados de Windows. Tarda 10–30 minutos.", "Analyse et répare les fichiers Windows endommagés. Dure 10 à 30 minutes.", "Сканує та відновлює пошкоджені файли Windows. Триває 10–30 хвилин."),
        ["Op_Dism_Name"] = E("DISM /RestoreHealth", "DISM /RestoreHealth", "DISM /RestoreHealth", "DISM /RestoreHealth", "DISM /RestoreHealth", "DISM /RestoreHealth"),
        ["Op_Dism_Desc"] = E("Naprawia magazyn składników Windows. 15–45 minut.", "Repairs the Windows component store. 15–45 minutes.", "Repariert den Windows-Komponentenspeicher. 15–45 Minuten.", "Repara el almacén de componentes de Windows. 15–45 minutos.", "Répare le magasin de composants Windows. 15 à 45 minutes.", "Відновлює сховище компонентів Windows. 15–45 хвилин."),
        ["Op_WuReset_Name"] = E("Reset składników Windows Update", "Reset Windows Update components", "Windows-Update-Komponenten zurücksetzen", "Restablecer componentes de Windows Update", "Réinitialiser les composants Windows Update", "Скидання компонентів Windows Update"),
        ["Op_WuReset_Desc"] = E("Zatrzymuje WU, czyści SoftwareDistribution i catroot2.", "Stops WU, clears SoftwareDistribution and catroot2.", "Stoppt WU, leert SoftwareDistribution und catroot2.", "Detiene WU, limpia SoftwareDistribution y catroot2.", "Arrête WU, vide SoftwareDistribution et catroot2.", "Зупиняє WU, очищає SoftwareDistribution і catroot2."),
        ["Op_SysMain_Name"] = E("Wyłącz SysMain (Superfetch)", "Disable SysMain (Superfetch)", "SysMain (Superfetch) deaktivieren", "Desactivar SysMain (Superfetch)", "Désactiver SysMain (Superfetch)", "Вимкнути SysMain (Superfetch)"),
        ["Op_SysMain_Desc"] = E("Ustawia SysMain na Ręczny. Zmniejsza zużycie RAM — zalecane na SSD.", "Sets SysMain to Manual. Reduces RAM usage — recommended on SSDs.", "Setzt SysMain auf Manuell. Reduziert RAM-Nutzung — auf SSDs empfohlen.", "Establece SysMain en Manual. Reduce el uso de RAM — recomendado en SSD.", "Définit SysMain sur Manuel. Réduit l'utilisation de la RAM — recommandé sur SSD.", "Встановлює SysMain у режим «Вручну». Зменшує використання RAM — рекомендовано для SSD."),
        ["Op_Search_Name"] = E("Wyłącz indeksowanie Windows Search", "Disable Windows Search indexing", "Windows Search-Indizierung deaktivieren", "Desactivar indexación de Windows Search", "Désactiver l'indexation Windows Search", "Вимкнути індексування Windows Search"),
        ["Op_Search_Desc"] = E("Ustawia WSearch na Ręczny. Zmniejsza I/O.", "Sets WSearch to Manual. Reduces disk I/O.", "Setzt WSearch auf Manuell. Reduziert die Datenträger-E/A.", "Establece WSearch en Manual. Reduce la E/S de disco.", "Définit WSearch sur Manuel. Réduit les E/S disque.", "Встановлює WSearch у режим «Вручну». Зменшує дискові операції."),
        ["Op_Telemetry_Name"] = E("Wyłącz telemetrię (DiagTrack)", "Disable telemetry (DiagTrack)", "Telemetrie deaktivieren (DiagTrack)", "Desactivar telemetría (DiagTrack)", "Désactiver la télémétrie (DiagTrack)", "Вимкнути телеметрію (DiagTrack)"),
        ["Op_Telemetry_Desc"] = E("Ustawia DiagTrack na Ręczny. Prywatność i oszczędność CPU.", "Sets DiagTrack to Manual. Privacy and CPU savings.", "Setzt DiagTrack auf Manuell. Datenschutz und CPU-Einsparung.", "Establece DiagTrack en Manual. Privacidad y ahorro de CPU.", "Définit DiagTrack sur Manuel. Confidentialité et économie de CPU.", "Встановлює DiagTrack у режим «Вручну». Приватність та економія CPU."),
        ["Op_OptimizeDisk_Name"] = E("Optymalizuj dysk systemowy (C:)", "Optimize system drive (C:)", "Systemlaufwerk optimieren (C:)", "Optimizar unidad del sistema (C:)", "Optimiser le disque système (C:)", "Оптимізувати системний диск (C:)"),
        ["Op_OptimizeDisk_Desc"] = E("Wykrywa SSD/HDD i uruchamia TRIM (SSD) lub defragmentację (HDD).", "Detects SSD/HDD and runs TRIM (SSD) or defragmentation (HDD).", "Erkennt SSD/HDD und führt TRIM (SSD) oder Defragmentierung (HDD) aus.", "Detecta SSD/HDD y ejecuta TRIM (SSD) o desfragmentación (HDD).", "Détecte SSD/HDD et exécute TRIM (SSD) ou défragmentation (HDD).", "Визначає SSD/HDD і запускає TRIM (SSD) або дефрагментацію (HDD)."),

        // Logs
        ["Log_Title"] = E("Dziennik zdarzeń", "Event log", "Ereignisprotokoll", "Registro de eventos", "Journal des événements", "Журнал подій"),
        ["Log_Subtitle"] = E("Historia wszystkich operacji wykonanych w tej sesji.", "History of all operations performed in this session.", "Verlauf aller in dieser Sitzung durchgeführten Vorgänge.", "Historial de todas las operaciones realizadas en esta sesión.", "Historique de toutes les opérations effectuées dans cette session.", "Історія всіх операцій, виконаних у цій сесії."),
        ["Log_Filter"] = E("FILTR:", "FILTER:", "FILTER:", "FILTRO:", "FILTRE :", "ФІЛЬТР:"),
        ["Log_All"] = E("Wszystko", "All", "Alle", "Todo", "Tout", "Усе"),
        ["Log_Warning"] = E("Ostrzeżenie", "Warning", "Warnung", "Advertencia", "Avertissement", "Попередження"),
        ["Log_Error"] = E("Błąd", "Error", "Fehler", "Error", "Erreur", "Помилка"),
        ["Log_Success"] = E("Sukces", "Success", "Erfolg", "Éxito", "Succès", "Успіх"),
        ["Log_OpenFile"] = E("Otwórz plik dziennika", "Open log file", "Protokolldatei öffnen", "Abrir archivo de registro", "Ouvrir le fichier journal", "Відкрити файл журналу"),
        ["Log_Clear"] = E("Wyczyść", "Clear", "Löschen", "Borrar", "Effacer", "Очистити"),
        ["Log_Export"] = E("Eksportuj", "Export", "Exportieren", "Exportar", "Exporter", "Експорт"),
        ["Log_Count"] = E("{0} wpisów", "{0} entries", "{0} Einträge", "{0} entradas", "{0} entrées", "Записів: {0}"),
        ["Log_NotCreated"] = E("Plik dziennika nie został jeszcze utworzony.", "The log file has not been created yet.", "Die Protokolldatei wurde noch nicht erstellt.", "El archivo de registro aún no se ha creado.", "Le fichier journal n'a pas encore été créé.", "Файл журналу ще не створено."),
        ["Log_DialogTitle"] = E("Dziennik", "Log", "Protokoll", "Registro", "Journal", "Журнал"),
        ["Log_ClearConfirm"] = E("Wyczyścić wszystkie wpisy z bieżącej sesji?", "Clear all entries from the current session?", "Alle Einträge der aktuellen Sitzung löschen?", "¿Borrar todas las entradas de la sesión actual?", "Effacer toutes les entrées de la session actuelle ?", "Очистити всі записи поточної сесії?"),
        ["Log_ClearTitle"] = E("Wyczyść dziennik", "Clear log", "Protokoll löschen", "Borrar registro", "Effacer le journal", "Очистити журнал"),
        ["Log_ExportTitle"] = E("Eksport dziennika", "Export log", "Protokoll exportieren", "Exportar registro", "Exporter le journal", "Експорт журналу"),
        ["Log_ExportDone"] = E("Zapisano dziennik do: {0}", "Log saved to: {0}", "Protokoll gespeichert unter: {0}", "Registro guardado en: {0}", "Journal enregistré dans : {0}", "Журнал збережено до: {0}"),
        ["Log_ExportEmpty"] = E("Dziennik jest pusty — brak wpisów do eksportu.", "The log is empty — nothing to export.", "Das Protokoll ist leer — nichts zu exportieren.", "El registro está vacío — nada que exportar.", "Le journal est vide — rien à exporter.", "Журнал порожній — немає що експортувати."),
    };
}
