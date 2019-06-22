using System;
using HelperLibrary.NetCore;
using System.Data;


namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\Users\Gayan\Documents\MSOL\DEBTOR_123456781234_ALL LETTER TYPES.txt";

            DataTable dt = TextFileRW.readTextFileToTable(fileName,"\t");
            foreach(DataRow row  in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    Console.Write($"{row[col].ToString()} ");
                }
                Console.Write("\n"); 
            }
            Console.Read();
        }
    }
}
