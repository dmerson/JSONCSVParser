﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;


namespace CreateCampusLogicImportFile
{
    class Program
    {
       
       

        private static bool IsCommaPresent(string columnValue)
        {
            return columnValue.Contains(",");
        }
        static void Main(string[] args)
        {
            
            string connectionString= System.Configuration.ConfigurationManager.ConnectionStrings["IntendedServer"].ConnectionString;
            string singleQuote = "\"";
            string doubleQuote = "\"\"";
            string endOfLIne = "\r\n";
            var sqlStatement = System.Configuration.ConfigurationManager.AppSettings["sql"];
            string lastColumnName ="";
            string finalFilePath = System.Configuration.ConfigurationManager.AppSettings["finalFilePath"];
            var columnDeliminator = ",";
            var commaDeliminatorReplace = ";";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                Console.WriteLine(DateTime.Now);
                
                using (SqlCommand cmd = new SqlCommand(sqlStatement))
                {
                    cmd.CommandTimeout =(int.Parse(System.Configuration.ConfigurationManager.AppSettings["commandTimeout"]));
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            var fileString = new StringBuilder();
                            var lastColumnNumber = dt.Columns.Count-1;
                            lastColumnName = dt.Columns[lastColumnNumber].ColumnName;

                            foreach (DataColumn column in dt.Columns)
                            {
                                
                                fileString.Append(column.ColumnName);
                                if (column.ColumnName != lastColumnName)
                                {
                                    fileString.Append(columnDeliminator);
                                }
                                
                            }

                         
                            
                            fileString.Append(endOfLIne);
                            
                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    
                                    if (IsCommaPresent(row[column.ColumnName].ToString()))
                                    {
                                       //JSON Columns start and end with single Quote and need to double each quote within JSON
                                        fileString.Append(singleQuote);
                                       
                                        fileString.Append(row[column.ColumnName].ToString().Replace(singleQuote, doubleQuote));
                                        fileString.Append(singleQuote);
                                    }
                                    else
                                    {
                                        
                                        fileString.Append(row[column.ColumnName].ToString());
                                    }
                                    if (column.ColumnName != lastColumnName)
                                    {
                                        fileString.Append(columnDeliminator);
                                    }
                                }

                                
                                fileString.Append(endOfLIne);
                            }
                            File.WriteAllText(finalFilePath, fileString.ToString());
                         
                        }
                    }
                }
            }
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Done");
            //Console.Read();
        }

       
    }
}
