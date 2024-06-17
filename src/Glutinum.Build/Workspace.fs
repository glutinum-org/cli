module Build.Workspace

open EasyBuild.FileSystemProvider

type Workspace = RelativeFileSystem<".">

type VirtualWorkspace =
    VirtualFileSystem<
        ".",
        """
dist
    fable_modules
        .gitignore
"""
     >
