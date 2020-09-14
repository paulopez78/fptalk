using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace csharp
{
    public class Samples
    {
        [Fact]
        public void Composition_Test()
        {
            Assert.Equal(Add5(Add2(2)), Add2Then5(2));

            Assert.Equal(IsEven(Add5(Add2(2))), IsEventIfAdd2Then5(2));

            Assert.Equal(
                2.Add2().Add5().IsEven(), 
                IsEventIfAdd2Then5(2)
            );

            bool IsEven(int x) => x.IsEven();

            int Add2 (int x) => x.Add2();

            int Add5 (int x) => x.Add5();

            int Add2Then5 (int x) => Add5(Add2(x));

            bool IsEventIfAdd2Then5 (int x) => IsEven(Add5(Add2(x)));
        }

        [Fact]
        public void IEnumerableSelect_Tests()
        {
            IEnumerable<int> yearsOfExperience = new[] { 8, 5, 2, 2, 10, 0 };

            Assert.Equal(IsSenior(yearsOfExperience), yearsOfExperience.Select(x => IsSenior(x)));
            Assert.Equal(IsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));

            Print(yearsOfExperience.Select(IsSenior));
            Print(IsSenior(yearsOfExperience));

            Func<IEnumerable<int>, IEnumerable<bool>> liftedIsSenior = Lift<int,bool>(IsSenior);

            Assert.Equal(liftedIsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));
            Assert.Equal(liftedIsSenior(yearsOfExperience), IsSenior(yearsOfExperience));

            Print(liftedIsSenior(yearsOfExperience));

            8.Level().Promote().Years().IsSenior(); // true

            Print(yearsOfExperience.Select(Level).Select(Promote).Select(Years).Select(IsSenior));
            yearsOfExperience.Select(Log).Select(IsSenior).Select(Log).ToArray();
        }

        [Fact]
        public async Task Task_Select_Tests()
        {
            Task<int> yearsOfExperience = Task.FromResult(6);

            Assert.Equal(IsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));
            
            Task<bool> result1 = yearsOfExperience.Select(IsSenior);
            bool isSenior = await result1;
            Assert.True(isSenior);

            Task<bool> result2 = yearsOfExperience.Select(Level).Select(Promote).Select(Years).Select(IsSenior);
            isSenior = await result2;
            Assert.True(isSenior);

            var liftedIsSenior = LiftTask<int,bool>(IsSenior);
            Assert.Equal(liftedIsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));

            await yearsOfExperience.Select(Log).Select(IsSenior).Select(Log);
        }

        [Fact]
        public async Task Nullable_Select_Tests()
        {
            Nullable<int> GetYearsOfExperience(string email) 
                => email switch {
                    "bob@abax.no"   => 3,
                    "alice@abax.no" => 4,
                    _               => null,
                };
        }

        bool IsSenior(int years) => years.IsSenior(); // int -> bool

        IEnumerable<bool> IsSenior(IEnumerable<int> years) => years.Select(IsSenior); // IEnumerable<int> -> IEnumerable<bool>

        Task<bool> IsSenior(Task<int> years) => years.Select(IsSenior); // Task<int> -> Task<bool>

        void Print<T>(IEnumerable<T> list) => Console.WriteLine(string.Join(",", list));

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Lift<TSource, TResult>(Func<TSource, TResult> func)
            => source => source.Select(func);

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func)
            => Lift(func);

        Func<Task<TSource>, Task<TResult>> LiftTask<TSource, TResult>(Func<TSource, TResult> func)
            => source => source.Select(func);

        Func<Task<TSource>, Task<TResult>> MapTask<TSource, TResult>(Func<TSource, TResult> func)
            => LiftTask(func);

        string Level(int years) => years.Level();

        string Promote(string level) => level.Promote();

        int Years(string level) => level.Years();

        T Log<T>(T result)
        {
            Console.Write($"LOG {result},");
            return result;
        }
    }

    public static class Extensions
    {
        public static bool IsEven(this int x) => x % 2 == 0;

        public static int Add2 (this int x) => x + 2;

        public static int Add5 (this int x) => x + 5;

        public static bool IsSenior(this int years) => years > 5; // int -> bool

        public static string Level(this int years) 
        => years switch
        {
            > 10 => "Rockstar",
            > 8  => "Ninja",
            > 5  => "Senior",
            > 3  => "Mid",
            > 0  => "Junior",
            _    => "No idea what I'm doing",
        };

        public static string Promote(this string level) 
        => level switch
        {
            "Ninja"  => "Rockstar",
            "Senior" => "Ninja",
            "Mid"    => "Senior",
            "Junior" => "Mid",
            _        => level
        };

        public static int Years(this string level) 
        => level switch
        {
            "Rockstar" => 12,
            "Ninja"    => 10,
            "Senior"   => 8,
            "Mid"      => 6,
            "Junior"   => 2,
            _          => 0
        };

        public static string Upgrade(string skill)
            => skill switch
            {
                "fsharp" => "haskell",
                "csharp" => "fsharp",
                "docker" => "k8s",
                "azure"  => "gcp",
                _        => skill
            };

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func)
        {
            foreach (var item in source)
            {
                yield return func(item);
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (var element in source) {
                foreach (var subElement in selector(element)) {
                    yield return subElement;
                }
            }
        }

        public static async Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> selector)
        {
            var x = await source;
            return selector(x);
        }

        public static async Task<TResult> SelectMany<TSource, TResult>(this Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            var x = await source;
            return await selector(x);
        }
    }
}