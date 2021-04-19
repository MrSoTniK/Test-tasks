using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.IO;

namespace Arrears
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {       
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            AddingWindow addingWindow = new AddingWindow();
            addingWindow.Show();           
        }

        private void IdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0)));
        }

        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            bool isNotNull = int.TryParse(IdTextBox.Text, out int id);
            bool isExist = false;
            Structure rowForDelete = new Structure(0,"0","0","0","0",0,0,0,0,0,0,0,0,0,0,0,0,0);
            if (isNotNull == true) 
            {
                foreach (var arrear in (this.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList)
                {
                    if (arrear.Id == id) 
                    {
                        rowForDelete = arrear;
                        isExist = true;
                        break;
                    }                 
                }

                if(isExist == true) 
                {
                    (this.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList.Remove(rowForDelete);
                    using (ApplicationContext appContext = new ApplicationContext())
                    {
                        appContext.DataBase.Remove(rowForDelete);
                        appContext.SaveChanges();                     
                    }
                    this.arrearsSfDataGrid.View.Refresh();
                }
                else 
                {
                    MessageBox.Show("Строки с таким ID не существует!");
                }
            }
            else 
            {
                MessageBox.Show("Текстовый блок с ID не заполнен!");
            }           
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            CompareWindow compWind = new CompareWindow();
            compWind.Show();
        }

        private void TxtButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath, line;
            FileStream fileStream;
            StreamReader reader;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true) 
            {
                filePath = openFileDialog.FileName;
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(filePath);
                while((line = reader.ReadLine()) != null) 
                {
                    WriteToTable(line);
                }
                fileStream.Close();
                reader.Close();
            }           
        }

        private void WriteToTable(string line) 
        {
            string value ="";
            List<string> stringValues = new List<string>();
            List<double> values = new List<double>();
            int quantity = 0, number = 0;            

            if(line.StartsWith("ТБ=01") == false && line.StartsWith("*") == false && line.StartsWith("#") == false && line.StartsWith("|") == false) 
            {
                foreach(char symbol in line) 
                { 
                    if(quantity < 4) 
                    {
                        number = quantity;
                        value = AddStringValues(symbol, stringValues, value, out quantity, number);
                    }
                    else 
                    {
                        value = AddIntValues(symbol, values, value);                       
                    }                   
                }

                if(stringValues.Count == 4 && values.Count >= 13) 
                {
                    int id = 0;
                    int rowsQuantity = (this.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList.Count;
                    if(rowsQuantity != 0) 
                    {
                        id = (this.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList[rowsQuantity - 1].Id + 1;
                    }                  
                   
                    Structure structure = new Structure(id, stringValues[0], stringValues[1], stringValues[2], stringValues[3], values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11], values[12]);
                    (this.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList.Add(structure);

                    using (ApplicationContext appContext = new ApplicationContext())
                    {
                        appContext.DataBase.AddRange(structure);
                        appContext.SaveChanges();
                    }

                    this.arrearsSfDataGrid.View.Refresh();
                }               
            }               
        }

        private string AddStringValues(char symbol, List<string> stringValues, string value, out int quantity, int number) 
        {
            quantity = number;
            if (symbol == '|')
            {
                stringValues.Add(value);
                value = "";
                quantity++;
            }
            else
            {
                value += symbol;               
            }

            return value;
        }

        private string AddIntValues(char symbol, List<double> values, string value)
        {
            double number = 0;
            if (symbol == '|')
            {
                double.TryParse(value, out number);
                values.Add(number);
                value = "";               
            }
            else
            {
                value += symbol;               
            }
            return value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (this.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList.Clear();
            this.arrearsSfDataGrid.Dispose();
            using (ApplicationContext appContext = new ApplicationContext())
            {
                appContext.Dispose();
            }
        }

        private void UnloadButton_Click(object sender, RoutedEventArgs e)
        {
            UploadWindow uploadWindow = new UploadWindow();
            uploadWindow.Show();
        }
    }

    public class ValuesModel 
    {  
        public List<Structure> ArrearsList { get; private set; }

        public ValuesModel() 
        {
            ArrearsList = new List<Structure>();           
            GetValues();
        }      

        public void GetValues()
        {
            using (ApplicationContext appContext = new ApplicationContext())
            {
                ArrearsList = appContext.DataBase.ToList();
            }
        }      
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Structure> DataBase { get; private set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=arrearsDataBase;Username=postgres;Password=qrc81o27h");
        }
    }

    public class Structure
    {
        public int Id { get; private set; }
        public string AccountNumberPartOne { get; private set; }
        public string AccountNumberPartTwo { get; private set; }
        public string AccountNumberPartThree { get; private set; }
        public string AccountNumberPartFour { get; private set; }
        public double YearStartArrearsSumTotal { get; private set; }
        public double YearStartArrearsSumLongterm { get; private set; }
        public double YearStartArrearsOverdue { get; private set; }
        public double ArrearsChangeIncreaseTotal { get; private set; }
        public double ArrearsChangeIncreaseNonmonetary { get; private set; }
        public double ArrearsChangeDecreaseTotal { get; private set; }
        public double ArrearsChangeDecreaseNonmonetary { get; private set; }
        public double YearEndArrearsSumTotal { get; private set; }
        public double YearEndArrearsSumLongterm { get; private set; }
        public double YearEndArrearsSumOverdue { get; private set; }
        public double LastSimilarPeriodArrearsSumTotal { get; private set; }
        public double LastSimilarPeriodArrearsSumLongterm { get; private set; }
        public double LastSimilarPeriodArrearsSumOverdue { get; private set; }

        public Structure(int id, string accountNumberPartOne, string accountNumberPartTwo, string accountNumberPartThree, string accountNumberPartFour,
                         double yearStartArrearsSumTotal, double yearStartArrearsSumLongterm, double yearStartArrearsOverdue, double arrearsChangeIncreaseTotal,
                         double arrearsChangeIncreaseNonmonetary, double arrearsChangeDecreaseTotal, double arrearsChangeDecreaseNonmonetary, double yearEndArrearsSumTotal,
                         double yearEndArrearsSumLongterm, double yearEndArrearsSumOverdue, double lastSimilarPeriodArrearsSumTotal, double lastSimilarPeriodArrearsSumLongterm,
                         double lastSimilarPeriodArrearsSumOverdue)
        {
            Id = id;
            AccountNumberPartOne = accountNumberPartOne;
            AccountNumberPartTwo = accountNumberPartTwo;
            AccountNumberPartThree = accountNumberPartThree;
            AccountNumberPartFour = accountNumberPartFour;
            YearStartArrearsSumTotal = yearStartArrearsSumTotal;
            YearStartArrearsSumLongterm = yearStartArrearsSumLongterm;
            YearStartArrearsOverdue = yearStartArrearsOverdue;
            ArrearsChangeIncreaseTotal = arrearsChangeIncreaseTotal;
            ArrearsChangeIncreaseNonmonetary = arrearsChangeIncreaseNonmonetary;
            ArrearsChangeDecreaseTotal = arrearsChangeDecreaseTotal;
            ArrearsChangeDecreaseNonmonetary = arrearsChangeDecreaseNonmonetary;
            YearEndArrearsSumTotal = yearEndArrearsSumTotal;
            YearEndArrearsSumLongterm = yearEndArrearsSumLongterm;
            YearEndArrearsSumOverdue = yearEndArrearsSumOverdue;
            LastSimilarPeriodArrearsSumTotal = lastSimilarPeriodArrearsSumTotal;
            LastSimilarPeriodArrearsSumLongterm = lastSimilarPeriodArrearsSumLongterm;
            LastSimilarPeriodArrearsSumOverdue = lastSimilarPeriodArrearsSumOverdue;
        }

        public bool CheckValue(int columnNumber, double value, string compareSign) 
        {
            double chosenValue;
            bool isTrue = false;
            chosenValue= ChooseHeader(columnNumber);
            switch (compareSign) 
            {
                case "=":
                    if(chosenValue == value) 
                    {
                        isTrue = true;
                    }
                break;
                case "<":
                    if (chosenValue < value)
                    {
                        isTrue = true;
                    }
                break;
                case ">":
                    if (chosenValue > value)
                    {
                        isTrue = true;
                    }
                break;
                case "!=":
                    if (chosenValue != value)
                    {
                        isTrue = true;
                    }
                    break;
                case ">=":
                    if (chosenValue >= value)
                    {
                        isTrue = true;
                    }
                break;
                case "<=":
                    if (chosenValue <= value)
                    {
                        isTrue = true;
                    }
                break;
            }

            return isTrue;
        }

        private double ChooseHeader(int columnNumber) 
        {
            double value = 0;
            switch (columnNumber) 
            {
                case 2:
                    value = YearStartArrearsSumTotal;
                break;
                case 3:
                    value = YearStartArrearsSumLongterm;
                break;
                case 4:
                    value = YearStartArrearsOverdue;
                break;
                case 5:
                    value = ArrearsChangeIncreaseTotal;
                break;
                case 6:
                    value = ArrearsChangeIncreaseNonmonetary;
                break;
                case 7:
                    value = ArrearsChangeDecreaseTotal;
                break;
                case 8:
                    value = ArrearsChangeDecreaseNonmonetary;
                break;
                case 9:
                    value = YearEndArrearsSumTotal;
                break;
                case 10:
                    value = YearEndArrearsSumLongterm;
                break;
                case 11:
                    value = YearEndArrearsSumOverdue;
                break;
                case 12:
                    value = LastSimilarPeriodArrearsSumTotal;
                break;
                case 13:
                    value = LastSimilarPeriodArrearsSumLongterm;
                break;
                case 14:
                    value = LastSimilarPeriodArrearsSumOverdue;
                break;
            }
            return value;
        }
    }
}
