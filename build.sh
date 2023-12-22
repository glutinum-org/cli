#!/bin/sh -x

dotnet tool restore
dotnet run --project src/Glutinum.Build/Glutinum.Build.fsproj -- $@
