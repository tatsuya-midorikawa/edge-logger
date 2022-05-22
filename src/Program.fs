task {
  do! Logger.output Logger.winsrv'filepath "test"
}
|> System.Threading.Tasks.Task.WaitAll