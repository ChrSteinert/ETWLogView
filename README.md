# LogView

A tool to transiently view ETW log messages.

## Usage

There are two usage scenarios. Either listen to a set of given, named providers, or (try) to scan a .NET assembly for types inheriting `System.Diagnostics.Tracing.EventSource`, and listen to them.

## `--providers`

The `--providers` flag takes a list of names. Those named providers will be enables in the application and any messages occurring on the local machine from those providers will be shown on the console output.

### Example usage

```ps
LogView --providers EUCC.Generation.Text EUCC.Generation.Excel
```

## `--assembly`

This flag tries to reflect the given .NET assembly, looking for implementations of `System.Diagnostics.Tracing.EventSource`
> [!NOTE]
> Referenced assemblies are not taken into account!

## Example usage

```ps
LogView --assembly "/EUC Crawler/EUCC.Generation.Excel.dll"
```