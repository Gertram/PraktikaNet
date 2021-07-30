using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Praktika
{
    public partial class AddPrinterForm : Form
    {
        public AddPrinterForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        public string PrinterName { get => textBox2.Text; }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
