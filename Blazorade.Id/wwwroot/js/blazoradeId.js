
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

    function invokeFailure(payload) {
        console.debug("invokeFailure", payload);

        if (args.failureCallback && args.failureCallback.target) {
            args.failureCallback.target.invokeMethodAsync(
                args.failureCallback.methodName,
                payload
            );
        }
    }

    const popup = window.open(authorizeUrl, "authPopup", features);
    //const popup = null;

    if (!popup) {
        invokeFailure({
            error: "Popup could not be opened.",
            reason: 1 // Enum value used in C# to indicate popup could not be opened.
        });
        return;
    }

    let completed = false;
    let closePollTimer = null;

    function cleanup() {
        console.debug("cleanup");

        window.removeEventListener("message", onMessage);
        if (closePollTimer) {
            clearInterval(closePollTimer);
            closePollTimer = null;
        }
    }

    function completeSuccess(url) {
        console.debug("completeSuccess", url);

        if (completed) return;
        completed = true;
        cleanup();

        try { popup.close(); } catch { }

        args.successCallback.target.invokeMethodAsync(args.successCallback.methodName, url);
    }

    function completeFailure(payload) {
        console.debug("completeFailure", payload);

        if (completed) return;
        completed = true;
        cleanup();

        try { popup.close(); } catch { }

        invokeFailure(payload);
    }

    function onMessage(event) {
        console.debug("onMessage", event);

        // Keep your existing origin check
        if (event.origin != window.location.origin) {
            return;
        }

        // Defensive: ensure expected shape
        const url = event?.data?.url;
        if (!url) {
            // If you prefer, treat this as failure instead of ignore
            return;
        }

        completeSuccess(url);
    }

    window.addEventListener("message", onMessage);

    // Detect user closing the popup before we receive a response
    closePollTimer = setInterval(() => {
        // If the popup is gone or closed and we haven't completed, treat as failure
        if (!popup || popup.closed) {
            completeFailure({
                error: "Popup was closed before authorization completed.",
                reason: 2 // Enum value used in C# to indicate user closed popup.
            });
        }
    }, 250);
}

export function openAuthorizationIframe(args) {
    console.debug("openAuthorizationIframe", args);

    const authorizeUrl = args.data.authorizeUrl;

    // Reason code 3 must align with the server-side AuthorizationCodeFailureReason value used for iframe timeouts.
    // It is sent when the iframe does not complete within the configured timeout.
    const timeoutMs = 1000;

    function invokeFailure(payload) {
        console.debug("invokeFailure", payload);

        if (args.failureCallback && args.failureCallback.target) {
            args.failureCallback.target.invokeMethodAsync(
                args.failureCallback.methodName,
                payload
            );
        }
    }

    let completed = false;
    let timer = null;
    let iframe = null;

    function cleanup() {
        console.debug("cleanup (iframe)");

        window.removeEventListener("message", onMessage);
        if (timer) {
            clearTimeout(timer);
            timer = null;
        }
        try {
            if (iframe) {
                iframe.remove();
                iframe = null;
            }
        } catch { }
    }

    function completeSuccess(url) {
        console.debug("completeSuccess (iframe)", url);

        if (completed) return;
        completed = true;
        cleanup();

        args.successCallback.target.invokeMethodAsync(args.successCallback.methodName, url);
    }

    function completeFailure(payload) {
        console.debug("completeFailure (iframe)", payload);

        if (completed) return;
        completed = true;
        cleanup();

        invokeFailure(payload);
    }

    function onMessage(event) {
        console.debug("onMessage (iframe)", event);

        // Same origin check as popup version
        if (event.origin !== window.location.origin) {
            return;
        }

        const url = event?.data?.url;
        if (!url) {
            return;
        }

        completeSuccess(url);
    }

    window.addEventListener("message", onMessage);

    timer = setTimeout(() => {
        completeFailure({
            error: "Iframe authorization timed out.",
            reason: 3
        });
    }, timeoutMs);

    iframe = document.createElement("iframe");
    iframe.style.width = "0";
    iframe.style.height = "0";
    iframe.style.border = "0";
    iframe.style.position = "absolute";
    iframe.style.left = "-9999px";
    iframe.style.top = "-9999px";
    iframe.setAttribute("aria-hidden", "true");
    iframe.tabIndex = -1;

    document.body.appendChild(iframe);

    // Navigate after append for reliability
    iframe.src = authorizeUrl;
}

export function signalAuthorizationResponse(args) {
    console.debug("signalAuthorizationResponse", args);

    try {
        const payload = { url: args.data.responseUrl };

        if (window.opener && !window.opener.closed) {
            // Popup flow
            window.opener.postMessage(payload);
        }
        else if (window.parent && window.parent !== window) {
            // Iframe flow
            window.parent.postMessage(payload);
        }

        args.successCallback.target.invokeMethodAsync(args.successCallback.methodName, true);
    }
    catch (ex) {
        console.error(ex);
        args.failureCallback.target.invokeMethodAsync(args.failureCallback.methodName, ex);
    }
}
