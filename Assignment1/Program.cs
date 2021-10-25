using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using static System.Diagnostics.Stopwatch;

namespace DirWalker
{
    public class DirWalker
    {
        public bool matchfile(string path_file, string regx_pattern)
        {
            MatchCollection mc = Regex.Matches(path_file, regx_pattern);
            return (mc.Count > 0);
        }

        public void traverse(String path)
        {
            string[] list = Directory.GetDirectories(path);
            if (list == null) return;
            foreach (string dirpath in list)
            {
                if (Directory.Exists(dirpath))
                {
                    traverse(dirpath);
                    Console.WriteLine("Dir: " + dirpath);
                }
            }
            string[] filelist = Directory.GetFiles(path);
            foreach (string filepath in filelist)
            {
                bool file_matched = matchfile(filepath, "(?<=)[.\\s\\S]*?(?=(data.csv))");
                Console.WriteLine("File:" + filepath + "======>" + file_matched);
            }
        }



        public void parse(String filename)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(filename))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Process row
                        string[] fields = parser.ReadFields();

                        foreach (string field in fields)
                        {
                            Console.Write(field);
                            Console.Write(",");
                        }
                        Console.WriteLine("");
                    }
                }

            }
            finally
            {
                Console.WriteLine("");
            }

        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            String tmp_path = @"D:\empty";
            String tmp_file = @"D:\data.csv";
            string tmp_regx = "(?<=)[.\\s\\S]*?(?=(data.csv))";

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DirWalker dw = new DirWalker();

            dw.traverse(tmp_path);
            dw.parse(tmp_file);

            sw.Stop();
            Console.WriteLine("Running time = " + sw.ElapsedTicks / (decimal)System.Diagnostics.Stopwatch.Frequency + "s;");
        }
    }
}
