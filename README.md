# Poppo Library

## Install

- Add to manifest.json

```
    "jp.megamin.poppokoubou.library": "https://github.com/medamap/PoppoLibrary.git?path=Assets/PoppoKoubou",
```

## Require Library

- Add to manifest.json

```
    "com.cysharp.messagepipe": "1.8.1",
    "com.cysharp.messagepipe.interprocess": "1.8.1",
    "com.cysharp.messagepipe.vcontainer": "1.8.1",
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
    "com.cysharp.zstring": "2.6.0",
    "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.16.8",
    "org.nuget.messagepack": "3.1.3",
    "org.nuget.microsoft.net.stringtools": "1.0.0",
    "org.nuget.observablecollections.r3": "3.3.3",
    "org.nuget.r3": "1.2.9",
```

## scopedRegistries

- Merge to manifest.json

```yaml
  "scopedRegistries": [
    {
      "name": "Unity NuGet",
      "url": "https://unitynuget-registry.azurewebsites.net",
      "scopes": [
        "org.nuget"
      ],
      "overrideBuiltIns": false
    },
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.cysharp"
      ],
      "overrideBuiltIns": false
    }
  ]
```
