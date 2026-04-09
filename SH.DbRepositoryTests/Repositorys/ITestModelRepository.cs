using SH.DbRepository;

namespace SH.DbRepositoryTests.Repositorys
{
    public interface ITestModelRepository : IRepository<int, Tests.TestModel>
    {
    }
}