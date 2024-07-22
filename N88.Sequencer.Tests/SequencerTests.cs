namespace N88.Sequencer.Tests
{
	using N88.Sequencer;
	using NUnit.Framework;

	[TestFixture]
	public class SequencerTests
	{
		[Test]
		public void Update_WhenCalledWithoutTransitionRegisteredForState_ReturnsCurrentState()
		{
			const string InitialState = "00";
			var sequencer = new Sequencer<TestState, string, string>(InitialState);
			var updatedState = sequencer.Update(new TestState());
			Assert.That(updatedState, Is.EqualTo(InitialState));
		}

		[Test]
		public void Update_WhenActionRegistered_CallsRegisteredAction()
		{
			const string CurrentState = "00";
			var sequencer = new Sequencer<TestState, string, string>(CurrentState);
			sequencer.RegisterAction("00", "action", state =>
			{
				state.Updated = true;
				return state;
			});

			var testState = new TestState();
			sequencer.Update(testState);

			Assert.That(testState.Updated, Is.EqualTo(true));
		}

		[Test]
		public void PlaySequence_WhenMultipleActionsRegistered_CallsAllRegisteredActions()
		{
			const string CurrentState = "00";
			var sequencer = new Sequencer<TestState, string, string>(CurrentState);
			sequencer.RegisterAction("00", "action01", state =>
			{
				state.Count += 1;
				return state;
			});
			sequencer.RegisterAction("00", "action02", state =>
			{
				state.Count += 2;
				return state;
			});

			var testState = new TestState();
			sequencer.Update(testState);

			Assert.That(testState.Count, Is.EqualTo(3));
		}


		[Test]
		public void TransitionState_WhenTransitionRegistered_StateChanges()
		{
			const string InitialState = "00";
			const string NextState = "01";
			var sequencer = new Sequencer<TestState, string, string>(InitialState);
			sequencer.RegisterTransition(InitialState,
				context => true,
				NextState);

			var testState = new TestState();
			var currentState = sequencer.Update(testState);

			Assert.That(currentState, Is.EqualTo(NextState));
		}

		private class TestState
		{
			public int Count;
			public bool Updated;
		}
	}
}