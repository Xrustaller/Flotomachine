using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flotomachine.Utility;

namespace Flotomachine.Services;

public static class DataBaseService
{
    private static MainBaseContext DataBase { get; set; }

    public static Exception Initialize(string path)
    {
        string fullPath = Path.Join(path, App.Settings.Configuration.DataBase.FileName);
        if (!File.Exists(fullPath))
        {
            HttpHelper.DownloadFile(App.Settings.Configuration.Main.DefaultDataBaseUrl, fullPath);
        }

        try
        {
            DataBase = new MainBaseContext(MainBaseContext.BuildDbContextOptionsSqlite("Data Source=" + fullPath));
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }

    public static Exception Get(Action<MainBaseContext> action)
    {
        try
        {
            action.Invoke(DataBase);
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Exception GetAndSet(Action<MainBaseContext> action)
    {
        try
        {
            action.Invoke(DataBase);
            DataBase.SaveChanges();
            return null;
        }
        catch (Exception e)
        {
            // DataBase.
            return e;
        }
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

    public static List<Module> GetModules() => DataBase.Modules.Where(p => p.Active).ToList();
    public static List<ModuleField> GetModulesFields() => DataBase.ModuleFields.Where(p => p.Active).ToList();
    public static List<ModuleField> GetModulesFields(int moduleId) => DataBase.ModuleFields.Where(p => p.ModuleId == moduleId && p.Active).ToList();
    public static List<ModuleField> GetModulesFields(Module module) => DataBase.ModuleFields.Where(p => p.ModuleId == module.Id && p.Active).ToList();

    public static List<ModuleField> GetAllModulesFields()
    {
        List<ModuleField> result = new(20);
        foreach (List<ModuleField> item in DataBaseService.DataBase.Modules.Select(p => p.Fields))
        {
            result.AddRange(item);
        }
        return result;
    }

    public static List<int> GetExperiments(User user) => DataBase.Experiments.Where(p => p.UserId == user.Id).Select(p => p.Id).ToList();

    public static List<ExperimentData> GetExperimentData(int experimentId) => DataBase.ExperimentDatas.Where(p => p.ExperimentId == experimentId).ToList();
    public static List<ExperimentDataValue> GetExperimentDataValues(int experimentDataId) => DataBase.ExperimentDataValues.Where(p => p.ExperimentDataId == experimentDataId).ToList();

    public static Experiment CreateExperiment(User user, int timerTick)
    {
        Experiment experiment = new Experiment(user, timerTick);
        DataBase.Experiments.Add(experiment);
        DataBase.SaveChanges();
        return experiment;
    }

    public static Experiment UpdateExperiment(Experiment experiment)
    {
        DataBase.Experiments.Update(experiment);
        DataBase.SaveChanges();
        return experiment;
    }

    public static ExperimentData AddExperimentData(Experiment experiment)
    {
        ExperimentData data = new ExperimentData(experiment);
        DataBase.ExperimentDatas.Add(data);
        DataBase.SaveChanges();
        return data;
    }

    public static void AddExperimentDataValues(ExperimentData experimentData, List<ExperimentDataValue> datas)
    {
        foreach (ExperimentDataValue data in datas)
        {
            data.ExperimentDataId = experimentData.Id;
            DataBase.ExperimentDataValues.Add(data);
        }
        DataBase.SaveChanges();
    }
}
