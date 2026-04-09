using SH.DbRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SH.DbRepositoryTests.Repositorys
{
    public class TestModelRepository : BaseRepository<int, Tests.TestModel>, ITestModelRepository
    {
        public TestModelRepository(Tests.AppDbContext dbContext) : base(dbContext)
        {
        }
    }

}
