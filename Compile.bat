cd C:\src\comet
git pull
dotnet build src/Comet.Game --configuration Release
xcopy /c /f /s /e /y "src\Release\net5.0" "C:\zfserver" /exclude:exclude.txt
pause