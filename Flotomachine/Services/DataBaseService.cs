using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flotomachine.Utility;
using Microsoft.EntityFrameworkCore;

namespace Flotomachine.Services;

public static class DataBaseService
{
	private static MainBaseContext DataBase { get; set; }

	public static Exception? Initialize(string path)
	{
		string fullPath = Path.Join(path, App.Settings.Configuration.DataBase.FileName);
		if (!File.Exists(fullPath))
		{
			HttpHelper.FileDownloadAndSave(App.Settings.Configuration.DataBase.DefaultDataBaseUrl, fullPath);
		}

		try
		{
			DbContextOptions<MainBaseContext> opt = MainBaseContext.BuildDbContextOptionsSqlite("Data Source=" + fullPath);
			DataBase = new MainBaseContext(opt);
		}
		catch (Exception e)
		{
			return e;
		}

		return null;
	}

	public static Exception? Get(Action<MainBaseContext> action)
	{
		try
		{
			lock (DataBase)
			{
				action.Invoke(DataBase);
			}
			return null;
		}
		catch (Exception e)
		{
			Console.WriteLine("DataBase service get error");
			LogManager.ErrorLog(e, "ErrorLog_DataBaseGet");
			return e;
		}
	}
	public static Exception? GetAndSet(Action<MainBaseContext> action)
	{
		try
		{
			lock (DataBase)
			{
				action.Invoke(DataBase);
				DataBase.SaveChanges();
			}
			return null;
		}
		catch (Exception e)
		{
			Console.WriteLine("DataBase service get_and_set error");
			LogManager.ErrorLog(e, "ErrorLog_DataBaseGetAndSet");
			return e;
		}
	}

	public static List<User> GetUsers()
	{
		List<User> result = null!;
		Get(context => result = context.Users.ToList());
		return result ?? new List<User>();
		//.Where(p => !p.IsDelete())
	}

	public static User? GetUser(string username)
	{
		User? result = null;
		Get(context => result = context.Users.FirstOrDefault(user => user.Username == username));
		return result;
		//.Where(p => !p.IsDelete())
	}
	public static User? GetUser(int id)
	{
		User? result = null;
		Get(context => result = context.Users.FirstOrDefault(p => p.Id == id));
		return result;
		//.Where(p => !p.IsDelete())
	}

	public static User? GetUser(byte[] cardId)
	{
		CardId? card = null;
		Get(context => card = context.CardIds.FirstOrDefault(p => p.CardBytes == cardId));
		return card == null ? null : GetUser(card.UserId);
	}

	public static User CreateUser(string username, string passHash) => CreateUser(new User(username, passHash));
	public static User CreateUser(User user)
	{
		User result = user;
		GetAndSet(context =>
		{
			context.Users.Add(result);
			context.SaveChanges();
			result = GetUser(result.Username);
		});
		return result;
	}
	public static List<CardId> GetCardIds(User? user)
	{
		if (user == null)
		{
			return new List<CardId>();
		}

		List<CardId> result = new();
		Get(context => result = context.CardIds.Where(p => p.UserId == user.Id).ToList());
		return result;
	}

	public static CardId? GetCard(byte[] cardBytes)
	{
		CardId? result = null;
		Get(context => result = context.CardIds.FirstOrDefault(p => p.CardBytes == cardBytes));
		return result;
	}

	public static void CreateCard(User user, byte[] cardId) => GetAndSet(context => context.CardIds.Add(new CardId(user, cardId)));
	public static void CreateCard(int user, byte[] cardId) => GetAndSet(context => context.CardIds.Add(new CardId(user, cardId)));
	public static void DeleteCard(CardId card) => GetAndSet(context => context.CardIds.Remove(card));

	//public static List<Module> GetActiveModules() => DataBase.Modules.Where(p => p.Active).ToList();
	public static Module? GetModule(int moduleId)
	{
		Module? result = null;
		Get(context => result = context.Modules.FirstOrDefault(p => p.Id == moduleId));
		return result;
	}
	//public static List<ModuleField> GetActiveModulesFields() => DataBase.ModuleFields.Where(p => p.Active).ToList();
	//public static List<ModuleField> GetActiveModulesFields(int moduleId) => DataBase.ModuleFields.Where(p => p.ModuleId == moduleId && p.Active).ToList();
	//public static List<ModuleField> GetActiveModulesFields(Module module) => DataBase.ModuleFields.Where(p => p.ModuleId == module.Id && p.Active).ToList();

	public static List<ModuleField> GetModulesFields()
	{
		List<ModuleField> result = new();
		Get(context =>
		{
			foreach (List<ModuleField> item in context.Modules.Select(p => p.Fields))
			{
				result.AddRange(item);
			}
		});
		return result;
	}

	public static List<int> GetExperiments(User user)
	{
		List<int> result = new();
		Get(context => result = context.Experiments.Where(p => p.UserId == user.Id).Select(p => p.Id).ToList());
		return result;
	}

	public static List<ExperimentData> GetExperimentData(int experimentId)
	{
		List<ExperimentData> result = new();
		Get(context => result = context.ExperimentDatas.Where(p => p.ExperimentId == experimentId).ToList());
		return result;
	}

	public static List<ExperimentDataValue> GetExperimentDataValues(int experimentDataId)
	{
		List<ExperimentDataValue> result = new();
		Get(context => result = context.ExperimentDataValues.Where(p => p.ExperimentDataId == experimentDataId).ToList());
		return result;
	}

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
