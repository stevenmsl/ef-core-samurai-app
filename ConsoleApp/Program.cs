using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace ConsoleApp
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            QueryAndUpdateBattle_Disconnected();

            /*
            RetrieveAndUpdateMultipleSamurais();
            */

            /* insert various types of entities at once */
            /*
            InsertVariousTypes();  
            */
            /* Batch commends */
            /*
            GetSamurais("Before Add:");
            InsertMultipleSamurais();
            GetSamurais("After Add:");
            */

            /*
                context.Database.EnsureCreated();
                GetSamurais("Before Add:");
                AddSamurai();
                GetSamurais("After Add:");
                Console.Write("Press any key...");
                Console.ReadKey();
            */
        }

        private static void QueryAndUpdateBattle_Disconnected()
        {
            var battle = _context.Battles.FirstOrDefault
                (b => b.Name == "Battle of Okehazama");
            if (battle == null)
            {
                _context.Battles.Add(new Battle
                {
                    Name = "Battle of Okehazama",
                    StartDate = new DateTime(1560, 05, 01),
                    EndDate = new DateTime(1560, 06, 15)
                });
                _context.SaveChanges();
            }

            battle = _context.Battles.AsNoTracking().First
                (b => b.Name == "Battle of Okehazama");
            battle.EndDate = new DateTime(1560, 06, 30);
            using (var newContextInstance = new SamuraiContext())
            {

                /*
                    New DbContext instance has no tracking info you must inform the context about object state
                    Update method will start tracking object and mark its state as ‘Modified’                  
                */

                /*  
                 *  All fields will be updated as EF Core wouldn’t know that only EndDate has been updated 
                    
                    Microsoft.EntityFrameworkCore.Database.Command[20101]
                    Executed DbCommand (2ms) [Parameters=[@p3='1', @p0='1560-06-30T00:00:00', @p1='Battle of Okehazama' (Size = 4000), @p2='1560-05-01T00:00:00'], CommandType='Text', CommandTimeout='30']
                    SET NOCOUNT ON;
                    UPDATE [Battles] SET [EndDate] = @p0, [Name] = @p1, [StartDate] = @p2
                    WHERE [Id] = @p3;
                    SELECT @@ROWCOUNT;
                 
                */
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }

        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            /*
              Executed DbCommand (52ms) [Parameters=[@__p_0='1', @__p_1='4'], CommandType='Text', CommandTimeout='30']
              SELECT [s].[Id], [s].[ClanId], [s].[Name]
              FROM [Samurais] AS [s]
              ORDER BY (SELECT 1)
              OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY 
            */
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();

            /*
                  Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand(7ms) [Parameters=[@p1='2', @p0='TashaSan' (Size = 4000), @p3='3', @p2='Number3San' (Size = 4000), @p5='4', @p4='Number 4San' (Size = 4000), @p7='5', @p6='KikuchioSan' (Size = 4000)], CommandType='Text', CommandTimeout='30']
                  SET NOCOUNT ON;
                  UPDATE[Samurais] SET[Name] = @p0
                  WHERE[Id] = @p1;
                  SELECT @@ROWCOUNT;

                  UPDATE[Samurais] SET[Name] = @p2
                  WHERE[Id] = @p3;
                  SELECT @@ROWCOUNT;

                  UPDATE[Samurais] SET[Name] = @p4
                  WHERE[Id] = @p5;
                  SELECT @@ROWCOUNT;

                  UPDATE[Samurais] SET[Name] = @p6
                  WHERE[Id] = @p7;
                  SELECT @@ROWCOUNT;
            */

            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();            
        }

        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "Kikuchio" };
            var clan = new Clan { ClanName = "Imperial Clan" };
            // You don’t need to specify which db set to add the entity individually
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
        }

        private static void InsertMultipleSamurais()
        {

            var samurai = new Samurai { Name = "Sampson" };
            var samurai2 = new Samurai { Name = "Tasha" };
            var samurai3 = new Samurai { Name = "Number3" };
            var samurai4 = new Samurai { Name = "Number 4" };
            // At least 4 operations needed for SQL server 
            // provider to batch commands 
            _context.Samurais
                .AddRange(samurai, samurai2, samurai3, samurai4);
            _context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Sampson" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach(var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
    }
}
