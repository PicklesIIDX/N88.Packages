namespace N88.Sequencer.Performance
{
	using BenchmarkDotNet.Running;

	public static class Program
	{
		private static void Main(string[] args)
		{
			BenchmarkRunner.Run<SequencerPerf>();
		}
	}
}