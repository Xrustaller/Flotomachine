using Flotomachine.Utility;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flotomachine.Services;

public static class DataBaseService
{
    private static MainBaseContext DataBase { get; set; }

    public static Exception Initialize(string path)
    {
        string fullPath = Path.Join(path, App.Settings.Configuration.DataBase.FileName);
        if (!File.Exists(fullPath))
        {
            HttpHelper.FileDownloadAndSave(App.Settings.Configuration.DataBase.DefaultDataBaseUrl, fullPath);
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
            return e;
        }
    }

    [CanBeNull]
    public static User GetUser(string username) => DataBase.Users.FirstOrDefault(p => p.Username == username); //.Where(p => !p.IsDelete())

    public static User CreateUser(string username, string passHash) => CreateUser(new User(username, passHash));
    public static User CreateUser(User user)
    {
        DataBase.Users.Add(user);
        DataBase.SaveChanges();
        return GetUser(user.Username);
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

    public static void CreateCard(User user, byte[] cardId) => GetAndSet(context => context.CardIds.Add(new CardId(user, cardId)));
    public static void CreateCard(int user, byte[] cardId) => GetAndSet(context => context.CardIds.Add(new CardId(user, cardId)));
    public static void DeleteCard(CardId card) => GetAndSet(context => context.CardIds.Remove(card));

    public static List<Module> GetActiveModules() => DataBase.Modules.Where(p => p.Active).ToList();
    public static Module GetModule(int moduleId) => DataBase.Modules.FirstOrDefault(p => p.Id == moduleId);
    public static List<ModuleField> GetActiveModulesFields() => DataBase.ModuleFields.Where(p => p.Active).ToList();
    public static List<ModuleField> GetActiveModulesFields(int moduleId) => DataBase.ModuleFields.Where(p => p.ModuleId == moduleId && p.Active).ToList();
    public static List<ModuleField> GetActiveModulesFields(Module module) => DataBase.ModuleFields.Where(p => p.ModuleId == module.Id && p.Active).ToList();

    public static List<ModuleField> GetModulesFields()
    {
        List<ModuleField> result = new();
        foreach (List<ModuleField> item in DataBase.Modules.Select(p => p.Fields))
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
        GetAndSet(context => context.Experiments.Add(experiment));
        return experiment;
    }

    public static Experiment UpdateExperiment(Experiment experiment)
    {
        GetAndSet(context => context.Experiments.Update(experiment));
        return experiment;
    }

    public static ExperimentData AddExperimentData(Experiment experiment)
    {
        ExperimentData data = new ExperimentData(experiment);
        GetAndSet(context => context.ExperimentDatas.Add(data));
        return data;
    }

    public static void AddExperimentDataValues(ExperimentData experimentData, List<ExperimentDataValue> datas)
    {
        GetAndSet(context =>
        {
            foreach (ExperimentDataValue data in datas)
            {
                data.ExperimentDataId = experimentData.Id;
                context.ExperimentDataValues.Add(data);
            }
        });
    }
}
