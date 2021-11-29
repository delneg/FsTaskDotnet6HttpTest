# Enable Dynamic PGO
export DOTNET_TieredPGO=1

# AOT images aren't instrumented so we need to disable them and collect
# relevant PGO data for literally everything. It affects startup time badly, 
# but leads to higher performance after warm up.
export DOTNET_ReadyToRun=0

# For .NET 7.0 we hopefully will enable full-fledged OSR, but for now methods with loops 
# always bypass tier0, however, we do need them in tier0 to be instrumented for PGO.
export DOTNET_TC_QuickJitForLoops=1

dotnet run -c Release
