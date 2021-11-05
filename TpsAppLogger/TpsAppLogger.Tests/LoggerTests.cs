using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TpsAppLogger;
using System.IO;


namespace TpsAppLogger.Tests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public void Logger_ParamValidationByTime()
        {
            const bool expectedBool = true;
            const int expectedTime = 120;

            Logger l = new Logger(expectedBool, false, expectedTime, 0);

            Assert.That(l.TimeClean, Is.EqualTo(expectedBool));
            Assert.That(l.MinutesClean, Is.EqualTo(expectedTime));
        }

        [Test]
        public void Logger_ParamValidationBySize()
        {
            const bool expectedBool = true;
            const int expectedSize = 120;
            Logger l = new Logger(false, expectedBool, 0, expectedSize);

            Assert.That(l.SizeClean, Is.EqualTo(expectedBool));
            Assert.That(l.KilobytesSize, Is.EqualTo(expectedSize));
        }
        [Test]
        public void Logger_CheckAppendLineMethod()
        {
            const bool expectedBool = true;
            const int expectedSize = 120;
            const string filename = "test1.txt";

            Logger l = new Logger(false, expectedBool, 0, expectedSize);
            l.Delete_From_Log_File(filename);
            l.Append_to_log_file(filename, "append1");

            var expectedLines = l.Read_all_lines_from_file(filename).Count;
            Assert.That(expectedLines, Is.EqualTo(3));
            
        }

        [Test]
        public void Logger_CheckReadLines()
        {
            const bool expectedBool = true;
            const int expectedSize = 120;
            const string filename = "test2.txt";

            Logger l = new Logger(false, expectedBool, 0, expectedSize);
           
            for (int i = 0; i < 10; i++)
            {
                l.Append_to_log_file(filename, "append" + i.ToString());
            }

            var expectedLines = l.Read_all_lines_from_file(filename).Count;
            var lastLogText = l.Read_last_log_from_file(filename)[1];

            Assert.That(expectedLines, Is.EqualTo(30));
            Assert.That(lastLogText, Is.EqualTo("append9"));

            l.Delete_From_Log_File(filename);
        }
        [Test]
        public void Logger_CheckIfFileSizeExceed()
        {
            const bool expectedBool = true;
            const int expectedSize = 1;
            const string filename = "test3.txt";

            using (StreamWriter sw = File.CreateText(filename)) ;
            Logger l = new Logger(false, expectedBool, 0, expectedSize);
            int iteration = 0;

            while(new FileInfo(filename).Length < expectedSize*1000)
            {
                l.Append_to_log_file(filename, "append" + iteration.ToString());
                iteration++;
            }
            l.Append_to_log_file(filename, "appendPrvi");
            l.Append_to_log_file(filename, "appendPoslednji");

            var expectedLines = l.Read_all_lines_from_file(filename).Count;
            var lastLogText = l.Read_last_log_from_file(filename)[1];

            Assert.That(expectedLines, Is.EqualTo(6));
            Assert.That(lastLogText, Is.EqualTo("appendPoslednji"));

            l.Delete_From_Log_File(filename);
        }

        [Test]
        public void Logger_CheckIfTimeExceed()
        {
            const bool expectedBool = true;
            const int expectedTime = 1;
            const string filename = "test4.txt";

            Logger l = new Logger(expectedBool, false, expectedTime, 0);
            l.Append_to_log_file(filename, "append");
            DateTime BreakLoopDateTime = DateTime.Now.AddMinutes(expectedTime);
             while (DateTime.Now < BreakLoopDateTime)
             {
                l.Append_to_log_file(filename, "append" + DateTime.Now.ToString());

             }
             l.Append_to_log_file(filename, "appendPoslednji");

             var expectedLines = l.Read_all_lines_from_file(filename).Count;
             var lastLogText = l.Read_last_log_from_file(filename)[1];
             
            Assert.That(expectedLines, Is.EqualTo(3));
            Assert.That(lastLogText, Is.EqualTo("appendPoslednji"));
        }
    }

}

