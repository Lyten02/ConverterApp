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
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPDFMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTXTMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPNGMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.printMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userManualMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickStartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calcHelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.formulasMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitsTableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeysMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkUpdatesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportBugMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tipsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contactsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabConverter = new System.Windows.Forms.TabPage();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.tabCalculator = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
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
            this.calcTabPanel = new System.Windows.Forms.TableLayoutPanel();
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historyDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimalPlaces)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.helpMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip.TabIndex = 0;
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importHistoryMenuItem,
            this.exportMenuItem,
            this.toolStripSeparator3,
            this.printMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileMenuItem.Text = "Файл";
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openMenuItem.Text = "📂 Открыть...";
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.Size = new System.Drawing.Size(264, 22);
            this.saveMenuItem.Text = "💾 Сохранить";
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.S)));
            this.saveAsMenuItem.Size = new System.Drawing.Size(264, 22);
            this.saveAsMenuItem.Text = "💾 Сохранить как...";
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(261, 6);
            this.importMenuItem.Name = "importMenuItem";
            this.importMenuItem.Size = new System.Drawing.Size(264, 22);
            this.importMenuItem.Text = "📥 Импорт данных...";
            this.exportMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportPDFMenuItem,
            this.exportCSVMenuItem,
            this.exportTXTMenuItem,
            this.exportPNGMenuItem});
            this.exportMenuItem.Name = "exportMenuItem";
            this.exportMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportMenuItem.Text = "📤 Экспорт результатов";
            this.exportPDFMenuItem.Name = "exportPDFMenuItem";
            this.exportPDFMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportPDFMenuItem.Text = "📄 PDF";
            this.exportCSVMenuItem.Name = "exportCSVMenuItem";
            this.exportCSVMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportCSVMenuItem.Text = "📊 CSV (Excel)";
            this.exportTXTMenuItem.Name = "exportTXTMenuItem";
            this.exportTXTMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportTXTMenuItem.Text = "📝 TXT";
            this.exportPNGMenuItem.Name = "exportPNGMenuItem";
            this.exportPNGMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportPNGMenuItem.Text = "🖼️ PNG (Скриншот)";
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(261, 6);
            this.printMenuItem.Name = "printMenuItem";
            this.printMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printMenuItem.Size = new System.Drawing.Size(264, 22);
            this.printMenuItem.Text = "🖨️ Печать...";
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(261, 6);
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exitMenuItem.Text = "❌ Выход";
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userManualMenuItem,
            this.quickStartMenuItem,
            this.calcHelpMenuItem,
            this.toolStripSeparator4,
            this.formulasMenuItem,
            this.unitsTableMenuItem,
            this.hotkeysMenuItem,
            this.toolStripSeparator5,
            this.aboutMenuItem,
            this.checkUpdatesMenuItem,
            this.reportBugMenuItem,
            this.toolStripSeparator6,
            this.tipsMenuItem,
            this.contactsMenuItem,
            this.licenseMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(65, 20);
            this.helpMenuItem.Text = "Справка";
            this.userManualMenuItem.Name = "userManualMenuItem";
            this.userManualMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.userManualMenuItem.Size = new System.Drawing.Size(258, 22);
            this.userManualMenuItem.Text = "📖 Руководство пользователя";
            this.quickStartMenuItem.Name = "quickStartMenuItem";
            this.quickStartMenuItem.Size = new System.Drawing.Size(258, 22);
            this.quickStartMenuItem.Text = "🚀 Быстрый старт";
            this.calcHelpMenuItem.Name = "calcHelpMenuItem";
            this.calcHelpMenuItem.Size = new System.Drawing.Size(258, 22);
            this.calcHelpMenuItem.Text = "🔧 Как использовать калькулятор";
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(255, 6);
            this.formulasMenuItem.Name = "formulasMenuItem";
            this.formulasMenuItem.Size = new System.Drawing.Size(258, 22);
            this.formulasMenuItem.Text = "📐 Формулы конвертации";
            this.unitsTableMenuItem.Name = "unitsTableMenuItem";
            this.unitsTableMenuItem.Size = new System.Drawing.Size(258, 22);
            this.unitsTableMenuItem.Text = "📊 Таблица единиц";
            this.hotkeysMenuItem.Name = "hotkeysMenuItem";
            this.hotkeysMenuItem.Size = new System.Drawing.Size(258, 22);
            this.hotkeysMenuItem.Text = "⌨️ Горячие клавиши";
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(255, 6);
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(258, 22);
            this.aboutMenuItem.Text = "ℹ️ О программе";
            this.checkUpdatesMenuItem.Name = "checkUpdatesMenuItem";
            this.checkUpdatesMenuItem.Size = new System.Drawing.Size(258, 22);
            this.checkUpdatesMenuItem.Text = "🔄 Проверить обновления";
            this.reportBugMenuItem.Name = "reportBugMenuItem";
            this.reportBugMenuItem.Size = new System.Drawing.Size(258, 22);
            this.reportBugMenuItem.Text = "🐛 Сообщить об ошибке";
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(255, 6);
            this.tipsMenuItem.Name = "tipsMenuItem";
            this.tipsMenuItem.Size = new System.Drawing.Size(258, 22);
            this.tipsMenuItem.Text = "💡 Советы и хитрости";
            this.contactsMenuItem.Name = "contactsMenuItem";
            this.contactsMenuItem.Size = new System.Drawing.Size(258, 22);
            this.contactsMenuItem.Text = "📞 Контакты";
            this.licenseMenuItem.Name = "licenseMenuItem";
            this.licenseMenuItem.Size = new System.Drawing.Size(258, 22);
            this.licenseMenuItem.Text = "📜 Лицензия";
            this.importHistoryMenuItem.Name = "importHistoryMenuItem";
            this.importHistoryMenuItem.Size = new System.Drawing.Size(264, 22);
            this.importHistoryMenuItem.Text = "📥 Импорт истории";
            this.mainTabControl.Controls.Add(this.tabConverter);
            this.mainTabControl.Controls.Add(this.tabCalculator);
            this.mainTabControl.Controls.Add(this.tabHistory);
            this.mainTabControl.Controls.Add(this.tabSettings);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.mainTabControl.Location = new System.Drawing.Point(0, 24);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1024, 722);
            this.mainTabControl.TabIndex = 1;
            this.tabConverter.Location = new System.Drawing.Point(4, 26);
            this.tabConverter.Name = "tabConverter";
            this.tabConverter.Padding = new System.Windows.Forms.Padding(10);
            this.tabConverter.Size = new System.Drawing.Size(1016, 692);
            this.tabConverter.TabIndex = 0;
            this.tabConverter.Text = "🔄 Конвертатор";
            this.tabConverter.UseVisualStyleBackColor = true;
            this.tabHistory.Location = new System.Drawing.Point(4, 26);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(10);
            this.tabHistory.Size = new System.Drawing.Size(1016, 692);
            this.tabHistory.TabIndex = 1;
            this.tabHistory.Text = "📊 История";
            this.tabHistory.UseVisualStyleBackColor = true;
            this.tabCalculator.Location = new System.Drawing.Point(4, 26);
            this.tabCalculator.Name = "tabCalculator";
            this.tabCalculator.Padding = new System.Windows.Forms.Padding(10);
            this.tabCalculator.Size = new System.Drawing.Size(1016, 692);
            this.tabCalculator.TabIndex = 2;
            this.tabCalculator.Text = "🧮 Калькулятор";
            this.tabCalculator.UseVisualStyleBackColor = true;
            this.tabSettings.Location = new System.Drawing.Point(4, 26);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(10);
            this.tabSettings.Size = new System.Drawing.Size(1016, 692);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "⚙️ Настройки";
            this.tabSettings.UseVisualStyleBackColor = true;
            this.converterPanel.Location = new System.Drawing.Point(0, 0);
            this.converterPanel.Name = "converterPanel";
            this.converterPanel.Size = new System.Drawing.Size(200, 100);
            this.converterPanel.TabIndex = 0;
            this.typePanel.Location = new System.Drawing.Point(0, 0);
            this.typePanel.Name = "typePanel";
            this.typePanel.Size = new System.Drawing.Size(200, 100);
            this.typePanel.TabIndex = 0;
            this.lblConversionType.Location = new System.Drawing.Point(0, 0);
            this.lblConversionType.Name = "lblConversionType";
            this.lblConversionType.Size = new System.Drawing.Size(100, 23);
            this.lblConversionType.TabIndex = 0;
            this.cboType.Location = new System.Drawing.Point(0, 0);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(121, 21);
            this.cboType.TabIndex = 0;
            this.conversionPanel.Location = new System.Drawing.Point(0, 0);
            this.conversionPanel.Name = "conversionPanel";
            this.conversionPanel.Size = new System.Drawing.Size(200, 100);
            this.conversionPanel.TabIndex = 0;
            this.inputGroupBox.Location = new System.Drawing.Point(0, 0);
            this.inputGroupBox.Name = "inputGroupBox";
            this.inputGroupBox.Size = new System.Drawing.Size(200, 100);
            this.inputGroupBox.TabIndex = 0;
            this.inputGroupBox.TabStop = false;
            this.arrowLabel.Location = new System.Drawing.Point(0, 0);
            this.arrowLabel.Name = "arrowLabel";
            this.arrowLabel.Size = new System.Drawing.Size(100, 23);
            this.arrowLabel.TabIndex = 0;
            this.outputGroupBox.Location = new System.Drawing.Point(0, 0);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(200, 100);
            this.outputGroupBox.TabIndex = 0;
            this.outputGroupBox.TabStop = false;
            this.buttonPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(200, 100);
            this.buttonPanel.TabIndex = 0;
            this.btnConvert.Location = new System.Drawing.Point(0, 0);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 0;
            this.btnClear.Location = new System.Drawing.Point(0, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 0;
            this.btnExport.Location = new System.Drawing.Point(0, 0);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 0;
            this.btnExportPrint.Location = new System.Drawing.Point(0, 0);
            this.btnExportPrint.Name = "btnExportPrint";
            this.btnExportPrint.Size = new System.Drawing.Size(75, 23);
            this.btnExportPrint.TabIndex = 0;
            this.historyPanel.Location = new System.Drawing.Point(0, 0);
            this.historyPanel.Name = "historyPanel";
            this.historyPanel.Size = new System.Drawing.Size(200, 100);
            this.historyPanel.TabIndex = 0;
            this.historyFilterPanel.Location = new System.Drawing.Point(0, 0);
            this.historyFilterPanel.Name = "historyFilterPanel";
            this.historyFilterPanel.Size = new System.Drawing.Size(200, 100);
            this.historyFilterPanel.TabIndex = 0;
            this.cboHistoryFilter.Location = new System.Drawing.Point(0, 0);
            this.cboHistoryFilter.Name = "cboHistoryFilter";
            this.cboHistoryFilter.Size = new System.Drawing.Size(121, 21);
            this.cboHistoryFilter.TabIndex = 0;
            this.txtHistorySearch.Location = new System.Drawing.Point(0, 0);
            this.txtHistorySearch.Name = "txtHistorySearch";
            this.txtHistorySearch.Size = new System.Drawing.Size(100, 20);
            this.txtHistorySearch.TabIndex = 0;
            this.btnHistorySearch.Location = new System.Drawing.Point(0, 0);
            this.btnHistorySearch.Name = "btnHistorySearch";
            this.btnHistorySearch.Size = new System.Drawing.Size(75, 23);
            this.btnHistorySearch.TabIndex = 0;
            this.historyDataGrid.Location = new System.Drawing.Point(0, 0);
            this.historyDataGrid.Name = "historyDataGrid";
            this.historyDataGrid.Size = new System.Drawing.Size(240, 150);
            this.historyDataGrid.TabIndex = 0;
            this.historyButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.historyButtonPanel.Name = "historyButtonPanel";
            this.historyButtonPanel.Size = new System.Drawing.Size(200, 100);
            this.historyButtonPanel.TabIndex = 0;
            this.btnClearHistory.Location = new System.Drawing.Point(0, 0);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(75, 23);
            this.btnClearHistory.TabIndex = 0;
            this.btnExportCSV.Location = new System.Drawing.Point(0, 0);
            this.btnExportCSV.Name = "btnExportCSV";
            this.btnExportCSV.Size = new System.Drawing.Size(75, 23);
            this.btnExportCSV.TabIndex = 0;
            this.btnExportPDF.Location = new System.Drawing.Point(0, 0);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(75, 23);
            this.btnExportPDF.TabIndex = 0;
            this.calcTabPanel.Location = new System.Drawing.Point(0, 0);
            this.calcTabPanel.Name = "calcTabPanel";
            this.calcTabPanel.Size = new System.Drawing.Size(200, 100);
            this.calcTabPanel.TabIndex = 0;
            this.settingsPanel.Location = new System.Drawing.Point(0, 0);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(200, 100);
            this.settingsPanel.TabIndex = 0;
            this.numberFormatGroup.Location = new System.Drawing.Point(0, 0);
            this.numberFormatGroup.Name = "numberFormatGroup";
            this.numberFormatGroup.Size = new System.Drawing.Size(200, 100);
            this.numberFormatGroup.TabIndex = 0;
            this.numberFormatGroup.TabStop = false;
            this.lblDecimalPlaces.Location = new System.Drawing.Point(0, 0);
            this.lblDecimalPlaces.Name = "lblDecimalPlaces";
            this.lblDecimalPlaces.Size = new System.Drawing.Size(100, 23);
            this.lblDecimalPlaces.TabIndex = 0;
            this.numDecimalPlaces.Location = new System.Drawing.Point(0, 0);
            this.numDecimalPlaces.Name = "numDecimalPlaces";
            this.numDecimalPlaces.Size = new System.Drawing.Size(120, 20);
            this.numDecimalPlaces.TabIndex = 0;
            this.chkThousandsSeparator.Location = new System.Drawing.Point(0, 0);
            this.chkThousandsSeparator.Name = "chkThousandsSeparator";
            this.chkThousandsSeparator.Size = new System.Drawing.Size(104, 24);
            this.chkThousandsSeparator.TabIndex = 0;
            this.chkScientificNotation.Location = new System.Drawing.Point(0, 0);
            this.chkScientificNotation.Name = "chkScientificNotation";
            this.chkScientificNotation.Size = new System.Drawing.Size(104, 24);
            this.chkScientificNotation.TabIndex = 0;
            this.appearanceGroup.Location = new System.Drawing.Point(0, 0);
            this.appearanceGroup.Name = "appearanceGroup";
            this.appearanceGroup.Size = new System.Drawing.Size(200, 100);
            this.appearanceGroup.TabIndex = 0;
            this.appearanceGroup.TabStop = false;
            this.lblTheme.Location = new System.Drawing.Point(0, 0);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(100, 23);
            this.lblTheme.TabIndex = 0;
            this.cboTheme.Location = new System.Drawing.Point(0, 0);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(121, 21);
            this.cboTheme.TabIndex = 0;
            this.chkAnimations.Location = new System.Drawing.Point(0, 0);
            this.chkAnimations.Name = "chkAnimations";
            this.chkAnimations.Size = new System.Drawing.Size(104, 24);
            this.chkAnimations.TabIndex = 0;
            this.chkSoundEffects.Location = new System.Drawing.Point(0, 0);
            this.chkSoundEffects.Name = "chkSoundEffects";
            this.chkSoundEffects.Size = new System.Drawing.Size(104, 24);
            this.chkSoundEffects.TabIndex = 0;
            this.hotkeyGroup.Location = new System.Drawing.Point(0, 0);
            this.hotkeyGroup.Name = "hotkeyGroup";
            this.hotkeyGroup.Size = new System.Drawing.Size(200, 100);
            this.hotkeyGroup.TabIndex = 0;
            this.hotkeyGroup.TabStop = false;
            this.lblHotkeyInfo.Location = new System.Drawing.Point(0, 0);
            this.lblHotkeyInfo.Name = "lblHotkeyInfo";
            this.lblHotkeyInfo.Size = new System.Drawing.Size(100, 23);
            this.lblHotkeyInfo.TabIndex = 0;
            this.settingsButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.settingsButtonPanel.Name = "settingsButtonPanel";
            this.settingsButtonPanel.Size = new System.Drawing.Size(200, 100);
            this.settingsButtonPanel.TabIndex = 0;
            this.btnResetSettings.Location = new System.Drawing.Point(0, 0);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(75, 23);
            this.btnResetSettings.TabIndex = 0;
            this.btnApplySettings.Location = new System.Drawing.Point(0, 0);
            this.btnApplySettings.Name = "btnApplySettings";
            this.btnApplySettings.Size = new System.Drawing.Size(75, 23);
            this.btnApplySettings.TabIndex = 0;
            this.btnSaveSettings.Location = new System.Drawing.Point(0, 0);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSettings.TabIndex = 0;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 746);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip.TabIndex = 2;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(88, 17);
            this.lblStatus.Text = "Готов к работе";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "ModernMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ConverterApp - Универсальный конвертер © 2025 Чумаченко Даниэль";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.historyDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimalPlaces)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private void InitializeConverterTab()
        {
            this.converterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.converterPanel.Dock = DockStyle.Fill;
            this.converterPanel.ColumnCount = 1;
            this.converterPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.converterPanel.RowCount = 1;
            this.converterPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            var leftPanel = new TableLayoutPanel();
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.RowCount = 4;
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            leftPanel.Padding = new Padding(50, 20, 50, 20);
            this.typePanel = new Panel();
            this.typePanel.Dock = DockStyle.Fill;
            var centerContainer = new TableLayoutPanel();
            centerContainer.Dock = DockStyle.Fill;
            centerContainer.RowCount = 3;
            centerContainer.ColumnCount = 3;
            centerContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            centerContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            centerContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            centerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            centerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450F));
            centerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            var innerPanel = new Panel();
            innerPanel.Size = new Size(450, 70);
            this.lblConversionType = new Label();
            this.lblConversionType.Text = "Выберите тип конвертации:";
            this.lblConversionType.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            this.lblConversionType.Location = new Point(0, 0);
            this.lblConversionType.Size = new Size(450, 30);
            this.lblConversionType.TextAlign = ContentAlignment.MiddleCenter;
            this.cboType = new ComboBox();
            this.cboType.Font = new Font("Segoe UI", 12F);
            this.cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboType.Location = new Point(25, 35);
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
            innerPanel.Controls.Add(this.lblConversionType);
            innerPanel.Controls.Add(this.cboType);
            centerContainer.Controls.Add(innerPanel, 1, 1);
            this.typePanel.Controls.Add(centerContainer);
            this.conversionPanel = new TableLayoutPanel();
            this.conversionPanel.Dock = DockStyle.Fill;
            this.conversionPanel.ColumnCount = 3;
            this.conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            this.conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            this.conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            this.conversionPanel.Padding = new Padding(30);
            this.inputGroupBox = new GroupBox();
            this.inputGroupBox.Text = "Ввод";
            this.inputGroupBox.Dock = DockStyle.Fill;
            this.inputGroupBox.Font = new Font("Segoe UI", 10F);
            this.inputGroupBox.Padding = new Padding(10);
            var inputPanel = new TableLayoutPanel();
            inputPanel.Dock = DockStyle.Fill;
            inputPanel.RowCount = 2;
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.txtInput = new TextBox();
            this.txtInput.Dock = DockStyle.Fill;
            this.txtInput.Font = new Font("Segoe UI", 18F);
            this.txtInput.TextAlign = HorizontalAlignment.Center;
            this.txtInput.Height = 50;
            this.cboFromUnit = new ComboBox();
            this.cboFromUnit.Dock = DockStyle.Fill;
            this.cboFromUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFromUnit.Font = new Font("Segoe UI", 12F);
            this.cboFromUnit.DropDownHeight = 200;
            inputPanel.Controls.Add(this.txtInput, 0, 0);
            inputPanel.Controls.Add(this.cboFromUnit, 0, 1);
            this.inputGroupBox.Controls.Add(inputPanel);
            this.arrowLabel = new Label();
            this.arrowLabel.Text = "➡️";
            this.arrowLabel.Font = new Font("Segoe UI", 36F);
            this.arrowLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.arrowLabel.Dock = DockStyle.Fill;
            this.outputGroupBox = new GroupBox();
            this.outputGroupBox.Text = "Результат";
            this.outputGroupBox.Dock = DockStyle.Fill;
            this.outputGroupBox.Font = new Font("Segoe UI", 10F);
            this.outputGroupBox.Padding = new Padding(10);
            var outputPanel = new TableLayoutPanel();
            outputPanel.Dock = DockStyle.Fill;
            outputPanel.RowCount = 2;
            outputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            outputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
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
            this.cboToUnit.Font = new Font("Segoe UI", 12F);
            this.cboToUnit.DropDownHeight = 200;
            outputPanel.Controls.Add(this.txtOutput, 0, 0);
            outputPanel.Controls.Add(this.cboToUnit, 0, 1);
            this.outputGroupBox.Controls.Add(outputPanel);
            this.conversionPanel.Controls.Add(this.inputGroupBox, 0, 0);
            this.conversionPanel.Controls.Add(this.arrowLabel, 1, 0);
            this.conversionPanel.Controls.Add(this.outputGroupBox, 2, 0);
            var buttonContainer = new TableLayoutPanel();
            buttonContainer.Dock = DockStyle.Fill;
            buttonContainer.RowCount = 1;
            buttonContainer.ColumnCount = 3;
            buttonContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 650F));
            buttonContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.buttonPanel = new FlowLayoutPanel();
            this.buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.buttonPanel.Anchor = AnchorStyles.None;
            this.btnConvert = CreateStyledButton("Рассчитать", Color.FromArgb(0, 123, 255)); 
            this.btnClear = CreateStyledButton("Очистить", Color.FromArgb(108, 117, 125)); 
            this.btnExport = CreateStyledButton("Экспорт", Color.FromArgb(40, 167, 69)); 
            this.btnExportPrint = CreateStyledButton("Печать", Color.FromArgb(255, 193, 7)); 
            this.buttonPanel.Controls.Add(this.btnConvert);
            this.buttonPanel.Controls.Add(this.btnClear);
            this.buttonPanel.Controls.Add(this.btnExport);
            this.buttonPanel.Controls.Add(this.btnExportPrint);
            buttonContainer.Controls.Add(this.buttonPanel, 1, 0);
            leftPanel.Controls.Add(this.typePanel, 0, 0);
            leftPanel.Controls.Add(this.conversionPanel, 0, 1);
            leftPanel.Controls.Add(buttonContainer, 0, 3);
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
            this.btnHistorySearch.Size = new Size(40, 25);
            this.btnHistorySearch.Font = new Font("Segoe UI", 10F);
            this.btnHistorySearch.FlatStyle = FlatStyle.Flat;
            this.btnHistorySearch.BackColor = Color.FromArgb(0, 123, 255); 
            this.btnHistorySearch.ForeColor = Color.White;
            this.btnHistorySearch.FlatAppearance.BorderSize = 1;
            this.btnHistorySearch.FlatAppearance.BorderColor = ControlPaint.Dark(Color.FromArgb(0, 123, 255), 0.1f);
            this.btnHistorySearch.Cursor = Cursors.Hand;
            this.btnHistorySearch.Tag = Color.FromArgb(0, 123, 255);
            this.btnHistorySearch.MouseEnter += StyledButton_MouseEnter;
            this.btnHistorySearch.MouseLeave += StyledButton_MouseLeave;
            this.historyFilterPanel.Controls.Add(filterLabel);
            this.historyFilterPanel.Controls.Add(this.cboHistoryFilter);
            this.historyFilterPanel.Controls.Add(searchLabel);
            this.historyFilterPanel.Controls.Add(this.txtHistorySearch);
            this.historyFilterPanel.Controls.Add(this.btnHistorySearch);
            this.historyDataGrid = new DataGridView();
            this.historyDataGrid.Dock = DockStyle.Fill;
            this.historyDataGrid.AllowUserToAddRows = false;
            this.historyDataGrid.AllowUserToDeleteRows = false;
            this.historyDataGrid.ReadOnly = true;
            this.historyDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.historyDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.historyDataGrid.Columns.Add("DateTime", "Дата/Время");
            this.historyDataGrid.Columns.Add("Operation", "Операция");
            this.historyDataGrid.Columns.Add("Result", "Результат");
            this.historyButtonPanel = new FlowLayoutPanel();
            this.historyButtonPanel.Dock = DockStyle.Fill;
            this.historyButtonPanel.FlowDirection = FlowDirection.LeftToRight;
            this.historyButtonPanel.Padding = new Padding(10);
            this.btnClearHistory = CreateStyledButton("Очистить историю", Color.FromArgb(220, 53, 69)); 
            this.btnExportCSV = CreateStyledButton("Экспорт в CSV", Color.FromArgb(40, 167, 69)); 
            this.btnExportPDF = CreateStyledButton("Экспорт в PDF", Color.FromArgb(23, 162, 184)); 
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
            this.calcTabPanel.RowCount = 2;
            this.calcTabPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            this.calcTabPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.calcTabDisplay = new TextBox();
            this.calcTabDisplay.Dock = DockStyle.Fill;
            this.calcTabDisplay.Font = new Font("Segoe UI", 24F);
            this.calcTabDisplay.TextAlign = HorizontalAlignment.Right;
            this.calcTabDisplay.ReadOnly = true;
            this.calcTabDisplay.BackColor = Color.White;
            this.calcTabDisplay.Text = "0";
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
            this.calcTabPanel.Controls.Add(this.calcTabDisplay, 0, 0);
            this.calcTabPanel.Controls.Add(this.calcTabButtonPanel, 0, 1);
            this.tabCalculator.Controls.Add(this.calcTabPanel);
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
            this.appearanceGroup = new GroupBox();
            this.appearanceGroup.Text = "🎨 Внешний вид";
            this.appearanceGroup.Dock = DockStyle.Fill;
            this.appearanceGroup.Font = new Font("Segoe UI", 10F);
            var appearancePanel = new TableLayoutPanel();
            appearancePanel.Dock = DockStyle.Fill;
            appearancePanel.RowCount = 3;
            appearancePanel.ColumnCount = 2;
            appearancePanel.Padding = new Padding(10);
            this.chkAnimations = new CheckBox();
            this.chkAnimations.Text = "Анимации";
            this.chkAnimations.Checked = true;
            this.chkSoundEffects = new CheckBox();
            this.chkSoundEffects.Text = "Звуковые эффекты";
            
            appearancePanel.Controls.Add(this.chkAnimations, 0, 0);
            appearancePanel.Controls.Add(this.chkSoundEffects, 0, 1);
            this.appearanceGroup.Controls.Add(appearancePanel);
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
            this.settingsButtonPanel = new FlowLayoutPanel();
            this.settingsButtonPanel.Dock = DockStyle.Fill;
            this.settingsButtonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.settingsButtonPanel.Padding = new Padding(10);
            this.btnSaveSettings = CreateStyledButton("Сохранить", Color.FromArgb(76, 175, 80));
            this.btnApplySettings = CreateStyledButton("Применить", Color.FromArgb(40, 167, 69)); 
            this.btnResetSettings = CreateStyledButton("Сбросить настройки", Color.FromArgb(108, 117, 125)); 
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
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = ControlPaint.Dark(color, 0.1f);
            button.Cursor = Cursors.Hand;
            button.Margin = new Padding(5);
            button.Tag = color;
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
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPDFMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCSVMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportTXTMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPNGMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem printMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userManualMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickStartMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calcHelpMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem formulasMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unitsTableMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hotkeysMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkUpdatesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportBugMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem tipsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contactsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importHistoryMenuItem;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabConverter;
        private System.Windows.Forms.TabPage tabHistory;
        private System.Windows.Forms.TabPage tabCalculator;
        private System.Windows.Forms.TabPage tabSettings;
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
        private System.Windows.Forms.TableLayoutPanel calcTabPanel;
        private System.Windows.Forms.TextBox calcTabDisplay;
        private System.Windows.Forms.TableLayoutPanel calcTabButtonPanel;
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
