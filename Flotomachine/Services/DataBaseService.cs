using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flotomachine.Services
{
    public static class DataBaseService
    {
        public static MainBaseContext DataBase { get; private set; }

        public static Exception Initialize()
        {
            string path = Directory.GetCurrentDirectory();
            try
            {
                DataBase = new MainBaseContext(MainBaseContext.BuildDbContextOptionsSqlite("Data Source=" + Path.Join(path, "Flotomachine.db")));
                DataBase.Users.LoadAsync();
                DataBase.CardIds.LoadAsync();
                DataBase.ModuleTypes.LoadAsync();
                DataBase.ModuleFields.LoadAsync();
                DataBase.Experiments.LoadAsync();
                DataBase.ExperimentDatas.LoadAsync();
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        [CanBeNull]
        public static User GetUser(string username) => DataBase.Users.FirstOrDefault(p => p.Username == username); //.Where(p => !p.IsDelete())

        public static User CreateUser(string username, string passhash) => CreateUser(new User(username, passhash));
        public static User CreateUser(User user)
        {
            DataBase.Users.Add(user);
            DataBase.SaveChanges();
            return GetUser(user.Username);
        }

        public static User ChangePassword(string username, string newPasshash)
        {
            User user = GetUser(username);
            if (user == null)
            {
                return null;
            }
            user.PassHash = newPasshash;
            ChangePassword(user);
            return user;
        }

        public static void ChangePassword(User user)
        {
            DataBase.Users.Update(user);
            DataBase.SaveChanges();
        }

        public static List<User> GetUsers() => DataBase.Users.ToList(); //.Where(p => !p.IsDelete())

        [CanBeNull]
        public static User GetUser(int id) => DataBase.Users.FirstOrDefault(p => p.Id == id); //.Where(p => !p.IsDelete())

        [CanBeNull]
        public static User GetUser(byte[] cardId)
        {
            CardId card = DataBase.CardIds.FirstOrDefault(p => p.CardBytes == cardId);
            return card == null ? null : GetUser(card.UserId);
        }

        public static List<CardId> GetCardIds() => DataBase.CardIds.ToList();
        public static List<CardId> GetCardIds(User user) => user == null ? new List<CardId>() : DataBase.CardIds.Where(p => p.UserId == user.Id).ToList();

        public static List<CardId> GetCardIds(int id) => DataBase.CardIds.Where(p => p.UserId == id).ToList();

        public static CardId GetCard(int id) => DataBase.CardIds.FirstOrDefault(p => p.Id == id);

        public static CardId GetCard(byte[] cardBytes) => DataBase.CardIds.FirstOrDefault(p => p.CardBytes == cardBytes);

        public static void CreateCard(User user, byte[] cardId)
        {
            DataBase.CardIds.Add(new CardId(user, cardId));
            DataBase.SaveChanges();
        }

        public static void CreateCard(int user, byte[] cardId)
        {
            DataBase.CardIds.Add(new CardId(user, cardId));
            DataBase.SaveChanges();
        }

        public static void DeleteCard(CardId card)
        {
            DataBase.CardIds.Remove(card);
            DataBase.SaveChanges();
        }
    }
}