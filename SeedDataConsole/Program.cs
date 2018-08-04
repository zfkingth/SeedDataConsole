using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SeedDataConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace SeedDataConsole
{
    class Program
    {

        public static string ConnectionString = "Server=.;Database=XLDDSMTest;Trusted_Connection=True;";
        static void Main(string[] args)
        {
            Console.WriteLine("是否删除所有数据，然后重新插入数据？y/n");
            DSMContext dbcontext = new DSMContext();

            int cnt = dbcontext.SensorInfo.Count();



            var key = Console.ReadKey();
            Console.WriteLine();
            if (key.KeyChar == 'y')
            {
                deleteAllValues(dbcontext);
                cnt = dbcontext.SensorInfo.Count();
                if (cnt > 0)
                {
                    Console.WriteLine("删除数据失败");


                    prepareQuit();

                    return;
                }


            }

            if (cnt == 0)
            {
                Stopwatch stop = new Stopwatch();
                stop.Start();
                int insertSensorCnt = 9899;
                int insertDataCntPerSersor = 87600;
                seedSensor_Data(insertSensorCnt, insertDataCntPerSersor);
                stop.Stop();

                Console.WriteLine(string.Format("插入 {0:N0} 条数据，用时 {1:N0} 毫秒。",
                   (long)insertDataCntPerSersor * insertDataCntPerSersor, stop.ElapsedMilliseconds));
            }

            cnt = dbcontext.SensorInfo.Count();

            Console.WriteLine("测点数量: " + cnt);

            prepareQuit();
        }

        private static ProjectInfo getFirstProject(DSMContext dbcontext)
        {
            var item = dbcontext.ProjectInfo.FirstOrDefault();

            if (item == null)
            {
                item = new ProjectInfo();
                item.Id = Guid.NewGuid();
                item.Name = "测试工程。";
                dbcontext.Add(item);
                dbcontext.SaveChanges();

            }

            return item;
        }

        private static void prepareQuit()
        {
            Console.WriteLine("回车 退出");

            Console.ReadLine();


        }

        private static void deleteAllValues(DSMContext dbcontext)
        {
            string sqlDeleteStatement =
                @"alter table  [dbo].[SensorDataOrigin]  nocheck CONSTRAINT ALL;
                TRUNCATE TABLE[dbo].[SensorDataOrigin];
                delete from[dbo].[SensorInfo];
                alter table  [dbo].[SensorDataOrigin]  check CONSTRAINT ALL;";
            dbcontext.Database.ExecuteSqlCommand(sqlDeleteStatement);



        }

        private static void seedSensor_Data(int sensorCnt, int dataCntPerSersor)
        {
            DSMContext dbcontext = new DSMContext();
            dbcontext.ChangeTracker.AutoDetectChangesEnabled = false;

            string sqlForFull = @"USE master ;  
ALTER DATABASE XLDDSM1 SET RECOVERY FULL ;  ";
            string sqlForSimple = @"USE master ;  
ALTER DATABASE XLDDSM1 SET RECOVERY Simple ;  ";

            dbcontext.Database.ExecuteSqlCommand(sqlForSimple);

            Console.WriteLine("cleared all data in database");

            DataTable originDataTable = new DataTable();
            originDataTable.Columns.Add("Id", typeof(Guid));
            originDataTable.Columns.Add("SensorId", typeof(Guid));
            originDataTable.Columns.Add("MeaTime", typeof(DateTime));
            originDataTable.Columns.Add("MeaValue1", typeof(float));
            originDataTable.Columns.Add("MeaValue2", typeof(float));
            originDataTable.Columns.Add("MeaValue3", typeof(float));
            originDataTable.Columns.Add("ResValue1", typeof(float));
            originDataTable.Columns.Add("ResValue2", typeof(float));
            originDataTable.Columns.Add("ResValue3", typeof(float));
            originDataTable.Columns.Add("Status", typeof(byte));


            //

            //DataTable sensorInfoTable = new DataTable();
            //sensorInfoTable.Columns.Add("Id", typeof(Guid));
            //sensorInfoTable.Columns.Add("ProjectId", typeof(Guid));
            //sensorInfoTable.Columns.Add("SensorCode", typeof(string));


            DateTime startDate = new DateTime(1900, 1, 1);


            ProjectInfo project = getFirstProject(dbcontext);

            List<SensorInfo> sensorList = new List<SensorInfo>(sensorCnt);
            for (int i = 0; i < sensorCnt; i++)
            {
                //Console.WriteLine(string.Format("inserting No. {0} Sensor info ", i + 1));

                string sensorCode = "test" + i.ToString("d5");
                SensorInfo sen = new SensorInfo();
                sen.Id = Guid.NewGuid();
                sen.SensorCode = sensorCode;
                sen.ProjectId = project.Id;


                sensorList.Add(sen);
            }

            Stopwatch stop = new Stopwatch();
            stop.Start();
            dbcontext.BulkInsert(sensorList);
            stop.Stop();
            Console.WriteLine(string.Format("insert all sersor info Elapsed Milliseconds :{0} ", stop.ElapsedMilliseconds));

            var orderEnum = sensorList.OrderBy(s => s.Id);
            int index = 0;
            foreach (var senItem in orderEnum)
            {
                originDataTable.Clear();
                for (int j = 0; j < dataCntPerSersor; j++)
                {

                    var item = originDataTable.NewRow();
                    item["Id"] = Guid.NewGuid();
                    item["SensorId"] = senItem.Id;
                    item["MeaTime"] = startDate.AddHours(0.5 * j);
                    item["MeaValue1"] = j;
                    item["MeaValue2"] = j;
                    item["MeaValue3"] = j;
                    item["ResValue1"] = j;
                    item["ResValue2"] = j;
                    item["ResValue3"] = j;
                    item["Status"] = 0;


                    originDataTable.Rows.Add(item);
                }

                Console.Write(string.Format("inserting {0}  data , No. {1:d5},", senItem.SensorCode, index));
                stop = new Stopwatch();
                stop.Start();
                bulkcopy(originDataTable);
                stop.Stop();

                Console.WriteLine(string.Format(" Elapsed Milliseconds :{0} ", stop.ElapsedMilliseconds));
                index++;
            }
            //避免尾巴


            dbcontext.Database.ExecuteSqlCommand(sqlForFull);

        }

        private static void bulkcopy(DataTable dt)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var sqlBulkCopy = new SqlBulkCopy(connection))
                {
                    sqlBulkCopy.DestinationTableName = "SensorDataOrigin";
                    sqlBulkCopy.WriteToServer(dt);
                }
            }
        }
    }
}
