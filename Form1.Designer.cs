namespace ncCreate
{
    partial class Form1
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
            this.Btn_OpenFile = new System.Windows.Forms.Button();
            this.Btn_NcCreate = new System.Windows.Forms.Button();
            this.original_code = new System.Windows.Forms.RichTextBox();
            this.converted_code = new System.Windows.Forms.RichTextBox();
            this.markItems = new System.Windows.Forms.RichTextBox();
            this.outsideItems = new System.Windows.Forms.RichTextBox();
            this.insideItems = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Btn_OpenFile
            // 
            this.Btn_OpenFile.Location = new System.Drawing.Point(12, 12);
            this.Btn_OpenFile.Name = "Btn_OpenFile";
            this.Btn_OpenFile.Size = new System.Drawing.Size(75, 36);
            this.Btn_OpenFile.TabIndex = 0;
            this.Btn_OpenFile.Text = "Open";
            this.Btn_OpenFile.UseVisualStyleBackColor = true;
            this.Btn_OpenFile.Click += new System.EventHandler(this.Btn_OpenFile_Click);
            // 
            // Btn_NcCreate
            // 
            this.Btn_NcCreate.Location = new System.Drawing.Point(93, 12);
            this.Btn_NcCreate.Name = "Btn_NcCreate";
            this.Btn_NcCreate.Size = new System.Drawing.Size(75, 36);
            this.Btn_NcCreate.TabIndex = 0;
            this.Btn_NcCreate.Text = "Create NC";
            this.Btn_NcCreate.UseVisualStyleBackColor = true;
            this.Btn_NcCreate.Click += new System.EventHandler(this.Btn_NcCreate_Click);
            // 
            // original_code
            // 
            this.original_code.Location = new System.Drawing.Point(12, 79);
            this.original_code.Name = "original_code";
            this.original_code.Size = new System.Drawing.Size(337, 359);
            this.original_code.TabIndex = 1;
            this.original_code.Text = "";
            // 
            // converted_code
            // 
            this.converted_code.Location = new System.Drawing.Point(451, 79);
            this.converted_code.Name = "converted_code";
            this.converted_code.Size = new System.Drawing.Size(337, 359);
            this.converted_code.TabIndex = 1;
            this.converted_code.Text = "";
            // 
            // markItems
            // 
            this.markItems.Location = new System.Drawing.Point(794, 326);
            this.markItems.Name = "markItems";
            this.markItems.Size = new System.Drawing.Size(337, 112);
            this.markItems.TabIndex = 2;
            this.markItems.Text = "";
            // 
            // outsideItems
            // 
            this.outsideItems.Location = new System.Drawing.Point(794, 208);
            this.outsideItems.Name = "outsideItems";
            this.outsideItems.Size = new System.Drawing.Size(337, 112);
            this.outsideItems.TabIndex = 3;
            this.outsideItems.Text = "";
            // 
            // insideItems
            // 
            this.insideItems.Location = new System.Drawing.Point(794, 90);
            this.insideItems.Name = "insideItems";
            this.insideItems.Size = new System.Drawing.Size(337, 112);
            this.insideItems.TabIndex = 4;
            this.insideItems.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1309, 450);
            this.Controls.Add(this.insideItems);
            this.Controls.Add(this.outsideItems);
            this.Controls.Add(this.markItems);
            this.Controls.Add(this.converted_code);
            this.Controls.Add(this.original_code);
            this.Controls.Add(this.Btn_NcCreate);
            this.Controls.Add(this.Btn_OpenFile);
            this.Name = "Form1";
            this.Text = "V6 Laser CNC";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_OpenFile;
        private System.Windows.Forms.Button Btn_NcCreate;
        private System.Windows.Forms.RichTextBox original_code;
        private System.Windows.Forms.RichTextBox converted_code;
        private System.Windows.Forms.RichTextBox markItems;
        private System.Windows.Forms.RichTextBox outsideItems;
        private System.Windows.Forms.RichTextBox insideItems;
    }
}

