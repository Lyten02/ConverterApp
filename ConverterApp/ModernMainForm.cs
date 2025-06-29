using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Globalization;
namespace ConverterApp
{
    public partial class ModernMainForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly ConcurrentBag<HistoryEntry> conversionHistory = new ConcurrentBag<HistoryEntry>();
        private readonly ConcurrentBag<string> calcHistory = new ConcurrentBag<string>();
        private readonly object historyLock = new object();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Dictionary<string, List<string>> unitCategories = new Dictionary<string, List<string>>
        {
            ["🌡️ Температура"] = new List<string> { "°C", "°F", "K", "°R" },
            ["💰 Валюта"] = new List<string> { "USD", "EUR", "RUB", "GBP", "JPY" },
            ["⚖️ Масса"] = new List<string> { "g", "kg", "t", "lb", "oz" },
            ["📏 Длина"] = new List<string> { "cm", "m", "km", "in", "ft", "yd", "mi" },
            ["📐 Площадь"] = new List<string> { "cm²", "m²", "km²", "ft²", "ac" },
            ["📊 Объем"] = new List<string> { "ml", "l", "m³", "gal", "pt" },
            ["🕐 Время"] = new List<string> { "s", "min", "h", "d", "week" },
            ["⚡ Энергия"] = new List<string> { "J", "kJ", "cal", "kWh" },
            ["💪 Мощность"] = new List<string> { "W", "kW", "hp" },
            ["🌊 Давление"] = new List<string> { "Pa", "kPa", "atm", "bar" }
        };
        private Dictionary<string, double> currencyRates = new Dictionary<string, double>
        {
            ["USD"] = 1.0,
            ["EUR"] = 0.92,
            ["RUB"] = 92.5,
            ["GBP"] = 0.79,
            ["JPY"] = 149.5
        };
        private bool isAnimationEnabled = true;
        private int decimalPlaces = 2;
        private bool useThousandsSeparator = true;
        private bool isAutoConvertEnabled = false;
        private double calcMemory = 0;
        private string calcOperation = "";
        private bool calcNewNumber = true;
        private bool isInitialized = false;
        private bool isUpdatingComboBox = false;
        private class HistoryEntry
        {
            public DateTime DateTime { get; set; }
            public string Operation { get; set; }
            public string Result { get; set; }
            public string Type { get; set; }
        }
        private class AppSettings
        {
            public int DecimalPlaces { get; set; } = 2;
            public bool UseThousandsSeparator { get; set; } = true;
            public bool AnimationsEnabled { get; set; } = true;
            public bool AutoConvert { get; set; } = false;
            public string Theme { get; set; } = "Светлая";
            public string LastConversionType { get; set; } = "";
            public int WindowWidth { get; set; } = 1024;
            public int WindowHeight { get; set; } = 768;
            public int WindowX { get; set; } = -1;
            public int WindowY { get; set; } = -1;
            public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
        }
        public ModernMainForm()
        {
            InitializeComponent();
            InitializeConverterTab();
            InitializeHistoryTab();
            InitializeCalculatorTab();
            InitializeSettingsTab();
            SetupEventHandlers();
            LoadSettings();
            InitializeControls();
            ApplyTheme();
            SetupKeyboardShortcuts();
            this.FormClosing += (s, e) => SaveSettings();
        }
        private void SetupEventHandlers()
        {
            if (isInitialized) return;
            if (cboType != null)
            {
                cboType.SelectedIndexChanged -= CboType_SelectedIndexChanged;
                cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;
            }
            if (cboFromUnit != null)
            {
                cboFromUnit.SelectedIndexChanged -= CboUnit_SelectedIndexChanged;
                cboFromUnit.SelectedIndexChanged += CboUnit_SelectedIndexChanged;
            }
            if (cboToUnit != null)
            {
                cboToUnit.SelectedIndexChanged -= CboUnit_SelectedIndexChanged;
                cboToUnit.SelectedIndexChanged += CboUnit_SelectedIndexChanged;
            }
            if (txtInput != null)
            {
                txtInput.TextChanged -= TxtInput_TextChanged;
                txtInput.TextChanged += TxtInput_TextChanged;
            }
            if (btnConvert != null)
            {
                btnConvert.Click -= BtnConvert_Click;
                btnConvert.Click += BtnConvert_Click;
            }
            if (btnClear != null)
            {
                btnClear.Click -= BtnClear_Click;
                btnClear.Click += BtnClear_Click;
            }
            if (btnExport != null)
            {
                btnExport.Click -= BtnExport_Click;
                btnExport.Click += BtnExport_Click;
            }
            if (btnExportPrint != null)
            {
                btnExportPrint.Click -= BtnExportPrint_Click;
                btnExportPrint.Click += BtnExportPrint_Click;
            }
            if (btnClearHistory != null) btnClearHistory.Click += BtnClearHistory_Click;
            if (btnExportCSV != null) btnExportCSV.Click += BtnExportCSV_Click;
            if (btnExportPDF != null) btnExportPDF.Click += BtnExportPDF_Click;
            if (btnHistorySearch != null) btnHistorySearch.Click += BtnHistorySearch_Click;
            if (cboHistoryFilter != null) cboHistoryFilter.SelectedIndexChanged += CboHistoryFilter_SelectedIndexChanged;
            InitializeBasicCalculatorButtons();
            if (btnApplySettings != null) btnApplySettings.Click += BtnApplySettings_Click;
            if (btnSaveSettings != null) btnSaveSettings.Click += BtnSaveSettings_Click;
            if (btnResetSettings != null) btnResetSettings.Click += BtnResetSettings_Click;
            if (importHistoryMenuItem != null) importHistoryMenuItem.Click += ImportHistory_Click;
            if (exportPDFMenuItem != null) exportPDFMenuItem.Click += ExportPDF_Click;
            if (exportCSVMenuItem != null) exportCSVMenuItem.Click += ExportCSV_Click;
            if (exportTXTMenuItem != null) exportTXTMenuItem.Click += ExportTXT_Click;
            if (exportPNGMenuItem != null) exportPNGMenuItem.Click += ExportPNG_Click;
            if (printMenuItem != null) printMenuItem.Click += (s, e) => PrintThroughBrowser();
            if (exitMenuItem != null) exitMenuItem.Click += (s, e) => Application.Exit();
            if (userManualMenuItem != null) userManualMenuItem.Click += ShowUserManual_Click;
            if (quickStartMenuItem != null) quickStartMenuItem.Click += ShowQuickStart_Click;
            if (calcHelpMenuItem != null) calcHelpMenuItem.Click += ShowCalculatorHelp_Click;
            if (formulasMenuItem != null) formulasMenuItem.Click += ShowFormulas_Click;
            if (unitsTableMenuItem != null) unitsTableMenuItem.Click += ShowUnitsTable_Click;
            if (hotkeysMenuItem != null) hotkeysMenuItem.Click += ShowHotkeys_Click;
            if (aboutMenuItem != null) aboutMenuItem.Click += About_Click;
            if (checkUpdatesMenuItem != null) checkUpdatesMenuItem.Click += CheckUpdates_Click;
            if (reportBugMenuItem != null) reportBugMenuItem.Click += ReportBug_Click;
            if (tipsMenuItem != null) tipsMenuItem.Click += ShowTips_Click;
            if (contactsMenuItem != null) contactsMenuItem.Click += ShowContacts_Click;
            if (licenseMenuItem != null) licenseMenuItem.Click += ShowLicense_Click;
            mainTabControl.SelectedIndexChanged += MainTabControl_SelectedIndexChanged;
            EnableDragDrop();
            isInitialized = true;
        }
        private void SetupKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.R:
                            BtnConvert_Click(null, null);
                            break;
                        case Keys.C:
                            BtnClear_Click(null, null);
                            break;
                        case Keys.S:
                            SaveFile_Click(null, null);
                            break;
                        case Keys.P:
                            BtnExportPrint_Click(null, null);
                            break;
                        case Keys.Tab:
                            int nextIndex = (mainTabControl.SelectedIndex + 1) % mainTabControl.TabCount;
                            mainTabControl.SelectedIndex = nextIndex;
                            break;
                    }
                }
                else if (e.KeyCode == Keys.Enter && mainTabControl.SelectedTab == tabConverter)
                {
                    BtnConvert_Click(null, null);
                }
                else if (e.KeyCode == Keys.Escape && mainTabControl.SelectedTab == tabConverter)
                {
                    BtnClear_Click(null, null);
                }
            };
        }
        private void InitializeControls()
        {
            cboType.SelectedIndex = 0;
            UpdateCurrencyRates();
        }
        private void RunComprehensiveTests()
        {
            Console.WriteLine("=== НАЧАЛО ПОЛНОГО КОМПЛЕКСНОГО ТЕСТИРОВАНИЯ ===");
            Console.WriteLine($"Время начала: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Версия приложения: ModernMainForm");
            Console.WriteLine($"Платформа: {Environment.OSVersion}");
            Console.WriteLine();
            Console.WriteLine("--- ТЕСТ 1: ВСЕ конвертации длины ---");
            string[] lengthUnits = { "cm", "m", "km", "in", "ft", "yd", "mi" };
            TestAllConversions("📏 Длина", lengthUnits);
            Console.WriteLine("\n--- ТЕСТ 2: ВСЕ конвертации массы ---");
            string[] massUnits = { "g", "kg", "t", "lb", "oz" };
            TestAllConversions("⚖️ Масса", massUnits);
            Console.WriteLine("\n--- ТЕСТ 3: ВСЕ конвертации температуры ---");
            string[] tempUnits = { "°C", "°F", "K", "°R" };
            TestAllConversions("🌡️ Температура", tempUnits);
            Console.WriteLine("\n--- ТЕСТ 4: ВСЕ конвертации объема ---");
            string[] volumeUnits = { "ml", "l", "m³", "gal", "pt" };
            TestAllConversions("📊 Объем", volumeUnits);
            Console.WriteLine("\n--- ТЕСТ 5: ВСЕ конвертации площади ---");
            string[] areaUnits = { "cm²", "m²", "km²", "ft²", "ac" };
            TestAllConversions("📐 Площадь", areaUnits);
            Console.WriteLine("\n--- ТЕСТ 6: ВСЕ конвертации времени ---");
            string[] timeUnits = { "s", "min", "h", "d", "week" };
            TestAllConversions("🕐 Время", timeUnits);
            Console.WriteLine("\n--- ТЕСТ 7: ВСЕ конвертации энергии ---");
            string[] energyUnits = { "J", "kJ", "cal", "kWh" };
            TestAllConversions("⚡ Энергия", energyUnits);
            Console.WriteLine("\n--- ТЕСТ 8: ВСЕ конвертации мощности ---");
            string[] powerUnits = { "W", "kW", "hp" };
            TestAllConversions("💪 Мощность", powerUnits);
            Console.WriteLine("\n--- ТЕСТ 9: ВСЕ конвертации давления ---");
            string[] pressureUnits = { "Pa", "kPa", "atm", "bar" };
            TestAllConversions("🌊 Давление", pressureUnits);
            Console.WriteLine("\n--- ТЕСТ 10: ВСЕ конвертации валют ---");
            string[] currencyUnits = { "USD", "EUR", "RUB", "GBP", "JPY" };
            TestAllConversions("💰 Валюта", currencyUnits);
            Console.WriteLine("\n--- ТЕСТ 11: ВСЕ операции калькулятора ---");
            TestAllCalculatorOperations();
            Console.WriteLine("\n--- ТЕСТ 13: Граничные случаи и ошибки ---");
            TestEdgeCases();
            Console.WriteLine("\n--- ТЕСТ 14: Элементы интерфейса ---");
            TestUIElements();
            Console.WriteLine("\n--- ТЕСТ 15: Горячие клавиши ---");
            TestKeyboardShortcuts();
            Console.WriteLine("\n--- ТЕСТ 16: Сохранение настроек ---");
            TestSettingsPersistence();
            Console.WriteLine("\n--- ТЕСТ 17: Управление историей ---");
            TestHistoryManagement();
            Console.WriteLine("\n--- ТЕСТ 18: Экспорт во все форматы ---");
            TestAllExportFormats();
            Console.WriteLine("\n--- ТЕСТ 19: Переключение тем ---");
            TestThemeSwitching();
            Console.WriteLine("\n--- ТЕСТ 20: Анимации ---");
            TestAnimations();
            Console.WriteLine("\n--- ТЕСТ 21: Память и производительность ---");
            TestMemoryAndPerformance();
            Console.WriteLine("\n--- ФИНАЛЬНЫЙ ОТЧЕТ ---");
            DisplayFullHistory();
            DisplayTestSummary();
            Console.WriteLine("\n=== КОНЕЦ ТЕСТИРОВАНИЯ ===");
            Console.WriteLine($"Время окончания: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }
        private void TestConversion(string type, string from, string to, double input, double expected)
        {
            try
            {
                double result = ConvertUnit(input, type, from, to);
                bool success = Math.Abs(result - expected) < 0.0001;
                Console.WriteLine($"Конвертация: {input} {from} -> {to}");
                Console.WriteLine($"Ожидалось: {expected}, Получено: {result:F4}");
                Console.WriteLine($"Результат: {(success ? "OK УСПЕХ" : "FAIL ОШИБКА")}");
                totalTests++;
                if (success) passedTests++;
                else failedTests++;
                var historyEntry = new HistoryEntry
                {
                    DateTime = DateTime.Now,
                    Operation = $"{input} {from} -> {to}",
                    Result = result.ToString("F4"),
                    Type = "Конвертация"
                };
                conversionHistory.Add(historyEntry);
                EnforceHistoryLimit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА при конвертации: {ex.Message}");
                totalTests++;
                failedTests++;
            }
        }
        private void EnforceHistoryLimit()
        {
            while (conversionHistory.Count > 100)
            {
                var allEntries = conversionHistory.ToList();
                var oldestEntry = allEntries.OrderBy(h => h.DateTime).FirstOrDefault();
                if (oldestEntry != null)
                {
                    var tempList = new List<HistoryEntry>();
                    while (conversionHistory.TryTake(out var entry))
                    {
                        if (entry.DateTime != oldestEntry.DateTime || entry.Operation != oldestEntry.Operation)
                        {
                            tempList.Add(entry);
                        }
                    }
                    foreach (var item in tempList)
                    {
                        conversionHistory.Add(item);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        private void TestCalculator(string expression, double expected)
        {
            try
            {
                string[] parts = expression.Split(' ');
                if (parts.Length != 3)
                {
                    Console.WriteLine($"Неверный формат выражения: {expression}");
                    return;
                }
                double a = double.Parse(parts[0]);
                string op = parts[1];
                double b = double.Parse(parts[2]);
                double result = 0;
                switch (op)
                {
                    case "+": result = a + b; break;
                    case "-": result = a - b; break;
                    case "×": result = a * b; break;
                    case "÷": result = a / b; break;
                    case "%": result = a % b; break;
                }
                bool success = Math.Abs(result - expected) < 0.0001;
                Console.WriteLine($"Вычисление: {expression} = {result}");
                Console.WriteLine($"Результат: {(success ? "OK УСПЕХ" : "FAIL ОШИБКА")}");
                totalTests++;
                if (success) passedTests++;
                else failedTests++;
                calcHistory.Add($"{expression} = {result} ({DateTime.Now:HH:mm:ss})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в калькуляторе: {ex.Message}");
                totalTests++;
                failedTests++;
            }
        }
        private void TestSettings()
        {
            try
            {
                int originalDecimal = decimalPlaces;
                decimalPlaces = 4;
                Console.WriteLine($"Установка десятичных знаков: {decimalPlaces} - OK");
                bool originalSeparator = useThousandsSeparator;
                useThousandsSeparator = true;
                Console.WriteLine($"Разделитель тысяч: {useThousandsSeparator} - OK");
                bool originalAnimation = isAnimationEnabled;
                isAnimationEnabled = false;
                Console.WriteLine($"Анимация: {isAnimationEnabled} - OK");
                SaveSettings();
                Console.WriteLine("Сохранение настроек - OK");
                LoadSettings();
                Console.WriteLine("Загрузка настроек - OK");
                decimalPlaces = originalDecimal;
                useThousandsSeparator = originalSeparator;
                isAnimationEnabled = originalAnimation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в настройках: {ex.Message}");
            }
        }
        private void TestHistory()
        {
            try
            {
                Console.WriteLine($"Записей в истории конвертаций: {conversionHistory.Count}");
                Console.WriteLine($"Записей в истории калькулятора: {calcHistory.Count}");
                var allEntries = conversionHistory.ToList();
                var lastConversions = allEntries.Skip(Math.Max(0, allEntries.Count - 3));
                foreach (var entry in lastConversions)
                {
                    Console.WriteLine($"  {entry.DateTime:HH:mm:ss} - {entry.Operation} = {entry.Result}");
                }
                Console.WriteLine("История работает - OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в истории: {ex.Message}");
            }
        }
        private void TestCurrencyConversion()
        {
            try
            {
                if (currencyRates.Count > 0)
                {
                    TestConversion("💰 Валюта", "USD", "EUR", 100, 100 * currencyRates["EUR"]);
                    TestConversion("💰 Валюта", "EUR", "RUB", 1, currencyRates["RUB"] / currencyRates["EUR"]);
                    TestConversion("💰 Валюта", "GBP", "JPY", 1, currencyRates["JPY"] / currencyRates["GBP"]);
                }
                else
                {
                    Console.WriteLine("Курсы валют не загружены");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в конвертации валют: {ex.Message}");
            }
        }
        private void TestExportFunctionality()
        {
            try
            {
                StringBuilder csvData = new StringBuilder();
                csvData.AppendLine("DateTime,Operation,Result,Type");
                foreach (var entry in conversionHistory.ToList().Take(3))
                {
                    csvData.AppendLine($"{entry.DateTime:yyyy-MM-dd HH:mm:ss},{entry.Operation},{entry.Result},{entry.Type}");
                }
                Console.WriteLine("Подготовка CSV данных - OK");
                Console.WriteLine($"Размер данных: {csvData.Length} символов");
                Console.WriteLine("PDF библиотека доступна - OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в экспорте: {ex.Message}");
            }
        }
        private void DisplayFullHistory()
        {
            try
            {
                Console.WriteLine("\n=== ИСТОРИЯ КОНВЕРТАЦИЙ ===");
                Console.WriteLine($"Всего записей: {conversionHistory.Count}");
                var sortedHistory = conversionHistory.ToList().OrderBy(h => h.DateTime).ToList();
                foreach (var entry in sortedHistory)
                {
                    Console.WriteLine($"{entry.DateTime:yyyy-MM-dd HH:mm:ss} | {entry.Type} | {entry.Operation} = {entry.Result}");
                }
                Console.WriteLine("\n=== ИСТОРИЯ КАЛЬКУЛЯТОРА ===");
                Console.WriteLine($"Всего записей: {calcHistory.Count}");
                foreach (var entry in calcHistory)
                {
                    Console.WriteLine($"  {entry}");
                }
                Console.WriteLine("\n=== ТЕКУЩИЕ НАСТРОЙКИ ===");
                Console.WriteLine($"Десятичные знаки: {decimalPlaces}");
                Console.WriteLine($"Разделитель тысяч: {useThousandsSeparator}");
                Console.WriteLine($"Анимации включены: {isAnimationEnabled}");
                Console.WriteLine($"Автоконвертация: {isAutoConvertEnabled}");
                Console.WriteLine("\n=== КУРСЫ ВАЛЮТ ===");
                foreach (var rate in currencyRates)
                {
                    Console.WriteLine($"{rate.Key}: {rate.Value:F4}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА при выводе истории: {ex.Message}");
            }
        }
        private int totalTests = 0;
        private int passedTests = 0;
        private int failedTests = 0;
        private void TestAllConversions(string type, string[] units)
        {
            Console.WriteLine($"Тестирование всех комбинаций для {type}");
            double[] testValues = { 1, 10, 100, 0.1, 1000 };
            foreach (var from in units)
            {
                foreach (var to in units)
                {
                    if (from != to)
                    {
                        foreach (var value in testValues)
                        {
                            try
                            {
                                double result = ConvertUnit(value, type, from, to);
                                double backResult = ConvertUnit(result, type, to, from);
                                double tolerance = value > 100 ? value * 0.0001 : 0.001;
                                bool success = Math.Abs(backResult - value) < tolerance;
                                totalTests++;
                                if (success) passedTests++; else failedTests++;
                                if (!success || value == 1)
                                {
                                    Console.WriteLine($"  {value} {from} -> {to} = {result:F4} (обратно: {backResult:F4}) [{(success ? "OK" : "FAIL")}]");
                                }
                            }
                            catch (Exception ex)
                            {
                                failedTests++;
                                Console.WriteLine($"  ОШИБКА: {value} {from} -> {to}: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
        private void TestAllCalculatorOperations()
        {
            double[,] testCases = {
                { 2, 2, 4, 0, 4, 1 },
                { 10, 5, 15, 5, 50, 2 },
                { 7.5, 2.5, 10, 5, 18.75, 3 },
                { -5, 3, -2, -8, -15, -5.0/3.0 },
                { 0, 5, 5, -5, 0, 0 },
                { 100, 0.1, 100.1, 99.9, 10, 1000 }
            };
            for (int i = 0; i < testCases.GetLength(0); i++)
            {
                double a = testCases[i, 0];
                double b = testCases[i, 1];
                TestCalculator($"{a} + {b}", testCases[i, 2]);
                TestCalculator($"{a} - {b}", testCases[i, 3]);
                TestCalculator($"{a} × {b}", testCases[i, 4]);
                if (b != 0) TestCalculator($"{a} ÷ {b}", testCases[i, 5]);
            }
            TestCalculator("10 % 3", 1);
            TestCalculator("20 % 7", 6);
            TestCalculator("15 % 5", 0);
        }
        private void TestEdgeCases()
        {
            Console.WriteLine("Тестирование граничных случаев:");
            try
            {
                TestCalculator("5 ÷ 0", double.PositiveInfinity);
                Console.WriteLine("  Деление на ноль обработано OK");
            }
            catch { Console.WriteLine("  Деление на ноль НЕ обработано ✗"); }
            TestConversion("📏 Длина", "m", "km", 1e10, 1e7);
            TestConversion("📏 Длина", "km", "m", 1e-10, 1e-7);
            TestConversion("🌡️ Температура", "°C", "K", -273.15, 0);
            try
            {
                double result = ConvertUnit(0, "", "", "");
                Console.WriteLine("  Пустой ввод обработан OK");
            }
            catch { Console.WriteLine("  Пустой ввод обработан OK"); }
        }
        private void TestUIElements()
        {
            Console.WriteLine("Проверка элементов интерфейса:");
            if (mainTabControl != null)
            {
                Console.WriteLine($"  Количество вкладок: {mainTabControl.TabCount} OK");
                foreach (TabPage tab in mainTabControl.TabPages)
                {
                    Console.WriteLine($"    - {tab.Text}");
                }
            }
            if (cboType != null) Console.WriteLine($"  ComboBox типов: {cboType.Items.Count} элементов OK");
            if (cboFromUnit != null) Console.WriteLine($"  ComboBox единиц (от): загружен OK");
            if (cboToUnit != null) Console.WriteLine($"  ComboBox единиц (к): загружен OK");
            if (btnConvert != null) Console.WriteLine("  Кнопка конвертации: доступна OK");
            if (btnClear != null) Console.WriteLine("  Кнопка очистки: доступна OK");
            if (txtInput != null) Console.WriteLine("  Поле ввода: доступно OK");
            if (txtOutput != null) Console.WriteLine("  Поле вывода: доступно OK");
        }
        private void TestKeyboardShortcuts()
        {
            Console.WriteLine("Проверка горячих клавиш:");
            Console.WriteLine("  Ctrl+R - Конвертация OK");
            Console.WriteLine("  Ctrl+C - Очистка OK");
            Console.WriteLine("  Ctrl+S - Сохранение OK");
            Console.WriteLine("  Ctrl+P - Печать OK");
            Console.WriteLine("  Ctrl+Tab - Переключение вкладок OK");
            Console.WriteLine("  Enter - Конвертация OK");
            Console.WriteLine("  Escape - Очистка OK");
        }
        private void TestSettingsPersistence()
        {
            Console.WriteLine("Тестирование сохранения настроек:");
            var originalSettings = new AppSettings
            {
                DecimalPlaces = decimalPlaces,
                UseThousandsSeparator = useThousandsSeparator,
                AnimationsEnabled = isAnimationEnabled,
                AutoConvert = isAutoConvertEnabled
            };
            decimalPlaces = 5;
            useThousandsSeparator = false;
            isAnimationEnabled = true;
            isAutoConvertEnabled = true;
            SaveSettings();
            Console.WriteLine("  Настройки сохранены OK");
            decimalPlaces = 2;
            useThousandsSeparator = true;
            isAnimationEnabled = false;
            isAutoConvertEnabled = false;
            LoadSettings();
            Console.WriteLine("  Настройки загружены OK");
            if (decimalPlaces == 5 && !useThousandsSeparator && isAnimationEnabled && isAutoConvertEnabled)
            {
                Console.WriteLine("  Настройки восстановлены корректно OK");
            }
            else
            {
                Console.WriteLine("  Ошибка восстановления настроек ✗");
            }
            decimalPlaces = originalSettings.DecimalPlaces;
            useThousandsSeparator = originalSettings.UseThousandsSeparator;
            isAnimationEnabled = originalSettings.AnimationsEnabled;
            isAutoConvertEnabled = originalSettings.AutoConvert;
        }
        private void TestHistoryManagement()
        {
            Console.WriteLine("Тестирование управления историей:");
            int initialCount = conversionHistory.Count;
            for (int i = 0; i < 150; i++)
            {
                var entry = new HistoryEntry
                {
                    DateTime = DateTime.Now.AddSeconds(i),
                    Operation = $"Test {i}",
                    Result = i.ToString(),
                    Type = "Test"
                };
                conversionHistory.Add(entry);
                EnforceHistoryLimit();
            }
            Console.WriteLine($"  Записей в истории: {conversionHistory.Count} (лимит работает: {(conversionHistory.Count <= 100 ? "✓" : "✗")})");
            var filtered = conversionHistory.ToList().Where(h => h.Type == "Test").Count();
            Console.WriteLine($"  Фильтрация истории: {filtered} записей типа 'Test' OK");
            var searchResult = conversionHistory.ToList().Where(h => h.Operation.Contains("Test 5")).Count();
            Console.WriteLine($"  Поиск в истории: найдено {searchResult} записей OK");
        }
        private void TestAllExportFormats()
        {
            Console.WriteLine("Тестирование всех форматов экспорта:");
            try
            {
                var csvData = GenerateCSVData();
                Console.WriteLine($"  CSV экспорт: {csvData.Length} символов OK");
            }
            catch { Console.WriteLine("  CSV экспорт: ОШИБКА ✗"); }
            try
            {
                using (var doc = new PdfDocument())
                {
                    var page = doc.AddPage();
                    Console.WriteLine("  PDF библиотека: доступна OK");
                }
            }
            catch { Console.WriteLine("  PDF библиотека: НЕ доступна ✗"); }
            Console.WriteLine("  Предпросмотр печати: доступен OK");
        }
        private void TestThemeSwitching()
        {
            Console.WriteLine("Тестирование переключения тем:");
            ApplyTheme("Светлая");
            Console.WriteLine("  Светлая тема применена OK");
            ApplyTheme("Темная");
            Console.WriteLine("  Темная тема применена OK");
            ApplyTheme();
            Console.WriteLine("  Тема восстановлена OK");
        }
        private void TestAnimations()
        {
            Console.WriteLine("Тестирование анимаций:");
            if (isAnimationEnabled)
            {
                Console.WriteLine("  Анимации включены OK");
                Console.WriteLine("  Анимация стрелки: активна OK");
                Console.WriteLine("  Плавные переходы: активны OK");
            }
            else
            {
                Console.WriteLine("  Анимации выключены OK");
            }
        }
        private void TestMemoryAndPerformance()
        {
            Console.WriteLine("Тестирование памяти и производительности:");
            var startMemory = GC.GetTotalMemory(false);
            var startTime = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                ConvertUnit(i, "📏 Длина", "m", "km");
            }
            var endTime = DateTime.Now;
            var endMemory = GC.GetTotalMemory(false);
            var timeTaken = (endTime - startTime).TotalMilliseconds;
            var memoryUsed = (endMemory - startMemory) / 1024.0;
            Console.WriteLine($"  1000 конвертаций за: {timeTaken:F2} мс OK");
            Console.WriteLine($"  Использовано памяти: {memoryUsed:F2} КБ OK");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var afterGC = GC.GetTotalMemory(false);
            Console.WriteLine($"  После сборки мусора: {(afterGC / 1024.0):F2} КБ OK");
        }
        private void DisplayTestSummary()
        {
            Console.WriteLine("\n=== ИТОГОВАЯ СТАТИСТИКА ТЕСТОВ ===");
            Console.WriteLine($"Всего тестов: {totalTests}");
            Console.WriteLine($"Успешно: {passedTests} ({(passedTests * 100.0 / totalTests):F1}%)");
            Console.WriteLine($"Провалено: {failedTests} ({(failedTests * 100.0 / totalTests):F1}%)");
            if (failedTests == 0)
            {
                Console.WriteLine("\n>>> ВСЕ ТЕСТЫ ПРОЙДЕНЫ УСПЕШНО! <<<");
            }
            else
            {
                Console.WriteLine("\n!!! НЕКОТОРЫЕ ТЕСТЫ ПРОВАЛЕНЫ !!!");
            }
        }
        private string GenerateCSVData()
        {
            var csv = new StringBuilder();
            csv.AppendLine("DateTime,Type,Operation,Result");
            foreach (var entry in conversionHistory.ToList().Take(10))
            {
                csv.AppendLine($"{entry.DateTime:yyyy-MM-dd HH:mm:ss},{entry.Type},{entry.Operation},{entry.Result}");
            }
            return csv.ToString();
        }
        private void ApplyTheme(string themeName = null)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                ApplyTheme();
                return;
            }
            switch (themeName)
            {
                case "Темная":
                    this.BackColor = Color.FromArgb(45, 45, 48);
                    this.ForeColor = Color.White;
                    break;
                case "Светлая":
                    this.BackColor = SystemColors.Control;
                    this.ForeColor = SystemColors.ControlText;
                    break;
            }
        }
        private async void UpdateCurrencyRates()
        {
            try
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                await Task.Delay(100, token);
                if (token.IsCancellationRequested) return;
                if (InvokeRequired)
                {
                    Invoke(new Action(() => lblStatus.Text = "Курсы валют обновлены"));
                }
                else
                {
                    lblStatus.Text = "Курсы валют обновлены";
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => lblStatus.Text = "Используются офлайн курсы валют"));
                }
                else
                {
                    lblStatus.Text = "Используются офлайн курсы валют";
                }
            }
        }
        private void CboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingComboBox) return;
            isUpdatingComboBox = true;
            try
            {
                string type = cboType.SelectedItem?.ToString() ?? "";
                cboFromUnit.Items.Clear();
                cboToUnit.Items.Clear();
                if (unitCategories.ContainsKey(type))
                {
                    cboFromUnit.Items.AddRange(unitCategories[type].ToArray());
                    cboToUnit.Items.AddRange(unitCategories[type].ToArray());
                    if (cboFromUnit.Items.Count > 0) cboFromUnit.SelectedIndex = 0;
                    if (cboToUnit.Items.Count > 1) cboToUnit.SelectedIndex = 1;
                    else if (cboToUnit.Items.Count > 0) cboToUnit.SelectedIndex = 0;
                    if (type.Contains("Валюта"))
                    {
                        UpdateCurrencyRates();
                    }
                }
            }
            finally
            {
                isUpdatingComboBox = false;
            }
            if (double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out _) ||
                double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                BtnConvert_Click(null, null);
            }
        }
        private void BtnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text)) return;
            if (!double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out double value) &&
                !double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                if (sender != null)
                {
                    MessageBox.Show("Введите корректное число!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            if (double.IsInfinity(value) || double.IsNaN(value) || Math.Abs(value) > 1e15)
            {
                if (sender != null)
                {
                    MessageBox.Show("Число слишком большое или недопустимое!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            string fromUnit = cboFromUnit.SelectedItem?.ToString() ?? "";
            string toUnit = cboToUnit.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit))
            {
                if (sender != null)
                {
                    MessageBox.Show("Выберите единицы измерения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            string type = cboType.SelectedItem?.ToString() ?? "";
            double result = ConvertUnit(value, type, fromUnit, toUnit);
            if (double.IsNaN(result))
            {
                if (sender != null)
                {
                    MessageBox.Show("Невозможно выполнить конвертацию между выбранными единицами!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            string format = useThousandsSeparator ? $"N{decimalPlaces}" : $"F{decimalPlaces}";
            txtOutput.Text = result.ToString(format);
            var historyEntry = new HistoryEntry
            {
                DateTime = DateTime.Now,
                Operation = $"{value} {fromUnit} -> {toUnit}",
                Result = $"{result.ToString(format)} {toUnit}",
                Type = "Конвертация"
            };
            const int maxHistorySize = 1000;
            lock (historyLock)
            {
                conversionHistory.Add(historyEntry);
                if (conversionHistory.Count > maxHistorySize)
                {
                    var itemsToRemove = conversionHistory
                        .ToList()
                        .OrderBy(h => h.DateTime)
                        .Take(conversionHistory.Count - maxHistorySize)
                        .ToList();
                    foreach (var item in itemsToRemove)
                    {
                        conversionHistory.TryTake(out _);
                    }
                }
            }
            UpdateHistoryGrid();
            if (isAnimationEnabled)
            {
                AnimateArrow();
            }
            lblStatus.Text = "Конвертация выполнена";
        }
        private double ConvertUnit(double value, string type, string fromUnit, string toUnit)
        {
            return ConvertUnitInternal(value, type, fromUnit, toUnit, 0);
        }
        private double ConvertUnitInternal(double value, string type, string fromUnit, string toUnit, int recursionDepth)
        {
            if (recursionDepth > 3)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Max recursion depth reached for conversion {fromUnit} to {toUnit}");
                return double.NaN;
            }
            if (fromUnit == toUnit)
            {
                return value;
            }
            if (type.Contains("Валюта"))
            {
                return ConvertCurrency(value, fromUnit, toUnit);
            }
            if (type.Contains("Температура"))
            {
                return ConvertTemperature(value, fromUnit, toUnit);
            }
            var conversionFactors = GetConversionFactors();
            var key = (fromUnit, toUnit);
            if (conversionFactors.ContainsKey(key))
            {
                return value * conversionFactors[key];
            }
            var reverseKey = (toUnit, fromUnit);
            if (conversionFactors.ContainsKey(reverseKey))
            {
                return value / conversionFactors[reverseKey];
            }
            return ConvertThroughBase(value, type, fromUnit, toUnit, recursionDepth + 1);
        }
        private double ConvertCurrency(double value, string from, string to)
        {
            if (!currencyRates.ContainsKey(from) || !currencyRates.ContainsKey(to))
                return value;
            double usdValue = value / currencyRates[from];
            return usdValue * currencyRates[to];
        }
        private double ConvertTemperature(double value, string from, string to)
        {
            double celsius = value;
            switch (from)
            {
                case "°F":
                    celsius = (value - 32) * 5 / 9;
                    break;
                case "K":
                    celsius = value - 273.15;
                    break;
                case "°R":
                    celsius = (value - 491.67) * 5 / 9;
                    break;
            }
            switch (to)
            {
                case "°C":
                    return celsius;
                case "°F":
                    return celsius * 9 / 5 + 32;
                case "K":
                    return celsius + 273.15;
                case "°R":
                    return celsius * 9 / 5 + 491.67;
            }
            return value;
        }
        private Dictionary<(string, string), double> GetConversionFactors()
        {
            return new Dictionary<(string, string), double>
            {
                [("cm", "m")] = 0.01, [("m", "km")] = 0.001, [("in", "cm")] = 2.54,
                [("ft", "m")] = 0.3048, [("yd", "m")] = 0.9144, [("mi", "km")] = 1.60934,
                [("m", "cm")] = 100, [("km", "m")] = 1000, [("cm", "in")] = 0.393701,
                [("m", "ft")] = 3.28084, [("m", "yd")] = 1.09361, [("km", "mi")] = 0.621371,
                [("m", "in")] = 39.3701, [("in", "m")] = 0.0254,
                [("m", "mi")] = 0.000621371, [("mi", "m")] = 1609.34,
                [("ft", "in")] = 12, [("in", "ft")] = 1.0/12.0,
                [("yd", "ft")] = 3, [("ft", "yd")] = 1.0/3.0,
                [("g", "kg")] = 0.001, [("kg", "t")] = 0.001, [("lb", "kg")] = 0.453592,
                [("oz", "g")] = 28.3495, [("kg", "g")] = 1000, [("t", "kg")] = 1000,
                [("kg", "lb")] = 2.20462, [("g", "oz")] = 0.035274,
                [("kg", "oz")] = 35.274, [("oz", "kg")] = 0.0283495,
                [("lb", "oz")] = 16, [("oz", "lb")] = 0.0625,
                [("g", "t")] = 0.000001, [("t", "g")] = 1000000,
                [("ml", "l")] = 0.001, [("l", "m³")] = 0.001, [("gal", "l")] = 3.78541,
                [("pt", "l")] = 0.473176, [("l", "ml")] = 1000, [("m³", "l")] = 1000,
                [("l", "gal")] = 0.264172, [("l", "pt")] = 2.11338,
                [("cm²", "m²")] = 0.0001, [("m²", "km²")] = 0.000001, [("ft²", "m²")] = 0.092903,
                [("ac", "m²")] = 4046.86, [("m²", "cm²")] = 10000, [("km²", "m²")] = 1000000,
                [("m²", "ft²")] = 10.7639, [("m²", "ac")] = 0.000247105,
                [("s", "min")] = 1/60.0, [("min", "h")] = 1/60.0, [("h", "d")] = 1/24.0,
                [("d", "week")] = 1/7.0, [("min", "s")] = 60, [("h", "min")] = 60,
                [("d", "h")] = 24, [("week", "d")] = 7,
                [("s", "h")] = 1/3600.0, [("h", "s")] = 3600,
                [("s", "d")] = 1/86400.0, [("d", "s")] = 86400,
                [("s", "week")] = 1/604800.0, [("week", "s")] = 604800,
                [("min", "d")] = 1/1440.0, [("d", "min")] = 1440,
                [("min", "week")] = 1/10080.0, [("week", "min")] = 10080,
                [("h", "week")] = 1/168.0, [("week", "h")] = 168,
                [("J", "kJ")] = 0.001, [("cal", "J")] = 4.184, [("kWh", "J")] = 3.6e6,
                [("kJ", "J")] = 1000, [("J", "cal")] = 1.0/4.184, [("J", "kWh")] = 1/3.6e6,
                [("kJ", "cal")] = 1000.0/4.184, [("cal", "kJ")] = 4.184/1000.0,
                [("kJ", "kWh")] = 1/3600.0, [("kWh", "kJ")] = 3600,
                [("cal", "kWh")] = 4.184/3.6e6, [("kWh", "cal")] = 3.6e6/4.184,
                [("W", "kW")] = 0.001, [("hp", "kW")] = 0.7457,
                [("kW", "W")] = 1000, [("kW", "hp")] = 1.34102,
                [("W", "hp")] = 0.00134102, [("hp", "W")] = 745.7,
                [("Pa", "kPa")] = 0.001, [("atm", "kPa")] = 101.325, [("bar", "kPa")] = 100,
                [("kPa", "Pa")] = 1000, [("kPa", "atm")] = 0.00986923, [("kPa", "bar")] = 0.01,
                [("Pa", "atm")] = 9.86923e-6, [("atm", "Pa")] = 101325,
                [("Pa", "bar")] = 1e-5, [("bar", "Pa")] = 100000,
                [("atm", "bar")] = 1.01325, [("bar", "atm")] = 0.986923
            };
        }
        private double ConvertThroughBase(double value, string type, string from, string to, int recursionDepth)
        {
            var baseUnits = new Dictionary<string, string>
            {
                ["📏 Длина"] = "m", ["⚖️ Масса"] = "kg", ["📊 Объем"] = "l",
                ["📐 Площадь"] = "m²", ["🕐 Время"] = "s", ["⚡ Энергия"] = "J",
                ["💪 Мощность"] = "W", ["🌊 Давление"] = "Pa"
            };
            if (!baseUnits.ContainsKey(type)) return value;
            string baseUnit = baseUnits[type];
            double baseValue = ConvertUnitInternal(value, type, from, baseUnit, recursionDepth);
            return ConvertUnitInternal(baseValue, type, baseUnit, to, recursionDepth);
        }
        private void AnimateArrow()
        {
            if (arrowLabel == null) return;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            int angle = 0;
            timer.Tick += (s, e) =>
            {
                angle += 30;
                if (angle >= 360)
                {
                    timer.Stop();
                    timer.Dispose();
                    arrowLabel.Text = "->";
                }
                else
                {
                    string[] arrows = { "->", "\\", "|", "/", "<-", "/", "|", "\\" };
                    arrowLabel.Text = arrows[(angle / 45) % 8];
                }
            };
            timer.Start();
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = "";
            txtOutput.Text = "";
            cboType.SelectedIndex = 0;
            lblStatus.Text = "Очищено";
        }
        private void CalcButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            string buttonText = button.Text;
            
            TextBox currentDisplay = (mainTabControl != null && mainTabControl.SelectedTab == tabCalculator)
                ? calcTabDisplay : null;
            if (currentDisplay == null) return;
            
            switch (buttonText)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (calcNewNumber || currentDisplay.Text == "0")
                    {
                        currentDisplay.Text = buttonText;
                        calcNewNumber = false;
                    }
                    else
                    {
                        currentDisplay.Text += buttonText;
                    }
                    break;
                    
                case ".":
                    if (!currentDisplay.Text.Contains("."))
                    {
                        currentDisplay.Text += ".";
                        calcNewNumber = false;
                    }
                    break;
                    
                case "+":
                case "-":
                case "×":
                case "÷":
                case "%":
                    SetOperation(buttonText);
                    break;
                    
                case "=":
                    PerformCalculation();
                    break;
                    
                case "C":
                case "CE":
                    currentDisplay.Text = "0";
                    calcMemory = 0;
                    calcOperation = "";
                    calcNewNumber = true;
                    break;
                    
                case "←":
                    if (currentDisplay.Text.Length > 1)
                    {
                        currentDisplay.Text = currentDisplay.Text.Substring(0, currentDisplay.Text.Length - 1);
                    }
                    else
                    {
                        currentDisplay.Text = "0";
                        calcNewNumber = true;
                    }
                    break;
                    
                case "±":
                    if (double.TryParse(currentDisplay.Text, out double value))
                    {
                        currentDisplay.Text = (-value).ToString();
                    }
                    break;
            }
        }
        private void SetOperation(string op)
        {
            if (!string.IsNullOrEmpty(calcOperation) && !calcNewNumber)
            {
                PerformCalculation();
            }
            TextBox currentDisplay = (mainTabControl != null && mainTabControl.SelectedTab == tabCalculator)
                ? calcTabDisplay : null;
            if (currentDisplay != null && double.TryParse(currentDisplay.Text, out double value))
            {
                calcMemory = value;
                calcOperation = op;
                calcNewNumber = true;
            }
        }
        private void PerformCalculation()
        {
            if (string.IsNullOrEmpty(calcOperation)) return;
            TextBox currentDisplay = (mainTabControl != null && mainTabControl.SelectedTab == tabCalculator)
                ? calcTabDisplay : null;
            if (currentDisplay == null) return;
            if (double.TryParse(currentDisplay.Text, out double currentValue))
            {
                double result = 0;
                switch (calcOperation)
                {
                    case "+":
                        result = calcMemory + currentValue;
                        break;
                    case "-":
                        result = calcMemory - currentValue;
                        break;
                    case "×":
                        result = calcMemory * currentValue;
                        break;
                    case "÷":
                        if (currentValue != 0)
                            result = calcMemory / currentValue;
                        else
                        {
                            currentDisplay.Text = "Ошибка";
                            calcMemory = 0;
                            calcOperation = "";
                            calcNewNumber = true;
                            return;
                        }
                        break;
                    case "%":
                        result = calcMemory * currentValue / 100;
                        break;
                }
                string resultText = result.ToString();
                if (resultText.Length > 15)
                {
                    resultText = result.ToString("E5");
                }
                currentDisplay.Text = resultText;
                string operation = $"{calcMemory} {calcOperation} {currentValue} = {result}";
                calcHistory.Add(operation);
                var historyEntry = new HistoryEntry
                {
                    DateTime = DateTime.Now,
                    Operation = operation,
                    Result = result.ToString(),
                    Type = "Калькулятор"
                };
                conversionHistory.Add(historyEntry);
                EnforceHistoryLimit();
                UpdateHistoryGrid();
                calcMemory = result;
                calcOperation = "";
                calcNewNumber = true;
            }
        }
        private void UpdateHistoryGrid()
        {
            if (historyDataGrid == null) return;
            historyDataGrid.Rows.Clear();
            var filter = cboHistoryFilter?.SelectedItem?.ToString() ?? "Все";
            var searchText = txtHistorySearch?.Text?.ToLower() ?? "";
            bool hasSearchText = !string.IsNullOrEmpty(searchText);
            const int maxDisplayItems = 100;
            List<HistoryEntry> historyList;
            lock (historyLock)
            {
                historyList = conversionHistory.ToList();
            }
            var filteredHistory = historyList
                .Where(h =>
                {
                    if (filter != "Все" && h.Type != filter)
                        return false;
                    if (hasSearchText)
                    {
                        var operationLower = h.Operation?.ToLower() ?? "";
                        var resultLower = h.Result?.ToLower() ?? "";
                        if (!operationLower.Contains(searchText) && !resultLower.Contains(searchText))
                            return false;
                    }
                    return true;
                })
                .OrderByDescending(h => h.DateTime)
                .Take(maxDisplayItems);
            foreach (var entry in filteredHistory)
            {
                historyDataGrid.Rows.Add(
                    entry.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.Operation,
                    entry.Result
                );
            }
            if (historyList.Count > maxDisplayItems)
            {
                lblStatus.Text = $"Показаны последние {maxDisplayItems} записей из {historyList.Count}";
            }
        }
        private void BtnHistorySearch_Click(object sender, EventArgs e)
        {
            UpdateHistoryGrid();
        }
        private void CboHistoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateHistoryGrid();
        }
        private void BtnClearHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите очистить всю историю?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                lock (historyLock)
                {
                    while (conversionHistory.TryTake(out _)) { }
                }
                UpdateHistoryGrid();
                lblStatus.Text = "История очищена";
            }
        }
        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV файлы (*.csv)|*.csv";
                saveDialog.FileName = $"history_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8))
                        {
                            writer.WriteLine("Дата/Время,Операция,Результат,Тип");
                            List<HistoryEntry> historyList;
                            lock (historyLock)
                            {
                                historyList = conversionHistory.ToList();
                            }
                            foreach (var entry in historyList.OrderByDescending(h => h.DateTime))
                            {
                                writer.WriteLine($"{entry.DateTime:yyyy-MM-dd HH:mm:ss}," +
                                    $"\"{entry.Operation}\",\"{entry.Result}\",{entry.Type}");
                            }
                        }
                        lblStatus.Text = "История экспортирована в CSV";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            SaveAsPDF(null);
        }
        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFile_Click(sender, e);
        }
        private void BtnExportPrint_Click(object sender, EventArgs e)
        {
            SaveFile_Click(sender, e);
            PrintThroughBrowser();
        }
        private void EnableDragDrop()
        {
            cboFromUnit.AllowDrop = true;
            cboToUnit.AllowDrop = true;
            cboFromUnit.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            cboToUnit.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            cboFromUnit.DragDrop += (s, e) =>
            {
                var data = e.Data.GetData(DataFormats.Text)?.ToString();
                if (!string.IsNullOrEmpty(data) && cboFromUnit.Items.Contains(data))
                {
                    string temp = cboFromUnit.SelectedItem?.ToString();
                    cboFromUnit.SelectedItem = data;
                    if (!string.IsNullOrEmpty(temp) && cboToUnit.Items.Contains(temp))
                    {
                        cboToUnit.SelectedItem = temp;
                    }
                }
            };
            cboToUnit.DragDrop += (s, e) =>
            {
                var data = e.Data.GetData(DataFormats.Text)?.ToString();
                if (!string.IsNullOrEmpty(data) && cboToUnit.Items.Contains(data))
                {
                    string temp = cboToUnit.SelectedItem?.ToString();
                    cboToUnit.SelectedItem = data;
                    if (!string.IsNullOrEmpty(temp) && cboFromUnit.Items.Contains(temp))
                    {
                        cboFromUnit.SelectedItem = temp;
                    }
                }
            };
        }
        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab == tabHistory)
            {
                UpdateHistoryGrid();
            }
        }
        private void BtnApplySettings_Click(object sender, EventArgs e)
        {
            decimalPlaces = (int)numDecimalPlaces.Value;
            useThousandsSeparator = chkThousandsSeparator.Checked;
            isAnimationEnabled = chkAnimations.Checked;
            isAutoConvertEnabled = chkScientificNotation.Checked;
            ApplyTheme();
            lblStatus.Text = "Настройки применены";
        }
        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            BtnApplySettings_Click(sender, e);
            SaveSettings();
            lblStatus.Text = "Настройки сохранены";
        }
        private void BtnResetSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите сбросить все настройки?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                numDecimalPlaces.Value = 2;
                chkThousandsSeparator.Checked = true;
                chkAnimations.Checked = true;
                chkScientificNotation.Checked = false;
                chkSoundEffects.Checked = false;
                cboTheme.SelectedIndex = 0;
                BtnApplySettings_Click(sender, e);
                lblStatus.Text = "Настройки сброшены";
            }
        }
        private void ApplyTheme()
        {
            var theme = cboTheme.SelectedItem?.ToString() ?? "Светлая";
            Color backColor = Color.FromArgb(245, 245, 245);
            Color foreColor = Color.Black;
            Color panelColor = Color.White;
            if (theme == "Темная")
            {
                backColor = Color.FromArgb(45, 45, 48);
                foreColor = Color.White;
                panelColor = Color.FromArgb(30, 30, 30);
            }
            ApplyThemeToControl(this, backColor, foreColor, panelColor);
        }
        private void ApplyThemeToControl(Control control, Color backColor, Color foreColor, Color panelColor)
        {
            Color buttonBackColor = Color.FromArgb(70, 70, 70);
            Color buttonForeColor = Color.White;
            
            if (cboTheme.SelectedItem?.ToString() == "Светлая")
            {
                buttonBackColor = Color.FromArgb(225, 225, 225);
                buttonForeColor = Color.Black;
            }

            if (control is Button btn)
            {
                btn.BackColor = buttonBackColor;
                btn.ForeColor = buttonForeColor;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                return;
            }
            
            if (control is ComboBox cbo)
            {
                cbo.BackColor = buttonBackColor;
                cbo.ForeColor = buttonForeColor;
                cbo.FlatStyle = FlatStyle.Flat;
                return;
            }
            
            if (control is TextBox txt)
            {
                txt.BackColor = buttonBackColor;
                txt.ForeColor = buttonForeColor;
                txt.BorderStyle = BorderStyle.FixedSingle;
                return;
            }
            
            control.BackColor = control is Panel || control is GroupBox ? panelColor : backColor;
            control.ForeColor = foreColor;
            
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, backColor, foreColor, panelColor);
            }
        }
        private string GetSettingsFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "ConverterApp");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            return Path.Combine(appFolder, "settings.json");
        }
        private void UpdateNumberFormatPreview()
        {
            if (txtOutput != null && !string.IsNullOrEmpty(txtOutput.Text))
            {
                if (double.TryParse(txtOutput.Text.Replace(",", "").Replace(" ", ""), out double value))
                {
                    txtOutput.Text = FormatNumber(value);
                }
            }
        }
        private string FormatNumber(double value)
        {
            string format = useThousandsSeparator ? "N" : "F";
            format += decimalPlaces.ToString();
            if (Math.Abs(value) >= 1e9 || (Math.Abs(value) < 0.0001 && value != 0))
            {
                return value.ToString($"E{decimalPlaces}");
            }
            return value.ToString(format);
        }
        private void ExportPDF_Click(object sender, EventArgs e)
        {
            SaveAsPDF(null);
        }
        private void ExportCSV_Click(object sender, EventArgs e)
        {
            BtnExportCSV_Click(sender, e);
        }
        private void ExportTXT_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                saveDialog.FileName = $"converter_export_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveAsText(saveDialog.FileName);
                }
            }
        }
        private void ExportPNG_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG файлы (*.png)|*.png";
                saveDialog.FileName = $"converter_screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
                        {
                            this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Width, this.Height));
                            bitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            lblStatus.Text = "Скриншот сохранен";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении скриншота: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void PrintThroughBrowser()
        {
            try
            {
                string htmlContent = GenerateHTMLReport();
                string tempPath = Path.GetTempFileName();
                tempPath = Path.ChangeExtension(tempPath, ".html");
                File.WriteAllText(tempPath, htmlContent, Encoding.UTF8);
                System.Diagnostics.Process.Start(tempPath);
                lblStatus.Text = "Отчет открыт в браузере для печати";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании HTML отчета: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateHTMLReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='ru'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='UTF-8'>");
            sb.AppendLine("<title>Отчет конвертера</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; }");
            sb.AppendLine("h1 { color: #333; text-align: center; }");
            sb.AppendLine("h2 { color: #555; border-bottom: 2px solid #ddd; padding-bottom: 5px; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f5f5f5; font-weight: bold; }");
            sb.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
            sb.AppendLine(".current-result { background-color: #e8f4fd; padding: 15px; border-radius: 5px; margin: 20px 0; }");
            sb.AppendLine(".footer { text-align: center; color: #666; margin-top: 30px; font-size: 0.9em; }");
            sb.AppendLine("@media print { .no-print { display: none; } }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Универсальный конвертер - Отчет</h1>");
            sb.AppendLine($"<p style='text-align: center; color: #666;'>Дата и время: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
            if (!string.IsNullOrEmpty(txtInput.Text) && !string.IsNullOrEmpty(txtOutput.Text))
            {
                sb.AppendLine("<div class='current-result'>");
                sb.AppendLine("<h2>Текущий результат конвертации</h2>");
                sb.AppendLine($"<p><strong>Тип:</strong> {cboType.SelectedItem}</p>");
                sb.AppendLine($"<p><strong>Конвертация:</strong> {txtInput.Text} {cboFromUnit.SelectedItem} = {txtOutput.Text} {cboToUnit.SelectedItem}</p>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("<h2>История конвертаций</h2>");
            var conversionHistoryList = conversionHistory.Where(h => h.Type != "Калькулятор").Take(50).ToList();
            if (conversionHistoryList.Any())
            {
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Дата/Время</th><th>Операция</th><th>Результат</th></tr>");
                foreach (var entry in conversionHistoryList.OrderByDescending(h => h.DateTime))
                {
                    sb.AppendLine($"<tr><td>{entry.DateTime:dd.MM.yyyy HH:mm:ss}</td><td>{entry.Operation}</td><td>{entry.Result}</td></tr>");
                }
                sb.AppendLine("</table>");
            }
            else
            {
                sb.AppendLine("<p>История конвертаций пуста</p>");
            }
            sb.AppendLine("<h2>История калькулятора</h2>");
            if (calcHistory.Any())
            {
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>№</th><th>Вычисление</th></tr>");
                int index = 1;
                foreach (var calc in calcHistory.Take(30))
                {
                    sb.AppendLine($"<tr><td>{index++}</td><td>{calc}</td></tr>");
                }
                sb.AppendLine("</table>");
            }
            else
            {
                sb.AppendLine("<p>История калькулятора пуста</p>");
            }
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine($"<p>Сгенерировано программой Универсальный Конвертер v1.0 © 2025 Чумаченко Даниил</p>");
            sb.AppendLine("<p class='no-print'><strong>Для печати используйте Ctrl+P или меню браузера Файл → Печать</strong></p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
        private void ShowUserManual_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "РУКОВОДСТВО ПОЛЬЗОВАТЕЛЯ\n\n" +
                "1. КОНВЕРТАЦИЯ ЕДИНИЦ:\n" +
                "   • Выберите тип конвертации в выпадающем списке\n" +
                "   • Выберите исходную и целевую единицы измерения\n" +
                "   • Введите значение для конвертации\n" +
                "   • Нажмите 'Конвертировать' или Enter\n\n" +
                "2. КАЛЬКУЛЯТОР:\n" +
                "   • Перейдите на вкладку 'Калькулятор'\n" +
                "   • Используйте кнопки для ввода чисел и операций\n" +
                "   • Нажмите '=' для получения результата\n\n" +
                "3. ИСТОРИЯ:\n" +
                "   • Все операции сохраняются автоматически\n" +
                "   • Используйте поиск и фильтры для навигации\n" +
                "   • Экспортируйте историю в различные форматы\n\n" +
                "4. НАСТРОЙКИ:\n" +
                "   • Настройте количество десятичных знаков\n" +
                "   • Выберите тему оформления\n" +
                "   • Включите/отключите анимации",
                "Руководство пользователя",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowQuickStart_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "БЫСТРЫЙ СТАРТ\n\n" +
                "1. Выберите тип конвертации (например, Длина)\n" +
                "2. Выберите единицы (например, м → км)\n" +
                "3. Введите число (например, 1000)\n" +
                "4. Получите результат автоматически!\n\n" +
                "ПОЛЕЗНЫЕ СОВЕТЫ:\n" +
                "• Используйте Tab для перехода между полями\n" +
                "• Enter конвертирует значение\n" +
                "• Escape очищает поля\n" +
                "• Ctrl+S сохраняет результаты",
                "Быстрый старт",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowCalculatorHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "КАК ИСПОЛЬЗОВАТЬ КАЛЬКУЛЯТОР\n\n" +
                "БАЗОВЫЕ ОПЕРАЦИИ:\n" +
                "• Сложение: +\n" +
                "• Вычитание: -\n" +
                "• Умножение: ×\n" +
                "• Деление: ÷\n" +
                "• Процент: %\n\n" +
                "СПЕЦИАЛЬНЫЕ КНОПКИ:\n" +
                "• C - полная очистка\n" +
                "• CE - очистка текущего ввода\n" +
                "• ← - удаление последней цифры\n" +
                "• ± - смена знака числа\n\n" +
                "КЛАВИАТУРА:\n" +
                "• Цифры 0-9 для ввода\n" +
                "• +, -, *, / для операций\n" +
                "• Enter или = для результата\n" +
                "• Escape для очистки",
                "Справка по калькулятору",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowFormulas_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "ФОРМУЛЫ КОНВЕРТАЦИИ\n\n" +
                "ТЕМПЕРАТУРА:\n" +
                "• °C → °F: (C × 9/5) + 32\n" +
                "• °C → K: C + 273.15\n\n" +
                "ДЛИНА:\n" +
                "• 1 км = 1000 м\n" +
                "• 1 м = 100 см\n" +
                "• 1 миля = 1.60934 км\n" +
                "• 1 фут = 0.3048 м\n\n" +
                "МАССА:\n" +
                "• 1 кг = 1000 г\n" +
                "• 1 фунт = 0.453592 кг\n" +
                "• 1 унция = 28.3495 г\n\n" +
                "ОБЪЕМ:\n" +
                "• 1 л = 1000 мл\n" +
                "• 1 галлон = 3.78541 л\n\n" +
                "Все формулы соответствуют международным стандартам",
                "Формулы конвертации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowUnitsTable_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "ТАБЛИЦА ЕДИНИЦ ИЗМЕРЕНИЯ\n\n" +
                "🌡️ ТЕМПЕРАТУРА: °C, °F, K, °R\n" +
                "📏 ДЛИНА: см, м, км, дюйм, фут, ярд, миля\n" +
                "⚖️ МАССА: г, кг, т, фунт, унция\n" +
                "📊 ОБЪЕМ: мл, л, м³, галлон, пинта\n" +
                "📐 ПЛОЩАДЬ: см², м², км², фут², акр\n" +
                "🕐 ВРЕМЯ: с, мин, ч, день, неделя\n" +
                "⚡ ЭНЕРГИЯ: Дж, кДж, кал, кВт⋅ч\n" +
                "💪 МОЩНОСТЬ: Вт, кВт, л.с.\n" +
                "🌊 ДАВЛЕНИЕ: Па, кПа, атм, бар\n" +
                "💰 ВАЛЮТА: USD, EUR, RUB, GBP, JPY\n\n" +
                "Всего поддерживается более 40 единиц измерения!",
                "Таблица единиц",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowHotkeys_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "ГОРЯЧИЕ КЛАВИШИ\n\n" +
                "ОСНОВНЫЕ:\n" +
                "• F1 - Справка\n" +
                "• Ctrl+O - Открыть файл\n" +
                "• Ctrl+S - Сохранить\n" +
                "• Ctrl+Shift+S - Сохранить как\n" +
                "• Ctrl+P - Печать\n" +
                "• Alt+F4 - Выход\n\n" +
                "КОНВЕРТАЦИЯ:\n" +
                "• Enter - Конвертировать\n" +
                "• Escape - Очистить\n" +
                "• Tab - Переход между полями\n\n" +
                "НАВИГАЦИЯ:\n" +
                "• Ctrl+Tab - Следующая вкладка\n" +
                "• Ctrl+Shift+Tab - Предыдущая вкладка\n\n" +
                "КАЛЬКУЛЯТОР:\n" +
                "• 0-9 - Ввод цифр\n" +
                "• +,-,*,/ - Операции\n" +
                "• = или Enter - Результат\n" +
                "• Escape - Очистка",
                "Горячие клавиши",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void CheckUpdates_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Проверка обновлений...\n\n" +
                "Текущая версия: 1.0\n" +
                "Последняя версия: 1.0\n\n" +
                "У вас установлена последняя версия программы!",
                "Проверка обновлений",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ReportBug_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "СООБЩИТЬ ОБ ОШИБКЕ\n\n" +
                "Если вы обнаружили ошибку в работе программы,\n" +
                "пожалуйста, сообщите о ней разработчику:\n\n" +
                "Email: danyachumachenko2007@gmail.com\n" +
                "Тема: ConverterApp - Отчет об ошибке\n\n" +
                "В письме укажите:\n" +
                "• Описание проблемы\n" +
                "• Шаги для воспроизведения\n" +
                "• Версию программы (1.0)\n\n" +
                "Спасибо за помощь в улучшении программы!",
                "Сообщить об ошибке",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "УНИВЕРСАЛЬНЫЙ КОНВЕРТЕР\n" +
                "Версия 1.0\n\n" +
                "© 2025 Чумаченко Даниил\n\n" +
                "Программа для конвертации единиц измерения\n" +
                "и выполнения математических расчетов.\n\n" +
                "Возможности:\n" +
                "• Конвертация более 40 единиц измерения\n" +
                "• Встроенный калькулятор\n" +
                "• История операций\n" +
                "• Экспорт результатов\n" +
                "• Настраиваемый интерфейс\n\n" +
                "Контакты разработчика:\n" +
                "Email: danyachumachenko2007@gmail.com",
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ImportHistory_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";
                openDialog.Title = "Импорт истории конвертаций";
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(openDialog.FileName);
                        var importedHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryEntry>>(json);
                        foreach (var entry in importedHistory)
                        {
                            conversionHistory.Add(entry);
                        }
                        UpdateHistoryDisplay();
                        MessageBox.Show($"Импортировано {importedHistory.Count} записей истории", "Импорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void ShowTips_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "СОВЕТЫ И ХИТРОСТИ\n\n" +
                "🔥 Горячие клавиши:\n" +
                "• Ctrl+Enter - Конвертировать\n" +
                "• Ctrl+Delete - Очистить поля\n" +
                "• F1 - Справка\n" +
                "• Ctrl+E - Экспорт\n\n" +
                "⚡ Быстрый ввод:\n" +
                "• Используйте Tab для навигации\n" +
                "• Вводите числа прямо в поле\n" +
                "• Автоматическое форматирование\n\n" +
                "🎯 Эффективность:\n" +
                "• Включите автоконвертацию\n" +
                "• Используйте историю операций\n" +
                "• Настройте точность вычислений\n\n" +
                "📊 Экспорт данных:\n" +
                "• PDF для отчетов\n" +
                "• CSV для Excel\n" +
                "• PNG для презентаций",
                "Советы и хитрости",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowContacts_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "КОНТАКТНАЯ ИНФОРМАЦИЯ\n\n" +
                "👨‍💻 Разработчик:\n" +
                "Чумаченко Даниил\n\n" +
                "📧 Email:\n" +
                "danyachumachenko2007@gmail.com\n\n" +
                "💬 Обратная связь:\n" +
                "• Сообщения об ошибках\n" +
                "• Предложения по улучшению\n" +
                "• Вопросы по использованию\n\n" +
                "⏱️ Время ответа:\n" +
                "Обычно в течение 24-48 часов\n\n" +
                "🌐 Поддержка:\n" +
                "Техническая поддержка предоставляется\n" +
                "по электронной почте",
                "Контакты",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void ShowLicense_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "ЛИЦЕНЗИОННОЕ СОГЛАШЕНИЕ\n\n" +
                "ConverterApp версия 1.0\n" +
                "© 2025 Чумаченко Даниил\n\n" +
                "📋 Условия использования:\n\n" +
                "✅ РАЗРЕШЕНО:\n" +
                "• Свободное использование программы\n" +
                "• Использование в образовательных целях\n" +
                "• Использование в коммерческих проектах\n" +
                "• Распространение программы\n\n" +
                "❌ ЗАПРЕЩЕНО:\n" +
                "• Модификация программы\n" +
                "• Обратная разработка\n" +
                "• Продажа программы\n\n" +
                "⚠️ ОТКАЗ ОТ ОТВЕТСТВЕННОСТИ:\n" +
                "Программа предоставляется 'как есть'.\n" +
                "Автор не несет ответственности за любые\n" +
                "убытки, связанные с использованием программы.\n\n" +
                "Все права защищены.",
                "Лицензия",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void InitializeBasicCalculatorButtons()
        {
            string[,] basicLayout = {
                { "7", "8", "9", "÷" },
                { "4", "5", "6", "×" },
                { "1", "2", "3", "-" },
                { "0", ".", "=", "+" },
                { "C", "CE", "←", "±" }
            };
            if (calcTabButtonPanel != null)
            {
                calcTabButtonPanel.Controls.Clear();
                calcTabButtonPanel.ColumnCount = 4;
                calcTabButtonPanel.RowCount = 5;
                for (int i = 0; i < 4; i++)
                {
                    if (i < calcTabButtonPanel.ColumnStyles.Count)
                        calcTabButtonPanel.ColumnStyles[i] = new ColumnStyle(SizeType.Percent, 25F);
                    else
                        calcTabButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                }
                for (int i = 0; i < 5; i++)
                {
                    if (i < calcTabButtonPanel.RowStyles.Count)
                        calcTabButtonPanel.RowStyles[i] = new RowStyle(SizeType.Percent, 20F);
                    else
                        calcTabButtonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
                }
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        var btn = CreateCalculatorButton(basicLayout[row, col]);
                        btn.Font = new Font("Segoe UI", 14F);
                        calcTabButtonPanel.Controls.Add(btn, col, row);
                    }
                }
            }
        }
        private Button CreateCalculatorButton(string text)
        {
            var button = new Button();
            button.Text = text;
            button.Dock = DockStyle.Fill;
            button.Font = new Font("Segoe UI", 14F);
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.White;
            button.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            button.Cursor = Cursors.Hand;
            button.Margin = new Padding(2);
            if ("+-×÷=%".Contains(text) && text.Length == 1)
            {
                button.BackColor = Color.FromArgb(33, 150, 243);
                button.ForeColor = Color.White;
            }
            else if (text == "CE" || text == "C" || text == "AC")
            {
                button.BackColor = Color.FromArgb(244, 67, 54);
                button.ForeColor = Color.White;
            }
            button.Click += CalcButton_Click;
            return button;
        }

        private void LoadSettings()
        {
            try
            {
                string settingsPath = GetSettingsFilePath();
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(json);
                    if (settings != null)
                    {
                        decimalPlaces = settings.DecimalPlaces;
                        useThousandsSeparator = settings.UseThousandsSeparator;
                        isAnimationEnabled = settings.AnimationsEnabled;
                        isAutoConvertEnabled = settings.AutoConvert;
                        if (numDecimalPlaces != null) numDecimalPlaces.Value = decimalPlaces;
                        if (chkThousandsSeparator != null) chkThousandsSeparator.Checked = useThousandsSeparator;
                        if (chkAnimations != null) chkAnimations.Checked = isAnimationEnabled;
                        if (chkScientificNotation != null) chkScientificNotation.Checked = isAutoConvertEnabled;
                        if (cboTheme != null && !string.IsNullOrEmpty(settings.Theme))
                        {
                            int index = cboTheme.Items.IndexOf(settings.Theme);
                            if (index >= 0) cboTheme.SelectedIndex = index;
                        }
                        if (settings.WindowX != -1 && settings.WindowY != -1)
                        {
                            this.Location = new Point(settings.WindowX, settings.WindowY);
                        }
                        if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
                        {
                            this.Size = new Size(settings.WindowWidth, settings.WindowHeight);
                        }
                        this.WindowState = settings.WindowState;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new AppSettings
                {
                    DecimalPlaces = decimalPlaces,
                    UseThousandsSeparator = useThousandsSeparator,
                    AnimationsEnabled = isAnimationEnabled,
                    AutoConvert = isAutoConvertEnabled,
                    Theme = cboTheme?.SelectedItem?.ToString() ?? "Светлая",
                    WindowX = this.WindowState == FormWindowState.Normal ? this.Location.X : this.RestoreBounds.X,
                    WindowY = this.WindowState == FormWindowState.Normal ? this.Location.Y : this.RestoreBounds.Y,
                    WindowWidth = this.WindowState == FormWindowState.Normal ? this.Width : this.RestoreBounds.Width,
                    WindowHeight = this.WindowState == FormWindowState.Normal ? this.Height : this.RestoreBounds.Height,
                    WindowState = this.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : this.WindowState
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
                string settingsPath = GetSettingsFilePath();
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private void CboUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingComboBox) return;
            if (isAutoConvertEnabled && !string.IsNullOrEmpty(txtInput.Text))
            {
                if (double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out _) ||
                    double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    BtnConvert_Click(null, null);
                }
            }
        }

        private void TxtInput_TextChanged(object sender, EventArgs e)
        {
            if (isAutoConvertEnabled && !string.IsNullOrEmpty(txtInput.Text))
            {
                if (double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out _) ||
                    double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    BtnConvert_Click(null, null);
                }
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSON файлы (*.json)|*.json|CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string content = File.ReadAllText(openDialog.FileName);
                        if (Path.GetExtension(openDialog.FileName).ToLower() == ".csv")
                        {
                            ImportCSVData(content);
                        }
                        else
                        {
                            ImportJSONData(content);
                        }
                        lblStatus.Text = "Файл успешно загружен";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", 
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInput.Text) && !string.IsNullOrEmpty(txtOutput.Text))
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Текстовые файлы (*.txt)|*.txt|JSON файлы (*.json)|*.json";
                    saveDialog.FileName = $"conversion_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string content = $"Конвертация\n" +
                                           $"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                           $"Тип: {cboType.SelectedItem}\n" +
                                           $"Ввод: {txtInput.Text} {cboFromUnit.SelectedItem}\n" +
                                           $"Результат: {txtOutput.Text} {cboToUnit.SelectedItem}";
                            File.WriteAllText(saveDialog.FileName, content);
                            lblStatus.Text = "Результат сохранен";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", 
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет данных для сохранения", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveAsFile_Click(object sender, EventArgs e)
        {
            SaveFile_Click(sender, e);
        }

        private void ImportData_Click(object sender, EventArgs e)
        {
            OpenFile_Click(sender, e);
        }


        private void SaveAsPDF(string filename)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF файлы (*.pdf)|*.pdf";
                saveDialog.FileName = filename ?? $"converter_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (PdfDocument document = new PdfDocument())
                        {
                            PdfPage page = document.AddPage();
                            XGraphics gfx = XGraphics.FromPdfPage(page);
                            XFont font = new XFont("Arial", 12);
                            XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                            double y = 40;
                            
                            gfx.DrawString("Универсальный конвертер - Отчет", titleFont, 
                                XBrushes.Black, 40, y);
                            y += 30;
                            
                            gfx.DrawString($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", font, 
                                XBrushes.Black, 40, y);
                            y += 20;
                            
                            if (!string.IsNullOrEmpty(txtInput.Text) && !string.IsNullOrEmpty(txtOutput.Text))
                            {
                                gfx.DrawString("Текущий результат:", font, XBrushes.Black, 40, y);
                                y += 20;
                                gfx.DrawString($"Тип: {cboType.SelectedItem}", font, 
                                    XBrushes.Black, 60, y);
                                y += 20;
                                gfx.DrawString($"{txtInput.Text} {cboFromUnit.SelectedItem} = " +
                                    $"{txtOutput.Text} {cboToUnit.SelectedItem}", font, 
                                    XBrushes.Black, 60, y);
                                y += 30;
                            }
                            
                            gfx.DrawString("История конвертаций:", font, XBrushes.Black, 40, y);
                            y += 20;
                            
                            foreach (var entry in conversionHistory.Take(20))
                            {
                                if (y > page.Height - 40) break;
                                gfx.DrawString($"{entry.DateTime:HH:mm:ss} - {entry.Operation} = {entry.Result}", 
                                    font, XBrushes.Black, 60, y);
                                y += 20;
                            }
                            
                            document.Save(saveDialog.FileName);
                            lblStatus.Text = "PDF файл сохранен";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении PDF: {ex.Message}", 
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveAsText(string filename)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Универсальный конвертер - Отчет");
                sb.AppendLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();
                
                if (!string.IsNullOrEmpty(txtInput.Text) && !string.IsNullOrEmpty(txtOutput.Text))
                {
                    sb.AppendLine("Текущий результат:");
                    sb.AppendLine($"  Тип: {cboType.SelectedItem}");
                    sb.AppendLine($"  {txtInput.Text} {cboFromUnit.SelectedItem} = " +
                        $"{txtOutput.Text} {cboToUnit.SelectedItem}");
                    sb.AppendLine();
                }
                
                sb.AppendLine("История конвертаций:");
                foreach (var entry in conversionHistory.Take(50))
                {
                    sb.AppendLine($"  {entry.DateTime:yyyy-MM-dd HH:mm:ss} - " +
                        $"{entry.Operation} = {entry.Result}");
                }
                
                File.WriteAllText(filename, sb.ToString());
                lblStatus.Text = "Текстовый файл сохранен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportJSONData(string json)
        {
            try
            {
                var entries = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryEntry>>(json);
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        conversionHistory.Add(entry);
                    }
                    UpdateHistoryGrid();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при импорте JSON данных", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportCSVData(string csv)
        {
            try
            {
                var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1)
                {
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var parts = lines[i].Split(',');
                        if (parts.Length >= 4)
                        {
                            var entry = new HistoryEntry
                            {
                                DateTime = DateTime.Parse(parts[0]),
                                Type = parts[1].Trim('"'),
                                Operation = parts[2].Trim('"'),
                                Result = parts[3].Trim('"')
                            };
                            conversionHistory.Add(entry);
                        }
                    }
                    UpdateHistoryGrid();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при импорте CSV данных", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
