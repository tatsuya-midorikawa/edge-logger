namespace jp.dsi.logger.misc

open System.IO.Pipes
open System.Text
open System.Buffers
open System.Security.Principal
open System.IO

module Pipes =
  [<Literal>]
  let pwsh'name = $"jp.dsi.logger.pwsh.15632680-eedf-418a-aa2a-334dbb121d38"
  let enc = UnicodeEncoding()

  let inline create'pwsh'server () = new NamedPipeServerStream(pwsh'name, PipeDirection.InOut)
  let inline create'pwsh'client () = new NamedPipeClientStream(".", pwsh'name, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation)

  let inline read (stream: Stream) =
    let size = 256
    let mutable str = StringBuilder(size)
    let append (buf: array<byte>) = enc.GetString(buf) |> str.Append |> ignore
    let rec read pos =
      let buf = ArrayPool.Shared.Rent(size)
      let len = stream.Read(buf, pos, size)
      if size <> len then
        let buf' = ArrayPool.Shared.Rent(len)
        System.Array.Copy(buf, buf', len)
        append(buf')
        ArrayPool.Shared.Return(buf)
        ArrayPool.Shared.Return(buf')
      else
        append(buf)
        ArrayPool.Shared.Return(buf)
        read (pos + len)
    read 0
    str.ToString()

  let write (msg: string) (stream: Stream)  =
  //let write (msg: string) (stream: NamedPipeClientStream)  =
    let buf = enc.GetBytes msg
    let len = buf.Length
    stream.Write(buf, 0, len)
    stream.Flush()