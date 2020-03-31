using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIC.DataBase;
namespace PIC.Classes
{
    public class Settings
    {
        public static void ConfigDataBase()
        {
            if (!System.IO.File.Exists(@"C:\Users\alann\Videos\PIC\SQL\PIC.sqlite"))
            {
                SQLDataBase.CreateDataBase();
                SQLDataBase.CreateTable();
                SQLDataBase.RestoreUtterancesData();
            }
        }
    }
}
