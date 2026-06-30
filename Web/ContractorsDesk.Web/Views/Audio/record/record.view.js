class AudioRecordingView extends DomEventComponent {
    constructor() {
        super();
        this.timer = null;
        this.recorder = new AudioRecorder();
        this.AIWebhookUrl = null;
        this.uploadedAudio = null;
    }

    start(i) {
        $(i).addClass("d-none");
        $("#timer-display").removeClass('d-none');
        $("#btnEnd").removeClass("d-none");

        $(".mic-container .circle").addClass('active');
        $(".audio-controls").addClass('d-none');
        $("#btnSubmit").addClass("d-none");
        $("#btnReStart").addClass("d-none");
        $(".upload-status").addClass('d-none');

        this.timer.start();
        this.recorder.start();
    }

    end(i) {
        $(i).addClass("d-none");
        $("#timer-display").addClass('d-none');

        $(".audio-controls").removeClass('d-none');
        $(".mic-container .circle").removeClass('active');
        $("#btnSubmit").removeClass("d-none");
        $("#btnReStart").removeClass("d-none");
        this.timer.reset();
        this.recorder.stop();
    }

    #showUploadStatus(status) {
        $(".preloader").addClass('d-none');
        $("#btnReStart").addClass("d-none");
        $(".audio-controls").addClass('d-none');

        $(".progress-container").removeClass('d-none');
        $("#btnStart").removeClass("d-none");
        $(".upload-status").removeClass('d-none');

        if (status) {
            $(".upload-status-symbol").addClass('fa-check-circle').addClass('text-success');
            $(".upload-status-text").html('Upload Success!');
        }
        else {
            $(".upload-status-symbol").addClass('fa-times-circle').addClass('text-danger');
            $(".upload-status-text").html('Upload Failed! Please try again later.');
        }
    }

    async upload() {
        $(".preloader").removeClass('d-none');

        $(".progress-container").addClass('d-none');
        $("#btnSubmit").addClass("d-none");
        $(".upload-status").addClass('d-none');

        const file = this.recorder.file;
        const formData = new FormData();
        formData.append("audioFile", file);

        const response = await fetch("/api/upload/audio", {
            method: "POST",
            body: formData,
        });

        const result = await response.json();
        if (response.ok) {
            this.uploadedAudio = result.data;
        } else {
            this.#showUploadStatus(false);
        }
    }

    async onUpload() {
        await this.upload();
        if (this.uploadedAudio) {
            await this.#callAIWebhook();
            this.#showUploadStatus(true);
        }
    }

    async #callAIWebhook() {
        //if prod: https://n8n.lumberjack.so/webhook/apex;
       // debugger;
        const fileUrl = this.uploadedAudio.fileUrl;
        const ref = this.uploadedAudio.id;

        const response = await fetch(this.AIWebhookUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: [JSON.stringify({
                url: fileUrl,
                ref: ref,
                baseUrl: config.getBaseUrl()
            })],
        });

        const result = await response.json();
        if (response.ok) {
            console.log('AI Response', response.statusText);
        } else {
            console.log('AI API Error', response.statusText);
        }
    }

    init() {
        const timerDisplay = document.getElementById('timer-display');
        this.timer = new Timer(timerDisplay);
        this.recorder.audioControl = document.querySelector('#audio');
        const webHookUrl = document.getElementById("webHookUrl").value;
        this.AIWebhookUrl = webHookUrl;
        console.log('webHookUrl', webHookUrl);
    }
}

$(document).ready(async () => {
    const view = new AudioRecordingView();
    view.init();
});
