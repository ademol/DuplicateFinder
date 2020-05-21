# DuplicateFinder

Find duplicate files on a system. Works on Linux & Windows. Probably will work on Mac. 

build Self-contained deployment with:
dotnet publish -c Release -r win10-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true 
dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true

At the moment dont know why it gives: NETSDK1098: errors. The generated executable works. 




