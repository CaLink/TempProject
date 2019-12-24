using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookMaker
{
    /// <summary>
    /// Логика взаимодействия для BookEditor.xaml
    /// </summary>
    public partial class BookEditor : Window
    {
        public BookEditor()
        {
            InitializeComponent();

            ReloadPages(); // Подгружаем доступные книги
        }


        private void Button_Click(object sender, RoutedEventArgs e) // Соранение страницы
        {
            if (string.IsNullOrEmpty(PageName.Text.Trim()) || string.IsNullOrEmpty(RTB.Text))  //Если пусто, то не сохраняем
            {
                MessageBox.Show("Придумайте название или Заполните страницу");
                return;
            }

            try
            {
                if (new FileInfo(Environment.CurrentDirectory + @"\Book\" + PageName.Text + ".txt").Exists) // Если имя занято, то не сохраняем
                {
                    MessageBox.Show("Такое имя уже используется. Придумайте другое");
                    return;
                }

                using (FileStream fs = new FileStream(Environment.CurrentDirectory + @"\Book\" + "Глава " + (listBox.Items.Count+1) + " - " + PageName.Text + ".txt", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))  // Сохраняем файл
                {
                    sw.Write(RTB.Text);
                }
            }
            catch { MessageBox.Show("Нельзя использовать такое название главы"); } // Если кто-то не правильное имя ввел, то не пускаем его

            
            ReloadPages(); // Обновляем список
        }

        void ReloadPages() // Ну обновление списка метод
        {
            if (!new DirectoryInfo(Environment.CurrentDirectory + @"\Book").Exists)
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Book");

            listBox.Items.Clear();
            List<string> pages = Directory.GetFiles(Environment.CurrentDirectory + @"\Book", "*.txt").ToList();

            pages.ForEach(
                x =>
                {
                    FileInfo temp = new FileInfo(x);
                    listBox.Items.Add(new BookFile { FileName = temp.Name.Substring(0, temp.Name.Length - 4), FilePath = temp.FullName, FileContent = GetContenr(temp.FullName) });
                });


            listBox.DisplayMemberPath = "FileName";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Удаление странницы учебника (и его файла)
        {
            if (listBox.SelectedIndex == -1)
                return;

            File.Delete(((BookFile)listBox.SelectedItem).FilePath);

            ReloadPages();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) // Кнопка добавить странницу. Чистит все, что можно
        {
            PageName.Text = "";
            RTB.Text = "";
            listBox.SelectedIndex = -1;
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)  // Открытие страницы
        {
            if (listBox.SelectedIndex == -1)
                return;

            BookFile temp = (BookFile)listBox.SelectedItem;
            PageName.Text = temp.FileName;

            RTB.Text = temp.FileContent;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // Кнопка "Сохранить", которая ничего не сохраняет (Но пользователь не знает об этом)
        {
            this.Close();
        }

        private void Button_Click1(object sender, RoutedEventArgs e) // Метод для изменения главы. Почти тоже, что и с сохранением, только тут позволятеся перезаписать файл
        {
            if (listBox.SelectedIndex == -1)
                return;

            if (string.IsNullOrEmpty(PageName.Text.Trim()) || string.IsNullOrEmpty(RTB.Text))
            {
                MessageBox.Show("Заполните страницу");
                return;
            }

            var temp = (BookFile)listBox.SelectedItem;


            try
            {
                new FileInfo(Environment.CurrentDirectory + @"\Book\" + "Глава " + (listBox.SelectedIndex + 1) + " - " + PageName.Text + ".txt"); 
                File.Delete(temp.FilePath); // Генератор ошибки, чтобы правильно работала последовательность кода

                using (FileStream fs = new FileStream(Environment.CurrentDirectory + @"\Book\" + "Глава "+ (listBox.SelectedIndex +1)+ " - "+ PageName.Text + ".txt", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(RTB.Text);
                }
                ReloadPages();
            }
            catch { MessageBox.Show("Нельзя использовать такое название главы"); }
        }

        string GetContenr(string file) // Получаем содержимое файла
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
