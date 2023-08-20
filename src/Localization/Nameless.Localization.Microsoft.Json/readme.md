# Nameless Localization Microsoft Extensions Json

JSON based file localization library for Microsoft Extensions Localization.

## Content

### How To Use It

Simply add all support services and replace the current _IStringLocalizerFactory_
with this library implementation.

e.g.:

```
// On Configure(IServiceCollection services) method
// Do not forget to add IFileProvider (Physical) extensions

// call to AddLocalization
services.AddLocalization();

// Add support services and replace localizer factory
services.AddSingleton<ICultureContext, CultureContext>();
services.AddSingleton<ITranslationManager, FileTranslationManager>();
services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
```