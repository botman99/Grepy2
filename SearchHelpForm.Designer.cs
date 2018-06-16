namespace Grepy2
{
	partial class SearchHelpForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchHelpForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.HelpRichTextBox = new System.Windows.Forms.RichTextBox();
			this.SearchHelpMoreButton = new System.Windows.Forms.Button();
			this.SearchHelpOkButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.HelpRichTextBox);
			this.panel1.Location = new System.Drawing.Point(17, 17);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(7);
			this.panel1.Size = new System.Drawing.Size(749, 387);
			this.panel1.TabIndex = 0;
			// 
			// HelpRichTextBox
			// 
			this.HelpRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.HelpRichTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HelpRichTextBox.Location = new System.Drawing.Point(7, 7);
			this.HelpRichTextBox.Name = "HelpRichTextBox";
			this.HelpRichTextBox.ReadOnly = true;
			this.HelpRichTextBox.Size = new System.Drawing.Size(735, 373);
			this.HelpRichTextBox.TabIndex = 0;
			this.HelpRichTextBox.TabStop = false;
			this.HelpRichTextBox.Text = resources.GetString("HelpRichTextBox.Text");
			this.HelpRichTextBox.WordWrap = false;
			this.HelpRichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.SearchHelpForm_LinkClicked);
			// 
			// SearchHelpMoreButton
			// 
			this.SearchHelpMoreButton.Location = new System.Drawing.Point(312, 421);
			this.SearchHelpMoreButton.Name = "SearchHelpMoreButton";
			this.SearchHelpMoreButton.Size = new System.Drawing.Size(71, 25);
			this.SearchHelpMoreButton.TabIndex = 1;
			this.SearchHelpMoreButton.Text = "More Help";
			this.SearchHelpMoreButton.UseVisualStyleBackColor = true;
			this.SearchHelpMoreButton.Click += new System.EventHandler(this.SearchHelpMoreButton_Click);
			// 
			// SearchHelpOkButton
			// 
			this.SearchHelpOkButton.Location = new System.Drawing.Point(399, 421);
			this.SearchHelpOkButton.Name = "SearchHelpOkButton";
			this.SearchHelpOkButton.Size = new System.Drawing.Size(71, 25);
			this.SearchHelpOkButton.TabIndex = 2;
			this.SearchHelpOkButton.Text = "OK";
			this.SearchHelpOkButton.UseVisualStyleBackColor = true;
			this.SearchHelpOkButton.Click += new System.EventHandler(this.SearchHelpOkButton_Click);
			// 
			// SearchHelpForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 458);
			this.Controls.Add(this.SearchHelpOkButton);
			this.Controls.Add(this.SearchHelpMoreButton);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchHelpForm";
			this.ShowIcon = false;
			this.Text = "Search Help";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchHelpForm_Closing);
			this.Shown += new System.EventHandler(this.SearchHelpForm_Shown);
			this.Move += new System.EventHandler(this.SearchHelpForm_Move);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RichTextBox HelpRichTextBox;
		private System.Windows.Forms.Button SearchHelpMoreButton;
		private System.Windows.Forms.Button SearchHelpOkButton;
	}
}