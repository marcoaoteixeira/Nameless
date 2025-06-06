# Nameless Localization Json

JSON based file localization library for Microsoft Extensions Localization.

## How To Use It

```CSHARP
// Using IServiceCollection

services.AddJsonLocalization(new LocalizationOptions {
    // The folder where the translation files should be looked for.
    // Will be combined with the current Microsoft.Extensions.FileProviders.IFileProvider
    // instance root path. So, make sure to add it to your services.
    TranslationFolderName = "Localization",

    // Whether it will watch the translation files for changes
    // and reload if necessary.
    WatchFileForChanges = true
});

```

## How It Works Internally

It will look for any JSON file, inside the specified folder, that's named with the culture (ISO) you'd like to use.

E.g.

```
    en-US.json
    pt-BR.json
    en-EN.json
```

If `WatchFileForChanges` is `true` then any change to the translation files will cause a reload in the translation system, but only for the specific translation.

## How Translation File Should Looks Like

```JSON
    {
        "Culture": "pt-BR",
        "Regions": [
            {
                "Name": "[ASSEMBLY_NAME] TYPE_FULLNAME",
                "Messages": [
                    {
                        "ID": "Original Text",
                        "Text": "Translated Text"
                    },
                ]
            },
        ],
    }
```