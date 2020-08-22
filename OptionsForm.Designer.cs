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
			this.label7 = new System.Windows.Forms.Label();
			this.FileListFontTextBox = new System.Windows.Forms.TextBox();
			this.FileListFontButton = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.SearchResultsFontTextBox = new System.Windows.Forms.TextBox();
			this.SearchResultsFontButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// WindowsExplorerCheckBox
			// 
			this.WindowsExplorerCheckBox.AutoSize = true;
			this.WindowsExplorerCheckBox.Location = new System.Drawing.Point(52, 42);
			this.WindowsExplorerCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.WindowsExplorerCheckBox.Name = "WindowsExplorerCheckBox";
			this.WindowsExplorerCheckBox.Size = new System.Drawing.Size(287, 21);
			this.WindowsExplorerCheckBox.TabIndex = 0;
			this.WindowsExplorerCheckBox.Text = "Enable Windows File Explorer integration";
			this.WindowsExplorerCheckBox.UseVisualStyleBackColor = true;
			// 
			// WindowsFileAssociationCheckBox
			// 
			this.WindowsFileAssociationCheckBox.AutoSize = true;
			this.WindowsFileAssociationCheckBox.Checked = true;
			this.WindowsFileAssociationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.WindowsFileAssociationCheckBox.Location = new System.Drawing.Point(52, 262);
			this.WindowsFileAssociationCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.WindowsFileAssociationCheckBox.Name = "WindowsFileAssociationCheckBox";
			this.WindowsFileAssociationCheckBox.Size = new System.Drawing.Size(273, 21);
			this.WindowsFileAssociationCheckBox.TabIndex = 5;
			this.WindowsFileAssociationCheckBox.Text = "Use Windows file association for editor";
			this.WindowsFileAssociationCheckBox.UseVisualStyleBackColor = true;
			this.WindowsFileAssociationCheckBox.CheckedChanged += new System.EventHandler(this.WindowsFileAssociationCheckBox_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(48, 306);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Custom Editor:";
			// 
			// CustomEditorTextBox
			// 
			this.CustomEditorTextBox.Location = new System.Drawing.Point(52, 325);
			this.CustomEditorTextBox.Name = "CustomEditorTextBox";
			this.CustomEditorTextBox.ReadOnly = true;
			this.CustomEditorTextBox.Size = new System.Drawing.Size(517, 22);
			this.CustomEditorTextBox.TabIndex = 6;
			// 
			// CustomEditorButton
			// 
			this.CustomEditorButton.Enabled = false;
			this.CustomEditorButton.Location = new System.Drawing.Point(572, 324);
			this.CustomEditorButton.Name = "CustomEditorButton";
			this.CustomEditorButton.Size = new System.Drawing.Size(44, 27);
			this.CustomEditorButton.TabIndex = 7;
			this.CustomEditorButton.Text = "...";
			this.CustomEditorButton.UseVisualStyleBackColor = true;
			this.CustomEditorButton.Click += new System.EventHandler(this.CustomEditorButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(48, 369);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(250, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Syntax: \"<Editor Executable>\" [$F] [$L]";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(48, 396);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(97, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "$F = Filename";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(48, 423);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(119, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "$L = Line number";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(48, 450);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(454, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Example: \"C:\\Program Files (x86)\\Notepad++\\notepad++.exe\" \"$F\" -n$L";
			// 
			// WorkerThreadsComboBox
			// 
			this.WorkerThreadsComboBox.FormattingEnabled = true;
			this.WorkerThreadsComboBox.Location = new System.Drawing.Point(279, 80);
			this.WorkerThreadsComboBox.Margin = new System.Windows.Forms.Padding(4);
			this.WorkerThreadsComboBox.Name = "WorkerThreadsComboBox";
			this.WorkerThreadsComboBox.Size = new System.Drawing.Size(48, 24);
			this.WorkerThreadsComboBox.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(48, 84);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(223, 17);
			this.label6.TabIndex = 0;
			this.label6.Text = "Number of search worker threads:";
			// 
			// OptionsOkButton
			// 
			this.OptionsOkButton.Location = new System.Drawing.Point(161, 515);
			this.OptionsOkButton.Margin = new System.Windows.Forms.Padding(4);
			this.OptionsOkButton.Name = "OptionsOkButton";
			this.OptionsOkButton.Size = new System.Drawing.Size(95, 31);
			this.OptionsOkButton.TabIndex = 8;
			this.OptionsOkButton.Text = "OK";
			this.OptionsOkButton.UseVisualStyleBackColor = true;
			this.OptionsOkButton.Click += new System.EventHandler(this.OptionsOkButton_Click);
			// 
			// OptionsHelpButton
			// 
			this.OptionsHelpButton.Location = new System.Drawing.Point(291, 515);
			this.OptionsHelpButton.Margin = new System.Windows.Forms.Padding(4);
			this.OptionsHelpButton.Name = "OptionsHelpButton";
			this.OptionsHelpButton.Size = new System.Drawing.Size(95, 31);
			this.OptionsHelpButton.TabIndex = 9;
			this.OptionsHelpButton.Text = "Help";
			this.OptionsHelpButton.UseVisualStyleBackColor = true;
			this.OptionsHelpButton.Click += new System.EventHandler(this.OptionsHelpButton_Click);
			// 
			// OptionsCancelButton
			// 
			this.OptionsCancelButton.Location = new System.Drawing.Point(420, 515);
			this.OptionsCancelButton.Margin = new System.Windows.Forms.Padding(4);
			this.OptionsCancelButton.Name = "OptionsCancelButton";
			this.OptionsCancelButton.Size = new System.Drawing.Size(95, 31);
			this.OptionsCancelButton.TabIndex = 10;
			this.OptionsCancelButton.Text = "Cancel";
			this.OptionsCancelButton.UseVisualStyleBackColor = true;
			this.OptionsCancelButton.Click += new System.EventHandler(this.OptionsCancelButton_Click);
			// 
			// DeferRichTextCheckBox
			// 
			this.DeferRichTextCheckBox.AutoSize = true;
			this.DeferRichTextCheckBox.Location = new System.Drawing.Point(52, 174);
			this.DeferRichTextCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.DeferRichTextCheckBox.Name = "DeferRichTextCheckBox";
			this.DeferRichTextCheckBox.Size = new System.Drawing.Size(466, 21);
			this.DeferRichTextCheckBox.TabIndex = 3;
			this.DeferRichTextCheckBox.Text = "Defer RichText display of search results until after search is complete";
			this.DeferRichTextCheckBox.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(49, 130);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(140, 17);
			this.label7.TabIndex = 3;
			this.label7.Text = "File Information Font:";
			// 
			// FileListFontTextBox
			// 
			this.FileListFontTextBox.Location = new System.Drawing.Point(194, 127);
			this.FileListFontTextBox.Name = "FileListFontTextBox";
			this.FileListFontTextBox.ReadOnly = true;
			this.FileListFontTextBox.Size = new System.Drawing.Size(375, 22);
			this.FileListFontTextBox.TabIndex = 0;
			this.FileListFontTextBox.TabStop = false;
			// 
			// FileListFontButton
			// 
			this.FileListFontButton.Location = new System.Drawing.Point(572, 126);
			this.FileListFontButton.Name = "FileListFontButton";
			this.FileListFontButton.Size = new System.Drawing.Size(44, 27);
			this.FileListFontButton.TabIndex = 2;
			this.FileListFontButton.Text = "...";
			this.FileListFontButton.UseVisualStyleBackColor = true;
			this.FileListFontButton.Click += new System.EventHandler(this.FileListFontButton_Click);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(48, 218);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(140, 17);
			this.label8.TabIndex = 0;
			this.label8.Text = "Search Results Font:";
			// 
			// SearchResultsFontTextBox
			// 
			this.SearchResultsFontTextBox.Location = new System.Drawing.Point(194, 215);
			this.SearchResultsFontTextBox.Name = "SearchResultsFontTextBox";
			this.SearchResultsFontTextBox.ReadOnly = true;
			this.SearchResultsFontTextBox.Size = new System.Drawing.Size(375, 22);
			this.SearchResultsFontTextBox.TabIndex = 0;
			this.SearchResultsFontTextBox.TabStop = false;
			// 
			// SearchResultsFontButton
			// 
			this.SearchResultsFontButton.Location = new System.Drawing.Point(572, 214);
			this.SearchResultsFontButton.Name = "SearchResultsFontButton";
			this.SearchResultsFontButton.Size = new System.Drawing.Size(44, 27);
			this.SearchResultsFontButton.TabIndex = 4;
			this.SearchResultsFontButton.Text = "...";
			this.SearchResultsFontButton.UseVisualStyleBackColor = true;
			this.SearchResultsFontButton.Click += new System.EventHandler(this.SearchResultsFontButton_Click);
			// 
			// OptionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(672, 574);
			this.Controls.Add(this.SearchResultsFontButton);
			this.Controls.Add(this.SearchResultsFontTextBox);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.FileListFontButton);
			this.Controls.Add(this.FileListFontTextBox);
			this.Controls.Add(this.label7);
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
			this.Margin = new System.Windows.Forms.Padding(4);
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
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox FileListFontTextBox;
		private System.Windows.Forms.Button FileListFontButton;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox SearchResultsFontTextBox;
		private System.Windows.Forms.Button SearchResultsFontButton;
	}
}