module Winsrv

type Service = { Name: string; Status: string }
let inline getServices () =
  System.ServiceProcess.ServiceController.GetServices()
let inline collect (services: System.ServiceProcess.ServiceController[]) =
  services |> Array.map (fun s -> { Name = s.ServiceName; Status = $"{s.Status}" })