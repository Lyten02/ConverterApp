using System.Drawing;
using System.Windows.Forms;
using System;

namespace ConverterApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculatorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pinCalculatorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.inputPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblInput = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.cboFromUnit = new System.Windows.Forms.ComboBox();
            this.outputPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.cboToUnit = new System.Windows.Forms.ComboBox();
            this.buttonPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCalculator = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.outputPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.calculatorMenuItem,
            this.helpMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(900, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem,
            this.saveMenuItem,
            this.printMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileMenuItem.Text = "Файл";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openMenuItem.Text = "Открыть";
            this.openMenuItem.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveMenuItem.Text = "Сохранить";
            this.saveMenuItem.Click += new System.EventHandler(this.SaveFile_Click);
            // 
            // printMenuItem
            // 
            this.printMenuItem.Name = "printMenuItem";
            this.printMenuItem.Size = new System.Drawing.Size(180, 22);
            this.printMenuItem.Text = "Печать";
            this.printMenuItem.Click += new System.EventHandler(this.PrintResults_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitMenuItem.Text = "Выход";
            this.exitMenuItem.Click += new System.EventHandler(this.Exit_Click);
            // 
            // calculatorMenuItem
            // 
            this.calculatorMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pinCalculatorMenuItem});
            this.calculatorMenuItem.Name = "calculatorMenuItem";
            this.calculatorMenuItem.Size = new System.Drawing.Size(90, 20);
            this.calculatorMenuItem.Text = "Калькулятор";
            this.calculatorMenuItem.Click += new System.EventHandler(this.Calculator_Click);
            // 
            // pinCalculatorMenuItem
            // 
            this.pinCalculatorMenuItem.Name = "pinCalculatorMenuItem";
            this.pinCalculatorMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pinCalculatorMenuItem.Text = "Закрепить калькулятор";
            this.pinCalculatorMenuItem.Click += new System.EventHandler(this.PinCalculator_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpItem,
            this.historyMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(65, 20);
            this.helpMenuItem.Text = "Справка";
            // 
            // helpItem
            // 
            this.helpItem.Name = "helpItem";
            this.helpItem.Size = new System.Drawing.Size(180, 22);
            this.helpItem.Text = "Помощь";
            this.helpItem.Click += new System.EventHandler(this.Help_Click);
            // 
            // historyMenuItem
            // 
            this.historyMenuItem.Name = "historyMenuItem";
            this.historyMenuItem.Size = new System.Drawing.Size(180, 22);
            this.historyMenuItem.Text = "История";
            this.historyMenuItem.Click += new System.EventHandler(this.History_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.tableLayoutPanel);
            this.mainPanel.Controls.Add(this.buttonPanel);
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.Location = new Point(0, 24);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new Padding(10);
            this.mainPanel.Size = new Size(900, 554);
            this.mainPanel.TabIndex = 1;
            this.mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.cboType, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.inputPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.outputPanel, 1, 1);
            this.tableLayoutPanel.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Location = new Point(10, 10);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new Size(880, 464);
            this.tableLayoutPanel.TabIndex = 0;
            this.tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // cboType
            // 
            this.cboType.Dock = DockStyle.Fill;
            this.cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "Длина",
            "Масса",
            "Объем",
            "Температура",
            "Скорость",
            "Площадь",
            "Время",
            "Энергия",
            "Мощность",
            "Давление"});
            this.tableLayoutPanel.SetColumnSpan(this.cboType, 2);
            this.cboType.Location = new Point(3, 3);
            this.cboType.Name = "cboType";
            this.cboType.Size = new Size(874, 23);
            this.cboType.TabIndex = 0;
            this.cboType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // inputPanel
            // 
            this.inputPanel.ColumnCount = 2;
            this.inputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.inputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.inputPanel.Controls.Add(this.lblInput, 0, 0);
            this.inputPanel.Controls.Add(this.txtInput, 0, 1);
            this.inputPanel.Controls.Add(this.cboFromUnit, 1, 1);
            this.inputPanel.Dock = DockStyle.Fill;
            this.inputPanel.Location = new Point(3, 43);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.RowCount = 2;
            this.inputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.inputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.inputPanel.Size = new Size(434, 418);
            this.inputPanel.TabIndex = 1;
            this.inputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Dock = DockStyle.Fill;
            this.lblInput.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblInput.Location = new System.Drawing.Point(3, 0);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(211, 30);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Ввод:";
            this.lblInput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblInput.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // 
            // txtInput
            // 
            this.txtInput.Dock = DockStyle.Fill;
            this.inputPanel.SetColumnSpan(this.txtInput, 2);
            this.txtInput.Location = new System.Drawing.Point(3, 33);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(428, 23);
            this.txtInput.TabIndex = 2;
            this.txtInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // cboFromUnit
            // 
            this.cboFromUnit.Dock = DockStyle.Fill;
            this.cboFromUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFromUnit.FormattingEnabled = true;
            this.cboFromUnit.Location = new System.Drawing.Point(218, 33);
            this.cboFromUnit.Name = "cboFromUnit";
            this.cboFromUnit.Size = new System.Drawing.Size(213, 23);
            this.cboFromUnit.TabIndex = 3;
            this.cboFromUnit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            // 
            // outputPanel
            // 
            this.outputPanel.ColumnCount = 2;
            this.outputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outputPanel.Controls.Add(this.lblOutput, 0, 0);
            this.outputPanel.Controls.Add(this.txtOutput, 0, 1);
            this.outputPanel.Controls.Add(this.cboToUnit, 1, 1);
            this.outputPanel.Dock = DockStyle.Fill;
            this.outputPanel.Location = new System.Drawing.Point(443, 43);
            this.outputPanel.Name = "outputPanel";
            this.outputPanel.RowCount = 2;
            this.outputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.outputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputPanel.Size = new System.Drawing.Size(434, 418);
            this.outputPanel.TabIndex = 2;
            this.outputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Dock = DockStyle.Fill;
            this.lblOutput.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblOutput.Location = new System.Drawing.Point(3, 0);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(211, 30);
            this.lblOutput.TabIndex = 0;
            this.lblOutput.Text = "Вывод:";
            this.lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = DockStyle.Fill;
            this.outputPanel.SetColumnSpan(this.txtOutput, 2);
            this.txtOutput.Location = new System.Drawing.Point(3, 33);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(428, 23);
            this.txtOutput.TabIndex = 1;
            this.txtOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // cboToUnit
            // 
            this.cboToUnit.Dock = DockStyle.Fill;
            this.cboToUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboToUnit.FormattingEnabled = true;
            this.cboToUnit.Location = new System.Drawing.Point(218, 33);
            this.cboToUnit.Name = "cboToUnit";
            this.cboToUnit.Size = new System.Drawing.Size(213, 23);
            this.cboToUnit.TabIndex = 2;
            this.cboToUnit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            // 
            // buttonPanel
            // 
            this.buttonPanel.ColumnCount = 5;
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.buttonPanel.Controls.Add(this.btnConvert, 1, 0);
            this.buttonPanel.Controls.Add(this.btnClear, 0, 0);
            this.buttonPanel.Controls.Add(this.btnSave, 2, 0);
            this.buttonPanel.Controls.Add(this.btnPrint, 3, 0);
            this.buttonPanel.Controls.Add(this.btnCalculator, 4, 0);
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(10, 474);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.RowCount = 1;
            this.buttonPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonPanel.Size = new System.Drawing.Size(880, 70);
            this.buttonPanel.TabIndex = 1;
            this.buttonPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // btnConvert
            // 
            this.btnConvert.Dock = DockStyle.Fill;
            this.btnConvert.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnConvert.Location = new System.Drawing.Point(177, 3);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(170, 64);
            this.btnConvert.TabIndex = 2;
            this.btnConvert.Text = "🔄 Конвертировать";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // btnClear
            // 
            this.btnClear.Dock = DockStyle.Fill;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClear.Location = new System.Drawing.Point(3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(168, 64);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "🗑️ Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Dock = DockStyle.Fill;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnSave.Location = new System.Drawing.Point(353, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(170, 64);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "💾 Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.SaveFile_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Dock = DockStyle.Fill;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnPrint.Location = new System.Drawing.Point(529, 3);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(170, 64);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "🖨️ Печать";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.PrintResults_Click);
            // 
            // btnCalculator
            // 
            this.btnCalculator.Dock = DockStyle.Fill;
            this.btnCalculator.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCalculator.Location = new System.Drawing.Point(705, 3);
            this.btnCalculator.Name = "btnCalculator";
            this.btnCalculator.Size = new System.Drawing.Size(172, 64);
            this.btnCalculator.TabIndex = 5;
            this.btnCalculator.Text = "Калькулятор";
            this.btnCalculator.UseVisualStyleBackColor = true;
            this.btnCalculator.Click += new System.EventHandler(this.Calculator_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 578);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(103, 17);
            this.lblStatus.Text = "Готов к работе";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "ConverterApp";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            this.outputPanel.ResumeLayout(false);
            this.outputPanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int padding = 10;
            int buttonHeight = 70;
            int menuHeight = menuStrip.Height;
            int statusHeight = statusStrip.Height;

            tableLayoutPanel.Size = new Size(mainPanel.ClientSize.Width - 2 * padding, mainPanel.ClientSize.Height - buttonHeight - 2 * padding);
            buttonPanel.Location = new Point(padding, mainPanel.ClientSize.Height - buttonHeight - padding);
            buttonPanel.Size = new Size(mainPanel.ClientSize.Width - 2 * padding, buttonHeight);
            this.ClientSize = new Size(this.ClientSize.Width, menuHeight + mainPanel.ClientSize.Height + statusHeight);
        }

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculatorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pinCalculatorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpItem;
        private System.Windows.Forms.ToolStripMenuItem historyMenuItem;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.TableLayoutPanel inputPanel;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ComboBox cboFromUnit;
        private System.Windows.Forms.TableLayoutPanel outputPanel;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ComboBox cboToUnit;
        private System.Windows.Forms.TableLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnCalculator;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}