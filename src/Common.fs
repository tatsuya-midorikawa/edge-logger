[<AutoOpen>]
module Common
let inline msgbox'show (msg: string) = System.Windows.Forms.MessageBox.Show (msg, "edge-logger.exe") |> ignore