# EasyAR Xamarin Samples

----
## What is it?
Provides examples of using the EasyAR - Xamarin Binding Library available [here](https://github.com/joejensen/EasyAR-Xamarin), mostly with the Urho3D game engine.

----
## Usage
These samples were developed and tested using Visual Studio 2019 Pro on Windows with a remote Mac-Mini build server, I can't guarentee they'll work elsewhere.  

1. Update the license key in EasyARX-Samples\EasyARX-Samples\EasyARUtil.cs by following the [instructions from EasyAR](https://help.easyar.com/EasyAR%20Sense/v3/Getting%20Started/Getting-Started-with-EasyAR.html).
2. Run the project for the appropriate device.

----
## Samples
1.  Urho-Boxes: Renders a simple blue box over the top of detected image targets.
![Urho-Boxes Example](docs/urho-boxes.gif)

----
## TODO:
1.  At the moment the samples only work in the devices natural / portrait orientation.  The background(orthogonal) projection matrices need to be modified to account for other orientations.
2.  Provide documentation on how all the pieces fit together.
3.  *Update Xamarin-Essential to 1.4.0 when released*.  Currently Xamarin-Essential for Android has a [critical bug causing a memory leak](https://github.com/xamarin/Essentials/issues/917) that will crash the examples after a few minutes.  This has already been fixed for Xamarin-Essentials 1.4.0 but not yet released to nuget.

----
## License

While this project is [unlicensed](http://unlicense.org/), it wraps and depends on the SDK distributed by [EasyAR](https://www.easyar.com/) and licensed under [EasyAR's licensing terms](https://easyar.com/view/protocol.html).

This projects goal is solely to make it simpler to use EasyAR in a Xamarin project, not to replace, fix, test, or add any functionality to EasyAR itself.