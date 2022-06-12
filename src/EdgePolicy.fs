module EdgePolicy

  open System.Text.Json.Serialization
  open Microsoft.Win32
  open FSharp.Core.CompilerServices


  [<Literal>]
  let current'version = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion"
  [<Literal>]
  let edition'version = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\EditionVersion"

  // OS のバージョン情報を取得する.
  let inline fetchWindowsVersion () =
    use hklm = Registry.LocalMachine.OpenSubKey(current'version, false)
    let name = hklm.GetValue("ProductName") |> string
    let disp = hklm.GetValue("DisplayVersion") |> string
    use hklm = Registry.LocalMachine.OpenSubKey(edition'version, false)
    let build = hklm.GetValue("EditionBuildNumber") |> string
    let qfe = hklm.GetValue("EditionBuildQfe") |> string
    $"{name} {disp} (build {build}.{qfe})"

  // Edge のバージョン情報を取得する.
  let inline fetchEdgeVersion () = 
    let createVersionInfo = System.IO.Path.Combine >> System.Diagnostics.FileVersionInfo.GetVersionInfo
    try
       [| 
         System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
         @"Microsoft\Edge\Application\msedge.exe" 
       |]
       |> createVersionInfo
       |> (fun vi -> vi.ProductVersion)
     with _ -> "not found"

  [<Literal>]
  let edge = @"SOFTWARE\Policies\Microsoft\Edge"
  [<Literal>]
  let edge'update = @"SOFTWARE\Policies\Microsoft\EdgeUpdate"
  [<Literal>]
  let edge'webview2 = @"SOFTWARE\Policies\Microsoft\Edge\WebView2"
  
  type Policy = {
    [<JsonPropertyOrder(1)>] name: string
    [<JsonPropertyOrder(2)>] level: string;
    [<JsonPropertyOrder(3)>] scope: string;
    [<JsonPropertyOrder(4)>] value: obj;
  }
  type Policies = {
    [<JsonPropertyOrder(1)>] Metadata : {| os: string; version: string |}
    [<JsonPropertyOrder(2)>] EdgePolicies : Policy[]
    [<JsonPropertyOrder(3)>] EdgeUpdate : Policy[]
    [<JsonPropertyOrder(4)>] WebView2 : Policy[]
  }

  // ポリシーを再帰的に読み取る.
  let rec dig(key: RegistryKey) =
    if key = null
    then 
      [||]
    else
      let values =
        key.GetValueNames()
        |> Array.map (fun name -> { 
          name = name
          level = if key.Name.EndsWith("Recommended") then "recommended" else "mandatory"
          scope = if key.Name.StartsWith("HKEY_LOCAL_MACHINE") then "machine" else "user"
          value = Registry.GetValue(key.Name, name, "") } )

      let children = 
        key.GetSubKeyNames()
        |> Array.map (fun name -> 
          use key = key.OpenSubKey(name)
          let xs = dig (key)
          { name = name
            level = if key.Name.EndsWith("Recommended") then "recommended" else "mandatory"
            scope = if key.Name.StartsWith("HKEY_LOCAL_MACHINE") then "machine" else "user"
            value = xs :> obj })

      Array.append values children

  let inline getvalues (key: string) =
    use hklm'mandatory = Registry.LocalMachine.OpenSubKey(key, false)
    use hklm'recommended = Registry.LocalMachine.OpenSubKey(System.IO.Path.Combine(key, "Recommended"), false)
    use hkcu'mandatory = Registry.CurrentUser.OpenSubKey(key, false)
    use hkcu'recommended = Registry.CurrentUser.OpenSubKey(System.IO.Path.Combine(key, "Recommended"), false)
    let acc = System.Collections.Generic.Dictionary<string, obj>()

    let f name = 
      let hklm'v = if hklm'mandatory <> null then hklm'mandatory.GetValue(name) else null
      let hklm'rv = if hklm'recommended <> null then hklm'recommended.GetValue(name) else null
      let hkcu'v = if hkcu'mandatory <> null then hkcu'mandatory.GetValue(name) else null
      let hkcu'rv = if hkcu'recommended <> null then hkcu'recommended.GetValue(name) else null
      let hklm'scope = "machine"
      let hkcu'scope = "user"

      match (hklm'v, hklm'rv, hkcu'v, hkcu'rv) with
      | (null, null, null, null) -> ()
      | (hklm'v, null, null, null) -> 
        acc.TryAdd(name, {| level = "mandatory"; scope = hklm'scope; value = hklm'v |} :> obj) |> ignore
      | (hklm'v, hklm'rv, null, null) -> 
        let superseded = {| level = "recommended"; scope = hklm'scope; value = hklm'rv |}
        acc.TryAdd(name, {| level = "mandatory"; scope = hklm'scope; superseded = superseded; value = hklm'v |} :> obj) |> ignore
      | (null, null, hkcu'v, null) -> 
        acc.TryAdd(name, {| level = "mandatory"; scope = hkcu'scope; value = hkcu'v |} :> obj) |> ignore
      | (null, null, hkcu'v, hkcu'rv) ->
        let superseded = {| level = "recommended"; scope = hkcu'scope; value = hkcu'rv |}
        acc.TryAdd(name, {| level = "mandatory"; scope = hkcu'scope; superseded = superseded; value = hkcu'v |} :> obj) |> ignore
      | (hklm'v, null, hkcu'v, null) ->
        let superseded = {| level = "recommended"; scope = hkcu'scope; value = hkcu'rv |}
        acc.TryAdd(name, {| level = "mandatory"; scope = hklm'scope; superseded = superseded; value = hklm'v |} :> obj) |> ignore
      | (hklm'v, hklm'rv, hkcu'v, null) -> 
        let superseded = [|
          {| level = "recommended"; scope = hklm'scope; value = hklm'rv |} :> obj
          {| level = "mandatory"; scope = hkcu'scope; value = hkcu'v |} |]
        acc.TryAdd(name, {| level = "mandatory"; scope = hklm'scope; superseded = superseded; value = hklm'v |} :> obj) |> ignore
      | (hklm'v, null, hkcu'v, hkcu'rv) -> 
        let superseded = [|
          {| level = "recommended"; scope = hkcu'scope; value = hkcu'rv |} :> obj
          {| level = "mandatory"; scope = hkcu'scope; value = hkcu'v |} |]
        acc.TryAdd(name, {| level = "mandatory"; scope = hklm'scope; superseded = superseded; value = hklm'v |} :> obj) |> ignore
      | (hklm'v, hklm'rv, hkcu'v, hkcu'rv) -> 
        let superseded = [|
          {| level = "recommended"; scope = hklm'scope; value = hklm'rv |} :> obj
          {| level = "mandatory"; scope = hkcu'scope; superseded = {| level = "recommended"; scope = hkcu'scope; value = hkcu'rv |}; value = hkcu'v |} |]
        acc.TryAdd(name, {| level = "mandatory"; scope = hklm'scope; superseded = superseded; value = hklm'v |} :> obj) |> ignore

    if hklm'mandatory <> null then hklm'mandatory.GetValueNames() |> Array.iter f
    if hkcu'mandatory <> null then hkcu'mandatory.GetValueNames() |> Array.iter f
    if hklm'recommended <> null then hklm'recommended.GetValueNames() |> Array.iter f
    if hkcu'recommended <> null then hkcu'recommended.GetValueNames() |> Array.iter f

    acc

  // TODO: 
  let inline getlistvalues (key: string) =
    use hklm'mandatory = Registry.LocalMachine.OpenSubKey(key, false)
    use hklm'recommended = Registry.LocalMachine.OpenSubKey(System.IO.Path.Combine(key, "Recommended"), false)
    use hkcu'mandatory = Registry.CurrentUser.OpenSubKey(key, false)
    use hkcu'recommended = Registry.CurrentUser.OpenSubKey(System.IO.Path.Combine(key, "Recommended"), false)
    let acc = System.Collections.Generic.Dictionary<string, obj>()
    
    let f (key: RegistryKey) subkey = 
      use key = key.OpenSubKey(subkey)
      let v = getvalues key.Name
      ()
    let a =
      hklm'mandatory.GetSubKeyNames()
      |> Array.filter (fun n -> not (n.EndsWith "Recommended"))


    use mandatory = Registry.LocalMachine.OpenSubKey(key, false)
    if mandatory = null 
    then null
    else
      let f name =
        let value = mandatory.GetValue(name)
        {| name = name; value = value |} :> obj
      mandatory.GetValueNames() |> Array.map f
    
  let dig'(key: string) =
    let a = getvalues key

    ()


  // Edge ポリシー情報を取得する.
  let inline fetch () =

    let load reg =
      let mutable c = ArrayCollector<Policy>()
      //let mutable c = ArrayCollector<Policy>()
      use hklm = Registry.LocalMachine.OpenSubKey(reg, false)
      let xs = dig hklm
      c.AddMany xs
      use hkcu = Registry.CurrentUser.OpenSubKey(reg, false)
      let xs = dig(hkcu)
      c.AddMany xs
      c.Close()
      
    let os = fetchWindowsVersion()
    let version = fetchEdgeVersion()
    // SOFTWARE\Policies\Microsoft\Edge
    let edge' = load edge
    // SOFTWARE\Policies\Microsoft\EdgeUpdate
    let update = load edge'update
    // SOFTWARE\Policies\Microsoft\Edge\WebView2
    let webview2 = load edge'webview2

    {
      Metadata = {| os = os; version = version |}; 
      EdgePolicies = edge'; 
      EdgeUpdate = update; 
      WebView2 = webview2; 
    }