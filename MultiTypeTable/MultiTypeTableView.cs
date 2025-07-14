using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 210 210 210 для текста
//38 38 46  фон
//28 27 33   подложка

namespace MultiTypeTable
{
	public partial class MultiTypeTableView : UserControl
	{
		

		private MultiTypeTableItem[] _items;
		
		// If we need to disable scroling to last selected elements
		// Uncommit this
		//protected override void OnLostFocus(EventArgs e)
		//{
		//	base.OnLostFocus(e);
		//	ActiveControl = panel;
		//}



		protected override Point ScrollToControl(Control activeControl)
		{
			if (activeControl != panel)
			{
				return base.ScrollToControl(activeControl);
			}
			else
			{			
				return this.DisplayRectangle.Location;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ActiveControl = panel;
            base.BorderStyle = BorderStyle.None;
			base.BackColor = ActiveControl.BackColor;
        }

		public MultiTypeTableView()
		{
			InitializeComponent();
		}

		public MultiTypeTableItem[] Items
		{
			get => _items;
			set
			{
				_items = value;
				RefreshView();
			}
		}

		private void RefreshView()
		{
			if (_items == null) return;
			Hide();
			SuspendLayout();
			panel.SuspendLayout();
			UnsubscrineFromEventsAndClearPanel();
			panel.RowCount = _items.Length;
			int i = 0;
			foreach (var item in _items)
			{
				var lbl = new Label
				{
					Text = item.Name, // + "   $" + item.Alias + "$", // + Environment.NewLine + "Alias $" + item.Alias + "$",
 //                   Margin = new Padding(5, 7, 5, 5),
                    Dock = DockStyle.Fill,
                    Font = new Font("Funnel Sans", 10),
					Height = (int)(FontHeight * 2.5f),
                };
                lbl.Margin = new Padding(5, 7, 5, 5);
                lbl.ForeColor = Color.FromArgb(210,210,210);
				
                panel.Controls.Add(lbl, 0, i);
				var ctrl = item.CreateControl();

				ctrl.KeyDown += MultiTypeTableView_KeyDown;

				panel.Controls.Add(ctrl, 1, i);
				ctrl.TabStop = true;
				ctrl.TabIndex = i;

                #region Styling examples
                // this code is slow ======================================
                // RowStyles can be not empty
                if (panel.RowStyles.Count <= i)
                {
                	panel.RowStyles.Add(new RowStyle(SizeType.Absolute, lbl.Height + 0));
                }
                ctrl.Font = new Font("Funnel Sans", 10);
				 
                ctrl.BackColor = Color.FromArgb(38, 38, 46);

                ctrl.Margin = new Padding(5, 5, 5, 5);
                //=========================================================
                //ctrl.BackColor = Color.Blue;

                //ctrl..Font = new Font("Roboto", 10);
                //ctrl.Font = new Font("Roboto", 10);
                //ctrl.Height = 20;
                //ctrl.Width = 20;
                //ctrl.BackColor = Color.Blue;
                //ctrl.ForeColor = Color.Red;

                ////left, top, right, bottom
                //ctrl.Margin = new Padding(5, 5, 5, 15);
                //ctrl.Padding = new Padding(10, 10, 10, 10);
                #endregion

                i++;
			}
			panel.ResumeLayout(true);
			ResumeLayout(true);
			Show();
		}

		private void MultiTypeTableView_KeyDown(object sender, KeyEventArgs e)
		{
			var control = (Control)sender;
			bool lastControl = control.TabIndex + 1 == _items.Length;

			if (e.KeyCode == Keys.Enter && !lastControl)
			{
				ProcessTabKey(true);
				e.SuppressKeyPress = true;
			}
		}

		private void UnsubscrineFromEventsAndClearPanel()
		{
			if (panel != null)
			{
				foreach (Control control in panel.Controls)
				{
					control.KeyDown -= MultiTypeTableView_KeyDown;
				}
			}
			panel.Controls.Clear();
            panel.RowStyles.Clear();
        }		
	}
}
