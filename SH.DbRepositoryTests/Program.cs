using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SH.DbRepository;
using SH.DbRepositoryTests;
using SH.DbRepositoryTests.Tests;



var services = new ServiceCollection();
services.AddScoped<IUnitOfWork, UnitOfWorkEfCore<AppDbContext>>();
services.AddScoped<SH.DbRepositoryTests.Repositorys.ITestModelRepository, SH.DbRepositoryTests.Repositorys.TestModelRepository>();
services.AddDbContext<SH.DbRepositoryTests.Tests.AppDbContext>(options =>
{
    options.UseInMemoryDatabase("TestDb");
});

var serviceProvider = services.BuildServiceProvider();
IUnitOfWork unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
var testRepo = serviceProvider.GetRequiredService<SH.DbRepositoryTests.Repositorys.ITestModelRepository>();

for (int i = 0; i < 100; i++)
{
    await testRepo.AddAsync(new TestModel { Name = $"Test----{i}" });
}
await unitOfWork.CommitAsync();

testRepo.Tracking = false;
var all = await testRepo.GetAllAsync();
Utils.WriteLineForMagenta($"Count: {all.Count()}");
try
{
    testRepo.Tracking = false;
    var topList = await testRepo.QueryAsync(p => p.Id > 10, 20, p => p.Id, true);
    Utils.WriteLineForMagenta($"TopList Count: {topList.Count()}");
    var item = topList.First();
    Utils.WriteLineForMagenta($"Item Id: {item.Id}, Name: {item.Name}");
    item.Name = "Test";
    await testRepo.UpdateAsync(item);
    await unitOfWork.CommitAsync();
}
catch (Exception)
{
    Utils.WriteForYellow("异常就对了");
}
testRepo.Tracking = true;

var  topList1 = await testRepo.QueryAsync(p => p.Id > 10, 20, p => p.Id, true);
Utils.WriteLineForMagenta($"TopList Count: {topList1.Count()}");
var item1 = topList1.First();
Utils.WriteLineForMagenta($"Item Id: {item1.Id}, Name: {item1.Name}");
item1.Name = "Test";
await testRepo.UpdateAsync(item1);
await unitOfWork.CommitAsync();
Utils.WriteLineForCyan($"Item Id: {item1.Id}, Name: {item1.Name}");








