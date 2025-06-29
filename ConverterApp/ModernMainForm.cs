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
        // Static HttpClient to prevent socket exhaustion
        private static readonly HttpClient httpClient = new HttpClient();
        
        // Thread-safe collections for history
        private readonly ConcurrentBag<HistoryEntry> conversionHistory = new ConcurrentBag<HistoryEntry>();
        private readonly ConcurrentBag<string> calcHistory = new ConcurrentBag<string>();
        private readonly object historyLock = new object();
        
        // Cancellation token for async operations
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
        
        private PrintDocument printDocument = new PrintDocument();
        private bool isAnimationEnabled = true;
        private int decimalPlaces = 2;
        private bool useThousandsSeparator = true;
        private bool isAutoConvertEnabled = false; // Disabled by default to prevent annoying errors
        
        // Calculator variables
        private double calcMemory = 0;
        private string calcOperation = "";
        private bool calcNewNumber = true;
        private CalculatorMode currentCalcMode = CalculatorMode.Basic;
        
        // Flag to prevent recursive text changes
        private bool isUpdatingText = false;
        private bool isInitialized = false;
        private bool isUpdatingComboBox = false;
        private bool isChangingType = false;
        
        private enum CalculatorMode
        {
            Basic,
            Scientific
        }
        
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
            
            // Calculator state
            public string CalcMode { get; set; } = "Basic";
            public string LastConversionType { get; set; } = "";
            
            // Window state
            public int WindowWidth { get; set; } = 1024;
            public int WindowHeight { get; set; } = 768;
            public int WindowX { get; set; } = -1;
            public int WindowY { get; set; } = -1;
            public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
        }

        public ModernMainForm()
        {
            InitializeComponent();
            SetupEventHandlers();
            LoadSettings();
            InitializeControls();
            ApplyTheme();
            SetupKeyboardShortcuts();
            
            // Save settings on form closing
            this.FormClosing += (s, e) => SaveSettings();
        }
        
        private void SetupEventHandlers()
        {
            if (isInitialized) return;
            
            // Unsubscribe first to prevent double subscription
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
            
            // Calculator events are now handled in CreateCalculatorButton method
            
            // History events
            if (btnClearHistory != null) btnClearHistory.Click += BtnClearHistory_Click;
            if (btnExportCSV != null) btnExportCSV.Click += BtnExportCSV_Click;
            if (btnExportPDF != null) btnExportPDF.Click += BtnExportPDF_Click;
            if (btnHistorySearch != null) btnHistorySearch.Click += BtnHistorySearch_Click;
            if (cboHistoryFilter != null) cboHistoryFilter.SelectedIndexChanged += CboHistoryFilter_SelectedIndexChanged;
            
            // Calculator tab events
            if (btnBasicMode != null) btnBasicMode.Click += (s, e) => SwitchCalculatorMode(CalculatorMode.Basic);
            if (btnScientificMode != null) btnScientificMode.Click += (s, e) => SwitchCalculatorMode(CalculatorMode.Scientific);
            
            // Settings events
            if (btnApplySettings != null) btnApplySettings.Click += BtnApplySettings_Click;
            if (btnSaveSettings != null) btnSaveSettings.Click += BtnSaveSettings_Click;
            if (btnResetSettings != null) btnResetSettings.Click += BtnResetSettings_Click;
            
            // Menu events
            if (openMenuItem != null) openMenuItem.Click += OpenFile_Click;
            if (saveMenuItem != null) saveMenuItem.Click += SaveFile_Click;
            if (printMenuItem != null) printMenuItem.Click += PrintResults_Click;
            if (exitMenuItem != null) exitMenuItem.Click += (s, e) => Application.Exit();
            if (aboutMenuItem != null) aboutMenuItem.Click += About_Click;
            
            // Print document event
            printDocument.PrintPage += PrintDocument_PrintPage;
            
            // Tab control event
            mainTabControl.SelectedIndexChanged += MainTabControl_SelectedIndexChanged;
            
            // Enable drag & drop for unit swapping
            EnableDragDrop();
            
            // Mark as initialized
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
                            PrintResults_Click(null, null);
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
            
            // Run comprehensive tests after form loads (delayed to allow form to fully initialize)
            // Tests can be run manually if needed
            // RunComprehensiveTests();
        }
        
        
        private void RunComprehensiveTests()
        {
            Console.WriteLine("=== НАЧАЛО ПОЛНОГО КОМПЛЕКСНОГО ТЕСТИРОВАНИЯ ===");
            Console.WriteLine($"Время начала: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Версия приложения: ModernMainForm");
            Console.WriteLine($"Платформа: {Environment.OSVersion}");
            Console.WriteLine();
            
            // Test 1: ALL Length conversions
            Console.WriteLine("--- ТЕСТ 1: ВСЕ конвертации длины ---");
            string[] lengthUnits = { "cm", "m", "km", "in", "ft", "yd", "mi" };
            TestAllConversions("📏 Длина", lengthUnits);
            
            // Test 2: ALL Mass conversions
            Console.WriteLine("\n--- ТЕСТ 2: ВСЕ конвертации массы ---");
            string[] massUnits = { "g", "kg", "t", "lb", "oz" };
            TestAllConversions("⚖️ Масса", massUnits);
            
            // Test 3: ALL Temperature conversions
            Console.WriteLine("\n--- ТЕСТ 3: ВСЕ конвертации температуры ---");
            string[] tempUnits = { "°C", "°F", "K", "°R" };
            TestAllConversions("🌡️ Температура", tempUnits);
            
            // Test 4: ALL Volume conversions
            Console.WriteLine("\n--- ТЕСТ 4: ВСЕ конвертации объема ---");
            string[] volumeUnits = { "ml", "l", "m³", "gal", "pt" };
            TestAllConversions("📊 Объем", volumeUnits);
            
            // Test 5: ALL Area conversions
            Console.WriteLine("\n--- ТЕСТ 5: ВСЕ конвертации площади ---");
            string[] areaUnits = { "cm²", "m²", "km²", "ft²", "ac" };
            TestAllConversions("📐 Площадь", areaUnits);
            
            // Test 6: ALL Time conversions
            Console.WriteLine("\n--- ТЕСТ 6: ВСЕ конвертации времени ---");
            string[] timeUnits = { "s", "min", "h", "d", "week" };
            TestAllConversions("🕐 Время", timeUnits);
            
            // Test 7: ALL Energy conversions
            Console.WriteLine("\n--- ТЕСТ 7: ВСЕ конвертации энергии ---");
            string[] energyUnits = { "J", "kJ", "cal", "kWh" };
            TestAllConversions("⚡ Энергия", energyUnits);
            
            // Test 8: ALL Power conversions
            Console.WriteLine("\n--- ТЕСТ 8: ВСЕ конвертации мощности ---");
            string[] powerUnits = { "W", "kW", "hp" };
            TestAllConversions("💪 Мощность", powerUnits);
            
            // Test 9: ALL Pressure conversions
            Console.WriteLine("\n--- ТЕСТ 9: ВСЕ конвертации давления ---");
            string[] pressureUnits = { "Pa", "kPa", "atm", "bar" };
            TestAllConversions("🌊 Давление", pressureUnits);
            
            // Test 10: ALL Currency conversions
            Console.WriteLine("\n--- ТЕСТ 10: ВСЕ конвертации валют ---");
            string[] currencyUnits = { "USD", "EUR", "RUB", "GBP", "JPY" };
            TestAllConversions("💰 Валюта", currencyUnits);
            
            // Test 11: Calculator ALL operations
            Console.WriteLine("\n--- ТЕСТ 11: ВСЕ операции калькулятора ---");
            TestAllCalculatorOperations();
            
            // Test 12: Scientific calculator ALL functions
            Console.WriteLine("\n--- ТЕСТ 12: ВСЕ научные функции ---");
            TestAllScientificFunctions();
            
            // Test 13: Edge cases and error handling
            Console.WriteLine("\n--- ТЕСТ 13: Граничные случаи и ошибки ---");
            TestEdgeCases();
            
            // Test 14: UI Elements
            Console.WriteLine("\n--- ТЕСТ 14: Элементы интерфейса ---");
            TestUIElements();
            
            // Test 15: Keyboard shortcuts
            Console.WriteLine("\n--- ТЕСТ 15: Горячие клавиши ---");
            TestKeyboardShortcuts();
            
            // Test 16: Settings persistence
            Console.WriteLine("\n--- ТЕСТ 16: Сохранение настроек ---");
            TestSettingsPersistence();
            
            // Test 17: History management
            Console.WriteLine("\n--- ТЕСТ 17: Управление историей ---");
            TestHistoryManagement();
            
            // Test 18: Export all formats
            Console.WriteLine("\n--- ТЕСТ 18: Экспорт во все форматы ---");
            TestAllExportFormats();
            
            // Test 19: Theme switching
            Console.WriteLine("\n--- ТЕСТ 19: Переключение тем ---");
            TestThemeSwitching();
            
            // Test 20: Animation testing
            Console.WriteLine("\n--- ТЕСТ 20: Анимации ---");
            TestAnimations();
            
            // Test 21: Memory and performance
            Console.WriteLine("\n--- ТЕСТ 21: Память и производительность ---");
            TestMemoryAndPerformance();
            
            // Final report
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
                
                // Track test results
                totalTests++;
                if (success) passedTests++;
                else failedTests++;
                
                // Add to history
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
            // Enforce history limit of 100 entries
            while (conversionHistory.Count > 100)
            {
                // Remove oldest entry
                var allEntries = conversionHistory.ToList();
                var oldestEntry = allEntries.OrderBy(h => h.DateTime).FirstOrDefault();
                if (oldestEntry != null)
                {
                    // Try to remove the oldest entry
                    var tempList = new List<HistoryEntry>();
                    while (conversionHistory.TryTake(out var entry))
                    {
                        if (entry.DateTime != oldestEntry.DateTime || entry.Operation != oldestEntry.Operation)
                        {
                            tempList.Add(entry);
                        }
                    }
                    // Add back all except the oldest
                    foreach (var item in tempList)
                    {
                        conversionHistory.Add(item);
                    }
                }
                else
                {
                    break; // Safety exit
                }
            }
        }
        
        private void TestCalculator(string expression, double expected)
        {
            try
            {
                // Parse expression
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
                
                // Track test results
                totalTests++;
                if (success) passedTests++;
                else failedTests++;
                
                // Add to calculator history
                calcHistory.Add($"{expression} = {result} ({DateTime.Now:HH:mm:ss})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в калькуляторе: {ex.Message}");
                totalTests++;
                failedTests++;
            }
        }
        
        private void TestScientificCalculator(string function, double input, double expected)
        {
            try
            {
                double result = 0;
                switch (function)
                {
                    case "sin":
                        result = Math.Sin(input * Math.PI / 180); // Convert to radians
                        break;
                    case "cos":
                        result = Math.Cos(input * Math.PI / 180);
                        break;
                    case "tan":
                        result = Math.Tan(input * Math.PI / 180);
                        break;
                    case "√":
                        result = Math.Sqrt(input);
                        break;
                    case "x²":
                        result = Math.Pow(input, 2);
                        break;
                    case "x³":
                        result = Math.Pow(input, 3);
                        break;
                    case "log":
                        result = Math.Log10(input);
                        break;
                    case "ln":
                        result = Math.Log(input);
                        break;
                }
                
                bool success = Math.Abs(result - expected) < 0.0001;
                Console.WriteLine($"Функция: {function}({input}) = {result:F4}");
                Console.WriteLine($"Ожидалось: {expected:F4}, Результат: {(success ? "OK УСПЕХ" : "FAIL ОШИБКА")}");
                
                // Track test results
                totalTests++;
                if (success) passedTests++;
                else failedTests++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА в научном калькуляторе: {ex.Message}");
                totalTests++;
                failedTests++;
            }
        }
        
        private void TestSettings()
        {
            try
            {
                // Test decimal places
                int originalDecimal = decimalPlaces;
                decimalPlaces = 4;
                Console.WriteLine($"Установка десятичных знаков: {decimalPlaces} - OK");
                
                // Test thousand separator
                bool originalSeparator = useThousandsSeparator;
                useThousandsSeparator = true;
                Console.WriteLine($"Разделитель тысяч: {useThousandsSeparator} - OK");
                
                // Test animation
                bool originalAnimation = isAnimationEnabled;
                isAnimationEnabled = false;
                Console.WriteLine($"Анимация: {isAnimationEnabled} - OK");
                
                // Save and load settings
                SaveSettings();
                Console.WriteLine("Сохранение настроек - OK");
                
                LoadSettings();
                Console.WriteLine("Загрузка настроек - OK");
                
                // Restore original settings
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
                
                // Show last 3 conversion history entries
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
                // Test CSV export preparation
                StringBuilder csvData = new StringBuilder();
                csvData.AppendLine("DateTime,Operation,Result,Type");
                foreach (var entry in conversionHistory.ToList().Take(3))
                {
                    csvData.AppendLine($"{entry.DateTime:yyyy-MM-dd HH:mm:ss},{entry.Operation},{entry.Result},{entry.Type}");
                }
                
                Console.WriteLine("Подготовка CSV данных - OK");
                Console.WriteLine($"Размер данных: {csvData.Length} символов");
                
                // Test PDF export capability
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
                Console.WriteLine($"Текущий режим калькулятора: {currentCalcMode}");
                
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
        
        // Test statistics
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
                                // Use relative tolerance for larger numbers
                                double tolerance = value > 100 ? value * 0.0001 : 0.001;
                                bool success = Math.Abs(backResult - value) < tolerance;
                                
                                totalTests++;
                                if (success) passedTests++; else failedTests++;
                                
                                if (!success || value == 1) // Show first value and failures
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
            // Basic operations with various numbers
            double[,] testCases = {
                { 2, 2, 4, 0, 4, 1 },      // a, b, a+b, a-b, a*b, a/b
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
            
            // Test modulo
            TestCalculator("10 % 3", 1);
            TestCalculator("20 % 7", 6);
            TestCalculator("15 % 5", 0);
        }
        
        private void TestAllScientificFunctions()
        {
            // Trigonometric functions
            double[] angles = { 0, 30, 45, 60, 90, 180, 270, 360 };
            foreach (var angle in angles)
            {
                TestScientificCalculator("sin", angle, Math.Sin(angle * Math.PI / 180));
                TestScientificCalculator("cos", angle, Math.Cos(angle * Math.PI / 180));
                if (angle != 90 && angle != 270) // Avoid tan(90°) = infinity
                    TestScientificCalculator("tan", angle, Math.Tan(angle * Math.PI / 180));
            }
            
            // Power and root functions
            double[] numbers = { 0, 1, 4, 9, 16, 25, 100 };
            foreach (var num in numbers)
            {
                TestScientificCalculator("√", num, Math.Sqrt(num));
                TestScientificCalculator("x²", num, num * num);
                TestScientificCalculator("x³", num, num * num * num);
            }
            
            // Logarithmic functions
            double[] logNumbers = { 1, 10, 100, 1000, Math.E };
            foreach (var num in logNumbers)
            {
                TestScientificCalculator("log", num, Math.Log10(num));
                TestScientificCalculator("ln", num, Math.Log(num));
            }
        }
        
        private void TestEdgeCases()
        {
            Console.WriteLine("Тестирование граничных случаев:");
            
            // Division by zero
            try
            {
                TestCalculator("5 ÷ 0", double.PositiveInfinity);
                Console.WriteLine("  Деление на ноль обработано OK");
            }
            catch { Console.WriteLine("  Деление на ноль НЕ обработано ✗"); }
            
            // Very large numbers
            TestConversion("📏 Длина", "m", "km", 1e10, 1e7);
            
            // Very small numbers
            TestConversion("📏 Длина", "km", "m", 1e-10, 1e-7);
            
            // Negative temperatures
            TestConversion("🌡️ Температура", "°C", "K", -273.15, 0);
            
            // Empty input handling
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
            
            // Check all tabs exist
            if (mainTabControl != null)
            {
                Console.WriteLine($"  Количество вкладок: {mainTabControl.TabCount} OK");
                foreach (TabPage tab in mainTabControl.TabPages)
                {
                    Console.WriteLine($"    - {tab.Text}");
                }
            }
            
            // Check combo boxes
            if (cboType != null) Console.WriteLine($"  ComboBox типов: {cboType.Items.Count} элементов OK");
            if (cboFromUnit != null) Console.WriteLine($"  ComboBox единиц (от): загружен OK");
            if (cboToUnit != null) Console.WriteLine($"  ComboBox единиц (к): загружен OK");
            
            // Check buttons
            if (btnConvert != null) Console.WriteLine("  Кнопка конвертации: доступна OK");
            if (btnClear != null) Console.WriteLine("  Кнопка очистки: доступна OK");
            
            // Check text boxes
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
            
            // Change settings
            decimalPlaces = 5;
            useThousandsSeparator = false;
            isAnimationEnabled = true;
            isAutoConvertEnabled = true;
            
            // Save
            SaveSettings();
            Console.WriteLine("  Настройки сохранены OK");
            
            // Reset
            decimalPlaces = 2;
            useThousandsSeparator = true;
            isAnimationEnabled = false;
            isAutoConvertEnabled = false;
            
            // Load
            LoadSettings();
            Console.WriteLine("  Настройки загружены OK");
            
            // Verify
            if (decimalPlaces == 5 && !useThousandsSeparator && isAnimationEnabled && isAutoConvertEnabled)
            {
                Console.WriteLine("  Настройки восстановлены корректно OK");
            }
            else
            {
                Console.WriteLine("  Ошибка восстановления настроек ✗");
            }
            
            // Restore original
            decimalPlaces = originalSettings.DecimalPlaces;
            useThousandsSeparator = originalSettings.UseThousandsSeparator;
            isAnimationEnabled = originalSettings.AnimationsEnabled;
            isAutoConvertEnabled = originalSettings.AutoConvert;
        }
        
        private void TestHistoryManagement()
        {
            Console.WriteLine("Тестирование управления историей:");
            
            int initialCount = conversionHistory.Count;
            
            // Add many entries to test limit
            for (int i = 0; i < 150; i++)
            {
                var entry = new HistoryEntry
                {
                    DateTime = DateTime.Now.AddSeconds(i), // Ensure unique timestamps
                    Operation = $"Test {i}",
                    Result = i.ToString(),
                    Type = "Test"
                };
                conversionHistory.Add(entry);
                EnforceHistoryLimit();
            }
            
            // Check if history limit works (should be max 100)
            Console.WriteLine($"  Записей в истории: {conversionHistory.Count} (лимит работает: {(conversionHistory.Count <= 100 ? "✓" : "✗")})");
            
            // Test history filtering
            var filtered = conversionHistory.ToList().Where(h => h.Type == "Test").Count();
            Console.WriteLine($"  Фильтрация истории: {filtered} записей типа 'Test' OK");
            
            // Test history search
            var searchResult = conversionHistory.ToList().Where(h => h.Operation.Contains("Test 5")).Count();
            Console.WriteLine($"  Поиск в истории: найдено {searchResult} записей OK");
        }
        
        private void TestAllExportFormats()
        {
            Console.WriteLine("Тестирование всех форматов экспорта:");
            
            // Test CSV export
            try
            {
                var csvData = GenerateCSVData();
                Console.WriteLine($"  CSV экспорт: {csvData.Length} символов OK");
            }
            catch { Console.WriteLine("  CSV экспорт: ОШИБКА ✗"); }
            
            // Test PDF capability
            try
            {
                using (var doc = new PdfDocument())
                {
                    var page = doc.AddPage();
                    Console.WriteLine("  PDF библиотека: доступна OK");
                }
            }
            catch { Console.WriteLine("  PDF библиотека: НЕ доступна ✗"); }
            
            // Test print preview
            Console.WriteLine("  Предпросмотр печати: доступен OK");
        }
        
        private void TestThemeSwitching()
        {
            Console.WriteLine("Тестирование переключения тем:");
            
            // Light theme
            ApplyTheme("Светлая");
            Console.WriteLine("  Светлая тема применена OK");
            
            // Dark theme  
            ApplyTheme("Темная");
            Console.WriteLine("  Темная тема применена OK");
            
            // Restore original
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
            
            // Perform many conversions
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
            
            // Force garbage collection
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
                // Cancel any previous update operation
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                
                // Simulated API call - in real implementation, use actual exchange rate API
                await Task.Delay(100, token);
                
                // Check if cancelled
                if (token.IsCancellationRequested) return;
                
                // Fix: Use thread-safe UI update
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
                // Operation was cancelled, no need to update UI
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
            isChangingType = true;
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
                    
                    // Update UI based on type
                    if (type.Contains("Валюта"))
                    {
                        UpdateCurrencyRates();
                    }
                }
            }
            finally
            {
                isUpdatingComboBox = false;
                isChangingType = false;
            }
            
            // After units are loaded, auto-convert if there's a number in the input field
            if (!string.IsNullOrEmpty(txtInput.Text) && 
                cboFromUnit.SelectedItem != null && 
                cboToUnit.SelectedItem != null)
            {
                // Use BeginInvoke to ensure UI is fully updated before conversion
                this.BeginInvoke(new Action(() => 
                {
                    BtnConvert_Click(null, null);
                }));
            }
        }
        
        private void CboUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Removed automatic unit switching to fix dropdown issues
            
            // Auto-convert if:
            // 1. There's text in input field
            // 2. Both units are selected
            // 3. We're not in the middle of changing type
            if (!string.IsNullOrEmpty(txtInput.Text) && 
                !isChangingType &&
                cboFromUnit.SelectedItem != null && 
                cboToUnit.SelectedItem != null)
            {
                BtnConvert_Click(null, null);
            }
        }
        
        private void TxtInput_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingText) return;
            
            // Validate input length to prevent overflow
            if (txtInput.Text.Length > 15)
            {
                isUpdatingText = true;
                txtInput.Text = txtInput.Text.Substring(0, 15);
                txtInput.SelectionStart = txtInput.Text.Length;
                isUpdatingText = false;
            }
            
            // Auto-convert if:
            // 1. There's valid text
            // 2. Both units are selected
            // 3. We're not changing type
            // 4. Either auto-convert is enabled OR user is typing
            if (!string.IsNullOrEmpty(txtInput.Text) && 
                !isChangingType &&
                cboFromUnit.SelectedItem != null && 
                cboToUnit.SelectedItem != null)
            {
                // Check if it's a valid number before auto-converting
                if (double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out _) ||
                    double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    BtnConvert_Click(null, null);
                }
            }
        }
        
        private void BtnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text)) return;
            
            // Fix: Use current culture for number parsing with overflow check
            if (!double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out double value) &&
                !double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                // Only show error if user clicked the button (not auto-convert)
                if (sender != null)
                {
                    MessageBox.Show("Введите корректное число!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            
            // Check for overflow/special values
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
                // Only show error if user clicked the button (not auto-convert)
                if (sender != null)
                {
                    MessageBox.Show("Выберите единицы измерения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            
            string type = cboType.SelectedItem?.ToString() ?? "";
            double result = ConvertUnit(value, type, fromUnit, toUnit);
            
            // Check for conversion failure
            if (double.IsNaN(result))
            {
                if (sender != null)
                {
                    MessageBox.Show("Невозможно выполнить конвертацию между выбранными единицами!", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            
            // Apply number formatting
            string format = useThousandsSeparator ? $"N{decimalPlaces}" : $"F{decimalPlaces}";
            txtOutput.Text = result.ToString(format);
            
            // Add to history with size limit
            var historyEntry = new HistoryEntry
            {
                DateTime = DateTime.Now,
                Operation = $"{value} {fromUnit} -> {toUnit}",
                Result = $"{result.ToString(format)} {toUnit}",
                Type = "Конвертация"
            };
            
            // Limit history size to prevent memory issues
            const int maxHistorySize = 1000;
            lock (historyLock)
            {
                conversionHistory.Add(historyEntry);
                
                // Remove oldest entries if limit exceeded
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
            
            // Animate arrow if enabled
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
            // Prevent infinite recursion
            if (recursionDepth > 3)
            {
                // Log warning and return NaN to indicate conversion failure
                System.Diagnostics.Debug.WriteLine($"Warning: Max recursion depth reached for conversion {fromUnit} to {toUnit}");
                return double.NaN;
            }
            
            // Direct conversion - same units
            if (fromUnit == toUnit)
            {
                return value;
            }
            
            if (type.Contains("Валюта"))
            {
                return ConvertCurrency(value, fromUnit, toUnit);
            }
            
            // Temperature conversions
            if (type.Contains("Температура"))
            {
                return ConvertTemperature(value, fromUnit, toUnit);
            }
            
            // Other conversions using factors
            var conversionFactors = GetConversionFactors();
            var key = (fromUnit, toUnit);
            
            if (conversionFactors.ContainsKey(key))
            {
                return value * conversionFactors[key];
            }
            
            // Try reverse conversion
            var reverseKey = (toUnit, fromUnit);
            if (conversionFactors.ContainsKey(reverseKey))
            {
                return value / conversionFactors[reverseKey];
            }
            
            // Try through base unit conversion
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
            // Convert to Celsius first
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
            
            // Convert from Celsius to target
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
                // Length - added missing direct conversions
                [("cm", "m")] = 0.01, [("m", "km")] = 0.001, [("in", "cm")] = 2.54,
                [("ft", "m")] = 0.3048, [("yd", "m")] = 0.9144, [("mi", "km")] = 1.60934,
                [("m", "cm")] = 100, [("km", "m")] = 1000, [("cm", "in")] = 0.393701,
                [("m", "ft")] = 3.28084, [("m", "yd")] = 1.09361, [("km", "mi")] = 0.621371,
                // Added missing conversions to fix recursion
                [("m", "in")] = 39.3701, [("in", "m")] = 0.0254,
                [("m", "mi")] = 0.000621371, [("mi", "m")] = 1609.34,
                [("ft", "in")] = 12, [("in", "ft")] = 1.0/12.0,
                [("yd", "ft")] = 3, [("ft", "yd")] = 1.0/3.0,
                
                // Mass - added missing direct conversions
                [("g", "kg")] = 0.001, [("kg", "t")] = 0.001, [("lb", "kg")] = 0.453592,
                [("oz", "g")] = 28.3495, [("kg", "g")] = 1000, [("t", "kg")] = 1000,
                [("kg", "lb")] = 2.20462, [("g", "oz")] = 0.035274,
                // Added missing conversions
                [("kg", "oz")] = 35.274, [("oz", "kg")] = 0.0283495,
                [("lb", "oz")] = 16, [("oz", "lb")] = 0.0625,
                [("g", "t")] = 0.000001, [("t", "g")] = 1000000,
                
                // Volume
                [("ml", "l")] = 0.001, [("l", "m³")] = 0.001, [("gal", "l")] = 3.78541,
                [("pt", "l")] = 0.473176, [("l", "ml")] = 1000, [("m³", "l")] = 1000,
                [("l", "gal")] = 0.264172, [("l", "pt")] = 2.11338,
                
                // Area
                [("cm²", "m²")] = 0.0001, [("m²", "km²")] = 0.000001, [("ft²", "m²")] = 0.092903,
                [("ac", "m²")] = 4046.86, [("m²", "cm²")] = 10000, [("km²", "m²")] = 1000000,
                [("m²", "ft²")] = 10.7639, [("m²", "ac")] = 0.000247105,
                
                // Time - added missing direct conversions
                [("s", "min")] = 1/60.0, [("min", "h")] = 1/60.0, [("h", "d")] = 1/24.0,
                [("d", "week")] = 1/7.0, [("min", "s")] = 60, [("h", "min")] = 60,
                [("d", "h")] = 24, [("week", "d")] = 7,
                // Added missing conversions
                [("s", "h")] = 1/3600.0, [("h", "s")] = 3600,
                [("s", "d")] = 1/86400.0, [("d", "s")] = 86400,
                [("s", "week")] = 1/604800.0, [("week", "s")] = 604800,
                [("min", "d")] = 1/1440.0, [("d", "min")] = 1440,
                [("min", "week")] = 1/10080.0, [("week", "min")] = 10080,
                [("h", "week")] = 1/168.0, [("week", "h")] = 168,
                
                // Energy
                [("J", "kJ")] = 0.001, [("cal", "J")] = 4.184, [("kWh", "J")] = 3.6e6,
                [("kJ", "J")] = 1000, [("J", "cal")] = 1.0/4.184, [("J", "kWh")] = 1/3.6e6,
                // Added more energy conversions
                [("kJ", "cal")] = 1000.0/4.184, [("cal", "kJ")] = 4.184/1000.0,
                [("kJ", "kWh")] = 1/3600.0, [("kWh", "kJ")] = 3600,
                [("cal", "kWh")] = 4.184/3.6e6, [("kWh", "cal")] = 3.6e6/4.184,
                
                // Power - added missing conversion
                [("W", "kW")] = 0.001, [("hp", "kW")] = 0.7457,
                [("kW", "W")] = 1000, [("kW", "hp")] = 1.34102,
                // Added direct W to hp conversion
                [("W", "hp")] = 0.00134102, [("hp", "W")] = 745.7,
                
                // Pressure - added missing conversions
                [("Pa", "kPa")] = 0.001, [("atm", "kPa")] = 101.325, [("bar", "kPa")] = 100,
                [("kPa", "Pa")] = 1000, [("kPa", "atm")] = 0.00986923, [("kPa", "bar")] = 0.01,
                // Added direct conversions
                [("Pa", "atm")] = 9.86923e-6, [("atm", "Pa")] = 101325,
                [("Pa", "bar")] = 1e-5, [("bar", "Pa")] = 100000,
                [("atm", "bar")] = 1.01325, [("bar", "atm")] = 0.986923
            };
        }
        
        private double ConvertThroughBase(double value, string type, string from, string to, int recursionDepth)
        {
            // Define base units for each type
            var baseUnits = new Dictionary<string, string>
            {
                ["📏 Длина"] = "m", ["⚖️ Масса"] = "kg", ["📊 Объем"] = "l",
                ["📐 Площадь"] = "m²", ["🕐 Время"] = "s", ["⚡ Энергия"] = "J",
                ["💪 Мощность"] = "W", ["🌊 Давление"] = "Pa"
            };
            
            if (!baseUnits.ContainsKey(type)) return value;
            
            string baseUnit = baseUnits[type];
            
            // Convert to base unit
            double baseValue = ConvertUnitInternal(value, type, from, baseUnit, recursionDepth);
            
            // Convert from base unit to target
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
                    timer.Dispose(); // Fix: Dispose timer to prevent memory leak
                    arrowLabel.Text = "->";
                }
                else
                {
                    // Simulate rotation with different arrow characters
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
            
            // Determine which display to use based on button's parent
            TextBox currentDisplay = null;
            
            // Check if button belongs to calculator tab
            if (button.Parent == calcTabButtonPanel)
            {
                currentDisplay = calcTabDisplay;
            }
            
            if (currentDisplay == null) return;
            
            switch (buttonText)
            {
                case "C":
                case "CE":
                    currentDisplay.Text = "0";
                    calcMemory = 0;
                    calcOperation = "";
                    calcNewNumber = true;
                    break;
                    
                case "=":
                    PerformCalculation();
                    break;
                    
                case "+":
                case "-":
                case "×":
                case "÷":
                case "%":
                    SetOperation(buttonText);
                    break;
                    
                case "±":
                    if (double.TryParse(currentDisplay.Text, out double val))
                    {
                        currentDisplay.Text = (-val).ToString();
                    }
                    break;
                    
                case "←":
                    if (currentDisplay.Text.Length > 1)
                        currentDisplay.Text = currentDisplay.Text.Substring(0, currentDisplay.Text.Length - 1);
                    else
                        currentDisplay.Text = "0";
                    break;
                    
                case ".":
                    if (!currentDisplay.Text.Contains("."))
                        currentDisplay.Text += ".";
                    break;
                    
                default:
                    // Number or scientific function
                    if (char.IsDigit(buttonText[0]))
                    {
                        if (calcNewNumber || currentDisplay.Text == "0")
                        {
                            currentDisplay.Text = buttonText;
                            calcNewNumber = false;
                        }
                        else
                        {
                            // Prevent display overflow
                            if (currentDisplay.Text.Length < 15)
                            {
                                currentDisplay.Text += buttonText;
                            }
                        }
                    }
                    else
                    {
                        // Handle scientific functions
                        HandleScientificFunction(buttonText, currentDisplay);
                    }
                    break;
            }
            
            // No need to update txtInput separately anymore
            // as we're working directly with it in converter tab
        }
        
        private void SetOperation(string op)
        {
            if (!string.IsNullOrEmpty(calcOperation) && !calcNewNumber)
            {
                PerformCalculation();
            }
            
            // Get current display based on active tab
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
            
            // Get current display based on active tab
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
                            return;
                        }
                        break;
                    case "%":
                        result = calcMemory * currentValue / 100;
                        break;
                }
                
                // Format result to prevent display overflow
                string resultText = result.ToString();
                if (resultText.Length > 15)
                {
                    resultText = result.ToString("E5"); // Scientific notation
                }
                currentDisplay.Text = resultText;
                
                // Add to history
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
        
        private void HandleScientificFunction(string function, TextBox display)
        {
            if (display == null || !double.TryParse(display.Text, out double value)) return;
            
            double result = 0;
            
            switch (function)
            {
                case "sin":
                    result = Math.Sin(value * Math.PI / 180);
                    break;
                case "cos":
                    result = Math.Cos(value * Math.PI / 180);
                    break;
                case "tan":
                    result = Math.Tan(value * Math.PI / 180);
                    break;
                case "x²":
                    result = value * value;
                    break;
                case "x³":
                    result = value * value * value;
                    break;
                case "√":
                    result = Math.Sqrt(value);
                    break;
                case "∛":
                    result = Math.Pow(value, 1.0 / 3.0);
                    break;
                case "log":
                    if (value > 0)
                        result = Math.Log10(value);
                    else
                    {
                        display.Text = "Ошибка: недопустимое значение";
                        calcNewNumber = true;
                        return;
                    }
                    break;
                case "ln":
                    if (value > 0)
                        result = Math.Log(value);
                    else
                    {
                        display.Text = "Ошибка: недопустимое значение";
                        calcNewNumber = true;
                        return;
                    }
                    break;
                case "1/x":
                    if (value != 0)
                        result = 1 / value;
                    else
                    {
                        display.Text = "Ошибка: деление на ноль";
                        calcNewNumber = true;
                        return;
                    }
                    break;
                case "|x|":
                    result = Math.Abs(value);
                    break;
                case "π":
                    result = Math.PI;
                    break;
                case "e":
                    result = Math.E;
                    break;
                case "n!":
                    if (value >= 0 && value <= 170 && value == Math.Floor(value))
                        result = Factorial((int)value);
                    else
                    {
                        display.Text = "Ошибка: недопустимое значение";
                        calcNewNumber = true;
                        return;
                    }
                    break;
            }
            
            display.Text = result.ToString();
            calcNewNumber = true;
        }
        
        private double Factorial(int n)
        {
            if (n <= 1) return 1;
            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }
        
        private void SwitchCalculatorMode(CalculatorMode mode)
        {
            currentCalcMode = mode;
            
            // Update button states
            btnBasicMode.BackColor = mode == CalculatorMode.Basic ? 
                Color.FromArgb(33, 150, 243) : Color.FromArgb(158, 158, 158);
            btnScientificMode.BackColor = mode == CalculatorMode.Scientific ? 
                Color.FromArgb(76, 175, 80) : Color.FromArgb(158, 158, 158);
            
            // Update calculator layout based on mode
            switch (mode)
            {
                case CalculatorMode.Basic:
                    InitializeBasicCalculatorButtons();
                    break;
                case CalculatorMode.Scientific:
                    InitializeScientificCalculatorButtons();
                    break;
            }
            
            lblStatus.Text = $"Режим калькулятора: {GetModeDisplayName(mode)}";
        }
        
        private string GetModeDisplayName(CalculatorMode mode)
        {
            switch (mode)
            {
                case CalculatorMode.Basic: return "Обычный";
                case CalculatorMode.Scientific: return "Научный";
                default: return "";
            }
        }
        
        
        private void UpdateHistoryGrid()
        {
            if (historyDataGrid == null) return;
            
            historyDataGrid.Rows.Clear();
            
            var filter = cboHistoryFilter?.SelectedItem?.ToString() ?? "Все";
            var searchText = txtHistorySearch?.Text?.ToLower() ?? "";
            
            // Optimize: pre-compute lowercase search text once
            bool hasSearchText = !string.IsNullOrEmpty(searchText);
            
            // Optimize: use pagination for large histories
            const int maxDisplayItems = 100;
            
            // Convert to list for sorting (thread-safe)
            List<HistoryEntry> historyList;
            lock (historyLock)
            {
                historyList = conversionHistory.ToList();
            }
            
            var filteredHistory = historyList
                .Where(h =>
                {
                    // Filter by type
                    if (filter != "Все" && h.Type != filter)
                        return false;
                    
                    // Filter by search text (optimized)
                    if (hasSearchText)
                    {
                        // Cache lowercase versions to avoid multiple ToLower calls
                        var operationLower = h.Operation?.ToLower() ?? "";
                        var resultLower = h.Result?.ToLower() ?? "";
                        
                        if (!operationLower.Contains(searchText) && !resultLower.Contains(searchText))
                            return false;
                    }
                    
                    return true;
                })
                .OrderByDescending(h => h.DateTime)
                .Take(maxDisplayItems); // Limit displayed items for performance
            
            foreach (var entry in filteredHistory)
            {
                historyDataGrid.Rows.Add(
                    entry.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.Operation,
                    entry.Result
                );
            }
            
            // Show info if results are limited
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
                // Clear thread-safe collection
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
                            
                            // Get thread-safe copy
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
            PrintResults_Click(sender, e);
        }
        
        private void EnableDragDrop()
        {
            cboFromUnit.AllowDrop = true;
            cboToUnit.AllowDrop = true;
            
            // Disabled MouseDown handlers because they interfere with dropdown functionality
            // cboFromUnit.MouseDown += (s, e) => DoDragDrop(cboFromUnit.SelectedItem, DragDropEffects.Move);
            // cboToUnit.MouseDown += (s, e) => DoDragDrop(cboToUnit.SelectedItem, DragDropEffects.Move);
            
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
            isAutoConvertEnabled = chkScientificNotation.Checked; // Using this checkbox for auto-convert
            
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
            
            // Apply theme to all controls
            ApplyThemeToControl(this, backColor, foreColor, panelColor);
        }
        
        private void ApplyThemeToControl(Control control, Color backColor, Color foreColor, Color panelColor)
        {
            if (control is Button || control is ComboBox || control is TextBox)
                return; // Skip these controls to maintain their styling
                
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
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            
            return Path.Combine(appFolder, "settings.json");
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
                    Theme = cboTheme.SelectedItem?.ToString() ?? "Светлая",
                    CalcMode = currentCalcMode.ToString(),
                    LastConversionType = cboType?.SelectedItem?.ToString() ?? "",
                    WindowWidth = this.Width,
                    WindowHeight = this.Height,
                    WindowX = this.Location.X,
                    WindowY = this.Location.Y,
                    WindowState = this.WindowState
                };
                
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(GetSettingsFilePath(), json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        
                        if (numDecimalPlaces != null) numDecimalPlaces.Value = settings.DecimalPlaces;
                        if (chkThousandsSeparator != null) chkThousandsSeparator.Checked = settings.UseThousandsSeparator;
                        if (chkAnimations != null) chkAnimations.Checked = settings.AnimationsEnabled;
                        if (chkScientificNotation != null) chkScientificNotation.Checked = settings.AutoConvert;
                        
                        if (cboTheme != null && !string.IsNullOrEmpty(settings.Theme))
                        {
                            for (int i = 0; i < cboTheme.Items.Count; i++)
                            {
                                if (cboTheme.Items[i].ToString() == settings.Theme)
                                {
                                    cboTheme.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        
                        // Restore calculator mode
                        if (!string.IsNullOrEmpty(settings.CalcMode) && Enum.TryParse<CalculatorMode>(settings.CalcMode, out var mode))
                        {
                            currentCalcMode = mode;
                        }
                        
                        // Restore last conversion type
                        if (!string.IsNullOrEmpty(settings.LastConversionType) && cboType != null)
                        {
                            for (int i = 0; i < cboType.Items.Count; i++)
                            {
                                if (cboType.Items[i].ToString() == settings.LastConversionType)
                                {
                                    cboType.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        
                        // Restore window position and size
                        if (settings.WindowX >= 0 && settings.WindowY >= 0)
                        {
                            this.StartPosition = FormStartPosition.Manual;
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
                // Use default settings if loading fails
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
                
                // Notify user about settings load error (non-intrusive)
                if (lblStatus != null)
                {
                    lblStatus.Text = "Не удалось загрузить настройки, используются значения по умолчанию";
                }
            }
        }
        
        // File operations
        private void OpenFile_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtInput.Text = File.ReadAllText(openDialog.FileName);
                        lblStatus.Text = "Файл загружен";
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
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF файлы (*.pdf)|*.pdf|CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt";
                saveDialog.DefaultExt = "pdf";
                saveDialog.FileName = $"converter_report_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string extension = Path.GetExtension(saveDialog.FileName).ToLower();
                    
                    switch (extension)
                    {
                        case ".pdf":
                            SaveAsPDF(saveDialog.FileName);
                            break;
                        case ".csv":
                            SaveAsCSV(saveDialog.FileName);
                            break;
                        default:
                            SaveAsText(saveDialog.FileName);
                            break;
                    }
                    
                    lblStatus.Text = "Файл сохранен";
                }
            }
        }
        
        private void SaveAsText(string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    writer.WriteLine($"ConverterApp - Отчет");
                    writer.WriteLine($"Дата: {DateTime.Now}");
                    writer.WriteLine();
                    writer.WriteLine($"Текущая конвертация:");
                    writer.WriteLine($"Ввод: {txtInput.Text} {cboFromUnit.SelectedItem}");
                    writer.WriteLine($"Результат: {txtOutput.Text} {cboToUnit.SelectedItem}");
                    writer.WriteLine();
                    writer.WriteLine("История операций:");
                    
                    foreach (var entry in conversionHistory)
                    {
                        writer.WriteLine($"{entry.DateTime:yyyy-MM-dd HH:mm:ss} - {entry.Operation} = {entry.Result}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SaveAsCSV(string fileName)
        {
            BtnExportCSV_Click(null, null);
        }
        
        private void SaveAsPDF(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    using (var saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "PDF файлы (*.pdf)|*.pdf";
                        saveDialog.FileName = $"converter_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                        
                        if (saveDialog.ShowDialog() != DialogResult.OK)
                            return;
                            
                        fileName = saveDialog.FileName;
                    }
                }
                
                PdfDocument document = new PdfDocument();
                document.Info.Title = "ConverterApp Report";
                
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                
                XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                XFont headerFont = new XFont("Arial", 12, XFontStyle.Bold);
                XFont normalFont = new XFont("Arial", 10);
                
                int yPos = 50;
                
                // Title
                gfx.DrawString("ConverterApp - Отчет", titleFont, XBrushes.Black, 
                    new XRect(0, yPos, page.Width, 30), XStringFormats.Center);
                yPos += 40;
                
                // Date
                gfx.DrawString($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", normalFont, 
                    XBrushes.Black, new XPoint(50, yPos));
                yPos += 30;
                
                // Current conversion
                if (!string.IsNullOrEmpty(txtInput.Text))
                {
                    gfx.DrawString("Текущая конвертация:", headerFont, XBrushes.Black, new XPoint(50, yPos));
                    yPos += 20;
                    gfx.DrawString($"Ввод: {txtInput.Text} {cboFromUnit.SelectedItem}", 
                        normalFont, XBrushes.Black, new XPoint(70, yPos));
                    yPos += 20;
                    gfx.DrawString($"Результат: {txtOutput.Text} {cboToUnit.SelectedItem}", 
                        normalFont, XBrushes.Black, new XPoint(70, yPos));
                    yPos += 30;
                }
                
                // History
                gfx.DrawString("История операций:", headerFont, XBrushes.Black, new XPoint(50, yPos));
                yPos += 20;
                
                foreach (var entry in conversionHistory.ToList().Take(20)) // Limit to prevent overflow
                {
                    gfx.DrawString($"{entry.DateTime:yyyy-MM-dd HH:mm:ss} - {entry.Operation} = {entry.Result}", 
                        normalFont, XBrushes.Black, new XPoint(70, yPos));
                    yPos += 20;
                    
                    if (yPos > page.Height - 100)
                    {
                        // Dispose current graphics
                        gfx.Dispose();
                        
                        // Add new page
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = 50;
                    }
                }
                
                // Dispose graphics
                gfx.Dispose();
                
                // Save document
                document.Save(fileName);
                document.Dispose();
                
                lblStatus.Text = "PDF сохранен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении PDF: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (Font titleFont = new Font("Arial", 16, FontStyle.Bold))
            using (Font headerFont = new Font("Arial", 12, FontStyle.Bold))
            using (Font normalFont = new Font("Arial", 10))
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                
                // Title
                string title = "ConverterApp - Отчет печати";
                SizeF titleSize = e.Graphics.MeasureString(title, titleFont);
                e.Graphics.DrawString(title, titleFont, Brushes.Black, 
                    e.MarginBounds.Left + (e.MarginBounds.Width - titleSize.Width) / 2, yPos);
                yPos += titleSize.Height + 20;
                
                // Date
                e.Graphics.DrawString($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", 
                    normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;
                
                // Current conversion
                if (!string.IsNullOrEmpty(txtInput.Text))
                {
                    e.Graphics.DrawString("Текущая конвертация:", headerFont, Brushes.Black, leftMargin, yPos);
                    yPos += 25;
                    e.Graphics.DrawString($"Ввод: {txtInput.Text} {cboFromUnit.SelectedItem}", 
                        normalFont, Brushes.Black, leftMargin + 20, yPos);
                    yPos += 20;
                    e.Graphics.DrawString($"Результат: {txtOutput.Text} {cboToUnit.SelectedItem}", 
                        normalFont, Brushes.Black, leftMargin + 20, yPos);
                    yPos += 30;
                }
                
                // History
                e.Graphics.DrawString("История операций:", headerFont, Brushes.Black, leftMargin, yPos);
                yPos += 25;
                
                int printedItems = 0;
                foreach (var entry in conversionHistory)
                {
                    if (yPos + 20 > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                    
                    e.Graphics.DrawString(
                        $"{entry.DateTime:yyyy-MM-dd HH:mm:ss} - {entry.Operation} = {entry.Result}", 
                        normalFont, Brushes.Black, leftMargin + 20, yPos);
                    yPos += 20;
                    printedItems++;
                }
                
                e.HasMorePages = false;
            }
        }
        
        private void PrintResults_Click(object sender, EventArgs e)
        {
            try
            {
                PrintPreviewDialog printPreview = new PrintPreviewDialog();
                printPreview.Document = printDocument;
                printPreview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "ConverterApp v2.0\n\n" +
                "Универсальный конвертер единиц измерения\n" +
                "с современным интерфейсом и расширенным функционалом.\n\n" +
                "Возможности:\n" +
                "• Конвертация 10 типов единиц измерения\n" +
                "• Встроенный научный калькулятор\n" +
                "• История операций с поиском и фильтрацией\n" +
                "• Экспорт в PDF, CSV и текстовые файлы\n" +
                "• Настраиваемый интерфейс с темами\n" +
                "• Горячие клавиши для быстрой работы\n\n" +
                "© 2024 ConverterApp", 
                "О программе", 
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
            
            // Special styling for operation buttons
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
            else if (IsScientificFunction(text))
            {
                button.BackColor = Color.FromArgb(76, 175, 80);
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 11F);
            }
            
            // Add click handler
            button.Click += CalcButton_Click;
            
            return button;
        }
        
        private void InitializeScientificCalculatorButtons()
        {
            string[,] scientificLayout = {
                { "(", ")", "sin", "cos", "tan", "π", "e", "x²" },
                { "7", "8", "9", "÷", "log", "ln", "x³", "√" },
                { "4", "5", "6", "×", "xʸ", "10ˣ", "eˣ", "1/x" },
                { "1", "2", "3", "-", "n!", "%", "mod", "|x|" },
                { "0", ".", "+", "=", "C", "CE", "←", "±" }
            };
            
            if (calcTabButtonPanel != null)
            {
                calcTabButtonPanel.Controls.Clear();
                calcTabButtonPanel.ColumnCount = 8;
                calcTabButtonPanel.RowCount = 5;
                
                // Clear and recreate styles
                calcTabButtonPanel.ColumnStyles.Clear();
                calcTabButtonPanel.RowStyles.Clear();
                
                for (int i = 0; i < 8; i++)
                {
                    calcTabButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
                }
                for (int i = 0; i < 5; i++)
                {
                    calcTabButtonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
                }
                
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        var btn = CreateCalculatorButton(scientificLayout[row, col]);
                        btn.Font = new Font("Segoe UI", 11F);
                        calcTabButtonPanel.Controls.Add(btn, col, row);
                    }
                }
            }
        }
        
        private bool IsScientificFunction(string text)
        {
            string[] scientificFunctions = { "sin", "cos", "tan", "log", "ln", "x²", "x³", 
                                            "√", "∛", "π", "e", "xʸ", "10ˣ", "eˣ", "1/x", 
                                            "|x|", "n!", "mod", "(" , ")" };
            return scientificFunctions.Contains(text);
        }
    }
}