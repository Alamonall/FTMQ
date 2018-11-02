namespace FTMQ
{
    partial class combinedQueryForm
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
            this.listQuerysCheckBox = new System.Windows.Forms.CheckedListBox();
            this.buttonsGroup = new System.Windows.Forms.GroupBox();
            this.execButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.buttonsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // listQuerysCheckBox
            // 
            this.listQuerysCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listQuerysCheckBox.FormattingEnabled = true;
            this.listQuerysCheckBox.Location = new System.Drawing.Point(0, 0);
            this.listQuerysCheckBox.Name = "listQuerysCheckBox";
            this.listQuerysCheckBox.Size = new System.Drawing.Size(702, 450);
            this.listQuerysCheckBox.TabIndex = 0;
            this.listQuerysCheckBox.SelectedIndexChanged += new System.EventHandler(this.listQuerysCheckBox_SelectedIndexChanged);
            // 
            // buttonsGroup
            // 
            this.buttonsGroup.Controls.Add(this.execButton);
            this.buttonsGroup.Controls.Add(this.cancelButton);
            this.buttonsGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsGroup.Location = new System.Drawing.Point(0, 414);
            this.buttonsGroup.Name = "buttonsGroup";
            this.buttonsGroup.Size = new System.Drawing.Size(702, 36);
            this.buttonsGroup.TabIndex = 1;
            this.buttonsGroup.TabStop = false;
            // 
            // execButton
            // 
            this.execButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.execButton.Location = new System.Drawing.Point(540, 7);
            this.execButton.Name = "execButton";
            this.execButton.Size = new System.Drawing.Size(75, 23);
            this.execButton.TabIndex = 1;
            this.execButton.Text = "Выполнить";
            this.execButton.UseVisualStyleBackColor = true;
            this.execButton.Click += new System.EventHandler(this.execButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(621, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Отменить";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // combinedQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 450);
            this.Controls.Add(this.buttonsGroup);
            this.Controls.Add(this.listQuerysCheckBox);
            this.Name = "combinedQueryForm";
            this.Text = "selectQueryForm + КЗ";
            this.Load += new System.EventHandler(this.combinedQueryForm_Load);
            this.buttonsGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox listQuerysCheckBox;
        private System.Windows.Forms.GroupBox buttonsGroup;
        private System.Windows.Forms.Button execButton;
        private System.Windows.Forms.Button cancelButton;
    }
}