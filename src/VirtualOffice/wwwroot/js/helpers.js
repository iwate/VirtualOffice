export function debounce(handler, timeout) {
    let timeoutId = null;
    return function () {
        if (timeoutId === null) {
            timeoutId = setTimeout(() => {
                handler.apply(this);
                timeoutId = null;
            }, timeout);
        }
    };
}

export class SkywayFSM {
    constructor({ suffix, peer, roomId, localStream, mode, onClose, onData, onStream, onPeerLeave }) {
        this.peer = peer;
        this.suffix = suffix;
        this.localStream = localStream;
        this.mode = mode;
        this.onClose = onClose;
        this.onData = onData;
        this.onStream = onStream;
        this.onPeerLeave = onPeerLeave;
        this.status = 'ready';
        this.reset(roomId, localStream);
    }
    reset(roomId, localStream) {
        if (this.roomId !== roomId || this.localStream !== localStream) {
            this.roomId = roomId;
            this.localStream = localStream;
            this.next();
        }
    }
    next() {
        if (this.status === 'ready') {
            this.join();
            return;
        }
        if (this.status === 'open') {
            this.close();
            return;
        }
    }
    join() {
        this.room = this.peer.joinRoom(this.roomId + this.suffix, {
            mode: this.mode,
            stream: this.localStream,
        });
        this.room.once('open', () => {
            this.status = 'open';
        });
        this.room.once('close', () => {
            this.onClose();
            this.status = 'ready';
            this.next();
        });
        this.room.on('data', this.onData);
        this.room.on('stream', this.onStream);
        this.room.on('peerLeave', this.onPeerLeave);
        this.status === 'connecting';
    }
    close() {
        this.room.close();
    }
}

export function addStreamStopListener(stream, callback) {
    stream.addEventListener('ended', function () {
        callback();
        callback = function () { };
    }, false);
    stream.addEventListener('inactive', function () {
        callback();
        callback = function () { };
    }, false);
    stream.getTracks().forEach(function (track) {
        track.addEventListener('ended', function () {
            callback();
            callback = function () { };
        }, false);
        track.addEventListener('inactive', function () {
            callback();
            callback = function () { };
        }, false);
    });
}
