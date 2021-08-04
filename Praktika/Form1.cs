using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Printing.NET;
using System.IO;

namespace Praktika
{
    public partial class Form1 : Form
    {
        protected const string MonitorName = "mfilemon";
        protected const string PortName = "TESTPORT:";
        protected const string DriverName = "Test Driver";
        protected const string PrinterName = "Test Printer";

        protected const string MonitorFile = "Printing Tests/mfilemon.dll";
        protected const string DriverFile = "Printing Tests/pscript5.dll";
        protected const string DriverDataFile = "Printing Tests/testprinter.ppd";
        protected const string DriverConfigFile = "Printing Tests/ps5ui.dll";
        protected const string DriverHelpFile = "Printing Tests/pscript.hlp";

        private Driver[] drivers;
        private Printer[] printers;
        public Form1()
        {
            PrintingApi.TryStart();
            InitializeComponent();
            var monitors = Monitor.All;
            var ports = Port.All;
            var drivers = Driver.All;
            update();
        }
        private void createPrinter()
        {
            (new System.Threading.Thread(delegate ()
            {
                var monitor = PrintingApi.Factory.CreateMonitor(MonitorName, MonitorFile);
                var port = PrintingApi.Factory.OpenPort(PortName, monitor);
                var driver = PrintingApi.Factory.InstallDriver(DriverName, DriverFile, DriverDataFile, DriverConfigFile, DriverHelpFile, 3, Printing.NET.Environment.Current, Printing.NET.DataType.RAW, null, monitor);
                var printer = PrintingApi.Factory.RunPrinter(PrinterName, port, driver);
            })).Start();
        }
        private void update()
        {
            try
            {
                drivers = Printing.NET.Driver.All;
                listBox1.Items.Clear();
                if (drivers == null)
                {
                    return;
                }
                //string[] array = {"Adobe PDF", "Fax", "Microsoft Print to PDF", "Microsoft XPS Document Writer", "PDF24 Fax", "S-1-5-21-3717614810-1076030755-1338514324-1002:OneNote for Windows 10", "Отправить в OneNote 16", "Отправить в OneNote 2013"};
                //printers = printers.Where(val => !array.Contains(val.Name)).ToArray();
                foreach (var printer in drivers)
                {
                    listBox1.Items.Add(printer.Name);
                }
                label1.Text = "";
                listBox2.Items.Clear();
                button3.Visible = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var index = listBox1.SelectedIndex;
                var driver = drivers[index];
                label1.Text = driver.Name;
                listBox2.Items.Clear();
                updatePrinters();
                button3.Visible = true;

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*var form = new AddPrinterForm();
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }*/

            var th = (new System.Threading.Thread(delegate ()
            {
                var monitor = PrintingApi.Factory.CreateMonitor(MonitorName, MonitorFile);
                if (monitor == null)
                {
                    MessageBox.Show("Не удалось создать монитор");
                }
                var port = PrintingApi.Factory.OpenPort(PortName, monitor);
                if (port == null)
                {
                    MessageBox.Show("Не удалось создать порт");
                }
                var driver = PrintingApi.Factory.InstallDriver(DriverName, DriverFile, DriverDataFile, DriverConfigFile, DriverHelpFile, 3, Printing.NET.Environment.Current, Printing.NET.DataType.RAW, null, monitor);
                if (driver == null)
                {
                    MessageBox.Show("Не удалось создать драйвер");
                }
                var printer = PrintingApi.Factory.RunPrinter("Новый", port, driver);
                if (printer == null)
                {
                    MessageBox.Show("Не удалось создать принтер");
                }

            }));
            th.Start();
            th.Join();
            update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотитие удалить этот драйвер и все принтеры, связанные с ним?", "Внимание", MessageBoxButtons.YesNo);

            if (res != DialogResult.Yes)
            {
                return;
            }
            var index = listBox1.SelectedIndex;
            var driver = drivers[index];
            try
            {
                deleteDriver(driver);
                update();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        private void deleteDriver(Driver driver)
        {
            try
            {
                PrintingApi.TryRestart();
                driver.Uninstall(null);
                PrintingApi.TryRestart();
                if (driver.Monitor != null)
                {
                    driver.Monitor.Uninstall(null);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотитие удалить все драйверы принтеров?", "Внимание", MessageBoxButtons.YesNo);

            if (res != DialogResult.Yes)
            {
                return;
            }

            try
            {
                foreach (var driver in drivers)
                {
                    deleteDriver(driver);
                }
                update();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        private void updatePrinters()
        {
            button5.Visible = false;
            var driver = listBox1.SelectedItem;
            printers = Printer.All.Where(item => item.Driver.Name == driver.ToString()).ToArray();
            foreach (var printer in printers)
            {
                listBox2.Items.Add(printer.Name);
            }
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button5.Visible = true;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотитие удалить этот принтер?", "Внимание", MessageBoxButtons.YesNo);

            if (res != DialogResult.Yes)
            {
                return;
            }
            var index = listBox2.SelectedIndex;
            var printer = printers[index];
            PrintingApi.TryRestart();
            printer.Uninstall();
            updatePrinters();
        }

    }
}
