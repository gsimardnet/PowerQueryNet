# PowerQueryNet
Run Power Query M formula language from .NET (C#)

## About
PowerQueryNet allows you to run M formulas commonly used in Power BI and Excel (aka Get & Transform) from any .NET application.

## Download

Installer : [PowerQueryNet.msi](https://github.com/gsimardnet/PowerQueryNet/releases/download/v1.0.0/PowerQueryNet.msi)

Samples : [PowerQueryNet.Samples.zip](Download/PowerQueryNet.Samples.zip)

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
## Build requirements

* Visual Studio 2015+

To build the Setup project, [WiX Toolset](http://wixtoolset.org/releases/) must be installed.

## Copyright

Copyright 2018

Licensed under the [MIT License](LICENSE)
