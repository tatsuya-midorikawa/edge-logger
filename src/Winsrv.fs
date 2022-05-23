module Winsrv


type Service = { Name: string; Status: string }

let getServices () = System.ServiceProcess.ServiceController.GetServices()

let collect (services: System.ServiceProcess.ServiceController[]) =
  services
  |> Array.map (fun s -> { Name = s.ServiceName; Status = $"{s.Status}" })