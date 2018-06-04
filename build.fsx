#r "paket: groupref CI//"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.DotNet
open Fake.DotNet.DotNet.Options

Target.create "Help" (fun _ -> 
  Trace.trace "Hello from Fake"
) 

// Lib

Target.create "Clean" (fun _ -> 
  Shell.deleteDir "build"
) 

Target.create "Build" (fun _ -> 
  DotNet.exec (withWorkingDirectory "./src") "build" "--output ./../build/bin" |> ignore
) 

// Tests

Target.create "CleanTests" (fun _ -> 
  Shell.deleteDir "./tests/bin"
  Shell.deleteDir "./tests/obj"
) 

Target.create "BuildTests" (fun _ -> 
  DotNet.exec (withWorkingDirectory "./tests") "build" "" |> ignore
) 

Target.create "BuildAll" ignore

// Dependencies
open Fake.Core.TargetOperators

"Clean"
  ==> "Build"

"CleanTests"
  ==> "BuildTests"

"BuildAll" <== ["Build"; "BuildTests"]


Target.runOrDefault "Help"