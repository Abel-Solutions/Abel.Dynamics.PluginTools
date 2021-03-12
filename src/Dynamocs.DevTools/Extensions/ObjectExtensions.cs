namespace Dynamocs.DevTools
{
	public static class ObjectExtensions
	{
		public static T GetValue<T>(this object obj, string name)
		{
			if (obj.GetType().GetProperty(name) is var prop && prop != null)
				return (T)prop.GetValue(obj);

			if (obj.GetType().GetField(name) is var field && field != null)
				return (T)field.GetValue(obj);

			return default;
		}

		public static void SetValue(this object obj, string name, object value)
		{
			if (obj.GetType().GetProperty(name) is var prop && prop != null)
				prop.SetValue(obj, value);

			else if (obj.GetType().GetField(name) is var field && field != null)
				field.SetValue(obj, value);
		}
	}
}