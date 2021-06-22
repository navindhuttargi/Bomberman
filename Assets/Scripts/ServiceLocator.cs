using System;
using System.Collections.Generic;

public static class ServiceLocator
{
	private static Dictionary<object, object> _serviceContainer = null;
	public static Dictionary<object, object> serviceContainer => _serviceContainer;
	public static void InitializeContainer()
	{
		_serviceContainer = null;
		_serviceContainer = new Dictionary<object, object>()
		{
			{typeof(IGridGenerator),new GridGenerator() },
			{typeof(IPlayerSpawner),new PlayerSpawner() },
			{typeof(IEnemySpawner),new EnemySpawner() },
			{typeof(IGridHandler),new GridHandler() },
			{typeof(IBomb),new Bomb() }
		};
	}
	public static T GetService<T>()
	{
		try
		{
			return (T)_serviceContainer[typeof(T)];
		}
		catch (Exception ex)
		{
			throw new Exception("Service not implemented");
		}
	}
}
