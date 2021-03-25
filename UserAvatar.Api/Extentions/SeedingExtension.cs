using System;
using System.Collections.Generic;
using System.Linq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Api.Extentions
{
    public static class SeedingExtension
    {
        internal static void EnsureAdminCreated(UserAvatarContext context)
        {
            var adminUser = context.Users.Any(x=> x.Email == "admin@admin.com" 
                                                  && x.Login == "admin");
            if (!adminUser)
            {
                context.Users.Add(new User
                {
                    Email = "admin@admin.com",
                    Login = "admin",
                    PasswordHash = PasswordHash.CreateHash("admin"),
                    Role = "admin",
                });
            }
            context.SaveChanges();
        }

        public static void EnsureEventsCreated(UserAvatarContext context)
        {
            var eventList = PopulateEvents();

            foreach (var (events , score) in eventList)
            {
                if (!context.Events.Any(x => x.Name == events))
                    context.Events.Add(new Event
                    {
                        Name = events,
                        Score = score
                    });
            }
            context.SaveChanges();
        }
        public  static void EnsureRanksCreated(UserAvatarContext context)
        {
            var rankList = PopulateRanks();

            foreach (var (rank , score) in rankList)
            {
                if (!context.Ranks.Any(x => x.Name == rank))
                    context.Ranks.Add(new Rank
                    {
                        Name = rank,
                        Score = score
                    });
            }
            context.SaveChanges();
        }

        private static Dictionary<string, int> PopulateRanks()
        {
            return new()
            {
                {"Hetman", 1000},
                {"Ataman", 900},
                {"Esau", 700},
                {"Centurion", 900},
                {"Cossack", 300},
                {"Private", 100},
                {"NPC", 0}
            };
        }
        
        private static Dictionary<string, int> PopulateEvents()
        {
            return new()
            {
                { EventType.Registration, 20 },
                { EventType.Login, 15 },
                { EventType.Logout, 13 },
                { EventType.CreateBoard, 10 },
                { EventType.SendInvite, 5 },
                { EventType.CreateCardOnOwnBoard, 3 },
                { EventType.CreateCardOnAlienBoard, 4 },
                { EventType.ChangeCardStatusOnOwnBoard, 1 },
                { EventType.ChangeCardStatusOnAlienBoard, 2 },
                { EventType.ChangeUserBalansByAdmin, -1 }
            };
            
        }
    }
}