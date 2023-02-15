dotnet publish .\Flotomachine\Flotomachine.csproj /t:CreateDeb /p:RuntimeIdentifier=linux-arm /p:SelfContained=true /p:TargetFramework=net7.0 /p:Configuration=Release /p:PublishProfile=Properties\PublishProfiles\RaspberryNet.pubxml

pause