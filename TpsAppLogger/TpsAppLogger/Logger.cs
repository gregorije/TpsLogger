using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TpsAppLogger
{
    public class Logger
    {
        private int MinutesToEmptyFile { get; set; }
        private string LogSeparator { get; set; }
        public bool TimeClean { get; private set; }
        public bool SizeClean { get; private set; }
        public int MinutesClean { get; private set; }
        public int KilobytesSize { get; private set; }

    
        public Logger(bool time_clean, bool size_clean, int minutes_clean, int kilobytes_clean)
        {
            LogSeparator = "*************************************************";
            TimeClean = time_clean;
            SizeClean = size_clean;

            MinutesClean = TimeClean ? minutes_clean : 0;
            KilobytesSize = SizeClean ? kilobytes_clean : 0;

            if(SizeClean && KilobytesSize <= 0)
                throw new ArgumentException("KilobytesSize za kreiranje objekta nije ispravan!");
            else if(TimeClean && MinutesClean <= 0)
                throw new ArgumentException("MinutesClean za kreiranje objekta nije ispravan!");               
        }

        private void EmptyFileIfTimePassed(String filename)
        {      
            string[] lines = File.ReadAllLines(filename);
            //probaj da dohvatis za:               
            //2. prazan fajl
            //3. pun fajl
            if (lines.Length !=0 && lines[0] != null)
            {
                var firstLineDateTime = DateTime.Parse(lines[0]);
                var timeDifferenceInMiliseconds = (DateTime.Now - firstLineDateTime).TotalMilliseconds;
                var millisecondsInMinute = 60000;

                if (timeDifferenceInMiliseconds > MinutesToEmptyFile * millisecondsInMinute)
                {
                    File.WriteAllText(filename, String.Empty);
                }
            }         
        }
        private void EmptyFileIfSizeExceeded(String filename)
        {
            long fileSize = new FileInfo(filename).Length;
            if (fileSize > KilobytesSize * 1000)
            {
                File.WriteAllText(filename, String.Empty);
            }      
        }
        private void ConditionCheckForFileClean(string filename)
        {
            if(TimeClean)
                EmptyFileIfTimePassed(filename);
            if(SizeClean)
                EmptyFileIfSizeExceeded(filename);        
        }
        private Boolean ConditionCheckIfFileExist(string filename)
        {
            if (!File.Exists(filename))
                return false;
            else
                return true;    
        }
        private void CreateLogFile(string filename)
        {
            using (StreamWriter sw = File.CreateText(filename));
        }
        public void Delete_From_Log_File(string filename)
        {
            File.WriteAllText(filename, String.Empty);
        }
        public void Append_to_log_file(string filename, string log)
        {
            if(!ConditionCheckIfFileExist(filename))
                CreateLogFile(filename);

            ConditionCheckForFileClean(filename);
            
            using (StreamWriter sw = File.AppendText(filename))
            {
                var LogDateTime = DateTime.Now.ToString();
                sw.WriteLine($"{LogDateTime}");
                sw.WriteLine($"{log}");
                sw.WriteLine($"{LogSeparator}");           
            }       
        }
        public List<String> Read_all_lines_from_file(String filename) {
            if (!ConditionCheckIfFileExist(filename))
                throw new ArgumentException("Ne postoji fajl iz kog se iscitava ceo log");

            return new List<string>(File.ReadAllLines(filename));        
        }
        public List<String> Read_last_log_from_file(String filename)
        {
            if (!ConditionCheckIfFileExist(filename))
                throw new ArgumentException("Ne postoji fajl iz kog se iscitava poslednji log");

            List<String> returnLogGroup = new List<String>();

            String[] allLines = File.ReadAllLines(filename);
            var logStartIndex = allLines.Length - 3;

            for (int i = logStartIndex; i < allLines.Length; i++)
            {
                returnLogGroup.Add(allLines[i]);
            }
            return returnLogGroup;
        }
        
    }

}

