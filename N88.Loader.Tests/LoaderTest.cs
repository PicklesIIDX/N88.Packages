namespace N88.Loader.Tests
{
	using NSubstitute;

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

		[Test]
		public async Task TryRelease_when_asset_loaded_releases_asset()
		{
			var loader = new Loader();
			var source = Substitute.For<ISource<string>>();
			source.LoadAsync("butts", Arg.Any<CancellationToken>()).Returns(new List<string> { "butt" });
			loader.Register(source);
			await loader.LoadAsync<string>("butts", CancellationToken.None);
			loader.TryRelease<string>("butts");
			source.Received(1).TryRelease("butts");
		}

		[Test]
		public void Register_when_type_registered_throws_argument_exception()
		{
			var loader = new Loader();
			var source = Substitute.For<ISource<string>>();
			loader.Register(source);
			Assert.Throws<ArgumentException>(() => loader.Register(source));
		}

		[Test]
		public async Task TryRelease_when_more_specific_type_releases()
		{
			var loader = new Loader();
			var source = new FamilySource();
			loader.Register(source);
			await loader.LoadAllAsync<ChildClass>("baby", CancellationToken.None);
			var released = loader.TryRelease<ChildClass>("baby");
			Assert.That(released, Is.True);
		}

		private class ParentClass;

		private class ChildClass : ParentClass;

		private sealed class FamilySource : ISource<ParentClass>
		{
			private ChildClass? baby;
			public void Dispose()
			{
				//
			}

			public Task<IReadOnlyList<ParentClass>> LoadAsync(string key, CancellationToken token)
			{
				if (key == "baby")
				{
					baby = new ChildClass();
					return Task.FromResult<IReadOnlyList<ParentClass>>(new List<ParentClass> {baby});
				}

				throw new ArgumentException($"no item for {key}");
			}

			public bool TryRelease(string key)
			{
				if (key == "baby" && baby != null)
				{
					baby = null;
					return true;
				}

				return false;
			}
		}

		private sealed class MockSlowSource : ISource<string>
		{
			public async Task<IReadOnlyList<string>> LoadAsync(string key, CancellationToken token)
			{
				await Task.Delay(5, token);
				return new List<string> {key};
			}

			public bool TryRelease(string key)
			{
				return true;
			}

			public void Dispose()
			{
				// nothing is loaded
			}
		}
	}
}