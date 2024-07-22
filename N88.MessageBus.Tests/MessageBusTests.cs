namespace N88.MessageBus.Tests
{
	using System;
	using NUnit.Framework;
	using MessageBus;

	[TestFixture]
	public class MessageBusTests
	{
		[Test]
		public void Call_WhenDelegateSubscribed_PassesContextToDelegates()
		{
			var flag = false;
			var messageBus = new MessageBus<bool>();
			messageBus.Subscribe("test", context => { flag = context; });
			messageBus.Call("test", true);

			Assert.That(flag, Is.True);
		}

		[Test]
		public void Call_WhenMultipleDelegatesSubscribed_PassesContextToDelegates()
		{
			var flag01 = false;
			var flag02 = false;
			var messageBus = new MessageBus<bool>();
			messageBus.Subscribe("test", context => { flag01 = context; });
			messageBus.Subscribe("test", context => { flag02 = context; });
			messageBus.Call("test", true);

			Assert.That(flag01, Is.True);
			Assert.That(flag02, Is.True);
		}

		[Test]
		public void Call_WhenDelegatesSubscribedToDifferentEndpoints_PassesContextOnlyToEndpointSubscriptions()
		{
			var flag01 = false;
			var flag02 = false;
			var messageBus = new MessageBus<bool>();
			messageBus.Subscribe("test", context => { flag01 = context; });
			messageBus.Subscribe("other", context => { flag02 = context; });
			messageBus.Call("test", true);

			Assert.That(flag01, Is.True);
			Assert.That(flag02, Is.False);
		}

		[Test]
		public void GetSubscriptionId_SubscribingToSameEndpoint_ReturnsUniqueIds()
		{
			var messageBus = new MessageBus<bool>();
			var id01 = messageBus.Subscribe("test", context => { });
			var id02 = messageBus.Subscribe("test", context => { });

			Assert.That(id01, Is.Not.EqualTo(id02));
		}

		[Test]
		public void Call_WhenNoDelegatesSubscribedToEndpoints_NothingHappens()
		{
			var flag01 = false;
			var messageBus = new MessageBus<bool>();
			messageBus.Subscribe("test", context => { flag01 = context; });
			messageBus.Call("other", true);

			Assert.That(flag01, Is.False);
		}

		[Test]
		public void Unsubscribe_WhenUnsubscribed_CallDoesNotInvokeDelegate()
		{
			var flag01 = false;
			var messageBus = new MessageBus<bool>();
			var id = messageBus.Subscribe("test", context => { flag01 = context; });
			messageBus.Unsubscribe(id);
			messageBus.Call("test", true);

			Assert.That(flag01, Is.False);
		}


		[Test]
		public void Unsubscribe_WhenUnsubscribingEmptyEndpoint_NothingHappens()
		{
			var messageBus = new MessageBus<bool>();
			messageBus.Unsubscribe(0);
		}

		[Test]
		public void Subscribe_WhenNullEndpoint_ThrowArgumentNullException()
		{
			var messageBus = new MessageBus<bool>();
			Assert.Throws<ArgumentNullException>(() => { messageBus.Subscribe(null!, context => { }); });
		}


		[Test]
		public void Call_WhenNullEndpoint_ThrowArgumentNullException()
		{
			var messageBus = new MessageBus<bool>();
			Assert.Throws<ArgumentNullException>(() => { messageBus.Call(null!, false); });
		}
	}
}
