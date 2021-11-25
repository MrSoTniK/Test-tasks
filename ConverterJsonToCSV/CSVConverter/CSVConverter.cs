using System;
using System.IO;
using System.Text;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Data;
using Newtonsoft.Json;

namespace CSVConverter
{
    public interface IConverterToType<T> : IDisposable
    {
        DataTable Convert(T filePath);
    }

    public interface IWriterToFile<T> : IDisposable
    {
        void WriteToFile(T text, T filePath, Encoding encoding);
    }

    public interface IFormatterToFileFormat<T> : IDisposable
    {
        string Format(T text, string delimiter);
    }

    public interface IConverterAttributesSetter<T> : IDisposable
    {
        public string Delimiter { get; }
        public string JsonFilePath { get; }
        public string CsvFilePath { get;  }
        public Encoding Encoding { get;  }
        public bool isShowConsoleMessages { get; }

        void SetParameters(T text);
        void SetEncoding(string text);
        void SetNullValues();
    }

    public class AttributesSetter : IConverterAttributesSetter<string[]>
    {    
        public string Delimiter { get; private set; }
        public string JsonFilePath { get; private set; }
        public string CsvFilePath { get; private set; }
        public Encoding Encoding { get; private set; }
        public bool isShowConsoleMessages { get; private set; }

        public void SetParameters(string[] text)
        {
            isShowConsoleMessages = true;
            for (int i = 0; i < text.Length; i+=2) 
            {
                if((i + 1) < text.Length || text[i] == "-q") 
                {
                    switch (text[i])
                    {
                        case "-i":      //путь к входному файлу json
                            JsonFilePath = text[i + 1];
                            break;
                        case "-o":      //путь к выходному файлу csv через параметр
                            CsvFilePath = text[i + 1];
                            break;
                        case "-s":     //указание вида разделителя полей csv
                            Delimiter = text[i + 1];
                            break;
                        case "-e":     //вид выходной кодировки файла (например, utf8 или windows1251)
                            SetEncoding(text[i + 1]);
                            break;
                        case "-q":    //не выводить в консоль никакие сообщения и завершать программу без ожидания ввода с клавиатуры
                            isShowConsoleMessages = false;
                            break;
                    }
                }                        
            }
        }

        public void SetEncoding(string text)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            switch (text) 
            {
                case "UTF8":
                    Encoding = Encoding.UTF8;
                    break;
                case "ASCII":
                    Encoding = Encoding.ASCII;
                    break;              
                case "Unicode":
                    Encoding = Encoding.Unicode;
                    break;
                case "UTF32":
                    Encoding = Encoding.UTF32;
                    break;              
                case "windows1251":                    
                    Encoding = Encoding.GetEncoding("windows-1251");
                    break;
                default:
                    Encoding = Encoding.UTF8;
                    break;
            }
        }

        public void Dispose() {}

        public void SetNullValues()
        {
            if (Delimiter == null)
                Delimiter = ",";
            if (CsvFilePath == null)
                CsvFilePath = JsonFilePath + ".csv";
        }
    }

    public class JsonToDataTableConverter : IConverterToType<string>
    {
        public DataTable Convert(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream)) 
                {
                    string result = reader.ReadToEnd();
                    DataTable table = (DataTable)JsonConvert.DeserializeObject(result, (typeof(DataTable)));
                    reader.Close();
                    return table;
                }               
            }
        }
        public void Dispose() {}
    }

    public class FormatterJsonToCSV : IFormatterToFileFormat<DataTable>
    {
        public string Format(DataTable table, string delimiter)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
            };

            using (StringWriter writer = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(writer, config))
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        csvWriter.WriteField(column.ColumnName);
                    }
                    csvWriter.NextRecord();
                    csvWriter.Flush();
                    string result = writer.ToString();

                    foreach (DataRow row in table.Rows)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                            csvWriter.WriteField(row[i]);
                        csvWriter.NextRecord();
                        csvWriter.Flush();
                        result = writer.ToString();
                    }
                                      
                    return result;
                }
            }
        }

        public void Dispose(){}
    }

    public class WriterToFileCSV : IWriterToFile<string>
    {
        public void Dispose(){}

        public void WriteToFile(string text, string filePath, Encoding encoding)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream, encoding))
                {
                    writer.Write(text);
                }
            }
        }
    }

    public class JsonToCSVConverter : IDisposable
    {
        private IConverterToType<string> _converter;
        private IFormatterToFileFormat<DataTable> _formatter;
        private IWriterToFile<string> _writer;
        
        public void Convert(string jsonFilePath, string csvFilePath, string delimiter, Encoding encoding) 
        {
            using(_converter = new JsonToDataTableConverter()) 
            {
                using(_formatter = new FormatterJsonToCSV()) 
                {
                    using(_writer = new WriterToFileCSV()) 
                    {
                        string result =_formatter.Format(_converter.Convert(jsonFilePath), delimiter);
                        _writer.WriteToFile(result, csvFilePath, encoding);
                    }                    
                }
            }
        }
        public void Dispose(){}
    }
}
