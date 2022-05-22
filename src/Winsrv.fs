module Winsrv

let getServices() = System.ServiceProcess.ServiceController.GetServices()

