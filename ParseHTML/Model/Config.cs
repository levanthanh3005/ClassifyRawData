using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Config
{
    public static String dataSource;
    public static String dbName;
    public static String user;
    public static String pass;
    public static void readConfig()
    {
        string[] lines = System.IO.File.ReadAllLines(@"../../ConnectionConfig.dat");
        dataSource = lines[0];
        dbName = lines[1];
        user = lines[2];
        pass = lines[3];
    }
}
