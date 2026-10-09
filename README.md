# TaPi — Tab Picker

**TaPi (Tab Picker)** is a lightweight Windows utility that helps you focus an already-open browser tab or open a new one if it isn't present. It supports **Brave, with Google Chrome as a fallback**.

Instead of opening duplicate tabs, TaPi searches existing tabs **by title** and activates the matching one.

## Features

* **Tab selection:** Find and activate tabs using partial title matching.
* **Brave-first detection:** Prioritizes Brave and falls back to Chrome if Brave isn't running.
* **Fallback URL:** Opens a specified URL when no matching tab is found.
* **Case-insensitive search:** Matches tab titles regardless of capitalization.
* **Windows UI Automation:** Uses native Windows accessibility APIs to interact with browser tabs.
* **Execution timing:** Reports the time taken to activate a matching tab.

## Requirements

* Windows 10 or Windows 11.
* Brave or Google Chrome.
* The .NET SDK to build from source.
* A self-contained published build can run without a separate .NET runtime installation.

## Installation

<!-- ### Option 1: Use a prebuilt release

1. Download the TaPi release for Windows.
2. Extract the release files into a folder of your choice.
3. Keep the executable and any accompanying published files together.
4. Run `TaPi.exe` from Command Prompt, PowerShell, or a compatible application launcher. -->

<!-- ### Option 2: Build from source -->
### Build from source

1. Install the [.NET SDK](https://dotnet.microsoft.com/download).

2. Clone or download the source code.

3. Open a terminal in the directory containing `TaPi.csproj`.

4. Run:

   ```bat
   dotnet publish TaPi.csproj -c Release -r win-x64 --self-contained true -o "publish"
   ```

5. Run `TaPi.exe` from the generated `publish` directory.

This produces a Release build for 64-bit Windows and includes the .NET runtime.

## Usage

TaPi accepts a tab title and an optional fallback URL:

```text
TaPi.exe <tab-title> [fallback-url]
```

| Argument       | Required | Description                                        |
| -------------- | -------- | -------------------------------------------------- |
| `tab-title`    | Yes      | Text to search for in existing browser tab titles. |
| `fallback-url` | No       | URL to open if no matching tab is found.           |

### Examples

**Activate an existing tab**

```bat
TaPi.exe "YouTube"
```

**Search using a partial title**

```bat
TaPi.exe "GitHub"
```

This can match a title such as `GitHub - Pull requests`.

**Open a URL if no matching tab exists**

```bat
TaPi.exe "YouTube" "https://www.youtube.com"
```

**Search for a page with a multiword title**

```bat
TaPi.exe "Google Docs" "https://docs.google.com"
```

Enclose arguments containing spaces in double quotes.

## How it works

1. Checks whether Brave is running; if not, checks for Chrome.
2. Uses Windows UI Automation to locate browser tabs.
3. Searches for a case-insensitive partial match against each tab's title.
4. Identifies the containing browser window and verifies its process.
5. Selects the matching tab and brings its window to the foreground.
6. Opens the fallback URL if no matching tab is found and a URL was provided.

**Search order:** When multiple tabs match, TaPi focuses the first match encountered during its search, which in testing corresponds to the most recently accessed matching tab.

**Browser priority:** If Brave is running, TaPi searches Brave only. It does not search Chrome if the requested tab is missing from Brave.

## Launcher integration

TaPi can be configured as a command in compatible launchers, including PowerToys Run or Kando.

Example:

```bat
"path\to\TaPi.exe" "YouTube" "https://www.youtube.com"
```

Replace the executable path, tab title, and fallback URL with your own values.

## Troubleshooting

**The application reports that `TaPi.dll` cannot be found**

Ensure the complete published output is available. Copying only the executable may omit required files.

**A matching tab is not activated**

* Verify that the browser is running.
* Try a shorter or more distinctive part of the tab title.
* Check whether the tab is open in the browser TaPi prioritizes.

**The fallback URL does not open**

Verify that the URL is valid and that Windows has a default browser configured.