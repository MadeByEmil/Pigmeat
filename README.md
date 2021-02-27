# WDHAN

[![WDHAN Branding](https://github.com/MadeByEmil/WDHAN/raw/master/branding/logo.png)](https://github.com/MadeByEmil/WDHAN)

[![.NET Core (Linux)](https://github.com/MadeByEmil/WDHAN/workflows/.NET%20Core%20(Linux)/badge.svg)](https://github.com/MadeByEmil/WDHAN/actions?query=workflow%3A%22.NET+Core+%28Linux%29%22)
[![.NET Core (deb)](https://github.com/MadeByEmil/WDHAN/workflows/.NET%20Core%20(deb)/badge.svg)](https://github.com/MadeByEmil/WDHAN/actions?query=workflow%3A%22.NET+Core+%28deb%29%22)
[![.NET Core (rpm)](https://github.com/MadeByEmil/WDHAN/workflows/.NET%20Core%20(rpm)/badge.svg)](https://github.com/MadeByEmil/WDHAN/actions?query=workflow%3A%22.NET+Core+%28rpm%29%22)

[![.NET Core (Windows)](https://github.com/MadeByEmil/WDHAN/workflows/.NET%20Core%20(Windows)/badge.svg)](https://github.com/MadeByEmil/WDHAN/actions?query=workflow%3A%22.NET+Core+%28Windows%29%22)
[![.NET Core (macOS)](https://github.com/MadeByEmil/WDHAN/workflows/.NET%20Core%20(macOS)/badge.svg)](https://github.com/MadeByEmil/WDHAN/actions?query=workflow%3A%22.NET+Core+%28macOS%29%22)

WDHAN is a static content publishing tool for the modern web. It takes data in the form of JSON or YAML, plugs it into a Liquid context, wraps it up in some HTML & CSS, and spits out a website that can be hosted online. If you don't know what any of that means, it's OK. Point is, it's an easier way to publish on the internet, regardless of your skill level. WDHAN is for corporations, organizations, friends, peers, bloggers, hobbyists, and everyone under the sun.

## WDHAN will get you online
 WDHAN does *exactly* what it's told. It doesn't have any tricks to learn or exceptions to the rules. It just works.
 With WDHAN, there's no more digging through cluttered bits of reused HTML, or updating old databases with strange syntax.
 You can focus on the only thing that matters to you: being online.

## Getting started
 * Download WDHAN
   * Download the latest build artifact from our [commits](https://github.com/MadeByEmil/WDHAN/commits/master) page that corresponds to your operating system.
 * Read up on the [documentation](https://MadeByEmil.github.io/WDHAN)
 * Take a look at [other people's WDHAN projects](https://github.com/topics/wdhan)

## Need help?
 If you're in need of assistance, you can reach code contributors on our GitHub repository.
 Check our [issues](https://github.com/MadeByEmil/WDHAN/issues) page to see if your problem has already been resolved. If not, [raise a new issue](https://github.com/MadeByEmil/WDHAN/issues/new/choose).
 
 [![Maintained code documentation](https://github.com/MadeByEmil/WDHAN/workflows/Maintained%20code%20documentation/badge.svg)](https://MadeByEmil.github.io/WDHAN)

## Building from source
  Pre-requisites:
  - [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

  #### Windows
  ```
  dotnet build --configuration Release --runtime win-x64
  ```
  Outputs to ```.\bin\Release\netcoreapp3.1```

  #### macOS
  ```
  dotnet build --configuration Release --runtime osx-x64
  ```
  Outputs to ```./bin/Release/netcoreapp3.1```

  #### Linux
  ```
  dotnet build --configuration Release --runtime linux-x64
  ```
  Outputs to ```./bin/Release/netcoreapp3.1```

## Forking
 This repository contains the code for both `WDHAN.Core`, a static media publishing library, and `WDHAN`, its reference implementation.
 Before forking, please try and see if you can accomplish your goals with a plugin. The `WDHAN` tool will always remain fully compatible with the `WDHAN.Core` library, and thus using it will be far more maintainable from both code and production standpoints.
 If you plan on forking this project, it is suggested you do so by building another tool, and not by forking the `WDHAN.Core` library as well. Otherwise, you risk falling behind in code quality, project performance, and security.

## Credits
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/c0403d9ba4494e7c820394cf9bafa917)](https://app.codacy.com/gh/MadeByEmil/WDHAN?utm_source=github.com&utm_medium=referral&utm_content=MadeByEmil/WDHAN&utm_campaign=Badge_Grade_Dashboard)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FMadeByEmil%2FWDHAN.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FMadeByEmil%2FWDHAN?ref=badge_shield)

[![CodeQL](https://github.com/MadeByEmil/WDHAN/workflows/CodeQL/badge.svg)](https://securitylab.github.com/tools/codeql)
[![OSSAR](https://github.com/MadeByEmil/WDHAN/workflows/OSSAR/badge.svg)](https://aka.ms/mscadocs)

 Authored by Emil Sayahi ([@MadeByEmil](https://github.com/MadeByEmil))

 View all [contributors](https://github.com/MadeByEmil/WDHAN/graphs/contributors)

---

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FMadeByEmil%2FWDHAN.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FMadeByEmil%2FWDHAN?ref=badge_large)

---
Copyright (C) 2020 Emil Sayahi

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3.


This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.


You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
