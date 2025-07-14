using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MultiTypeTable
{

	public abstract class MultiTypeTableItem
	{
		public string Name { get; set; }
		public string Alias { get; set; }
		public string Type { get; set; } 

		internal Control CreateControl()
		{
			return CreateControlImpl();
		}

		internal abstract Control CreateControlImpl();

		protected MultiTypeTableItem(string name, string alias, string type) // NEW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		{
			Name = name;
			Alias = alias;
			Type = type;
        }

		internal const AnchorStyles Anchoring = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
	}


	public class StringItem : MultiTypeTableItem
	{
		private readonly int _maxSize;
		private readonly string[] _history;
		private readonly bool _autocomplete;
		private readonly string _defValue;

		public string Value { get; private set; }

		public StringItem(string name, string alias, string type, int maxSize, string defValue, string[] history = null, bool autocomplete = false) : base(name, alias, type)
		{
			_maxSize = maxSize;
			_history = history;
			_autocomplete = autocomplete;
			_defValue = defValue;
			Value = _defValue;

            if (_autocomplete && _history == null)
			{

				throw new ArgumentNullException(nameof(history));
			}
		}
  

        internal override Control CreateControlImpl()
		{
			if (!_autocomplete)
			{
				var tb = new TextBox
				{
					MaxLength = _maxSize,
					Anchor = Anchoring,
					Text = _defValue
				};

				tb.TextChanged += (sender, args) => Value = tb.Text;
				tb.BackColor = Color.FromArgb(38,38,46);
				tb.ForeColor = Color.FromArgb(210,210,210);
                
                tb.BorderStyle = BorderStyle.None;

                return tb;
			}
			else
			{
				var cb = new ComboBox
				{
					Anchor = Anchoring,
					DropDownStyle = ComboBoxStyle.DropDown,
					AutoCompleteMode = AutoCompleteMode.Suggest,
					AutoCompleteSource = AutoCompleteSource.CustomSource,
					AutoCompleteCustomSource = new AutoCompleteStringCollection(),
					MaxLength = _maxSize,
					Text = _defValue,

					 BackColor = Color.FromArgb(38, 38, 46),
					 

					ForeColor = Color.FromArgb(210, 210, 210),
					// Убираем рамку
					FlatStyle = FlatStyle.Flat


				};

				//cb.FlatAppearance.BorderSize = 0;
				//.cb.DrawMode = DrawMode.OwnerDrawFixed;
                cb.AutoCompleteCustomSource.AddRange(_history);
				cb.Items.AddRange(_history);
				cb.TextChanged += (sender, args) => Value = cb.Text;
				cb.SelectedIndexChanged += (sender, args) => Value = (string)cb.SelectedItem;

                cb.BackColor = Color.FromArgb(38, 38, 46);
                cb.ForeColor = Color.FromArgb(210, 210, 210);

                

                // This code is very slow ====================================
                //For fix winforms bug
                cb.Resize += (sender, e) =>
				{
					if (!cb.IsHandleCreated)
						return;
					cb.BeginInvoke(new Action(() => cb.SelectionLength = 0));
				};

				cb.Leave += (sender, e) =>
				{
					cb.BeginInvoke(new Action(() => cb.SelectionLength = 0));
				};
				// This code is very slow ====================================

				cb.MouseWheel += (sender, e) =>
				{
					((HandledMouseEventArgs)e).Handled = true;
				};

                cb.BackColor = Color.FromArgb(38, 38, 46);
                cb.ForeColor = Color.FromArgb(210, 210, 210);

				cb.DrawMode = DrawMode.OwnerDrawFixed;
				
                return cb;
			}
			
		}
	}


	public class NumberItem : MultiTypeTableItem
	{
		private readonly string _format;
		private readonly decimal _defValue;

		public decimal Value { get; private set; }
		public string StringValue { get; private set; }

		private static readonly Regex _numRegex = new Regex(@"[+-]?[0-9,.]+([eE][+-]?[0-9]+)?");

		public NumberItem(string name, string alias, string type, string format) : this(name, alias, type, format, 0)
		{
            
        }

		public NumberItem(string name, string alias, string type, string format, decimal defValue) : base(name, alias, type)
		{
			_format = format;
			_defValue = defValue;
			Value = defValue;
         

        }

        internal override Control CreateControlImpl() =>
			new FormattingNumberBox(this, _format, _defValue)
			{
				Anchor = Anchoring,

                 ForeColor = Color.FromArgb(210, 210, 210),
                 BackColor = Color.FromArgb(210, 210, 210)

            };

		private sealed class FormattingNumberBox : TextBox
		{
			private readonly NumberItem _numberItem;
			private readonly string _format;
			private readonly decimal _defValue;

			public FormattingNumberBox(NumberItem numberItem, string format, decimal defValue)
			{
				_numberItem = numberItem;
				_format = format;
				_defValue = defValue;

				this.ForeColor = Color.FromArgb(210,210,210);
                this.BackColor = Color.FromArgb(210, 210, 210);

                this.BorderStyle = BorderStyle.None;
                OnLostFocus(null);
			}

			protected override void OnLostFocus(EventArgs e)
			{
				Text = _numRegex.Match(Text).Value;
				var number = _defValue;
				if (decimal.TryParse(Text, out var num))
				{
					number = num;
				}

				Text = number.ToString(_format);
				_numberItem.Value = number;
				_numberItem.StringValue = Text;
			}
		}
	}

	public class DateItem : MultiTypeTableItem
	{
		private readonly string _format;
		private readonly DateTime _defDate;
        private MaskedTextBox maskedTextBox;
        private Button dropDownButton;
        private MonthCalendar calendar;
        private Form popupForm;
        private DateTime _value = DateTime.Now;

        // Цветовая схема
        private readonly Color _backColor = Color.FromArgb(38, 38, 46);
        private readonly Color _foreColor = Color.FromArgb(230, 230, 230);
        private readonly Color _buttonBackColor = Color.FromArgb(55, 55, 65);
        private readonly Color _hoverColor = Color.FromArgb(65, 65, 75);


        public DateTime Value { get; private set; }
		public string StringValue { get; set; }

		public DateItem(string name, string alias, string type, string format) : this(name, alias, type, format, DateTime.Now)
		{
		}

		public DateItem(string name, string alias, string type, string format, DateTime defDate) : base(name, alias, type)
		{
			_format = format;
			_defDate = defDate;
			Value = defDate;

			var formatForDefValues = "{0:" + format + "}";

			StringValue = String.Format(CultureInfo.InvariantCulture, formatForDefValues, defDate);

		}

		internal override Control CreateControlImpl()
		{
			var impl = new MyDateTimePicker
			{
				// Format = DateTimePickerFormat.Custom,
				//CustomFormat = _format,
				////Value = _defDate,

				BackColor = Color.LightYellow,
				ForeColor = Color.DarkBlue,
				//Font = new Font("Segoe UI", 10, FontStyle.Bold),
				DateFormat = "dd.MM.yyyy",
				MinDate = new DateTime(2000, 1, 1),
				MaxDate = new DateTime(2100, 1, 1),
				Value = DateTime.Now,
				Anchor = Anchoring,
				Font = new Font("Funnel Sans", 10)


		};

			impl.ValueChanged += (sender, args) =>
			{
				Value = impl.Value;
				StringValue = impl.Text;
				impl.Font = impl.Font;
			};


			impl.BackColor = Color.FromArgb(38, 38, 46);       // Тёмный фон
			impl.ForeColor = Color.FromArgb(230, 230, 230);
			return impl;

		}


public class MyDateTimePicker : UserControl
	{
			private MaskedTextBox maskedTextBox;
			private Button dropDownButton;
			private MonthCalendar calendar;
			private Form popupForm;

			private DateTime _value = DateTime.Now;
			private DateTime _minDate = DateTimePicker.MinimumDateTime;
			private DateTime _maxDate = DateTimePicker.MaximumDateTime;

			// ✅ Свойства

			 
			public DateTime Value
			{
				get => _value;
				set
				{
					if (value < MinDate) _value = MinDate;
					else if (value > MaxDate) _value = MaxDate;
					else _value = value;

					UpdateText();
					OnValueChanged(EventArgs.Empty);
				}
			}

		 
			public DateTime MinDate
			{
				get => _minDate;
				set => _minDate = value;
			}

		 
			public DateTime MaxDate
			{
				get => _maxDate;
				set => _maxDate = value;
			}

			 
			public string DateFormat { get; set; } = "dd/MM/yyyy";

		 
			public string InputMask { get; set; } = "00/00/0000";

			// ✅ Цвета и шрифт
			public override Color BackColor
			{
				get => maskedTextBox.BackColor;
				set
				{
					maskedTextBox.BackColor = value;
					base.BackColor = value;
				}
			}

			public override Color ForeColor
			{
				get => maskedTextBox.ForeColor;
				set
				{
					maskedTextBox.ForeColor = value;
					base.ForeColor = value;
				}
			}

			public override Font Font
			{
				get => base.Font;
				set
				{
					base.Font = value;
					maskedTextBox.Font = value;
					dropDownButton.Font = value;
				}
			}

			// ✅ Событие ValueChanged
		 
			public event EventHandler ValueChanged;

			public MyDateTimePicker()
			{
				this.Height = 24;

				maskedTextBox = new MaskedTextBox
				{
					Dock = DockStyle.Fill,
					Mask = InputMask,
					BorderStyle = BorderStyle.None,
					TextAlign = HorizontalAlignment.Left,
					PromptChar = '_',
					Text = DateTime.Now.ToString("dd/MM/yyyy")
				};

				dropDownButton = new Button
				{
					Dock = DockStyle.Right,
					Width = 24,
					Text = "▼",
					FlatStyle = FlatStyle.Flat
				};
				dropDownButton.FlatAppearance.BorderSize = 0;
				dropDownButton.BackColor = Color.Transparent;

				this.Controls.Add(maskedTextBox);
				this.Controls.Add(dropDownButton);

				dropDownButton.Click += DropDownButton_Click;
				maskedTextBox.Leave += MaskedTextBox_Leave;
				maskedTextBox.KeyDown += MaskedTextBox_KeyDown;

				UpdateText();
			}

			// ✅ Календарь
			private void DropDownButton_Click(object sender, EventArgs e)
			{
				ApplyTextToValue();

				if (popupForm != null)
				{
					popupForm.Close();
					return;
				}

				calendar = new MonthCalendar
				{
					MaxSelectionCount = 1,
					SelectionStart = Value,
					MinDate = MinDate,
					MaxDate = MaxDate
				};

				calendar.DateSelected += Calendar_DateSelected;

				popupForm = new Form
				{
					FormBorderStyle = FormBorderStyle.None,
					ShowInTaskbar = false,
					StartPosition = FormStartPosition.Manual,
					BackColor = Color.White,
					Size = calendar.Size
				};

				popupForm.Deactivate += (s, ea) => ClosePopup();
				popupForm.FormClosed += (s, ea) => popupForm = null;
				popupForm.Controls.Add(calendar);

				var location = this.Parent.PointToScreen(new Point(this.Left, this.Bottom));
				popupForm.Location = location;
				popupForm.Show();
			}

			private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
			{
				Value = e.Start;
				ClosePopup();
			}

			private void ClosePopup()
			{
				if (popupForm != null)
				{
					popupForm.Close();
					popupForm = null;
				}
			}

			// ✅ Применение текста из MaskedTextBox
			private void ApplyTextToValue()
			{
				if (DateTime.TryParseExact(maskedTextBox.Text, DateFormat, CultureInfo.InvariantCulture,
						DateTimeStyles.None, out DateTime dt))
				{
					Value = dt;
				}
				else
				{
					UpdateText();
				}
			}

			private void MaskedTextBox_Leave(object sender, EventArgs e)
			{
				ApplyTextToValue();
			}

			private void MaskedTextBox_KeyDown(object sender, KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter)
				{
					ApplyTextToValue();
					e.Handled = e.SuppressKeyPress = true;
				}
				else if (e.KeyCode == Keys.Escape)
				{
					UpdateText();
					e.Handled = e.SuppressKeyPress = true;
				}
			}

			private void UpdateText()
			{
				


				maskedTextBox.Text = Value.ToString(DateFormat);
				
			}

			protected virtual void OnValueChanged(EventArgs e)
			{
				ValueChanged?.Invoke(this, e);
			}
		}

////..-----------------------
	public override string ToString()
		{
			return Value.ToString(_format) ?? "";
		}
	}

	public class ListBoxItem : MultiTypeTableItem
	{
		private readonly string[] _items;
		private readonly string _defValue;
		public string Value { get; private set; }

		private ComboBox _cb;

		public ListBoxItem(string name, string alias, string type, string[] items, string defValue) : base(name, alias, type)
		{
			_items = items;
			_defValue = defValue;
			Value = _defValue;
            
        }

		internal override Control CreateControlImpl()
		{
			var cb = new ComboBox
			{
				Anchor = Anchoring,
				DropDownStyle = ComboBoxStyle.DropDownList,
			};
			cb.Items.AddRange(_items);
			cb.SelectedItem = _defValue;
			cb.SelectedIndexChanged += (sender, args) => Value = (string)cb.SelectedItem;

					 


			cb.Enter += (sender, args) =>
			{
				if (Control.MouseButtons == MouseButtons.None)
				{
					var dropTimer = new System.Timers.Timer(100); // 100 ms delay before dropdown
					dropTimer.AutoReset = false;
					dropTimer.Elapsed += (s, arg) =>
					{
						if (cb != null)
						{
							if (!cb.Focused)
								cb.Focus();
							cb.DroppedDown = true;
						}
					};
					dropTimer.Start();
				}
			};

			cb.MouseWheel += (sender, e) =>
			{
				((HandledMouseEventArgs)e).Handled = true;
			};

			cb.FlatStyle = FlatStyle.Flat; // или FlatStyle.Popup
			cb.BackColor = Color.FromArgb(38, 38, 46);
            cb.ForeColor = Color.FromArgb(210, 210, 210);
            _cb = cb;
			return cb;
		}
	}

	public class CheckBoxItem : MultiTypeTableItem
	{
		private readonly bool _defValue;
		public bool Value { get; private set; }
		public string StringValue { get; set; }
		public string Trues_tring { get; private set; }
		public string False_string { get; private set; }

		public CheckBoxItem(string name, string alias, string type, string trues_tring, string false_string, bool defValue) : base(name, alias, type)
		{
			Trues_tring = trues_tring;
			False_string = false_string;
			StringValue = (defValue == true) ? Trues_tring : False_string;
			_defValue = defValue;
			Value = _defValue;

        }

		internal override Control CreateControlImpl()
		{
			var cb = new MyCheckBox
			{
				Checked = _defValue
			};

			cb.CheckedChanged += (sender, args) =>
			{
				Value = cb.Checked;
				StringValue = (cb.Checked == true) ? Trues_tring : False_string;
			};

            // cb.BackColor = Color.FromArgb(38, 222, 46);
            //..cb.ForeColor = Color.FromArgb(210, 222, 210);

            cb.BackColor = Color.FromArgb(38, 38, 46);
            cb.ForeColor = Color.FromArgb(210, 210, 210);

            return cb;
		}


		private class MyCheckBox : CheckBox
		{
			/// <inheritdoc />
			protected override void OnGotFocus(EventArgs e)
			{
				OnMouseEnter(e);
				base.OnGotFocus(e);
			}

			/// <inheritdoc />
			protected override void OnLostFocus(EventArgs e)
			{
				OnMouseLeave(e);
				base.OnLostFocus(e);
			}
		}
	}
}