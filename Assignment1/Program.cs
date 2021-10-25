using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using static System.Diagnostics.Stopwatch;
using System.Collections.Generic;
using System.Collections;

namespace DirWalker
{
    public class DirWalker
    {
        public ArrayList FetchInput(String path, String pattern)
        {
            ArrayList all_files = new ArrayList();
            ArrayList fetched_files = new ArrayList();

            all_files = Traverse(path, all_files);
            foreach(string file in all_files)
            {
                if(MatchFile(file, pattern))
                {
                    fetched_files.Add(file);
                }
            }
            return fetched_files;
        }

        public ArrayList Traverse(String path, ArrayList files)
        {
            string[] list_path = Directory.GetDirectories(path);
            if (list_path == null) return files;
            foreach (string dirpath in list_path)
            {
                if (Directory.Exists(dirpath))
                {
                    Traverse(dirpath, files);
                }
            }
            string[] filelist = Directory.GetFiles(path);
            foreach (string filepath in filelist)
            {
                files.Add(filepath);
            }

            return files;
        }
        
        public bool MatchFile(string path_file, string regx_pattern)
        {
            MatchCollection mc = Regex.Matches(path_file, regx_pattern);
            return (mc.Count > 0);
        }

        public ArrayList IndexingEmptyVal(ArrayList field_, ArrayList loc_)
        {
            while (field_.Contains(""))
            {
                int tmp_loc = field_.IndexOf("");
                loc_.Add(tmp_loc);
                field_[tmp_loc] = "_";
                IndexingEmptyVal(field_, loc_);

            }
            return loc_;
        }

        public void ExpToTxt(string filename_, string msg_)
        {
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(msg_);
            using (FileStream fsWrite = new FileStream(filename_, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };

        }

        public Hashtable Parse(String inputfile, String logfile, String outputfile)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(inputfile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    ArrayList header = new ArrayList();
                    ArrayList count = new ArrayList();
                    int n_row = 0;
                    int n_valid = 0;
                    int n_invalid = 0;
                    
                    while (!parser.EndOfData)
                    {
                        //Process row
                        n_row++;
                        
                        if(n_row == 1)
                        {
                            header = new ArrayList(parser.ReadFields());
                            string msg = string.Join(", ", (string[])header.ToArray(typeof(string)));
                            msg += "\r\n";
                            ExpToTxt(outputfile, msg);
                        }
                        else
                        {
                            ArrayList fields = new ArrayList(parser.ReadFields());
                            if (fields.Contains(""))
                            {
                                // Invalid line
                                String msg_row = "Invalid Row: " + n_row + ", ";

                                n_invalid++;
                                ArrayList loc = new ArrayList();
                                loc = IndexingEmptyVal(fields, loc);
                                foreach (int i_loc in loc)
                                {
                                    string msg_field = "Field '" + header[i_loc] + "';";
                                    string msg = "File <" + inputfile + "> " + msg_row + msg_field + "\r\n";
                                    ExpToTxt(logfile, msg);
                                }
          
                            }
                            else
                            {
                                //Valid line
                                n_valid++;
                                ExpToTxt(outputfile, "File <" + inputfile + "> ");
                                string msg = string.Join(", ", (string[])fields.ToArray(typeof(string)));
                                msg += "\r\n";
                                ExpToTxt(outputfile, msg);

                            }

                        }

                    }
                    Hashtable n_val = new Hashtable();
                    n_val.Add("n_valid", n_valid);
                    n_val.Add("n_invalid", n_invalid);
                    return n_val;
                };

            }
            finally
            {
                Console.Write("");
            }
            
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            String path_input = @"D:\Sample Data";
            String file_pattern = "(?<=)[.\\sCustomerData\\S]*?(?=(.csv))";
            String file_log = @"D:\log.txt";
            String file_output = @"D:\Output.txt";

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DirWalker dw = new DirWalker();
            ArrayList inputfiles = dw.FetchInput(path_input, file_pattern);

            Hashtable total_n = new Hashtable();
            total_n.Add("n_valid", 0);
            total_n.Add("n_invalid", 0);

            foreach (String tmp_input in inputfiles)
            {
                Hashtable tmp_count = dw.Parse(tmp_input, file_log, file_output);
                int tmp_n_valid = (int)total_n["n_valid"] + (int)tmp_count["n_valid"];
                int tmp_n_invalid = (int)total_n["n_invalid"] + (int)tmp_count["n_invalid"];
                total_n["n_valid"] = tmp_n_valid;
                total_n["n_invalid"] = tmp_n_invalid;
            }

            string msg_n_valid = "Total number of rows valid = " + total_n["n_valid"] + "\r\n";
            string msg_n_invalid = "Total number of rows skipped = " + total_n["n_invalid"] + "\r\n";
            dw.ExpToTxt(file_log, msg_n_valid);
            dw.ExpToTxt(file_log, msg_n_invalid);

            sw.Stop();
            decimal runningtime = sw.ElapsedTicks / (decimal)System.Diagnostics.Stopwatch.Frequency;
            string msg_timer = "Running time = " + runningtime + "s\r\n";
            dw.ExpToTxt(file_log, msg_timer);

        }
    }
}
