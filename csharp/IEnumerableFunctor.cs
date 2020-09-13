using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using static System.Linq.Enumerable;

namespace csharp
{
    public class IEnumerableFunctor
    {
        [Fact]
        public void IEnumerableFunctor_Tests()
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
        }

        bool IsSenior(int years) => years.IsSenior(); // int -> bool

        IEnumerable<bool> IsSenior(IEnumerable<int> years) => years.Select(IsSenior); // IEnumerable<int> -> IEnumerable<bool>

        void Print<T>(IEnumerable<T> list) => Console.WriteLine(string.Join(",", list));

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Lift<TSource, TResult>(Func<TSource, TResult> func)
            => source => source.Select(func);

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func)
            => Lift(func);

        string Level(int years) => years.Level();

        string Promote(string level) => level.Promote();

        int Years(string level) => level.Years();
    }

    public static class IEnumerableFunctorExtenstions
    {
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

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func)
        {
            foreach (var item in source)
            {
                yield return func(item);
            }
        }
    }
}