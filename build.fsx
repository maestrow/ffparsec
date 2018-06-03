#r "paket: groupref CI//"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.DotNet.DotNet.Options

Target.create "Default" (fun _ -> 
  Trace.trace "Hello from Fake"
) 

Target.create "Build" (fun _ -> 
  DotNet.exec (withWorkingDirectory "./src") "build" "--output ./../build/bin" |> ignore
) 


Target.runOrDefault "Default"