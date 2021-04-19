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
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace Arrears
{
    /// <summary>
    /// Логика взаимодействия для UploadWindow.xaml
    /// </summary>
    public partial class UploadWindow : Window
    {
        public UploadWindow()
        {
            InitializeComponent();
        }

        private void TextBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AddingWindow addWindow = new AddingWindow();
            e.Handled = !((Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (addWindow.DS_Count(((TextBox)sender).Text) < 1))));
        }

        private void XmlButton_Click(object sender, RoutedEventArgs e)
        {
            List<Structure> arrearsList = new List<Structure>();
            List<Structure> filteredArrears = new List<Structure>();
            string compareSign;
            int columnNumber;
            double value;
            bool isConditionSatisfied = true;

            using (ApplicationContext appContext = new ApplicationContext())
            {
                arrearsList = appContext.DataBase.ToList();
            }

            isConditionSatisfied = FilterArrears(ComboBox1, ComboBoxCompareSymbols1, TextBox1, out columnNumber, out compareSign, out value, arrearsList, filteredArrears);
            if(isConditionSatisfied == false) 
            {
                filteredArrears = arrearsList;
            }
            FilterArrears(ComboBox2, ComboBoxCompareSymbols2, TextBox2, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);
            FilterArrears(ComboBox3, ComboBoxCompareSymbols3, TextBox3, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);
            FilterArrears(ComboBox2, ComboBoxCompareSymbols4, TextBox4, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);
            FilterArrears(ComboBox5, ComboBoxCompareSymbols5, TextBox5, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);

            if(filteredArrears.Count != 0) 
            {
                string fileName = "arrears.xml";                

                XDocument xdoc = new XDocument();
                XElement root = new XElement("RootXml");

                XElement reportElement = new XElement("Report");
                XAttribute albumCode = new XAttribute("AlbumCode","MEC_K");
                XAttribute code = new XAttribute("Code", "042");
                reportElement.Add(albumCode);
                reportElement.Add(code);

                XElement form = new XElement("FormVariant");
                XAttribute nsiVariant = new XAttribute("NsiVariantCode", "0000");
                XAttribute number = new XAttribute("Number", "1");
                form.Add(nsiVariant);
                form.Add(number);

                XElement table = new XElement("Table");
                XAttribute stringCode = new XAttribute("Code", "Строка");
                table.Add(stringCode);

                XElement data;
                XAttribute account;
                XAttribute classification;
                double sum2 = 0, sum3 = 0, sum4 = 0, sum5 = 0, sum6 = 0, sum7 = 0, sum8 = 0, sum9 = 0, sum10 = 0, sum11 = 0, sum12 = 0, sum13 = 0, sum14 = 0;
                foreach(Structure arrear in filteredArrears) 
                {
                    data = new XElement("Data");
                    account = new XAttribute("СинСчет", arrear.AccountNumberPartThree.ToString());
                    classification = new XAttribute("КОСГУ", arrear.AccountNumberPartFour.ToString());
                    data.Add(account);
                    data.Add(classification);
                    AddDataTag(CheckBox2, data, arrear.YearStartArrearsSumTotal, "_x2", sum2, out sum2);
                    AddDataTag(CheckBox3, data, arrear.YearStartArrearsSumLongterm, "_x3", sum3, out sum3);
                    AddDataTag(CheckBox4, data, arrear.YearStartArrearsOverdue, "_x4", sum4, out sum4);
                    AddDataTag(CheckBox2, data, arrear.ArrearsChangeIncreaseTotal, "_x5", sum5, out sum5);
                    AddDataTag(CheckBox6, data, arrear.ArrearsChangeIncreaseNonmonetary, "_x6", sum6, out sum6);
                    AddDataTag(CheckBox7, data, arrear.ArrearsChangeDecreaseTotal, "_x7", sum7, out sum7);
                    AddDataTag(CheckBox8, data, arrear.ArrearsChangeDecreaseNonmonetary, "_x8", sum8, out sum8);
                    AddDataTag(CheckBox9, data, arrear.YearEndArrearsSumTotal, "_x9", sum9, out sum9);
                    AddDataTag(CheckBox10, data, arrear.YearEndArrearsSumLongterm, "_x10", sum10, out sum10);
                    AddDataTag(CheckBox11, data, arrear.YearEndArrearsSumOverdue, "_x11", sum11, out sum11);
                    AddDataTag(CheckBox12, data, arrear.LastSimilarPeriodArrearsSumTotal, "_x12", sum12, out sum12);
                    AddDataTag(CheckBox13, data, arrear.LastSimilarPeriodArrearsSumLongterm, "_x13", sum13, out sum13);
                    AddDataTag(CheckBox14, data, arrear.LastSimilarPeriodArrearsSumOverdue, "_x14", sum14, out sum14);

                    if (data.Attributes().ToList().Count > 2) 
                    {
                        table.Add(data);
                    }                    
                }

                data = new XElement("Data");
                account = new XAttribute("СинСчет", "88888");
                classification = new XAttribute("КОСГУ", "888");
                data.Add(account);
                data.Add(classification);
                XAttribute sumX2 = new XAttribute("_x2", sum2.ToString());
                data.Add(sumX2);
                XAttribute sumX3 = new XAttribute("_x3", sum3.ToString());
                data.Add(sumX3);
                XAttribute sumX4 = new XAttribute("_x4", sum4.ToString());
                data.Add(sumX4);
                XAttribute sumX5 = new XAttribute("_x5", sum5.ToString());
                data.Add(sumX5);
                XAttribute sumX6 = new XAttribute("_x6", sum6.ToString());
                data.Add(sumX6);
                XAttribute sumX7 = new XAttribute("_x7", sum7.ToString());
                data.Add(sumX7);
                XAttribute sumX8 = new XAttribute("_x8", sum8.ToString());
                data.Add(sumX8);
                XAttribute sumX9 = new XAttribute("_x9", sum9.ToString());
                data.Add(sumX9);
                XAttribute sumX10 = new XAttribute("_x10", sum10.ToString());
                data.Add(sumX10);
                XAttribute sumX11 = new XAttribute("_x11", sum11.ToString());
                data.Add(sumX11);
                XAttribute sumX12 = new XAttribute("_x12", sum12.ToString());
                data.Add(sumX12);
                XAttribute sumX13 = new XAttribute("_x13", sum13.ToString());
                data.Add(sumX13);
                XAttribute sumX14 = new XAttribute("_x14", sum14.ToString());
                data.Add(sumX14);

                table.Add(data);
                form.Add(table);
                reportElement.Add(form);
                root.Add(reportElement);

                xdoc.Add(root);
                xdoc.Save(fileName);

                filteredArrears.Clear();
            }
        }

        private void AddDataTag(CheckBox checkBox, XElement data, double arrearsElement, string x, double previousSum, out double currentSum)  
        {
            currentSum = previousSum;
            if (checkBox.IsChecked == true && arrearsElement > 0)
            {                  
                XAttribute value = new XAttribute(x, arrearsElement.ToString());
                currentSum += arrearsElement;
                data.Add(value);
            }
        }
        private bool FilterArrears(ComboBox comboBox1, ComboBox comboBox2, TextBox textBox, out int columnNumber, out string compareSign, out double value, List<Structure> arrearsList, List<Structure> filteredArrears) 
        {
            bool isSuccess = false, isConditionSatisfied = true;
            columnNumber = 0;
            compareSign = "";
            value = 0;
            if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null && textBox.Text != null)
            {
                ComboBoxItem item1 = (ComboBoxItem)comboBox1.SelectedItem;
                string name1 = item1.Content.ToString();
                int.TryParse(name1, out columnNumber);

                ComboBoxItem item2 = (ComboBoxItem)comboBox2.SelectedItem;
                compareSign = item2.Content.ToString();

                double.TryParse(textBox.Text, out value);
                List<Structure> filter = arrearsList;
                filteredArrears.Clear();

                foreach (Structure arrear in filter)
                {
                    isSuccess = arrear.CheckValue(columnNumber, value, compareSign);
                    if (isSuccess == true)
                    {
                        filteredArrears.Add(arrear);
                    }
                }               
            }
            else 
            {
                isConditionSatisfied = false;
            }

            return isConditionSatisfied;
        }

        private void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            List<Structure> arrearsList = new List<Structure>();
            List<Structure> filteredArrears = new List<Structure>();
            string compareSign;
            int columnNumber;
            double value;
            bool isConditionSatisfied = true;

            using (ApplicationContext appContext = new ApplicationContext())
            {
                arrearsList = appContext.DataBase.ToList();
            }

            isConditionSatisfied = FilterArrears(ComboBox1, ComboBoxCompareSymbols1, TextBox1, out columnNumber, out compareSign, out value, arrearsList, filteredArrears);
            if (isConditionSatisfied == false)
            {
                filteredArrears = arrearsList;
            }
            FilterArrears(ComboBox2, ComboBoxCompareSymbols2, TextBox2, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);
            FilterArrears(ComboBox3, ComboBoxCompareSymbols3, TextBox3, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);
            FilterArrears(ComboBox2, ComboBoxCompareSymbols4, TextBox4, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);
            FilterArrears(ComboBox5, ComboBoxCompareSymbols5, TextBox5, out columnNumber, out compareSign, out value, filteredArrears, filteredArrears);

            if(filteredArrears.Count != 0) 
            {                
                Excel.Application excel = new Excel.Application { Visible = true };
                
                Excel.Workbook workBook;
                Excel.Worksheet workSheet;
               // Excel.Workbooks 
                workBook = excel.Workbooks.Add();
                workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);
                workSheet.Cells.HorizontalAlignment = Excel.Constants.xlCenter;
                workSheet.Cells.VerticalAlignment = Excel.Constants.xlCenter;
                
                Excel.Range rng1 = (Excel.Range)workSheet.get_Range("A1", "A4").Cells;
                rng1.Merge();               
                workSheet.Cells[1, 1] = "Номер (код) счета бюджетного учета";              
                workSheet.Cells[5, 1] = "1";

                List<CheckBox> checkBoxes = new List<CheckBox>();
                checkBoxes.Add(CheckBox2);
                checkBoxes.Add(CheckBox3);
                checkBoxes.Add(CheckBox4);
                checkBoxes.Add(CheckBox5);
                checkBoxes.Add(CheckBox6);
                checkBoxes.Add(CheckBox7);
                checkBoxes.Add(CheckBox8);
                checkBoxes.Add(CheckBox9);
                checkBoxes.Add(CheckBox10);
                checkBoxes.Add(CheckBox11);
                checkBoxes.Add(CheckBox12);
                checkBoxes.Add(CheckBox13);
                checkBoxes.Add(CheckBox14);
                List<double> sums = new List<double>();
                while(sums.Count < 13) 
                {
                    sums.Add(0);
                }

                int keyHeaderId = 6;
                foreach(Structure arrear in filteredArrears) 
                {
                    workSheet.Cells[keyHeaderId, 1] = arrear.AccountNumberPartOne + " " + arrear.AccountNumberPartTwo + " " + arrear.AccountNumberPartThree + " " + arrear.AccountNumberPartFour;
                    List<double> values = new List<double>();
                    values.Add(arrear.YearStartArrearsSumTotal);
                    sums[0] += arrear.YearStartArrearsSumTotal;
                    values.Add(arrear.YearStartArrearsSumLongterm);
                    sums[1] += arrear.YearStartArrearsSumLongterm;
                    values.Add(arrear.YearStartArrearsOverdue);
                    sums[2] += arrear.YearStartArrearsOverdue;
                    values.Add(arrear.ArrearsChangeIncreaseTotal);
                    sums[3] += arrear.ArrearsChangeIncreaseTotal;
                    values.Add(arrear.ArrearsChangeIncreaseNonmonetary);
                    sums[4] += arrear.ArrearsChangeIncreaseNonmonetary;
                    values.Add(arrear.ArrearsChangeDecreaseTotal);
                    sums[5] += arrear.ArrearsChangeDecreaseTotal;
                    values.Add(arrear.ArrearsChangeDecreaseNonmonetary);
                    sums[6] += arrear.ArrearsChangeDecreaseNonmonetary;
                    values.Add(arrear.YearEndArrearsSumTotal);
                    sums[7] += arrear.YearEndArrearsSumTotal;
                    values.Add(arrear.YearEndArrearsSumLongterm);
                    sums[8] += arrear.YearEndArrearsSumLongterm;
                    values.Add(arrear.YearEndArrearsSumOverdue);
                    sums[9] += arrear.YearEndArrearsSumOverdue;
                    values.Add(arrear.LastSimilarPeriodArrearsSumTotal);
                    sums[10] += arrear.LastSimilarPeriodArrearsSumTotal;
                    values.Add(arrear.LastSimilarPeriodArrearsSumLongterm);
                    sums[11] += arrear.LastSimilarPeriodArrearsSumLongterm;
                    values.Add(arrear.LastSimilarPeriodArrearsSumOverdue);
                    sums[12] += arrear.LastSimilarPeriodArrearsSumOverdue;
                    AddValues(checkBoxes, values, workSheet, keyHeaderId, out keyHeaderId);
                }
              
                workSheet.Cells[keyHeaderId, 1] = "Всего:";                
                int sumId = 0;
                int columnId = 2;
                int cellsQuantity = 0;
                foreach (CheckBox checkBox in checkBoxes) 
                {
                    if (checkBox.IsChecked == true) 
                    {
                        workSheet.Cells[keyHeaderId, columnId] = sums[sumId];
                        columnId++;
                        cellsQuantity++;
                    }
                    sumId++;
                }

                cellsQuantity++;
                if(cellsQuantity > 1) 
                {
                    workSheet.Range[workSheet.Cells[1, 2], workSheet.Cells[1, cellsQuantity]].Merge();
                    workSheet.Cells[1, 2] = "Сумма задолженности, руб";
                }

                int position = 0;
                AddHeaders(workSheet, checkBoxes, position);

                workSheet.Columns.AutoFit();
                workSheet.Rows.AutoFit();
                excel.Quit();               
            }
        }

        private void AddValues(List<CheckBox> checkBoxes, List<double> values, Excel.Worksheet workSheet, int previousRowId, out int currentRowId) 
        {
            int count = 0;
            int columnId = 2;
            int headerId = 2;

            currentRowId = previousRowId;
            foreach (CheckBox checkBox in checkBoxes) 
            { 
                if(checkBox.IsChecked == true) 
                {
                    workSheet.Cells[5, columnId] = headerId.ToString();
                    if(values[count] > 0) 
                    {
                        workSheet.Cells[currentRowId, columnId] = values[count];
                    }
                    else 
                    {
                        workSheet.Cells[currentRowId, columnId] = "-";
                    }                    
                    columnId++;                    
                }
                count++;
                headerId++;
            }
            currentRowId++;
        }

        private void AddBeginOfTheYearHeader(Excel.Worksheet workSheet, List<CheckBox> checkBoxes, out int position) 
        {
            position = 0;

            for (int i = 0; i <= 2; i++)
            {
                if (checkBoxes[i].IsChecked == true) 
                {
                    position++;
                }
            }           

            switch (position) 
            {
                case 1:
                    workSheet.Cells[2, 2] = "на начало года";
                break;
                case 2:
                    Excel.Range rng1 = (Excel.Range)workSheet.get_Range("B2", "C2").Cells;
                    rng1.Merge();
                    workSheet.Cells[2, 2] = "на начало года";
                break;
                case 3:
                    Excel.Range rng2 = (Excel.Range)workSheet.get_Range("B2", "D2").Cells;
                    rng2.Merge();
                    workSheet.Cells[2, 2] = "на начало года";
                break;
            }
        }

        private int AddArrearChangeHeader(Excel.Worksheet workSheet, List<CheckBox> checkBoxes, int previousPosition) 
        {
            int position = 0;

            for (int i = 3; i <= 6; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    position++;
                }
            }

            AddBeginOfTheYearHeader(workSheet, checkBoxes, out previousPosition);          
            previousPosition+=2;           
          
            switch (position)
            {
                case 1:
                    workSheet.Cells[2, previousPosition] = "изменение задолженности";
                    break;
                case 2:
                    previousPosition++;
                    workSheet.Range[workSheet.Cells[2, previousPosition-1], workSheet.Cells[2, previousPosition]].Merge();
                    workSheet.Cells[2, previousPosition-1] = "изменение задолженности";
                    previousPosition++;
                    break;
                case 3:
                    previousPosition += 2;
                    workSheet.Range[workSheet.Cells[2, previousPosition-2], workSheet.Cells[2, previousPosition]].Merge();
                    workSheet.Cells[2, previousPosition-2] = "изменение задолженности";
                    previousPosition++;
                    break;
                case 4:
                    previousPosition += 3;
                    workSheet.Range[workSheet.Cells[2, previousPosition - 3], workSheet.Cells[2, previousPosition]].Merge();
                    workSheet.Cells[2, previousPosition - 3] = "изменение задолженности";
                    previousPosition++;
                    break;
            }

            return previousPosition;
        }

        private int AddEndPeriodHeader(Excel.Worksheet workSheet, List<CheckBox> checkBoxes, int previousPosition)
        {
            int position = 0;
            previousPosition = AddArrearChangeHeader(workSheet, checkBoxes, previousPosition);

            for (int i = 7; i <= 9; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    position++;
                }
            }

            switch (position) 
            {
                case 1:
                    workSheet.Cells[2, previousPosition] = "на конец отчетного периода";
                    previousPosition++;
                    break;
                case 2:
                    workSheet.Range[workSheet.Cells[2, previousPosition], workSheet.Cells[2, previousPosition + 1]].Merge();
                    workSheet.Cells[2, previousPosition] = "на конец отчетного периода";
                    previousPosition += 2;
                    break;
                case 3:
                    workSheet.Range[workSheet.Cells[2, previousPosition], workSheet.Cells[2, previousPosition + 2]].Merge();
                    workSheet.Cells[2, previousPosition] = "на конец отчетного периода";
                    previousPosition += 3;
                    break;
            }

            return previousPosition;
        }

        private void AddHeaders(Excel.Worksheet workSheet, List<CheckBox> checkBoxes, int previousPosition) 
        {
            int position = 0;
            previousPosition = AddEndPeriodHeader(workSheet, checkBoxes, previousPosition);

            for (int i = 10; i <= 12; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    position++;
                }
            }

            switch (position) 
            {
                case 1:
                    workSheet.Cells[2, previousPosition] = "на конец аналогичного периода прошлого";
                    break;
                case 2:
                    workSheet.Range[workSheet.Cells[2, previousPosition], workSheet.Cells[2, previousPosition + 1]].Merge();
                    workSheet.Cells[2, previousPosition] = "на конец аналогичного периода прошлого";
                    break;
                case 3:
                    workSheet.Range[workSheet.Cells[2, previousPosition], workSheet.Cells[2, previousPosition + 2]].Merge();
                    workSheet.Cells[2, previousPosition] = "на конец аналогичного периода прошлого";
                    break;
            }   
            
            for(int i = 2; i<= 14; i++) 
            {
                double val = 0;
                if(workSheet.Cells[5, i].Value != null) 
                {
                    workSheet.Range[workSheet.Cells[3, i], workSheet.Cells[4, i]].Merge();
                    val = workSheet.Cells[5, i].Value;
                }
                
                switch (val) 
                {
                    case 2:             
                        workSheet.Cells[3, i] = "всего";
                        break;
                    case 3:
                        workSheet.Cells[3, i] = "долгосрочная";
                        break;
                    case 4:
                        workSheet.Cells[3, i] = "просроченная";
                        break;
                    case 5:
                        workSheet.Cells[3, i] = "увеличение, всего";
                        break;
                    case 6:
                        workSheet.Cells[3, i] = "увеличение, в том числе неденежные";
                        break;
                    case 7:
                        workSheet.Cells[3, i] = "уменьшение, всего";
                        break;
                    case 8:
                        workSheet.Cells[3, i] = "уменьшение, в том числе неденежные";
                        break;
                    case 9:
                        workSheet.Cells[3, i] = "всего";
                        break;
                    case 10:
                        workSheet.Cells[3, i] = "долгосрочная";
                        break;
                    case 11:
                        workSheet.Cells[3, i] = "просроченная";
                        break;
                    case 12:
                        workSheet.Cells[3, i] = "всего";
                        break;
                    case 13:
                        workSheet.Cells[3, i] = "долгосрочная";
                        break;
                    case 14:
                        workSheet.Cells[3, i] = "просроченная";
                        break;
                }
            }
        }


    }
}
