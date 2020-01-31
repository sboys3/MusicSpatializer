mkdir tmpOut
mkdir tmpOut\Plugins
copy bin\x64\Release\MusicSpatializer.dll tmpOut\Plugins\MusicSpatializer.dll
del plugin.zip
cd tmpOut
7z a -tZip -mm=Deflate -mfb=258 -mpass=15 -r ../plugin.zip *
cd ../
rmdir tmpOut /s /q