# edge-logger

This tool is designed to retrieve information related to Microsoft Edge.

## Getting Started

Specify command options for log information to be output.

| option | description | require admin permission |
| :-- | :-- | :--: |
| -wsrv, --winsrv | Output Windows services logs. | - |
| -e, --edge | Output all Microsoft Edge's logs. | ✅ |
| &nbsp;&nbsp;├&nbsp;&nbsp;&nbsp; -egp, --edgepolicy | Output Microsoft Edge policy logs. | - |
| &nbsp;&nbsp;├&nbsp;&nbsp;&nbsp; -egc, --edgecrash | Output Microsoft Edge crash reports. | - |
| &nbsp;&nbsp;├&nbsp;&nbsp;&nbsp; -egi, --edgeinst | Output Microsoft Edge installation logs. | ✅ |
| &nbsp;&nbsp;└&nbsp;&nbsp;&nbsp; -egu, --edgeupd | Output Microsoft Edge update logs. | - |
| -inet, --inetopt | Output internet option logs. | - |
| -env, --env | Output Logon user info and enviroment logs. | - |
| -nw, --network | Collecting netsh, net-export and psr logs. | ✅ |
| &nbsp;&nbsp;├&nbsp;&nbsp;&nbsp; -nx, --netexport | Collecting net-export and psr logs. | - |
| &nbsp;&nbsp;└&nbsp;&nbsp;&nbsp; -nsh, --netsh | Collecting netsh and psr logs. | ✅ |
| -psr, --psr | Collecting psr logs. | - |
| -f, --full | Output all logs. | ✅ |

If you want to output all logs, do the following:

```cmd
edge-logger.exe -f
```

Also, if you want only Windows services and Edge logs to be output, specify as follows:
```cmd
edge-logger.exe -wsrv -egp
```

The output directory is C:\logs by default.
To change it, specify the following options:

| option | description | default |
| :-- | :-- | :-- |
| -o &lt;path&gt;, --dir &lt;path&gt; | Specify the output directory for log. | C:\logs |

For example:

```cmd
edge-logger.exe -o "C:\dist\output"
```

## Other Commands

### stop
The stop command forcibly stops running commands.
Mainly used when an edge-logger is accidentally terminated.

| option | specifiable parameters |
| :-- | :-- |
| -p, --parameters | `psr`, `netsh` |

| parameters | require admin permission |
| :-- | :--: |
| `psr` | - |
| `netsh` | ✅ |

example 1:
```cmd
edge-logger.exe stop psr
```

example 2:
> **🚩Important**<br>
> Parameters must not contain spaces.
```cmd
edge-logger.exe stop psr,netsh
```