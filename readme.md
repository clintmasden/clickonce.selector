
# clickonce.selector
A .NET Standard/C# utility for selecting and executing a published project.

#### Setup
- This utility is useful for executables only
- Turn off the option **Use ".deploy" file extension**


#### Getting Started:

 - Compile clickonce.selector.exe
 - Modify the appsettings.json file to point to your publish directory
 - Modify the appsettings.json file to point to your stable release version
 -- pass argument **b** to execute the latest version
-- otherwise it will execute the version specified

```
{
  "appSettings": {
    "publishDirectory": "C:\\deployments\\program\\bleeding",
    "stableVersion": "1.0.0.1"
  }
}
```
