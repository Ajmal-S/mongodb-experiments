using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsoleApplication9
{
    class Program
    {
        protected const string FilePathIndexingCollectionName = "filePathIndexCollection";
        protected const string TestCaseNameIndexingCollectionName = "testNameIndexCollection";
        protected const string FileIdTestIdCollectionName = "fileIndexAndTestNameIndexCollection";
        private static string fileIndexId = "";
        private static List<string> TestIdList = new List<string>();
        private static List<string> FileNameList = new List<string>();
        private static List<string> TestNameList = new List<string>();
        static void Main(string[] args)
        {

            string mongoDbUrl = "mongodb://vbothra:varun@ds041593.mongolab.com:41593/rtsdb";
            var MongoDbClient = new MongoClient(mongoDbUrl);
            var Database = MongoDbClient.GetDatabase("rtsdb");
            var FileIndexCollection = Database.GetCollection<BsonDocument>(FilePathIndexingCollectionName);
            string k = @"C:\\a\\Source\\Core\\Core\\FileAppender.cs".GetHashCode().ToString();
            var TestNameIndexCollection = Database.GetCollection<BsonDocument>(TestCaseNameIndexingCollectionName);
            var FileIndexAndTestNameIndexPairCollection =
                Database.GetCollection<BsonDocument>(FileIdTestIdCollectionName);
            var TestNameIndexPairCollection =
                Database.GetCollection<BsonDocument>(TestCaseNameIndexingCollectionName);
            var findFilter = Builders<BsonDocument>.Filter.Eq("FilePath", "C:\\a\\Source\\Core\\Core\\PlatformDispatcherTimer.cs");
            GetData(FileIndexCollection, findFilter).Wait();
            Console.WriteLine(fileIndexId);
            var filter = Builders<BsonDocument>.Filter.Eq("FileId", fileIndexId);
            GetDataForFile(FileIndexAndTestNameIndexPairCollection, filter).Wait();
            var query = Builders<BsonArray>.Filter.In("_id", TestIdList.ToBson());
            GetDataForTests(TestNameIndexCollection, query).Wait();


#if DEBUG
            Console.ReadLine();
#endif

        }

        private static async Task GetData(IMongoCollection<BsonDocument> FileIndexCollection,
            FilterDefinition<BsonDocument> findFilter)
        {
            await FileIndexCollection.Find(findFilter).ForEachAsync(data =>
                fileIndexId = data["_id"].ToString());
            await FileIndexCollection.Find(findFilter).ForEachAsync(data =>
                Console.WriteLine("Id: {0} File: {1}", data["_id"], data["FilePath"]));
        }

        private static async Task GetDataForFile(IMongoCollection<BsonDocument> FileIndexAndTestNameIndexPairCollection,
            FilterDefinition<BsonDocument> findFilter)
        {
            var result = await FileIndexAndTestNameIndexPairCollection.Find(findFilter).ToListAsync();
            foreach (var res in result)
            {
                Console.WriteLine(res);
                TestIdList.Add(res["TestId"].ToString());
            }
            TestIdList = TestIdList.Distinct().ToList();
        }

        private static async Task GetDataForTests(IMongoCollection<BsonDocument> FileIndexAndTestNameIndexPairCollection,
            FilterDefinition<BsonArray> findFilter)
        {
            var result = await FileIndexAndTestNameIndexPairCollection.Find(findFilter).ToListAsync();
            foreach (var res in result)
            {
                Console.WriteLine(res);
                TestNameList.Add(res["TestName"].ToString());
            }
            
        }
    }
}

