using System;
using System.Collections.Generic;
using System.IO;
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

namespace BookMaker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int findIndexOfText = -1;  //Мертвая переменная, т.к. выделять слова она не захотела (Участвовала в прикольном алгоритме с выделением слов)
        int findIndexOfPage = -1;  //Брат мертвой переменной. Он молодец - работает.

        int selectedIndex = 0;  //Используется для переключения между страницами

        public MainWindow()
        {
            InitializeComponent();
            ReloadPages();  //При инициализации окна, загружаем учебник (из файлов)
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)  // Открытие окна редактирования учебника
        {
            new BookEditor().ShowDialog();
            ReloadPages();
        }

        void ReloadPages() // Метод обновления ListBox'а
        {
            if (!new DirectoryInfo(Environment.CurrentDirectory + @"\Book").Exists)
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Book");  //Проверка на наличие папки (а как следствие и файлов)

            listBox.Items.Clear(); // ЧИСТИ-ЧИСТИ-ЧИСТИ
            List<string> pages = Directory.GetFiles(Environment.CurrentDirectory + @"\Book", "*.txt").ToList(); // Временный массив, содержащий все файлы в папке учебника

            pages.ForEach(  //Заполняем ListBox из временного массива
                x =>
                {
                    FileInfo temp = new FileInfo(x);
                    listBox.Items.Add(new BookFile { FileName = temp.Name.Substring(0, temp.Name.Length - 4), FilePath = temp.FullName, FileContent = GetContenr(temp.FullName) });
                });//                                           (Обрезание имени файла)                         (Полный путь)           (Содержание файла)
             
            // Указываем ListBox'у, какое именно свойство класса нужно отобразить (Имя файла)
            listBox.DisplayMemberPath = "FileName";

        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)  // На DoubleClick по элементу ListBox открываем какую-либо страницу книги
        {
            if (listBox.SelectedIndex == -1)
                return;

            BookFile temp = (BookFile)listBox.SelectedItem;

            RTB.Text = temp.FileContent;

            selectedIndex = listBox.SelectedIndex;
        }

        string GetContenr(string file) //Считываем весь файл
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();

            }
        }

        private void MovePage(object sender, RoutedEventArgs e) // На метод ссылаются 2 кнопки (Назад, Вперед)
        {                                                       // Расчитывает страницу, которую нужно выделить в ListBox'е
            if (listBox.Items.Count < 2)                        // Ну и выводит её на экран в TextBox
                return;

            switch (((Button)sender).Content)
            {
                case "Назад":
                    if (selectedIndex - 1 < 0)
                    {
                        selectedIndex = listBox.Items.Count - 1;
                    }
                    else
                    {
                        selectedIndex--;
                    }
                    listBox.SelectedIndex = selectedIndex;

                    BookFile temp = (BookFile)listBox.Items[selectedIndex];
                    RTB.Text = temp.FileContent;
                    break;

                case "Вперед":
                    if (selectedIndex + 1 > listBox.Items.Count - 1)
                    {
                        selectedIndex = 0;
                    }
                    else
                    {
                        selectedIndex++;
                    }
                    listBox.SelectedIndex = selectedIndex;

                    BookFile temp2 = (BookFile)listBox.Items[selectedIndex];
                    RTB.Text = temp2.FileContent;
                    break;
            }
        }

        private void NullNullNull(object sender, TextChangedEventArgs e)  // Обнуляем поля, используемые Поиском (каждый раз, когда строка для поиска меняется)
        {
            findIndexOfText = 0;
            findIndexOfPage = 0;

        }

        private void FindPage(object sender, RoutedEventArgs e) //Метод для поиска нужно страницы
        {
            if (string.IsNullOrEmpty(Find.Text))  // Если текста нет, то ничего не делать не будем
                return;

            if (listBox.Items.Count < 0) // Если страниц нет, то и искать не нужно
                return;

            if (listBox.Items.Count - 1 < findIndexOfPage)  // Если мы просмотрели все странички, а найти ничего не удалось, то нужно за собой прибрать
            {
                MessageBox.Show("Поиск окончен");
                findIndexOfText = 0;
                findIndexOfPage = 0;
                return;
            }

            BookFile temp = (BookFile)listBox.Items[findIndexOfPage];  // Берем информацию по страницы, на которой будет вестись поиск

            if (temp.FileContent.IndexOf(Find.Text) >= 0) // Если искрмая строка имеется в этой странице, То открываем её.
            {
                listBox.SelectedIndex = findIndexOfPage;
                RTB.Text = temp.FileContent;
                findIndexOfPage++; // Теперь поиск будет вестись на другой странице

                //RTB.Select(temp.FileContent.IndexOf(Find.Text), Find.Text.Length);          // Чет он не захотел выделять нормально. Тут должен был работать поиск внутри файла
                //findIndexOfText = temp.FileContent.IndexOf(Find.Text) + Find.Text.Length;  //Чет не сработало красева
            }
            else       // Если нам не удалось найти на текущей страннице, То делаем рекурсию.Если нам не удалось найти на текущей страннице, То делаем рекурсию.Если нам не удалось найти на текущей страннице, То делаем рекурсию.Если нам не удалось найти на текущей страннице, То делаем рекурсию.
            {
                findIndexOfPage++;
                FindPage(null, null); //Чтобы понять рекурсию, нужно понять рекурсию побольше (Хе-Хе)
            }


        }

        // Методы блокирующие ввод в TextBox, в котором выводится учебник
        private void RTB_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            return;
        }

        // Методы блокирующие ввод в TextBox, в котором выводится учебник
        private void RTB_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            return;
        }
    }

}
