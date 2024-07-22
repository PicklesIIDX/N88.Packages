namespace N88.Sequencer.Performance
{
	using BenchmarkDotNet.Attributes;
	using N88.Sequencer;

	[MemoryDiagnoser]
	public class SequencerPerf
	{
		private readonly SequenceContext context = new() { SomeValue = 2 };

		private readonly ExternalObject externalObject = new();
		private readonly Sequencer<SequenceContext, string, string> sequencer = new("00");
		private readonly Sequencer<SequenceContext, string, string> sequencerClosures = new("01");

		[GlobalSetup]
		public void Setup()
		{
			sequencer.RegisterAction("00", "00/00", Action);
			sequencerClosures.RegisterAction("01", "01/01",
				sequenceContext =>
				{
					sequenceContext.SomeValue = ActionClosure(externalObject, sequenceContext.SomeValue,
						sequenceContext.SomeValue);
					return sequenceContext;
				});
		}

		[Benchmark(OperationsPerInvoke = 1)]
		public void SequenceWithoutClosures()
		{
			context.SomeValue = 2;
			for (var i = 0; i < 8; i++)
			{
				sequencer.Update(context);
			}
		}

		[Benchmark(OperationsPerInvoke = 1)]
		public void SequenceWithClosures()
		{
			context.SomeValue = 2;
			for (var i = 0; i < 8; i++)
			{
				sequencerClosures.Update(context);
			}
		}

		private SequenceContext Action(SequenceContext arg)
		{
			arg.SomeValue = externalObject.Add(arg.SomeValue, arg.SomeValue);
			return arg;
		}

		private int ActionClosure(ExternalObject external, int x, int y)
		{
			return external.Add(x, y);
		}

		private class ExternalObject
		{
			public int Add(int x, int y)
			{
				return x + y;
			}
		}

		private class SequenceContext
		{
			public int SomeValue;
		}
	}
}