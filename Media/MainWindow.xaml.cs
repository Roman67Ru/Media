using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Media
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public static class Param
    {
        public static int idProgr = -1;
    }
    public partial class MainWindow : Window
    {
        ApplicationContext db;
        public MainWindow()
        {
            InitializeComponent();
            db = new ApplicationContext();

            UpdateListProgram();

        }
       
        public void UpdateListProgram()
        {
            try
            {
                ListProgram.ItemsSource = db.TVPrograms.Select(x => new { x.name, x.idProgram }).ToList();
                ListProgram.DisplayMemberPath = "name";
                ListProgram.SelectedValuePath = "idProgram";
                ListProgram.SelectedIndex = 0;

                UpdateListVideo();
            }
            catch (Exception) { }
        }

        public void UpdateListVideo()
        {
            try
            {
                int idP = Convert.ToInt32(ListProgram.SelectedValue.ToString());
                ListVideo.ItemsSource = db.VideoCards.Where(x => x.idProgram == idP).Select(x => new { x.name, x.idVideo }).ToList();
                ListVideo.DisplayMemberPath = "name";
                ListVideo.SelectedValuePath = "idVideo";
                ListVideo.SelectedIndex = 0;

                UpdateVideo();
            }
            catch (Exception) { }
        }

        public void UpdateVideo()
        {
            try
            {
                int idV = Convert.ToInt32(ListVideo.SelectedValue.ToString());
                VideoCards video = db.VideoCards.Find(idV);

                NameVideo.Content = video.name;
                PathVideo.Content = video.path;
                FrameCount.Content = video.timing;
                DisplayAspectRatio.Content = video.format;
                SizeCadr.Content = video.size;

                media.Source = new Uri(video.path);
            }
            catch (Exception) {
                NameVideo.Content = "";
                PathVideo.Content = "";
                FrameCount.Content = "";
                DisplayAspectRatio.Content = "";
                SizeCadr.Content = "";

                media.Source = null;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TVProgramWindow tV = new TVProgramWindow();
            tV.Show();
            this.Hide();
          
        }


        private void ListProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(ListProgram.SelectedValue.ToString());

                var Program = db.TVPrograms.Find(id);
                LabelName.Content = Program.name;
                LabelInfo.Text = Program.info;
                LabelActors.Content = Program.actors;
                LabelYear.Content = Program.year+"г.";
            }
            catch (Exception) { };

            UpdateListVideo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ListProgram.SelectedValue != null)
            {
                int IdP = Convert.ToInt32(ListProgram.SelectedValue.ToString());
                while (db.VideoCards.Where(x => x.idProgram == IdP).Count() != 0)
                {
                    VideoCards video = db.VideoCards.Where(x => x.idProgram == IdP).First();

                    try
                    {
                        System.IO.File.Delete(video.path);
                    }
                    catch (Exception) { }

                    db.VideoCards.Remove(video);
                    db.SaveChanges();
                }

                TVProgram tV = (TVProgram)db.TVPrograms.Find(IdP);
                db.TVPrograms.Remove(tV);
                db.SaveChanges();
                MessageBox.Show("ok");

                UpdateListProgram();
            }
            else
            {
                MessageBox.Show("Ошибка, проверьте наличие данных");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void Grid_Drop(object sender, DragEventArgs e)
        {
            

                String[] path = (String[])e.Data.GetData(System.Windows.DataFormats.FileDrop, true);
                String FileName = System.IO.Path.GetFileName(path[0].ToString());
                if (FileName.Length > 0)
                {

                    String VideoPath = path[0].ToString();

                   string NewName = string.Format(@"{0}", Guid.NewGuid());
                    NewName = NewName + System.IO.Path.GetExtension(VideoPath);

                    string path2 = Environment.CurrentDirectory + "\\Video\\" + NewName;

                //если название файла на русском выдаст ошибку... Придется временно сохранить
                //(кавычки не помогают, батник дает тот же результат, скопированная команда в ручную выполняется успешно)
                File.Copy(VideoPath, path2, true);
                    media.Source = new Uri(VideoPath);
                    
                    string go = "cd " + Environment.CurrentDirectory+" && MediaInfo --LogFile=Test.json \"" + path2 + "\" --Output=JSON";
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            RedirectStandardInput = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };


                    process.Start();
                    using (StreamWriter pWriter = process.StandardInput)
                    {
                        if (pWriter.BaseStream.CanWrite)
                        {
                            pWriter.WriteLine(go);
                        }
                    }
                 try
                 {

                Thread.Sleep(1000);
                // удаляем временный файл
                System.IO.File.Delete(path2);
 
                
                    string pathJson = Environment.CurrentDirectory + "\\Test.json";
                    string Text = File.ReadAllText(pathJson);
                    var json = JToken.Parse(Text);

                    NameVideo.Content = FileName;
                    PathVideo.Content = VideoPath;
                    FrameCount.Content = json["media"]["track"][1]["FrameCount"].ToString();
                    DisplayAspectRatio.Content = json["media"]["track"][1]["DisplayAspectRatio"].ToString();
                    int a = Convert.ToInt32(json["media"]["track"][1]["Width"].ToString());
                    int b = Convert.ToInt32(json["media"]["track"][1]["Height"].ToString());
                    int c = Convert.ToInt32(json["media"]["track"][1]["BitDepth"].ToString());
                    int result = a * b * c;
                    SizeCadr.Content = Convert.ToString(result);

                    System.IO.File.Delete(pathJson);
                }
                catch (Exception) { MessageBox.Show("Приносим свои извенения, программа не допускает наличие в пути русских букв, переместите корневую папку, расположение: " + Environment.CurrentDirectory); }

            }
            e.Handled = true;
          }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if((db.VideoCards.Where(x=>x.path== PathVideo.Content.ToString()).Count() == 0)&&(PathVideo.Content.ToString()!=""))
            {
                if (ListProgram.SelectedValue != null)
                {
                    string VideoPath = PathVideo.Content.ToString();
                    string NewName = string.Format(@"{0}", Guid.NewGuid());
                    NewName = NewName + System.IO.Path.GetExtension(PathVideo.Content.ToString());
                    string path2 = Environment.CurrentDirectory + "\\Video\\" + NewName;

                    File.Copy(VideoPath, path2, true);

                    VideoCards video = new VideoCards
                    {
                        name = NameVideo.Content.ToString(),
                        path = path2,
                        timing = FrameCount.Content.ToString(),
                        format = DisplayAspectRatio.Content.ToString(),
                        size = SizeCadr.Content.ToString(),
                        idProgram = Convert.ToInt32(ListProgram.SelectedValue.ToString())
                    };
                    db.VideoCards.Add(video);
                    db.SaveChanges();
                    PathVideo.Content = path2;
                    MessageBox.Show("ok");
                    UpdateListVideo();
                }
                else
                {
                    MessageBox.Show("Ошибка, не выбрана передача");
                }
            }
            else
            {
                MessageBox.Show("Переместите видео в центральную область");
            }
        }

        private void Media_Loaded(object sender, RoutedEventArgs e)
        {

            media.Play();
            Thread.Sleep(1000);
            media.Pause();
        }

        private void Media_Drop(object sender, DragEventArgs e)
        {
          
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            media.Play();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            media.Pause();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            media.Stop();
            media.Play();
            Thread.Sleep(200);
            media.Pause();
            media.Stop();
        }

        private void ListVideo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateVideo();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                int Id = Convert.ToInt32(ListVideo.SelectedValue.ToString());

                VideoCards video = db.VideoCards.Find(Id);
                System.IO.File.Delete(video.path);
                db.VideoCards.Remove(video);
                db.SaveChanges();
                MessageBox.Show("ok");
                UpdateListVideo();
            }
            catch (Exception) { MessageBox.Show("Ошибка, проверьте наличие данных"); }
          
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (ListProgram.SelectedValue != null)
            {
                Param.idProgr = Convert.ToInt32(ListProgram.SelectedValue.ToString());
                TVProgramWindow tV = new TVProgramWindow();
                tV.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Ошибка, проверьте наличие данных");
            }
            
        }
     
    }
}
