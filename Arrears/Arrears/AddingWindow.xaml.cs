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
    /// Логика взаимодействия для AddingWindow.xaml
    /// </summary>
    public partial class AddingWindow : Window
    {
        public AddingWindow()
        {
            InitializeComponent();
        }

        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

        private void AddingButton_Click(object sender, RoutedEventArgs e)
        {
            bool isNotNull;
            string oneA, oneB, oneC, oneD;
            double two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen;
            MainWindow mainWindow = new MainWindow();

            oneA = TextBox1a.Text;
            oneB = TextBox1b.Text;
            oneC = TextBox1c.Text;
            oneD = TextBox1d.Text;
            isNotNull = double.TryParse(TextBox2.Text, out two);
            isNotNull = double.TryParse(TextBox3.Text, out three);
            isNotNull = double.TryParse(TextBox4.Text, out four);
            isNotNull = double.TryParse(TextBox5.Text, out five);
            isNotNull = double.TryParse(TextBox6.Text, out six);
            isNotNull = double.TryParse(TextBox7.Text, out seven);
            isNotNull = double.TryParse(TextBox8.Text, out eight);
            isNotNull = double.TryParse(TextBox9.Text, out nine);
            isNotNull = double.TryParse(TextBox10.Text, out ten);
            isNotNull = double.TryParse(TextBox11.Text, out eleven);
            isNotNull = double.TryParse(TextBox12.Text, out twelve);
            isNotNull = double.TryParse(TextBox13.Text, out thirteen);
            isNotNull = double.TryParse(TextBox14.Text, out fourteen);

            if(isNotNull == true) 
            {
                int id = 0;
                int quantity = (mainWindow.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList.Count;               
                if(quantity != 0) 
                {
                   id = (mainWindow.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList[quantity - 1].Id + 1;
                }
               
                Structure structure = new Structure(id, oneA, oneB, oneC, oneD, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen);
                (mainWindow.arrearsSfDataGrid.DataContext as ValuesModel).ArrearsList.Add(structure);

                using (ApplicationContext appContext = new ApplicationContext())
                {                    
                    appContext.DataBase.AddRange(structure);
                    appContext.SaveChanges();
                }

                Application.Current.MainWindow.Close();
                mainWindow.Show();
                this.Close();
            }
            else 
            {
                MessageBox.Show("Заполните пустой текстовый блок!");
            }
           
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0)));
        }    

        private void TextBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((TextBox)sender).Text)<1))));
        }
    }
}
