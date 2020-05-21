# WinRT + F#
[C#/WinRT]: https://github.com/microsoft/CsWinRT
[SDK Contracts]: https://www.nuget.org/packages/Microsoft.Windows.SDK.Contracts
[Rust]: https://github.com/microsoft/winrt-rs
[Python]: https://github.com/microsoft/xlang/tree/master/src/package/pywinrt/projection
[C++]: https://github.com/microsoft/cppwinrt
[Avalonia.FuncUI]: https://github.com/AvaloniaCommunity/Avalonia.FuncUI

In the last build event (May 2020) one project was shown [C#/WinRT] which is a projection of the WinRT API over C#, this projection is compatible with .netstandard2.0 and .net5 (once it's released). This is not the first time an attempt to expose the WinRT API to Win32 apps has been made, the last one was [SDK Contracts] and while you could use most of the WinRT APIs it had some limitations around certain APIs like Bluetooth and if you were an F#'er like me, you were in bad luck because the SDK Contracts didn't even allow your project to compile that stuff is now past and the next iteration (which I believe is a better take) is here.

The projection is also available for [C++], [Rust] and [Python].


# Samples
Check the [Core](./Core) project where I tried to put most of the WinRT API code

- Media
    >Includes some of the `Windows.Media` APIs
- Network
    >Includes some of the `Windows.Networking` APIs
- Power
    >Includes some of the `Windows.System.Power` APIs


Inside the [Demo](./Demo) project you will be able to find a simple [Avalonia.FuncUI] application (win32 app) that takes advantage of these APIs



> You might wonder why Avalonia in the first place, isn't Avalonia cross-platform?
>
> Doesn't this would make the app windows only?

Yes and no.

The main reason is that I prefer the MVU style that Avalonia.FuncUI provides to create and prototype desktop applications, the second one there are always ways to enhance your application for the platform you are running in, you could use code directives, runtime checks and other kinds of stuff to make your users have better experiences and integrations for the platform



If you find any bugs/suggestions please open a new issue