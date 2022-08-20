module IO

open System.IO
open System.Text
open System.Buffers

let private enc = UnicodeEncoding()

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

let inline write (stream: Stream) (msg: string) =
  let buf = enc.GetBytes msg
  let len = buf.Length
  stream.Write(buf, 0, len)
  stream.Flush()