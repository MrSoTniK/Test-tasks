using System;
using CSVConverter;

namespace DSotnikov.Appricot.Task
{
    public class Program
    {
        static void Main(string[] args)
        {
            using(AttributesSetter setter = new AttributesSetter()) 
            {
                setter.SetParameters(args);
                setter.SetNullValues();
                string path = setter.JsonFilePath;

                if (path != null && path.Contains(".json")) 
                {
                    using (JsonToCSVConverter converter = new JsonToCSVConverter())
                    {
                        converter.Convert(setter.JsonFilePath, setter.CsvFilePath, setter.Delimiter, setter.Encoding);
                        if (setter.isShowConsoleMessages) 
                        {
                            Console.WriteLine("Конвертация завершена. Нажмите любую кнопку для закрытия программы...");
                            Console.ReadLine();
                        }                           
                    }
                }
                else 
                {
                    Console.WriteLine("Не введен или не правильно введен аттрибут пути к json-файлу. Нажмите любую кнопку для закрытия программы...");
                    Console.ReadLine();
                }
            }
        }
    }   
}
