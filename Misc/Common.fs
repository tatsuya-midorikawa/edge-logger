namespace jp.dsi.logger.misc

open System
open System.Threading
open System.Threading.Tasks
open System.IO.Pipes
open System.Text
open System.Buffers
open System.Security.Principal
open System.IO
open System.Diagnostics

// Asynchronous processing
module Task =
  let cts = new CancellationTokenSource()
  let inline run<'T> (task: unit -> 'T) = Task.Run(task, cts.Token)