module MDX

open Node.Api
open System.Text.RegularExpressions
open Fable.Core.JsInterop
open Fable.Core

[<AutoOpen>]
module Files =

  // let private pathJoinPrefix (prefix) (extraPath) = path.join(prefix, extraPath)

  let rec getAllFilesRecursively fullPath =
    if fs.statSync(!^fullPath).isFile() then
      Seq.singleton fullPath
    else
      !^fullPath
      |> fs.readdirSync
      |> Seq.collect getAllFilesRecursively

let root = process.cwd()

let getFiles (contentClass) =
  path.join(root, "data", contentClass)
  |> getAllFilesRecursively
  |> Seq.map (fun file -> file.Replace('\\', '/'))

let formatSlug =
  let reg = Regex(@"/\.(mdx|md)/")
  fun slug -> reg.Replace(slug, "")

let (|FileExists|_|) (ext: string) (fileName: string) =
  let fullName = $"{fileName}{ext}"
  if fs.existsSync !^fullName then
    fs.readFileSync (fullName,"utf8") |> Some
  else
    None

let setESBuildPath () =
  process.env?ESBUILD_BINARY_PATH <-
    if process.platform = Node.Base.Platform.Win32 then
      path.join(root, "node_modules", "esbuild", "esbuild.exe")
    else
      path.join(root, "node_modules", "esbuild", "bin", "esbuild")
  
[<AutoOpen>]
module Markdown =
    //note: probably need to use ts2fable to bring in some of these packages.
    type CoreProcessorOptions = obj
    type FrontMatter = obj

    [<ImportMember("mdx-bundler")>]
    let bundleMDX(file: string, source: string, files: obj, xdmOptions: CoreProcessorOptions * FrontMatter -> CoreProcessorOptions): int = jsNative

let getFilesBySlug (contentClass: string, slug: string) = promise {
  let source =
    match path.join(root, "data", contentClass, $"{slug}") with
    | FileExists ".mdx" content
    | FileExists ".md" content -> Some content
    | _ -> None

  setESBuildPath ()
    
  let toc = Array.empty

  let! (code, frontmatter) = 

}