namespace Grepy2
{
	partial class OptionsForm
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
			this.WindowsExplorerCheckBox = new System.Windows.Forms.CheckBox();
			this.WindowsFileAssociationCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.CustomEditorTextBox = new System.Windows.Forms.TextBox();
			this.CustomEditorButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.WorkerThreadsComboBox = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.OptionsOkButton = new System.Windows.Forms.Button();
			this.OptionsHelpButton = new System.Windows.Forms.Button();
			this.OptionsCancelButton = new System.Windows.Forms.Button();
			this.DeferRichTextCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// WindowsExplorerCheckBox
			// 
			this.WindowsExplorerCheckBox.AutoSize = true;
			this.WindowsExplorerCheckBox.Location = new System.Drawing.Point(39, 34);
			this.WindowsExplorerCheckBox.Name = "WindowsExplorerCheckBox";
			this.WindowsExplorerCheckBox.Size = new System.Drawing.Size(218, 17);
			this.WindowsExplorerCheckBox.TabIndex = 1;
			this.WindowsExplorerCheckBox.Text = "Enable Windows File Explorer integration";
			this.WindowsExplorerCheckBox.UseVisualStyleBackColor = true;
			// 
			// WindowsFileAssociationCheckBox
			// 
			this.WindowsFileAssociationCheckBox.AutoSize = true;
			this.WindowsFileAssociationCheckBox.Checked = true;
			this.WindowsFileAssociationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.WindowsFileAssociationCheckBox.Location = new System.Drawing.Point(39, 136);
			this.WindowsFileAssociationCheckBox.Name = "WindowsFileAssociationCheckBox";
			this.WindowsFileAssociationCheckBox.Size = new System.Drawing.Size(208, 17);
			this.WindowsFileAssociationCheckBox.TabIndex = 3;
			this.WindowsFileAssociationCheckBox.Text = "Use Windows file association for editor";
			this.WindowsFileAssociationCheckBox.UseVisualStyleBackColor = true;
			this.WindowsFileAssociationCheckBox.CheckedChanged += new System.EventHandler(this.WindowsFileAssociationCheckBox_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(36, 168);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Custom Editor:";
			// 
			// CustomEditorTextBox
			// 
			this.CustomEditorTextBox.Location = new System.Drawing.Point(39, 184);
			this.CustomEditorTextBox.Name = "CustomEditorTextBox";
			this.CustomEditorTextBox.ReadOnly = true;
			this.CustomEditorTextBox.Size = new System.Drawing.Size(390, 20);
			this.CustomEditorTextBox.TabIndex = 4;
			// 
			// CustomEditorButton
			// 
			this.CustomEditorButton.Enabled = false;
			this.CustomEditorButton.Location = new System.Drawing.Point(429, 184);
			this.CustomEditorButton.Name = "CustomEditorButton";
			this.CustomEditorButton.Size = new System.Drawing.Size(33, 20);
			this.CustomEditorButton.TabIndex = 5;
			this.CustomEditorButton.Text = "...";
			this.CustomEditorButton.UseVisualStyleBackColor = true;
			this.CustomEditorButton.Click += new System.EventHandler(this.CustomEditorButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(36, 219);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(192, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Syntax: \"<Editor Executable>\" [$F] [$L]";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(36, 241);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(73, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "$F = Filename";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(36, 263);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "$L = Line number";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(36, 285);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(350, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Example: \"C:\\Program Files (x86)\\Notepad++\\notepad++.exe\" \"$F\" -n$L";
			// 
			// WorkerThreadsComboBox
			// 
			this.WorkerThreadsComboBox.FormattingEnabled = true;
			this.WorkerThreadsComboBox.Location = new System.Drawing.Point(209, 65);
			this.WorkerThreadsComboBox.Name = "WorkerThreadsComboBox";
			this.WorkerThreadsComboBox.Size = new System.Drawing.Size(37, 21);
			this.WorkerThreadsComboBox.TabIndex = 2;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(36, 68);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(167, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Number of search worker threads:";
			// 
			// OptionsOkButton
			// 
			this.OptionsOkButton.Location = new System.Drawing.Point(121, 338);
			this.OptionsOkButton.Name = "OptionsOkButton";
			this.OptionsOkButton.Size = new System.Drawing.Size(71, 25);
			this.OptionsOkButton.TabIndex = 6;
			this.OptionsOkButton.Text = "OK";
			this.OptionsOkButton.UseVisualStyleBackColor = true;
			this.OptionsOkButton.Click += new System.EventHandler(this.OptionsOkButton_Click);
			// 
			// OptionsHelpButton
			// 
			this.OptionsHelpButton.Location = new System.Drawing.Point(218, 338);
			this.OptionsHelpButton.Name = "OptionsHelpButton";
			this.OptionsHelpButton.Size = new System.Drawing.Size(71, 25);
			this.OptionsHelpButton.TabIndex = 7;
			this.OptionsHelpButton.Text = "Help";
			this.OptionsHelpButton.UseVisualStyleBackColor = true;
			this.OptionsHelpButton.Click += new System.EventHandler(this.OptionsHelpButton_Click);
			// 
			// OptionsCancelButton
			// 
			this.OptionsCancelButton.Location = new System.Drawing.Point(315, 338);
			this.OptionsCancelButton.Name = "OptionsCancelButton";
			this.OptionsCancelButton.Size = new System.Drawing.Size(71, 25);
			this.OptionsCancelButton.TabIndex = 8;
			this.OptionsCancelButton.Text = "Cancel";
			this.OptionsCancelButton.UseVisualStyleBackColor = true;
			this.OptionsCancelButton.Click += new System.EventHandler(this.OptionsCancelButton_Click);
			// 
			// DeferRichTextCheckBox
			// 
			this.DeferRichTextCheckBox.AutoSize = true;
			this.DeferRichTextCheckBox.Location = new System.Drawing.Point(39, 102);
			this.DeferRichTextCheckBox.Name = "DeferRichTextCheckBox";
			this.DeferRichTextCheckBox.Size = new System.Drawing.Size(350, 17);
			this.DeferRichTextCheckBox.TabIndex = 9;
			this.DeferRichTextCheckBox.Text = "Defer RichText display of search results until after search is complete";
			this.DeferRichTextCheckBox.UseVisualStyleBackColor = true;
			// 
			// OptionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 386);
			this.Controls.Add(this.DeferRichTextCheckBox);
			this.Controls.Add(this.OptionsCancelButton);
			this.Controls.Add(this.OptionsHelpButton);
			this.Controls.Add(this.OptionsOkButton);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.WorkerThreadsComboBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.CustomEditorButton);
			this.Controls.Add(this.CustomEditorTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.WindowsFileAssociationCheckBox);
			this.Controls.Add(this.WindowsExplorerCheckBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsForm";
			this.ShowIcon = false;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.OptionsForm_Load);
			this.Shown += new System.EventHandler(this.OptionsForm_Shown);
			this.Move += new System.EventHandler(this.OptionsForm_Move);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox WindowsExplorerCheckBox;
		private System.Windows.Forms.CheckBox WindowsFileAssociationCheckBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox CustomEditorTextBox;
		private System.Windows.Forms.Button CustomEditorButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox WorkerThreadsComboBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button OptionsOkButton;
		private System.Windows.Forms.Button OptionsHelpButton;
		private System.Windows.Forms.Button OptionsCancelButton;
		private System.Windows.Forms.CheckBox DeferRichTextCheckBox;
	}
}