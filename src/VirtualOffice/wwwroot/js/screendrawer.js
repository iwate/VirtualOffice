let socket = null;

let onopen = null;
export function onScreenDrawerOpen(callback) {
    onopen = callback;
}

let onclose = null;
export function onScreenDrawerClose(callback) {
    onclose = callback;
}

export function connectScreenDrawer() {
    if (socket === null || socket.readyState === 3) {
        socket = new WebSocket('wss://localhost:44190/draw');
        socket.onopen = () => {
            if (typeof onopen === 'function') onopen();
        };
        socket.onclose = () => {
            if (typeof onclose === 'function') onclose();
        };
    }
}

export function drawScreenDrawer(payload) {
    if (socket) {
        socket.send(payload);
    }
}