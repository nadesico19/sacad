# Building Instructions

To make Sacad work properly with the desired version of AutoCAD, we need to
build a .NET Framework library named `SacadMgd.dll` with ObjectARX SDK.

This document assumes that the reader is familiar with .NET Framework, which
means the ability to use Visual Studio to build a library independently. In
addition, the instructions written here are all recommendations, not
requirements. To reference the standardized process of building ObjectARX
libraries, you should read the official guide released by Autodesk.

## Environment

I will keep the C# language features to version 6.0, so Visual Studio 2015
Community Edition will be enough to build this library.

First of all, create a Class Library project named `SacadMgd` in the Visual
studio. In my workspace, the target version of .NET Framework is set to v4.6.1.
Then, add the existing .cs source file under `/SacadMgd/` into the project by
link or copy.

Next, add the dependent libraries. There are two parts of libraries in the
dependency.

### Newtonsoft.Json

Just install this package from NuGet, the latest version 13.0.1 will be ok.

### ObjectARX

If you have the SDK from version 2010 to 2012, add `AcMgd.dll` and `AcDbMgd.dll`
to the dependencies of the project.

If you have the SDK version 2013 or later, another `AcCoreMgd.dll` should also
be added besides the two files listed above.

Do not forget unchecking `Copy Local` flag, in the property panel of these DLL
from ObjectARX SDK.

### Output Files

When built successfully, there will be only `SacadMgd.dll`
and `Newtonsoft.Json.dll` in the output folder.

## Deployment

The output files should be placed under `/sacad/dll/{version}` folder.
The `version` placeholder should be replaced with number from 2010 to the latest
version of AutoCAD.

A typical structure of directory after deployment will be:

```
sacad
└──dll
   ├─2010
   │ ├─Newtonsoft.Json.dll
   │ └─SacadMgd.dll
   │
   └─2013
     ├─Newtonsoft.Json.dll
     └─SacadMgd.dll
```

When Sacad search for the DLL, it walks backward from the version your code have
requested. So if you create a client instance for AutoCAD 2020, the search will
hit the DLL under `/sacad/dll/2013`.

Typically, due to the difference of dependencies between ObjectARX 2012 and
2013, you should at least build two version of this library, to cover all
AutoCAD versions from 2010 to the latest.