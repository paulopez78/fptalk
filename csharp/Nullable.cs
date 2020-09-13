namespace csharp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xunit;
    using System.Threading.Tasks;

    public class Nullable
    {
        // [Fact]
        public async Task NullableFunctor()
        {
            // Lazy<int> years = new Lazy<int>(() => 3);

            Nullable<int> yearsOfExperience = 3;

            yearsOfExperience.Select(IsSenior).Select(Log);

            // Console.WriteLine("RESULT IS:" + await task);
        }

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func) 
            => source => source.Select(func);

        T Log<T>(T result)
        {
            Console.WriteLine($"nullable log: {result}");
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
}