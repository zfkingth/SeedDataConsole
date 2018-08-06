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
        // Scaffold-DbContext "Server=.;Database=DSM3;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context DSMContext

        public static string PrefixName = "test";
        public static DateTimeOffset StartDate = new DateTimeOffset((new DateTime(1900, 1, 1)));


        static void Main(string[] args)
        {
            DSMContext dbcontext = new DSMContext();
            Console.WriteLine("数据库连接字符串为：" + dbcontext.Database.GetDbConnection().ConnectionString);
);
            Console.WriteLine("是否接着上一次操作插入数据，保证数据量达到测试要求？yes/no");
            int alreadySensorCnt = 0;

            var key = Console.ReadLine();
            Console.WriteLine();


            int insertSensorCnt = 9899;
            int insertDataCntPerSensor = 87600;
            if (string.Equals(key, "yes", StringComparison.OrdinalIgnoreCase))
            {
                alreadySensorCnt = getSensorCntInDataOrigin(dbcontext);
                if (alreadySensorCnt < insertSensorCnt)
                {
                    //可以接着之前的插入

                    Stopwatch stop = new Stopwatch();
                    stop.Start();

                    seedSensor_Data(alreadySensorCnt, insertSensorCnt, insertDataCntPerSensor);
                    stop.Stop();

                    Console.WriteLine(string.Format("插入 {0:N0} 条数据，用时 {1:N0} 毫秒。",
                       (long)(insertSensorCnt - alreadySensorCnt) * insertDataCntPerSensor, stop.ElapsedMilliseconds));
                }
            }
            alreadySensorCnt = dbcontext.SensorInfo.Count();

            Console.WriteLine("测点数量: " + alreadySensorCnt);
            ProjectInfo project = getFirstProject(dbcontext);


            const int testLoopCnt = 100;
            Console.WriteLine(string.Format("单测点按时间区分查询测试：{0}次", testLoopCnt));

            int cntLoop = 0;
            long cntTime = 0;

            for (int i = 0; i < testLoopCnt; i++)
            {
                cntTime += testQuerySingle(dbcontext, project, insertSensorCnt);
                cntLoop++;
            }

            Console.WriteLine(string.Format("平均耗时：{0} ms。", cntTime / cntLoop));

            cntLoop = 0;
            cntTime = 0;
            Console.WriteLine(string.Format("20测点按时间区分查询测试：{0}次", testLoopCnt));

            for (int i = 0; i < testLoopCnt; i++)
            {
                cntTime += testQueryMultiply(dbcontext, project, insertSensorCnt);
                cntLoop++;
            }


            Console.WriteLine(string.Format("平均耗时：{0} ms。", cntTime / cntLoop));

            prepareQuit();
        }

        private static int getSensorCntInDataOrigin(DSMContext dbcontext)
        {

            //插入的时候是按照id的升序插入的。
            var allSensorList = dbcontext.SensorInfo.OrderBy(s => s.Id).ToList();

            int cnt = 0;
            for (int i = 0; i < allSensorList.Count(); i++)
            {
                var sensor = allSensorList[i];
                bool exist = (from item in dbcontext.SensorDataOrigin
                              where item.SensorId == sensor.Id && item.MeaTime == StartDate
                              select item).Any();
                if (exist)
                {
                    Console.WriteLine(string.Format("{0} 的数据已经插入", sensor.SensorCode));
                    cnt++;
                }
                else
                {
                    break;
                }
            }

            return cnt;
        }

        private static long testQueryMultiply(DSMContext db, ProjectInfo project, int insertSensorCnt)
        {

            //clearDbCache(db);


            List<Guid> sensorIdList = getRandom20SensorIdList(db, project, insertSensorCnt);




            var query = from i in db.SensorDataOrigin
                        where sensorIdList.Contains(i.SensorId) && i.MeaTime > StartDate.AddYears(2) && i.MeaTime < StartDate.AddYears(2).AddDays(30)
                        select i;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var result = query.ToArray();
            watch.Stop();

            Console.WriteLine(string.Format("随机查询并返回20测点多个数据，返回数据{0}条, 耗时 {1:N0} ms。",
                result.Length, watch.ElapsedMilliseconds));
            return watch.ElapsedMilliseconds;

        }

        private static long testQuerySingle(DSMContext db, ProjectInfo project, int insertSensorCnt)
        {

            //clearDbCache(db);
            SensorInfo sensor = getRandomSensor(db, project, insertSensorCnt);


            var query = from i in db.SensorDataOrigin
                        where i.SensorId == sensor.Id && i.MeaTime > StartDate.AddYears(2) && i.MeaTime < StartDate.AddYears(2).AddMonths(6)
                        select i;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var result = query.ToArray();
            watch.Stop();

            Console.WriteLine(string.Format("随机查询并返回单个测点半年数据，返回数据{0}条, 耗时 {1:N0} ms。sensor code: {2}",
                result.Length, watch.ElapsedMilliseconds, sensor.SensorCode));
            return watch.ElapsedMilliseconds;

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

        private static void seedSensor_Data(int alreadySensorCnt, int sensorCnt, int dataCntPerSersor)
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

            if (alreadySensorCnt == 0)
            { //只有第一次才插入sensor info
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
            }

            var orderEnum = dbcontext.SensorInfo.OrderBy(s => s.Id).ToList();

            List<SensorDataOrigin> dataList = new List<SensorDataOrigin>(dataCntPerSersor);
            for (int index = alreadySensorCnt; index < orderEnum.Count; index++)
            {
                var senItem = orderEnum[index];
                dataList.Clear();
                for (int j = 0; j < dataCntPerSersor; j++)
                {

                    var item = new SensorDataOrigin();
                    item.SensorId = senItem.Id;
                    item.MeaTime = StartDate.AddHours(1 * j);
                    item.Origin = (byte)(j % 2);
                    item.MeaValue1 = j;
                    item.ResValue1 = j;


                    dataList.Add(item);
                }

                Console.Write(string.Format("inserting data of {0} , order: {1:d5},", senItem.SensorCode, index));
                Stopwatch stop = new Stopwatch();
                stop.Start();
                dbcontext = new DSMContext();
                dbcontext.ChangeTracker.AutoDetectChangesEnabled = false;
                dbcontext.BulkInsert(dataList, bulkOption);
                stop.Stop();

                Console.WriteLine(string.Format(" Elapsed :{0}  ms", stop.ElapsedMilliseconds));
            }
            //避免尾巴



        }

        private static void bulkcopy(DataTable dt, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
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
