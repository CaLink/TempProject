using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMaker
{
    public class BookFile
    {
        // Это класс для хранения информации о файле.
        // Его объекты буду находится в ListBox

        public string FileName { get; set; }  //Имя файла без расширения (.xxx)
        public string FilePath { get; set; } //Полный путь к файлу
        public string FileContent { get; set; } //Тут содержание файла

    }
}
