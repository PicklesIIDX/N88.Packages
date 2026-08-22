namespace N88.Loader.Tests
{
	[TestFixture]
	[TestOf(typeof(Loader))]
	public class LoaderTest
	{

		[Test]
		public async Task LoadAllAsync_respects_cancellation_token()
		{
			var loader = new Loader();
			loader.Register(new MockSlowSource());
			var source = new CancellationTokenSource();
			source.CancelAfter(1);
			var result = loader.LoadAllAsync<string>("butts", source.Token);
			await Task.Delay(1, CancellationToken.None);
			Assert.That(result.IsCanceled, Is.True);
		}
	}

	public class MockSlowSource : ISource<string>
	{
		public async Task<IReadOnlyList<string>> LoadAsync(string key, CancellationToken token)
		{
			await Task.Delay(5, token);
			return new List<string> {key};
		}
	}
}