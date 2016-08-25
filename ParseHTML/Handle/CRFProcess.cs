using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class CRFProcess
{
    public CRFProcess()
    {
        // crf_test -m model testdata.dat
    }
    public List<Word> doProcess(List<Word> lsWord)
    {
        string output = string.Empty;
        string error = string.Empty;

        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c cd ../../CRF && crf_test -m model testdata.dat");
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
        processStartInfo.UseShellExecute = false;

        Process process = Process.Start(processStartInfo);

        using (StreamReader streamReader = process.StandardOutput)
        {
            output = streamReader.ReadToEnd();
        }

        using (StreamReader streamReader = process.StandardError)
        {
            error = streamReader.ReadToEnd();
        }

        Console.WriteLine("The following output was detected:");
        Console.WriteLine(output);
        String[] lsLine = output.Split('\n');
        foreach(String s in lsLine)
        {
            Console.WriteLine("=>"+s);
        }
        return lsWord;
    }
}