// Put variables in global scope to make them available to the browser console.
const constraints = window.constraints = {
    audio: false,
    video: true
};

export async function initialize(videoElement, dotnet) {

    console.log("initializing");
    try {
        let stream = await navigator.mediaDevices.getUserMedia(constraints);
        handleSuccess(stream, videoElement);
        dotnet.invokeMethodAsync("OnCameraStreaming");
    } catch (e) {
        handleError(e, dotnet);
    }
}

function handleSuccess(stream, video) {
    const videoTracks = stream.getVideoTracks();
    console.log('Got stream with constraints:', constraints);
    console.log(`Using video device: ${videoTracks[0].label}`);
    window.stream = stream; // make variable available to browser console
    window.vid = video;
    video.srcObject = stream;
    video.play();
}

function handleError(error, dotnet) {
    if (error.name === 'ConstraintNotSatisfiedError') {
        const v = constraints.video;
        errorMsg(`The resolution ${v.width.exact}x${v.height.exact} px is not supported by your device.`, error, dotnet);
    } else if (error.name === 'PermissionDeniedError') {
        errorMsg('Permissions have not been granted to use your camera and ' +
            'microphone, you need to allow the page access to your devices in ' +
            'order for the demo to work.', error, dotnet);
    }
    errorMsg(`getUserMedia error: ${error.name}`, error, dotnet);
}

function errorMsg(msg, error, dotnet) {
    if (typeof error !== 'undefined') {
        console.error(error);
    }
    dotnet.invokeMethodAsync("OnCameraStreamingError", msg);
}

export function getSnapshot(videoElement) {
    let video = videoElement;
    let canvas = document.createElement("canvas");
    let context = canvas.getContext('2d');
    canvas.setAttribute('width', videoElement.videoWidth);
    canvas.setAttribute('height', video.videoHeight);
    context.drawImage(videoElement, 0, 0, videoElement.videoWidth, video.videoHeight);
    let data = canvas.toDataURL('image/png');
    canvas.remove();
    return data;
}