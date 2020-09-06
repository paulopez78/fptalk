namespace csharp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xunit;
    using static CompositionExtensions;
    using static Collections;

    public class Basics
    {
        [Fact]
        public void Composition()
        {
            IEnumerable<int> yearsOfExperience = new[] { 8, 5, 2, 2, 10, 0 };

            Func<int,bool> isSenior = years => years > 5;

            var elevatedIsSenior = Map(isSenior);

            isSenior(8); // true
            elevatedIsSenior(new[]{ 8, 5, 2}); // [ true, false, true ]


            IEnumerable<bool> seniors1 = yearsOfExperience.Select(IsSenior);
            IEnumerable<bool> seniors2 = yearsOfExperience.Select(isSenior);
            IEnumerable<bool> seniors3 = yearsOfExperience.Select(years => years > 5);

            yearsOfExperience
            .Select(Level)
            .Select(Log)
            .Select(Promote)
            .Select(Log)
            .Select(Years)
            .Select(Log)
            .Select(IsSenior)
            .Select(Log);
            // .ToArray();
        }

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func) 
            => source => source.Select(func);

        T Log<T>(T result)
        {
            Console.WriteLine($"LOG {result}");
            return result;
        }
        bool IsSenior(int years) => years > 5;

        string Level(int years) 
            => years switch
            {
                > 10 => "Rockstar",
                > 8  => "Ninja",
                > 5  => "Senior",
                > 3  => "Mid",
                > 0  => "Junior",
                _    => "No idea what I'm doing",
            };

        string Promote(string level) 
            => level switch
            {
                "Ninja"  => "Rockstar",
                "Senior" => "Ninja",
                "Mid"    => "Senior",
                "Junior" => "Mid",
                _        => "Junior",
            };

        int Years(string level) 
            => level switch
            {
                "Rockstar" => 12,
                "Ninja"    => 10,
                "Senior"   => 8,
                "Mid"      => 6,
                "Junior"   => 3,
                _          => 0
            };
    }

    public static class CompositionExtensions {

        public static bool IsEven(this int x) => x % 2 == 0;

        public static string Level(int years) 
            => years switch
            {
                > 10 => "Rockstar",
                > 8  => "Ninja",
                > 5  => "Senior",
                > 3  => "Mid",
                > 0  => "Junior",
                _    => "No idea what I'm doing",
            };

        public static string Promote(string level) 
            => level switch
            {
                "Ninja"  => "Rockstar",
                "Senior" => "Ninja",
                "Mid"    => "Senior",
                "Junior" => "Mid",
                _        => "Junior",
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
    }
}