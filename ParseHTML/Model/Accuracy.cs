using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public static class Accuracy
{
    public static int totalCount = 0;
    public static int wrongItemCount = 0;
    public static List<Issue> lsIssue = new List<Issue>();
    public static String id;
    public static String url;
    public static void addItemCount()
    {
        totalCount++;
    }
    public static void addWrongItemCount()
    {
        wrongItemCount++;
    }
    public static double getAccuracy()
    {
        return 100 - (wrongItemCount * 100 / totalCount);
    }
    public static void show()
    {
        Console.WriteLine("Accuracy:"+getAccuracy());
    }
    public static void addIssue(Issue issue)
    {
        lsIssue.Add(issue);
    }
    public static void saveFile()
    {
        Console.WriteLine("Save issue file");
        StreamWriter file = new StreamWriter(@"../../LogIssue.dat");
        foreach (Issue i in lsIssue)
        {
            file.WriteLine("id:"+i.id+" with url:" + i.id);
            file.WriteLine("Message:"+i.msg);
        }
        file.WriteLine("Accuracy:" + getAccuracy()+" %");
        file.Close();
        Console.WriteLine("Done Writefile for CRF");
    }
    public class Issue
    {
        public String id;
        public String url;
        public String msg;
        public Issue(String id, String url, String msg)
        {
            this.id = id;
            this.url = url;
            this.msg = msg;
        }
        public void show()
        {
            Console.WriteLine(id+" "+msg);
        }
    }
}
