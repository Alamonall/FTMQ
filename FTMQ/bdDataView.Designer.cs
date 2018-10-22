using System.Drawing;
using System.Windows.Forms;

namespace FTMQ
{
    partial class bdDataView
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.failMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.combinedQuerysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.egeQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.giaQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlCommandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.giaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.egeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewWindow = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataGridView1.Location = new System.Drawing.Point(0, 24);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1047, 454);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEnter);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.failMenu,
            this.sqlCommandsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1047, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // failMenu
            // 
            this.failMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.combinedQuerysToolStripMenuItem});
            this.failMenu.Name = "failMenu";
            this.failMenu.Size = new System.Drawing.Size(48, 20);
            this.failMenu.Text = "Файл";
            // 
            // combinedQuerysToolStripMenuItem
            // 
            this.combinedQuerysToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.egeQueryToolStripMenuItem,
            this.giaQueryToolStripMenuItem});
            this.combinedQuerysToolStripMenuItem.Name = "combinedQuerysToolStripMenuItem";
            this.combinedQuerysToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.combinedQuerysToolStripMenuItem.Text = "Комбинированные запросы";
            // 
            // egeQueryToolStripMenuItem
            // 
            this.egeQueryToolStripMenuItem.Name = "egeQueryToolStripMenuItem";
            this.egeQueryToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.egeQueryToolStripMenuItem.Text = "Запросы ЕГЭ";
            this.egeQueryToolStripMenuItem.Click += new System.EventHandler(this.egeQueryToolStripMenuItem_Click);
            // 
            // giaQueryToolStripMenuItem
            // 
            this.giaQueryToolStripMenuItem.Name = "giaQueryToolStripMenuItem";
            this.giaQueryToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.giaQueryToolStripMenuItem.Text = "Запросы ГИА";
            this.giaQueryToolStripMenuItem.Click += new System.EventHandler(this.giaQueryToolStripMenuItem_Click);
            // 
            // sqlCommandsToolStripMenuItem
            // 
            this.sqlCommandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.giaToolStripMenuItem,
            this.egeToolStripMenuItem,
            this.editQueryToolStripMenuItem});
            this.sqlCommandsToolStripMenuItem.Name = "sqlCommandsToolStripMenuItem";
            this.sqlCommandsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.sqlCommandsToolStripMenuItem.Text = "Запросы";
            // 
            // giaToolStripMenuItem
            // 
            this.giaToolStripMenuItem.Name = "giaToolStripMenuItem";
            this.giaToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.giaToolStripMenuItem.Text = "ГИА";
            // 
            // egeToolStripMenuItem
            // 
            this.egeToolStripMenuItem.Name = "egeToolStripMenuItem";
            this.egeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.egeToolStripMenuItem.Text = "EGE";
            // 
            // editQueryToolStripMenuItem
            // 
            this.editQueryToolStripMenuItem.Name = "editQueryToolStripMenuItem";
            this.editQueryToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.editQueryToolStripMenuItem.Text = "Отредактировать запрос";
            this.editQueryToolStripMenuItem.Click += new System.EventHandler(this.editQueryToolStripMenuItemToolStripMenuItem_Click);
            // 
            // previewWindow
            // 
            this.previewWindow.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previewWindow.Font = new System.Drawing.Font("Arial", 12F);
            this.previewWindow.HideSelection = false;
            this.previewWindow.Location = new System.Drawing.Point(0, 487);
            this.previewWindow.Name = "previewWindow";
            this.previewWindow.ReadOnly = true;
            this.previewWindow.Size = new System.Drawing.Size(1047, 56);
            this.previewWindow.TabIndex = 3;
            this.previewWindow.Text = "";
            // 
            // bdDataView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1047, 543);
            this.Controls.Add(this.previewWindow);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1063, 582);
            this.Name = "bdDataView";
            this.Text = "Табличка";
            this.Load += new System.EventHandler(this.BdDataView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;        
        private ToolStripMenuItem sqlCommandsToolStripMenuItem;
        private ToolStripMenuItem giaToolStripMenuItem;
        private ToolStripMenuItem egeToolStripMenuItem;
        private ToolStripMenuItem failMenu;
        private ToolStripMenuItem editQueryToolStripMenuItem;
        private RichTextBox previewWindow;
        private ToolStripMenuItem combinedQuerysToolStripMenuItem;
        private ToolStripMenuItem egeQueryToolStripMenuItem;
        private ToolStripMenuItem giaQueryToolStripMenuItem;
    }
}