interface OptionsInline {
    users: { [key: string]: string }
}

// 1. If only index signature generate an EmitIndexer
// 2. If no index signature generate a ParamObject
// 3. If both, generate a normal Interface with EmitIndexer and standard properties for usage with jsOptions, etc.
