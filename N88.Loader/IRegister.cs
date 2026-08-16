namespace N88.Loader
{
	public interface IRegister
	{
		void Register<T>(ISource<T> source);
		void RegisterDecoded<T>(ISource<byte[]> source, IDecoder<T> decoder);
	}
}