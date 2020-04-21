using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Grepy2
{
	public partial class FontPicker : Form
	{
		// NOTE: see http://sourceforge.net/projects/customfontdialog/

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]  // JLB
		class LOGFONT {
			public int lfHeight;
			public int lfWidth;
			public int lfEscapement;
			public int lfOrientation;
			public int lfWeight;
			public byte lfItalic;
			public byte lfUnderline;
			public byte lfStrikeOut;
			public byte lfCharSet;
			public byte lfOutPrecision;
			public byte lfClipPrecision;
			public byte lfQuality;
			public byte lfPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string lfFaceName;
		}

		private bool bFontPickerFormInitComplete;
		private bool bFixedWidthOnly;
		public Font SelectedFont;

		public FontPicker(Font InFont)
		{
			bFontPickerFormInitComplete = false;

			InitializeComponent();

			bFixedWidthOnly = false;
			SelectedFont = null;

			using (Bitmap bmp = new Bitmap(1, 1))
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					if( IsMonospaced(g, InFont) )
					{
						FixedWidthFontCheckBox.Checked = true;  // this will cause InitializeFontList() to get called
					}
				}
			}

			if( !bFixedWidthOnly )  // if this is still false, we need to initialize the font list
			{
				InitializeFontList();
			}

			BoldCheckBox.Checked = InFont.Bold;
			ItalicCheckBox.Checked = InFont.Italic;

			SizeTextBox.Text = Convert.ToString(InFont.Size);  // size must be set before selecting the font name
			if( SelectFontByName(InFont.Name) )
			{
				SelectedFont = InFont;
			}

			richTextBox1.Clear();	
			richTextBox1.Font = InFont;
			richTextBox1.AppendText("The Quick Brown Fox Jumped Over The Lazy Dog's Back.");
		}

		private void FontPicker_Load(object sender, EventArgs e)
		{
			int pos_x = -1;
			int pos_y = -1;
			if( Config.Get(Config.KEY.FontPickerPosX, ref pos_x) && Config.Get(Config.KEY.FontPickerPosY, ref pos_y) )
			{
				Location = new Point(pos_x, pos_y);
			}
			else  // otherwise, center the window on the main form
			{
				this.CenterToParent();
			}

			bFontPickerFormInitComplete = true;  // window initialization is complete, okay to write config settings now
		}

		private void FontPicker_Move(object sender, EventArgs e)
		{
			if( bFontPickerFormInitComplete )
			{
				Config.Set(Config.KEY.FontPickerPosX, Location.X);
				Config.Set(Config.KEY.FontPickerPosY, Location.Y);
			}
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			if( SelectedFont != null )
			{
				this.DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				MessageBox.Show("'You haven't selected a Font", "Invalid Font");
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.SelectedFont = null;
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		static bool IsMonospaced(Graphics g, Font f)
		{
			float w1, w2;

			w1 = g.MeasureString("i", f).Width;
			w2 = g.MeasureString("W", f).Width;
			return w1 == w2;
		}

		static bool IsSymbolFont(Font font)
		{
			const byte SYMBOL_FONT = 2;

			LOGFONT logicalFont = new LOGFONT();
			font.ToLogFont(logicalFont);
			return logicalFont.lfCharSet == SYMBOL_FONT;
		}

		public void InitializeFontList()
		{
			SelectedFont = null;
			FontTextBox.Text = "";

			FontListBox.Items.Clear();

			using (Bitmap bmp = new Bitmap(1, 1))
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					foreach (FontFamily f in FontFamily.Families)
					{
						try
						{
							if( (f.Name != null) || (f.Name != "") )
							{
								Font newfont = new Font(f, 10);

								if( !IsSymbolFont(newfont) )
								{
									if( !bFixedWidthOnly || IsMonospaced(g, newfont) )
									{
										FontListBox.Items.Add(newfont);
									}
								}
							}
						}
						catch {}
					}
				}
			}
		}

		private void ItalicCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateSampleText();
		}

		private void BoldBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateSampleText();
		}

		private void FontList_DrawItem(object sender, DrawItemEventArgs e)
		{
			// Draw the background of the ListBox control for each item.
			e.DrawBackground();

			Font font = (Font)FontListBox.Items[e.Index];
			e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);

			// If the ListBox has focus, draw a focus rectangle around the selected item.
			e.DrawFocusRectangle();

		}

		private void FontListIndedChanged(object sender, EventArgs e)
		{
			if(FontListBox.SelectedItem != null)
			{
				if( !FontTextBox.Focused )
				{
					Font f = (Font)FontListBox.SelectedItem;
					FontTextBox.Text = f.Name;
				}

				UpdateSampleText();
			}
		}

		private void FixedWidthFontCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			bFixedWidthOnly = FixedWidthFontCheckBox.Checked;

			InitializeFontList();
		}

		private void FontMouseClick(object sender, MouseEventArgs e)
		{
			FontTextBox.SelectAll();
		}

		private bool SelectFontByName(string InName)
		{
            for( int i = 0; i < FontListBox.Items.Count; i++ )
            {
                string str = ((Font)FontListBox.Items[i]).Name;
                if( String.Equals(str, InName, StringComparison.OrdinalIgnoreCase) )
                {
                    FontListBox.SelectedIndex = i;

                    const uint WM_VSCROLL = 0x0115;
                    const uint SB_THUMBPOSITION = 4;

                    uint b = ((uint)(FontListBox.SelectedIndex) << 16) | (SB_THUMBPOSITION & 0xffff);
                    SendMessage(FontListBox.Handle, WM_VSCROLL, b, 0);

                    return true;
                }               
            }

			return false;
		}

		private void FontTextChanged(object sender, EventArgs e)
		{
            if( !FontTextBox.Focused )
			{
				return;
			}

			SelectFontByName(FontTextBox.Text);
		}

		private void SizeKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.D0:
				case Keys.D1:
				case Keys.D2:
				case Keys.D3:
				case Keys.D4:
				case Keys.D5:
				case Keys.D6:
				case Keys.D7:
				case Keys.D8:
				case Keys.D9:
				case Keys.End:
				case Keys.Enter:
				case Keys.Home:
				case Keys.Back:
				case Keys.Delete:
				case Keys.Escape:
				case Keys.Left:
				case Keys.Right:
					break;

				case Keys.Decimal:
				case (Keys)190: //decimal point
					if( SizeTextBox.Text.Contains(".") )
					{
						e.SuppressKeyPress = true;
						e.Handled = true;
					}
					break;

				default:
					e.SuppressKeyPress = true;
					e.Handled = true;
					break;
			}
		}

		private void SizeTextChanged(object sender, EventArgs e)
		{
			if( SizeListBox.Items.Contains(SizeTextBox.Text) )
			{
				SizeListBox.SelectedItem = SizeTextBox.Text;
			}
			else
			{
				SizeListBox.ClearSelected();
			}
			
			UpdateSampleText();
		}

		private void SizeSelectedIndexChanged(object sender, EventArgs e)
		{
			if(SizeListBox.SelectedItem != null)
			{
				SizeTextBox.Text = SizeListBox.SelectedItem.ToString();
			}
		}

		public int IndexOf(FontFamily ff)
		{
			for (int i = 1; i < FontListBox.Items.Count; i++)
			{
				Font f = (Font)FontListBox.Items[i];

				if( f.FontFamily.Name == ff.Name )
				{
					return i;
				}
			}

			return -1;
		}

		public FontFamily SelectedFontFamily
		{
			get 
			{
				if( FontListBox.SelectedItem != null )
				{
					return ((Font)FontListBox.SelectedItem).FontFamily;
				}

				return null;
			}
			set
			{
				if( value == null )
				{
					FontListBox.ClearSelected();
				}
				else
				{
					FontListBox.SelectedIndex = IndexOf(value);
				}

			}
		}

		private void UpdateSampleText()
		{
			float size = SizeTextBox.Text != "" ? float.Parse(SizeTextBox.Text) : 1;

			FontStyle style = BoldCheckBox.Checked ? FontStyle.Bold : FontStyle.Regular;

			if( ItalicCheckBox.Checked )
			{
				style |= FontStyle.Italic;
			}

			richTextBox1.Clear();	

			if( SelectedFontFamily != null)
			{
				SelectedFont = new Font(SelectedFontFamily, size, style);

				richTextBox1.Font = SelectedFont;
				richTextBox1.AppendText("The Quick Brown Fox Jumped Over The Lazy Dog's Back.");
			}
		}
	}
}
