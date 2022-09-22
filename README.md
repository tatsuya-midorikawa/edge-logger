# edge-logger

This tool is designed to retrieve information related to Microsoft Edge.

## Getting Started

Specify command options for log information to be output.

| option | description | requires an admin |
| :-- | :-- | :--: |
| -wsrv | Output Windows services logs. | - |
| -egp | Output Microsoft Edge policy logs. | - |
| -egi | Output Microsoft Edge installation logs. | ✅ |
| -egu | Output Microsoft Edge update logs. | - |
| -inet | Output internet option logs. | - |
| -env | Output Logon user info and enviroment logs. | - |
| -nx | Collecting net-export and psr logs. | - |
| -nsh | Collecting netsh and psr logs. | ✅ |
| -nw | Collecting netsh, net-export and psr logs. | ✅ |
| -psr | Collecting psr logs. | - |
| -f | Output all logs. | ✅ |

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
| -o &lt;path&gt; | Specify the output directory for log. | C:\logs |

For example:

```cmd
edge-logger.exe -o "C:\dist\output"
```