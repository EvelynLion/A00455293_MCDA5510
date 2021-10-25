# A00455293_MCDA5510

The C# program goes through the data.csv files and check the client info;
-- for completed / valid rows, it counted and recorded in output file;
-- for incompleted / invalid rows, it counted and recorded in logs;
-- also, there is a timer within the program, indicating the excuting time.
-- in summary, Total execution time – Total number of valid rows – Total number of skipped rows will be logged.

To improve its efficiency, the program will not check every value in these csv files. Instead, it iterates by row, detects empty value using Array.Contains() method, and locates the field of empty columns with Array.IndexOf() method.

In C#, we use stream to read and write files, which can be applied to generate the log file.

