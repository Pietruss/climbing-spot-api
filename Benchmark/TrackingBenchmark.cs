using BenchmarkDotNet.Attributes;
using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class TrackingBenchmark
    {
        [Benchmark]
        public int WithTracking()
        {
            var optionsBulier = new DbContextOptionsBuilder<ClimbingDbContext>().UseSqlServer("Server=localhost;Database=ClimbingSpot;Trusted_Connection=True");
            var _dbContext = new ClimbingDbContext(optionsBulier.Options);

            var climbingSpot = _dbContext
                .ClimbingSpot
                .ToList();

            return climbingSpot.Count;
        }

        [Benchmark]
        public int WithNoTracking()
        {
            var optionsBulier = new DbContextOptionsBuilder<ClimbingDbContext>().UseSqlServer("Server=localhost;Database=ClimbingSpot;Trusted_Connection=True");
            var _dbContext = new ClimbingDbContext(optionsBulier.Options);

            var climbingSpot = _dbContext
                .ClimbingSpot
                .AsNoTracking()
                .ToList();

            return climbingSpot.Count;
        }
    }
}
