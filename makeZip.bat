mkdir tmpOut
mkdir tmpOut\Plugins
copy bin\x64\Release\MusicSpatializer.dll tmpOut\Plugins\MusicSpatializer.dll
del MusicSpatializerPlugin.zip
7z a -tZip -mm=Deflate -mfb=258 -mpass=15 -r MusicSpatializerPlugin.zip ./tmpOut/*
rmdir tmpOut /s /q