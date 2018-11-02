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
            this.SuspendLayout();
            // 
            // bdDataView
            // 
            this.ClientSize = new System.Drawing.Size(844, 495);
            this.DoubleBuffered = true;
            this.Name = "bdDataView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

      
        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;        
        private ToolStripMenuItem sqlCommandsToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editQueryToolStripMenuItem;
        private RichTextBox previewWindow;
        private ToolStripMenuItem bdSelectionToolStripMenuItem;
        private ToolStripMenuItem egeBDToolStripMenuItem;
        private ToolStripMenuItem ogeBDToolStripMenuItem;
        private ToolStripMenuItem combineQueryToolStripMenuItem;
        private ToolStripMenuItem completeQuerysToolStripMenuItem;
        private GroupBox contForData;
        private DoubleBufferedDataGridView dataGridView1;
    }
}