[<AutoOpen>]
module Common

let defaultof<'T> = Unchecked.defaultof<'T>
type root = HKLM = 0 | HKCU = 1
type dict<'T, 'U> = System.Collections.Generic.Dictionary<'T, 'U>
type pair = System.Collections.Generic.KeyValuePair<string, obj> 
type regs = seq<root * string>