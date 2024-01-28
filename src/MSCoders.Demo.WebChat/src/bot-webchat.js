import { generateUUID } from "../utils/uuid.js";

// Ids
const idWebChat = process.env.WEB_CHAT_PREFIX + "webchat";
const idWebChatButton = idWebChat + "-button";
const idWebChatContainer = idWebChat + "-container";
const idWebChatHolder = idWebChat + "-holder";
const idWebChatHeader = idWebChat + "-header";
const idWebChatMinimizeButton = idWebChat + "-button-minimize";
const idWebChatRoot = idWebChat + "-root";

const startMaximizedBotWebChat = shouldStartMaximizedBotWebChat();

let dragBotWebChatElement;
let isBotWebChatInitiated = false;
let directLine;

function addScript(urlScript, async, defer) {
    const script = document.createElement("script");
    script.type = "text/javascript";
    script.setAttribute("src", urlScript);
    script.setAttribute("crossorigin", "anonymous");

    if (async) {
        script.setAttribute("async", "");
    }

    if (defer) {
        script.setAttribute("defer", "");
    }

    document.head.appendChild(script);
}

function getViewPort() {
    return {
        left: botWebChatSettings.boundaryPadding,
        top: botWebChatSettings.boundaryPadding,
        right:
            Math.max(
                document.documentElement.clientWidth || 0,
                window.innerWidth || 0
            ) - botWebChatSettings.boundaryPadding,
        bottom:
            Math.max(
                document.documentElement.clientHeight || 0,
                window.innerHeight || 0
            ) -
            botWebChatSettings.boundaryPadding -
            1,
    };
}

function detectDragOrClick() {
    if (!window.isDraggingBotWebChatElement) {
        initBotWebChat();
    }
}

function dragMoveBotWebChatElement(elementSource, elementTarget) {
    let initCursorPosX = 0;
    let initCursorPosY = 0;

    elementSource.onmouseenter = function (e) {
        e.preventDefault();
        elementSource.classList.add("drag");
    };

    elementSource.onmouseleave = function (e) {
        e.preventDefault();
        elementSource.classList.remove("drag");
        elementSource.classList.remove("dragging");
    };

    elementSource.onmousedown = function (e) {
        e.preventDefault();

        elementSource.classList.remove("drag");
        elementSource.classList.add("dragging");

        dragBotWebChatElement = elementTarget;

        initCursorPosX = e.clientX;
        initCursorPosY = e.clientY;
    };

    document.onmouseup = function (e) {
        if (dragBotWebChatElement) {
            elementSource.classList.add("drag");
            elementSource.classList.remove("dragging");
            dragBotWebChatElement = null;
            e.preventDefault();
            e.stopPropagation();
        }
    };

    document.onmousemove = function (e) {
        if (dragBotWebChatElement) {
            const viewport = getViewPort();
            const rect = dragBotWebChatElement.getBoundingClientRect();
            const offsetX = initCursorPosX - e.clientX;
            const offsetY = initCursorPosY - e.clientY;
            initCursorPosX = e.clientX;
            initCursorPosY = e.clientY;

            const newLeft = rect.left - offsetX;
            const newTop = rect.top - offsetY;

            if (
                newLeft >= viewport.left &&
                newTop >= viewport.top &&
                newLeft + rect.width <= viewport.right &&
                newTop + rect.height <= viewport.bottom
            ) {
                dragBotWebChatElement.style.left = newLeft + "px";
                dragBotWebChatElement.style.top = newTop + "px";
            }
        }
    };
}

async function initBotWebChat() {
    directLine = window.WebChat.createDirectLine({
        token: process.env.DIRECT_LINE_TOKEN,
        domain: process.env.DIRECT_LINE_DOMAIN,
    });

    if (!isBotWebChatInitiated) {

        // Customizations may required changing how some settings are initialized.
        // Using auxiliary variables, those customizations can be done here.
        const _userId = generateUUID();
        const _userName = "";
        const _locale = navigator.language || navigator.userLanguage;
        const _store = initBotWebChatStore();
        const _attachmentMiddleware = initBotWebChatAttachmentMiddleware(_store);

        // Additional information about the parameters of the `renderWebChat` method can be found here:
        // https://github.com/microsoft/BotFramework-WebChat/blob/master/docs/API.md#web-chat-api-reference
        //
        // Information about Direct Line domains can be found here:
        // https://learn.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-api-reference?view=azure-bot-service-4.0#base-uri
        window.WebChat.renderWebChat(
            {
                adaptiveCardsHostConfig: botWebChatSettings.adaptiveCardsHostConfig,
                directLine: directLine,
                attachmentMiddleware: _attachmentMiddleware,
                locale: _locale,
                store: _store,
                styleOptions: botWebChatSettings.styleOptions,
                userID: _userId,
                username: _userName,
            },
            document.getElementById(idWebChat)
        );

        isBotWebChatInitiated = true;

        document.querySelector(`#${idWebChat} > *`).focus();
    }

    showBotWebChat();
}

function initBotWebChatMinimizeButton() {
    const buttonMinimize = document.createElement("button");
    buttonMinimize.setAttribute("id", idWebChatMinimizeButton);
    buttonMinimize.setAttribute("onclick", "hideBotWebChat()");
    return buttonMinimize;
}

function initBotWebChatAttachmentMiddleware(store) {
    return () =>
        (next) =>
            ({ activity, attachment, ...others }) => {
                const { activities } = store.getState();
                const messageActivities = activities.filter((act) => act.type === "message");
                const recentBotMessage = messageActivities.pop() === activity;

                // If the attachment is an Adaptive Card, manage if it should be disabled or not.
                if (attachment.contentType === "application/vnd.microsoft.card.adaptive") {
                    // Add here any additional conditions to determine if the Adaptive Card should be disabled.
                    const shouldDisableAdaptiveCard = !recentBotMessage;

                    return Components.AdaptiveCardContent({
                        actionPerformedClassName: "card__action--performed", // Class name applied by the Bot Framework Webchat component when an adaptive card action is performed.
                        content: attachment.content,
                        disabled: shouldDisableAdaptiveCard,
                    });
                } else {
                    return next({ activity, attachment, ...others });
                }
            };
}

function initBotWebChatHeader() {
    const divHeader = document.createElement("div");
    divHeader.setAttribute("id", idWebChatHeader);
    divHeader.appendChild(initBotWebChatMinimizeButton());
    return divHeader;
}

function initBotWebChatButton() {
    const buttonWebChat = document.createElement("button");
    buttonWebChat.setAttribute("id", idWebChatButton);
    buttonWebChat.setAttribute("onclick", "initBotWebChat()");

    if (startMaximizedBotWebChat) {
        buttonWebChat.style.display = "none";
    }

    return buttonWebChat;
}

function initBotWebChatStore() {
    // Additional information about action types can be found here: https://codez.deedx.cz/posts/web-chat-events-summary/

    return window.WebChat.createStore({}, ({ dispatch }) => (next) => (action) => {
        // Sometimes, values from local storage are required as soon as a connection is fulfilled with the bot service.
        if (action.type === "DIRECT_LINE/CONNECT_FULFILLED") {
            dispatch({
                type: "WEB_CHAT/SEND_EVENT",
                payload: {
                    name: "webchat/greetings",
                },
            });
        }

        return next(action);
    });
}

function initBotWebChatWindow() {
    const divWebchat = document.createElement("div");
    divWebchat.setAttribute("id", idWebChat);
    divWebchat.setAttribute("role", "main"); // This is mandatory for the Bot Framework Webchat component to work properly.

    return divWebchat;
}

function preventBotWebChatElementOutOfScreen(element) {
    if (element) {
        const viewport = getViewPort();
        let rect = element.getBoundingClientRect();

        /* Resize the bot's webchat if it gets out of screen, because it is larger than the new window's size. */

        if (rect.width > viewport.right) {
            element.style.width = `${viewport.right - botWebChatSettings.boundaryPadding}px`;
            rect = element.getBoundingClientRect(); // Get the bot's webchat dimensions again after resizing the window, because they've changed.
        }

        if (rect.height > viewport.bottom) {
            element.style.height = `${viewport.bottom - botWebChatSettings.boundaryPadding}px`;
            rect = element.getBoundingClientRect(); // Get the bot's webchat dimensions again after resizing the window, because they've changed.
        }

        /* Reposition the bot's web chat window if it gets out of screen. */

        if (rect.left < viewport.left) {
            element.style.left = `${viewport.left}px`;
        }

        if (rect.top < viewport.top) {
            element.style.top = `${viewport.top}px`;
        }

        if (rect.right > viewport.right) {
            element.style.left = `${viewport.right - rect.width}px`;
        }

        if (rect.bottom > viewport.bottom) {
            element.style.top = `${viewport.bottom - rect.height}px`;
        }
    }
}

function renderBotWebChat() {
    addScript("https://unpkg.com/simple-update-in/dist/simple-update-in.production.min.js", true, false); // Library used to easily send payloads to the bot service.
    addScript("https://cdn.botframework.com/botframework-webchat/latest/webchat.js", true, true);

    const header = initBotWebChatHeader();
    const button = initBotWebChatButton();

    const holder = document.createElement("div");
    holder.setAttribute("id", idWebChatHolder);
    holder.appendChild(header);
    holder.appendChild(initBotWebChatWindow());
    holder.tabIndex = -1;

    const container = document.createElement("div");
    container.setAttribute("id", idWebChatContainer);
    container.appendChild(holder);
    container.appendChild(button);
    container.tabIndex = -1;

    const root = document.getElementById(idWebChatRoot);
    root.appendChild(container);

    // Is the bot's web chat draggable?
    if (botWebChatSettings.dragBotWebChat) {
        dragMoveBotWebChatElement(header, container);
    }

    // Is the bot's web chat button draggable?
    button.addEventListener("mouseenter", function () {
        if (botWebChatSettings.dragBotWebChatButton) {
            button.setAttribute("onclick", "detectDragOrClick()");
            dragMoveBotWebChatElement(button, container);
        }
    });

    // Is the bot's web chat maximized at startup?
    if (startMaximizedBotWebChat) {
        holder.style.display = "block";
    }

    // Can the bot's web chat be resized?
    if (botWebChatSettings.resizeBotWebChat) {
        renderBotWebChatResizers(container);
    }
}

function renderBotWebChatResizers(container) {
    function initResizer(sideClass) {
        const resizer = document.createElement("div");
        resizer.classList.add(sideClass);
        resizer.addEventListener("mousedown", handleResize);
        return resizer;
    }

    /* Add sides */

    container.appendChild(initResizer("resizer--n"));
    container.appendChild(initResizer("resizer--s"));
    container.appendChild(initResizer("resizer--w"));
    container.appendChild(initResizer("resizer--e"));

    /* Add corners */

    container.appendChild(initResizer("resizer--nw"));
    container.appendChild(initResizer("resizer--ne"));
    container.appendChild(initResizer("resizer--sw"));
    container.appendChild(initResizer("resizer--se"));

    /* Event listeners */

    // Get the current size of the container, and use it as the minimum size for resizing,
    // thus honoring the CSS properties.
    const minimumHeight = container.offsetHeight;
    const minimumWidth = container.offsetWidth;

    let originalHeight;
    let originalWidth;
    let originalPosX = 0;
    let originalPosY = 0;
    let originalCursorPosX = 0;
    let originalCursorPosY = 0;

    function handleResize(e) {
        e.preventDefault();

        const currentResizer = e.target;

        const rect = container.getBoundingClientRect();
        originalWidth = container.offsetWidth;
        originalHeight = container.offsetHeight;
        originalPosX = rect.left;
        originalPosY = rect.top;
        originalCursorPosX = e.pageX;
        originalCursorPosY = e.pageY;

        document.addEventListener("mousemove", resize);

        /* Stop resizing if the mouse leaves the resizer element */
        document.addEventListener("mouseup", stopResize);
        document.addEventListener("mouseleave", stopResize);

        function resize(e) {
            const viewport = getViewPort();

            function resizeSouth(e) {
                const height = originalHeight + (e.pageY - originalCursorPosY);
                if (height > minimumHeight && height < viewport.bottom - originalPosY) {
                    container.style.height = height + "px";
                }
            }

            function resizeNorth(e) {
                const height =
                    originalHeight - (e.clientY - originalCursorPosY);
                if (height > minimumHeight && height < originalPosY + originalHeight - viewport.top) {
                    container.style.height = height + "px";
                    container.style.top = originalPosY + (e.pageY - originalCursorPosY) + "px";
                }
            }

            function resizeEast(e) {
                const width = originalWidth + (e.pageX - originalCursorPosX);
                if (width > minimumWidth && width <= viewport.right - originalPosX) {
                    container.style.width = width + "px";
                }
            }

            function resizeWest(e) {
                const width = originalWidth - (e.pageX - originalCursorPosX);
                if (width > minimumWidth && width <= originalPosX + originalWidth - viewport.left) {
                    container.style.width = width + "px";
                    container.style.left = originalPosX + (e.pageX - originalCursorPosX) + "px";
                }
            }

            if (currentResizer.classList.contains("resizer--s")) {
                resizeSouth(e);
            }

            if (currentResizer.classList.contains("resizer--n")) {
                resizeNorth(e);
            }

            if (currentResizer.classList.contains("resizer--e")) {
                resizeEast(e);
            }

            if (currentResizer.classList.contains("resizer--w")) {
                resizeWest(e);
            }

            if (currentResizer.classList.contains("resizer--se")) {
                resizeSouth(e);
                resizeEast(e);
            }

            if (currentResizer.classList.contains("resizer--sw")) {
                resizeSouth(e);
                resizeWest(e);
            }

            if (currentResizer.classList.contains("resizer--ne")) {
                resizeNorth(e);
                resizeEast(e);
            }

            if (currentResizer.classList.contains("resizer--nw")) {
                resizeNorth(e);
                resizeWest(e);
            }
        }

        function stopResize() {
            document.removeEventListener("mousemove", resize);
        }
    }
}

function hideBotWebChat() {
    document.getElementById(idWebChatButton).style.display = "block";
    document.getElementById(idWebChatHolder).style.display = "none";    
}

function showBotWebChat() {
    document.getElementById(idWebChatButton).style.display = "none";
    document.getElementById(idWebChatHolder).style.display = "block";
}

function shouldStartMaximizedBotWebChat() {
    return /Android|webOS|iPhone|iPad/i.test(navigator.userAgent) && window.innerWidth <= botWebChatSettings.startMaximizedMobileMinWidth
        ? botWebChatSettings.startMaximizedMobile
        : botWebChatSettings.startMaximized;
}

(function () {
    renderBotWebChat();

    window.isDraggingBotWebChatElement = false;

    document.addEventListener("mousedown", () => (window.isDraggingBotWebChatElement = false));
    document.addEventListener("mousemove", () => (window.isDraggingBotWebChatElement = true));
})();

window.detectDragOrClick = detectDragOrClick;
window.hideBotWebChat = hideBotWebChat;
window.initBotWebChat = initBotWebChat;

window.addEventListener("load", function () {
    if (startMaximizedBotWebChat) {
        initBotWebChat();
    }
});

window.addEventListener("resize", (event) => {
    preventBotWebChatElementOutOfScreen(
        document.getElementById(idWebChatContainer)
    );
    preventBotWebChatElementOutOfScreen(
        document.getElementById(idWebChatButton)
    );
});
