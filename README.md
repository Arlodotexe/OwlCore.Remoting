# OwlCore.Remoting [![Version](https://img.shields.io/nuget/v/OwlCore.Remoting.svg)](https://www.nuget.org/packages/OwlCore.Remoting)

A lightweight and ultra-flexible RPC framework for .NET Standard 2.0, unidirectionally or bidirectionally.

When a member call is intercepted, we gather the information needed to replicate the unique member change, and emit an [IRemoteMemberMessage](src/Transfer/Messages/IRemoteMemberMessage.cs), which is handled by you.

By transferring this information out and applying the instructions on another machine in the same order, you can effectively control that instance remotely.

By serializing this information and replaying it later, you can effectively record and play back all the member changes.

## Example
### See the [Docs](./docs) for more.

```csharp
using OwlCore.Remoting;

// These attribute can be applied to a specific property/method, or an entire class.
// It even works when MemberRemote is in a base class!
[RemoteProperty]
[RemoteMethod]
[RemoteOptions(RemotingDirection.ClientToHost)]
public class MyClass : IDisposable
{
    private MemberRemote _memberRemote;

    public MemberRemote()
    {
        // Pass the instance you want to remote into MemberRemote() with an ID that is identical on both machines for that instance.
        // An instance will not receive member changes until you do this.
        // Optionally leave out the message handler. Uses the default set by MemberRemote.SetDefaultMessageHandler(handler);
        _memberRemote = new MemberRemote(this, "InstanceIdThatMatchesOnBothMachines", myMessageHandler);
    }

    // When the property setter is called, the value is captured and the property setter is invoked remotely.
    [RemoteProperty, RemoteOptions(RemotingDirection.Bidirectional)] 
    public int CurrentIndex { get; set; }

    // Method will be called remotely, including parameters.
    [RemoteMethod, RemoteOptions(RemotingDirection.InboundHost | RemotingDirection.Outbound)] 
    public void SomeMethod(int data, string[] moreData)
    {
        // code here ...
        // Execution may complete before other machines are done. 
        // If you need to wait for other machines to finish execution, use the _memberRemote.RemoteWaitAsync() and _memberRemote.RemoteReleaseAsync() extension methods.
    }

    public void Dispose()
    {
        // Dispose of the MemberRemote when finished. Forgetting to do this WILL result in a memory leak.
        _memberRemote.Dispose();
    }
}
```

## Install

Published releases are available on [NuGet](https://www.nuget.org/packages/OwlCore.Remoting). To install, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

    PM> Install-Package OwlCore.Remoting
    
Or using [dotnet](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet)

    > dotnet add package OwlCore.Remoting

## Financing

We accept donations, and we do not have any active bug bounties.

If you’re looking to contract a new project, new features, improvements or bug fixes, please contact me.

## Versioning

Version numbering follows the Semantic versioning approach. However, if the major version is `0`, the code is considered alpha and breaking changes may occur as a minor update.

## License

We’re using the MIT license for 3 reasons:
1. We're here to share useful code. You may use any part of it freely, as the MIT license allows. 
2. A library is no place for viral licensing.
3. Easy code transition to larger community-based projects, such as the .NET Community Toolkit.

