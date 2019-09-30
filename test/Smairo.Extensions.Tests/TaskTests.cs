using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace Smairo.Extensions.Tests
{
    public class TaskTests
    {
        [Fact]
        public async Task Should_Get_Task_Collections()
        {
            int expectedCrunchTotal = 10;

            // Enumberable to list
            List<string> crunched1 = await CrunchStringsEnumerable(expectedCrunchTotal).ToListAsync();
            Assert.Equal(expectedCrunchTotal, crunched1.Count);

            // List to enumerable
            IEnumerable<string> crunched2 = await CrunchStringsList(expectedCrunchTotal).AsEnumerableAsync();
            Assert.Equal(expectedCrunchTotal, crunched2.Count());

            // Enumerable to queryable
            IQueryable<string> crunched3 = await CrunchStringsEnumerable(expectedCrunchTotal).AsQueryableAsync();
            Assert.Equal(expectedCrunchTotal, crunched3.Count());

            // Enumerable to array
            string[] crunched4 = await CrunchStringsEnumerable(expectedCrunchTotal).ToArrayAsync();
            Assert.Equal(expectedCrunchTotal, crunched4.Length);

            // List to array
            string[] crunched5 = await CrunchStringsList(expectedCrunchTotal).ToArrayAsync();
            Assert.Equal(expectedCrunchTotal, crunched5.Length);
        }

        private static Task<IEnumerable<string>> CrunchStringsEnumerable(int expectedCrunchTotal)
        {
            var result = new List<string>();
            for (var i = 0; i < expectedCrunchTotal; i++)
            {
                result.Add(Guid.NewGuid().ToString());
            }
            return Task.FromResult(result.AsEnumerable());
        }

        private static Task<List<string>> CrunchStringsList(int expectedCrunchTotal)
        {
            var result = new List<string>();
            for (var i = 0; i < expectedCrunchTotal; i++)
            {
                result.Add(Guid.NewGuid().ToString());
            }
            return Task.FromResult(result);
        }
    }
}
