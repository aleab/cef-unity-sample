# CefUnitySample
This project, based on [SethGibson's CEF_01](https://github.com/SethGibson/CEF_01), is an example of how to use the [*Chromium Embedded Framework*](https://bitbucket.org/chromiumembedded/cef) and [*CefGlue*](https://github.com/OutSystems/CefGlue) in Unity to display a web browser in a scene.

## Dependencies
* **`CefGlue`**, the .NET/Mono bindings for CEF3.
* **`libcef`** (download binaries [here](https://cef-builds.spotifycdn.com/index.html)).

#### Notes
- *libcef*'s version must match the one used by *CefGlue*, declared in *`CefGlue/Interop/version.g.cs`* ([link](https://github.com/OutSystems/CefGlue/blob/4472/CefGlue/Interop/version.g.cs)).
- When building CefGlue, be sure to target a .NET Framework supported by Unity (3.5 is fine) and the correct platform.

This project has only been tested on Windows 10 with the 64-bit version of Unity 5.6.0p4; the version of *libcef* used is `3.2987.1601.gf035232`.
Currently, because of an [issue with CEF](https://github.com/SethGibson/CEF_01/issues/1), the Unity Editor crashes if the scene (any scene) is played more than once.
