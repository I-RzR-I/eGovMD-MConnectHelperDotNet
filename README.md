> **Note** This repository is developed using .netstandard2.0, .netstandard2.1, net5.0

[![NuGet Version](https://img.shields.io/nuget/v/MConnectHelperDotNet.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/MConnectHelperDotNet/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/MConnectHelperDotNet.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/MConnectHelperDotNet)


One important reason for developing this repository is to quickly implement the governmental interoperability service connection provided by [e-governance agency](https://egov.md/), named `MConnect`, available in the Republic of Moldova.<br/>

<p align="center">
    <a href="https://mconnect.gov.md">
        <img src="assets/mconnect.png"/>
    </a>
</p>

<br/>

Proceed to the service portal where you can read more about them by clicking [here](https://mconnect.gov.md).

The current repository appears as a result of several implementations in projects from scratch, losing a lot of time and desire for a more easy way of implementation in new projects.
This repository is a wrapper for the currently available service. Using a few configuration parameters from the application settings file `appsettings.json`, you may implement them very easily into your own application.<br/>
Using the wrapper you will no longer be forced to install the application certificate on the current machine/server.
<br/>

Available configuration settings are: 
* `ServiceCertificate` -> Service/application certificate;
* `ClientCertificatePath` -> Client/application certificate path (file with *.pfx at the end);
* `ClientCertificatePassword` -> Client/application certificate password.

For more information about that, follow the info from using doc.

**In case you wish to use it in your project, u can install the package from <a href="https://www.nuget.org/packages/MConnectHelperDotNet" target="_blank">nuget.org</a>** or specify what version you want:

> `Install-Package MConnectHelperDotNet -Version x.x.x.x`

## Content
1. [USING](docs/usage.md)
1. [CHANGELOG](docs/CHANGELOG.md)
1. [BRANCH-GUIDE](docs/branch-guide.md)