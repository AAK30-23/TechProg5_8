using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechProg5_8;

namespace TechProg5_8
{
    public partial class Form1 : Form
    {
        private List<List<string>> tabDataList;
        private DataTable newDataTable;
        public Form1()
        {
            InitializeComponent();
            DataGreedView();
            tabDataList = new List<List<string>>();
            ClearFiles();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            TabPage newTabPage = new TabPage(" " + (tabControl1.TabCount + 1));

            Random random = new Random();

            System.Windows.Forms.Label Label1 = new System.Windows.Forms.Label();
            Label1.Location = label1.Location;
            Label1.Size = label1.Size;
            Label1.Text = label1.Text;

            System.Windows.Forms.Label Label2 = new System.Windows.Forms.Label();
            Label2.Location = label2.Location;
            Label2.Size = label2.Size;
            Label2.Text = label2.Text;

            System.Windows.Forms.Label Label3 = new System.Windows.Forms.Label();
            Label3.Location = label3.Location;
            Label3.Size = label3.Size;
            Label3.Text = label3.Text;

            System.Windows.Forms.Label Label4 = new System.Windows.Forms.Label();
            Label4.Location = label4.Location;
            Label4.Size = label4.Size;
            Label4.Text = label4.Text;

            System.Windows.Forms.Label Label5 = new System.Windows.Forms.Label();
            Label5.Location = label5.Location;
            Label5.Size = label5.Size;
            Label5.Text = label5.Text;

            for (int i = 0; i < 5; i++)
            {
                TextBox textBox = new TextBox();
                textBox.Location = new Point(60, 10 + i * 50); // Adjust position
                textBox.Size = new Size(150, 20);
                //textBox.Font = new Font("Times New Roman", 12.0f);
                newTabPage.Controls.Add(textBox);
            }

            newTabPage.Controls.Add(Label1);
            newTabPage.Controls.Add(Label2);
            newTabPage.Controls.Add(Label3);
            newTabPage.Controls.Add(Label4);
            newTabPage.Controls.Add(Label5);
            tabControl1.TabPages.Add(newTabPage);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Нет вкладок для удаления");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            tabDataList.Clear();
            listBox1.Items.Clear();

            try
            {
                // Iterate through each TabPage in the TabControl
                foreach (TabPage tabPage in tabControl1.TabPages)
                {
                    List<string> tabValues = new List<string>();

                    foreach (Control control in tabPage.Controls)
                    {
                        if (control is TextBox textBox)
                        {
                            tabValues.Add(textBox.Text);
                        }
                    }
                    tabDataList.Add(tabValues);
                }

                foreach (List<string> tabValues in tabDataList)
                {
                    if (tabValues.Count >= 5)
                    {
                        try
                        {
                            //Считывание и расчеты
                            double X0 = double.Parse(tabValues[0]);
                            double Xk = double.Parse(tabValues[1]);
                            int Nx = int.Parse(tabValues[2]);
                            int Ny = int.Parse(tabValues[3]);
                            string[] yValues = tabValues[4].Split(' ');
                            Array.Sort(yValues);

                            List<double> xList = new List<double>();
                            List<double> yList = new List<double>();
                            List<double> resultList = new List<double>();

                            //string yListString = string.Join("; ", yList);
                            //MessageBox.Show(yListString);
                            if (X0 >= Xk)
                            {
                                throw new ArgumentException("Неверный ввод: начальное значение должно быть меньше конечного.");
                            }

                            if (Nx < 2)
                            {
                                throw new ArgumentException("Неверный ввод: Количество x-ов должно быть не менее 2.");
                            }

                            if (Ny < 2)
                            {
                                throw new ArgumentException("Неверный ввод: Количество y-ов должно быть не менее 2.");

                            }

                            if (yValues.Length > Ny || yValues.Length < Ny)
                            {
                                throw new ArgumentException("Неверный ввод: Количество необходимых y-ов указано в Ny.");
                            }

                            for (int i = 0; i < Nx; i++)
                            {
                                for (int j = 0; j < Ny; j++)
                                {
                                    double Xi = X0 + i * (Xk - X0) / (Nx - 1);
                                    xList.Add(Xi);
                                    double Yj = Convert.ToDouble(yValues[j]);
                                    yList.Add(Yj);

                                    func Func = new func(Xi, Yj);

                                    if (Func.IsValid)
                                    {
                                        double result = Func.Result;
                                        resultList.Add(result);
                                    }
                                    else
                                    {
                                        CreateMyError($"G{tabDataList.IndexOf(tabValues) + 1:D4}.dat", "G(x) = x/exp(y) ", Xi, Yj, "Неверный ввод");
                                        resultList.Add(double.NaN);
                                    }
                                }
                            }

                            listBox1.Items.Add("Tab " + (tabDataList.IndexOf(tabValues) + 1) + ":");

                            // Отображение
                            for (int i = 0; i < Nx * Ny; i++)
                            {
                                listBox1.Items.Add("G(" + xList[i].ToString() + "; " + yList[i].ToString() + ") = " + resultList[i].ToString());
                            }

                            ToFile(tabDataList.IndexOf(tabValues) + 1, xList, yList, resultList);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Ошибка. Проверьте входные данные");
                        }
                    }
                    else
                    {
                        listBox1.Items.Add("Недопустимые данные Tab " + (tabDataList.IndexOf(tabValues) + 1));
                    }
                }
            }
            catch (FormatException ex)
            {
                listBox1.Items.Add("Ошибка: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                listBox1.Items.Add("Ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Ошибка: " + ex.Message);
            }
        }

        private void ClearFiles()
        {
            string directoryPath = Directory.GetCurrentDirectory();
            string[] dataFiles = Directory.GetFiles(directoryPath, "G*.dat");

            foreach (string file in dataFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении файла: " + ex.Message);
                }
            }
        }

        private void ToFile(int tabNumber, List<double> xList, List<double> yList, List<double> resultList)
        {
            string fileName = "G" + tabNumber.ToString("D4") + ".dat";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Рассчитываемая функция: G = x/exp(y)");
                writer.WriteLine($"Количество значений X и Y: {xList.Count}");
                writer.WriteLine("X\t\t\t\t\t\tY\t\t\tResult");
                for (int i = 0; i < xList.Count; i++)
                {
                    writer.WriteLine($"{xList[i].ToString().PadRight(20)}\t{yList[i].ToString().PadRight(10)}\t{resultList[i].ToString().PadRight(10)}");
                }

                /*writer.Write("x/y\t\t\t\t\t\t\t");
                // Выводим заголовки
                for (int i = 0; i < xList.Count; i+=2)
                {
                    writer.Write($"<{xList[i]}> \t\t\t\t\t\t\t");
                }
                writer.WriteLine();

                /*writer.Write("x\\y\t\t");
                foreach (var y in yList)
                {
                    foreach (var x in xList)
                    {
                        double result = x / Math.Exp(y);
                        writer.Write(result.ToString().PadRight(20));
                    }
                    writer.WriteLine();
                }*/
            }
        }

        private void CreateMyProgram()
        {
            string logFileName = "myProgram.log";
            string logfilePath = Path.Combine(Environment.CurrentDirectory, logFileName);
            if (File.Exists(logfilePath))
            {
                File.Delete(logfilePath);
            }

            string prog = "Название программы: TechProg5_7\nНомер варианта: 24\n";
            string time = $"Дата и время начала выполнения расчёта: {DateTime.Now}\n";
            string func = "Рассчитываемая функция: G(x) = x/exp(y) ;\n";

            StringBuilder resultFiles = new StringBuilder();
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                resultFiles.Append($"G{i + 1:D4}.dat\n");
            }

            string logContent = prog + time + func + resultFiles.ToString();
            using (StreamWriter writer = new StreamWriter(logFileName))
            {
                writer.Write(logContent);
            }
            MessageBox.Show("Файл myProgram.log создан.");
        }

        private void CreateMyError(string fileName, string funcName, double x, double y, string errorType)
        {
            string errorLogFileName = "Error.log";
            string errorLogFilePath = Path.Combine(Environment.CurrentDirectory, errorLogFileName);

            using (StreamWriter writer = new StreamWriter(errorLogFilePath, true))
            {
                writer.WriteLine($"Файл: {fileName}");
                writer.WriteLine($"Функция: {funcName}");
                writer.WriteLine($"x: {x}, y: {y}");
                writer.WriteLine($"Тип ошибки: {errorType}");
            }
        }

        private void buttonToFile_Click(object sender, EventArgs e)
        {
            CreateMyProgram();
            ListBox2();
        }

        private void buttonFromFile_Click(object sender, EventArgs e)
        {
            ReadFromFile();
            dataGridView1.DataSource = newDataTable;
        }

        public void DataGreedView()
        {
            newDataTable = new DataTable();
            newDataTable.Columns.Add("Вкладка", typeof(string));
            newDataTable.Columns.Add("x", typeof(double));
            newDataTable.Columns.Add("y", typeof(double));
            newDataTable.Columns.Add("G(x; y)", typeof(double));
        }

        public void ListBox2()
        {
            string ProgLogName = "myProgram.log";
            string ProgLogPath = Path.Combine(Environment.CurrentDirectory, ProgLogName);

            if (!File.Exists(ProgLogPath))
            {
                MessageBox.Show("Файл myProgram.log не найден.");
                return;
            }
            try
            {
                string[] lines = File.ReadAllLines(ProgLogPath);
                IEnumerable<string> fileNames = lines.Skip(4);
                fileNames = fileNames.Select(line => line.Trim());
                foreach (string fileName in fileNames)
                {
                    listBox2.Items.Add(fileName); // Добавляем имя файла в listBox
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        public void ReadFromFile()
        {
            string ProgLogName = "myProgram.log";
            string ProgLogPath = Path.Combine(Environment.CurrentDirectory, ProgLogName);

            if (!File.Exists(ProgLogPath))
            {
                MessageBox.Show("Файл myProgram.log не найден.");
                return;
            }
            try
            {
                string[] lines = File.ReadAllLines(ProgLogPath);
                IEnumerable<string> fileNames = lines.Skip(4);
                fileNames = fileNames.Select(line => line.Trim());
                foreach (string fileName in listBox2.SelectedItems)
                {
                    if (fileNames.Contains(fileName))
                    {
                        Read(fileName);
                    }
                }
                MessageBox.Show("Данные успешно считаны.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        public void Read(string fileName)
        {
            try
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                if (!File.Exists(filePath))
                {
                    CreateMyError(fileName, "Файл не найден", 0, 0, "Файл не найден");
                    return;
                }
                using (StreamReader reader = new StreamReader(filePath))
                {
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split('\t');

                        if (parts.Length >= 3 && double.TryParse(parts[0], out double x) && double.TryParse(parts[1], out double y) && double.TryParse(parts[2], out double result))
                        {
                            newDataTable.Rows.Add(fileName, x, y, result);
                        }
                        else
                        {
                            CreateMyError(fileName, "Неверные данные", 0, 0, "Неверные данные");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CreateMyError(fileName, "Ошибка при чтении файла", 0, 0, ex.Message);
            }
        }
    }
}

