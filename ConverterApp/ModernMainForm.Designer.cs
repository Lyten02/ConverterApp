using System.Drawing;
using System.Windows.Forms;
using System;

namespace ConverterApp
{
    partial class ModernMainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (components != null)
                {
                    components.Dispose();
                }
                
                // Dispose HttpClient (static, so only if form is last instance)
                // Note: In production, HttpClient should be managed at application level
                
                // Dispose PrintDocument
                if (printDocument != null)
                {
                    printDocument.Dispose();
                    printDocument = null;
                }
                
                // Dispose CancellationTokenSource
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Main Menu
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            
            // Tab Control
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabConverter = new System.Windows.Forms.TabPage();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.tabCalculator = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            
            // Converter Tab Components
            this.converterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.typePanel = new System.Windows.Forms.Panel();
            this.lblConversionType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            
            this.conversionPanel = new System.Windows.Forms.TableLayoutPanel();
            this.inputGroupBox = new System.Windows.Forms.GroupBox();
            this.arrowLabel = new System.Windows.Forms.Label();
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnExportPrint = new System.Windows.Forms.Button();
            
            
            // History Tab Components
            this.historyPanel = new System.Windows.Forms.TableLayoutPanel();
            this.historyFilterPanel = new System.Windows.Forms.Panel();
            this.cboHistoryFilter = new System.Windows.Forms.ComboBox();
            this.txtHistorySearch = new System.Windows.Forms.TextBox();
            this.btnHistorySearch = new System.Windows.Forms.Button();
            this.historyDataGrid = new System.Windows.Forms.DataGridView();
            this.historyButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.btnExportCSV = new System.Windows.Forms.Button();
            this.btnExportPDF = new System.Windows.Forms.Button();
            
            // Calculator Tab Components
            this.calcTabPanel = new System.Windows.Forms.TableLayoutPanel();
            this.calcModePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBasicMode = new System.Windows.Forms.Button();
            this.btnScientificMode = new System.Windows.Forms.Button();
            this.btnProgrammerMode = new System.Windows.Forms.Button();
            
            // Settings Tab Components
            this.settingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.numberFormatGroup = new System.Windows.Forms.GroupBox();
            this.lblDecimalPlaces = new System.Windows.Forms.Label();
            this.numDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.chkThousandsSeparator = new System.Windows.Forms.CheckBox();
            this.chkScientificNotation = new System.Windows.Forms.CheckBox();
            
            this.appearanceGroup = new System.Windows.Forms.GroupBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cboTheme = new System.Windows.Forms.ComboBox();
            this.chkAnimations = new System.Windows.Forms.CheckBox();
            this.chkSoundEffects = new System.Windows.Forms.CheckBox();
            
            this.hotkeyGroup = new System.Windows.Forms.GroupBox();
            this.lblHotkeyInfo = new System.Windows.Forms.Label();
            
            this.settingsButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.btnApplySettings = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            
            // Status Bar
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            
            // Suspend Layout
            this.menuStrip.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            
            // Form Properties
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "ModernMainForm";
            this.Text = "ConverterApp - Универсальный конвертер";
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Menu Strip
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip.TabIndex = 0;
            
            // File Menu
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.openMenuItem,
                this.saveMenuItem,
                this.printMenuItem,
                this.toolStripSeparator1,
                this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileMenuItem.Text = "Файл";
            
            // Help Menu
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(65, 20);
            this.helpMenuItem.Text = "Справка";
            
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileMenuItem,
                this.helpMenuItem});
            
            // Main Tab Control
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 24);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1024, 722);
            this.mainTabControl.TabIndex = 1;
            this.mainTabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            
            // Converter Tab
            this.tabConverter.Text = "🔄 ConvertApp";
            this.tabConverter.UseVisualStyleBackColor = true;
            this.tabConverter.Padding = new System.Windows.Forms.Padding(10);
            
            // History Tab
            this.tabHistory.Text = "📊 История";
            this.tabHistory.UseVisualStyleBackColor = true;
            this.tabHistory.Padding = new System.Windows.Forms.Padding(10);
            
            // Calculator Tab
            this.tabCalculator.Text = "🧮 Calculator";
            this.tabCalculator.UseVisualStyleBackColor = true;
            this.tabCalculator.Padding = new System.Windows.Forms.Padding(10);
            
            // Settings Tab
            this.tabSettings.Text = "⚙️ Настройки";
            this.tabSettings.UseVisualStyleBackColor = true;
            this.tabSettings.Padding = new System.Windows.Forms.Padding(10);
            
            this.mainTabControl.TabPages.Add(this.tabConverter);
            this.mainTabControl.TabPages.Add(this.tabHistory);
            this.mainTabControl.TabPages.Add(this.tabCalculator);
            this.mainTabControl.TabPages.Add(this.tabSettings);
            
            // Status Strip
            this.statusStrip.Location = new System.Drawing.Point(0, 746);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip.TabIndex = 2;
            
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 17);
            this.lblStatus.Text = "Готов к работе";
            this.statusStrip.Items.Add(this.lblStatus);
            
            // Add controls to form
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            
            // Resume Layout
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            
            InitializeConverterTab();
            InitializeHistoryTab();
            InitializeCalculatorTab();
            InitializeSettingsTab();
        }
        
        private void InitializeConverterTab()
        {
            // Main converter panel layout
            this.converterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.converterPanel.Dock = DockStyle.Fill;
            this.converterPanel.ColumnCount = 1;
            this.converterPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.converterPanel.RowCount = 1;
            this.converterPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            
            // Create centered panel for conversion controls
            var leftPanel = new TableLayoutPanel();
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.RowCount = 4;
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            leftPanel.Padding = new Padding(50, 20, 50, 20); // Add horizontal padding
            
            // Type selection panel
            this.typePanel = new Panel();
            this.typePanel.Dock = DockStyle.Fill;
            this.typePanel.Padding = new Padding(20);
            
            this.lblConversionType = new Label();
            this.lblConversionType.Text = "Выберите тип конвертации:";
            this.lblConversionType.Font = new Font("Segoe UI", 12F);
            this.lblConversionType.Location = new Point(20, 10);
            this.lblConversionType.Size = new Size(300, 25);
            
            this.cboType = new ComboBox();
            this.cboType.Font = new Font("Segoe UI", 11F);
            this.cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboType.Location = new Point(20, 40);
            this.cboType.Size = new Size(400, 35);
            this.cboType.Items.AddRange(new object[] {
                "🌡️ Температура",
                "💰 Валюта",
                "⚖️ Масса",
                "📏 Длина",
                "📐 Площадь",
                "📊 Объем",
                "🕐 Время",
                "⚡ Энергия",
                "💪 Мощность",
                "🌊 Давление"
            });
            
            this.typePanel.Controls.Add(this.lblConversionType);
            this.typePanel.Controls.Add(this.cboType);
            
            // Conversion panel
            this.conversionPanel = new TableLayoutPanel();
            this.conversionPanel.Dock = DockStyle.Fill;
            this.conversionPanel.ColumnCount = 3;
            this.conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            this.conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            this.conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            this.conversionPanel.Padding = new Padding(30);
            
            // Input group
            this.inputGroupBox = new GroupBox();
            this.inputGroupBox.Text = "Ввод";
            this.inputGroupBox.Dock = DockStyle.Fill;
            this.inputGroupBox.Font = new Font("Segoe UI", 10F);
            this.inputGroupBox.Padding = new Padding(10);
            
            var inputPanel = new TableLayoutPanel();
            inputPanel.Dock = DockStyle.Fill;
            inputPanel.RowCount = 2;
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            
            this.txtInput = new TextBox();
            this.txtInput.Dock = DockStyle.Fill;
            this.txtInput.Font = new Font("Segoe UI", 18F);
            this.txtInput.TextAlign = HorizontalAlignment.Center;
            this.txtInput.Height = 50;
            
            this.cboFromUnit = new ComboBox();
            this.cboFromUnit.Dock = DockStyle.Fill;
            this.cboFromUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFromUnit.Font = new Font("Segoe UI", 11F);
            
            inputPanel.Controls.Add(this.txtInput, 0, 0);
            inputPanel.Controls.Add(this.cboFromUnit, 0, 1);
            this.inputGroupBox.Controls.Add(inputPanel);
            
            // Arrow
            this.arrowLabel = new Label();
            this.arrowLabel.Text = "➡️";
            this.arrowLabel.Font = new Font("Segoe UI", 36F);
            this.arrowLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.arrowLabel.Dock = DockStyle.Fill;
            
            // Output group
            this.outputGroupBox = new GroupBox();
            this.outputGroupBox.Text = "Результат";
            this.outputGroupBox.Dock = DockStyle.Fill;
            this.outputGroupBox.Font = new Font("Segoe UI", 10F);
            this.outputGroupBox.Padding = new Padding(10);
            
            var outputPanel = new TableLayoutPanel();
            outputPanel.Dock = DockStyle.Fill;
            outputPanel.RowCount = 2;
            outputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            outputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            
            this.txtOutput = new TextBox();
            this.txtOutput.Dock = DockStyle.Fill;
            this.txtOutput.Font = new Font("Segoe UI", 18F);
            this.txtOutput.TextAlign = HorizontalAlignment.Center;
            this.txtOutput.ReadOnly = true;
            this.txtOutput.BackColor = Color.WhiteSmoke;
            this.txtOutput.Height = 50;
            
            this.cboToUnit = new ComboBox();
            this.cboToUnit.Dock = DockStyle.Fill;
            this.cboToUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboToUnit.Font = new Font("Segoe UI", 11F);
            
            outputPanel.Controls.Add(this.txtOutput, 0, 0);
            outputPanel.Controls.Add(this.cboToUnit, 0, 1);
            this.outputGroupBox.Controls.Add(outputPanel);
            
            this.conversionPanel.Controls.Add(this.inputGroupBox, 0, 0);
            this.conversionPanel.Controls.Add(this.arrowLabel, 1, 0);
            this.conversionPanel.Controls.Add(this.outputGroupBox, 2, 0);
            
            // Button panel
            this.buttonPanel = new FlowLayoutPanel();
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            this.buttonPanel.Padding = new Padding(20);
            
            this.btnConvert = CreateStyledButton("Расчитать", Color.FromArgb(33, 150, 243));
            this.btnClear = CreateStyledButton("Очистить", Color.FromArgb(158, 158, 158));
            this.btnExport = CreateStyledButton("Экспорт", Color.FromArgb(76, 175, 80));
            this.btnExportPrint = CreateStyledButton("Экспорт и Печать", Color.FromArgb(255, 152, 0));
            
            this.buttonPanel.Controls.Add(this.btnConvert);
            this.buttonPanel.Controls.Add(this.btnClear);
            this.buttonPanel.Controls.Add(this.btnExport);
            this.buttonPanel.Controls.Add(this.btnExportPrint);
            
            leftPanel.Controls.Add(this.typePanel, 0, 0);
            leftPanel.Controls.Add(this.conversionPanel, 0, 1);
            leftPanel.Controls.Add(this.buttonPanel, 0, 3);
            
            this.converterPanel.Controls.Add(leftPanel, 0, 0);
            
            this.tabConverter.Controls.Add(this.converterPanel);
        }
        
        
        private void InitializeHistoryTab()
        {
            this.historyPanel = new TableLayoutPanel();
            this.historyPanel.Dock = DockStyle.Fill;
            this.historyPanel.RowCount = 3;
            this.historyPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            this.historyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.historyPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            
            // Filter panel
            this.historyFilterPanel = new Panel();
            this.historyFilterPanel.Dock = DockStyle.Fill;
            this.historyFilterPanel.Padding = new Padding(10);
            
            var filterLabel = new Label();
            filterLabel.Text = "Фильтр:";
            filterLabel.Location = new Point(10, 20);
            filterLabel.Size = new Size(60, 25);
            
            this.cboHistoryFilter = new ComboBox();
            this.cboHistoryFilter.Location = new Point(80, 18);
            this.cboHistoryFilter.Size = new Size(150, 25);
            this.cboHistoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboHistoryFilter.Items.AddRange(new[] { "Все", "Конвертации", "Калькулятор" });
            this.cboHistoryFilter.SelectedIndex = 0;
            
            var searchLabel = new Label();
            searchLabel.Text = "Поиск:";
            searchLabel.Location = new Point(250, 20);
            searchLabel.Size = new Size(50, 25);
            
            this.txtHistorySearch = new TextBox();
            this.txtHistorySearch.Location = new Point(310, 18);
            this.txtHistorySearch.Size = new Size(200, 25);
            
            this.btnHistorySearch = new Button();
            this.btnHistorySearch.Text = "🔍";
            this.btnHistorySearch.Location = new Point(520, 18);
            this.btnHistorySearch.Size = new Size(30, 25);
            
            this.historyFilterPanel.Controls.Add(filterLabel);
            this.historyFilterPanel.Controls.Add(this.cboHistoryFilter);
            this.historyFilterPanel.Controls.Add(searchLabel);
            this.historyFilterPanel.Controls.Add(this.txtHistorySearch);
            this.historyFilterPanel.Controls.Add(this.btnHistorySearch);
            
            // Data grid
            this.historyDataGrid = new DataGridView();
            this.historyDataGrid.Dock = DockStyle.Fill;
            this.historyDataGrid.AllowUserToAddRows = false;
            this.historyDataGrid.AllowUserToDeleteRows = false;
            this.historyDataGrid.ReadOnly = true;
            this.historyDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.historyDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Add columns
            this.historyDataGrid.Columns.Add("DateTime", "Дата/Время");
            this.historyDataGrid.Columns.Add("Operation", "Операция");
            this.historyDataGrid.Columns.Add("Result", "Результат");
            
            // Button panel
            this.historyButtonPanel = new FlowLayoutPanel();
            this.historyButtonPanel.Dock = DockStyle.Fill;
            this.historyButtonPanel.FlowDirection = FlowDirection.LeftToRight;
            this.historyButtonPanel.Padding = new Padding(10);
            
            this.btnClearHistory = CreateStyledButton("Очистить историю", Color.FromArgb(244, 67, 54));
            this.btnExportCSV = CreateStyledButton("Экспорт в CSV", Color.FromArgb(76, 175, 80));
            this.btnExportPDF = CreateStyledButton("Экспорт в PDF", Color.FromArgb(33, 150, 243));
            
            this.historyButtonPanel.Controls.Add(this.btnClearHistory);
            this.historyButtonPanel.Controls.Add(this.btnExportCSV);
            this.historyButtonPanel.Controls.Add(this.btnExportPDF);
            
            this.historyPanel.Controls.Add(this.historyFilterPanel, 0, 0);
            this.historyPanel.Controls.Add(this.historyDataGrid, 0, 1);
            this.historyPanel.Controls.Add(this.historyButtonPanel, 0, 2);
            
            this.tabHistory.Controls.Add(this.historyPanel);
        }
        
        private void InitializeCalculatorTab()
        {
            this.calcTabPanel = new TableLayoutPanel();
            this.calcTabPanel.Dock = DockStyle.Fill;
            this.calcTabPanel.RowCount = 3;
            this.calcTabPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            this.calcTabPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.calcTabPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            
            // Calculator display
            this.calcTabDisplay = new TextBox();
            this.calcTabDisplay.Dock = DockStyle.Fill;
            this.calcTabDisplay.Font = new Font("Segoe UI", 24F);
            this.calcTabDisplay.TextAlign = HorizontalAlignment.Right;
            this.calcTabDisplay.ReadOnly = true;
            this.calcTabDisplay.BackColor = Color.White;
            this.calcTabDisplay.Text = "0";
            
            // Calculator buttons panel
            this.calcTabButtonPanel = new TableLayoutPanel();
            this.calcTabButtonPanel.Dock = DockStyle.Fill;
            this.calcTabButtonPanel.ColumnCount = 8;
            this.calcTabButtonPanel.RowCount = 5;
            
            for (int i = 0; i < 8; i++)
            {
                this.calcTabButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            }
            for (int i = 0; i < 5; i++)
            {
                this.calcTabButtonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            }
            
            // Mode selection panel
            this.calcModePanel = new FlowLayoutPanel();
            this.calcModePanel.Dock = DockStyle.Fill;
            this.calcModePanel.FlowDirection = FlowDirection.LeftToRight;
            
            this.btnBasicMode = CreateStyledButton("Обычный", Color.FromArgb(33, 150, 243));
            this.btnScientificMode = CreateStyledButton("Научный", Color.FromArgb(76, 175, 80));
            this.btnProgrammerMode = CreateStyledButton("Программист", Color.FromArgb(255, 152, 0));
            
            this.calcModePanel.Controls.Add(this.btnBasicMode);
            this.calcModePanel.Controls.Add(this.btnScientificMode);
            this.calcModePanel.Controls.Add(this.btnProgrammerMode);
            
            this.calcTabPanel.Controls.Add(this.calcTabDisplay, 0, 0);
            this.calcTabPanel.Controls.Add(this.calcTabButtonPanel, 0, 1);
            this.calcTabPanel.Controls.Add(this.calcModePanel, 0, 2);
            
            this.tabCalculator.Controls.Add(this.calcTabPanel);
            
            // Initialize with basic calculator buttons
            InitializeBasicCalculatorButtons();
        }
        
        private void InitializeSettingsTab()
        {
            this.settingsPanel = new TableLayoutPanel();
            this.settingsPanel.Dock = DockStyle.Fill;
            this.settingsPanel.RowCount = 4;
            this.settingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            this.settingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            this.settingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            this.settingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            this.settingsPanel.Padding = new Padding(20);
            
            // Number format group
            this.numberFormatGroup = new GroupBox();
            this.numberFormatGroup.Text = "📊 Формат чисел";
            this.numberFormatGroup.Dock = DockStyle.Fill;
            this.numberFormatGroup.Font = new Font("Segoe UI", 10F);
            
            var numberPanel = new TableLayoutPanel();
            numberPanel.Dock = DockStyle.Fill;
            numberPanel.RowCount = 3;
            numberPanel.ColumnCount = 2;
            numberPanel.Padding = new Padding(10);
            
            this.lblDecimalPlaces = new Label();
            this.lblDecimalPlaces.Text = "Десятичные знаки:";
            this.lblDecimalPlaces.Size = new Size(150, 25);
            
            this.numDecimalPlaces = new NumericUpDown();
            this.numDecimalPlaces.Minimum = 0;
            this.numDecimalPlaces.Maximum = 10;
            this.numDecimalPlaces.Value = 2;
            this.numDecimalPlaces.Size = new Size(100, 25);
            
            this.chkThousandsSeparator = new CheckBox();
            this.chkThousandsSeparator.Text = "Разделитель тысяч";
            this.chkThousandsSeparator.Checked = true;
            
            this.chkScientificNotation = new CheckBox();
            this.chkScientificNotation.Text = "Научная нотация (для больших чисел)";
            
            numberPanel.Controls.Add(this.lblDecimalPlaces, 0, 0);
            numberPanel.Controls.Add(this.numDecimalPlaces, 1, 0);
            numberPanel.Controls.Add(this.chkThousandsSeparator, 0, 1);
            numberPanel.Controls.Add(this.chkScientificNotation, 0, 2);
            
            this.numberFormatGroup.Controls.Add(numberPanel);
            
            // Appearance group
            this.appearanceGroup = new GroupBox();
            this.appearanceGroup.Text = "🎨 Внешний вид";
            this.appearanceGroup.Dock = DockStyle.Fill;
            this.appearanceGroup.Font = new Font("Segoe UI", 10F);
            
            var appearancePanel = new TableLayoutPanel();
            appearancePanel.Dock = DockStyle.Fill;
            appearancePanel.RowCount = 3;
            appearancePanel.ColumnCount = 2;
            appearancePanel.Padding = new Padding(10);
            
            this.lblTheme = new Label();
            this.lblTheme.Text = "Тема:";
            this.lblTheme.Size = new Size(100, 25);
            
            this.cboTheme = new ComboBox();
            this.cboTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboTheme.Items.AddRange(new[] { "Светлая", "Темная", "Системная" });
            this.cboTheme.SelectedIndex = 0;
            this.cboTheme.Size = new Size(150, 25);
            
            this.chkAnimations = new CheckBox();
            this.chkAnimations.Text = "Анимации";
            this.chkAnimations.Checked = true;
            
            this.chkSoundEffects = new CheckBox();
            this.chkSoundEffects.Text = "Звуковые эффекты";
            
            appearancePanel.Controls.Add(this.lblTheme, 0, 0);
            appearancePanel.Controls.Add(this.cboTheme, 1, 0);
            appearancePanel.Controls.Add(this.chkAnimations, 0, 1);
            appearancePanel.Controls.Add(this.chkSoundEffects, 0, 2);
            
            this.appearanceGroup.Controls.Add(appearancePanel);
            
            // Hotkey info group
            this.hotkeyGroup = new GroupBox();
            this.hotkeyGroup.Text = "⌨️ Горячие клавиши";
            this.hotkeyGroup.Dock = DockStyle.Fill;
            this.hotkeyGroup.Font = new Font("Segoe UI", 10F);
            
            this.lblHotkeyInfo = new Label();
            this.lblHotkeyInfo.Dock = DockStyle.Fill;
            this.lblHotkeyInfo.Padding = new Padding(10);
            this.lblHotkeyInfo.Text = 
                "Расчет: Ctrl+R или Enter\n" +
                "Очистить: Ctrl+C или Esc\n" +
                "Экспорт: Ctrl+S\n" +
                "Печать: Ctrl+P\n" +
                "Переключение вкладок: Ctrl+Tab";
            
            this.hotkeyGroup.Controls.Add(this.lblHotkeyInfo);
            
            // Settings buttons
            this.settingsButtonPanel = new FlowLayoutPanel();
            this.settingsButtonPanel.Dock = DockStyle.Fill;
            this.settingsButtonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.settingsButtonPanel.Padding = new Padding(10);
            
            this.btnSaveSettings = CreateStyledButton("Сохранить", Color.FromArgb(76, 175, 80));
            this.btnApplySettings = CreateStyledButton("Применить", Color.FromArgb(33, 150, 243));
            this.btnResetSettings = CreateStyledButton("Сбросить настройки", Color.FromArgb(158, 158, 158));
            
            this.settingsButtonPanel.Controls.Add(this.btnSaveSettings);
            this.settingsButtonPanel.Controls.Add(this.btnApplySettings);
            this.settingsButtonPanel.Controls.Add(this.btnResetSettings);
            
            this.settingsPanel.Controls.Add(this.numberFormatGroup, 0, 0);
            this.settingsPanel.Controls.Add(this.appearanceGroup, 0, 1);
            this.settingsPanel.Controls.Add(this.hotkeyGroup, 0, 2);
            this.settingsPanel.Controls.Add(this.settingsButtonPanel, 0, 3);
            
            this.tabSettings.Controls.Add(this.settingsPanel);
        }
        
        private Button CreateStyledButton(string text, Color color)
        {
            var button = new Button();
            button.Text = text;
            button.Size = new Size(140, 40);
            button.Font = new Font("Segoe UI", 10F);
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Margin = new Padding(5);
            
            // Store original color in Tag for hover effect
            button.Tag = color;
            
            // Add hover effect using named handlers to prevent memory leaks
            button.MouseEnter += StyledButton_MouseEnter;
            button.MouseLeave += StyledButton_MouseLeave;
            
            return button;
        }
        
        private void StyledButton_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Color originalColor)
            {
                button.BackColor = ControlPaint.Light(originalColor, 0.1f);
            }
        }
        
        private void StyledButton_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Color originalColor)
            {
                button.BackColor = originalColor;
            }
        }
        
        // Control declarations
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabConverter;
        private System.Windows.Forms.TabPage tabHistory;
        private System.Windows.Forms.TabPage tabCalculator;
        private System.Windows.Forms.TabPage tabSettings;
        
        // Converter tab controls
        private System.Windows.Forms.TableLayoutPanel converterPanel;
        private System.Windows.Forms.Panel typePanel;
        private System.Windows.Forms.Label lblConversionType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.TableLayoutPanel conversionPanel;
        private System.Windows.Forms.GroupBox inputGroupBox;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ComboBox cboFromUnit;
        private System.Windows.Forms.Label arrowLabel;
        private System.Windows.Forms.GroupBox outputGroupBox;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ComboBox cboToUnit;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnExportPrint;
        
        
        // History tab controls
        private System.Windows.Forms.TableLayoutPanel historyPanel;
        private System.Windows.Forms.Panel historyFilterPanel;
        private System.Windows.Forms.ComboBox cboHistoryFilter;
        private System.Windows.Forms.TextBox txtHistorySearch;
        private System.Windows.Forms.Button btnHistorySearch;
        private System.Windows.Forms.DataGridView historyDataGrid;
        private System.Windows.Forms.FlowLayoutPanel historyButtonPanel;
        private System.Windows.Forms.Button btnClearHistory;
        private System.Windows.Forms.Button btnExportCSV;
        private System.Windows.Forms.Button btnExportPDF;
        
        // Calculator tab controls
        private System.Windows.Forms.TableLayoutPanel calcTabPanel;
        private System.Windows.Forms.TextBox calcTabDisplay;
        private System.Windows.Forms.FlowLayoutPanel calcModePanel;
        private System.Windows.Forms.Button btnBasicMode;
        private System.Windows.Forms.Button btnScientificMode;
        private System.Windows.Forms.Button btnProgrammerMode;
        private System.Windows.Forms.TableLayoutPanel calcTabButtonPanel;
        
        // Settings tab controls
        private System.Windows.Forms.TableLayoutPanel settingsPanel;
        private System.Windows.Forms.GroupBox numberFormatGroup;
        private System.Windows.Forms.Label lblDecimalPlaces;
        private System.Windows.Forms.NumericUpDown numDecimalPlaces;
        private System.Windows.Forms.CheckBox chkThousandsSeparator;
        private System.Windows.Forms.CheckBox chkScientificNotation;
        private System.Windows.Forms.GroupBox appearanceGroup;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cboTheme;
        private System.Windows.Forms.CheckBox chkAnimations;
        private System.Windows.Forms.CheckBox chkSoundEffects;
        private System.Windows.Forms.GroupBox hotkeyGroup;
        private System.Windows.Forms.Label lblHotkeyInfo;
        private System.Windows.Forms.FlowLayoutPanel settingsButtonPanel;
        private System.Windows.Forms.Button btnResetSettings;
        private System.Windows.Forms.Button btnApplySettings;
        private System.Windows.Forms.Button btnSaveSettings;
        
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}