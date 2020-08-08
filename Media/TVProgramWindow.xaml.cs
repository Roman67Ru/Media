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

namespace Media
{
    /// <summary>
    /// Логика взаимодействия для TVProgramWindow.xaml
    /// </summary>
    public partial class TVProgramWindow : Window
    {
        ApplicationContext db;
        public TVProgramWindow()
        {
            InitializeComponent();
            db = new ApplicationContext();
            MainWindow main = new MainWindow();
            if (Param.idProgr != -1)
            {
               TVProgram program = db.TVPrograms.Find(Param.idProgr);
                nameTV.Text = program.name;
                infoTV.Text = program.info;
                actorsTV.Text = program.actors;
                yearTV.Text = program.year;
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow main = new MainWindow();
            if (Param.idProgr == -1)
            {
                if (nameTV.Text != "")
                {
                    var program = new TVProgram
                    {
                        name = nameTV.Text,
                        info = infoTV.Text,
                        actors = actorsTV.Text,
                        year = yearTV.Text
                    };
                    db.TVPrograms.Add(program);
                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Ошибка");
                }
              

            }
            else
            {
                TVProgram program = db.TVPrograms.Find(Param.idProgr);
                program.name = nameTV.Text;
                program.info = infoTV.Text;
                program.actors = actorsTV.Text;
                program.year = yearTV.Text;
                db.SaveChanges();
            }

            Param.idProgr = -1;
            nameTV.Text = "";
            infoTV.Text = "";
            actorsTV.Text = "";
            yearTV.Text = "";
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void YearTV_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Param.idProgr = -1;
            MainWindow main = new MainWindow();            
            main.Show();
            this.Hide();
        }
       
    }
}
