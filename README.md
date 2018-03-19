# PowerQueryNet
Run Power Query M formula language from .NET (C#)

## About
PowerQueryNet allows you to run M formulas commonly used in Power BI and Excel (aka Get & Transform) from any .NET application.

## Download

Installer: [PowerQueryNet.msi](../../releases/download/v1.0.3/PowerQueryNet.msi)

Dependency: [PowerQuerySdk.vsix 1.0.0.16](http://dakahn.gallery.vsassets.io/_apis/public/gallery/publisher/dakahn/extension/powerquerysdk/1.0.0.16/assetbyname/PowerQuerySdk.vsix) 

(Upon installation `PowerQuerySdk.vsix` must be in the same folder as `PowerQueryNet.msi`)

Samples: [PowerQueryNet.Samples.zip](../../releases/download/v1.0.3/PowerQueryNet.Samples.zip)

## Hello, World!

1. Install `PowerQueryNet.msi`
2. From you .NET project, add a reference to `PowerQueryNet.Client`
3. Run the following:
```txt
var q = new Query { Formula = "let hw = \"Hello World\" in hw" };
var pq = new PowerQueryCommand();
var result = pq.Execute(q);
DataTable dt = result.DataTable;
```
## Power Query App
Run queries in a standalone application
![PowerQueryApp](Samples/PowerQueryApp/PowerQueryApp.png "Power Query App")

## Build requirements

* Visual Studio 2015+

To build the Setup project, [WiX Toolset](http://wixtoolset.org/releases/) must be installed.

## Copyright

Copyright 2018

Licensed under the [MIT License](LICENSE)
