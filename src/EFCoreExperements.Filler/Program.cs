using EFCoreExperements.Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = configuration["ConnectionStrings"];

var optionsBuilder = new DbContextOptionsBuilder<MainContext>();
optionsBuilder.UseNpgsql(connectionString);

var context = new MainContext(optionsBuilder.Options);

context.CreateTables();

var countExistRows = context.LargeEntities.Count();

var intValueRandom = new Random();

var stringRandom = new Random();
StringBuilder stringBuilder25 = new StringBuilder(25);
StringBuilder stringBuilder100 = new StringBuilder(100);

var index = countExistRows;
var batchSize = 100_000;
var currentIndex = 0;
var maxTableSize = 12_000_000;

Console.WriteLine($"Rows in db {countExistRows}");

context.ChangeTracker.AutoDetectChangesEnabled = false;

while (index < maxTableSize)
{
    Console.WriteLine($"Current insert batch start index {index}");
    currentIndex = index;
    var rightIndex = currentIndex + batchSize < maxTableSize ? currentIndex + batchSize : maxTableSize;

    while (currentIndex < rightIndex)
    {
        context.LargeEntities.Add(new()
        {
            Id = currentIndex,
            IntValue = intValueRandom.Next(int.MinValue, int.MaxValue),
            String25 = GenerateRandomString(stringRandom, stringBuilder25),
            String100 = GenerateRandomString(stringRandom, stringBuilder100),
        });
        currentIndex++;
    }

    index = rightIndex;

    context.SaveChanges();
    context.ChangeTracker.Clear();
}

const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
static string GenerateRandomString(Random random, StringBuilder stringBuilder)
{
    stringBuilder.Clear();

    for (int i = 0; i < stringBuilder.Capacity; i++)
    {
        stringBuilder.Append(chars[random.Next(chars.Length)]);
    }

    return stringBuilder.ToString();
}