module Winsrv

type Service = { name: string; status: string }
let inline getServices () =
  System.ServiceProcess.ServiceController.GetServices()
let inline collect (services: System.ServiceProcess.ServiceController[]) =
  services |> Array.map (fun s -> { name = s.ServiceName; status = $"{s.Status}" })