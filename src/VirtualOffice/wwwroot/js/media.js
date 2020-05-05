export async function getDevices() {
    const cameras = [];
    const microphones = [];
    const devices = await navigator.mediaDevices.enumerateDevices();
    for (let dev of devices) {
        switch (dev.kind) {
            case 'videoinput':
                cameras.push(dev);
                break;
            case 'audioinput':
                microphones.push(dev);
                break;
        }
    }
    return { cameras, microphones };
}

const onReplaceFaceStreamCallbacks = [];
export function onReplaceFaceStream(callback) {
    onReplaceFaceStreamCallbacks.push(callback);
}

let faceStream = new MediaStream();
export async function setMediaDevice(videoDeviceId, audioDeviceId) {
    const videoEnabled = faceStream.getVideoTracks().reduce((enabled, track) => enabled || track.enabled, false);
    const audioEnabled = faceStream.getAudioTracks().reduce((enabled, track) => enabled && track.enabled, true);
    const media = await navigator.mediaDevices.getUserMedia({
        video: videoDeviceId ? {
            deviceId: videoDeviceId,
            width: { min: 256, ideal: 256, max: 1024 },
            height: { min: 194, ideal: 194, max: 776 }
        } : videoDeviceId !== null,
        audio: audioDeviceId ? {
            deviceId: audioDeviceId
        } : audioDeviceId !== null
    }).catch(err => {
        console.error(err);
        alert(err);
    });

    if (media) {
        const videoTrack = media.getVideoTracks()[0];
        if (videoTrack) 
            videoTrack.enabled = videoEnabled;

        const audioTrack = media.getAudioTracks()[0];
        if (audioTrack) 
            audioTrack.enabled = audioEnabled;
    }

    faceStream = media || new MediaStream();
    onReplaceFaceStreamCallbacks.forEach(callback => callback(faceStream));
}

const onReplaceDesktopStreamCallbacks = [];
export function onReplaceDesktopStream(callback) {
    onReplaceDesktopStreamCallbacks.push(callback);
}

let desktopStream = null;
export function sharingDesktopScreen() {
    return desktopStream !== null;
};
export async function setDesktopMedia() {
    if (sharingDesktopScreen())
        return;

    const media = await navigator.mediaDevices.getDisplayMedia({
        audio: false,
        video: {
            width: { max: 1920 }
        }
    }).catch(err => {
        console.log(err);
        alert(err);
    });

    if (media) {
        const videoTrack = media.getVideoTracks()[0];
        if (videoTrack) {
            videoTrack.addEventListener('ended', () => {
                onReplaceDesktopStreamCallbacks.forEach(callback => callback(null));
                desktopStream = null;
            });
            videoTrack.addEventListener('inactive', () => {
                onReplaceDesktopStreamCallbacks.forEach(callback => callback(null));
                desktopStream = null;
            });
        }
    }

    desktopStream = media || null;
    onReplaceDesktopStreamCallbacks.forEach(callback => callback(desktopStream));
}

export function stopDesktopMedia() {
    if (desktopStream)
        desktopStream.getTracks(track => track.stop());
}

export async function toggleVideo(enabled) {
    faceStream.getVideoTracks().forEach(track => track.enabled = enabled);
}

export async function toggleAudio(enabled) {
    faceStream.getAudioTracks().forEach(track => track.enabled = enabled);
}