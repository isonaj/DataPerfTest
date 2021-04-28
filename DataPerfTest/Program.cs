using DataPerfTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataPerfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var personCount = 100000;
            var planCount = 10;
            var learningItemCount = 20000;

            var task = Task.Run(async () =>
            {
                using (var db = new MyDbContext())
                    await db.Database.EnsureCreatedAsync();

                var items = await CreateLearningItems(learningItemCount);
                var persons = await CreatePersons(personCount);
                await CreateLearningPlans(personCount, planCount, persons, items);
                await CreateLearningHistory(personCount, planCount, persons, items);

                await LoadIndividualReport();
                await LoadTeamReport();
                await LoadSiteAssetReport();
            });

            task.Wait();
        }

        static async Task<List<Person>> CreatePersons(int count)
        {
            var timer = new Stopwatch();
            Console.WriteLine("Creating people");
            timer.Start();

            var persons = new List<Person>();
            using (var db = new MyDbContext())
            {
                for (var i = 0; i < count; i++)
                {
                    var person = new Person { EmployeeId = $"{i}", FirstName = $"FirstName{i}", LastName = $"LastName{i}" };
                    persons.Add(person);
                    db.Persons.Add(person);
                }
                await db.SaveChangesAsync();
                timer.Stop();
                Console.WriteLine($"Created {count} people in {timer.Elapsed.TotalSeconds} secs");

                Console.WriteLine("Applying managers to employees");
                timer.Start();
                for (var i = 0; i < count; i++)
                    persons[i].Manager = persons[i / 5];
                await db.SaveChangesAsync();
                timer.Stop();
                Console.WriteLine($"Managers applied in {timer.Elapsed.TotalSeconds} secs");
            }
            return persons;
        }

        static async Task CreateLearningPlans(int count, int planCount, List<Person> persons, List<LearningItem> items)
        {
            var timer = new Stopwatch();
            using (var db = new MyDbContext())
            {
                Console.WriteLine("Applying learning plans to employees");
                timer.Start();
                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < planCount; j++)
                    {
                        db.LearningPlans.Add(new LearningPlan
                        {
                            PersonId = persons[i].Id,
                            LearningItemId = items[j].Id,
                            RequiredDate = DateTime.Now.AddDays(1)
                        });
                    }
                }
                await db.SaveChangesAsync();
                timer.Stop();
                Console.WriteLine($"{count * planCount} Learning plans applied in {timer.Elapsed.TotalSeconds} secs");
            }
        }

        static async Task CreateLearningHistory(int count, int planCount, List<Person> persons, List<LearningItem> items)
        {
            var timer = new Stopwatch();
            using (var db = new MyDbContext())
            {
                Console.WriteLine("Applying learning history to employees");
                timer.Start();
                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < planCount; j++)
                    {
                        db.LearningHistories.Add(new LearningHistory
                        {
                            PersonId = persons[i].Id,
                            LearningItemId = items[j].Id,
                            ExpiryDate = DateTime.Now.AddDays(7),
                            CompletedDate = DateTime.Now.AddDays(-3)
                        });
                    }
                }
                await db.SaveChangesAsync();
                timer.Stop();
                Console.WriteLine($"{count * planCount} Learning history applied in {timer.Elapsed.TotalSeconds} secs");
            }
        }

        static async Task<List<LearningItem>> CreateLearningItems(int count)
        {
            var timer = new Stopwatch();
            Console.WriteLine("Creating learning items");
            timer.Start();

            var learningItems = new List<LearningItem>();
            using (var db = new MyDbContext())
            {
                for (var i = 0; i < count; i++)
                {
                    var learningItem = new LearningItem { Name = $"LearningItem{i}" };
                    db.LearningItems.Add(learningItem);
                    learningItems.Add(learningItem);
                }
                await db.SaveChangesAsync();
            }

            timer.Stop();
            Console.WriteLine($"Created {count} learningItems in {timer.Elapsed.TotalSeconds} secs");

            return learningItems;
        }

        static async Task LoadIndividualReport()
        {
            var timer = new Stopwatch();
            Console.WriteLine("Loading individual report");
            timer.Start();

            using (var db = new MyDbContext())
            {
                var query = from p in db.Persons
                            join plan in db.LearningPlans on p.Id equals plan.PersonId
                            join history in db.LearningHistories on new { plan.PersonId, plan.LearningItemId } equals new { history.PersonId, history.LearningItemId}
                            where p.EmployeeId == "1"
                            select new {
                                Person = p,
                                History = history
                            };
                var results = await query.ToListAsync();

                timer.Stop();
                Console.WriteLine($"Ran individual report of {results.Count} items in {timer.Elapsed.TotalSeconds} secs");
            }
        }

        static async Task LoadTeamReport()
        {
            var timer = new Stopwatch();
            Console.WriteLine("Loading team report");
            timer.Start();

            using (var db = new MyDbContext())
            {
                var query = from manager in db.Persons
                            where manager.EmployeeId == "2"
                            join p in db.Persons on manager.Id equals p.ManagerId
                            join plan in db.LearningPlans on p.Id equals plan.PersonId
                            join history in db.LearningHistories on new { plan.PersonId, plan.LearningItemId } equals new { history.PersonId, history.LearningItemId }
                            select new
                            {
                                Person = p,
                                History = history
                            };
                var results = await query.ToListAsync();

                timer.Stop();
                Console.WriteLine($"Ran team report of {results.Count} items in {timer.Elapsed.TotalSeconds} secs");
            }
        }

        static async Task LoadSiteAssetReport()
        {
            var timer = new Stopwatch();
            Console.WriteLine("Loading site and asset report");
            timer.Start();

            using (var db = new MyDbContext())
            {
                var now = DateTime.Now;
                var query = from p in db.Persons
                            join plan in db.LearningPlans on p.Id equals plan.PersonId
                            join history in db.LearningHistories on new { plan.PersonId, plan.LearningItemId } equals new { history.PersonId, history.LearningItemId }
                            group history by history.CompletedDate > now ? Compliance.Compliant : Compliance.NonCompliant  into grp
                            select new
                            {
                                Compliance = grp.Key,
                                Count = grp.Count()
                            };
                var results = await query.ToListAsync();

                timer.Stop();
                Console.WriteLine($"Ran site and asset report of {results.Single(c => c.Compliance == Compliance.Compliant).Count} compliant items in {timer.Elapsed.TotalSeconds} secs");
            }
        }

    }
}
