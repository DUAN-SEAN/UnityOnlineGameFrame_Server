using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MongoDBRobot
{
    class A
    {
        public void Dis()
        {
            Console.WriteLine("A");
        }
    }
    class BB : A
    {
        public void Dis()
        {
            Console.WriteLine("B");
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            //Test1();
            BB b = new BB();
            A a = b;
            Console.WriteLine(a.GetType());
            a.Dis();

            //var pre = PredictPosition(Vector2.Zero, new Vector2(0, 1), new Vector2(0, 75), 1f);

            //.WriteLine(pre.ToString());

            //ObjectId

            Console.ReadKey();

        }
        /// <summary>
        /// 位置预测函数
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="frictionAir"></param>
        /// <returns></returns>
        public static Vector2 PredictPosition(Vector2 position, Vector2 velocity, Vector2 Force, float time, float invMass = 1 / 75f, float stepTime = 1 / 60f, float LinearDamping = 0.0f)
        {
            Vector2 predictPosition = position;
            do
            {
                velocity += stepTime * (invMass * Force);//速度增加
                velocity *= 1.0f / (1.0f + stepTime * LinearDamping);//阻尼效应

                predictPosition += stepTime * velocity;//积分

                time -= stepTime;
            } while (time - stepTime > float.Epsilon);

            return predictPosition;
        }
        static void Test1()
        {
            try
            {


                // 连接到单实例MongoDB服务
                // 这种连接方式不会自动检测连接的节点是主节点还是复制集的成员节点
                //var client = new MongoClient();

                // 或者使用string连接本地服务器,localhost=127.0.0.1,连接到单实例
                string connection = $"mongodb://{dbConfig.ConnectHost}:{dbConfig.Port}/{dbConfig.DataBase}";
                string connection1 = $"mongodb://root:123456@{dbConfig.ConnectHost}:{dbConfig.Port}/admin";
                Log.Info(connection);
                var client = new MongoClient(connection);



                var dataBase = client.GetDatabase(dbConfig.DataBase);

                var collection = dataBase.GetCollection<Player>("Players");

                //var filter = Builders<Player>.Filter.Eq("username", "dr");
                //var result = collection.Find(filter).ToList();
                //foreach(var item in result)
                //{
                //    Console.WriteLine(item.DisPlayer());
                //}


                //{ //插入

                //    Player player = new Player();
                //    player.username = "test1";
                //    player.password = "123456";
                //    player.level = 100;
                //    player.userId = 1;
                //    player.friends = new List<Friend>();
                //    player.friends.Add(new Friend { userId = 2, username = "dr" });
                //    collection.InsertOne(player);


                //}

                //匹配是否有相等的
                //var filter = Builders<Player>.Filter.Eq("level", 99);   //匹配Name  为 “g” 的数据
                //var result = collection.DeleteMany(filter);
                //Console.WriteLine(result.DeletedCount);


                //更新数据1   
                //var filter = Builders<Player>.Filter.Eq("username", "test1");
                //var update = Builders<Player>.Update.Set("username", "sean");
                //var result = collection.UpdateOne(filter, update);
                //Console.WriteLine(result.MatchedCount);

                //更新数据2
                var filter = Builders<Player>.Filter.Eq("username", "sean");
                var update = Builders<Player>.Update.Set("friends", new List<Friend> { new Friend { userId = 2,username = "t1"},
                new Friend { userId = 2,username = "t2"},
                new Friend { userId = 3,username = "t3"},
                new Friend { userId = 4,username = "t4"},
                new Friend { userId = 5,username = "t7"}});
                //var result = collection.UpdateOne(filter, update);
                collection.UpdateOneAsync(filter, update).ContinueWith(t => { Console.WriteLine("插入成功"); });
                //Console.WriteLine(result.MatchedCount);

#pragma warning disable CS0219 // 变量“a”已被赋值，但从未使用过它的值
                Array a = null;
#pragma warning restore CS0219 // 变量“a”已被赋值，但从未使用过它的值




                //// Update Multiple Documents
                //var builder = Builders<Person>.Filter;
                //var filter = builder.Eq("Name", "g") & builder.Eq("Pwd", "123");
                //var update = Builders<Person>.Update.Set("field", "value");
                //var result = collection.UpdateMany(filter, update);


                ////Query Data
                //var filter = Builders<Person>.Filter.Eq("Name", "g");
                //var result = collection.Find(filter).ToList();

                ////Greater Than Operator($gt)    
                //var filter = Builders<Person>.Filter.Gt("Score", 60);
                //var result = collection.Find(filter).ToList();     //获取分数超过60的集合

                ////Less Than Operator($lt)
                //var filter = Builders<Person>.Filter.Lt("Score", 60);
                //var result = collection.Find(filter).ToList();    //获取分数低于60的集合

                ////添加单个索引 Name
                //var keys = Builders<Person>.IndexKeys.Ascending("Name");
                //collection.Indexes.CreateOne(keys);

                ////添加多个索引
                //var keys = Builders<Person>.IndexKeys.Ascending("Name").Ascending("Pwd");
                //collection.Indexes.CreateOne(keys);

                //{
                //    //插入
                //    var document = new BsonDocument
                //    {
                //    { "username", "MongoDB3" },
                //    { "password", "Database" },
                //    { "level", 1 },

                //    };
                //    collection.InsertOne(document);
                //    Log.Info("插入成功：" + document.ToJson());

                //}
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
    internal class Log
    {
        internal static void Error(Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        internal static void Info(string v)
        {
            Console.WriteLine(v);
        }
    }
    internal class dbConfig
    {
        internal static string ConnectHost = "114.115.146.185";
        internal static int Port = 27017;
        internal static string DataBase = "SpaceShooter";
        internal static string Collection_Players = "Players";
    }

    internal class Player
    {
        public ObjectId _id { set; get; }
        public long userId { set; get; }
        public string username { get; set; }

        public string password { get; set; }

        public int level { get; set; }

        public List<Friend> friends { get; set; }

        internal string DisPlayer()
        {
            return $"{_id} {username} {password} {level} ";
        }
    }
    internal class Friend
    {
        public long userId { get; set; }

        public string username { get; set; }


    }
}
