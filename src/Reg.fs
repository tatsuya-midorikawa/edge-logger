﻿module Reg

  open Microsoft.Win32
  open FSharp.Core.CompilerServices

  type Reg = { path: string; name: string; vtype: string; value: obj; values: Reg[]}
  
  let rec dig(key: RegistryKey) =
    if key = null
    then [||]
    else
      let values =
        key.GetValueNames()
        |> Array.map (fun name -> { path = key.Name; name = name; vtype = key.GetValueKind(name) |> string; value = Registry.GetValue(key.Name, name, ""); values = null })
      let children = 
        key.GetSubKeyNames()
        |> Array.map (fun name -> 
          use key = key.OpenSubKey(name)
          let xs = dig (key)
          { path = null; name = name; vtype = null; value = null; values = xs} )
      Array.append values children

  module IEReg =
    let regs = [|
      @"Software\Microsoft\Active Setup\Installed Components"
      @"SOFTWARE\Microsoft\Internet Explorer"
      @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer"
      @"Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders"
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\Ext"
      @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Ext"
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"
      @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Internet Settings"
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Ext"
      @"SOFTWARE\Policies\Microsoft\Internet Explorer"
      @"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings"
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing"
    |]

    let inline getIeRegistries () =
      let mutable c = ArrayCollector<Reg>()

      for reg in regs do
        use hklm = Registry.LocalMachine.OpenSubKey(reg, false)
        let xs = dig hklm
        c.AddMany xs
        use hkcu = Registry.CurrentUser.OpenSubKey(reg, false)
        let xs = dig hkcu
        c.AddMany xs

      c.Close()

  
  