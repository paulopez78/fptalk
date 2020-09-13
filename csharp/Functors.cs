using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using static System.Linq.Enumerable;
using System.Threading.Tasks;

namespace csharp
{
    public class UnitTest1
    {
        // [Fact]
        public void CollectionFunctor()
        {
            IEnumerable<int> experiences = new[] { 8, 1, 1, 2, 4, 0 };
            // [8, 1 , 1, 2, 4, 0]

            IEnumerable<int> moreExperiences = experiences.Select(exp => exp + 1);

            bool IsSenior(int years) => years > 5; 
            experiences.Select(IsSenior);


            // [9, 2 , 2, 4, 5, 1]
            Print(experiences);
            // Print(yearsPlusOne);

            int [] yearsCastArray= (int[])experiences;
            for(var i = 0; i < yearsCastArray.Length; i++)
            {
                yearsCastArray[i] = yearsCastArray[i] + 1;
            }
            // [9, 2 , 2, 4, 5, 1]

            Print(experiences);
            Print(yearsCastArray);
            // [9, 2 , 2, 4, 5, 1]
            
            int [] yearsToArray = experiences.ToArray();
            for(var i = 0; i < yearsToArray.Length; i++)
            {
                yearsToArray[i] = yearsToArray[i] + 1;
            }

            // var newYears = Select(experiences, y => y + 1);


            // IEnumerable<int> Select(IEnumerable<int> source)
            // {
            //     foreach (var item in source)
            //     {
            //         yield return item + 1;
            //     }
            // }

            // IEnumerable<int> Select(IEnumerable<int> source, Func<int, int> selector)
            // {
            //     foreach (var item in source)
            //     {
            //         yield return selector(item);
            //     }
            // }

            // IEnumerable<TResult> Select<TSource,TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
            // {
            //     foreach (var item in source)
            //     {
            //         yield return selector(item);
            //     }
            // }

            IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            {
                foreach (var element in source) {
                    foreach (var subElement in selector(element)) {
                        yield return subElement;
                    }
                }
            }

            Func<IEnumerable<TSource>, IEnumerable<TResult>> Bind<TSource, TResult>(Func<TSource, IEnumerable<TResult>> func)
                => source => source.SelectMany(func);
            

            // IEnumerable<int> Select(IEnumerable<int> source)
            // {
            //     List<int> result = new();

            //     foreach (var item in source)
            //     {
            //         result.Add(item + 1);
            //     }

            //     return result;
            // }

            // var result = experience.Select(LevelOfExperience).Select(Promote).Select(YearsOfExperience);

            IEnumerable<(string lang, int exp)> skills = new[] { ("csharp", 8), ("fsharp", 1), ("kotlin", 1), ("golang", 2), ("python", 4), ("haskell", 0) };

            var results = skills.Select(GetDescription);
            results = from x in skills select GetDescription(x);

            // Print(results);

            // empty list, collection
            IEnumerable<(string lang, int exp)> emptySkills= Empty<(string lang, int exp)>();
            var emptyResults = skills.Select(GetDescription);


            // map a function
            IEnumerable<int> listA = new[] { 8, 1, 1, 2, 4, 0 };
            IEnumerable<string> listB = new[] { "csharp", "fsharp", "kotlin", "golang", "python", "haskell"};
            var newresults = listB.Select<string, Func<int, string>>(lang => exp => GetDescription2(lang, exp));

        }

        // [Fact]
        public async Task SelectTasks()
        { 
            var alice = new Developer { Skills = new[] { ("csharp", 8), ("kotlin", 2),  ("python", 4) }};
            var bob   = new Developer { Skills = new[] { ("fsharp", 8), ("golang", 2), ("haskell", 1) }};

            IEnumerable<object> listOfObjects = new object[] { 1,2,3,4,5};

            var tasks = listOfObjects.Select(DoAsyncStuff);

            await Task.WhenAll(tasks);

            IEnumerable<int> developers = new [] { 1,2,3,4,5};

            var firstSkill = GetDeveloper(1)
                .Select(x => x.Skills.First())
                .Select(GetDescription)
                .ForEach(Console.WriteLine);

            var firstSkillm= from x in GetDeveloper(1)
                             select GetDescription(x.Skills.First());

            Task<Developer> GetDeveloper(int developerId) => 
                Task.FromResult(alice);

            Task DoAsyncStuff(object data) => Task.CompletedTask;
        }

        // [Fact]
        public void CollectionMonad()
        {
            var alice = new Developer { Skills = new[] { ("csharp", 8), ("kotlin", 2),  ("python", 4),  }};
            var bob   = new Developer { Skills = new[] { ("fsharp", 8), ("golang", 2), ("haskell", 1) }};

            IEnumerable<Developer> developers = new[] { alice, bob };

            // var seniorLanguage = GetDeveloperSkills("bob@abax.no").Select(skill => IsSenior(skill.));

            

            IEnumerable<(string language, int experience)> GetDeveloperSkills(string email) 
                =>  new[] { ("csharp", 8), ("kotlin", 2),  ("python", 4) };

            bool IsSenior (int years) => years > 5;


            IEnumerable<string> Languages(Developer dev) => dev.Skills.Select(x => x.lang);
        }

        void Print<T>(IEnumerable<T> list) => Console.WriteLine(string.Join(",", list));

        string GetDescription2(string lang, int exp)
            => $"{lang}-{LevelOfExperience(exp)}";

        string GetDescription((string lang, int exp) skill)
            => $"{skill.lang}-{LevelOfExperience(skill.exp)}";

        string LevelOfExperience(int years) => years switch
        {
            > 10 => "Rockstar",
            > 8  => "Ninja",
            > 5  => "Senior",
            > 3  => "Mid",
            > 0  => "Junior",
            _    => "No idea what I'm doing",
        };

        string[] Skills(string level) => level switch
        {
            "Ninja"  => new[] { "haskell", "kubernetes", "agile"},
        };

        string Promote(string level) => level switch
        {
            "Ninja"  => "Rockstar",
            "Senior" => "Ninja",
            "Mid"    => "Senior",
            "Junior" => "Mid",
            _        => "Junior",
        };

        int YearsOfExperience(string level) => level switch
        {
            "Rockstar" => 12,
            "Ninja"    => 10,
            "Senior"   => 8,
            "Mid"      => 6,
            "Junior"   => 2,
            _          => 0
        };
        
    }

    public record Developer { public IEnumerable<(string lang, int exp)> Skills; }

    public static class Collections
    {
        public static async Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> func)
            => func(await source);

        public static Lazy<TResult> Select<TSource, TResult>(this Lazy<TSource> source, Func<TSource, TResult> func)
            => new Lazy<TResult>(func(source.Value));

        public static Nullable<TResult> Select<TSource, TResult>(this Nullable<TSource> source, Func<TSource, TResult> func)
        where TSource : struct
        where TResult : struct 
        => source.HasValue ? func(source.Value) : null;


        public static async Task ForEach<TSource>(this Task<TSource> source, Action<TSource> action)
        {
            var unwrapped = await source;
            action(unwrapped);
        }

        // public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func)
        // {
        //     foreach (var item in source)
        //     {
        //         yield return func(item);
        //     }
        // }
    }
}