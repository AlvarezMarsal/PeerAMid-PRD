using System;
using System.Runtime.InteropServices;
using System.Threading;
using static PeerAMid.Utility.NativeMethods;

#nullable enable

namespace YardStickPortal;

public sealed class MessageFilter : IOleMessageFilter
{
    private IOleMessageFilter? _oldFilter;

    //
    // IOleMessageFilter functions.
    // Handle incoming thread requests.
    int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount,
        IntPtr lpInterfaceInfo) //Return the flag SERVERCALL_ISHANDLED.
    {
        return (int)SERVERCALL.SERVERCALL_ISHANDLED;
    }

    // Thread call was rejected, so try again.
    int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
    {
        if (dwRejectType == (int)SERVERCALL.SERVERCALL_RETRYLATER) return 99; // 1000; // 99;

        return -1;
    }

    int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType) //Return the flag PENDINGMSG_WAITDEFPROCESS.
    {
        return (int)PENDINGMSG.PENDINGMSG_WAITDEFPROCESS;
    }

    //
    // Class containing the IOleMessageFilter
    // thread error-handling functions.

    // Start the filter.
    public void Register()
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            IOleMessageFilter newFilter = this;
            CoRegisterMessageFilter(newFilter, out _oldFilter);
        }
        else
        {
            throw new COMException(
                "Unable to register message filter because the current thread apartment state is not STA.");
        }
    }

    // Done with the filter, close it.
    public void Revoke()
    {
        CoRegisterMessageFilter(_oldFilter!, out _);
    }
}
