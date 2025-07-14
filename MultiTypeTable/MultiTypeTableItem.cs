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
		public string Type { get; set; } // NEW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
										 /// </summary>
										 /// <returns></returns>

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


	// Format strings fall into two categories: Standard format strings and
	// user-defined format strings. A format string consisting of a single
	// alphabetic character (A-Z or a-z), optionally followed by a sequence of
	// digits (0-9), is a standard format string. All other format strings are
	// used-defined format strings.
	//
	// A standard format string takes the form Axx, where A is an
	// alphabetic character called the format specifier and xx is a
	// sequence of digits called the precision specifier. The format
	// specifier controls the type of formatting applied to the number and the
	// precision specifier controls the number of significant digits or decimal
	// places of the formatting operation. The following table describes the
	// supported standard formats.
	//
	// C c - Currency format. The number is
	// converted to a string that represents a currency amount. The conversion is
	// controlled by the currency format information of the NumberFormatInfo
	// used to format the number. The precision specifier indicates the desired
	// number of decimal places. If the precision specifier is omitted, the default
	// currency precision given by the NumberFormatInfo is used.
	//
	// D d - Decimal format. This format is
	// supported for integral types only. The number is converted to a string of
	// decimal digits, prefixed by a minus sign if the number is negative. The
	// precision specifier indicates the minimum number of digits desired in the
	// resulting string. If required, the number will be left-padded with zeros to
	// produce the number of digits given by the precision specifier.
	//
	// E e Engineering (scientific) format.
	// The number is converted to a string of the form
	// "-d.ddd...E+ddd" or "-d.ddd...e+ddd", where each
	// 'd' indicates a digit (0-9). The string starts with a minus sign if the
	// number is negative, and one digit always precedes the decimal point. The
	// precision specifier indicates the desired number of digits after the decimal
	// point. If the precision specifier is omitted, a default of 6 digits after
	// the decimal point is used. The format specifier indicates whether to prefix
	// the exponent with an 'E' or an 'e'. The exponent is always consists of a
	// plus or minus sign and three digits.
	//
	// F f Fixed point format. The number is
	// converted to a string of the form "-ddd.ddd....", where each
	// 'd' indicates a digit (0-9). The string starts with a minus sign if the
	// number is negative. The precision specifier indicates the desired number of
	// decimal places. If the precision specifier is omitted, the default numeric
	// precision given by the NumberFormatInfo is used.
	//
	// G g - General format. The number is
	// converted to the shortest possible decimal representation using fixed point
	// or scientific format. The precision specifier determines the number of
	// significant digits in the resulting string. If the precision specifier is
	// omitted, the number of significant digits is determined by the type of the
	// number being converted (10 for int, 19 for long, 7 for
	// float, 15 for double, 19 for Currency, and 29 for
	// Decimal). Trailing zeros after the decimal point are removed, and the
	// resulting string contains a decimal point only if required. The resulting
	// string uses fixed point format if the exponent of the number is less than
	// the number of significant digits and greater than or equal to -4. Otherwise,
	// the resulting string uses scientific format, and the case of the format
	// specifier controls whether the exponent is prefixed with an 'E' or an
	// 'e'.
	//
	// N n Number format. The number is
	// converted to a string of the form "-d,ddd,ddd.ddd....", where
	// each 'd' indicates a digit (0-9). The string starts with a minus sign if the
	// number is negative. Thousand separators are inserted between each group of
	// three digits to the left of the decimal point. The precision specifier
	// indicates the desired number of decimal places. If the precision specifier
	// is omitted, the default numeric precision given by the
	// NumberFormatInfo is used.
	//
	// X x - Hexadecimal format. This format is
	// supported for integral types only. The number is converted to a string of
	// hexadecimal digits. The format specifier indicates whether to use upper or
	// lower case characters for the hexadecimal digits above 9 ('X' for 'ABCDEF',
	// and 'x' for 'abcdef'). The precision specifier indicates the minimum number
	// of digits desired in the resulting string. If required, the number will be
	// left-padded with zeros to produce the number of digits given by the
	// precision specifier.
	//
	// Some examples of standard format strings and their results are shown in the
	// table below. (The examples all assume a default NumberFormatInfo.)
	//
	// Value        Format  Result
	// 12345.6789   C       $12,345.68
	// -12345.6789  C       ($12,345.68)
	// 12345        D       12345
	// 12345        D8      00012345
	// 12345.6789   E       1.234568E+004
	// 12345.6789   E10     1.2345678900E+004
	// 12345.6789   e4      1.2346e+004
	// 12345.6789   F       12345.68
	// 12345.6789   F0      12346
	// 12345.6789   F6      12345.678900
	// 12345.6789   G       12345.6789
	// 12345.6789   G7      12345.68
	// 123456789    G7      1.234568E8
	// 12345.6789   N       12,345.68
	// 123456789    N4      123,456,789.0000
	// 0x2c45e      x       2c45e
	// 0x2c45e      X       2C45E
	// 0x2c45e      X8      0002C45E
	//
	// Format strings that do not start with an alphabetic character, or that start
	// with an alphabetic character followed by a non-digit, are called
	// user-defined format strings. The following table describes the formatting
	// characters that are supported in user defined format strings.
	//
	// 
	// 0 - Digit placeholder. If the value being
	// formatted has a digit in the position where the '0' appears in the format
	// string, then that digit is copied to the output string. Otherwise, a '0' is
	// stored in that position in the output string. The position of the leftmost
	// '0' before the decimal point and the rightmost '0' after the decimal point
	// determines the range of digits that are always present in the output
	// string.
	//
	// # - Digit placeholder. If the value being
	// formatted has a digit in the position where the '#' appears in the format
	// string, then that digit is copied to the output string. Otherwise, nothing
	// is stored in that position in the output string.
	//
	// . - Decimal point. The first '.' character
	// in the format string determines the location of the decimal separator in the
	// formatted value; any additional '.' characters are ignored. The actual
	// character used as a the decimal separator in the output string is given by
	// the NumberFormatInfo used to format the number.
	//
	// , - Thousand separator and number scaling.
	// The ',' character serves two purposes. First, if the format string contains
	// a ',' character between two digit placeholders (0 or #) and to the left of
	// the decimal point if one is present, then the output will have thousand
	// separators inserted between each group of three digits to the left of the
	// decimal separator. The actual character used as a the decimal separator in
	// the output string is given by the NumberFormatInfo used to format the
	// number. Second, if the format string contains one or more ',' characters
	// immediately to the left of the decimal point, or after the last digit
	// placeholder if there is no decimal point, then the number will be divided by
	// 1000 times the number of ',' characters before it is formatted. For example,
	// the format string '0,,' will represent 100 million as just 100. Use of the
	// ',' character to indicate scaling does not also cause the formatted number
	// to have thousand separators. Thus, to scale a number by 1 million and insert
	// thousand separators you would use the format string '#,##0,,'.
	//
	// % - Percentage placeholder. The presence of
	// a '%' character in the format string causes the number to be multiplied by
	// 100 before it is formatted. The '%' character itself is inserted in the
	// output string where it appears in the format string.
	//
	// E+ E- e+ e-   - Scientific notation.
	// If any of the strings 'E+', 'E-', 'e+', or 'e-' are present in the format
	// string and are immediately followed by at least one '0' character, then the
	// number is formatted using scientific notation with an 'E' or 'e' inserted
	// between the number and the exponent. The number of '0' characters following
	// the scientific notation indicator determines the minimum number of digits to
	// output for the exponent. The 'E+' and 'e+' formats indicate that a sign
	// character (plus or minus) should always precede the exponent. The 'E-' and
	// 'e-' formats indicate that a sign character should only precede negative
	// exponents.
	//
	// \ - Literal character. A backslash character
	// causes the next character in the format string to be copied to the output
	// string as-is. The backslash itself isn't copied, so to place a backslash
	// character in the output string, use two backslashes (\\) in the format
	// string.
	//
	// 'ABC' "ABC" - Literal string. Characters
	// enclosed in single or double quotation marks are copied to the output string
	// as-is and do not affect formatting.
	//
	// ; - Section separator. The ';' character is
	// used to separate sections for positive, negative, and zero numbers in the
	// format string.
	//
	// Other - All other characters are copied to
	// the output string in the position they appear.
	//
	// For fixed point formats (formats not containing an 'E+', 'E-', 'e+', or
	// 'e-'), the number is rounded to as many decimal places as there are digit
	// placeholders to the right of the decimal point. If the format string does
	// not contain a decimal point, the number is rounded to the nearest
	// integer. If the number has more digits than there are digit placeholders to
	// the left of the decimal point, the extra digits are copied to the output
	// string immediately before the first digit placeholder.
	//
	// For scientific formats, the number is rounded to as many significant digits
	// as there are digit placeholders in the format string.
	//
	// To allow for different formatting of positive, negative, and zero values, a
	// user-defined format string may contain up to three sections separated by
	// semicolons. The results of having one, two, or three sections in the format
	// string are described in the table below.
	//
	// Sections:
	//
	// One - The format string applies to all values.
	//
	// Two - The first section applies to positive values
	// and zeros, and the second section applies to negative values. If the number
	// to be formatted is negative, but becomes zero after rounding according to
	// the format in the second section, then the resulting zero is formatted
	// according to the first section.
	//
	// Three - The first section applies to positive
	// values, the second section applies to negative values, and the third section
	// applies to zeros. The second section may be left empty (by having no
	// characters between the semicolons), in which case the first section applies
	// to all non-zero values. If the number to be formatted is non-zero, but
	// becomes zero after rounding according to the format in the first or second
	// section, then the resulting zero is formatted according to the third
	// section.
	//
	// For both standard and user-defined formatting operations on values of type
	// float and double, if the value being formatted is a NaN (Not
	// a Number) or a positive or negative infinity, then regardless of the format
	// string, the resulting string is given by the NaNSymbol,
	// PositiveInfinitySymbol, or NegativeInfinitySymbol property of
	// the NumberFormatInfo used to format the number.

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

	/*  
 Customized format patterns:
 P.S. Format in the table below is the internal number format used to display the pattern.

 Patterns   Format      Description                           Example
 =========  ==========  ===================================== ========
    "h"     "0"         hour (12-hour clock)w/o leading zero  3
    "hh"    "00"        hour (12-hour clock)with leading zero 03
    "hh*"   "00"        hour (12-hour clock)with leading zero 03

    "H"     "0"         hour (24-hour clock)w/o leading zero  8
    "HH"    "00"        hour (24-hour clock)with leading zero 08
    "HH*"   "00"        hour (24-hour clock)                  08

    "m"     "0"         minute w/o leading zero
    "mm"    "00"        minute with leading zero
    "mm*"   "00"        minute with leading zero

    "s"     "0"         second w/o leading zero
    "ss"    "00"        second with leading zero
    "ss*"   "00"        second with leading zero

    "f"     "0"         second fraction (1 digit)
    "ff"    "00"        second fraction (2 digit)
    "fff"   "000"       second fraction (3 digit)
    "ffff"  "0000"      second fraction (4 digit)
    "fffff" "00000"         second fraction (5 digit)
    "ffffff"    "000000"    second fraction (6 digit)
    "fffffff"   "0000000"   second fraction (7 digit)

    "F"     "0"         second fraction (up to 1 digit)
    "FF"    "00"        second fraction (up to 2 digit)
    "FFF"   "000"       second fraction (up to 3 digit)
    "FFFF"  "0000"      second fraction (up to 4 digit)
    "FFFFF" "00000"         second fraction (up to 5 digit)
    "FFFFFF"    "000000"    second fraction (up to 6 digit)
    "FFFFFFF"   "0000000"   second fraction (up to 7 digit)

    "t"                 first character of AM/PM designator   A
    "tt"                AM/PM designator                      AM
    "tt*"               AM/PM designator                      PM

    "d"     "0"         day w/o leading zero                  1
    "dd"    "00"        day with leading zero                 01
    "ddd"               short weekday name (abbreviation)     Mon
    "dddd"              full weekday name                     Monday
    "dddd*"             full weekday name                     Monday


    "M"     "0"         month w/o leading zero                2
    "MM"    "00"        month with leading zero               02
    "MMM"               short month name (abbreviation)       Feb
    "MMMM"              full month name                       Febuary
    "MMMM*"             full month name                       Febuary

    "y"     "0"         two digit year (year % 100) w/o leading zero           0
    "yy"    "00"        two digit year (year % 100) with leading zero          00
    "yyy"   "D3"        year                                  2000
    "yyyy"  "D4"        year                                  2000
    "yyyyy" "D5"        year                                  2000
    ...

    "z"     "+0;-0"     timezone offset w/o leading zero      -8
    "zz"    "+00;-00"   timezone offset with leading zero     -08
    "zzz"      "+00;-00" for hour offset, "00" for minute offset  full timezone offset   -07:30
    "zzz*"  "+00;-00" for hour offset, "00" for minute offset   full timezone offset   -08:00

    "K"    -Local       "zzz", e.g. -08:00
           -Utc         "'Z'", representing UTC
           -Unspecified ""               
           -DateTimeOffset      "zzzzz" e.g -07:30:15

    "g*"                the current era name                  A.D.

    ":"                 time separator                        : -- DEPRECATED - Insert separator directly into pattern (eg: "H.mm.ss")
    "/"                 date separator                        /-- DEPRECATED - Insert separator directly into pattern (eg: "M-dd-yyyy")
    "'"                 quoted string                         'ABC' will insert ABC into the formatted string.
    '"'                 quoted string                         "ABC" will insert ABC into the formatted string.
    "%"                 used to quote a single pattern characters      E.g.The format character "%y" is to print two digit year.
    "\"                 escaped character                     E.g. '\d' insert the character 'd' into the format string.
    other characters    insert the character into the format string. 

Pre-defined format characters: 
    (U) to indicate Universal time is used.
    (G) to indicate Gregorian calendar is used.

    Format              Description                             Real format                             Example
    =========           =================================       ======================                  =======================
    "d"                 short date                              culture-specific                        10/31/1999
    "D"                 long data                               culture-specific                        Sunday, October 31, 1999
    "f"                 full date (long date + short time)      culture-specific                        Sunday, October 31, 1999 2:00 AM
    "F"                 full date (long date + long time)       culture-specific                        Sunday, October 31, 1999 2:00:00 AM
    "g"                 general date (short date + short time)  culture-specific                        10/31/1999 2:00 AM
    "G"                 general date (short date + long time)   culture-specific                        10/31/1999 2:00:00 AM
    "m"/"M"             Month/Day date                          culture-specific                        October 31
(G)     "o"/"O"             Round Trip XML                          "yyyy-MM-ddTHH:mm:ss.fffffffK"          1999-10-31 02:00:00.0000000Z
(G)     "r"/"R"             RFC 1123 date,                          "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"   Sun, 31 Oct 1999 10:00:00 GMT
(G)     "s"                 Sortable format, based on ISO 8601.     "yyyy-MM-dd'T'HH:mm:ss"                 1999-10-31T02:00:00
                                                                ('T' for local time)
    "t"                 short time                              culture-specific                        2:00 AM
    "T"                 long time                               culture-specific                        2:00:00 AM
(G)     "u"                 Universal time with sortable format,    "yyyy'-'MM'-'dd HH':'mm':'ss'Z'"        1999-10-31 10:00:00Z
                        based on ISO 8601.
(U)     "U"                 Universal time with full                culture-specific                        Sunday, October 31, 1999 10:00:00 AM
                        (long date + long time) format
                        "y"/"Y"             Year/Month day                          culture-specific                        October, 1999

*/

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

			//impl.CalendarMonthBackground = Color.FromArgb(38, 38, 46); // Фон календаря
			//impl.CalendarForeColor = Color.FromArgb(230, 230, 230);    // Текст в календаре
			//impl.CalendarTitleBackColor = Color.FromArgb(58, 58, 70);  // Заголовок календаря
			//impl.CalendarTrailingForeColor = Color.FromArgb(100, 100, 100); // Дни из других месяцев
			//impl.CalendarTitleBackColor = Color.FromArgb(38,38,46);

			// Задаём цвета фона и текста для Edit-поля
			//impl.EditBackColor = Color.LightYellow;
			//impl.EditForeColor = Color.DarkBlue;

			//impl.DisplayBackColor = impl.BackColor;
			//impl.DisplayForeColor = impl.ForeColor;



			return impl;

		}

		//internal class MyDateTimePicker : DateTimePicker
		//{
		//	private TextBox editTextBox;
		//	private bool isEditing;
		//	private string originalFormat;

		//	// Цвета для отображения и редактирования
		//	public Color DisplayBackColor { get; set; } = Color.FromArgb(230, 230, 230);
		//	public Color DisplayForeColor { get; set; } = Color.FromArgb(38, 38, 46);
		//	public Color EditBackColor { get; set; } = Color.FromArgb(230, 230, 230);
		//	public Color EditForeColor { get; set; } = Color.FromArgb(38, 38, 46);

		//	public MyDateTimePicker()
		//	{
		//		this.SetStyle(ControlStyles.UserPaint, true);
		//		this.ShowUpDown = false;

		//		editTextBox = new TextBox
		//		{
		//			Visible = false,
		//			BorderStyle = BorderStyle.None,
		//			Font = this.Font,
		//			TextAlign = HorizontalAlignment.Left,
		//			ForeColor = EditForeColor,
		//			BackColor = EditBackColor,
				  
		//	};

		//		this.Controls.Add(editTextBox);

		//		editTextBox.KeyDown += EditTextBox_KeyDown;
		//		editTextBox.TextChanged += EditTextBox_TextChanged;
		//		editTextBox.LostFocus += EditTextBox_LostFocus;
		//		this.Click += AdvancedDateTimePicker_Click;
		//	}

		//	protected override void OnPaint(PaintEventArgs e)
		//	{
		//		base.OnPaint(e);

		//		if (!isEditing)
		//		{
		//			using (SolidBrush backBrush = new SolidBrush(DisplayBackColor))
		//			{
		//				e.Graphics.FillRectangle(backBrush, this.ClientRectangle);
		//			}

		//			string text = this.Value.ToString(this.CustomFormat);
		//			using (SolidBrush textBrush = new SolidBrush(DisplayForeColor))
		//			{
		//				Rectangle textRect = new Rectangle(2, 2, this.Width - 20, this.Height - 4);
		//				StringFormat stringFormat = new StringFormat
		//				{
		//					LineAlignment = StringAlignment.Center
		//				};
						 
		//				e.Graphics.DrawString(text, new Font("Funnel Sans", 10), textBrush, textRect, stringFormat);
		//			}

		//			ComboBoxRenderer.DrawDropDownButton(
		//				e.Graphics,
		//				new Rectangle(this.Width - 18, (this.Height - 16) / 2, 16, 16),
		//				System.Windows.Forms.VisualStyles.ComboBoxState.Normal
		//			);

		//			ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.Gray, ButtonBorderStyle.Solid);
		//		}
		//	}

		//	private void AdvancedDateTimePicker_Click(object sender, EventArgs e)
		//	{
		//		Point clickPos = this.PointToClient(Cursor.Position);
		//		if (clickPos.X < this.Width - 20)
		//		{
		//			StartEditing();
		//		}
		//	}

		//	private void StartEditing()
		//	{
		//		if (isEditing) return;

		//		isEditing = true;
		//		originalFormat = this.CustomFormat;
		//		this.CustomFormat = "dd/MM/yyyy";

		//		editTextBox.Text = this.Value.ToString("dd/MM/yyyy");
		//		editTextBox.Location = new Point(3, (this.Height - editTextBox.Height) / 2);
		//		editTextBox.Size = new Size(this.Width - 20, editTextBox.Height);
		//		editTextBox.Visible = true;
		//		editTextBox.Focus();
		//		editTextBox.SelectAll();

		//		this.Invalidate();
		//	}

		//	private void EndEditing(bool acceptChanges)
		//	{
		//		if (!isEditing) return;

		//		isEditing = false;
		//		editTextBox.Visible = false;
		//		this.CustomFormat = originalFormat;

		//		if (acceptChanges)
		//		{
		//			TryParseAndUpdateValue(editTextBox.Text);
		//		}

		//		this.Invalidate();
		//	}

		//	private void EditTextBox_TextChanged(object sender, EventArgs e)
		//	{
		//		if (isEditing)
		//		{
		//			TryParseAndUpdateValue(editTextBox.Text);
		//		}
		//	}

		//	private void TryParseAndUpdateValue(string text)
		//	{
		//		if (DateTime.TryParseExact(
		//			text,
		//			"dd.MM.yyyy",
		//			CultureInfo.InvariantCulture,
		//			DateTimeStyles.None,
		//			out DateTime newDate))
		//		{
		//			this.Value = newDate;
		//		}
		//	}

		//	private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
		//	{
		//		switch (e.KeyCode)
		//		{
		//			case Keys.Enter:
		//				EndEditing(true);
		//				e.Handled = e.SuppressKeyPress = true;
		//				break;

		//			case Keys.Escape:
		//				EndEditing(false);
		//				e.Handled = e.SuppressKeyPress = true;
		//				break;

		//			case Keys.Up:
		//				ModifyDate(1);
		//				e.Handled = e.SuppressKeyPress = true;
		//				break;

		//			case Keys.Down:
		//				ModifyDate(-1);
		//				e.Handled = e.SuppressKeyPress = true;
		//				break;
		//		}
		//	}

		//	private void ModifyDate(int delta)
		//	{
		//		try
		//		{
		//			int pos = editTextBox.SelectionStart;
		//			string text = editTextBox.Text;

		//			if (pos <= 2) // День
		//			{
		//				int day = int.Parse(text.Substring(0, 2)) + delta;
		//				day = Math.Max(1, Math.Min(DateTime.DaysInMonth(this.Value.Year, this.Value.Month), day));
		//				editTextBox.Text = day.ToString("00") + text.Substring(2);
		//				editTextBox.Select(0, 2);
		//			}
		//			else if (pos <= 5) // Месяц
		//			{
		//				int month = int.Parse(text.Substring(3, 2)) + delta;
		//				month = Math.Max(1, Math.Min(12, month));
		//				editTextBox.Text = text.Substring(0, 3) + month.ToString("00") + text.Substring(5);
		//				editTextBox.Select(3, 2);
		//			}
		//			else // Год
		//			{
		//				int year = int.Parse(text.Substring(6, 4)) + delta;
		//				year = Math.Max(1, Math.Min(9999, year));
		//				editTextBox.Text = text.Substring(0, 6) + year.ToString("0000");
		//				editTextBox.Select(6, 4);
		//			}
		//		}
		//		catch { }
		//	}

		//	private void EditTextBox_LostFocus(object sender, EventArgs e)
		//	{
		//		EndEditing(true);
		//	}

		//	protected override void OnValueChanged(EventArgs eventargs)
		//	{
		//		base.OnValueChanged(eventargs);
		//		if (isEditing)
		//		{
		//			editTextBox.Text = this.Value.ToString("dd.MM.yyyy");
		//		}
		//		this.Invalidate();
		//	}
		//}


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