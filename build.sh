#!/bin/sh -x

dotnet tool restore
dotnet run --project Glutinum.Build/Glutinum.Build.fsproj -- $@
