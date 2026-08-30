
# detects new public members not yet tracked and writes them into PublicAPI.Unshipped.txt
dotnet format analyzers --diagnostics RS0016 --severity info