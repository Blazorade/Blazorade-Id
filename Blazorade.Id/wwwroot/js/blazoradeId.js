
export function openAuthorizationPopup(args) {

    console.debug("openAuthorizationPopup", args);

    const authorizeUrl = args.data.authorizeUrl;
    const width = 500;
    const height = 650;

    const dualScreenLeft = window.screenLeft ?? window.screenX;
    const dualScreenTop = window.screenTop ?? window.screenY;

    const viewportWidth = window.innerWidth
        || document.documentElement.clientWidth
        || screen.width;

    const viewportHeight = window.innerHeight
        || document.documentElement.clientHeight
        || screen.height;

    const left = dualScreenLeft + (viewportWidth - width) / 2;
    const top = dualScreenTop + (viewportHeight - height) / 2;

    const features =
        "toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes" +
        `,width=${width},height=${height},top=${top},left=${left}`;

    const popup = window.open(authorizeUrl, "authPopup", features);

    if (!popup) {
        if (args.failureCallback && args.failureCallback.target) {
            args.failureCallback.target.invokeMethodAsync(args.failureCallback.methodName, { error: "Popup could not be opened.", popupFailed: true });
        }
        return;
    }

    function onMessage(event) {
        console.debug("onAuthorizationPopupResponse", event);

        if (event.origin != window.location.origin) {
            return;
        }

        window.removeEventListener("message", onMessage);

        try {
            popup.close();
        }
        catch { }

        args.successCallback.target.invokeMethodAsync(args.successCallback.methodName, event.data.url);
    }

    window.addEventListener("message", onMessage);
}

export function signalAuthorizationPopupResponseUrl(args) {
    console.debug("signalAuthorizationPopupResponseUrl", args);

    try {
        if (window.opener && !window.opener.closed) {
            window.opener.postMessage({ url: args.data.responseUrl });
        }

        args.successCallback.target.invokeMethodAsync(args.successCallback.methodName, true);
    }
    catch (ex)
    {
        console.error(ex);
        args.failureCallback.target.invokeMethodAsync(args.failureCallback.methodName, ex);
    }
}
 