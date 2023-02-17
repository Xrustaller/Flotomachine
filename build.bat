dotnet publish /t:CreateDeb /p:RuntimeIdentifier=linux-arm /p:SelfContained=true /p:TargetFramework=net7.0 /p:Configuration=Release
pause