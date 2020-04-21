namespace Grepy2
{
	partial class FontPicker
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.FontTextBox = new System.Windows.Forms.TextBox();
			this.SizeTextBox = new System.Windows.Forms.TextBox();
			this.FontListBox = new System.Windows.Forms.ListBox();
			this.SizeListBox = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ItalicCheckBox = new System.Windows.Forms.CheckBox();
			this.BoldCheckBox = new System.Windows.Forms.CheckBox();
			this.FixedWidthFontCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.FontOkButton = new System.Windows.Forms.Button();
			this.FontCancelButton = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Font:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(294, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 17);
			this.label2.TabIndex = 1;
			this.label2.Text = "Size:";
			// 
			// FontTextBox
			// 
			this.FontTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.FontTextBox.Location = new System.Drawing.Point(31, 33);
			this.FontTextBox.Name = "FontTextBox";
			this.FontTextBox.Size = new System.Drawing.Size(240, 23);
			this.FontTextBox.TabIndex = 1;
			this.FontTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FontMouseClick);
			this.FontTextBox.TextChanged += new System.EventHandler(this.FontTextChanged);
			// 
			// SizeTextBox
			// 
			this.SizeTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.SizeTextBox.Location = new System.Drawing.Point(296, 33);
			this.SizeTextBox.Name = "SizeTextBox";
			this.SizeTextBox.Size = new System.Drawing.Size(80, 23);
			this.SizeTextBox.TabIndex = 3;
			this.SizeTextBox.TextChanged += new System.EventHandler(this.SizeTextChanged);
			this.SizeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SizeKeyDown);
			// 
			// FontListBox
			// 
			this.FontListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.FontListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.FontListBox.FormattingEnabled = true;
			this.FontListBox.ItemHeight = 18;
			this.FontListBox.Location = new System.Drawing.Point(31, 56);
			this.FontListBox.Name = "FontListBox";
			this.FontListBox.Size = new System.Drawing.Size(240, 290);
			this.FontListBox.TabIndex = 2;
			this.FontListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.FontList_DrawItem);
			this.FontListBox.SelectedIndexChanged += new System.EventHandler(this.FontListIndedChanged);
			// 
			// SizeListBox
			// 
			this.SizeListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.SizeListBox.FormattingEnabled = true;
			this.SizeListBox.ItemHeight = 17;
			this.SizeListBox.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
			this.SizeListBox.Location = new System.Drawing.Point(296, 56);
			this.SizeListBox.Name = "SizeListBox";
			this.SizeListBox.Size = new System.Drawing.Size(80, 87);
			this.SizeListBox.TabIndex = 4;
			this.SizeListBox.SelectedIndexChanged += new System.EventHandler(this.SizeSelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ItalicCheckBox);
			this.groupBox1.Controls.Add(this.BoldCheckBox);
			this.groupBox1.Location = new System.Drawing.Point(400, 33);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(112, 81);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Font Style";
			// 
			// ItalicCheckBox
			// 
			this.ItalicCheckBox.AutoSize = true;
			this.ItalicCheckBox.Location = new System.Drawing.Point(20, 47);
			this.ItalicCheckBox.Name = "ItalicCheckBox";
			this.ItalicCheckBox.Size = new System.Drawing.Size(58, 21);
			this.ItalicCheckBox.TabIndex = 6;
			this.ItalicCheckBox.Text = "Italic";
			this.ItalicCheckBox.UseVisualStyleBackColor = true;
			this.ItalicCheckBox.CheckedChanged += new System.EventHandler(this.ItalicCheckBox_CheckedChanged);
			// 
			// BoldCheckBox
			// 
			this.BoldCheckBox.AutoSize = true;
			this.BoldCheckBox.Location = new System.Drawing.Point(20, 24);
			this.BoldCheckBox.Name = "BoldCheckBox";
			this.BoldCheckBox.Size = new System.Drawing.Size(58, 21);
			this.BoldCheckBox.TabIndex = 5;
			this.BoldCheckBox.Text = "Bold";
			this.BoldCheckBox.UseVisualStyleBackColor = true;
			this.BoldCheckBox.CheckedChanged += new System.EventHandler(this.BoldBox_CheckedChanged);
			// 
			// FixedWidthFontCheckBox
			// 
			this.FixedWidthFontCheckBox.AutoSize = true;
			this.FixedWidthFontCheckBox.Location = new System.Drawing.Point(296, 166);
			this.FixedWidthFontCheckBox.Name = "FixedWidthFontCheckBox";
			this.FixedWidthFontCheckBox.Size = new System.Drawing.Size(198, 21);
			this.FixedWidthFontCheckBox.TabIndex = 7;
			this.FixedWidthFontCheckBox.Text = "Show only fixed width fonts";
			this.FixedWidthFontCheckBox.UseVisualStyleBackColor = true;
			this.FixedWidthFontCheckBox.CheckedChanged += new System.EventHandler(this.FixedWidthFontCheckBox_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.richTextBox1);
			this.groupBox2.Location = new System.Drawing.Point(284, 200);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(228, 130);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sample Text";
			// 
			// FontOkButton
			// 
			this.FontOkButton.Location = new System.Drawing.Point(284, 336);
			this.FontOkButton.Name = "FontOkButton";
			this.FontOkButton.Size = new System.Drawing.Size(95, 31);
			this.FontOkButton.TabIndex = 8;
			this.FontOkButton.Text = "Ok";
			this.FontOkButton.UseVisualStyleBackColor = true;
			this.FontOkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// FontCancelButton
			// 
			this.FontCancelButton.Location = new System.Drawing.Point(417, 336);
			this.FontCancelButton.Name = "FontCancelButton";
			this.FontCancelButton.Size = new System.Drawing.Size(95, 31);
			this.FontCancelButton.TabIndex = 9;
			this.FontCancelButton.Text = "Cancel";
			this.FontCancelButton.UseVisualStyleBackColor = true;
			this.FontCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(3, 19);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(222, 108);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			// 
			// FontPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(524, 379);
			this.Controls.Add(this.FontCancelButton);
			this.Controls.Add(this.FontOkButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.FixedWidthFontCheckBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.SizeListBox);
			this.Controls.Add(this.FontListBox);
			this.Controls.Add(this.SizeTextBox);
			this.Controls.Add(this.FontTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FontPicker";
			this.ShowIcon = false;
			this.Text = "Font";
			this.Load += new System.EventHandler(this.FontPicker_Load);
			this.Move += new System.EventHandler(this.FontPicker_Move);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox FontTextBox;
		private System.Windows.Forms.TextBox SizeTextBox;
		private System.Windows.Forms.ListBox FontListBox;
		private System.Windows.Forms.ListBox SizeListBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox ItalicCheckBox;
		private System.Windows.Forms.CheckBox BoldCheckBox;
		private System.Windows.Forms.CheckBox FixedWidthFontCheckBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button FontOkButton;
		private System.Windows.Forms.Button FontCancelButton;
		private System.Windows.Forms.RichTextBox richTextBox1;
	}
}