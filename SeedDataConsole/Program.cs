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

        //command in package manager console
        // Scaffold-DbContext "Server=.;Database=XLDDSMTest;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context DSMContext
        public static string ConnectionString = "Server=.;Database=XLDDSMTest2;Trusted_Connection=True;";

        public static string PrefixName = "test";
        public static DateTime StartDate = new DateTime(1900, 1, 1);

        static void Main(string[] args)
        {
            Console.WriteLine("是否删除所有数据，然后重新插入数据？yes/no");
            DSMContext dbcontext = new DSMContext();

            int cnt = dbcontext.SensorInfo.Count();



            var key = Console.ReadLine();
            Console.WriteLine();
            if (string.Equals(key, "yes", StringComparison.OrdinalIgnoreCase))
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

            int insertSensorCnt = 9899;
            int insertDataCntPerSensor = 87600;
            if (cnt == 0)
            {
                Stopwatch stop = new Stopwatch();
                stop.Start();

                seedSensor_Data(insertSensorCnt, insertDataCntPerSensor);
                stop.Stop();

                Console.WriteLine(string.Format("插入 {0:N0} 条数据，用时 {1:N0} 毫秒。",
                   (long)insertSensorCnt * insertDataCntPerSensor, stop.ElapsedMilliseconds));
            }

            cnt = dbcontext.SensorInfo.Count();

            Console.WriteLine("测点数量: " + cnt);
            ProjectInfo project = getFirstProject(dbcontext);
            ConsoleKeyInfo input;
            Console.WriteLine("单测点按时间分别查询测试：");
            do
            {
                testQuerySingle(dbcontext, project, insertSensorCnt);
                Console.Write("输入n退出当前测试循环，");
                input = Console.ReadKey();
                Console.WriteLine();
            } while (input.KeyChar != 'n');

            Console.WriteLine("20测点按时间分别查询测试：");
            do
            {
                testQueryMultiply(dbcontext, project, insertSensorCnt);
                Console.Write("输入n退出当前测试循环，");
                input = Console.ReadKey();
                Console.WriteLine();
            } while (input.KeyChar != 'n');


            prepareQuit();
        }

        private static void testQueryMultiply(DSMContext db, ProjectInfo project, int insertSensorCnt)
        {

            //clearDbCache(db);


            List<Guid> sensorIdList = getRandom20SensorIdList(db, project, insertSensorCnt);




            var query = from i in db.SensorDataOrigin
                        where sensorIdList.Contains(i.SensorId) && i.MeaTime > StartDate.AddYears(2) && i.MeaTime < StartDate.AddYears(2).AddDays(15)
                        select i;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var result = query.ToArray();
            watch.Stop();

            Console.WriteLine(string.Format("随机查询并返回20测点多个数据，返回数据{0}条, 耗时 {1:N0} ms。",
                result.Length, watch.ElapsedMilliseconds));


        }

        private static void testQuerySingle(DSMContext db, ProjectInfo project, int insertSensorCnt)
        {

            //clearDbCache(db);
            SensorInfo sensor = getRandomSensor(db, project, insertSensorCnt);


            var query = from i in db.SensorDataOrigin
                        where i.SensorId == sensor.Id && i.MeaTime > StartDate.AddYears(2) && i.MeaTime < StartDate.AddYears(2).AddMonths(3)
                        select i;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var result = query.ToArray();
            watch.Stop();

            Console.WriteLine(string.Format("随机查询并返回单个测点半年数据，返回数据{0}条, 耗时 {1:N0} ms。sensor code: {2}",
                result.Length, watch.ElapsedMilliseconds, sensor.SensorCode));


        }

        private static SensorInfo getRandomSensor(DSMContext db, ProjectInfo project, int insertSensorCnt)
        {
            Random r = new Random();
            int index = r.Next(insertSensorCnt);
            string name = PrefixName + index.ToString("d5");
            var sensor = db.SensorInfo.First(s => s.ProjectId == project.Id && s.SensorCode == name);

            return sensor;


        }

        private static List<Guid> getRandom20SensorIdList(DSMContext db, ProjectInfo project, int insertSensorCnt)
        {
            List<string> codeList = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                Random r = new Random();
                int index = r.Next(insertSensorCnt);
                string name = PrefixName + index.ToString("d5");
                codeList.Add(name);
            }
            var query = from i in db.SensorInfo
                        where i.ProjectId == project.Id && codeList.Contains(i.SensorCode)
                        orderby i.Id
                        select i.Id;
            var result = query.ToList();
            return result;


        }

        private static void clearDbCache(DSMContext db)
        {
            string sql = @"CHECKPOINT;
DBCC DROPCLEANBUFFERS;";
            db.Database.ExecuteSqlCommand(sql);
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
            Console.WriteLine("回车 退出程序");

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




            Console.WriteLine("cleared all data in database");

            var bulkOption = new BulkConfig
            {
                PreserveInsertOrder = true,
                SetOutputIdentity = false,
                BatchSize = dataCntPerSersor
            };
            //

            //DataTable sensorInfoTable = new DataTable();
            //sensorInfoTable.Columns.Add("Id", typeof(Guid));
            //sensorInfoTable.Columns.Add("ProjectId", typeof(Guid));
            //sensorInfoTable.Columns.Add("SensorCode", typeof(string));




            ProjectInfo project = getFirstProject(dbcontext);

            List<SensorInfo> sensorList = new List<SensorInfo>(sensorCnt);
            for (int i = 0; i < sensorCnt; i++)
            {
                //Console.WriteLine(string.Format("inserting No. {0} Sensor info ", i + 1));

                string sensorCode = PrefixName + i.ToString("d5");
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

            List<SensorDataOrigin> dataList = new List<SensorDataOrigin>(dataCntPerSersor);

            int index = 0;
            foreach (var senItem in orderEnum)
            {
                dataList.Clear();
                for (int j = 0; j < dataCntPerSersor; j++)
                {

                    var item = new SensorDataOrigin();
                    item.SensorId = senItem.Id;
                    item.MeaTime = StartDate.AddHours(0.5 * j);
                    item.MeaValue1 = j;
                    item.MeaValue2 = j;
                    item.MeaValue3 = j;
                    item.ResValue1 = j;
                    item.ResValue2 = j;
                    item.ResValue3 = j;


                    dataList.Add(item);
                }

                Console.Write(string.Format("inserting data of {0} , order: {1:d5},", senItem.SensorCode, index));
                stop = new Stopwatch();
                stop.Start();
                dbcontext = new DSMContext();
                dbcontext.ChangeTracker.AutoDetectChangesEnabled = false;
                dbcontext.BulkInsert(dataList, bulkOption);
                stop.Stop();

                Console.WriteLine(string.Format(" Elapsed :{0}  ms", stop.ElapsedMilliseconds));
                index++;
            }
            //避免尾巴



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
