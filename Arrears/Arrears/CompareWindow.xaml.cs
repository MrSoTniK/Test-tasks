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

namespace Arrears
{
    /// <summary>
    /// Логика взаимодействия для CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        public CompareWindow()
        {
            InitializeComponent();
        }

        private void FirstIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0)));
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlockForCompare.Text = String.Empty;
            int firstId, secondId;                    
            double two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen;
            bool isNumber = true;
            bool isFirstValueExist = false;
            bool isSecondValueExist = false;
            isNumber = int.TryParse(FirstIdTextBox.Text, out firstId);
            isNumber = int.TryParse(SecondIdTextBox.Text, out secondId);
            MainWindow mainWindow = new MainWindow();
            Structure firstRow = new Structure(0, "0", "0", "0", "0", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Structure secondRow = new Structure(0, "0", "0", "0", "0", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            if(isNumber == true) 
            {
                foreach (var arrear in (mainWindow.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList)
                {
                    if (arrear.Id == firstId)
                    {
                        firstRow = arrear;
                        isFirstValueExist = true;
                    }
                    if(arrear.Id == secondId) 
                    {
                        secondRow = arrear;
                        isSecondValueExist = true;
                    }
                    if(isFirstValueExist == true && isSecondValueExist == true)
                    {
                        break;
                    }
                }
                if (isFirstValueExist == true && isSecondValueExist == true) 
                {
                    two = firstRow.YearStartArrearsSumTotal - secondRow.YearStartArrearsSumTotal;
                    three = firstRow.YearStartArrearsSumLongterm - secondRow.YearStartArrearsSumLongterm;
                    four = firstRow.YearStartArrearsOverdue - secondRow.YearStartArrearsOverdue;
                    five = firstRow.ArrearsChangeIncreaseTotal - secondRow.ArrearsChangeIncreaseTotal;
                    six = firstRow.ArrearsChangeIncreaseNonmonetary - secondRow.ArrearsChangeIncreaseNonmonetary;
                    seven = firstRow.ArrearsChangeDecreaseTotal - secondRow.ArrearsChangeDecreaseTotal;
                    eight = firstRow.ArrearsChangeDecreaseNonmonetary - secondRow.ArrearsChangeDecreaseNonmonetary;
                    nine = firstRow.YearEndArrearsSumTotal - secondRow.YearEndArrearsSumTotal;
                    ten = firstRow.YearEndArrearsSumLongterm - secondRow.YearEndArrearsSumLongterm;
                    eleven = firstRow.YearEndArrearsSumOverdue - secondRow.YearEndArrearsSumOverdue;
                    twelve = firstRow.LastSimilarPeriodArrearsSumTotal - secondRow.LastSimilarPeriodArrearsSumTotal;
                    thirteen = firstRow.LastSimilarPeriodArrearsSumLongterm - secondRow.LastSimilarPeriodArrearsSumLongterm;
                    fourteen = firstRow.LastSimilarPeriodArrearsSumOverdue - secondRow.LastSimilarPeriodArrearsSumOverdue;

                    TextBlockForCompare.Text = "Результат разности строк: \n" + "Сумма задолженности на начало года всего (2): " + two.ToString() + "\nСумма задолженности на начало года долгосрочная (3): " + three.ToString() +
                                                "\nСумма задолженности на начало года просроченая (4): " + four.ToString() + "\nИзменение задолженности увеличение всего (5): " + five.ToString() + "\nИзменение задолженности увеличение неденежные (6): " + six.ToString() +
                                                "\nИзменение задолженности уменьшение всего (7): " + seven.ToString() + "\nИзменение задолженности уменьшение неденежные (8): " + eight.ToString() + "\nСумма задолженности на конец отчетного периода всего (9): " + nine.ToString() +
                                                "\nСумма задолженности на конец отчетного периода долгосрочная (10): " + ten.ToString() + "\nСумма задолженности на конец отчетного периода просроченная (11): " + eleven.ToString() + "\nСумма задолженности на конец аналогичного периода прошлого всего (12): " + twelve.ToString() +
                                                "\nСумма задолженности на конец аналогичного периода прошлого долгосрочная (13): " + thirteen.ToString() + "\nСумма задолженности на конец аналогичного периода прошлого просроченная (14): " + fourteen.ToString();
                }
                if(isFirstValueExist == false && isSecondValueExist == false) 
                {
                    MessageBox.Show("Ошибка.Обеих строк с такими ID не существует в БД.");
                }
                else if(isFirstValueExist == false) 
                {
                    MessageBox.Show("Ошибка. Первой строки с такими ID не существует в БД.");
                }
                else if (isSecondValueExist == false)
                {
                    MessageBox.Show("Ошибка. Второй строки с такими ID не существует в БД.");
                }
            }
            else 
            {
                MessageBox.Show("Ошибка.Заполните оба текстовых блока!");
            }
        }
    }
}
