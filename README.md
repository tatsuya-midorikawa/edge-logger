# edge-logger

This tool is designed to retrieve information related to Microsoft Edge.

## Getting Started

Specify command options for log information to be output.

| option | description |
| :-- | :-- |
| -w | Output Windows services logs. |
| -e | Output Edge policy logs. |
| -i | Output internet option logs. |
| -f | Output all logs. |

If you want to output all logs, do the following:

```cmd
edge-logger.exe -f
```

Also, if you want only Windows services and Edge logs to be output, specify as follows:
```cmd
edge-logger.exe -w -e
```

The output directory is C:\logs by default.
To change it, specify the following options:

| option | description |
| :-- | :-- |
| -d &lt;path&gt; | Specify the output directory for log. |

For example:

```cmd
edge-logger.exe -d "C:\dist\output"
```