module Winsrv

type Service = { name: string; start_type: string; status: string }
let inline getServices () =
  System.ServiceProcess.ServiceController.GetServices()
let inline collect (services: System.ServiceProcess.ServiceController[]) =
  services |> Array.map (fun s -> { name = s.DisplayName; start_type = $"{s.StartType}"; status = $"{s.Status}" })