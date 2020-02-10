mkdir tmpOut
mkdir tmpOut\Plugins
copy bin\x64\Release\MusicSpatializer.dll tmpOut\Plugins\MusicSpatializer.dll
del plugin.zip
7z a -tZip -mm=Deflate -mfb=258 -mpass=15 -r plugin.zip ./tmpOut/*
rmdir tmpOut /s /q