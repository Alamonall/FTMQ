using System;
using System.Drawing;
using System.Windows.Forms;

namespace FTMQ
{
    partial class bdDataView : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;        

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.baseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.egeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ogeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.queryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combinedQueryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.completeQueryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToExcelTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.pieOfDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.failToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewBox = new System.Windows.Forms.RichTextBox();
            this.dataGroupBox = new System.Windows.Forms.GroupBox();
            this.dataGridView = new FTMQ.DoubleBufferedDataGridView();
            this.bdLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.dataGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.baseToolStripMenuItem,
            this.queryToolStripMenuItem,
            this.resultToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(785, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // baseToolStripMenuItem
            // 
            this.baseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.egeToolStripMenuItem1,
            this.ogeToolStripMenuItem1});
            this.baseToolStripMenuItem.Name = "baseToolStripMenuItem";
            this.baseToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.baseToolStripMenuItem.Text = "Base";
            // 
            // egeToolStripMenuItem1
            // 
            this.egeToolStripMenuItem1.Name = "egeToolStripMenuItem1";
            this.egeToolStripMenuItem1.Size = new System.Drawing.Size(96, 22);
            this.egeToolStripMenuItem1.Text = "Ege";
            this.egeToolStripMenuItem1.Click += new System.EventHandler(this.egeToolStripMenuItem1_Click);
            // 
            // ogeToolStripMenuItem1
            // 
            this.ogeToolStripMenuItem1.Name = "ogeToolStripMenuItem1";
            this.ogeToolStripMenuItem1.Size = new System.Drawing.Size(96, 22);
            this.ogeToolStripMenuItem1.Text = "Oge";
            this.ogeToolStripMenuItem1.Click += new System.EventHandler(this.ogeToolStripMenuItem1_Click);
            // 
            // queryToolStripMenuItem
            // 
            this.queryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.combinedQueryMenu,
            this.completeQueryMenu,
            this.editingToolStripMenuItem});
            this.queryToolStripMenuItem.Name = "queryToolStripMenuItem";
            this.queryToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.queryToolStripMenuItem.Text = "Query";
            // 
            // combinedQueryMenu
            // 
            this.combinedQueryMenu.Name = "combinedQueryMenu";
            this.combinedQueryMenu.Size = new System.Drawing.Size(130, 22);
            this.combinedQueryMenu.Text = "Combined";
            // 
            // completeQueryMenu
            // 
            this.completeQueryMenu.Name = "completeQueryMenu";
            this.completeQueryMenu.Size = new System.Drawing.Size(130, 22);
            this.completeQueryMenu.Text = "Complete";
            // 
            // editingToolStripMenuItem
            // 
            this.editingToolStripMenuItem.Name = "editingToolStripMenuItem";
            this.editingToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.editingToolStripMenuItem.Text = "Editing";
            this.editingToolStripMenuItem.Click += new System.EventHandler(this.editingToolStripMenuItem_Click);
            // 
            // resultToolStripMenuItem
            // 
            this.resultToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToExcelTSMI,
            this.pieOfDataToolStripMenuItem});
            this.resultToolStripMenuItem.Name = "resultToolStripMenuItem";
            this.resultToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.resultToolStripMenuItem.Text = "Result";
            // 
            // exportToExcelTSMI
            // 
            this.exportToExcelTSMI.Name = "exportToExcelTSMI";
            this.exportToExcelTSMI.Size = new System.Drawing.Size(152, 22);
            this.exportToExcelTSMI.Text = "Export to Excel";
            this.exportToExcelTSMI.Click += new System.EventHandler(this.exportToExcelTSMI_Click);
            // 
            // pieOfDataToolStripMenuItem
            // 
            this.pieOfDataToolStripMenuItem.Enabled = false;
            this.pieOfDataToolStripMenuItem.Name = "pieOfDataToolStripMenuItem";
            this.pieOfDataToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pieOfDataToolStripMenuItem.Text = "Pie of Data";
            this.pieOfDataToolStripMenuItem.Click += new System.EventHandler(this.pieOfDataToolStripMenuItem_Click);
            // 
            // failToolStripMenuItem
            // 
            this.failToolStripMenuItem.Name = "failToolStripMenuItem";
            this.failToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // previewBox
            // 
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previewBox.Enabled = false;
            this.previewBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.previewBox.Location = new System.Drawing.Point(0, 401);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(785, 60);
            this.previewBox.TabIndex = 2;
            this.previewBox.Text = "";
            // 
            // dataGroupBox
            // 
            this.dataGroupBox.Controls.Add(this.dataGridView);
            this.dataGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGroupBox.Location = new System.Drawing.Point(0, 24);
            this.dataGroupBox.Name = "dataGroupBox";
            this.dataGroupBox.Size = new System.Drawing.Size(785, 377);
            this.dataGroupBox.TabIndex = 3;
            this.dataGroupBox.TabStop = false;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(3, 0);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(1);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(779, 374);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
            this.dataGridView.Sorted += new System.EventHandler(this.dataGridView_Sorted);
            // 
            // bdLabel
            // 
            this.bdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bdLabel.AutoSize = true;
            this.bdLabel.Location = new System.Drawing.Point(740, 5);
            this.bdLabel.Name = "bdLabel";
            this.bdLabel.Size = new System.Drawing.Size(29, 13);
            this.bdLabel.TabIndex = 4;
            this.bdLabel.Text = "EGE";
            // 
            // bdDataView
            // 
            this.ClientSize = new System.Drawing.Size(785, 461);
            this.Controls.Add(this.bdLabel);
            this.Controls.Add(this.dataGroupBox);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "bdDataView";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.dataGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem failToolStripMenuItem;
        private ToolStripMenuItem baseToolStripMenuItem;
        private ToolStripMenuItem egeToolStripMenuItem1;
        private ToolStripMenuItem ogeToolStripMenuItem1;
        private ToolStripMenuItem queryToolStripMenuItem;
        private ToolStripMenuItem combinedQueryMenu;
        private ToolStripMenuItem completeQueryMenu;
        private ToolStripMenuItem editingToolStripMenuItem;
        private ToolStripMenuItem resultToolStripMenuItem;
        private ToolStripMenuItem exportToExcelTSMI;
        private RichTextBox previewBox;
        private GroupBox dataGroupBox;
        private DoubleBufferedDataGridView dataGridView;
        private Label bdLabel;
        private ToolStripMenuItem pieOfDataToolStripMenuItem;
    }
}