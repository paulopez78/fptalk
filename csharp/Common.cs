namespace csharp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xunit;
    using static Collections;
    using static CommonExtensions;
    using static System.Linq.Enumerable;
    using System.Threading.Tasks;

    public class Common
    {
        // [Fact]
        public void Common_Test()
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


            // level
            // .SelectMany(Skills)
            // .SelectMany(Skills)
            // .Select(Promote);

            // var a = "devops".Skills();

            new[] {"devops", "backend", "frontend"}.SelectMany(Skills).Select(Upgrade);
            Empty<string>().SelectMany(Skills).Select(Upgrade);


            yearsOfExperience.Select(Level).SelectMany(Skills).Select(Category);


            // new[] {"senior", "junior", "mid"}.SelectMany(Skills).Select(Category);

            Skills("devops");
            Skills("backend");

            Func<string, IEnumerable<string>> skills = Skills;

            var bindedSkills = Bind(skills);

            var backops = bindedSkills(new[] {"backend", "devops"}); 

            backops = new[] {"backend", "devops"}.SelectMany(Skills);

            var upgradedBackops = new[] {"backend", "devops"}.SelectMany(Skills).Select(Upgrade);

            IEnumerable<IEnumerable<string>> nestedBackops = new[] {"backend", "devops"}.Select(Skills);

            new[] {"backend", "devops"}.SelectMany(Skills).Select(Upgrade);

            new[] {"backend", "devops"}.Select(Skills);
            // [["docker", "azure"],["csharp", "visualstudio"]]

            // new[] {"backend", "devops"}.Select(Skills).Select(Upgrade);


            // Task<int> GetYearsOfExperience(string email) 
            //     => email switch {
            //         "bob@abax.no"   => Task.FromResult(3),
            //         "alice@abax.no" => Task.FromResult(4),
            //         _               => Task.FromResult(0),
            //     };

            // Maybe<int> GetYearsOfExperience(string email) 
            //     => email switch {
            //         "bob@abax.no"   => Maybe(3),
            //         "alice@abax.no" => Maybe(4),
            //         _               => Maybe.Nothing,
            //     };

            Nullable<int> GetYearsOfExperience(string email) 
                => email switch {
                    "bob@abax.no"   => 3,
                    "alice@abax.no" => 4,
                    _               => null,
                };
        }

        IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (var element in source) {
                foreach (var subElement in selector(element)) {
                    yield return subElement;
                }
            }
        }

        async Task<TResult> SelectMany<TSource, TResult>(Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            var x = await source;
            return await selector(x);
        }

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Bind<TSource, TResult>(Func<TSource, IEnumerable<TResult>> func)
            => source => source.SelectMany(func);

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

    public static class CommonExtensions {
        public static IEnumerable<string> Skills(string level)
            => level switch
            {
                "Rockstar" => new[] {"agile"},
                "Ninja"    => new[] {"fsharp, haskell"},
                "Senior"   => new[] {"k8s", "go"},
                "Mid"      => new[] {"bash", "docker"},
                "Junior"   => new[] {"csharp", "js"},
                _          => Empty<string>().ToArray()
            };

        public static string Upgrade(string skill)
            => skill switch
            {
                "scrum"  => "kanban",
                "fsharp" => "haskell",
                "csharp" => "fsharp",
                "docker" => "k8s",
                "js"     => "react",
                _        => ""
            };

        public static string Category(string skill)
            => skill switch
            {
                "fsharp"  => "backend",
                "csharp"  => "backend",
                "k8s"     => "devops",
                "docker"  => "devops",
                "js"      => "frontend",
                _         => ""
            };

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