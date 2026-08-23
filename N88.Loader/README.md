# N88.Loader
Generic loading library

## Loader
This is the object that you hand around to others that require accessing remote data.
The intent is to decouple the requestor with the request method so they can speak
in domain terms.

```csharp
// at initialization
var loader = new Loader();
loader.Register(new MySource<MyDomainType>());

// elsewhere in the code
var domainObject = await loader.LoadAsync<MyDomainType>(UniqueAssetIdentifier, CancellationToken.None);
```

Loaders work well as Singletons but can be created at will for subsystems to manage a loader.

## ISource<T>
You'll create implementations of this to handle translating from your asset sources into 
the given type. It's a combined asset loader and deserializer. You'll implement this via 
LoadAsync<T>. The key will inform how you access the data (e.g., a filepath on disk, a url to a cdn, etc.)

ISource is IDisposable so you'll have to implement a dispose method where you forcefully
release all assets loaded in memory. You may want to implement an exception if there
are any assets that were not released at disposal time to inform the application that
those assets will no longer be accessible.

TryRelease is the way in which the callers of LoadAsync can manually release assets.
Callers should use this when they no longer need the assets and before the source
goes out of scope to call Dispose. Your implementation will likely need to store a 
map of assets to keys to handle the release. The bool is so you can inform callers
if the release succeeded or not (failing in the case of releasing an already released
resource for example.)

## IDecoder<T>
These allow you to convert from a generic byte array to a type. This is convenient
if you have a source you want to use multiple times (like loading json files) but
you want to support multiple types to deserialize those source objects into. Use
a decoder to share that code.