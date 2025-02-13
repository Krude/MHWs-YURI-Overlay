﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YURI_Overlay;

internal class JsonDatabase<T> : IDisposable where T : class, new()
{
	public string Name { get; set; } = string.Empty;
	public string FilePath { get; set; } = Constants.PLUGIN_DATA_PATH;

	public T data;
	public FileSync fileSyncInstance;
	public JsonWatcher<T> jsonWatcherInstance;

	public EventHandler changed = delegate { };
	public EventHandler renamed = delegate { };
	public EventHandler renamedFrom = delegate { };
	public EventHandler renamedTo = delegate { };
	public EventHandler deleted = delegate { };
	public EventHandler error = delegate { };

	public JsonDatabase(string path, string name, T data = null)
	{
		try {
			Name = name;
			FilePath = path;

			string filePathName = Path.Combine(path, $"{name}.json");
			fileSyncInstance = new(filePathName);
			Load(data);

			jsonWatcherInstance = new(this);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	public T Load(T data = null)
	{
		try
		{
			jsonWatcherInstance?.Disable();
			LogManager.Info($"File \"{Name}.json\": Loading... ${data}");


			string json = data == null ? fileSyncInstance.Read() : JsonSerializer.Serialize(data, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);

			this.data = JsonSerializer.Deserialize<T>(json, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);
			fileSyncInstance.Write(json);


			LogManager.Info($"File \"{Name}.json\": Loaded!");
			jsonWatcherInstance?.DelayedEnable();
			return this.data;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
			this.data = new T();
			Save();
			return this.data;
		}
	}

	public bool Save()
	{
		try
		{
			LogManager.Info($"File \"{Name}.json\": Saving...");
			jsonWatcherInstance?.Disable();

			var json = JsonSerializer.Serialize(data, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);

			LogManager.Info($"File \"{Name}.json\": {json}");


			bool isSuccess = fileSyncInstance.Write(json);


			if(isSuccess) LogManager.Info($"File \"{Name}.json\": Saved!");
			else LogManager.Info($"File \"{Name}.json\": Saving failed!");

			jsonWatcherInstance?.DelayedEnable();
			return isSuccess;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
			return false;
		}
	}

	public void Delete()
	{
		LogManager.Info($"File \"{Name}.json\": Deleting...");
		Dispose();
		fileSyncInstance.Delete();
		LogManager.Info($"File \"{Name}.json\": Deleted!");
	}

	public void EmitChanged()
	{
		Utils.EmitEvents(this, changed);
	}

	public void EmitRenamedFrom()
	{
		Utils.EmitEvents(this, renamedFrom);
		Utils.EmitEvents(this, renamed);
	}

	public void EmitRenamedTo()
	{
		Utils.EmitEvents(this, renamedTo);
		Utils.EmitEvents(this, renamed);
	}

	public void EmitDeleted()
	{
		Utils.EmitEvents(this, deleted);
	}

	public void Dispose()
	{
		LogManager.Info($"File \"{Name}.json\": Disposing...");
		jsonWatcherInstance?.Dispose();
		LogManager.Info($"File \"{Name}.json\": Disposed!");
	}
}
