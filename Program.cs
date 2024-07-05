using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace SportsStatsTracker
{
    class Person
    {
       public string name = "Joao";
       public string age = "33";
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = config["CacheConnection"];

            using (var cache = ConnectionMultiplexer.Connect(connectionString))
            {
                IDatabase db = cache.GetDatabase();

                bool setValue = db.StringSet("test:key", "some value");
                Console.WriteLine($"SET: {setValue}");

                string? getValue = db.StringGet("test:key");
                Console.WriteLine($"GET: {getValue}");

                setValue = await db.StringSetAsync("counter","100");
                Console.WriteLine($"SET: {setValue}");

                getValue = await db.StringGetAsync("counter");
                Console.WriteLine($"GET: {getValue}");

                long newValue = await db.StringIncrementAsync("counter", 50);
                Console.WriteLine($"INCR new value = {newValue}");

                var result = await db.ExecuteAsync("ping");
                Console.WriteLine($"PING = {result.Type} : {result}");

                result = await db.ExecuteAsync("flushdb");
                Console.WriteLine($"FLUSHDB = {result.Type} : {result}");

                Person person = new Person();
                setValue = await db.StringSetAsync("person", JsonConvert.SerializeObject(person));
                Console.WriteLine($"SET: {setValue}");

                getValue = await db.StringGetAsync("person");
                Person aPerson = JsonConvert.DeserializeObject<Person>(getValue);
                Console.WriteLine($"GET: {getValue}");
                Console.WriteLine($"Person name: {aPerson.name}");
            }
        }
    }
}