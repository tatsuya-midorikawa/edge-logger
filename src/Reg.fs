module Reg

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

  module EdgeReg =
    type EdgeReg = { Edge: Reg[]; EdgeUpdate: Reg[]; WebView2: Reg[] }

    [<Literal>]
    let edge = @"SOFTWARE\Policies\Microsoft\Edge"
    [<Literal>]
    let edge'update = @"SOFTWARE\Policies\Microsoft\EdgeUpdate"
    [<Literal>]
    let edge'webview2 = @"SOFTWARE\Policies\Microsoft\Edge\WebView2"

    let inline getEdgeRegistries () =

      // SOFTWARE\Policies\Microsoft\Edge
      let mutable c = ArrayCollector<Reg>()
      use hklm'edge = Registry.LocalMachine.OpenSubKey(edge, false)
      let xs = dig(hklm'edge)
      c.AddMany xs
      use hkcu'edge = Registry.CurrentUser.OpenSubKey(edge, false)
      let xs = dig(hkcu'edge)
      c.AddMany xs
      let edge = c.Close()
      
      // SOFTWARE\Policies\Microsoft\EdgeUpdate
      c <- ArrayCollector<Reg>()
      use hklm'edge'update = Registry.LocalMachine.OpenSubKey(edge'update, false)
      let xs = dig(hklm'edge'update)
      c.AddMany xs
      use hkcu'edge'update = Registry.CurrentUser.OpenSubKey(edge'update, false)
      let xs = dig(hkcu'edge'update)
      c.AddMany xs
      let update = c.Close()

      // SOFTWARE\Policies\Microsoft\Edge\WebView2
      c <- ArrayCollector<Reg>()
      use hklm'edge'webview2 = Registry.LocalMachine.OpenSubKey(edge'webview2, false)
      let xs = dig(hklm'edge'webview2)
      c.AddMany xs
      use hkcu'edge'webview2 = Registry.CurrentUser.OpenSubKey(edge'webview2, false)
      let xs = dig(hkcu'edge'webview2)
      c.AddMany xs
      let webview2 = c.Close()

      { Edge = edge; EdgeUpdate = update; WebView2 = webview2; }

  module IEReg =
    
    [<Literal>]
    let act'installed_components = @"Software\Microsoft\Active Setup\Installed Components"
    [<Literal>]
    let ie = @"SOFTWARE\Microsoft\Internet Explorer"
    [<Literal>]
    let wow6432node'ie = @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer"
    [<Literal>]
    let ie'bho = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"
    [<Literal>]
    let exp'shell_folders = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
    [<Literal>]
    let exp'usr_shell_folders = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders"
    [<Literal>]
    let cv'ext = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Ext"
    [<Literal>]
    let wow6432node'cv'ext = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Ext"
    [<Literal>]
    let cv'internet_settings = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"
    [<Literal>]
    let wow6432node'cv'internet_settings = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Internet Settings"
    [<Literal>]
    let policies'ext = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Ext"
    [<Literal>]
    let policies'ie = @"SOFTWARE\Policies\Microsoft\Internet Explorer"
    [<Literal>]
    let policies'internet_settings = @"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings"    
    [<Literal>]
    let prov'software_publishing = @"SOFTWARE\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing"

    let inline getIeRegistries () =
      let mutable c = ArrayCollector<Reg>()

      // Software\Microsoft\Active Setup\Installed Components
      use hklm'act'installed_components = Registry.LocalMachine.OpenSubKey(act'installed_components, false)
      let xs = dig(hklm'act'installed_components)
      c.AddMany xs
      use hkcu'act'installed_components = Registry.CurrentUser.OpenSubKey(act'installed_components, false)
      let xs = dig(hkcu'act'installed_components)
      c.AddMany xs

      // SOFTWARE\Microsoft\Internet Explorer
      use hklm'ie = Registry.LocalMachine.OpenSubKey(ie, false)
      let xs = dig(hklm'ie)
      c.AddMany xs
      use hkcu'ie = Registry.CurrentUser.OpenSubKey(ie, false)
      let xs = dig(hkcu'ie)
      c.AddMany xs

      // SOFTWARE\Wow6432Node\Microsoft\Internet Explorer
      use hklm'wow6432node'ie = Registry.LocalMachine.OpenSubKey(wow6432node'ie, false)
      let xs = dig(hklm'wow6432node'ie)
      c.AddMany xs
      use hkcu'wow6432node'ie = Registry.CurrentUser.OpenSubKey(wow6432node'ie, false)
      let xs = dig(hkcu'wow6432node'ie)
      c.AddMany xs

      // Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects
      use hklm'ie'bho = Registry.LocalMachine.OpenSubKey(ie'bho, false)
      let xs = dig(hklm'ie'bho)
      c.AddMany xs
      use hkcu'ie'bho = Registry.CurrentUser.OpenSubKey(ie'bho, false)
      let xs = dig(hkcu'ie'bho)
      c.AddMany xs
      c.AddMany xs

      // SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders
      use hklm'exp'shell_folders = Registry.LocalMachine.OpenSubKey(exp'shell_folders, false)
      let xs = dig(hklm'exp'shell_folders)
      c.AddMany xs
      use hkcu'exp'shell_folders = Registry.CurrentUser.OpenSubKey(exp'shell_folders, false)
      let xs = dig(hkcu'exp'shell_folders)
      c.AddMany xs

      // SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders
      use hklm'exp'usr_shell_folders = Registry.LocalMachine.OpenSubKey(exp'usr_shell_folders, false)
      let xs = dig(hklm'exp'usr_shell_folders)
      c.AddMany xs
      use hkcu'exp'usr_shell_folders = Registry.CurrentUser.OpenSubKey(exp'usr_shell_folders, false)
      let xs = dig(hkcu'exp'usr_shell_folders)
      c.AddMany xs

      // SOFTWARE\Microsoft\Windows\CurrentVersion\Ext
      use hklm'cv'ext = Registry.LocalMachine.OpenSubKey(cv'ext, false)
      let xs = dig(hklm'cv'ext)
      c.AddMany xs
      use hkcu'cv'ext = Registry.CurrentUser.OpenSubKey(cv'ext, false)
      let xs = dig(hkcu'cv'ext)
      c.AddMany xs

      // SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Ext
      use hklm'wow6432node'cv'ext = Registry.LocalMachine.OpenSubKey(wow6432node'cv'ext, false)
      let xs = dig(hklm'wow6432node'cv'ext)
      c.AddMany xs
      use hkcu'wow6432node'cv'ext = Registry.CurrentUser.OpenSubKey(wow6432node'cv'ext, false)
      let xs = dig(hkcu'wow6432node'cv'ext)
      c.AddMany xs

      // SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings
      use hklm'cv'internet_settings = Registry.LocalMachine.OpenSubKey(cv'internet_settings, false)
      let xs = dig(hklm'cv'internet_settings)
      c.AddMany xs
      use hkcu'cv'internet_settings = Registry.CurrentUser.OpenSubKey(cv'internet_settings, false)
      let xs = dig(hkcu'cv'internet_settings)
      c.AddMany xs

      // SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Internet Settings
      use hklm'wow6432node'cv'internet_settings = Registry.LocalMachine.OpenSubKey(wow6432node'cv'internet_settings, false)
      let xs = dig(hklm'wow6432node'cv'internet_settings)
      c.AddMany xs
      use hkcu'wow6432node'cv'internet_settings = Registry.CurrentUser.OpenSubKey(wow6432node'cv'internet_settings, false)
      let xs = dig(hkcu'wow6432node'cv'internet_settings)
      c.AddMany xs

      // SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Ext
      use hklm'policies'ext = Registry.LocalMachine.OpenSubKey(policies'ext, false)
      let xs = dig(hklm'policies'ext)
      c.AddMany xs
      use hkcu'policies'ext = Registry.CurrentUser.OpenSubKey(policies'ext, false)
      let xs = dig(hkcu'policies'ext)
      c.AddMany xs

      // SOFTWARE\Policies\Microsoft\Internet Explorer
      use hklm'policies'ie = Registry.LocalMachine.OpenSubKey(policies'ie, false)
      let xs = dig(hklm'policies'ie)
      c.AddMany xs
      use hkcu'policies'ie = Registry.CurrentUser.OpenSubKey(policies'ie, false)
      let xs = dig(hkcu'policies'ie)
      c.AddMany xs

      // SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings
      use hklm'policies'internet_settings = Registry.LocalMachine.OpenSubKey(policies'internet_settings, false)
      let xs = dig(hklm'policies'internet_settings)
      c.AddMany xs
      use hkcu'policies'internet_settings = Registry.CurrentUser.OpenSubKey(policies'internet_settings, false)
      let xs = dig(hkcu'policies'internet_settings)
      c.AddMany xs

      // SOFTWARE\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing
      use hklm'prov'software_publishing = Registry.LocalMachine.OpenSubKey(prov'software_publishing, false)
      let xs = dig(hklm'prov'software_publishing)
      c.AddMany xs
      use hkcu'prov'software_publishing = Registry.CurrentUser.OpenSubKey(prov'software_publishing, false)
      let xs = dig(hkcu'prov'software_publishing)
      c.AddMany xs

      c.Close()

  
  