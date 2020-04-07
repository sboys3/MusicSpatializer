# Music Spatializer
This plugin for Beat Saber uses the included spatializer in beat saber for the music. Headphones are required to hear the effects of this. The audio responds to head movements and sounds more real. The saber noises already use it but the music is piped directly into your headphones by default. This captures the audio from the original audio source and sends it to separate left and right spatialized audio sources. There is also a third audio source that uses Unity's built in resonance zones to provide a bit of echo. In 360 maps the audio sources follow the chevron (I suggest you check it out, its kind of cool). There is also an option to enable a slight bass boost by adding a virtual subwoofer.

Dependencies: BSIPA and BSML

## Building

1. Download or clone to get a local copy
2. Edit `bslink.bat` to point to a BeatSaber installation
3. Run `bslink.bat` as administrator to create a folder junction(symlink) to your BeatSaber installation
4. Open `spatializer.sln` in in visual studio 2019
5. Change the configuration from `Debug` to `Release` and Change the target from `Any Cpu` to `x64`
6. Build solution(ctrl+shift+b)