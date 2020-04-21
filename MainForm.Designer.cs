namespace Grepy2
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.searchCtrlFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalSplitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verticalSplitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allSearchMatchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayLineNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.searchStatusLabel = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.PreviewAfterTrackBar = new System.Windows.Forms.TrackBar();
			this.label3 = new System.Windows.Forms.Label();
			this.PreviewBeforeTrackBar = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SearchCountLabel = new System.Windows.Forms.Label();
			this.SearchingLabel = new System.Windows.Forms.Label();
			this.SearchingProgressBar = new System.Windows.Forms.ProgressBar();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.FileListView = new System.Windows.Forms.ListView();
			this.FilenameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FileTypeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FolderHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.MatchesHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FileSizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.DateTimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.RichTextBox = new Grepy2.BetterRichTextBox();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PreviewAfterTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PreviewBeforeTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.searchCtrlFToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.stopSearchToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(779, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.TabStop = true;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// searchCtrlFToolStripMenuItem
			// 
			this.searchCtrlFToolStripMenuItem.Name = "searchCtrlFToolStripMenuItem";
			this.searchCtrlFToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.searchCtrlFToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
			this.searchCtrlFToolStripMenuItem.Text = "Search (Ctrl+F)";
			this.searchCtrlFToolStripMenuItem.Click += new System.EventHandler(this.searchCtrlFToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "Options";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.horizontalSplitToolStripMenuItem,
            this.verticalSplitToolStripMenuItem,
            this.allSearchMatchesToolStripMenuItem,
            this.displayLineNumbersToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// horizontalSplitToolStripMenuItem
			// 
			this.horizontalSplitToolStripMenuItem.Name = "horizontalSplitToolStripMenuItem";
			this.horizontalSplitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
			this.horizontalSplitToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			this.horizontalSplitToolStripMenuItem.Text = "Horizontal Split";
			this.horizontalSplitToolStripMenuItem.Click += new System.EventHandler(this.horizontalSplitToolStripMenuItem_Click);
			// 
			// verticalSplitToolStripMenuItem
			// 
			this.verticalSplitToolStripMenuItem.Name = "verticalSplitToolStripMenuItem";
			this.verticalSplitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
			this.verticalSplitToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			this.verticalSplitToolStripMenuItem.Text = "Vertical Split";
			this.verticalSplitToolStripMenuItem.Click += new System.EventHandler(this.verticalSplitToolStripMenuItem_Click);
			// 
			// allSearchMatchesToolStripMenuItem
			// 
			this.allSearchMatchesToolStripMenuItem.Name = "allSearchMatchesToolStripMenuItem";
			this.allSearchMatchesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
			this.allSearchMatchesToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			this.allSearchMatchesToolStripMenuItem.Text = "All Search Matches";
			this.allSearchMatchesToolStripMenuItem.Click += new System.EventHandler(this.allSearchMatchesToolStripMenuItem_Click);
			// 
			// displayLineNumbersToolStripMenuItem
			// 
			this.displayLineNumbersToolStripMenuItem.Checked = true;
			this.displayLineNumbersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayLineNumbersToolStripMenuItem.Name = "displayLineNumbersToolStripMenuItem";
			this.displayLineNumbersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N)));
			this.displayLineNumbersToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			this.displayLineNumbersToolStripMenuItem.Text = "Display Line Numbers";
			this.displayLineNumbersToolStripMenuItem.Click += new System.EventHandler(this.displayLineNumbersToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
			// 
			// stopSearchToolStripMenuItem
			// 
			this.stopSearchToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.stopSearchToolStripMenuItem.Name = "stopSearchToolStripMenuItem";
			this.stopSearchToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
			this.stopSearchToolStripMenuItem.Text = "Stop Search";
			this.stopSearchToolStripMenuItem.Click += new System.EventHandler(this.stopSearchToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.searchStatusLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(779, 21);
			this.panel1.TabIndex = 0;
			// 
			// searchStatusLabel
			// 
			this.searchStatusLabel.AutoSize = true;
			this.searchStatusLabel.Location = new System.Drawing.Point(4, 2);
			this.searchStatusLabel.Name = "searchStatusLabel";
			this.searchStatusLabel.Size = new System.Drawing.Size(0, 13);
			this.searchStatusLabel.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.Add(this.PreviewAfterTrackBar);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.PreviewBeforeTrackBar);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.SearchCountLabel);
			this.panel2.Controls.Add(this.SearchingLabel);
			this.panel2.Controls.Add(this.SearchingProgressBar);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 271);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(779, 31);
			this.panel2.TabIndex = 3;
			this.panel2.TabStop = true;
			// 
			// PreviewAfterTrackBar
			// 
			this.PreviewAfterTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PreviewAfterTrackBar.AutoSize = false;
			this.PreviewAfterTrackBar.LargeChange = 1;
			this.PreviewAfterTrackBar.Location = new System.Drawing.Point(712, 3);
			this.PreviewAfterTrackBar.Maximum = 5;
			this.PreviewAfterTrackBar.Name = "PreviewAfterTrackBar";
			this.PreviewAfterTrackBar.Size = new System.Drawing.Size(61, 21);
			this.PreviewAfterTrackBar.TabIndex = 2;
			this.PreviewAfterTrackBar.ValueChanged += new System.EventHandler(this.PreviewAfterTrackBar_ValueChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(682, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "After:";
			// 
			// PreviewBeforeTrackBar
			// 
			this.PreviewBeforeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PreviewBeforeTrackBar.AutoSize = false;
			this.PreviewBeforeTrackBar.LargeChange = 1;
			this.PreviewBeforeTrackBar.Location = new System.Drawing.Point(614, 3);
			this.PreviewBeforeTrackBar.Maximum = 5;
			this.PreviewBeforeTrackBar.Name = "PreviewBeforeTrackBar";
			this.PreviewBeforeTrackBar.Size = new System.Drawing.Size(61, 21);
			this.PreviewBeforeTrackBar.TabIndex = 1;
			this.PreviewBeforeTrackBar.ValueChanged += new System.EventHandler(this.PreviewBeforeTrackBar_ValueChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(575, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Before:";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(496, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Preview Text";
			// 
			// SearchCountLabel
			// 
			this.SearchCountLabel.AutoSize = true;
			this.SearchCountLabel.Enabled = false;
			this.SearchCountLabel.Location = new System.Drawing.Point(164, 7);
			this.SearchCountLabel.Name = "SearchCountLabel";
			this.SearchCountLabel.Size = new System.Drawing.Size(176, 13);
			this.SearchCountLabel.TabIndex = 0;
			this.SearchCountLabel.Text = "9,999,999 Folders, 99,999,999 Files";
			// 
			// SearchingLabel
			// 
			this.SearchingLabel.AutoSize = true;
			this.SearchingLabel.Enabled = false;
			this.SearchingLabel.Location = new System.Drawing.Point(109, 7);
			this.SearchingLabel.Name = "SearchingLabel";
			this.SearchingLabel.Size = new System.Drawing.Size(58, 13);
			this.SearchingLabel.TabIndex = 0;
			this.SearchingLabel.Text = "Searching:";
			// 
			// SearchingProgressBar
			// 
			this.SearchingProgressBar.Enabled = false;
			this.SearchingProgressBar.Location = new System.Drawing.Point(4, 6);
			this.SearchingProgressBar.MarqueeAnimationSpeed = 10;
			this.SearchingProgressBar.Name = "SearchingProgressBar";
			this.SearchingProgressBar.Size = new System.Drawing.Size(101, 15);
			this.SearchingProgressBar.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 45);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.FileListView);
			this.splitContainer1.Panel1MinSize = 0;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.RichTextBox);
			this.splitContainer1.Panel2MinSize = 60;
			this.splitContainer1.Size = new System.Drawing.Size(779, 226);
			this.splitContainer1.SplitterDistance = 119;
			this.splitContainer1.SplitterWidth = 3;
			this.splitContainer1.TabIndex = 2;
			this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
			// 
			// FileListView
			// 
			this.FileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FilenameHeader,
            this.FileTypeHeader,
            this.FolderHeader,
            this.MatchesHeader,
            this.FileSizeHeader,
            this.DateTimeHeader});
			this.FileListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FileListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FileListView.FullRowSelect = true;
			this.FileListView.GridLines = true;
			this.FileListView.Location = new System.Drawing.Point(0, 0);
			this.FileListView.MultiSelect = false;
			this.FileListView.Name = "FileListView";
			this.FileListView.Size = new System.Drawing.Size(775, 115);
			this.FileListView.TabIndex = 1;
			this.FileListView.UseCompatibleStateImageBehavior = false;
			this.FileListView.View = System.Windows.Forms.View.Details;
			this.FileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.FileListView_ColumnClick);
			this.FileListView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.FileListView_ColumnWidthChanged);
			this.FileListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.FileListView_ItemSelectionChanged);
			this.FileListView.DoubleClick += new System.EventHandler(this.FileListView_DoubleClick);
			this.FileListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseClick);
			// 
			// FilenameHeader
			// 
			this.FilenameHeader.Text = "Filename";
			this.FilenameHeader.Width = 140;
			// 
			// FileTypeHeader
			// 
			this.FileTypeHeader.Text = "Type";
			this.FileTypeHeader.Width = 50;
			// 
			// FolderHeader
			// 
			this.FolderHeader.Text = "Folder";
			this.FolderHeader.Width = 300;
			// 
			// MatchesHeader
			// 
			this.MatchesHeader.Text = "Matches";
			this.MatchesHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// FileSizeHeader
			// 
			this.FileSizeHeader.Text = "Filesize";
			this.FileSizeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.FileSizeHeader.Width = 80;
			// 
			// DateTimeHeader
			// 
			this.DateTimeHeader.Text = "Date/Time";
			this.DateTimeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.DateTimeHeader.Width = 140;
			// 
			// RichTextBox
			// 
			this.RichTextBox.DetectUrls = false;
			this.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RichTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RichTextBox.HideSelection = false;
			this.RichTextBox.Location = new System.Drawing.Point(0, 0);
			this.RichTextBox.Name = "RichTextBox";
			this.RichTextBox.ReadOnly = true;
			this.RichTextBox.Size = new System.Drawing.Size(775, 100);
			this.RichTextBox.TabIndex = 1;
			this.RichTextBox.Text = "";
			this.RichTextBox.WordWrap = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(779, 302);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(640, 250);
			this.Name = "MainForm";
			this.Text = "Grepy 2.0";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.Move += new System.EventHandler(this.MainForm_Move);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PreviewAfterTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PreviewBeforeTrackBar)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem searchCtrlFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem horizontalSplitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticalSplitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem allSearchMatchesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopSearchToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView FileListView;
		private System.Windows.Forms.Label searchStatusLabel;
		private System.Windows.Forms.ColumnHeader FilenameHeader;
		private System.Windows.Forms.ColumnHeader FileTypeHeader;
		private System.Windows.Forms.ColumnHeader FolderHeader;
		private System.Windows.Forms.ColumnHeader MatchesHeader;
		private System.Windows.Forms.ColumnHeader FileSizeHeader;
		private System.Windows.Forms.ColumnHeader DateTimeHeader;
		private System.Windows.Forms.TrackBar PreviewAfterTrackBar;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TrackBar PreviewBeforeTrackBar;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label SearchCountLabel;
		private System.Windows.Forms.Label SearchingLabel;
		private System.Windows.Forms.ProgressBar SearchingProgressBar;
		private BetterRichTextBox RichTextBox;
		private System.Windows.Forms.ToolStripMenuItem displayLineNumbersToolStripMenuItem;
	}
}

