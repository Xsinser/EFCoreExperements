using EFCoreExperements.Core.Context;
using EFCoreExperements.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = configuration["ConnectionStrings"] ?? throw new NullReferenceException("Empty required parameter ConnectionStrings");

var optionsBuilder = new DbContextOptionsBuilder<MainContext>();
optionsBuilder.UseNpgsql(connectionString);

var context = new MainContext(optionsBuilder.Options);

const int pageSize = 1_000_000;

long memoryUsed_list_t1 = await MeasureTaskMemoryAsync(async () => await GetDbDataToListAsync(context));

//GC.Collect(generation: 2, mode: GCCollectionMode.Forced, blocking: true, compacting: true);
//GC.WaitForPendingFinalizers();

//long memoryUsed_foreach_t1 = await MeasureTaskMemoryAsync(async () => await GetDbData2Async(context));
//long memoryUsed_enumerable_t1 = await MeasureTaskMemoryAsync(() => GetDbDataSync(context));

//GC.Collect(generation: 2, mode: GCCollectionMode.Forced, blocking: true, compacting: true);
//GC.WaitForPendingFinalizers();

//long memoryUsed_foreach_t2 = await MeasureTaskMemoryAsync(async () => await GetDbDataAsync(context));

//GC.Collect(generation: 2, mode: GCCollectionMode.Forced, blocking: true, compacting: true);
//GC.WaitForPendingFinalizers();

//long memoryUsed_list_t2 = await MeasureTaskMemoryAsync(async () => await GetDbDataToListAsync(context));


//Console.WriteLine($"1. Выгрузка массива 1 заход {memoryUsed_list_t1}");
//Console.WriteLine($"1. Выгрузка в цикле 1 заход {memoryUsed_foreach_t1}");
//Console.WriteLine($"1. Выгрузка в цикле 2 заход {memoryUsed_foreach_t2}");
//Console.WriteLine($"1. Выгрузка массива 2 заход {memoryUsed_list_t2}");

static async Task GetDbDataToListAsync(MainContext context)
{
    var data = await context.LargeEntities.AsNoTracking().Take(pageSize).ToListAsync();
    foreach (var item in data)
    {
        Console.WriteLine($"{item.Id}");
    }
}

static async Task GetDbDataAsync(MainContext context)
{
    await foreach (var item in context.LargeEntities.AsNoTracking().Take(pageSize).AsAsyncEnumerable())
    {
        Console.WriteLine($"{item.Id}");
    }
}

static Task GetDbDataSync(MainContext context)
{
    foreach (var item in context.LargeEntities.AsNoTracking().Take(pageSize).AsEnumerable())
    {
        Console.WriteLine($"{item.Id}");
    }
    return Task.CompletedTask;
}

static async Task GetDbData2Async(MainContext context)
{
    int i = 0;
    List<LargeEntity> data = new();
    await foreach (var item in context.LargeEntities.AsNoTracking().Take(pageSize).AsAsyncEnumerable())
    {
        if (i < 500)
        {
            data.Add(item);
            i++;
            continue;
        }
        else
        {
            foreach (var id in data)
            {
                Console.WriteLine($"{id.Id}");
            }
            data.Clear();
            i = 0;
        }
    }
}

static async Task<long> MeasureTaskMemoryAsync(Func<Task> taskFunc)
{
    GC.Collect(); // Принудительная очистка
    GC.WaitForPendingFinalizers();

    long before = GC.GetTotalMemory(true);
    await taskFunc();
    long after = GC.GetTotalMemory(true);

    return after - before;
}