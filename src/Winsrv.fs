module Winsrv

let getServices () = System.ServiceProcess.ServiceController.GetServices()
let collect (services: System.ServiceProcess.ServiceController[]) =
  services
  |> Array.map (fun s -> printfn $"    {s.ServiceName} : {s.Status}")