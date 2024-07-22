namespace N88.MessageBus
{
	using System.Collections.Generic;

	public class MessageBus<TContext>
	{
		public delegate void MessageBusDelegate(TContext context);

		private readonly Dictionary<int, MessageBusDelegate> delegateMap = new();
		private readonly Dictionary<string, List<int>> delegates = new();

		private int subscriptionId;

		public int Subscribe(string endpoint, MessageBusDelegate subscriptionDelegate)
		{
			var id = GetSubscriptionId();
			if (!delegates.ContainsKey(endpoint))
			{
				delegates[endpoint] = new List<int>();
			}
			delegates[endpoint].Add(id);
			delegateMap.Add(id, subscriptionDelegate);
			return id;
		}

		public void Unsubscribe(int id)
		{
			foreach ((_, var delegateList) in delegates)
			{
				delegateList.Remove(id);
			}
			delegateMap.Remove(id);
		}

		public void Call(string endpoint, TContext context)
		{
			if (!delegates.TryGetValue(endpoint, out var delegateList))
			{
				return;
			}
			foreach (var id in delegateList)
			{
				delegateMap[id].Invoke(context);
			}
		}

		private int GetSubscriptionId()
		{
			var newId = ++subscriptionId;
			return newId;
		}
	}
}