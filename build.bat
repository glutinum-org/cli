@echo off

set PYTHONIOENCODING=utf-8
dotnet tool restore
dotnet run --project Glutinum.Build/Glutinum.Build.fsproj -- %*
