using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Smairo.Extensions
{
    /// <summary>
    /// Extensions for task with collections in result
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Create <see cref="List{T}"/> from <see cref="IEnumerable{T}"/> that you can await
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> task)
        {
            return (await task)
                .ToList();
        }

        /// <summary>
        /// Create <see cref="IEnumerable{T}"/> from <see cref="List{T}"/> that you can await
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> AsEnumerableAsync<T>(this Task<List<T>> task)
        {
            return (await task)
                .AsEnumerable();
        }

        /// <summary>
        /// Create <see cref="IQueryable{T}"/> from <see cref="IEnumerable{T}"/> that you can await
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<IQueryable<T>> AsQueryableAsync<T>(this Task<IEnumerable<T>> task)
        {
            return (await task)
                .AsQueryable();
        }

        /// <summary>
        /// Create T[] from <see cref="IEnumerable{T}"/> that you can await
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<T[]> ToArrayAsync<T>(this Task<IEnumerable<T>> task)
        {
            return (await task)
                .ToArray();
        }

        /// <summary>
        /// Create T[] from <see cref="List{T}"/> that you can await
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        public static async Task<T[]> ToArrayAsync<T>(this Task<List<T>> task)
        {
            return (await task)
                .ToArray();
        }
    }
}