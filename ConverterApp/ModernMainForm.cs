using System;
using System.Collections.Generic;
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
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ConverterApp
{
    public partial class ModernMainForm : Form
    {
        private List<HistoryEntry> conversionHistory = new List<HistoryEntry>();
        private List<string> calcHistory = new List<string>();
        private Dictionary<string, List<string>> unitCategories = new Dictionary<string, List<string>>
        {
            { "🌡️ Температура", new List<string> { "°C", "°F", "K", "°R" } },
            { "💰 Валюта", new List<string> { "USD", "EUR", "RUB", "GBP", "JPY" } },
            { "⚖️ Масса", new List<string> { "g", "kg", "t", "lb", "oz" } },
            { "📏 Длина", new List<string> { "cm", "m", "km", "in", "ft", "yd", "mi" } },
            { "📐 Площадь", new List<string> { "cm²", "m²", "km²", "ft²", "ac" } },
            { "📊 Объем", new List<string> { "ml", "l", "m³", "gal", "pt" } },
            { "🕐 Время", new List<string> { "s", "min", "h", "d", "week" } },
            { "⚡ Энергия", new List<string> { "J", "kJ", "cal", "kWh" } },
            { "💪 Мощность", new List<string> { "W", "kW", "hp" } },
            { "🌊 Давление", new List<string> { "Pa", "kPa", "atm", "bar" } }
        };
        
        private Dictionary<string, double> currencyRates = new Dictionary<string, double>
        {
            { "USD", 1.0 },
            { "EUR", 0.92 },
            { "RUB", 92.5 },
            { "GBP", 0.79 },
            { "JPY", 149.5 }
        };
        
        private PrintDocument printDocument = new PrintDocument();
        private bool isAnimationEnabled = true;
        private int decimalPlaces = 2;
        private bool useThousandsSeparator = true;
        private bool isAutoConvertEnabled = false;
        
        // Calculator variables
        private double calcMemory = 0;
        private string calcOperation = "";
        private bool calcNewNumber = true;
        private CalculatorMode currentCalcMode = CalculatorMode.Basic;
        
        private enum CalculatorMode
        {
            Basic,
            Scientific,
            Programmer
        }
        
        private class HistoryEntry
        {
            public DateTime DateTime { get; set; }
            public string Operation { get; set; }
            public string Result { get; set; }
            public string Type { get; set; }
        }

        public ModernMainForm()
        {
            InitializeComponent();
            SetupEventHandlers();
            LoadSettings();
            InitializeControls();
            ApplyTheme();
            SetupKeyboardShortcuts();
        }
        
        private void SetupEventHandlers()
        {
            // Converter events
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;
            cboFromUnit.SelectedIndexChanged += CboUnit_SelectedIndexChanged;
            cboToUnit.SelectedIndexChanged += CboUnit_SelectedIndexChanged;
            txtInput.TextChanged += TxtInput_TextChanged;
            btnConvert.Click += BtnConvert_Click;
            btnClear.Click += BtnClear_Click;
            btnExport.Click += BtnExport_Click;
            btnExportPrint.Click += BtnExportPrint_Click;
            
            // Calculator events
            foreach (Control control in calcButtonPanel.Controls)
            {
                if (control is Button btn)
                {
                    btn.Click += CalcButton_Click;
                }
            }
            
            // History events
            btnClearHistory.Click += BtnClearHistory_Click;
            btnExportCSV.Click += BtnExportCSV_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnHistorySearch.Click += BtnHistorySearch_Click;
            cboHistoryFilter.SelectedIndexChanged += CboHistoryFilter_SelectedIndexChanged;
            
            // Calculator tab events
            btnBasicMode.Click += (s, e) => SwitchCalculatorMode(CalculatorMode.Basic);
            btnScientificMode.Click += (s, e) => SwitchCalculatorMode(CalculatorMode.Scientific);
            btnProgrammerMode.Click += (s, e) => SwitchCalculatorMode(CalculatorMode.Programmer);
            
            // Settings events
            btnApplySettings.Click += BtnApplySettings_Click;
            btnSaveSettings.Click += BtnSaveSettings_Click;
            btnResetSettings.Click += BtnResetSettings_Click;
            
            // Menu events
            openMenuItem.Click += OpenFile_Click;
            saveMenuItem.Click += SaveFile_Click;
            printMenuItem.Click += PrintResults_Click;
            exitMenuItem.Click += (s, e) => Application.Exit();
            aboutMenuItem.Click += About_Click;
            
            // Print document event
            printDocument.PrintPage += PrintDocument_PrintPage;
            
            // Tab control event
            mainTabControl.SelectedIndexChanged += MainTabControl_SelectedIndexChanged;
            
            // Enable drag & drop for unit swapping
            EnableDragDrop();
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
        }
        
        private async void UpdateCurrencyRates()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Simulated API call - in real implementation, use actual exchange rate API
                    await Task.Delay(100);
                    lblStatus.Text = "Курсы валют обновлены";
                }
            }
            catch
            {
                lblStatus.Text = "Используются офлайн курсы валют";
            }
        }
        
        private void CboType_SelectedIndexChanged(object sender, EventArgs e)
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
            
            if (isAutoConvertEnabled && !string.IsNullOrEmpty(txtInput.Text))
            {
                BtnConvert_Click(null, null);
            }
        }
        
        private void CboUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFromUnit.SelectedItem != null && cboToUnit.SelectedItem != null &&
                cboFromUnit.SelectedIndex == cboToUnit.SelectedIndex && cboFromUnit.Items.Count > 1)
            {
                cboToUnit.SelectedIndex = (cboToUnit.SelectedIndex + 1) % cboToUnit.Items.Count;
            }
            
            if (isAutoConvertEnabled && !string.IsNullOrEmpty(txtInput.Text))
            {
                BtnConvert_Click(null, null);
            }
        }
        
        private void TxtInput_TextChanged(object sender, EventArgs e)
        {
            if (isAutoConvertEnabled && !string.IsNullOrEmpty(txtInput.Text))
            {
                BtnConvert_Click(null, null);
            }
        }
        
        private void BtnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text)) return;
            
            if (!double.TryParse(txtInput.Text, out double value))
            {
                MessageBox.Show("Введите корректное число!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string fromUnit = cboFromUnit.SelectedItem?.ToString() ?? "";
            string toUnit = cboToUnit.SelectedItem?.ToString() ?? "";
            
            if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit))
            {
                MessageBox.Show("Выберите единицы измерения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string type = cboType.SelectedItem?.ToString() ?? "";
            double result = ConvertUnit(value, type, fromUnit, toUnit);
            
            // Apply number formatting
            string format = useThousandsSeparator ? $"N{decimalPlaces}" : $"F{decimalPlaces}";
            txtOutput.Text = result.ToString(format);
            
            // Add to history
            var historyEntry = new HistoryEntry
            {
                DateTime = DateTime.Now,
                Operation = $"{value} {fromUnit} → {toUnit}",
                Result = $"{result.ToString(format)} {toUnit}",
                Type = "Конвертация"
            };
            
            conversionHistory.Add(historyEntry);
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
            return ConvertThroughBase(value, type, fromUnit, toUnit);
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
                // Length
                {("cm", "m"), 0.01}, {("m", "km"), 0.001}, {("in", "cm"), 2.54},
                {("ft", "m"), 0.3048}, {("yd", "m"), 0.9144}, {("mi", "km"), 1.60934},
                
                // Mass
                {("g", "kg"), 0.001}, {("kg", "t"), 0.001}, {("lb", "kg"), 0.453592},
                {("oz", "g"), 28.3495},
                
                // Volume
                {("ml", "l"), 0.001}, {("l", "m³"), 0.001}, {("gal", "l"), 3.78541},
                {("pt", "l"), 0.473176},
                
                // Area
                {("cm²", "m²"), 0.0001}, {("m²", "km²"), 0.000001}, {("ft²", "m²"), 0.092903},
                {("ac", "m²"), 4046.86},
                
                // Time
                {("s", "min"), 1/60.0}, {("min", "h"), 1/60.0}, {("h", "d"), 1/24.0},
                {("d", "week"), 1/7.0},
                
                // Energy
                {("J", "kJ"), 0.001}, {("cal", "J"), 4.184}, {("kWh", "J"), 3.6e6},
                
                // Power
                {("W", "kW"), 0.001}, {("hp", "kW"), 0.7457},
                
                // Pressure
                {("Pa", "kPa"), 0.001}, {("atm", "kPa"), 101.325}, {("bar", "kPa"), 100}
            };
        }
        
        private double ConvertThroughBase(double value, string type, string from, string to)
        {
            // Define base units for each type
            var baseUnits = new Dictionary<string, string>
            {
                {"📏 Длина", "m"}, {"⚖️ Масса", "kg"}, {"📊 Объем", "l"},
                {"📐 Площадь", "m²"}, {"🕐 Время", "s"}, {"⚡ Энергия", "J"},
                {"💪 Мощность", "W"}, {"🌊 Давление", "Pa"}
            };
            
            if (!baseUnits.ContainsKey(type)) return value;
            
            string baseUnit = baseUnits[type];
            
            // Convert to base unit
            double baseValue = ConvertUnit(value, type, from, baseUnit);
            
            // Convert from base unit to target
            return ConvertUnit(baseValue, type, baseUnit, to);
        }
        
        private void AnimateArrow()
        {
            Timer timer = new Timer();
            timer.Interval = 50;
            int angle = 0;
            
            timer.Tick += (s, e) =>
            {
                angle += 30;
                if (angle >= 360)
                {
                    timer.Stop();
                    arrowLabel.Text = "➡️";
                }
                else
                {
                    // Simulate rotation with different arrow characters
                    string[] arrows = { "➡️", "↘️", "⬇️", "↙️", "⬅️", "↖️", "⬆️", "↗️" };
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
            
            switch (buttonText)
            {
                case "C":
                case "CE":
                    calcDisplay.Text = "0";
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
                    if (double.TryParse(calcDisplay.Text, out double val))
                    {
                        calcDisplay.Text = (-val).ToString();
                    }
                    break;
                    
                case "←":
                    if (calcDisplay.Text.Length > 1)
                        calcDisplay.Text = calcDisplay.Text.Substring(0, calcDisplay.Text.Length - 1);
                    else
                        calcDisplay.Text = "0";
                    break;
                    
                case ".":
                    if (!calcDisplay.Text.Contains("."))
                        calcDisplay.Text += ".";
                    break;
                    
                default:
                    // Number or scientific function
                    if (char.IsDigit(buttonText[0]))
                    {
                        if (calcNewNumber || calcDisplay.Text == "0")
                        {
                            calcDisplay.Text = buttonText;
                            calcNewNumber = false;
                        }
                        else
                        {
                            calcDisplay.Text += buttonText;
                        }
                    }
                    else
                    {
                        // Handle scientific functions
                        HandleScientificFunction(buttonText);
                    }
                    break;
            }
            
            // Update main converter input if integrated
            if (mainTabControl.SelectedTab == tabConverter && sender.Parent == calcButtonPanel)
            {
                txtInput.Text = calcDisplay.Text;
            }
        }
        
        private void SetOperation(string op)
        {
            if (!string.IsNullOrEmpty(calcOperation) && !calcNewNumber)
            {
                PerformCalculation();
            }
            
            if (double.TryParse(calcDisplay.Text, out double value))
            {
                calcMemory = value;
                calcOperation = op;
                calcNewNumber = true;
            }
        }
        
        private void PerformCalculation()
        {
            if (string.IsNullOrEmpty(calcOperation)) return;
            
            if (double.TryParse(calcDisplay.Text, out double currentValue))
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
                            calcDisplay.Text = "Ошибка";
                            return;
                        }
                        break;
                    case "%":
                        result = calcMemory * currentValue / 100;
                        break;
                }
                
                calcDisplay.Text = result.ToString();
                
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
                UpdateHistoryGrid();
                
                calcMemory = result;
                calcOperation = "";
                calcNewNumber = true;
            }
        }
        
        private void HandleScientificFunction(string function)
        {
            if (!double.TryParse(calcDisplay.Text, out double value)) return;
            
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
                    result = Math.Log10(value);
                    break;
                case "ln":
                    result = Math.Log(value);
                    break;
                case "1/x":
                    result = 1 / value;
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
            }
            
            calcDisplay.Text = result.ToString();
            calcNewNumber = true;
        }
        
        private void SwitchCalculatorMode(CalculatorMode mode)
        {
            currentCalcMode = mode;
            
            // Update button states
            btnBasicMode.BackColor = mode == CalculatorMode.Basic ? 
                Color.FromArgb(33, 150, 243) : Color.FromArgb(158, 158, 158);
            btnScientificMode.BackColor = mode == CalculatorMode.Scientific ? 
                Color.FromArgb(76, 175, 80) : Color.FromArgb(158, 158, 158);
            btnProgrammerMode.BackColor = mode == CalculatorMode.Programmer ? 
                Color.FromArgb(255, 152, 0) : Color.FromArgb(158, 158, 158);
            
            // Update calculator layout based on mode
            switch (mode)
            {
                case CalculatorMode.Basic:
                    InitializeBasicCalculatorButtons();
                    break;
                case CalculatorMode.Scientific:
                    // Already initialized in designer
                    break;
                case CalculatorMode.Programmer:
                    InitializeProgrammerCalculatorButtons();
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
                case CalculatorMode.Programmer: return "Программист";
                default: return "";
            }
        }
        
        private void InitializeProgrammerCalculatorButtons()
        {
            string[,] programmerLayout = {
                { "HEX", "DEC", "OCT", "BIN", "A", "B", "C", "D" },
                { "E", "F", "<<", ">>", "OR", "XOR", "NOT", "AND" },
                { "7", "8", "9", "÷", "(", ")", "MOD", "CE" },
                { "4", "5", "6", "×", "↑", "↓", "±", "←" },
                { "1", "2", "3", "-", "0", ".", "+", "=" }
            };
            
            calcTabButtonPanel.Controls.Clear();
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var btn = CreateCalculatorButton(programmerLayout[row, col]);
                    btn.Font = new Font("Segoe UI", 10F);
                    calcTabButtonPanel.Controls.Add(btn, col, row);
                }
            }
        }
        
        private void UpdateHistoryGrid()
        {
            historyDataGrid.Rows.Clear();
            
            var filter = cboHistoryFilter.SelectedItem?.ToString() ?? "Все";
            var searchText = txtHistorySearch.Text.ToLower();
            
            var filteredHistory = conversionHistory.Where(h =>
                (filter == "Все" || h.Type == filter) &&
                (string.IsNullOrEmpty(searchText) || 
                 h.Operation.ToLower().Contains(searchText) ||
                 h.Result.ToLower().Contains(searchText))
            ).OrderByDescending(h => h.DateTime);
            
            foreach (var entry in filteredHistory)
            {
                historyDataGrid.Rows.Add(
                    entry.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.Operation,
                    entry.Result
                );
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
                conversionHistory.Clear();
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
                            
                            foreach (var entry in conversionHistory)
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
            
            cboFromUnit.MouseDown += (s, e) => DoDragDrop(cboFromUnit.SelectedItem, DragDropEffects.Move);
            cboToUnit.MouseDown += (s, e) => DoDragDrop(cboToUnit.SelectedItem, DragDropEffects.Move);
            
            cboFromUnit.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            cboToUnit.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            
            cboFromUnit.DragDrop += (s, e) =>
            {
                if (e.Data.GetData(DataFormats.Text) != null)
                {
                    string temp = cboFromUnit.SelectedItem?.ToString();
                    cboFromUnit.SelectedItem = e.Data.GetData(DataFormats.Text);
                    cboToUnit.SelectedItem = temp;
                }
            };
            
            cboToUnit.DragDrop += (s, e) =>
            {
                if (e.Data.GetData(DataFormats.Text) != null)
                {
                    string temp = cboToUnit.SelectedItem?.ToString();
                    cboToUnit.SelectedItem = e.Data.GetData(DataFormats.Text);
                    cboFromUnit.SelectedItem = temp;
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
        
        private void SaveSettings()
        {
            try
            {
                var settings = new
                {
                    DecimalPlaces = decimalPlaces,
                    UseThousandsSeparator = useThousandsSeparator,
                    AnimationsEnabled = isAnimationEnabled,
                    AutoConvert = isAutoConvertEnabled,
                    Theme = cboTheme.SelectedItem?.ToString() ?? "Светлая"
                };
                
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("settings.json", json);
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
                if (File.Exists("settings.json"))
                {
                    string json = File.ReadAllText("settings.json");
                    dynamic settings = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    
                    numDecimalPlaces.Value = settings.DecimalPlaces;
                    chkThousandsSeparator.Checked = settings.UseThousandsSeparator;
                    chkAnimations.Checked = settings.AnimationsEnabled;
                    chkScientificNotation.Checked = settings.AutoConvert;
                    
                    for (int i = 0; i < cboTheme.Items.Count; i++)
                    {
                        if (cboTheme.Items[i].ToString() == settings.Theme.ToString())
                        {
                            cboTheme.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch
            {
                // Use default settings if loading fails
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
                
                foreach (var entry in conversionHistory.Take(20)) // Limit to prevent overflow
                {
                    gfx.DrawString($"{entry.DateTime:yyyy-MM-dd HH:mm:ss} - {entry.Operation} = {entry.Result}", 
                        normalFont, XBrushes.Black, new XPoint(70, yPos));
                    yPos += 20;
                    
                    if (yPos > page.Height - 100)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = 50;
                    }
                }
                
                document.Save(fileName);
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
    }
}