using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Security.Cryptography;

namespace ConverterApp
{
    public partial class MainForm : Form
    {
        private Form calcForm;
        private TextBox calcDisplay;
        private bool isCalculatorPinned = false;
        private List<string> conversionHistory = new List<string>();
        private List<string> calcHistory = new List<string>();
        private Dictionary<string, List<string>> unitCategories = new Dictionary<string, List<string>>
        {
            { "Длина", new List<string> { "cm", "m", "km", "in", "ft", "yd", "mi" } },
            { "Масса", new List<string> { "g", "kg", "t", "lb", "oz" } },
            { "Объем", new List<string> { "ml", "l", "m³", "gal", "qt" } },
            { "Температура", new List<string> { "°C", "°F", "K" } },
            { "Скорость", new List<string> { "km/h", "m/s", "mph", "kn" } },
            { "Площадь", new List<string> { "cm²", "m²", "km²", "ft²", "ac" } },
            { "Время", new List<string> { "s", "min", "h", "d" } },
            { "Энергия", new List<string> { "J", "kJ", "cal", "kWh" } },
            { "Мощность", new List<string> { "W", "kW", "hp" } },
            { "Давление", new List<string> { "Pa", "kPa", "atm", "bar" } }
        };
        private PrintDocument printDocument = new PrintDocument();

        public MainForm()
        {
            InitializeComponent();
            InitializeCalculator();
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;
            cboFromUnit.SelectedIndexChanged += CboUnit_SelectedIndexChanged;
            Calculator_Click(this, EventArgs.Empty);
            printDocument.PrintPage += PrintDocument_PrintPage;
            cboType.SelectedIndex = 0; // Инициализация с первого типа
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
            }
        }

        private void CboUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFromUnit.SelectedItem != null && cboToUnit.SelectedItem != null &&
                cboFromUnit.SelectedIndex == cboToUnit.SelectedIndex && cboFromUnit.Items.Count > 1)
            {
                cboToUnit.SelectedIndex = (cboToUnit.SelectedIndex + 1) % cboToUnit.Items.Count;
            }
        }

        private void InitializeCalculator()
        {
            calcForm = new Form
            {
                Text = "Calculator",
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Size = new Size(300, 400),
                StartPosition = FormStartPosition.CenterParent,
                TopMost = isCalculatorPinned,
                Visible = false
            };

            calcDisplay = new TextBox { Width = 280, Location = new Point(10, 10), ReadOnly = true, Multiline = true, Height = 50 };
            Button[] calcButtons = new Button[]
            {
                new Button { Text = "7", Location = new Point(10, 70), Size = new Size(60, 60) },
                new Button { Text = "8", Location = new Point(80, 70), Size = new Size(60, 60) },
                new Button { Text = "9", Location = new Point(150, 70), Size = new Size(60, 60) },
                new Button { Text = "/", Location = new Point(220, 70), Size = new Size(60, 60) },
                new Button { Text = "CE", Location = new Point(220, 140), Size = new Size(60, 60) },
                new Button { Text = "4", Location = new Point(10, 140), Size = new Size(60, 60) },
                new Button { Text = "5", Location = new Point(80, 140), Size = new Size(60, 60) },
                new Button { Text = "6", Location = new Point(150, 140), Size = new Size(60, 60) },
                new Button { Text = "*", Location = new Point(220, 140), Size = new Size(60, 60) },
                new Button { Text = "1", Location = new Point(10, 210), Size = new Size(60, 60) },
                new Button { Text = "2", Location = new Point(80, 210), Size = new Size(60, 60) },
                new Button { Text = "3", Location = new Point(150, 210), Size = new Size(60, 60) },
                new Button { Text = "-", Location = new Point(220, 210), Size = new Size(60, 60) },
                new Button { Text = "0", Location = new Point(10, 280), Size = new Size(130, 60) },
                new Button { Text = "+", Location = new Point(150, 280), Size = new Size(60, 60) },
                new Button { Text = "%", Location = new Point(220, 280), Size = new Size(60, 60) },
                new Button { Text = "=", Location = new Point(10, 350), Size = new Size(270, 60) }
            };

            foreach (var button in calcButtons)
            {
                button.Click += CalcButton_Click;
                calcForm.Controls.Add(button);
            }

            calcForm.Controls.Add(calcDisplay);
            calcForm.FormClosing += (s, e) => { e.Cancel = true; calcForm.Hide(); };
        }

        private void CalcButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text == "CE")
            {
                calcDisplay.Text = "";
            }
            else if (button.Text == "=")
            {
                try
                {
                    var result = new System.Data.DataTable().Compute(calcDisplay.Text, null);
                    string operation = calcDisplay.Text + " = " + result.ToString();
                    calcDisplay.Text = result.ToString();
                    calcHistory.Add($"{operation} ({DateTime.Now:HH:mm:ss})");
                }
                catch
                {
                    calcDisplay.Text = "Ошибка";
                }
            }
            else
            {
                calcDisplay.Text += button.Text;
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
            if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit) || fromUnit == toUnit)
            {
                MessageBox.Show("Выберите разные единицы или проверьте ввод!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string type = cboType.SelectedItem?.ToString() ?? "";
            double result = ConvertUnit(value, type, fromUnit, toUnit);
            txtOutput.Text = result.ToString("F2");
            conversionHistory.Add($"{value} {fromUnit} → {result} {toUnit} ({DateTime.Now:HH:mm:ss})");
            lblStatus.Text = "Конвертация выполнена";
        }

        private double ConvertUnit(double value, string type, string fromUnit, string toUnit)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit)) return value;

            Dictionary<(string, string), double> conversionFactors = new Dictionary<(string, string), double>
            {
                // Длина
                (("cm", "m"), 0.01), (("m", "cm"), 100), (("km", "m"), 1000), (("m", "km"), 0.001),
                (("in", "cm"), 2.54), (("cm", "in"), 0.393701), (("ft", "m"), 0.3048), (("m", "ft"), 3.28084),
                (("yd", "m"), 0.9144), (("m", "yd"), 1.09361), (("mi", "km"), 1.60934), (("km", "mi"), 0.621371),
                // Масса
                (("g", "kg"), 0.001), (("kg", "g"), 1000), (("t", "kg"), 1000), (("kg", "t"), 0.001),
                (("lb", "kg"), 0.453592), (("kg", "lb"), 2.20462), (("oz", "g"), 28.3495), (("g", "oz"), 0.035274),
                // Объем
                (("ml", "l"), 0.001), (("l", "ml"), 1000), (("m³", "l"), 1000), (("l", "m³"), 0.001),
                (("gal", "l"), 3.78541), (("l", "gal"), 0.264172), (("qt", "l"), 0.946353), (("l", "qt"), 1.05669),
                // Температура
                (("°C", "°F"), v => (v * 9 / 5) + 32), (("°F", "°C"), v => (v - 32) * 5 / 9),
                (("°C", "K"), v => v + 273.15), (("K", "°C"), v => v - 273.15),
                (("°F", "K"), v => (v + 459.67) * 5 / 9), (("K", "°F"), v => (v * 9 / 5) - 459.67),
                // Скорость
                (("km/h", "m/s"), 1 / 3.6), (("m/s", "km/h"), 3.6), (("mph", "km/h"), 1.60934), (("km/h", "mph"), 0.621371),
                (("kn", "km/h"), 1.852), (("km/h", "kn"), 0.539957),
                // Площадь
                (("cm²", "m²"), 0.0001), (("m²", "cm²"), 10000), (("km²", "m²"), 1000000), (("m²", "km²"), 0.000001),
                (("ft²", "m²"), 0.092903), (("m²", "ft²"), 10.7639), (("ac", "m²"), 4046.86), (("m²", "ac"), 0.000247105),
                // Время
                (("s", "min"), 1 / 60.0), (("min", "s"), 60), (("min", "h"), 1 / 60.0), (("h", "min"), 60),
                (("d", "h"), 24), (("h", "d"), 1 / 24.0),
                // Энергия
                (("J", "kJ"), 0.001), (("kJ", "J"), 1000), (("cal", "J"), 4.184), (("J", "cal"), 0.238845),
                (("kWh", "J"), 3.6e6), (("J", "kWh"), 1 / 3.6e6),
                // Мощность
                (("W", "kW"), 0.001), (("kW", "W"), 1000), (("hp", "kW"), 0.7457), (("kW", "hp"), 1.34102),
                // Давление
                (("Pa", "kPa"), 0.001), (("kPa", "Pa"), 1000), (("atm", "kPa"), 101.325), (("kPa", "atm"), 0.00986923),
                (("bar", "kPa"), 100), (("kPa", "bar"), 0.01)
            };

            var key = (fromUnit, toUnit);
            if (conversionFactors.ContainsKey(key))
            {
                var factor = conversionFactors[key];
                return factor is double d ? value * d : ((Func<double, double>)factor)(value);
            }
            return value; // Возвращаем исходное значение при отсутствии конверсии
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = "";
            txtOutput.Text = "";
            cboType.SelectedIndex = -1;
            cboFromUnit.SelectedIndex = -1;
            cboToUnit.SelectedIndex = -1;
            lblStatus.Text = "Очищено";
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtInput.Text = File.ReadAllText(openFileDialog.FileName);
                        lblStatus.Text = "Файл загружен";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF файлы (*.pdf)|*.pdf|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.DefaultExt = "pdf";
                saveFileDialog.FileName = $"conversion_report_{DateTime.Now:yyyyMMdd_HHmmss}";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                    if (extension == ".pdf")
                        SaveAsPDF(saveFileDialog.FileName);
                    else
                        SaveAsText(saveFileDialog.FileName);
                    lblStatus.Text = "Результат сохранен";
                }
            }
        }

        private void SaveAsText(string fileName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.WriteLine($"Дата: {DateTime.Now}");
                    writer.WriteLine("Конвертации:");
                    foreach (var entry in conversionHistory)
                        writer.WriteLine(entry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении текста: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveAsPDF(string fileName)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "ConverterApp Report";
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 12);

                string reportText = $"ConverterApp - Отчет\nДата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n\nТекущая конвертация:\nВвод: {txtInput.Text} {cboFromUnit.SelectedItem}\nВывод: {txtOutput.Text} {cboToUnit.SelectedItem}\n\nИстория конвертаций:";
                gfx.DrawString(reportText, font, XBrushes.Black, new XPoint(50, 50));
                float yPos = 150;
                foreach (var entry in conversionHistory)
                {
                    gfx.DrawString(entry, font, XBrushes.Black, new XPoint(50, yPos));
                    yPos += 20;
                    if (yPos > page.Height - 100 && conversionHistory.IndexOf(entry) < conversionHistory.Count - 1)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = 50;
                    }
                }

                document.Save(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении PDF: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (Font font = new Font("Arial", 12))
            {
                float yPos = 0;
                int margin = 50;
                string printText = $"ConverterApp - Печать отчета\nДата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n\nТекущая конвертация:\nВвод: {txtInput.Text} {cboFromUnit.SelectedItem}\nВывод: {txtOutput.Text} {cboToUnit.SelectedItem}\n\nИстория конвертаций:\n";
                foreach (var entry in conversionHistory)
                {
                    printText += $"{entry}\n";
                }

                float linesPerPage = e.MarginBounds.Height / font.GetHeight(e.Graphics);
                int charCount = e.Graphics.MeasureString(printText, font).Width > e.MarginBounds.Width ? printText.Length : 0;
                int lineCount = (int)Math.Floor(linesPerPage);
                int firstChar = 0;
                while (charCount > 0)
                {
                    e.Graphics.DrawString(printText.Substring(firstChar, charCount), font, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);
                    firstChar += charCount;
                    charCount -= charCount;
                    e.HasMorePages = firstChar < printText.Length;
                    if (e.HasMorePages)
                    {
                        printText = printText.Substring(firstChar);
                        e.Graphics.DrawString(printText, font, Brushes.Black, margin, yPos + margin);
                    }
                    break;
                }
            }
        }

        private void PrintResults_Click(object sender, EventArgs e)
        {
            try
            {
                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog
                {
                    Document = printDocument
                };
                printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Calculator_Click(object sender, EventArgs e)
        {
            if (!calcForm.Visible)
            {
                calcForm.Show(this);
                lblStatus.Text = "Калькулятор открыт";
            }
            else
            {
                calcForm.Hide();
                lblStatus.Text = "Калькулятор закрыт";
            }
        }

        private void PinCalculator_Click(object sender, EventArgs e)
        {
            isCalculatorPinned = !isCalculatorPinned;
            calcForm.TopMost = isCalculatorPinned;
            pinCalculatorMenuItem.Text = isCalculatorPinned ? "Открепить калькулятор" : "Закрепить калькулятор";
            lblStatus.Text = isCalculatorPinned ? "Калькулятор закреплен" : "Калькулятор откреплен";
        }

        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Это приложение для конвертации единиц измерения.\nВыберите тип, введите значение и единицы для конвертации.", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void History_Click(object sender, EventArgs e)
        {
            string historyText = string.Join("\n", conversionHistory);
            if (string.IsNullOrEmpty(historyText))
                historyText = "История пуста.";
            MessageBox.Show(historyText, "История конвертаций", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}