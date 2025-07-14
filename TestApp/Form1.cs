using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiTypeTable;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            string[] strings = { "e", "t" };
            InitializeComponent();
			multiTypeTableView1.Items = new MultiTypeTableItem[]
			{
				new NumberItem("Number of device 1", "C","D", "C", 15.0127M),
                new StringItem("string", "aliase", "string", 100, "привапт"),
                 new DateItem("Date", "date", "d","dd/MM/yyyy"),
                new StringItem("string", "aliase", "string", 100, "2123",new[]{"a", "b","c"} ,true),
                 new ListBoxItem("LisboxItem","list", "L", new[]{"a", "b","c"}, "defValue"),
                new NumberItem("Number of device 2", "C3","D", "C3", 15.0127M),
                new NumberItem("Number of device 3", "E","D", "E", 15.0127M),
                new NumberItem("Number of device", "E4","D", "E4", 15.0127M),
                new NumberItem("Number of device", "e10","D", "e10", 15.0127M),
                new NumberItem("Number of device", "F","D", "F", 15.0127M),
                new NumberItem("Number of device", "F3","D", "F3", 15.0127M),
                new NumberItem("Number of device", "F6","D", "F6", 15.0127M),
                new NumberItem("Number of device", "F0","D", "F0", 15.0127M),
                new NumberItem("Number of device", "G","D", "G", 15.0127M),
                new NumberItem("Number of device", "G5","D", "G5", 15.0127M),
                new NumberItem("Number of device", "G10","D", "G10", 15.0127M),
                new NumberItem("Number of device", "N","D", "N", 15617.0127M),
                new NumberItem("Number of device", "N3","D", "N3", 15617.0127M),
                new NumberItem("Number of device", "N6","D", "N6", 15617.0127M),
                new NumberItem("Number of device", "N0","D", "N0", 15617.0127M),
                new NumberItem("Number of device", "0.00","D", "0.00", 15.0127M),
                new NumberItem("Number of device", "0.00####","D", "0.00####", 15.0127M),
                new NumberItem("Number of device", "0.000000","D", "0.000000", 15.0127M),
               
                new DateItem("Date", "date", "d","dd/MM/yyyy"),
                new CheckBoxItem("Check Box1", "checkbox","Checkbox", "true string", "false string", true),
                new CheckBoxItem("Check Box2", "checkbox","Checkbox", "true string", "false string", false),
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
