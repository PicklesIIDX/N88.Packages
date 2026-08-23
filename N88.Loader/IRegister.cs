namespace N88.Loader
{
	/// <summary>
	/// Give this interface to initializers who will compose the
	/// sources and deserializers.
	/// </summary>
	public interface IRegister
	{
		void Register<T>(ISource<T> source);
		void RegisterDecoded<T>(ISource<byte[]> source, IDecoder<T> decoder);
	}
}