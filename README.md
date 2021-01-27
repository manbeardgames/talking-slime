# Talking Slime
This is a small demo project that I use on my [Twitch stream]() to animate the talking of a 2D slime sprite for my on stream avatar.

## Usage
The code for the application resides in the **TalkingSlime.cs** file.  At the top of the class are serval fields that have been marked as the ones that can be adjusted to suite your needs.

* `_microphoneName`:  This is the name of your microphone listening device to use for detecting audio input.  It can be a partial word.  
* `_micDeviceID`:  This is the device ID of your microphone.  You can find this value by running this game once within visual studio then checking the Output panel window which will show all detected input devices and their ids.  
* `_closeThreshold`: This is a float value between 0.0f and 100.0f.  This value determines at which point in the microphone levels it is determined that the microphone is closed.  To find your appropriate level, run the game and look at the window title bar to see the current levels and choose based on that information.  
* `_openThreshold`:  This is a float value between 0.0f and 100.0f.  This value determines at which point in the microphone's levels it is determined that the microphone is open.  To find your appropriate level, run the game and look at the window title bar to see the current levels and choose based on that information.  
* `_defaultScale`:  This is the default x and y axis scale value to use when rendering the mouth. This value is used when the microphone is in a "closed" state.  
* `_scaleMin`: This is the minimum x and y axis scale value to use when rendering the mouth only during the microphone "open" state.  
* `_scaleMax`:  This is the maximum x and y axis scale value to use when rendering the mouth only during the microphone "open" state.

## Sponsor On GitHub
[![](https://raw.githubusercontent.com/manbeardgames/monogame-aseprite/gh-pages-develop/static/img/github_sponsor.png)](https://github.com/sponsors/manbeardgames)  

Hi, my name is Christopher Whitley. I am an indie game developer and game development tool developer. I create tools primary for the MonoGame framework. All of the tools I develop are released as free and open-sourced software (FOSS), just like this one.

If you'd like to buy me a cup of coffee or just sponsor me and my projects in general, you can do so on [GitHub Sponsors](https://github.com/sponsors/manbeardgames). 

## License
```
MIT License

Copyright (c) 2020 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```