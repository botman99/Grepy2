namespace Grepy2
{
	partial class SearchForm
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
			this.RegExCheckBox = new System.Windows.Forms.CheckBox();
			this.MatchCaseCheckBox = new System.Windows.Forms.CheckBox();
			this.RecurseFolderCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SearchForComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.FileSpecComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.FolderComboBox = new System.Windows.Forms.ComboBox();
			this.FoldersButton = new System.Windows.Forms.Button();
			this.SearchOkButton = new System.Windows.Forms.Button();
			this.SearchHelpButton = new System.Windows.Forms.Button();
			this.SearchCancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// RegExCheckBox
			// 
			this.RegExCheckBox.AutoSize = true;
			this.RegExCheckBox.Location = new System.Drawing.Point(36, 21);
			this.RegExCheckBox.Name = "RegExCheckBox";
			this.RegExCheckBox.Size = new System.Drawing.Size(117, 17);
			this.RegExCheckBox.TabIndex = 1;
			this.RegExCheckBox.Text = "Regular Expression";
			this.RegExCheckBox.UseVisualStyleBackColor = true;
			// 
			// MatchCaseCheckBox
			// 
			this.MatchCaseCheckBox.AutoSize = true;
			this.MatchCaseCheckBox.Location = new System.Drawing.Point(217, 21);
			this.MatchCaseCheckBox.Name = "MatchCaseCheckBox";
			this.MatchCaseCheckBox.Size = new System.Drawing.Size(83, 17);
			this.MatchCaseCheckBox.TabIndex = 2;
			this.MatchCaseCheckBox.Text = "Match Case";
			this.MatchCaseCheckBox.UseVisualStyleBackColor = true;
			// 
			// RecurseFolderCheckBox
			// 
			this.RecurseFolderCheckBox.AutoSize = true;
			this.RecurseFolderCheckBox.Checked = true;
			this.RecurseFolderCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.RecurseFolderCheckBox.Location = new System.Drawing.Point(364, 21);
			this.RecurseFolderCheckBox.Name = "RecurseFolderCheckBox";
			this.RecurseFolderCheckBox.Size = new System.Drawing.Size(143, 17);
			this.RecurseFolderCheckBox.TabIndex = 3;
			this.RecurseFolderCheckBox.Text = "Recursive Folder Search";
			this.RecurseFolderCheckBox.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(33, 55);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Search For:";
			// 
			// SearchForComboBox
			// 
			this.SearchForComboBox.FormattingEnabled = true;
			this.SearchForComboBox.Location = new System.Drawing.Point(36, 71);
			this.SearchForComboBox.MaxDropDownItems = 10;
			this.SearchForComboBox.Name = "SearchForComboBox";
			this.SearchForComboBox.Size = new System.Drawing.Size(471, 21);
			this.SearchForComboBox.TabIndex = 4;
			this.SearchForComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchForComboBox_KeyDown);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(33, 109);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(218, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "File Specification(s) (use space for separator)";
			// 
			// FileSpecComboBox
			// 
			this.FileSpecComboBox.FormattingEnabled = true;
			this.FileSpecComboBox.Location = new System.Drawing.Point(36, 125);
			this.FileSpecComboBox.Name = "FileSpecComboBox";
			this.FileSpecComboBox.Size = new System.Drawing.Size(471, 21);
			this.FileSpecComboBox.TabIndex = 5;
			this.FileSpecComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileSpecComboBox_KeyDown);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(33, 163);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Folder To Search:";
			// 
			// FolderComboBox
			// 
			this.FolderComboBox.FormattingEnabled = true;
			this.FolderComboBox.Location = new System.Drawing.Point(36, 179);
			this.FolderComboBox.Name = "FolderComboBox";
			this.FolderComboBox.Size = new System.Drawing.Size(441, 21);
			this.FolderComboBox.TabIndex = 6;
			this.FolderComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FolderComboBox_KeyDown);
			// 
			// FoldersButton
			// 
			this.FoldersButton.Location = new System.Drawing.Point(478, 179);
			this.FoldersButton.Name = "FoldersButton";
			this.FoldersButton.Size = new System.Drawing.Size(30, 21);
			this.FoldersButton.TabIndex = 7;
			this.FoldersButton.Text = "...";
			this.FoldersButton.UseVisualStyleBackColor = true;
			this.FoldersButton.Click += new System.EventHandler(this.FoldersButton_Click);
			// 
			// SearchOkButton
			// 
			this.SearchOkButton.Location = new System.Drawing.Point(120, 233);
			this.SearchOkButton.Name = "SearchOkButton";
			this.SearchOkButton.Size = new System.Drawing.Size(71, 25);
			this.SearchOkButton.TabIndex = 8;
			this.SearchOkButton.Text = "OK";
			this.SearchOkButton.UseVisualStyleBackColor = true;
			this.SearchOkButton.Click += new System.EventHandler(this.SearchOkButton_Click);
			// 
			// SearchHelpButton
			// 
			this.SearchHelpButton.Location = new System.Drawing.Point(235, 233);
			this.SearchHelpButton.Name = "SearchHelpButton";
			this.SearchHelpButton.Size = new System.Drawing.Size(71, 25);
			this.SearchHelpButton.TabIndex = 9;
			this.SearchHelpButton.Text = "Help";
			this.SearchHelpButton.UseVisualStyleBackColor = true;
			this.SearchHelpButton.Click += new System.EventHandler(this.SearchHelpButton_Click);
			// 
			// SearchCancelButton
			// 
			this.SearchCancelButton.Location = new System.Drawing.Point(357, 233);
			this.SearchCancelButton.Name = "SearchCancelButton";
			this.SearchCancelButton.Size = new System.Drawing.Size(71, 25);
			this.SearchCancelButton.TabIndex = 10;
			this.SearchCancelButton.Text = "Cancel";
			this.SearchCancelButton.UseVisualStyleBackColor = true;
			this.SearchCancelButton.Click += new System.EventHandler(this.SearchCancelButton_Click);
			// 
			// SearchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(545, 280);
			this.Controls.Add(this.SearchCancelButton);
			this.Controls.Add(this.SearchHelpButton);
			this.Controls.Add(this.SearchOkButton);
			this.Controls.Add(this.FoldersButton);
			this.Controls.Add(this.FolderComboBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.FileSpecComboBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SearchForComboBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.RecurseFolderCheckBox);
			this.Controls.Add(this.MatchCaseCheckBox);
			this.Controls.Add(this.RegExCheckBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchForm";
			this.ShowIcon = false;
			this.Text = "Search";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchForm_Closing);
			this.Load += new System.EventHandler(this.SearchForm_Load);
			this.Shown += new System.EventHandler(this.SearchForm_Shown);
			this.Move += new System.EventHandler(this.SearchForm_Move);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox RegExCheckBox;
		private System.Windows.Forms.CheckBox MatchCaseCheckBox;
		private System.Windows.Forms.CheckBox RecurseFolderCheckBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox SearchForComboBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox FileSpecComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox FolderComboBox;
		private System.Windows.Forms.Button FoldersButton;
		private System.Windows.Forms.Button SearchOkButton;
		private System.Windows.Forms.Button SearchHelpButton;
		private System.Windows.Forms.Button SearchCancelButton;
	}
}