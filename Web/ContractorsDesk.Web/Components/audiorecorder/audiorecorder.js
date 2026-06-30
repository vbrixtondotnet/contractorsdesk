class AudioRecorder {
    constructor() {
        this.leftChannel = [];
        this.rightChannel = [];
        this.recorder = null;
        this.recording = false;
        this.recordingLength = 0;
        this.volume = null;
        this.audioInput = null;
        this.sampleRate = null;
        this.context = null;
        this.analyser = null;
        this.stream = null;
        this.tested = false;
        this.audioControl = null;
        this.file = null;
        this.init();
    }

    async init() {
        try {
            this.stream = await this.getStream();
            console.log('Got stream');
        } catch (err) {
            alert('Issue getting mic: ' + err.message);
        }
        this.setUpRecording();
    }

    async getStream(constraints = { audio: true, video: false }) {
        return navigator.mediaDevices.getUserMedia(constraints);
    }

    setUpRecording() {
        const AudioContext = window.AudioContext || window.webkitAudioContext;
        this.context = new AudioContext();
        this.sampleRate = this.context.sampleRate;

        this.volume = this.context.createGain();
        this.audioInput = this.context.createMediaStreamSource(this.stream);
        this.analyser = this.context.createAnalyser();

        this.audioInput.connect(this.analyser);

        const bufferSize = 2048;
        this.recorder = this.context.createScriptProcessor(bufferSize, 2, 2);

        this.analyser.connect(this.recorder);
        this.recorder.connect(this.context.destination);

        this.recorder.onaudioprocess = (e) => this.processAudio(e);
    }

    processAudio(e) {
        if (!this.recording) return;

        const left = e.inputBuffer.getChannelData(0);
        const right = e.inputBuffer.getChannelData(1);

        if (!this.tested) {
            this.tested = true;
            if (!left.reduce((a, b) => a + b)) {
                alert('There seems to be an issue with your Mic');
                this.stop();
                this.stream.getTracks().forEach(track => track.stop());
                this.context.close();
            }
        }

        this.leftChannel.push(new Float32Array(left));
        this.rightChannel.push(new Float32Array(right));
        this.recordingLength += left.length;
    }

    mergeBuffers(channelBuffer, recordingLength) {
        const result = new Float32Array(recordingLength);
        let offset = 0;
        channelBuffer.forEach(buffer => {
            result.set(buffer, offset);
            offset += buffer.length;
        });
        return result;
    }

    interleave(leftChannel, rightChannel) {
        const length = leftChannel.length + rightChannel.length;
        const result = new Float32Array(length);

        let inputIndex = 0;
        for (let index = 0; index < length;) {
            result[index++] = leftChannel[inputIndex];
            result[index++] = rightChannel[inputIndex];
            inputIndex++;
        }
        return result;
    }

    writeUTFBytes(view, offset, string) {
        for (let i = 0; i < string.length; i++) {
            view.setUint8(offset + i, string.charCodeAt(i));
        }
    }

    start() {
        this.recording = true;
        this.leftChannel.length = this.rightChannel.length = 0;
        this.recordingLength = 0;

        if (!this.context) this.setUpRecording();
    }

    stop() {
        this.recording = false;

        const leftBuffer = this.mergeBuffers(this.leftChannel, this.recordingLength);
        const rightBuffer = this.mergeBuffers(this.rightChannel, this.recordingLength);
        const interleaved = this.interleave(leftBuffer, rightBuffer);

        const buffer = new ArrayBuffer(44 + interleaved.length * 2);
        const view = new DataView(buffer);

        this.writeUTFBytes(view, 0, 'RIFF');
        view.setUint32(4, 44 + interleaved.length * 2, true);
        this.writeUTFBytes(view, 8, 'WAVE');
        this.writeUTFBytes(view, 12, 'fmt ');
        view.setUint32(16, 16, true);
        view.setUint16(20, 1, true);
        view.setUint16(22, 2, true);
        view.setUint32(24, this.sampleRate, true);
        view.setUint32(28, this.sampleRate * 4, true);
        view.setUint16(32, 4, true);
        view.setUint16(34, 16, true);
        this.writeUTFBytes(view, 36, 'data');
        view.setUint32(40, interleaved.length * 2, true);

        let index = 44;
        const volume = 1;
        interleaved.forEach(sample => {
            view.setInt16(index, sample * (0x7FFF * volume), true);
            index += 2;
        });

        const newid = () => {
            return 'xxxx-xxxx-4xxx-yxxx'.replace(/[xy]/g, function (char) {
                const random = Math.random() * 8 | 0;
                const value = char === 'x' ? random : (random & 0x3 | 0x8);
                return value.toString(8);
            });
        }
        const blob = new Blob([view], { type: 'audio/wav' });
        const audioUrl = URL.createObjectURL(blob);
        this.file = new File([blob], `${newid()}.wav`, { type: blob.type });
        this.audioControl.setAttribute('src', audioUrl);
    }

    pause() {
        this.recording = false;
        this.context.suspend();
    }

    resume() {
        this.recording = true;
        this.context.resume();
    }
}

//// Usage
//const recorder = new AudioRecorder();
//document.querySelector('#record').addEventListener('click', () => recorder.start());
//document.querySelector('#stop').addEventListener('click', () => recorder.stop());
//document.querySelector('#pause').addEventListener('click', () => recorder.pause());
//document.querySelector('#resume').addEventListener('click', () => recorder.resume());
