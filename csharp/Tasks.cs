namespace csharp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xunit;
    using System.Threading.Tasks;

    public class Tasks
    {
        // [Fact]
        public async Task TaskFunctor()
        {
            Func<int,bool> isSenior = years => years > 5;

            // var elevatedIsSenior = Map(isSenior);

            // isSenior(8); // true
            // elevatedIsSenior(new[]{ 8, 5, 2}); // [ true, false, true ]

            Task<int> yearsOfExperience = Task.FromResult(1);

            yearsOfExperience .Select(Level) .Select(Promote) .Select(Years) .Select(IsSenior);

            // Console.WriteLine("RESULT IS:" + await task);


        }

        void Sample()
        {

            // GetYearsOfExperience("bob@abax.no").Select(Level).Select(Promote) .Select(Years) .Select(IsSenior);

            // var yearsOfExperience = GetYearsOfExperience("bob@abax.no");


            // Maybe<int> GetYearsOfExperience(string email) {
            //     // Databae lookup by email
            //     return Maybe.Nothing<int>;
            //     // return Maybe<int>(5);
            // }

            // bool IsSeniorDeveloper(string email)
            //     => GetYearsOfExperience(email)
            //     .Select(Level).Select(Promote).Select(Years).Select(IsSenior)
            //     .Match(
            //         just: result => result
            //         nothing: false
            //     );
        }

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func) 
            => source => source.Select(func);

        T Log<T>(T result)
        {
            Console.WriteLine($"log: {result}");
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